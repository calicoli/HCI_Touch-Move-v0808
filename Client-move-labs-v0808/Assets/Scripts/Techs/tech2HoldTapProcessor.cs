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

    private HoldTapStatus curTarget2HoldTapStatus, prevTarget2HoldTapStatus;
    private bool touchSuccess;
    private HoldTapResult curHoldTapResult;

    private const int layerMask = 1 << 8;

    private int hitCnt = 0;

    private float delayTimer = 0f;
    private const float wait_time_before_vanish = 0.2f;

    private bool haveRecordedStamp = false;

    // Start is called before the first frame update
    void Start()
    {
        resetHoldTapParams();
    }

    void resetHoldTapParams()
    {
        delayTimer = wait_time_before_vanish;
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
        if (GlobalMemory.Instance && GlobalMemory.Instance.targetDragType == DragType.hold_tap)
        {
            uiController.updateDragMode("");
            if (curHoldTapResult != HoldTapResult.hold_tap_success
                || GlobalMemory.Instance.tech2Target1HoldTapResult != HoldTapResult.hold_tap_success)
            {
                Debug.Log("Trial c failed: " + curHoldTapResult.ToString());
                Debug.Log("Trial s failed: " + GlobalMemory.Instance.tech2Target1HoldTapResult.ToString());
                uiController.updatePosInfo(curHoldTapResult.ToString());
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

            if (GlobalMemory.Instance.lab1Target2Status == TargetStatus.total_on_screen_1)
            {
                if (GlobalMemory.Instance.tech2Target1HoldTapStatus == HoldTapStatus.holding_on_screen_1
                    && (curTarget2HoldTapStatus == HoldTapStatus.inactive_on_screen_1
                     || curTarget2HoldTapStatus == HoldTapStatus.tapped_on_screen_2))
                {
                    if (GlobalMemory.Instance.tech2Target1HoldTapStatus == HoldTapStatus.holding_on_screen_1
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
                        GlobalMemory.Instance.curLabPhase2RawData.touch2StartStamp = CurrentTimeMillis();
                        GlobalMemory.Instance.curLabPhase2RawData.touch2StartPos = Input.mousePosition;
                        GlobalMemory.Instance.curLabPhase2RawData.movePhase2StartPos = targetVisualizer.getTargetScreenPosition();
                        curTarget2HoldTapStatus = HoldTapStatus.tapped_on_screen_2;
                    }
                    else if (Input.GetMouseButton(0))
                    {
                        Vector3 intentPos = processScreenPosToGetWorldPosAtZeroZ(Input.mousePosition);
                        targetVisualizer.moveTarget(intentPos);
                        GlobalMemory.Instance.curLabPhase2RawData.touch2EndStamp = CurrentTimeMillis();
                        GlobalMemory.Instance.curLabPhase2RawData.touch2EndPos = Input.mousePosition;
                        GlobalMemory.Instance.curLabPhase2RawData.movePhase2EndPos = targetVisualizer.getTargetScreenPosition();
                        GlobalMemory.Instance.curLabPhase2RawData.targetReachEndpointStamp = CurrentTimeMillis();
                    }
                    else if (Input.GetMouseButtonUp(0))
                    {
                        Vector3 intentPos = processScreenPosToGetWorldPosAtZeroZ(Input.mousePosition);
                        targetVisualizer.moveTarget(intentPos);
                        GlobalMemory.Instance.curLabPhase2RawData.touch2EndStamp = CurrentTimeMillis();
                        GlobalMemory.Instance.curLabPhase2RawData.touch2EndPos = Input.mousePosition;
                        GlobalMemory.Instance.curLabPhase2RawData.movePhase2EndPos = targetVisualizer.getTargetScreenPosition();
                        GlobalMemory.Instance.curLabPhase2RawData.targetReachEndpointStamp = CurrentTimeMillis();
                    }
#elif UNITY_IOS || UNITY_ANDROID
                    if ( Input.touchCount == 1)
                    {
                        Touch touch = Input.GetTouch(0);
                        if (touch.phase == TouchPhase.Began)
                        {
                            Vector3 intentPos = processScreenPosToGetWorldPosAtZeroZ(touch.position);
                            targetVisualizer.moveTarget(intentPos);
                            GlobalMemory.Instance.curLabPhase2RawData.touch2StartStamp = CurrentTimeMillis();
                            GlobalMemory.Instance.curLabPhase2RawData.touch2StartPos = touch.position;
                            GlobalMemory.Instance.curLabPhase2RawData.movePhase2StartPos = targetVisualizer.getTargetScreenPosition();
                            curTarget2HoldTapStatus = HoldTapStatus.tapped_on_screen_2;
                        }
                        else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                        {
                            Vector3 intentPos = processScreenPosToGetWorldPosAtZeroZ(touch.position);
                            targetVisualizer.moveTarget(intentPos);
                            GlobalMemory.Instance.curLabPhase2RawData.touch2EndStamp = CurrentTimeMillis();
                            GlobalMemory.Instance.curLabPhase2RawData.touch2EndPos = touch.position;
                            GlobalMemory.Instance.curLabPhase2RawData.movePhase2EndPos = targetVisualizer.getTargetScreenPosition();
                            GlobalMemory.Instance.curLabPhase2RawData.targetReachEndpointStamp = CurrentTimeMillis();
                        }
                        else if (touch.phase == TouchPhase.Ended)
                        {
                            Vector3 intentPos = processScreenPosToGetWorldPosAtZeroZ(touch.position);
                            targetVisualizer.moveTarget(intentPos);
                            GlobalMemory.Instance.curLabPhase2RawData.touch2EndStamp = CurrentTimeMillis();
                            GlobalMemory.Instance.curLabPhase2RawData.touch2EndPos = touch.position;
                            GlobalMemory.Instance.curLabPhase2RawData.movePhase2EndPos = targetVisualizer.getTargetScreenPosition();
                            GlobalMemory.Instance.curLabPhase2RawData.targetReachEndpointStamp = CurrentTimeMillis();
                        }
                    }
#endif
                }
                else if (curTarget2HoldTapStatus == HoldTapStatus.tapped_on_screen_2
                    && GlobalMemory.Instance.tech2Target1HoldTapStatus == HoldTapStatus.tapped_on_screen_2)
                {
                    targetVisualizer.showTarget();
                    if (checkTouchEndPosCorrect())
                    {
                        targetVisualizer.correctTarget();
                        curTarget2HoldTapStatus = HoldTapStatus.tap_correct_on_screen_2;
                    }
                    else
                    {
                        targetVisualizer.wrongTarget();
                        curHoldTapResult = HoldTapResult.tap_failed_to_arrive_pos;
                        curTarget2HoldTapStatus = HoldTapStatus.t1tot2_trial_failed;
                    }
                }
                else if (curTarget2HoldTapStatus == HoldTapStatus.tap_correct_on_screen_2)
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
                        trialController.switchTrialPhase(PublicTrialParams.TrialPhase.a_successful_trial);
                    }
                }
                /*
                else if (GlobalMemory.Instance.tech2Target1HoldTapStatus == HoldTapStatus.t1tot2_trial_failed)
                {
                    targetVisualizer.hideTarget();
                    targetVisualizer.hideShadow();
                    curHoldTapResult = GlobalMemory.Instance.tech2Target1HoldTapResult;
                    curTarget2HoldTapStatus = HoldTapStatus.t1tot2_trial_failed;
                }*/
            }
            else if (GlobalMemory.Instance.lab1Target2Status == TargetStatus.total_on_screen_2)
            {
                if (curTarget2HoldTapStatus == HoldTapStatus.inactive_on_screen_2)
                {
                    if (Input.touchCount == 1)
                    {
                        //uiController.updateDragMode("t2-pos: " + targetVisualizer.getTargetColliderEnabled() 
                        //    + targetVisualizer.getTargetPosition().ToString()
                        //    + targetVisualizer.getShadowPosition().ToString());
                        Touch touch = Input.GetTouch(0);
                        touchSuccess = process1Touch4Target2(touch.position, 0);
                        if (touchSuccess)
                        {
                            if (touch.phase == TouchPhase.Began)
                            {
                                
                                GlobalMemory.Instance.curLabPhase1RawData.touch1StartStamp = CurrentTimeMillis();
                                GlobalMemory.Instance.curLabPhase1RawData.touch1StartPos = touch.position;
                                GlobalMemory.Instance.curLabPhase1RawData.targetReachMidpointStamp = CurrentTimeMillis();
                                targetVisualizer.activeTarget();
                                curTarget2HoldTapStatus = HoldTapStatus.holding_on_screen_2;
                            }
                        }
                    }
                    else
                    {
                        touchSuccess = false;
                    }
                }
                else if (curTarget2HoldTapStatus == HoldTapStatus.holding_on_screen_2)
                {
                    if (Input.touchCount == 1)
                    {
                        Touch touch = Input.GetTouch(0);
                        bool touchSuccess = process1Touch4Target2(touch.position, 0);
                        if (touchSuccess)
                        {
                            if (touch.phase == TouchPhase.Ended)
                            {
                                GlobalMemory.Instance.curLabPhase1RawData.touch1EndStamp = CurrentTimeMillis();
                                GlobalMemory.Instance.curLabPhase1RawData.touch1EndPos = touch.position;
                                GlobalMemory.Instance.curLabPhase1RawData.movePhase1EndPos = targetVisualizer.getTargetScreenPosition();
                                targetVisualizer.inactiveTarget();
                                if (GlobalMemory.Instance.tech2Target1HoldTapStatus == HoldTapStatus.tapped_on_screen_1)
                                {
                                    curTarget2HoldTapStatus = HoldTapStatus.tapped_on_screen_1;
                                }
                                else
                                {
                                    targetVisualizer.wrongTarget();
                                    curHoldTapResult = HoldTapResult.hold_released_before_tap;
                                    curTarget2HoldTapStatus = HoldTapStatus.t2tot1_trial_failed;
                                }
                            }
                        }
                        else
                        {
                            GlobalMemory.Instance.curLabPhase1RawData.touch1EndStamp = CurrentTimeMillis();
                            GlobalMemory.Instance.curLabPhase1RawData.touch1EndPos = touch.position;
                            GlobalMemory.Instance.curLabPhase1RawData.movePhase1EndPos = targetVisualizer.getTargetScreenPosition();
                            targetVisualizer.inactiveTarget();
                            targetVisualizer.wrongTarget();
                            curHoldTapResult = HoldTapResult.hold_outside_before_tap;
                            curTarget2HoldTapStatus = HoldTapStatus.t2tot1_trial_failed;
                        }
                    }
                    else
                    {
                        targetVisualizer.inactiveTarget();
                        touchSuccess = false;
                        targetVisualizer.wrongTarget();
                        curHoldTapResult = HoldTapResult.hold_released_before_tap;
                        curTarget2HoldTapStatus = HoldTapStatus.t2tot1_trial_failed;
                    }
                }
                else if (curTarget2HoldTapStatus == HoldTapStatus.tapped_on_screen_1)
                {
                    if (GlobalMemory.Instance.tech2Target1HoldTapStatus == HoldTapStatus.tap_correct_on_screen_1)
                    {
                        targetVisualizer.hideTarget();
                        targetVisualizer.hideShadow();
                        uiController.updatePosInfo(curHoldTapResult.ToString());
                        GlobalMemory.Instance.curLabPhase1RawData.targetReachEndpointInfoReceivedStamp = CurrentTimeMillis();
                        trialController.switchTrialPhase(PublicTrialParams.TrialPhase.a_successful_trial);
                    }
                }
            }

            GlobalMemory.Instance.tech2Target2HoldTapStatus = curTarget2HoldTapStatus;
            GlobalMemory.Instance.tech2Target2HoldTapResult = curHoldTapResult;
            if (curTarget2HoldTapStatus != prevTarget2HoldTapStatus
                && curTarget2HoldTapStatus != HoldTapStatus.holding_on_screen_1)
            {
                GlobalMemory.Instance.client.prepareNewMessage4Server(MessageType.HoldTapInfo);
            }
            prevTarget2HoldTapStatus = curTarget2HoldTapStatus;

            uiController.updateDebugInfo(curTarget2HoldTapStatus.ToString());
            uiController.updateStatusInfo(GlobalMemory.Instance.tech2Target1HoldTapStatus.ToString());
        }


    }

    private bool process1Touch4Target2(Vector2 pos, int targetid)
    {
        
        int hitid = -1;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(pos);
        uiController.updatePosInfo(ray.origin.ToString() + " " + Camera.main.transform.position.ToString());
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            
            uiController.updatePosInfo("t2-pos: " + targetVisualizer.getTargetPosition().ToString());
            if (hit.collider.gameObject.name.Length >= 9)
            {
                hitid = Convert.ToInt32(hit.collider.gameObject.name.Substring(7, 2));
                Debug.Log("info: " + hitid.ToString() + " " + hit.collider.gameObject.name);
                Debug.DrawLine(ray.origin, hit.point, Color.yellow);
                hitCnt++;
                uiController.updatePosInfo("hit-1: " + hitCnt.ToString() + " " + hitid.ToString() + " " + hit.collider.gameObject.name);
            }
            else
            {
                hitCnt += 2;
                uiController.updatePosInfo("hit-2: " + hitCnt.ToString() + " " + hit.collider.gameObject.name);
            }
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
        } else
        {
            return false;
        }
        */
    }

    private long CurrentTimeMillis()
    {
        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        TimeSpan diff = DateTime.Now.ToUniversalTime() - origin;
        return (long)diff.TotalMilliseconds;
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

    public void initParamsWhenTargetOnScreen1(int id2)
    {
        Debug.Log("tech2-initS1()");
        targetVisualizer.moveShadowWithPosID(id2);
        targetVisualizer.hideTarget();
        targetVisualizer.showShadow();
        prevTarget2HoldTapStatus = curTarget2HoldTapStatus = HoldTapStatus.inactive_on_screen_1;
        if (GlobalMemory.Instance)
        {
            GlobalMemory.Instance.curLabPhase2RawData.moveDestination = targetVisualizer.getShadowScreenPosition();
            GlobalMemory.Instance.tech2Target1HoldTapStatus
                = GlobalMemory.Instance.tech2Target2HoldTapStatus
                = HoldTapStatus.inactive_on_screen_1;
        }
        resetHoldTapParams();
    }
    public void initParamsWhenTargetOnScreen2(int id1)
    {
        Debug.Log("tech2-initS2()");
        targetVisualizer.moveTargetWithPosID(id1);
        targetVisualizer.showTarget();
        targetVisualizer.hideShadow();
        prevTarget2HoldTapStatus = curTarget2HoldTapStatus = HoldTapStatus.inactive_on_screen_2;
        if (GlobalMemory.Instance)
        {
            GlobalMemory.Instance.curLabPhase1RawData.moveStartPos
                = GlobalMemory.Instance.curLabPhase1RawData.movePhase1StartPos
                = targetVisualizer.getTargetScreenPosition();
            GlobalMemory.Instance.tech2Target1HoldTapStatus
                = GlobalMemory.Instance.tech2Target2HoldTapStatus
                = HoldTapStatus.inactive_on_screen_2;
        }
        resetHoldTapParams();
    }
}
