using System.Collections.Generic;
using M2MqttUnity;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt.Messages;
using Newtonsoft.Json;

public class MQTTClient : M2MqttUnityClient
{
    [SerializeField] private StringEvent messageReceived;

    private string apiTopic;

    protected override void Awake()
    {
        apiTopic = $"learnathon/{Application.productName}/api";
    }

    protected override void SubscribeTopics()
    {
        Debug.Log($"Subscribing to topic: {apiTopic}");
        
        client.Subscribe(new[] { apiTopic }, new[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
    }

    protected override void UnsubscribeTopics()
    {
        client.Unsubscribe(new[] { apiTopic });
    }
    
    protected override void DecodeMessage(string topic, byte[] message)
    {
        string msg = System.Text.Encoding.UTF8.GetString(message);
        
        Debug.Log("Received message: " + msg);
        var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(msg);
        
        string payload = dict["msg"];
        if (!dict.ContainsKey("msg"))
        {
            Debug.LogError("Message was received, but did not contain the 'msg' key. " +
                           "Make sure what was sent is of the form:" +
                           "{\n" +
                           "    \"msg\": \"YOUR_MESSAGE\"\n" +
                           "}");
            return;
        }
        
        messageReceived.Raise(payload);
    }
}
