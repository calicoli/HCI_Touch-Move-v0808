using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using static PublicInfo;
using static PublicDragParams;

public class ClientCenter : MonoBehaviour
{

    private Color disconnectColor = new Color(0.8156f, 0.3529f, 0.4313f);
    private Color connectColor = new Color(0f, 0f, 0f);

    private TcpClient socketConnection;
    private Thread clientReceiveThread;

    private string thisLabName, serverLabName;

    private Queue receivedQueue;

    private static char paramSeperators = ';';
    private static string[] stringSeparators = new string[] { "//##MSGEND##//" };


    void Awake()
    {
        receivedQueue = new Queue();
        if (receivedQueue.Count != 0)
        {
            receivedQueue.Clear();
        }
        socketConnection = null;
    }

    void Update()
    {
        bool isConnecting = (socketConnection != null);
        GlobalMemory.Instance.setConnectingStatus(isConnecting);

        while (receivedQueue.Count != 0)
        {
            processReceivedMessage();
        }
    }

    private void ConnectToTcpServer(string ipText)
    {
        try
        {
            clientReceiveThread = new Thread(() => ListenForData(ipText));
            clientReceiveThread.IsBackground = true;
            clientReceiveThread.Start();
        }
        catch (Exception e)
        {
            Debug.Log("On client connect exception " + e);
        }
    }

    private void ListenForData(string ipText)
    {
        socketConnection = null;
        try
        {
            socketConnection = new TcpClient(ipText, 8052);
            Byte[] bytes = new Byte[1024];
            while (true)
            {
                using (NetworkStream stream = socketConnection.GetStream())
                {
                    int length;
                    while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        var incommingData = new byte[length];
                        Array.Copy(bytes, 0, incommingData, 0, length);
                        string streamMsg = Encoding.ASCII.GetString(incommingData);
                        string[] rawMsg = streamMsg.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < rawMsg.Length; i++)
                        {
                            receivedQueue.Enqueue(rawMsg[i]);
                        }
                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

    private void sendMessage(string clientMessage)
    {
        if (socketConnection == null)
        {
            return;
        }
        try
        {
            NetworkStream stream = socketConnection.GetStream();
            if (stream.CanWrite)
            {
                clientMessage += stringSeparators[0];
                byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(clientMessage);
                stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
                Debug.Log("C sendMsg: " + clientMessage);
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

    private string getIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }

    private void processReceivedMessage()
    {
        string receiveMsg = (string)receivedQueue.Dequeue();
        Debug.Log("C rcvMsg: " + receiveMsg);
        string[] messages = receiveMsg.Split(';');
        MessageType msgType = (MessageType)Enum.Parse(typeof(MessageType), messages[0]);
        if (msgType == MessageType.Command)
        {
            analyzeCommand(messages);
        }
        else if (msgType == MessageType.Block)
        {
            analyzeBlockInfo(messages);
        }
        else if (msgType == MessageType.Scene)
        {
            analyzeSceneInfo(messages);
        }
        else if (msgType == MessageType.Trial)
        {
            analyzeTrialInfo(messages);
        }
        else if (msgType == MessageType.DragMode)
        {
            analyzeDragMode(messages);
        }
        else if (msgType == MessageType.DirectDragInfo)
        {
            analyzeDirectDragInfo(messages);
        }
        else if (msgType == MessageType.HoldTapInfo)
        {
            analyzeHoldTapInfo(messages);
        }
        else if (msgType == MessageType.ThrowCatchInfo)
        {
            analyzeThrowCatchInfo(messages);
        }
    }

    private void analyzeCommand(string[] messages)
    {
        ServerCommand cmd = (ServerCommand)Enum.Parse(typeof(ServerCommand), messages[1]);
        GlobalMemory.Instance.addServerCommandToQueue(cmd);
    }

    private void analyzeBlockInfo(string[] messages)
    {
        LabName name = (LabName)Enum.Parse(typeof(LabName), messages[1]);
        LabScene scene = (LabScene)Enum.Parse(typeof(LabScene), messages[2]);
        DragType type = (DragType)Enum.Parse(typeof(DragType), messages[3]);
        GlobalMemory.Instance.receiveLabInfoFromServer(name, scene, type);
    }

    private void analyzeSceneInfo(string[] messages)
    {
        GlobalMemory.Instance.curServerScene = (LabScene)Enum.Parse(typeof(LabScene), messages[1]);
    }

    private void analyzeTrialInfo(string[] messages)
    {
        int sRepetitionid = Convert.ToInt32(messages[1]);
        int sTrialid = Convert.ToInt32(messages[2]);
        int sTarget1id = Convert.ToInt32(messages[3]);
        int sTarget2id = Convert.ToInt32(messages[4]);
        string sTrialPhase = messages[5];

        //GlobalMemory.Instance.
        //    receiveTrialDataFromServer(sRepetitionid, sTrialid, sTarget2id, sTrialPhase);

    }

    private void analyzeDragMode(string[] messages)
    {
        DragType dt = (DragType)Enum.Parse(typeof(DragType), messages[1]);
    }

    private void analyzeDirectDragInfo(string[] messages)
    {
        DirectDragStatus target1Status = (DirectDragStatus)Enum.Parse(typeof(DirectDragStatus), messages[1]);
        float target1PosX = Convert.ToSingle(messages[2]);
        float target1PosY = Convert.ToSingle(messages[3]);
        GlobalMemory.Instance.receiveDirectDragInfoFromServer(target1Status, target1PosX, target1PosY);
    }

    private void analyzeHoldTapInfo(string[] messages)
    {
        HoldTapStatus target1Status = (HoldTapStatus)Enum.Parse(typeof(HoldTapStatus), messages[1]);
        GlobalMemory.Instance.receiveHoldTapInfoFromServer(target1Status);
    }

    private void analyzeThrowCatchInfo(string[] messages)
    {
        ThrowCatchStatus target1Status = (ThrowCatchStatus)Enum.Parse(typeof(ThrowCatchStatus), messages[1]);
        float target1PosX = Convert.ToSingle(messages[2]);
        float target1PosY = Convert.ToSingle(messages[3]);
        GlobalMemory.Instance.receiveThrowCatchInfoFromServer(target1Status, target1PosX, target1PosY);
    }

    public void connect(string address)
    {
        if (address != null)
        {
            ConnectToTcpServer(address);
        }
        else
        {
            Debug.Log("Do not have server ip yet");
        }
    }


    public void prepareNewMessage4Server(MessageType msgType)
    {
        string msgContent = "";
        if (msgType == MessageType.Angle)
        {
            Vector3 acc = Input.acceleration;
            msgContent = msgType.ToString() + paramSeperators
                        + acc.x + paramSeperators
                        + acc.y + paramSeperators
                        + acc.z + paramSeperators;
        }
        else if (msgType == MessageType.Scene)
        {
            string clientScene = GlobalMemory.Instance.curClientScene.ToString();
            msgContent = msgType.ToString() + paramSeperators
                        + clientScene + paramSeperators;
        }
        else if (msgType == MessageType.Trial)
        {
            Vector3 acc = Input.acceleration;
            msgContent = msgType.ToString() + paramSeperators
                       + acc.x + paramSeperators
                       + acc.y + paramSeperators
                       + acc.z + paramSeperators;
            switch (GlobalMemory.Instance.targetLabName)
            {
                case LabName.Lab1_move_28:
                    //msgContent = msgContent + getLabTrialMessage();
                    break;
                default:
                    break;
            }


        }
        else if (msgType == MessageType.DirectDragInfo)
        {
            string targetStatus = GlobalMemory.Instance.demoTarget2DirectDragStatus.ToString();
            string targetPosX = GlobalMemory.Instance.demoTarget2DirectDragPosition.x.ToString();
            string targetPosY = GlobalMemory.Instance.demoTarget2DirectDragPosition.y.ToString();
            msgContent = msgType.ToString() + paramSeperators
                       + targetStatus + paramSeperators
                       + targetPosX + paramSeperators
                       + targetPosY + paramSeperators;
        }
        else if (msgType == MessageType.HoldTapInfo)
        {
            string targetStatus = GlobalMemory.Instance.demoTarget2HoldTapStatus.ToString();
            msgContent = msgType.ToString() + paramSeperators
                       + targetStatus.ToString();
        }
        else if (msgType == MessageType.ThrowCatchInfo)
        {
            string t2Status = GlobalMemory.Instance.demoTarget2ThrowCatchStatus.ToString();
            string t2PosX = GlobalMemory.Instance.demoTarget2ThrowCatchPosition.x.ToString();
            string t2PosY = GlobalMemory.Instance.demoTarget2ThrowCatchPosition.y.ToString();
            msgContent = msgType.ToString() + paramSeperators
                       + t2Status + paramSeperators
                       + t2PosX + paramSeperators
                       + t2PosY + paramSeperators;
        }
        sendMessage(msgContent);
    }

    private string getLabTrialMessage()
    {
        string res;
        string cTrialid = GlobalMemory.Instance.curLabTrialid.ToString();
        string cRepeatitionid = GlobalMemory.Instance.curLabRepeateid.ToString();
        string cTarget1id = GlobalMemory.Instance.curLabTrial.firstid.ToString();
        string cTarget2id = GlobalMemory.Instance.curLabTrial.secondid.ToString();
        //string cTrialPhase = GlobalMemory.Instance.curLabTrialPhase.ToString();
        string cTouch2data = GlobalMemory.Instance.getTrialData4Server();
        res = cTrialid + paramSeperators
            + cRepeatitionid + paramSeperators
            + cTarget1id + paramSeperators
            + cTarget2id + paramSeperators
            //+ cTrialPhase + paramSeperators
            + cTouch2data + paramSeperators;
        Debug.Log("LabTrialMsg: " + res);
        return res;
    }
}
