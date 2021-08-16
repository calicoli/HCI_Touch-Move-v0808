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
    //[HideInInspector]
    //public LabInfos curLabInfos;
    [HideInInspector]
    public LabName targetLabName;
    [HideInInspector]
    public LabMode targetLabMode;
    [HideInInspector]
    public LabScene targetLabScene;
    [HideInInspector]
    public DragType targetDragType;

    // trial params
    [HideInInspector]
    public bool serverRefreshTrialData;
    [HideInInspector]
    public LabPhase curLabPhase;
    [HideInInspector]
    public int curLabRepeatid, curLabTrialid, curLabTrialNumber;
    [HideInInspector]
    public Trial curLabTrial;
    [HideInInspector]
    public TrialPhase curLabTrialPhase, serverLabTrialPhase;
    [HideInInspector]
    public TrialDataWithLocalTime curLabTrialData;
    // ↑ wait to update

    
    [HideInInspector]
    public bool refreshTarget2 = false;

    [HideInInspector]
    public TargetStatus lab1Target2Status = TargetStatus.total_on_screen_1;
    [HideInInspector]
    public DirectDragStatus tech1Target1DirectDragStatus, tech1Target2DirectDragStatus;
    [HideInInspector]
    public Vector3 tech1Target1DirectDragPosition, tech1Target2DirectDragPosition = Vector3.zero;
    [HideInInspector]
    public DirectDragResult tech1Target1DirectDragResult, tech1Target2DirectDragResult;

    [HideInInspector]
    public HoldTapStatus tech2Target1HoldTapStatus, tech2Target2HoldTapStatus;
    [HideInInspector]
    public HoldTapResult tech2Target1HoldTapResult, tech2Target2HoldTapResult;

    [HideInInspector]
    public ThrowCatchStatus tech3Target1ThrowCatchStatus, tech3Target2ThrowCatchStatus;
    [HideInInspector]
    public Vector3 tech3Target1ThrowCatchPosition, tech3Target2ThrowCatchPosition;
    [HideInInspector]
    public ThrowCatchResult tech3Target1ThrowCatchResult, tech3Target2ThrowCatchResult;

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

    public void receiveTrialDataFromServer (int num, int tid, int t1id, int t2id, string tPhase)
    {
        switch (targetLabName)
        {
            case LabName.Lab1_move_28:
                Debug.Log(num + " " + tid + " " + t1id + " " + t2id + " " + tPhase);
                curLabTrialNumber = num;
                curLabTrialid = tid;
                curLabTrial.setParams(tid, t1id, t2id);
                serverLabTrialPhase = (TrialPhase)Enum.Parse(typeof(TrialPhase), tPhase);
                break;
            default:
                break;
        }
        //serverRefreshTrialData = true;
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

    public void receiveLabInfoFromServer (LabName name, LabMode mode, LabScene scene, DragType type)
    {
        targetLabName = name;
        targetLabMode = mode;
        targetLabScene = scene;
        targetDragType = type;
        Debug.Log("Lab params: " + name + " " + mode + " " + scene + " " + type);
    }

    public void receiveDirectDragInfoFromServer(DirectDragStatus t1dd, float t1px, float t1py, DirectDragResult t1result)
    {
        tech1Target1DirectDragStatus = t1dd;
        tech1Target1DirectDragPosition = new Vector3(t1px, t1py, 0f);
        if (t1dd == DirectDragStatus.across_from_screen_1
            || t1dd == DirectDragStatus.across_end_from_screen_1
            || t1dd == DirectDragStatus.drag_phase2_on_screen_1)
        {
            float rightBound = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0)).x;
            tech1Target2DirectDragPosition = new Vector3(t1px - rightBound * 2, t1py, 0f);
            refreshTarget2 = true;
        }
        tech1Target1DirectDragResult = t1result;
    }


    public void receiveHoldTapInfoFromServer(HoldTapStatus t1ht, HoldTapResult t1result)
    {
        tech2Target1HoldTapStatus = t1ht;
        tech2Target1HoldTapResult = t1result;
    }

    public void receiveThrowCatchInfoFromServer(ThrowCatchStatus t1tc, float t1px, float t1py, ThrowCatchResult t1result)
    {
        tech3Target1ThrowCatchStatus = t1tc;
        tech3Target1ThrowCatchPosition = new Vector3(t1px, t1py, 0f);
        tech3Target1ThrowCatchResult = t1result;

        if (t1tc == ThrowCatchStatus.throw_successed_on_screen_1)
        {
            float rightBound = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0)).x;
            tech3Target2ThrowCatchPosition = new Vector3(t1px - rightBound * 2, t1py, 0f);
        }
        else if (t1tc == ThrowCatchStatus.t2_move_phase2_ongoing)
        {
            float rightBound = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0)).x;
            tech3Target2ThrowCatchPosition = new Vector3(t1px - rightBound * 2, t1py, 0f);
        }

    }
    #endregion
}
