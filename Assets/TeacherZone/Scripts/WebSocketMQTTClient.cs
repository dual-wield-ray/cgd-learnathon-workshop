using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using UnityEngine;

public class WebSocketMQTTClient : MonoBehaviour
{
    public string brokerAddress = "127.0.0.1";

    protected IMqttClient Client;
    protected IMqttClientOptions ClientOptions;
    
    protected Queue<string> messageQueue = new();

    private void Start()
    {
        var factory = new MqttFactory();
        Client = factory.CreateMqttClient();

        ClientOptions = new MqttClientOptionsBuilder()
            .WithClientId(Guid.NewGuid().ToString())
            .WithWebSocketServer(brokerAddress)
            .WithCleanSession()
            .WithKeepAlivePeriod(new TimeSpan(0, 1, 0))
            .Build();
        
        Client.ConnectAsync(ClientOptions);
        
        Client.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(e =>
        {
            messageQueue.Enqueue(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
        });

        Client.ConnectedHandler = new MqttClientConnectedHandlerDelegate(e =>
        {
            OnConnect();
            SubscribeTopics();
            return Task.CompletedTask;
        });

        Client.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(e =>
        {
            UnsubscribeTopics();
            OnDisconnect();
            return Task.CompletedTask;
        });
    }

    private void Update()
    {
        while (messageQueue.Count > 0)
        {
            var msg = messageQueue.Dequeue();
            OnMessage(msg);
        }
    }

    public void PublishMessage(string message, string topic, int qos)
    {
        var applicationMessage = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(message);
        if (qos == 0)
        {
            applicationMessage = applicationMessage.WithAtMostOnceQoS();
        }
        else if (qos == 1)
        {
            applicationMessage = applicationMessage.WithAtLeastOnceQoS();
        }
        else if (qos == 2)
        {
            applicationMessage = applicationMessage.WithExactlyOnceQoS();
        }
        
        Client.PublishAsync(applicationMessage.Build());
    }

    protected virtual void OnConnect()
    {
        
    }
    
    protected virtual void OnDisconnect()
    {
        
    }
    
    protected virtual void OnMessage(string message)
    {
        
    }

    protected virtual void SubscribeTopics()
    {
        
    }
    
    protected virtual void UnsubscribeTopics()
    {
        
    }
}
