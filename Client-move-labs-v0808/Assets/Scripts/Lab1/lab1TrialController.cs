using System;
using UnityEngine;
using static PublicInfo;
using static PublicLabParams;
using static PublicTrialParams;
using static PublicDragParams;

public class lab1TrialController : MonoBehaviour
{
    public lab1UIController uiController;
    public lab1TargetVisualizer targetController;
    public lab1PhaseController phaseController;

    public tech1DirectDragProcessor directDragProcessor;
    public tech2HoldTapProcessor holdTapProcessor;
    public tech3ThrowCatchProcessor throwCatchProcessor;

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
        disableAllTechniques();
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
                if (GlobalMemory.Instance.serverLabTrialPhase == TrialPhase.s_sent_trial_params)
                {
                    curTrialNumber = GlobalMemory.Instance.curLabTrialNumber;
                    curTrialIndex = GlobalMemory.Instance.curLabTrialid;
                    curTrial = GlobalMemory.Instance.curLabTrial;
                    GlobalMemory.Instance.serverLabTrialPhase = TrialPhase.inactive_phase;

                    int tid = curTrialIndex;
                    int nid = curTrialNumber;
                    string prefix = string.Format("T{0:D2}-N{1:D2}", tid, nid);
                    //GlobalMemory.Instance.curLabTrialData.setPrefix(prefix);

                    if (GlobalMemory.Instance.targetDragType == DragType.direct_drag)
                    {
                        GlobalMemory.Instance.tech1TrialData = new tech1DirectDragTrialData();
                        GlobalMemory.Instance.tech1TrialData.init(curTrial.index, curTrial.firstid, curTrial.secondid);
                    }
                    else if (GlobalMemory.Instance.targetDragType == DragType.hold_tap)
                    {
                        GlobalMemory.Instance.tech2TrialData = new tech2HoldTapTrialData();
                        GlobalMemory.Instance.tech2TrialData.init(curTrial.index, curTrial.firstid, curTrial.secondid);
                    }
                    else if (GlobalMemory.Instance.targetDragType == DragType.throw_catch)
                    {
                        GlobalMemory.Instance.tech3TrialData = new tech3ThrowCatchTrialData();
                        GlobalMemory.Instance.tech3TrialData.init(curTrial.index, curTrial.firstid, curTrial.secondid);
                    }
                    uiController.updateUniqueInfo(GlobalMemory.Instance.curLabUniqueTid.ToString());

                    // test mode show params
                    uiController.setTrialInfo(prefix, curTrial.firstid, curTrial.secondid);

                    curTrialPhase = TrialPhase.c_received_trial_params;
                    GlobalMemory.Instance.curLabTrialPhase = TrialPhase.c_received_trial_params;
                    sender.prepareNewMessage4Server(MessageType.Trial);
                }
                else if( GlobalMemory.Instance.serverLabTrialPhase == TrialPhase.block_end)
                {
                    curTrialPhase = TrialPhase.block_end;
                    GlobalMemory.Instance.serverLabTrialPhase = TrialPhase.inactive_phase;
                }
            }
            else if (curTrialPhase == TrialPhase.c_received_trial_params)
            {
                if (curTrial.firstid < curTrial.secondid)
                {
                    GlobalMemory.Instance.curLabPhase2RawData = new TrialPhase2RawData();
                    GlobalMemory.Instance.curLabPhase2RawData.init(curTrial.index, curTrial.firstid, curTrial.secondid);
                    GlobalMemory.Instance.lab1Target2Status = TargetStatus.total_on_screen_1;
                    switch (GlobalMemory.Instance.targetDragType)
                    {
                        case DragType.direct_drag:
                            directDragProcessor.initParamsWhenTargetOnScreen1(curTrial.secondid);
                            directDragProcessor.GetComponent<tech1DirectDragProcessor>().enabled = true;
                            break;
                        case DragType.hold_tap:
                            holdTapProcessor.initParamsWhenTargetOnScreen1(curTrial.secondid);
                            holdTapProcessor.GetComponent<tech2HoldTapProcessor>().enabled = true;
                            break;
                        case DragType.throw_catch:
                            throwCatchProcessor.initParamsWhenTargetOnScreen1(curTrial.secondid);
                            throwCatchProcessor.GetComponent<tech3ThrowCatchProcessor>().enabled = true;
                            break;
                    }
                    
                    //curTrialPhase = TrialPhase.a_trial_start_from_1;
                }
                else
                {
                    GlobalMemory.Instance.curLabPhase1RawData = new TrialPhase1RawData();
                    GlobalMemory.Instance.curLabPhase1RawData.init(curTrial.index, curTrial.firstid, curTrial.secondid);
                    GlobalMemory.Instance.lab1Target2Status = TargetStatus.total_on_screen_2;
                    switch (GlobalMemory.Instance.targetDragType)
                    {
                        case DragType.direct_drag:
                            directDragProcessor.initParamsWhenTargetOnScreen2(curTrial.firstid);
                            directDragProcessor.GetComponent<tech1DirectDragProcessor>().enabled = true;
                            break;
                        case DragType.hold_tap:
                            holdTapProcessor.initParamsWhenTargetOnScreen2(curTrial.firstid);
                            holdTapProcessor.GetComponent<tech2HoldTapProcessor>().enabled = true;
                            break;
                        case DragType.throw_catch:
                            throwCatchProcessor.initParamsWhenTargetOnScreen2(curTrial.firstid);
                            throwCatchProcessor.GetComponent<tech3ThrowCatchProcessor>().enabled = true;
                            break;
                    }
                    //curTrialPhase = TrialPhase.a_trial_start_from_2;
                }
                curTrialPhase = TrialPhase.a_trial_ongoing;
            }
            else if (curTrialPhase == TrialPhase.a_trial_ongoing)
            {
                // wait user to do a trial
            }
            else if (curTrialPhase == TrialPhase.a_successful_trial
                || curTrialPhase == TrialPhase.a_failed_trial)
            {
                disableAllTechniques();
                curTrialPhase = TrialPhase.a_trial_end;
                GlobalMemory.Instance.curLabTrialPhase = TrialPhase.a_trial_end;
                sender.prepareNewMessage4Server(MessageType.Trial);
            }
            else if (curTrialPhase == TrialPhase.a_trial_end)
            {
                curTrialPhase = TrialPhase.inactive_phase;
            }
            else if (curTrialPhase == TrialPhase.block_end)
            {
                phaseController.moveToPhase(LabPhase.end_experiment);
            }
            else
            {
                Debug.LogError("Something bad happened: ");
            }
            //uiController.updatePosInfo(curTrialPhase.ToString());
            string dd = directDragProcessor.GetComponent<tech1DirectDragProcessor>().isActiveAndEnabled ? "T" : "F";
            string ht = holdTapProcessor.GetComponent<tech2HoldTapProcessor>().isActiveAndEnabled ? "T" : "F";
            string tc = throwCatchProcessor.GetComponent<tech3ThrowCatchProcessor>().isActiveAndEnabled ? "T" : "F";
            uiController.updateDragInfo(dd + ht + tc + " " + GlobalMemory.Instance.lab1Target2Status.ToString());
            uiController.updatePhaseInfo(curTrialPhase.ToString());
        }
    }

    private void disableAllTechniques ()
    {
        directDragProcessor.GetComponent<tech1DirectDragProcessor>().enabled = false;
        holdTapProcessor.GetComponent<tech2HoldTapProcessor>().enabled = false;
        throwCatchProcessor.GetComponent<tech3ThrowCatchProcessor>().enabled = false;
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
