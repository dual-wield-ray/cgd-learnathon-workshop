using System.Collections.Generic;
using System.Text;
using M2MqttUnity;
using MQTTnet;
using MQTTnet.Client;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt.Messages;
using Newtonsoft.Json;
using MQTTnet.Extensions;

public class MQTTClient : UnityMQTTClient
{
    [SerializeField] private StringEvent messageReceived;

    private string _apiTopic;

    protected void Awake()
    {
        _apiTopic = $"learnathon/{Application.productName}/api";
    }

    protected override void SubscribeTopics()
    {
        Debug.Log($"Subscribing to topic: {_apiTopic}");
        
        Client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(_apiTopic).Build());
    }

    protected override void UnsubscribeTopics()
    {
        Client.UnsubscribeAsync(_apiTopic);
    }
    
    protected override void OnMessage(string message)
    {
        // Sanitize string sent by companion app
        message = message.Replace("\\", "");
        message = message.Replace("\"{", "{");
        message = message.Replace("}\"", "}");
        
        Debug.Log("Received message: " + message);
        var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);
        
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

    public void Publish(string topic, string message)
    {
        Debug.Log($"Publishing message: {message} + to topic: {topic}");
        PublishMessage(topic, JsonConvert.SerializeObject(message), 2);
    }
}