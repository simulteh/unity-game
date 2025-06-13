using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TCPAutomatedSimulator : MonoBehaviour
{
    public enum TCPMessageType
    {
        SYN,    // Synchronization
        SYN_ACK, // Synchronization-Acknowledgement
        ACK,    // Acknowledgement
        DATA,   // Data
        FIN,    // Finish
        FIN_ACK // Finish-Acknowledgement
    }

    [System.Serializable]
    public class TCPMessage
    {
        public TCPMessageType type;
        public int sequenceNumber;
        public int acknowledgementNumber;
        public string payload;

        public TCPMessage(TCPMessageType type, int seqNum, int ackNum, string payload = "")
        {
            this.type = type;
            this.sequenceNumber = seqNum;
            this.acknowledgementNumber = ackNum;
            this.payload = payload;
        }

        public override string ToString()
        {
            return $"[MSG Type: {type}, Seq: {sequenceNumber}, Ack: {acknowledgementNumber}, Payload: '{payload}']";
        }
    }

    public enum ConnectionState
    {
        Closed,
        Listen,
        SynSent,
        SynReceived,
        Established,
        FinWait1,
        FinWait2,
        TimeWait,
        CloseWait,
        LastAck,
        Closing
    }

    //Client-Server
    [System.Serializable]
    public class TCPEntity
    {
        public string entityName;
        public ConnectionState currentState = ConnectionState.Closed;

        //Buffer
        public Queue<TCPMessage> sendBuffer = new Queue<TCPMessage>();
        public Queue<TCPMessage> receiveBuffer = new Queue<TCPMessage>();

        public int sequenceNumber = 0;
        public int acknowledgementNumber = 0;

        [HideInInspector] public TCPEntity remoteEntity;

        [HideInInspector] public TCPAutomatedSimulator manager;

        public TCPEntity(string name, TCPAutomatedSimulator mgr)
        {
            entityName = name;
            manager = mgr;
        }

        public void Log(string message)
        {
            Debug.Log($"[{entityName}] {message}");
        }

        public void ReceiveMessage(TCPMessage msg)
        {
            Log($"Received: {msg}");
            receiveBuffer.Enqueue(msg);
            ProcessReceivedMessage(msg);
        }

        private void ProcessReceivedMessage(TCPMessage msg)
        {
            switch (currentState)
            {
                case ConnectionState.Closed:
                    if (msg.type == TCPMessageType.SYN && entityName == "Server")
                    {
                        Log("Received SYN in Closed state (Server).");
                        currentState = ConnectionState.SynReceived;
                        manager.StartCoroutine(manager.SendSYNACK(this, msg));
                    }
                    break;

                case ConnectionState.Listen:
                    if (msg.type == TCPMessageType.SYN)
                    {
                        Log("Received SYN in Listen state (Server).");
                        currentState = ConnectionState.SynReceived;
                        manager.StartCoroutine(manager.SendSYNACK(this, msg));
                    }
                    break;

                case ConnectionState.SynSent:
                    if (msg.type == TCPMessageType.SYN_ACK && msg.acknowledgementNumber == sequenceNumber + 1)
                    {
                        Log("Received SYN-ACK in SynSent state (Client).");
                        acknowledgementNumber = msg.sequenceNumber + 1;
                        manager.StartCoroutine(manager.SendACK(this, msg));
                    }
                    else if (msg.type == TCPMessageType.SYN) // Simultaneous open
                    {
                        Log("Received SYN in SynSent state (Client). Simultaneous open.");
                        acknowledgementNumber = msg.sequenceNumber + 1;
                        manager.StartCoroutine(manager.SendSYNACK(this, new TCPMessage(TCPMessageType.SYN, sequenceNumber, acknowledgementNumber)));
                        currentState = ConnectionState.SynReceived;
                    }
                    break;

                case ConnectionState.SynReceived:
                    if (msg.type == TCPMessageType.ACK && msg.acknowledgementNumber == sequenceNumber + 1)
                    {
                        Log("Received ACK in SynReceived state (Server).");
                        acknowledgementNumber = msg.sequenceNumber + 1;
                        currentState = ConnectionState.Established;
                        Log($"Connection Established! State: {currentState}");
                    }
                    break;

                case ConnectionState.Established:
                    if (msg.type == TCPMessageType.DATA)
                    {
                        Log($"Data received: '{msg.payload}'");
                        acknowledgementNumber = msg.sequenceNumber + msg.payload.Length;
                        manager.StartCoroutine(manager.SendACK(this, new TCPMessage(TCPMessageType.ACK, sequenceNumber, acknowledgementNumber)));
                    }
                    else if (msg.type == TCPMessageType.ACK)
                    {
                        Log($"Received ACK for sent data. Ack Num: {msg.acknowledgementNumber}");
                    }
                    else if (msg.type == TCPMessageType.FIN)
                    {
                        Log("Received FIN in Established state.");
                        acknowledgementNumber = msg.sequenceNumber + 1;
                        manager.StartCoroutine(manager.SendACK(this, new TCPMessage(TCPMessageType.ACK, sequenceNumber, acknowledgementNumber)));
                        currentState = ConnectionState.CloseWait;
                        Log($"State: {currentState}");
                    }
                    break;

                case ConnectionState.CloseWait:
                    break;

                case ConnectionState.FinWait1:
                    if (msg.type == TCPMessageType.ACK && msg.acknowledgementNumber == sequenceNumber + 1)
                    {
                        Log("Received ACK for our FIN in FinWait1 state.");
                        currentState = ConnectionState.FinWait2;
                        Log($"State: {currentState}");
                    }
                    else if (msg.type == TCPMessageType.FIN)
                    {
                        Log("Received FIN in FinWait1 state (simultaneous close).");
                        acknowledgementNumber = msg.sequenceNumber + 1;
                        manager.StartCoroutine(manager.SendACK(this, new TCPMessage(TCPMessageType.ACK, sequenceNumber, acknowledgementNumber)));
                        currentState = ConnectionState.Closing;
                        Log($"State: {currentState}");
                    }
                    break;

                case ConnectionState.FinWait2:
                    if (msg.type == TCPMessageType.FIN)
                    {
                        Log("Received FIN in FinWait2 state.");
                        acknowledgementNumber = msg.sequenceNumber + 1;
                        manager.StartCoroutine(manager.SendACK(this, new TCPMessage(TCPMessageType.ACK, sequenceNumber, acknowledgementNumber)));
                        currentState = ConnectionState.TimeWait;
                        Log($"State: {currentState}");
                        manager.StartCoroutine(manager.TimeWaitTimer(this));
                    }
                    break;

                case ConnectionState.TimeWait:
                    break;

                case ConnectionState.Closing:
                    if (msg.type == TCPMessageType.ACK && msg.acknowledgementNumber == sequenceNumber + 1)
                    {
                        Log("Received ACK in Closing state. (Simultaneous close completed)");
                        currentState = ConnectionState.TimeWait;
                        Log($"State: {currentState}");
                        manager.StartCoroutine(manager.TimeWaitTimer(this));
                    }
                    break;

                case ConnectionState.LastAck:
                    if (msg.type == TCPMessageType.ACK && msg.acknowledgementNumber == sequenceNumber + 1)
                    {
                        Log("Received ACK in LastAck state. Connection closed.");
                        currentState = ConnectionState.Closed;
                    }
                    break;
            }
        }
    }

    // Simulation Parameters
    [Header("TCP Simulation Parameters")]
    public float messageDelay = 1.0f; //In seconds
    public float dataSendInterval = 1.0f;
    public int numberOfDataPackets = 1;

    // Client & Server entity
    public TCPEntity client;
    public TCPEntity server;

    private int dataPacketsSent = 0;

    void Awake()
    {
        client = new TCPEntity("Client", this);
        server = new TCPEntity("Server", this);

        client.remoteEntity = server;
        server.remoteEntity = client;
    }

    void Start()
    {
        server.currentState = ConnectionState.Listen;
        server.Log($"Server started. State: {server.currentState}");

        StartCoroutine(AutoConnectRoutine());
    }

    private IEnumerator SimulateMessageSend(TCPEntity sender, TCPMessage msg)
    {
        yield return new WaitForSeconds(messageDelay);
        sender.Log($"Sending: {msg}");
        sender.remoteEntity?.ReceiveMessage(msg);
    }

    private IEnumerator SendSYN(TCPEntity sender)
    {
        sender.sequenceNumber = UnityEngine.Random.Range(1000, 5000);
        sender.acknowledgementNumber = 0;
        TCPMessage syn = new TCPMessage(TCPMessageType.SYN, sender.sequenceNumber, sender.acknowledgementNumber);
        yield return SimulateMessageSend(sender, syn);
        sender.currentState = ConnectionState.SynSent;
        sender.Log($"State: {sender.currentState}");
    }

    private IEnumerator SendSYNACK(TCPEntity sender, TCPMessage receivedSYN)
    {
        sender.acknowledgementNumber = receivedSYN.sequenceNumber + 1;
        sender.sequenceNumber = UnityEngine.Random.Range(5000, 10000);
        TCPMessage synAck = new TCPMessage(TCPMessageType.SYN_ACK, sender.sequenceNumber, sender.acknowledgementNumber);
        yield return SimulateMessageSend(sender, synAck);
        sender.currentState = ConnectionState.SynReceived;
        sender.Log($"State: {sender.currentState}");
    }

    private IEnumerator SendACK(TCPEntity sender, TCPMessage receivedMsg)
    {
        sender.acknowledgementNumber = receivedMsg.sequenceNumber + 1;
        sender.sequenceNumber++;
        TCPMessage ack = new TCPMessage(TCPMessageType.ACK, sender.sequenceNumber, sender.acknowledgementNumber);
        yield return SimulateMessageSend(sender, ack);
        if (sender.currentState == ConnectionState.SynSent)
        {
            sender.currentState = ConnectionState.Established;
            sender.Log($"Connection Established! State: {sender.currentState}");
            if (sender.entityName == "Client")
            {
                StartCoroutine(SendDataAutomated(sender));
            }
        }
    }

    private IEnumerator SendFIN(TCPEntity sender)
    {
        TCPMessage fin = new TCPMessage(TCPMessageType.FIN, sender.sequenceNumber, sender.acknowledgementNumber);
        yield return SimulateMessageSend(sender, fin);
        if (sender.currentState == ConnectionState.Established)
        {
            sender.currentState = ConnectionState.FinWait1;
        }
        else if (sender.currentState == ConnectionState.CloseWait)
        {
            sender.currentState = ConnectionState.LastAck;
        }
        sender.Log($"State: {sender.currentState}");
    }

    private IEnumerator TimeWaitTimer(TCPEntity entity)
    {
        entity.Log("Starting TimeWait timer (2 seconds for simulation).");
        yield return new WaitForSeconds(2f); // 2*MSL
        entity.currentState = ConnectionState.Closed;
        entity.Log($"State: {entity.currentState}. Connection closed.");
    }
    
    private IEnumerator AutoConnectRoutine()
    {
        yield return new WaitForSeconds(1.0f);
        client.Log("Client attempting to connect automatically...");
        StartCoroutine(SendSYN(client));
    }

    private IEnumerator SendDataAutomated(TCPEntity sender)
    {
        yield return new WaitForSeconds(dataSendInterval);
        while (sender.currentState == ConnectionState.Established && dataPacketsSent < numberOfDataPackets)
        {
            string data = $"Hello from {sender.entityName}! Packet {dataPacketsSent + 1}";
            sender.Log($"Sending automated data: '{data}'");
            TCPMessage dataMsg = new TCPMessage(TCPMessageType.DATA, sender.sequenceNumber, sender.acknowledgementNumber, data);
            sender.sendBuffer.Enqueue(dataMsg);
            yield return SimulateMessageSend(sender, dataMsg);
            sender.sequenceNumber += data.Length;
            dataPacketsSent++;
            yield return new WaitForSeconds(dataSendInterval);
        }

        if (sender.currentState == ConnectionState.Established && dataPacketsSent >= numberOfDataPackets)
        {
            sender.Log("Finished sending automated data. Initiating disconnect...");
            StartCoroutine(SendFIN(sender));
        }
    }

    private void FixedUpdate()
    {
        if (server.currentState == ConnectionState.CloseWait)
        {
            if (server.sendBuffer.Count == 0)
            {
                server.Log("Server completing close. Sending FIN from CloseWait state.");
                StartCoroutine(SendFIN(server));
            }
        }
    }
}