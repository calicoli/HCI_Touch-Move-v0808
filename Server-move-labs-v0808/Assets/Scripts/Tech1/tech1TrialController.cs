using System;
using UnityEngine;
using static PublicInfo;
using static PublicLabParams;
using static PublicTrialParams;
using static PublicDragParams;
using System.Collections;
using System.Collections.Generic;

public class tech1TrialController : MonoBehaviour
{
    public tech1UIController uiController;

    public tech1PhaseController phaseController;
    public tech1DirectDragProcessor directDragProcessor;

    private ServerCenter sender;
    private bool isConnecting;
    private bool inExperiment;

    private int repeatTimes, totalTrialsPerRepeatition;
    //private bool inProtraitBlock;
    private Lab1_move_28.Posture blockPosture;

    private int curRepeatTime, curTrialIndex;
    private TrialPhase prevTrialPhase, curTrialPhase;

    //private bool clientSaidMoveon;
    //private bool haveObjectOnScreen;

    private TrialSequence[] trialSequences;
    private TrialSequence curSequence;
    private Trial curTrial;

    private int curTrialNumber;
    private int retryTrialCount;
    private Queue<Trial> retryTrialQueue;

    // Start is called before the first frame update
    void Start()
    {
        isConnecting = false;
        inExperiment = false;

        //init params with GloabalController
        sender = GlobalMemory.Instance.server;
        totalTrialsPerRepeatition = GlobalMemory.Instance.curLabInfos.totalTrialCount;
        repeatTimes = GlobalMemory.Instance.curLabInfos.labMode == LabMode.Full ?
            Lab1_move_28.fullRepetitionCount: Lab1_move_28.testRepetitionCount;
        //inProtraitBlock = (GlobalMemory.Instance.curLab0BlockCondition.getOrientation()
        //    == Lab1_move_28.Orientation.protrait);
        blockPosture = GlobalMemory.Instance.curBlockCondition.getPosture();
        curRepeatTime = 0;
        curTrialIndex = TRIAL_START_INDEX;
        curTrialNumber = TRIAL_START_INDEX;

        prevTrialPhase = TrialPhase.block_end;
        curTrialPhase = TrialPhase.block_start;
        //clientSaidMoveon = false;
        //haveObjectOnScreen = false;

        trialSequences = new TrialSequence[repeatTimes + 1];
        curSequence = new TrialSequence();

        retryTrialCount = 0;
        retryTrialQueue = new Queue<Trial>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (isConnecting && inExperiment
            && GlobalMemory.Instance
            && GlobalMemory.Instance.curDragType == DragType.direct_drag)
        {
            if (prevTrialPhase != curTrialPhase)
            {
                Debug.Log("TrialPhase: " + prevTrialPhase + "->" + curTrialPhase);
                prevTrialPhase = curTrialPhase;
                GlobalMemory.Instance.curLabTrialPhase = curTrialPhase;
            }

            if (curTrialPhase == TrialPhase.block_start)
            {
                curTrialPhase = TrialPhase.repeatition_start;
            }
            else if (curTrialPhase == TrialPhase.repeatition_start)
            {
                if (curRepeatTime < repeatTimes)
                {
                    curRepeatTime++;
                    GlobalMemory.Instance.curLabRepeatid = curRepeatTime;
                    curTrialPhase = TrialPhase.repeatition_scheduling;
                }
                else
                {
                    curTrialPhase = TrialPhase.block_end;
                }
            }
            else if (curTrialPhase == TrialPhase.repeatition_scheduling)
            {
                //targetVisualizer.hideTarget();

                int bid = GlobalMemory.Instance.curBlockid;
                int rid = curRepeatTime;
                string prefix = string.Format("B{0:D2}-R{1:D2}", bid, rid);

                curSequence.setTrialLength(GlobalMemory.Instance.curLabInfos.totalTrialCount);
                curSequence.setPrefix(prefix);
                curSequence.setAllQuence(blockPosture);
                trialSequences[curRepeatTime] = curSequence;
                GlobalMemory.Instance.curLabTrialSequence = curSequence;
                GlobalMemory.Instance.writeCurrentRepeatIndexTrialSequenceToFile();

                curTrialNumber = TRIAL_START_INDEX - 1;
                curTrialIndex = TRIAL_START_INDEX - 1;
                curTrialPhase = TrialPhase.a_trial_set_params;
            }
            else if (curTrialPhase == TrialPhase.a_trial_set_params)
            {
                curTrialNumber++;
                GlobalMemory.Instance.curLabTrialNumber = curTrialNumber;
                if (curTrialNumber <= GlobalMemory.Instance.curLabInfos.totalTrialCount)
                {
                    // trial index
                    curTrialIndex++;
                }
                else
                {
                    Trial trial = retryTrialQueue.Peek();
                    curTrialIndex = trial.index;
                    retryTrialQueue.Dequeue();
                }
                GlobalMemory.Instance.curLabTrialid = curTrialIndex;

                // set trial
                curTrial.setParams(curTrialIndex,
                    curSequence.seqTarget1[curTrialIndex - 1],
                    curSequence.seqTarget2[curTrialIndex - 1]);
                curTrial.printParams();
                GlobalMemory.Instance.curLabTrial = curTrial;

                // set trial data
                //GlobalMemory.Instance.curLabTrialData = new TrialDataWithLocalTime();
                //GlobalMemory.Instance.curLabTrialData.init(curTrial.index, curTrial.firstid, curTrial.secondid);
                int bid = GlobalMemory.Instance.curBlockid;
                int rid = curRepeatTime;
                int tid = curTrialIndex;
                int nid = curTrialNumber;
                string prefix = string.Format("B{0:D2}-R{1:D2}-T{2:D2}-N{3:D2}", bid, rid, tid, nid);
                //GlobalMemory.Instance.curLabTrialData.setPrefix(prefix);

                // test mode show params
                uiController.setTrialInfo(prefix, curTrial.firstid, curTrial.secondid);

                curTrialPhase = TrialPhase.s_sent_trial_params;
                sender.prepareNewMessage4Client(MessageType.Trial);

            }
            else if (curTrialPhase == TrialPhase.s_sent_trial_params)
            {
                if (GlobalMemory.Instance.clientLabTrialPhase == TrialPhase.c_received_trial_params)
                {
                    if (curTrial.firstid < curTrial.secondid)
                    {
                        GlobalMemory.Instance.lab1Target1Status = TargetStatus.total_on_screen_1;
                        directDragProcessor.GetComponent<tech1DirectDragProcessor>().initParamsWhenTargetOnScreen1(curTrial.firstid);
                        //curTrialPhase = TrialPhase.a_trial_start_from_1;
                    }
                    else
                    {
                        GlobalMemory.Instance.lab1Target1Status = TargetStatus.total_on_screen_2;
                        directDragProcessor.GetComponent<tech1DirectDragProcessor>().initParamsWhenTargetOnScreen2(curTrial.secondid);
                        //curTrialPhase = TrialPhase.a_trial_start_from_2;
                    }
                    directDragProcessor.GetComponent<tech1DirectDragProcessor>().enabled = true;
                    curTrialPhase = TrialPhase.a_trial_ongoing;
                }
            }
            else if (curTrialPhase == TrialPhase.a_trial_ongoing)
            {
                // wait user to do a trial
            }
            else if (curTrialPhase == TrialPhase.a_successful_trial)
            {
                directDragProcessor.GetComponent<tech1DirectDragProcessor>().enabled = false;
                curTrialPhase = TrialPhase.a_trial_output_data;
            }
            else if (curTrialPhase == TrialPhase.a_failed_trial)
            {
                directDragProcessor.GetComponent<tech1DirectDragProcessor>().enabled = false;
                retryTrialCount++;
                retryTrialQueue.Enqueue(curTrial);
                Debug.Log("RetryTrialCount/Queue: " + retryTrialCount.ToString() + "/" + retryTrialQueue.Count.ToString());
                curTrialPhase = TrialPhase.a_trial_output_data;
            }
            else if (curTrialPhase == TrialPhase.a_trial_output_data)
            {
                bool writeFinished = false;
                GlobalMemory.Instance.writeCurrentTrialDataToFile(out writeFinished);
                if (writeFinished)
                {
                    curTrialPhase = TrialPhase.a_trial_end;
                }
            }
            else if (curTrialPhase == TrialPhase.a_trial_end)
            {
                if (curTrialNumber >= totalTrialsPerRepeatition && retryTrialQueue.Count == 0)
                {
                    retryTrialCount = 0;
                    retryTrialQueue.Clear();
                    curTrialPhase = TrialPhase.repeatition_start;
                    Debug.Log("Repetition end.");
                }
                else
                {
                    curTrialPhase = TrialPhase.a_trial_set_params;
                }
            }
            else if (curTrialPhase == TrialPhase.block_end)
            {
                sender.prepareNewMessage4Client(MessageType.Trial);
                phaseController.moveToPhase(LabPhase.end_experiment);
            }
            else
            {
                Debug.LogError("Something bad happened.");
            }

            uiController.updateDragMode("Remain retry trial: " + retryTrialQueue.Count.ToString());
            uiController.updatePosInfo(curTrialPhase.ToString());
        }
        
    }

    private bool process1Touch4Target1(Vector2 pos, int targetid)
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