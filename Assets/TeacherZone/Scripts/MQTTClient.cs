using MQTTnet;
using MQTTnet.Client;
using UnityEngine;

public class MQTTClient : WebSocketMQTTClient
{
    [SerializeField] private StringEvent messageReceived;

    private string _apiTopic;
    
    private const string DEFAULT_GAME_NAME = "your-game-name-here";

    protected void Awake()
    {
        _apiTopic = $"learnathon/{Application.productName}/api";

        if (Application.productName == DEFAULT_GAME_NAME)
        {
            Debug.LogWarning("Make sure to set your product name to the name of your game." +
                             "\nThe setting is found in top left corner of the window in Edit\\ProjectSettings\\Player." +
                             "\nThis will be used to allow others to send messages to your game specifically.");
        }
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

    protected override void OnConnect()
    {
        Debug.Log($"Connected to broker at address: {brokerAddress}");
        base.OnConnect();
    }

    protected override void OnDisconnect()
    {
        Debug.Log($"Disconnected from broker at address: {brokerAddress}");
        base.OnDisconnect();
    }

    protected override void OnMessage(string message)
    {
        Debug.Log("Received payload: " + message);
        
        // Extract message value
        // Note: this is done in a terribly hard-coded way because
        //       JSON serialization libraries (Newtonsoft etc.) depend on Reflection,
        //       and this is not allowed in a WebGL (AOT) context
        //       So for simplicity, given that we know our message format is very basic, just parse like this
        message = message.Replace("\\", "");
        message = message.Replace("\"{", "{");
        message = message.Replace("}\"", "}");
        message = message.Trim('{', '}', ' ', '\n');
        message = message.Remove(0, "\"msg:\" ".Length);
        message = message.Trim('\"');
        
        Debug.Log("Raising message: " + message);
        messageReceived.Raise(message);
    }
}