using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static PublicLabFactors;
using static PublicDragParams;

public class GlobalController : MonoBehaviour
{
    public static GlobalController Instance;

    public ServerController server;

    [HideInInspector]
    public string serverip;
    [HideInInspector]
    public ServerCommand curServerCommand;
    [HideInInspector]
    public LabScene curServerScene, curClientScene;

    [HideInInspector]
    public DragType demoDragType;
    [HideInInspector]
    public bool refreshTarget1 = false;

    [HideInInspector]
    public TargetStatus demoTarget1Status = TargetStatus.total_on_screen_1;
    [HideInInspector]
    public DirectDragStatus demoTarget1DirectDragStatus, demoTarget2DirectDragStatus;
    [HideInInspector]
    public Vector3 demoTarget1DirectDragPosition = Vector3.zero, demoTarget2DirectDragPosition;
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
    public void setConnectingStatus(bool con)
    {
        isConnecting = con;
    }

    public bool getConnectionStatus()
    {
        return isConnecting;
    }

    public void excuteCommand(ServerCommand cmd)
    {
        curServerCommand = cmd;
        server.prepareNewMessage4Client(MessageType.Command, cmd);
    }

    public void receiveDirectDragInfoFromClient(DirectDragStatus t2dd, float t2px, float t2py)
    {
        demoTarget2DirectDragStatus = t2dd;
        demoTarget2DirectDragPosition = new Vector3(t2px, t2py, 0f);
        if (t2dd == DirectDragStatus.across_from_screen_2
            || t2dd == DirectDragStatus.across_end_from_screen_2
            || t2dd == DirectDragStatus.drag_phase2_on_screen_2)
        {
            float rightBound = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0)).x;
            demoTarget1DirectDragPosition = new Vector3(t2px + rightBound * 2, t2py, 0f);
            refreshTarget1 = true;
        }
    }

    public void receiveHoldTapInfoFromClient ( HoldTapStatus t2hp )
    {
        demoTarget2HoldTapStatus = t2hp;
    }

    public void receiveThrowCatchInfoFromClinet ( ThrowCatchStatus t2tc, float t2px, float t2py )
    {
        demoTarget2ThrowCatchStatus = t2tc;
        if ( t2tc == ThrowCatchStatus.throw_successed_on_screen_2 )
        {
            float rightBound = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0)).x;
            demoTarget1ThrowCatchPosition = new Vector3(t2px + rightBound * 2, t2py, 0f);
        }
        else if ( t2tc == ThrowCatchStatus.t1_move_phase2_ongoing )
        {
            float rightBound = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0)).x;
            demoTarget1ThrowCatchPosition = new Vector3(t2px + rightBound * 2, t2py, 0f);
        }
    }
    #endregion
}
