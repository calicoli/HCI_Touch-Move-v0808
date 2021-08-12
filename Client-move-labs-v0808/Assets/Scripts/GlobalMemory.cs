using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PublicInfo;
using static PublicLabParams;
using static PublicBlockParams;
using static PublicTrialParams;
using static PublicDragParams;
using System;

public class GlobalMemory : MonoBehaviour
{

    public static GlobalMemory Instance;

    public ClientCenter client;
    public AngleProcessor angleProcessor;
    public FileProcessor fileProcssor;

    // entry params
    [HideInInspector]
    public string serverip;

    [HideInInspector]
    public Queue<ServerCommand> serverCmdQueue;
    [HideInInspector]
    public LabScene curServerScene, curClientScene;

    // block params
    [HideInInspector]
    public int curBlockid;
    [HideInInspector]
    public BlockCondition curBlockCondition;

    // entry params
    [HideInInspector]
    public WelcomePhase curIndexPhase;
    [HideInInspector]
    public bool isLabInfoSet;

    // trial params
    [HideInInspector]
    public bool serverRefreshTrialData;
    [HideInInspector]
    public LabPhase curLabPhase;
    [HideInInspector]
    public int curLabTrialid, curLabRepeateid;
    [HideInInspector]
    public Trial curLabTrial;
    //[HideInInspector]
    //public TrialPhase curLabTrialPhase;
    [HideInInspector]
    public TrialDataWithLocalTime curLabTrialData;
    // ↑ wait to update

    [HideInInspector]
    public LabName targetLabName;
    [HideInInspector]
    public LabScene targetLabScene;

    [HideInInspector]
    public DragType targetDragType;
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
            curClientScene = LabScene.Index_scene;
            curIndexPhase = WelcomePhase.in_entry_scene;
            serverCmdQueue = new Queue<ServerCommand>();
            isLabInfoSet = false;
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
        serverRefreshTrialData = false;
    }

    // Update is called once per frame
    void Update()
    {
        //client.connect(serverip);
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

    public void setAngleDetectStatus(bool open)
    {
        angleProcessor.setConveyAccStatus(open);
    }

    public string getTargetLabName ()
    {
        return targetLabName.ToString();
    }

    public void receiveTrialDataFromServer (int rid, int tid, int t1id, int t2id, string tPhase)
    {
        switch (targetLabName)
        {
            case LabName.Lab1_move_28:
                curLabRepeateid = rid;
                curLabTrialid = tid;
                curLabTrial.setParams(tid, t1id, t2id);
                //curLabTrialPhase = (TrialPhase)Enum.Parse(typeof(TrialPhase), tPhase);
                break;
            default:
                break;
        }
        serverRefreshTrialData = true;
    }

    public string getTrialData4Server()
    {
        string res = "";
        switch (targetLabName)
        {
            case LabName.Lab1_move_28:
                res = curLabTrialData.getAllData();
                break;
            default:
                break;
        }
        return res;
    }

    public void receiveLabInfoFromServer (LabName name, LabScene scene, DragType type)
    {
        targetLabName = name;
        targetLabScene = scene;
        targetDragType = type;
        Debug.Log("Lab params: " + name + " " + scene + " " + type);
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


    public void receiveHoldTapInfoFromServer(HoldTapStatus t1ht)
    {
        demoTarget1HoldTapStatus = t1ht;
    }

    public void receiveThrowCatchInfoFromServer(ThrowCatchStatus t1tc, float t1px, float t1py)
    {
        demoTarget1ThrowCatchStatus = t1tc;
        demoTarget1ThrowCatchPosition = new Vector3(t1px, t1py, 0f);

        if (t1tc == ThrowCatchStatus.throw_successed_on_screen_1)
        {
            float rightBound = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0)).x;
            demoTarget2ThrowCatchPosition = new Vector3(t1px - rightBound * 2, t1py, 0f);
        }
        else if (t1tc == ThrowCatchStatus.t2_move_phase2_ongoing)
        {
            float rightBound = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0)).x;
            demoTarget2ThrowCatchPosition = new Vector3(t1px - rightBound * 2, t1py, 0f);
        }

    }
    #endregion
}
