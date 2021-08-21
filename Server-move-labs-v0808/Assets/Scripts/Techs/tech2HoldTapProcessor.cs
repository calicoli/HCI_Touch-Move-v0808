using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PublicInfo;
using static PublicDragParams;
using System;

public class tech2HoldTapProcessor : MonoBehaviour
{
    public lab1UIController uiController;
    public lab1TrialController trialController;
    public lab1TargetVisualizer targetVisualizer;

    private HoldTapStatus curTarget1HoldTapStatus, prevTarget1HoldTapStatus;
    private bool touchSuccess;
    private HoldTapResult curHoldTapResult;

    private Vector2 holdStartPos, holdRealtimePos;

    private float delayTimer = 0f;
    private const float wait_time_before_vanish = 0.15f;

    private bool haveRecordedStamp = false;
    private bool retryStatusActive = false;

    // Start is called before the first frame update
    void Start()
    {
        resetHoldTapParams();
    }

    void resetHoldTapParams()
    {
        delayTimer = wait_time_before_vanish;
        holdStartPos = holdRealtimePos = Vector2.zero;
        retryStatusActive = false;
        haveRecordedStamp = false;
        touchSuccess = false;
        curHoldTapResult = HoldTapResult.hold_tap_success;
        if (GlobalMemory.Instance)
        {
            GlobalMemory.Instance.tech2Target1HoldTapResult
                = GlobalMemory.Instance.tech2Target2HoldTapResult
                = HoldTapResult.hold_tap_success;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GlobalMemory.Instance && GlobalMemory.Instance.curDragType == DragType.hold_tap)
        {
            if (curHoldTapResult != HoldTapResult.hold_tap_success
                || GlobalMemory.Instance.tech2Target2HoldTapResult != HoldTapResult.hold_tap_success)
            {
                Debug.Log("Trial s failed: " + curHoldTapResult.ToString());
                Debug.Log("Trial c failed: " + GlobalMemory.Instance.tech2Target2HoldTapResult.ToString());
                uiController.updatePosInfo(curHoldTapResult.ToString());
                HoldTapResult thisResult = curHoldTapResult != HoldTapResult.hold_tap_success ?
                    curHoldTapResult : GlobalMemory.Instance.tech2Target2HoldTapResult;
                GlobalMemory.Instance.curLabTrialData.techResult = thisResult.ToString();
                switch (thisResult)
                {
                    case HoldTapResult.hold_outside_before_tap:
                    case HoldTapResult.hold_released_before_tap:
                    case HoldTapResult.user_skip_current_trial:
                        GlobalMemory.Instance.curLabTrialData.setTrialAccuracy(false, false);
                        break;
                    case HoldTapResult.tap_failed_to_arrive_pos:
                        GlobalMemory.Instance.curLabTrialData.setTrialAccuracy(true, false);
                        break;
                }
                if (GlobalMemory.Instance.lab1Target1Status == TargetStatus.total_on_screen_1)
                {
                    GlobalMemory.Instance.curLabTrialData.device1TechResult = curHoldTapResult.ToString();
                    GlobalMemory.Instance.curLabTrialData.device2TechResult = GlobalMemory.Instance.tech2Target2HoldTapResult.ToString();
                }
                else if (GlobalMemory.Instance.lab1Target1Status == TargetStatus.total_on_screen_2)
                {
                    GlobalMemory.Instance.curLabTrialData.device1TechResult = GlobalMemory.Instance.tech2Target2HoldTapResult.ToString();
                    GlobalMemory.Instance.curLabTrialData.device2TechResult = curHoldTapResult.ToString();
                }
                targetVisualizer.wrongTarget();
                if (delayTimer > 0f)
                {
                    delayTimer -= Time.deltaTime;
                }
                else
                {
                    targetVisualizer.hideTarget();
                    targetVisualizer.hideShadow();
                    trialController.switchTrialPhase(PublicTrialParams.TrialPhase.a_failed_trial);
                }
            }

            if (Input.touchCount == 4)
            {
                curHoldTapResult = HoldTapResult.user_skip_current_trial;
                curTarget1HoldTapStatus = HoldTapStatus.retry_status_active;
                retryStatusActive = true;
            }

            if (GlobalMemory.Instance.lab1Target1Status == TargetStatus.total_on_screen_1)
            {
                if (curTarget1HoldTapStatus == HoldTapStatus.inactive_on_screen_1)
                {
                    if (Input.touchCount == 1)
                    {
                        Touch touch = Input.GetTouch(0);
                        touchSuccess = process1Touch4Target1(touch.position, 0);
                        if (touchSuccess && touch.phase == TouchPhase.Began)
                        {
                            GlobalMemory.Instance.curLabPhase1RawData.touch1StartStamp 
                                = GlobalMemory.Instance.curLabPhase1RawData.targetReachMidpointStamp = CurrentTimeMillis();
                            GlobalMemory.Instance.curLabPhase1RawData.touch1StartPos = touch.position;
                            GlobalMemory.Instance.tech2TrialData.holdStartPosition = touch.position;
                            holdStartPos = touch.position;
                            targetVisualizer.activeTarget();
                            curTarget1HoldTapStatus = HoldTapStatus.holding_on_screen_1;
                        }
                    }
                    else
                    {
                        touchSuccess = false;
                    }
                }
                else if (curTarget1HoldTapStatus == HoldTapStatus.holding_on_screen_1)
                {
                    if (GlobalMemory.Instance.tech2Target2HoldTapStatus == HoldTapStatus.tapped_on_screen_2
                    || GlobalMemory.Instance.tech2Target2HoldTapStatus == HoldTapStatus.wait_s1_to_received_tap
                    || GlobalMemory.Instance.tech2Target2HoldTapStatus == HoldTapStatus.t1tot2_trial_failed)
                    {
                        targetVisualizer.hideTarget();
                        targetVisualizer.hideShadow();
                        curTarget1HoldTapStatus = HoldTapStatus.tapped_on_screen_2;
                    }

                    else if (Input.touchCount == 1)
                    {
                        Touch touch = Input.GetTouch(0);
                        GlobalMemory.Instance.curLabPhase1RawData.touch1EndStamp = CurrentTimeMillis();
                        GlobalMemory.Instance.curLabPhase1RawData.touch1EndPos = touch.position;
                        GlobalMemory.Instance.curLabPhase1RawData.movePhase1EndPos = targetVisualizer.getTargetScreenPosition();
                        bool touchSuccess = process1Touch4Target1(touch.position, 0);
                        if (touchSuccess)
                        {
                            holdRealtimePos = touch.position;
                            calculateHoldOffset();
                            if (touch.phase == TouchPhase.Ended)
                            {
                                if (GlobalMemory.Instance.tech2Target2HoldTapStatus == HoldTapStatus.inactive_on_screen_1)
                                {
                                    targetVisualizer.inactiveTarget();
                                    targetVisualizer.wrongTarget();
                                    curHoldTapResult = HoldTapResult.hold_released_before_tap;
                                    curTarget1HoldTapStatus = HoldTapStatus.t1tot2_trial_failed;
                                }
                            }
                        }
                        else
                        {
                            targetVisualizer.inactiveTarget();
                            targetVisualizer.wrongTarget();
                            curHoldTapResult = HoldTapResult.hold_outside_before_tap;
                            curTarget1HoldTapStatus = HoldTapStatus.t1tot2_trial_failed;
                        }
                    }
                    else
                    {
                        touchSuccess = false;
                        targetVisualizer.inactiveTarget();
                        targetVisualizer.wrongTarget();
                        GlobalMemory.Instance.curLabPhase1RawData.touch1EndStamp = CurrentTimeMillis();
                        curHoldTapResult = HoldTapResult.hold_released_before_tap;
                        curTarget1HoldTapStatus = HoldTapStatus.t1tot2_trial_failed;
                    }
                }
                else if (curTarget1HoldTapStatus == HoldTapStatus.tapped_on_screen_2)
                {
                    if (Input.touchCount == 1 && (
                        Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Stationary
                        || Input.GetTouch(0).phase == TouchPhase.Ended) )
                    {
                        GlobalMemory.Instance.curLabPhase1RawData.touch1EndStamp = CurrentTimeMillis();
                        GlobalMemory.Instance.curLabPhase1RawData.touch1EndPos = Input.GetTouch(0).position;
                        GlobalMemory.Instance.curLabPhase1RawData.movePhase1EndPos = targetVisualizer.getTargetScreenPosition();
                    }
                    if (GlobalMemory.Instance.tech2Target2HoldTapStatus == HoldTapStatus.tap_correct_on_screen_2)
                    {
                        uiController.updatePosInfo(curHoldTapResult.ToString());
                        GlobalMemory.Instance.curLabPhase1RawData.targetReachEndpointInfoReceivedStamp = CurrentTimeMillis();
                        GlobalMemory.Instance.curLabTrialData.setTrialAccuracy(true, true);
                        GlobalMemory.Instance.curLabTrialData.techResult = curHoldTapResult.ToString();
                        trialController.switchTrialPhase(PublicTrialParams.TrialPhase.a_successful_trial);
                    }
                }
            }
            else if (GlobalMemory.Instance.lab1Target1Status == TargetStatus.total_on_screen_2)
            {
                if ( (GlobalMemory.Instance.tech2Target2HoldTapStatus == HoldTapStatus.holding_on_screen_2
                    || GlobalMemory.Instance.tech2Target2HoldTapStatus == HoldTapStatus.tapped_on_screen_1)
                    && (curTarget1HoldTapStatus == HoldTapStatus.inactive_on_screen_2
                     || curTarget1HoldTapStatus == HoldTapStatus.tapped_on_screen_1))
                {
                    if (GlobalMemory.Instance.tech2Target2HoldTapStatus == HoldTapStatus.holding_on_screen_2
                        && !haveRecordedStamp)
                    {
                        GlobalMemory.Instance.curLabPhase2RawData.targetReachMidpointInfoReceivedStamp = CurrentTimeMillis();
                        haveRecordedStamp = true;
                    }

#if UNITY_ANDROID && UNITY_EDITOR
                    if (Input.GetMouseButtonDown(0))
                    {
                        Vector3 intentPos = processScreenPosToGetWorldPosAtZeroZ(Input.mousePosition);
                        targetVisualizer.moveTarget(intentPos);
                        targetVisualizer.activeTarget();
                        targetVisualizer.showTarget();
                        GlobalMemory.Instance.curLabPhase2RawData.touch2StartStamp = CurrentTimeMillis();
                        GlobalMemory.Instance.curLabPhase2RawData.touch2StartPos = Input.mousePosition;
                        GlobalMemory.Instance.curLabPhase2RawData.movePhase2StartPos = targetVisualizer.getTargetScreenPosition();
                        curTarget1HoldTapStatus = HoldTapStatus.tapped_on_screen_1;
                    }
                    else if (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
                    {
                        Vector3 intentPos = processScreenPosToGetWorldPosAtZeroZ(Input.mousePosition);
                        targetVisualizer.moveTarget(intentPos);
                        GlobalMemory.Instance.curLabPhase2RawData.touch2EndStamp 
                            = GlobalMemory.Instance.curLabPhase2RawData.targetReachEndpointStamp = CurrentTimeMillis();
                        GlobalMemory.Instance.curLabPhase2RawData.touch2EndPos = Input.mousePosition;
                        GlobalMemory.Instance.curLabPhase2RawData.movePhase2EndPos = targetVisualizer.getTargetScreenPosition();
                        
                        if(Input.GetMouseButtonUp(0))
                        {
                            if (checkTouchEndPosCorrect())
                            {
                                targetVisualizer.correctTarget();
                                curTarget1HoldTapStatus = HoldTapStatus.wait_s2_to_received_tap;
                            }
                            else
                            {
                                targetVisualizer.wrongTarget();
                                curHoldTapResult = HoldTapResult.tap_failed_to_arrive_pos;
                                curTarget1HoldTapStatus = HoldTapStatus.t2tot1_trial_failed;
                            }
                        }
                    }
#elif UNITY_IOS || UNITY_ANDROID
                    if ( Input.touchCount == 1)
                    {
                        Touch touch = Input.GetTouch(0);
                        if (touch.phase == TouchPhase.Began)
                        {
                            Vector3 intentPos = processScreenPosToGetWorldPosAtZeroZ(touch.position);
                            targetVisualizer.moveTarget(intentPos);
                            targetVisualizer.activeTarget();
                            targetVisualizer.showTarget();
                            GlobalMemory.Instance.curLabPhase2RawData.touch2StartStamp = CurrentTimeMillis();
                            GlobalMemory.Instance.curLabPhase2RawData.touch2StartPos = touch.position;
                            GlobalMemory.Instance.curLabPhase2RawData.movePhase2StartPos = targetVisualizer.getTargetScreenPosition();
                            curTarget1HoldTapStatus = HoldTapStatus.tapped_on_screen_1;
                        }
                        else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Ended)
                        {
                            Vector3 intentPos = processScreenPosToGetWorldPosAtZeroZ(touch.position);
                            targetVisualizer.moveTarget(intentPos);
                            GlobalMemory.Instance.curLabPhase2RawData.touch2EndStamp 
                                = GlobalMemory.Instance.curLabPhase2RawData.targetReachEndpointStamp = CurrentTimeMillis();
                            GlobalMemory.Instance.curLabPhase2RawData.touch2EndPos = touch.position;
                            GlobalMemory.Instance.curLabPhase2RawData.movePhase2EndPos = targetVisualizer.getTargetScreenPosition();
                            
                            if (touch.phase == TouchPhase.Ended)
                            {
                                if (checkTouchEndPosCorrect())
                                {
                                    targetVisualizer.correctTarget();
                                    curTarget1HoldTapStatus = HoldTapStatus.wait_s2_to_received_tap;
                                }
                                else
                                {
                                    targetVisualizer.wrongTarget();
                                    curHoldTapResult = HoldTapResult.tap_failed_to_arrive_pos;
                                    curTarget1HoldTapStatus = HoldTapStatus.t2tot1_trial_failed;
                                }
                            }
                        }
                    }
#endif
                }
                else if (curTarget1HoldTapStatus == HoldTapStatus.wait_s2_to_received_tap)
                {
                    if (GlobalMemory.Instance.tech2Target2HoldTapStatus == HoldTapStatus.tapped_on_screen_1)
                    {
                        curTarget1HoldTapStatus = HoldTapStatus.tap_correct_on_screen_1;
                    }
                }
                else if (curTarget1HoldTapStatus == HoldTapStatus.tap_correct_on_screen_1)
                {
                    if (delayTimer > 0f)
                    {
                        delayTimer -= Time.deltaTime;
                    }
                    else
                    {
                        targetVisualizer.hideTarget();
                        targetVisualizer.hideShadow();
                        uiController.updatePosInfo(curHoldTapResult.ToString());
                        GlobalMemory.Instance.curLabTrialData.setTrialAccuracy(true, true);
                        GlobalMemory.Instance.curLabTrialData.techResult = curHoldTapResult.ToString();
                        trialController.switchTrialPhase(PublicTrialParams.TrialPhase.a_successful_trial);
                    }
                }
                /*
                else if (GlobalMemory.Instance.tech2Target2HoldTapStatus == HoldTapStatus.t2tot1_trial_failed)
                {
                    targetVisualizer.wrongTarget();
                    //targetVisualizer.hideTarget();
                    //targetVisualizer.hideShadow();
                    curHoldTapResult = GlobalMemory.Instance.tech2Target2HoldTapResult;
                    curTarget1HoldTapStatus = HoldTapStatus.t2tot1_trial_failed;
                }
                */
            }

            GlobalMemory.Instance.tech2Target1HoldTapStatus = curTarget1HoldTapStatus;
            GlobalMemory.Instance.tech2Target1HoldTapResult = curHoldTapResult;
            if (retryStatusActive
                && curTarget1HoldTapStatus == HoldTapStatus.retry_status_active
                && curHoldTapResult == HoldTapResult.user_skip_current_trial)
            {
                retryStatusActive = false;
                GlobalMemory.Instance.server.prepareNewMessage4Client(MessageType.HoldTapInfo);
            }
            else if (curTarget1HoldTapStatus != prevTarget1HoldTapStatus
                && curTarget1HoldTapStatus != HoldTapStatus.retry_status_active
                && curTarget1HoldTapStatus != HoldTapStatus.holding_on_screen_2
                && curTarget1HoldTapStatus != HoldTapStatus.wait_s1_to_received_tap)
            {
                GlobalMemory.Instance.server.prepareNewMessage4Client(MessageType.HoldTapInfo);
            }
            prevTarget1HoldTapStatus = curTarget1HoldTapStatus;

            uiController.updateDebugInfo(curTarget1HoldTapStatus.ToString());
            uiController.updateStatusInfo(GlobalMemory.Instance.tech2Target2HoldTapStatus.ToString());
            //uiController.updatePosInfo(curHoldTapResult.ToString());
        }
    }

    private void calculateHoldOffset()
    {
        float distance = Vector2.Distance(holdStartPos, holdRealtimePos);
        if (distance != 0 && distance > GlobalMemory.Instance.tech2TrialData.maxOffset)
        {
            GlobalMemory.Instance.tech2TrialData.maxOffset = distance;
            GlobalMemory.Instance.tech2TrialData.holdMaxOffsetPosition = holdRealtimePos;
            GlobalMemory.Instance.curLabTrialData.techData = GlobalMemory.Instance.tech2TrialData.getAllData();
        }
        if (distance != 0 && distance < GlobalMemory.Instance.tech2TrialData.minOffset)
        {
            GlobalMemory.Instance.tech2TrialData.minOffset = distance;
            GlobalMemory.Instance.tech2TrialData.holdMinOffsetPosition = holdRealtimePos;
            GlobalMemory.Instance.curLabTrialData.techData = GlobalMemory.Instance.tech2TrialData.getAllData();
        }
    }

    private long CurrentTimeMillis()
    {
        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        TimeSpan diff = DateTime.Now.ToUniversalTime() - origin;
        return (long)diff.TotalMilliseconds;
    }

    private bool process1Touch4Target1(Vector2 pos, int targetid)
    {
        
        int hitid = -1;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(pos);
        if (Physics.Raycast(ray, out hit))
        {
            hitid = Convert.ToInt32(hit.collider.gameObject.name.Substring(7, 2));
            Debug.Log("info: " + hitid.ToString() + " " + hit.collider.gameObject.name);
            Debug.DrawLine(ray.origin, hit.point, Color.yellow);
        }

        if (hitid == targetid)
            return true;
        else
            return false;
        /*
        float distance = Vector3.Distance(targetVisualizer.getTargetPosition(), processScreenPosToGetWorldPosAtZeroZ(pos));
        if (distance <= targetVisualizer.getShadowLocalScale().x / 2)
        {
            return true;
        }
        else
        {
            return false;
        }*/
    }

    private Vector3 processScreenPosToGetWorldPosAtZeroZ(Vector2 tp)
    {
        Vector3 pos = Vector3.zero;
        pos = Camera.main.ScreenToWorldPoint(new Vector3(tp.x, tp.y, 0));
        pos.z = 0f;
        return pos;
    }

    private bool checkTouchEndPosCorrect()
    {
        bool res = false;
        float distance = Vector3.Distance(targetVisualizer.getTargetPosition(), targetVisualizer.getShadowPosition());
        if (distance <= targetVisualizer.getShadowLocalScale().x / 2)
        {
            res = true;
        }
        return res;
    }

    public void initParamsWhenTargetOnScreen1(int firstid)
    {
        Debug.Log("tech2-initS1()");
        targetVisualizer.moveTargetWithPosID(firstid);
        targetVisualizer.showTarget();
        targetVisualizer.hideShadow();
        prevTarget1HoldTapStatus = curTarget1HoldTapStatus = HoldTapStatus.inactive_on_screen_1;
        if (GlobalMemory.Instance)
        {
            GlobalMemory.Instance.curLabPhase1RawData.moveStartPos
                = GlobalMemory.Instance.curLabPhase1RawData.movePhase1StartPos
                = targetVisualizer.getTargetScreenPosition();
            GlobalMemory.Instance.tech2Target1HoldTapStatus
                = GlobalMemory.Instance.tech2Target2HoldTapStatus
                = HoldTapStatus.inactive_on_screen_1;
        }
        resetHoldTapParams();
    }
    public void initParamsWhenTargetOnScreen2(int secondid)
    {
        Debug.Log("tech2-initS2()");
        targetVisualizer.moveShadowWithPosID(secondid);
        targetVisualizer.hideTarget();
        targetVisualizer.showShadow();
        prevTarget1HoldTapStatus = curTarget1HoldTapStatus = HoldTapStatus.inactive_on_screen_2;
        if (GlobalMemory.Instance)
        {
            GlobalMemory.Instance.curLabPhase2RawData.moveDestination = targetVisualizer.getShadowScreenPosition();
            GlobalMemory.Instance.tech2Target1HoldTapStatus
                = GlobalMemory.Instance.tech2Target2HoldTapStatus
                = HoldTapStatus.inactive_on_screen_2;
        }
        resetHoldTapParams();
    }
}
