using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static PublicInfo;
using static PublicLabParams;
using static PublicBlockParams;
using static PublicTrialParams;
using static PublicDragParams;

public class GlobalMemory: MonoBehaviour
{
    public static GlobalMemory Instance;

    public ServerCenter server;
    public FileProcessor fileProcessor;
    public AngleProcessor angleProcessor;

    [HideInInspector]
    public string sendInfo, rcvInfo;

    // index scene params
    [HideInInspector]
    public int userid;
    [HideInInspector]
    public string serverip;
    [HideInInspector]
    public Vector3 accClient;
    [HideInInspector]
    public float curAngle;
    [HideInInspector]
    public bool isUserLabInfoSet;

    [HideInInspector]
    public ServerCommand curServerCommand;
    [HideInInspector]
    public LabScene curServerScene, curClientScene;

    [HideInInspector]
    public LabInfos curLabInfos;
    [HideInInspector]
    public WelcomePhase curIndexPhase;

    // block params
    [HideInInspector]
    public int curBlockid;
    [HideInInspector]
    public BlockCondition curBlockCondition;

    // trial params
    [HideInInspector]
    public bool clientRefreshedTrialData;
    [HideInInspector]
    public LabPhase curLabPhase;
    [HideInInspector]
    public int curLabRepeatid, curLabTrialid, curLabTrialNumber;
    [HideInInspector]
    public Trial curLabTrial;
    [HideInInspector]
    public TrialPhase curLabTrialPhase, clientLabTrialPhase;
    [HideInInspector]
    public TrialSequence curLabTrialSequence;
    [HideInInspector]
    public TrialDataWithLocalTime curLabTrialData;
    [HideInInspector]
    public TrialPhase1RawData curLabPhase1RawData;
    [HideInInspector]
    public TrialPhase2RawData curLabPhase2RawData;
    /*
    [HideInInspector]
    public tech1DirectDragTrialData tech1TrialData;
    [HideInInspector]
    public tech2HoldTapTrialData tech2TrialData;
    [HideInInspector]
    public tech3ThrowCatchTrialData tech3TrialData;*/

    [HideInInspector]
    public DragType curDragType;
    [HideInInspector]
    public bool refreshTarget1 = false;
    [HideInInspector]
    public TargetStatus lab1Target1Status = TargetStatus.total_on_screen_1;

    [HideInInspector]
    public DirectDragStatus tech1Target1DirectDragStatus, tech1Target2DirectDragStatus;
    [HideInInspector]
    public Vector3 tech1Target1DirectDragPosition, tech1Target2DirectDragPosition;
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
    private BlockSequence seqBlocks;
    private BlockCondition[] conBlocks;

    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
            curServerScene = LabScene.Index_scene;
            curClientScene = LabScene.Index_scene;
            curBlockid = BLOCK_START_INDEX;
            curIndexPhase = WelcomePhase.in_entry_scene;
            isUserLabInfoSet = false;
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

    private bool scheduleBlocks()
    {
        if (curLabInfos.labName == LabName.Lab1_move_28)
        {
            conBlocks = new BlockCondition[curLabInfos.totalBlockCount + 1];

            // set variable: seqBlock
            seqBlocks.setBlockLength(LabName.Lab1_move_28);
            seqBlocks.setAllSequence(userid);

            // set variable: conBlock
            for (int blockid = BLOCK_START_INDEX; blockid <= curLabInfos.totalBlockCount; blockid++)
            {
                int pid = (int)seqBlocks.seqPosture[blockid - 1];
                int oid = (int)seqBlocks.seqOrientation[blockid - 1];
                int aid = (int)seqBlocks.seqAngle[blockid - 1];
                int sid = (int)seqBlocks.seqShape[blockid - 1];
                conBlocks[blockid] = new BlockCondition(blockid, pid, oid, aid, sid);
            }

            curBlockCondition = conBlocks[curBlockid];
            Debug.Log(curBlockCondition.getAllDataForFile());
            Debug.Log(curBlockCondition.getAllDataForSceneDisplay());
            return true;
        }
        return false;
    }

    private long CurrentTimeMillis()
    {
        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        TimeSpan diff = DateTime.Now.ToUniversalTime() - origin;
        return (long)diff.TotalMilliseconds;
    }

    private long AnthoerWayToGetCurrentTimeMillis()
    {
        return (long)(DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
    }

    #region Public Method
    public void setConnectingStatus(bool con)
    {
        isConnecting = con;
    }

    public bool getConnectionStatus()
    {
        return isConnecting;
    }

    public bool setLabParams(LabName name, bool isFullMode)
    {
        curLabInfos = new LabInfos();
        curLabInfos.setLabInfoWithName(name);
        if (!isFullMode)
        {
            curLabInfos.setTestModeParams();
        }
        bool end = scheduleBlocks();
        if (end)
        {
            Debug.Log("Set labParams: " + curLabInfos.labName + " " + curLabInfos.labMode);
            return true;
        }
        return false;
    }

    public string getLabSceneToEnter()
    {
        string targetScene = "";
        switch (curDragType)
        {
            case DragType.direct_drag:
            case DragType.hold_tap:
            case DragType.throw_catch:
                targetScene = LabScene.Lab1_move_3techs.ToString();
                break;
        }
        return targetScene;
    }

    public void moveToNextBlock()
    {
        curBlockid++;
        switch (curLabInfos.labName)
        {
            case LabName.Lab1_move_28:
                curBlockCondition = conBlocks[curBlockid];
                break;
        }

        server.prepareNewMessage4Client(MessageType.Command, ServerCommand.server_say_exit_lab);
        curIndexPhase = WelcomePhase.check_client_scene;
        string entrySceneName = (LabScene.Index_scene).ToString();
        SceneManager.LoadScene(entrySceneName);
    }

    public bool haveNextBlock()
    {
        if (curBlockid + 1 <= curLabInfos.totalBlockCount)
        {
            return true;
        }
        return false;
    }

    public void excuteCommand(ServerCommand cmd)
    {
        curServerCommand = cmd;
        server.prepareNewMessage4Client(MessageType.Command, cmd);
    }

    public void receiveTrialInfoFromClient(int cTrialNumber, int cTrialIndex,
       int cTarget1id, int cTarget2id, string cTrialPhase, string cTouchData)
    {
        switch (curLabInfos.labName)
        {
            case LabName.Lab1_move_28:
                if (cTrialNumber == curLabTrialNumber && cTrialIndex == curLabTrialid)
                {
                    clientRefreshedTrialData = true;
                    clientLabTrialPhase = (TrialPhase)Enum.Parse(typeof(TrialPhase), cTrialPhase);
                    if ( clientLabTrialPhase == TrialPhase.a_trial_end)
                    {
                        parseLabTouchDataString(cTouchData);
                    }
                }
                break;
        }
    }

    public void parseLabTouchDataString (string touchData)
    {
        Debug.Log("Befor parse: " + touchData);
        string[] messages = touchData.Split('#');
        int cTrialIndex = Convert.ToInt32(messages[0]);
        int cTarget1id = Convert.ToInt32(messages[1]);
        int cTarget2id = Convert.ToInt32(messages[2]);

        if (cTrialIndex == curLabTrial.index 
            && cTarget1id == curLabTrial.firstid && cTarget2id == curLabTrial.secondid)
        {
            if (lab1Target1Status == TargetStatus.total_on_screen_1)
            {
                curLabPhase2RawData.moveDestination = new Vector2(Convert.ToSingle(messages[3]), Convert.ToSingle(messages[4]));
                curLabPhase2RawData.touch2StartStamp = Convert.ToInt64(messages[5]);
                curLabPhase2RawData.touch2EndStamp = Convert.ToInt64(messages[6]);
                curLabPhase2RawData.targetReachMidpointInfoReceivedStamp = Convert.ToInt64(messages[7]);
                curLabPhase2RawData.targetReachEndpointStamp = Convert.ToInt64(messages[8]);
                curLabPhase2RawData.touch2StartPos = new Vector2(Convert.ToSingle(messages[9]), Convert.ToSingle(messages[10]));
                curLabPhase2RawData.touch2EndPos = new Vector2(Convert.ToSingle(messages[11]), Convert.ToSingle(messages[12]));
                curLabPhase2RawData.movePhase2StartPos = new Vector2(Convert.ToSingle(messages[13]), Convert.ToSingle(messages[14]));
                curLabPhase2RawData.movePhase2EndPos = new Vector2(Convert.ToSingle(messages[15]), Convert.ToSingle(messages[16]));
            }
            else if (lab1Target1Status == TargetStatus.total_on_screen_2)
            {
                curLabPhase1RawData.moveStartPos = new Vector2(Convert.ToSingle(messages[3]), Convert.ToSingle(messages[4]));
                curLabPhase1RawData.touch1StartStamp = Convert.ToInt64(messages[5]);
                curLabPhase1RawData.touch1EndStamp = Convert.ToInt64(messages[6]);
                curLabPhase1RawData.targetReachMidpointStamp = Convert.ToInt64(messages[7]);
                curLabPhase1RawData.targetReachEndpointInfoReceivedStamp = Convert.ToInt64(messages[8]);
                curLabPhase1RawData.touch1StartPos = new Vector2(Convert.ToSingle(messages[9]), Convert.ToSingle(messages[10]));
                curLabPhase1RawData.touch1EndPos = new Vector2(Convert.ToSingle(messages[11]), Convert.ToSingle(messages[12]));
                curLabPhase1RawData.movePhase1StartPos = new Vector2(Convert.ToSingle(messages[13]), Convert.ToSingle(messages[14]));
                curLabPhase1RawData.movePhase1EndPos = new Vector2(Convert.ToSingle(messages[15]), Convert.ToSingle(messages[16]));
            }
        }
    }

    public void receiveDirectDragInfoFromClient(DirectDragStatus t2dd, float t2px, float t2py, DirectDragResult t2result)
    {
        tech1Target2DirectDragStatus = t2dd;
        tech1Target2DirectDragPosition = new Vector3(t2px, t2py, 0f);
        if (t2dd == DirectDragStatus.across_from_screen_2
            || t2dd == DirectDragStatus.across_end_from_screen_2
            || t2dd == DirectDragStatus.drag_phase2_on_screen_2)
        {
            float rightBound = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0)).x;
            tech1Target1DirectDragPosition = new Vector3(t2px + rightBound * 2, t2py, 0f);
            refreshTarget1 = true;
        }
        tech1Target2DirectDragResult = t2result;
    }

    public void receiveHoldTapInfoFromClient(HoldTapStatus t2hp, HoldTapResult t2result)
    {
        tech2Target2HoldTapStatus = t2hp;
        tech2Target2HoldTapResult = t2result;
    }

    public void receiveThrowCatchInfoFromClinet(ThrowCatchStatus t2tc, float t2px, float t2py, ThrowCatchResult t2result)
    {
        tech3Target2ThrowCatchStatus = t2tc;
        tech3Target2ThrowCatchResult = t2result;
        if (t2tc == ThrowCatchStatus.throw_successed_on_screen_2)
        {
            float rightBound = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0)).x;
            tech3Target1ThrowCatchPosition = new Vector3(t2px + rightBound * 2, t2py, 0f);
        }
        else if (t2tc == ThrowCatchStatus.t1_move_phase2_ongoing)
        {
            float rightBound = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0)).x;
            tech3Target1ThrowCatchPosition = new Vector3(t2px + rightBound * 2, t2py, 0f);
        }
    }
    #endregion

    #region Public Write-file Method
    public void writeAllBlockConditionsToFile()
    {
        string allUserFileName = curLabInfos.labName.ToString() + "-"
            + curDragType.ToString() + "-"
            + curLabInfos.labMode.ToString() 
            + "-AllUsers.txt";
        string userFilename = curLabInfos.labName.ToString() + "-"
            + curDragType.ToString() + "-"
            + curLabInfos.labMode.ToString() + "-"
            + "User" + userid.ToString() + ".txt";
        DateTime dt = DateTime.Now;
        string strContent = dt.ToString() + Environment.NewLine
            + "All block conditions of user" + userid.ToString() + Environment.NewLine;
        //strContent += seqBlocks.getAllDataWithID(4);
        strContent += seqBlocks.getAllDataWithLabName();
        fileProcessor.writeNewDataToFile(allUserFileName, strContent);
        fileProcessor.writeNewDataToFile(userFilename, strContent);
    }

    public void writeCurrentBlockConditionToFile()
    {
        string userFilename = curLabInfos.labName.ToString() + "-"
            + curDragType.ToString() + "-"
            + curLabInfos.labMode.ToString() + "-"
            + "User" + userid.ToString() + ".txt";
        string strContent = Environment.NewLine + DateTime.Now.ToString() + Environment.NewLine;
        switch (curLabInfos.labName)
        {
            case LabName.Lab1_move_28:
                strContent += curBlockCondition.getAllDataForFile();
                break;
            default:
                break;
        }
        strContent += Environment.NewLine + string.Format("B{0:D2};", curBlockid) + getCurrentShapeAndAngle() + Environment.NewLine;
        fileProcessor.writeNewDataToFile(userFilename, strContent);
    }

    public void writeCurrentRepeatIndexTrialSequenceToFile()
    {
        string userFilename = curLabInfos.labName.ToString() + "-"
            + curDragType.ToString() + "-"
            + curLabInfos.labMode.ToString() + "-"
            + "User" + userid.ToString() + ".txt";
        string strContent = "";
        switch (curLabInfos.labName)
        {
            case LabName.Lab1_move_28:
                strContent = Environment.NewLine + curLabTrialSequence.getAllDataForFile();
                break;
            default:
                break;
        }
        fileProcessor.writeNewDataToFile(userFilename, strContent);
    }

    public void writeCurrentTrialDataToFile(out bool finishedWrite)
    {
        string userFilename = curLabInfos.labName.ToString() + "-"
            + curDragType.ToString() + "-"
            + curLabInfos.labMode.ToString() + "-"
            + "User" + userid.ToString() + ".txt";
        string strContent = "";
        switch (curLabInfos.labName)
        {
            case LabName.Lab1_move_28:
                curLabTrialData.conveyPhase1Data(curLabPhase1RawData);
                curLabTrialData.conveyPhase2Data(curLabPhase2RawData);
                strContent = curLabTrialData.getAllDataForFile();
                break;
            default:
                break;
        }
        strContent += getCurrentShapeAndAngle();

        fileProcessor.writeNewDataToFile(userFilename, strContent);
        finishedWrite = true;
    }

    public void writeAllBlocksFinishedFlagToFile()
    {
        string userFilename = curLabInfos.labName.ToString() + "-"
            + curDragType.ToString() + "-"
            + curLabInfos.labMode.ToString() + "-"
            + "User" + userid.ToString() + ".txt";
        string strContent = Environment.NewLine + DateTime.Now.ToString()
            + "~~AllBlocksFinished!~~" + Environment.NewLine;
        fileProcessor.writeNewDataToFile(userFilename, strContent);
    }

    public string getCurrentShapeAndAngle()
    {
        string res = "";
        switch (curLabInfos.labName)
        {
            case LabName.Lab1_move_28:
                res = curBlockCondition.getShape() + ";" + curAngle + ";";
                break;
            default:
                break;
        }
        return res;
    }
    #endregion
}
