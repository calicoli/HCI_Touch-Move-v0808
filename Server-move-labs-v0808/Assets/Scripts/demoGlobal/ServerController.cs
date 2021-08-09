using System;
using System.Collections;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using static PublicLabFactors;
using static PublicDragParams;

public class ServerController : MonoBehaviour
{
    private TcpListener tcpListener;
    private Thread tcpListenerThread;
    private TcpClient connectedTcpClient;

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

        tcpListenerThread = new Thread(new ThreadStart(ListenForIncommingRequests));
        tcpListenerThread.IsBackground = true;
        tcpListenerThread.Start();
    }

    // Update is called once per frame
    void Update()
    {
        GlobalController.Instance.serverip = getIPAddress();
        bool isConnecting = connectedTcpClient != null;

        GlobalController.Instance.setConnectingStatus(isConnecting);

        while ( receivedQueue.Count != 0 )
        {
            processReceivedMessage();
        }
    }
    private void ListenForIncommingRequests()
    {
        try
        {
            tcpListener = new TcpListener(IPAddress.Any, 8052);
            tcpListener.Start();
            Debug.Log("Server is listening");
            Byte[] bytes = new Byte[1024];
            while (true)
            {
                using (connectedTcpClient = tcpListener.AcceptTcpClient())
                {
                    using (NetworkStream stream = connectedTcpClient.GetStream())
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
        }
        catch (SocketException socketException)
        {
            Debug.Log("SocketException " + socketException.ToString());
        }
    }

    private void sendMessage(string serverMessage)
    {
        if (connectedTcpClient == null)
        {
            return;
        }
        try
        {
            NetworkStream stream = connectedTcpClient.GetStream();
            if (stream.CanWrite)
            {
                serverMessage += stringSeparators[0];
                byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(serverMessage);
                stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);
                //Debug.Log("Server sent his message - should be received by client");
                Debug.Log("S sendMsg: " + serverMessage);
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

    public void prepareNewMessage4Client(MessageType msgType, ServerCommand cmd)
    {
        if ( msgType == MessageType.Command )
        {
            string msgContent = msgType.ToString() + paramSeperators + cmd.ToString() + paramSeperators;
            sendMessage(msgContent);
        }
    }

    public void prepareNewMessage4Client(MessageType msgType)
    {
        string msgContent = "";
        if ( msgType == MessageType.DragMode )
        {
            //int blockid = GlobalController.Instance.curBlockid;
            string targetMode = GlobalController.Instance.demoDragType.ToString();
            msgContent = msgType.ToString() + paramSeperators
                       + targetMode + paramSeperators;
        }
        else if ( msgType == MessageType.DirectDragInfo )
        {
            string targetOutBound = GlobalController.Instance.demoTarget1DirectDragStatus.ToString();
            string targetPosX = GlobalController.Instance.demoTarget1DirectDragPosition.x.ToString();
            string targetPosY = GlobalController.Instance.demoTarget1DirectDragPosition.y.ToString();
            msgContent = msgType.ToString() + paramSeperators
                       + targetOutBound + paramSeperators
                       + targetPosX + paramSeperators
                       + targetPosY + paramSeperators;
        }
        else if ( msgType == MessageType.HoldTapInfo )
        {
            string t1Status = GlobalController.Instance.demoTarget1HoldTapStatus.ToString();
            msgContent = msgType.ToString() + paramSeperators
                       + t1Status + paramSeperators;
        }
        else if ( msgType == MessageType.ThrowCatchInfo)
        {
            string t1Status = GlobalController.Instance.demoTarget1ThrowCatchStatus.ToString();
            string t1PosX = GlobalController.Instance.demoTarget1ThrowCatchPosition.x.ToString();
            string t1PosY = GlobalController.Instance.demoTarget1ThrowCatchPosition.y.ToString();
            msgContent = msgType.ToString() + paramSeperators
                       + t1Status + paramSeperators
                       + t1PosX + paramSeperators
                       + t1PosY + paramSeperators;
        }
        sendMessage(msgContent);
    }

    private void processReceivedMessage()
    {
        string receiveMsg = (string)receivedQueue.Dequeue();
        Debug.Log("C rcvMsg: " + receiveMsg);
        string[] messages = receiveMsg.Split(';');
        MessageType msgType = (MessageType)Enum.Parse(typeof(MessageType), messages[0]);
        if ( msgType == MessageType.Command )
        {
            //analyzeCommand(messages);
        }
        else if ( msgType == MessageType.DirectDragInfo )
        {
            analyzeDirectDragInfo(messages);
        }
        else if ( msgType == MessageType.HoldTapInfo )
        {
            analyzeHoldTapInfo(messages);
        }
        else if ( msgType == MessageType.ThrowCatchInfo )
        {
            analyzeThrowCatchInfo(messages);
        }
    }

    private void analyzeDirectDragInfo( string[] messages )
    {
        DirectDragStatus target2Status = (DirectDragStatus)Enum.Parse(typeof(DirectDragStatus), messages[1]);
        float target2PosX = Convert.ToSingle(messages[2]);
        float target2PosY = Convert.ToSingle(messages[3]);
        GlobalController.Instance.receiveDirectDragInfoFromClient(target2Status, target2PosX, target2PosY);
    }

    private void analyzeHoldTapInfo( string[] messages )
    {
        HoldTapStatus target2Status = (HoldTapStatus)Enum.Parse(typeof(HoldTapStatus), messages[1]);
        GlobalController.Instance.receiveHoldTapInfoFromClient(target2Status);
    }

    private void analyzeThrowCatchInfo (string[] messages)
    {
        ThrowCatchStatus target2Status = (ThrowCatchStatus)Enum.Parse(typeof(ThrowCatchStatus), messages[1]);
        float target2PosX = Convert.ToSingle(messages[2]);
        float target2PosY = Convert.ToSingle(messages[3]);
        GlobalController.Instance.receiveThrowCatchInfoFromClinet(target2Status, target2PosX, target2PosY);
    }
}
