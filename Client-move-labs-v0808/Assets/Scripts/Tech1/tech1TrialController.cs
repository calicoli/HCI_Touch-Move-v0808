using System;
using UnityEngine;
using static PublicInfo;
using static PublicLabParams;
using static PublicTrialParams;
using static PublicDragParams;

public class tech1TrialController : MonoBehaviour
{
    public tech1UIController uiController;
    public tech1TargetVisualizer targetController;

    public tech1PhaseController phaseController;
    public tech1DirectDragProcessor directDragProcessor;

    private ClientCenter sender;
    private bool isConnecting;
    private bool inExperiment;

    private int curTrialNumber, curTrialIndex;
    private TrialPhase curTrialPhase, prevTrialPhase;

    private bool serverRefreshed;
    //private bool haveObjectOnScreen;

    private Trial curTrial;
    private TrialDataWithLocalTime trialData;

    // Start is called before the first frame update
    void Start()
    {
        isConnecting = false;
        inExperiment = false;

        // init params with GlobalMemory
        sender = GlobalMemory.Instance.client;

        curTrialIndex = TRIAL_START_INDEX;
        curTrialNumber = TRIAL_START_INDEX;

        
        prevTrialPhase = TrialPhase.block_end;
        curTrialPhase = TrialPhase.inactive_phase;
        serverRefreshed = false;
        //haveObjectOnScreen = false;

        curTrial = new Trial();
        GlobalMemory.Instance.curLabTrial = new Trial();
    }

    // Update is called once per frame
    void Update()
    {
        serverRefreshed = GlobalMemory.Instance.serverRefreshTrialData;
        if (isConnecting & inExperiment)
        {
            if (prevTrialPhase != curTrialPhase)
            {
                Debug.Log("TrialPhase: " + prevTrialPhase + "->" + curTrialPhase);
                prevTrialPhase = curTrialPhase;
                GlobalMemory.Instance.curLabTrialPhase = curTrialPhase;
            }

            if (curTrialPhase == TrialPhase.inactive_phase)
            {
                if (GlobalMemory.Instance.serverLabTrialPhase == TrialPhase.a_trial_set_params)
                {
                    curTrialNumber = GlobalMemory.Instance.curLabTrialNumber;
                    curTrialIndex = GlobalMemory.Instance.curLabTrialid;
                    curTrial = GlobalMemory.Instance.curLabTrial;
                    GlobalMemory.Instance.serverLabTrialPhase = TrialPhase.inactive_phase;

                    curTrialPhase = TrialPhase.c_received_trial_params;
                    GlobalMemory.Instance.curLabTrialPhase = TrialPhase.c_received_trial_params;
                    sender.prepareNewMessage4Server(MessageType.Trial);
                } else if( GlobalMemory.Instance.serverLabTrialPhase == TrialPhase.block_end)
                {
                    curTrialPhase = TrialPhase.block_end;
                    GlobalMemory.Instance.serverLabTrialPhase = TrialPhase.inactive_phase;
                }
            }
            else if (curTrialPhase == TrialPhase.c_received_trial_params)
            {
                if (curTrial.firstid < curTrial.secondid)
                {
                    GlobalMemory.Instance.lab1Target2Status = TargetStatus.total_on_screen_1;
                    directDragProcessor.GetComponent<tech1DirectDragProcessor>().initParamsWhenTargetOnScreen1(curTrial.secondid);
                    //curTrialPhase = TrialPhase.a_trial_start_from_1;
                }
                else
                {
                    GlobalMemory.Instance.lab1Target2Status = TargetStatus.total_on_screen_2;
                    directDragProcessor.GetComponent<tech1DirectDragProcessor>().initParamsWhenTargetOnScreen2(curTrial.firstid);
                    //curTrialPhase = TrialPhase.a_trial_start_from_2;
                }
                directDragProcessor.GetComponent<tech1DirectDragProcessor>().enabled = true;
                curTrialPhase = TrialPhase.a_trial_ongoing;
            }
            else if (curTrialPhase == TrialPhase.a_trial_ongoing)
            {
                // wait user to do a trial
            }
            else if (curTrialPhase == TrialPhase.a_successful_trial
                || curTrialPhase == TrialPhase.a_failed_trial)
            {
                directDragProcessor.GetComponent<tech1DirectDragProcessor>().enabled = false;
                curTrialPhase = TrialPhase.inactive_phase;
            }
            else if (curTrialPhase == TrialPhase.a_trial_end)
            {
                // omit this phase
            }
            else if (curTrialPhase == TrialPhase.block_end)
            {
                phaseController.moveToPhase(LabPhase.end_experiment);
            }
            else
            {
                Debug.LogError("Something bad happened: ");
            }
            uiController.updatePosInfo(curTrialPhase.ToString());
        }
    }

    private bool process1Touch4Target2(Vector2 pos, int targetid)
    {
        int hitid = -1;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(pos);
        if (Physics.Raycast(ray, out hit))
        {
            hitid = Convert.ToInt32(hit.collider.gameObject.name.Substring(9, 2));
            Debug.Log("info: " + hitid.ToString() + " " + hit.collider.gameObject.name);
            Debug.DrawLine(ray.origin, hit.point, Color.yellow);
        }

        if (hitid == targetid)
            return true;
        else
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
    public void setExperimentStatus(bool working)
    {
        inExperiment = working;
    }

    public void setConnectionStatus(bool con)
    {
        if (con != isConnecting)
        {
            isConnecting = con;
        }
    }

    public void switchTrialPhase(TrialPhase ph)
    {
        curTrialPhase = ph;
    }
    #endregion
}
