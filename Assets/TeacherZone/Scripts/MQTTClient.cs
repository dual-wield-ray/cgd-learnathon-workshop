using MQTTnet;
using MQTTnet.Client;
using UnityEngine;

public class MQTTClient : WebSocketMQTTClient
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

        message = message.Trim('{', '}', ' ', '\n');
        message = message.Remove(0, "\"msg:\" ".Length);
        message = message.Trim('\"');
        
        // if (!dict.ContainsKey("msg"))
        // {
        //     Debug.LogError("Message was received, but did not contain the 'msg' key. " +
        //                    "Make sure what was sent is of the form:" +
        //                    "{\n" +
        //                    "    \"msg\": \"YOUR_MESSAGE\"\n" +
        //                    "}");
        //     return;
        // }
        
        Debug.Log("Raising message: " + message);
        messageReceived.Raise(message);
    }
}