using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Protocol;
using System;
using System.Text;
using System.Threading.Tasks;

public class MyEvent : UnityEvent<string>
{
}


public class MQTT : MonoBehaviour
{
    private static IMqttClient client;
    private static IMqttClientOptions clientOptions;
    public bool isConnected = false;
    public String broker = ""; 
    public int brokerport = 8883; 
    public String clientid = "";
    public String username = ""; 
    public String password = "";

    //Define events
    public delegate void ReceivedMessage(string receivedtext);
    public static event ReceivedMessage OnMessage;
    public delegate void SentMessage(string receivedtext, string topic, int qos);
    public static event SentMessage OnSentMessage;
    public delegate void Connected();
    public static event Connected OnConnect;
    public delegate void Disconnected();
    public static event Disconnected OnDisconnect;

    // Define event
    public MyEvent TestEvent;


    private void Awake()
    {
        clientid = Guid.NewGuid().ToString();
        
        connect();
    }

    //Disconnect from server
    public void disconnect()
    {
        client.DisconnectAsync();
    }

    //Connect to the server
    public void connect()
    {
        //Task.Run(RunAsync);
        try
        {
            Debug.Log("### CONNECTING TO SERVER ###");
            client.ConnectAsync(clientOptions);
        }
        catch (Exception exception)
        {
            Debug.Log("### CONNECTING FAILED ###" + Environment.NewLine + exception);
        }
    }

    //Send a message
    public void sendmessage(string SendText, string topic, int qos)
    {
        //WithAtMostOnceQoS(0x00) - This service level guarantees a best-effort delivery. There is no guarantee of delivery. The recipient does not acknowledge receipt of the message and the message is not stored and re-transmitted by the sender. QoS level 0 is often called “fire and forget” and provides the same guarantee as the underlying TCP protocol.
        //WithAtLeastOnceQoS(0x01) - QoS level 1 guarantees that a message is delivered at least one time to the receiver. The sender stores the message until it gets a  PUBACK packet from the receiver that acknowledges receipt of the message. It is possible for a message to be sent or delivered multiple times.
        //WithExactlyOnceQoS(0x02) - QoS 2 is the highest level of service in MQTT. This level guarantees that each message is received only once by the intended recipients. QoS 2 is the safest and slowest quality of service level. The guarantee is provided by at least two request/response flows (a four-part handshake) between the sender and the receiver.

        if (qos == 0)
        {
            var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(SendText)
                .WithAtMostOnceQoS()
                .Build();
            client.PublishAsync(applicationMessage);
            if (OnSentMessage != null)
                OnSentMessage(SendText, topic, qos);
        }
        else if (qos == 1)
        {
            var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(SendText)
                .WithAtLeastOnceQoS()
                .Build();
            client.PublishAsync(applicationMessage);
            if (OnSentMessage != null)
                OnSentMessage(SendText, topic, qos);
        }
        else if (qos == 2)
        {
            var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(SendText)
                .WithExactlyOnceQoS()
                .Build();
            client.PublishAsync(applicationMessage);
            if (OnSentMessage != null)
                OnSentMessage(SendText, topic, qos);
        }
    }

    //Initialize all varaibles 
    private void Init()
    {
        var factory = new MqttFactory();
        client = factory.CreateMqttClient();

        clientOptions = new MqttClientOptionsBuilder()
            .WithClientId(clientid)
            .WithTcpServer(broker, brokerport)
            .WithCredentials(username, password)
            .WithCleanSession(true)
            .WithTls()
            .WithKeepAlivePeriod(new TimeSpan(0, 1, 0))
            .Build();

        client.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(e =>
        {
            //Debug.Log("### RECEIVED APPLICATION MESSAGE ###");
            //Debug.Log($"+ Topic = {e.ApplicationMessage.Topic}");
            Debug.Log($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
            //Debug.Log($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
            //Debug.Log($"+ Retain = {e.ApplicationMessage.Retain}");
            if (OnMessage != null)
                OnMessage(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
            TestEvent.Invoke(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));

        });

        client.ConnectedHandler = new MqttClientConnectedHandlerDelegate(async e =>
        {
            Debug.Log("### CONNECTED WITH SERVER ###");
            if (OnConnect != null)
                OnConnect();

            TestEvent.Invoke("Some MQTT Value COnnect");

            client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("#").Build());

            Debug.Log("### SUBSCRIBED ###");
        });

        client.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(async e =>
        {
            Debug.Log("### DISCONNECTED FROM SERVER ###");

            if (OnDisconnect != null)
                OnDisconnect();
        });

    }


    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        isConnected = client.IsConnected;
    }
}
