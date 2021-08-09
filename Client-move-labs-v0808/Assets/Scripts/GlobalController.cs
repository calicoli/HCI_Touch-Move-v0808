using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PublicLabFactors;
using static PublicDragParams;

public class GlobalController : MonoBehaviour
{
    public static GlobalController Instance;

    public ClientController client;

    // entry params
    [HideInInspector]
    public string serverip;

    [HideInInspector]
    public Queue<ServerCommand> serverCmdQueue;
    [HideInInspector]
    public LabScene curServerScene, curClientScene;

    [HideInInspector]
    public DragType demoDragType;
    [HideInInspector]
    public bool refreshTarget2 = false;

    [HideInInspector]
    public TargetStatus demoTarget2Status = TargetStatus.total_on_screen_1;
    [HideInInspector]
    public DirectDragStatus demoTarget1DirectDragStatus, demoTarget2DirectDragStatus;
    [HideInInspector]
    public Vector3 demoTarget1DirectDragPosition, demoTarget2DirectDragPosition = Vector3.zero;
    [HideInInspector]
    public HoldTapStatus demoTarget1HoldTapStatus, demoTarget2HoldTapStatus;
    [HideInInspector]
    public ThrowCatchStatus demoTarget1ThrowCatchStatus, demoTarget2ThrowCatchStatus;
    [HideInInspector]
    public Vector3 demoTarget1ThrowCatchPosition = Vector3.zero, demoTarget2ThrowCatchPosition;

    private bool isConnecting;

    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
            curClientScene = LabScene.Entry;
            serverCmdQueue = new Queue<ServerCommand>();
            isConnecting = false;
        }
        else if (Instance != null)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    #region Public Method

    public void connectServer()
    {
        client.connect(serverip);
    }

    public void setConnectingStatus(bool con)
    {
        isConnecting = con;
    }

    public bool getConnectionStatus()
    {
        return isConnecting;
    }
    public void addServerCommandToQueue(ServerCommand cmd)
    {
        serverCmdQueue.Enqueue(cmd);
    }

    public void switchDragType(DragType dt)
    {
        demoDragType = dt;
    }

    public void receiveDirectDragInfoFromServer(DirectDragStatus t1dd, float t1px, float t1py)
    {
        demoTarget1DirectDragStatus = t1dd;
        demoTarget1DirectDragPosition = new Vector3(t1px, t1py, 0f);
        if (t1dd == DirectDragStatus.across_from_screen_1
            || t1dd == DirectDragStatus.across_end_from_screen_1
            || t1dd == DirectDragStatus.drag_phase2_on_screen_1)
        {
            float rightBound = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0)).x;
            demoTarget2DirectDragPosition = new Vector3(t1px - rightBound * 2, t1py, 0f);
            refreshTarget2 = true;
        }
    }


    public void receiveHoldTapInfoFromServer ( HoldTapStatus t1ht )
    {
        demoTarget1HoldTapStatus = t1ht;
    }

    public void receiveThrowCatchInfoFromServer ( ThrowCatchStatus t1tc, float t1px, float t1py )
    {
        demoTarget1ThrowCatchStatus = t1tc;
        demoTarget1ThrowCatchPosition = new Vector3(t1px, t1py, 0f);

        if ( t1tc == ThrowCatchStatus.throw_successed_on_screen_1 )
        {
            float rightBound = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0)).x;
            demoTarget2ThrowCatchPosition = new Vector3(t1px - rightBound * 2, t1py, 0f);
        }
        else if ( t1tc == ThrowCatchStatus.t2_move_phase2_ongoing )
        {
            float rightBound = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0)).x;
            demoTarget2ThrowCatchPosition = new Vector3(t1px - rightBound * 2, t1py, 0f);
        }
        
    }
    #endregion
}
