﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PublicInfo;
using static PublicDragParams;
using System;

public class tech2HoldTapProcessor : MonoBehaviour
{
    public tech1UIController uiController;
    public tech1TrialController trialController;
    public tech1TargetVisualizer targetVisualizer;

    private HoldTapStatus curTarget2HoldTapStatus, prevTarget2HoldTapStatus;
    private bool touchSuccess;
    private HoldTapResult curHoldTapResult;

    // Start is called before the first frame update
    void Start()
    {
        resetHoldTapParams();
    }

    void resetHoldTapParams()
    {
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
            if (curHoldTapResult != HoldTapResult.hold_tap_success
                || GlobalMemory.Instance.tech2Target1HoldTapResult != HoldTapResult.hold_tap_success)
            {
                targetVisualizer.hideTarget();
                targetVisualizer.hideShadow();
                Debug.Log("Trial c failed: " + curHoldTapResult.ToString());
                Debug.Log("Trial s failed: " + GlobalMemory.Instance.tech2Target1HoldTapResult.ToString());
                uiController.updatePosInfo(curHoldTapResult.ToString());
                trialController.switchTrialPhase(PublicTrialParams.TrialPhase.a_failed_trial);
            }

            if (GlobalMemory.Instance.lab1Target2Status == TargetStatus.total_on_screen_1)
            {
                if (GlobalMemory.Instance.tech2Target1HoldTapStatus == HoldTapStatus.holding_on_screen_1
                    && (curTarget2HoldTapStatus == HoldTapStatus.inactive_on_screen_1
                     || curTarget2HoldTapStatus == HoldTapStatus.tapped_on_screen_2))
                {
#if UNITY_ANDROID && UNITY_EDITOR
                    if (Input.GetMouseButtonDown(0))
                    {
                        Vector3 intentPos = processScreenPosToGetWorldPosAtZeroZ(Input.mousePosition);
                        targetVisualizer.moveTarget(intentPos);
                        curTarget2HoldTapStatus = HoldTapStatus.tapped_on_screen_2;
                    }
                    else if (Input.GetMouseButton(0))
                    {
                        Vector3 intentPos = processScreenPosToGetWorldPosAtZeroZ(Input.mousePosition);
                        targetVisualizer.moveTarget(intentPos);
                    }
                    else if (Input.GetMouseButtonUp(0))
                    {
                        Vector3 intentPos = processScreenPosToGetWorldPosAtZeroZ(Input.mousePosition);
                        targetVisualizer.moveTarget(intentPos);
                    }
#elif UNITY_IOS || UNITY_ANDROID
                    if ( Input.touchCount == 1)
                    {
                        Touch touch = Input.GetTouch(0);
                        if (touch.phase == TouchPhase.Began)
                        {
                            Vector3 intentPos = processScreenPosToGetWorldPosAtZeroZ(touch.position);
                            targetVisualizer.moveTarget(intentPos);
                            curTarget2HoldTapStatus = HoldTapStatus.tapped_on_screen_2;
                        }
                        else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                        {
                            Vector3 intentPos = processScreenPosToGetWorldPosAtZeroZ(touch.position);
                            targetVisualizer.moveTarget(intentPos);
                        }
                        else if (touch.phase == TouchPhase.Ended)
                        {
                            Vector3 intentPos = processScreenPosToGetWorldPosAtZeroZ(touch.position);
                            targetVisualizer.moveTarget(intentPos);
                        }
                    }
#endif
                }
                else if (GlobalMemory.Instance.tech2Target1HoldTapStatus == HoldTapStatus.t1tot2_trial_failed)
                {
                    targetVisualizer.hideTarget();
                    targetVisualizer.hideShadow();
                    curHoldTapResult = GlobalMemory.Instance.tech2Target1HoldTapResult;
                    curTarget2HoldTapStatus = HoldTapStatus.t1tot2_trial_failed;
                    //trialController.switchTrialPhase(PublicTrialParams.TrialPhase.a_failed_trial);
                }
                else if (GlobalMemory.Instance.tech2Target1HoldTapStatus == HoldTapStatus.tapped_on_screen_2)
                {
                    targetVisualizer.showTarget();
                    //targetVisualizer.hideTarget();
                    targetVisualizer.hideShadow();

                    if (checkTouchEndPosCorrect())
                    {
                        trialController.switchTrialPhase(PublicTrialParams.TrialPhase.a_successful_trial);
                    }
                    else
                    {
                        curHoldTapResult = HoldTapResult.tap_failed_to_arrive_pos;
                        curTarget2HoldTapStatus = HoldTapStatus.t1tot2_trial_failed;
                    }
                }
            }
            else if (GlobalMemory.Instance.lab1Target2Status == TargetStatus.total_on_screen_2)
            {
                if (curTarget2HoldTapStatus == HoldTapStatus.inactive_on_screen_2)
                {
                    targetVisualizer.showTarget();
                    if (Input.touchCount == 1)
                    {
                        Touch touch = Input.GetTouch(0);
                        touchSuccess = process1Touch4Target2(touch.position, 0);
                        if (touchSuccess && touch.phase == TouchPhase.Began)
                        {
                            targetVisualizer.activeTarget();
                            curTarget2HoldTapStatus = HoldTapStatus.holding_on_screen_2;
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
                                targetVisualizer.inactiveTarget();
                                if (GlobalMemory.Instance.tech2Target1HoldTapStatus == HoldTapStatus.tapped_on_screen_1)
                                {
                                    curTarget2HoldTapStatus = HoldTapStatus.tapped_on_screen_1;
                                }
                                else
                                {
                                    curHoldTapResult = HoldTapResult.hold_released_before_tap;
                                    curTarget2HoldTapStatus = HoldTapStatus.t2tot1_trial_failed;
                                }
                            }
                        }
                        else
                        {
                            targetVisualizer.inactiveTarget();
                            curHoldTapResult = HoldTapResult.hold_outside_before_tap;
                            curTarget2HoldTapStatus = HoldTapStatus.t2tot1_trial_failed;
                        }
                    }
                    else
                    {
                        targetVisualizer.inactiveTarget();
                        touchSuccess = false;
                        curHoldTapResult = HoldTapResult.hold_released_before_tap;
                        curTarget2HoldTapStatus = HoldTapStatus.t2tot1_trial_failed;
                    }
                }
                else if (curTarget2HoldTapStatus == HoldTapStatus.tapped_on_screen_1)
                {
                    targetVisualizer.hideTarget();
                    targetVisualizer.hideShadow();
                    trialController.switchTrialPhase(PublicTrialParams.TrialPhase.a_successful_trial);
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
            GlobalMemory.Instance.tech2Target1HoldTapStatus
                = GlobalMemory.Instance.tech2Target2HoldTapStatus
                = HoldTapStatus.inactive_on_screen_2;
        }
        resetHoldTapParams();
    }
}