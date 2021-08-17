using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PublicInfo;
using static PublicDragParams;
using System;

public class tech1DirectDragProcessor : MonoBehaviour
{
    public lab1UIController uiController;
    public lab1TrialController trialController;
    public lab1TargetVisualizer targetVisualizer;

    private DirectDragStatus curTarget1DirectDragStatus, prevTarget1DirectDragStatus;
    private Vector3 prevTarget1Pos, curTarget1Pos;

    private bool touchSuccess;
    private Vector3 dragStartTouchPosInWorld;
    private Vector3 dragStartTargetPos;

    private DirectDragResult curDirectDragResult;

    private float rightBound;
    private const float minX = DRAG_MIN_X, maxX = DRAG_MAX_X, minY = DRAG_MIN_Y, maxY = DRAG_MAX_Y;

    private float delayTimer = 0f;
    private const float wait_time_before_vanish = 0.15f;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("ScreenHeight: " + Screen.height + "; ScreenWidth: " + Screen.width);
        rightBound = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0)).x;
        resetDirectDragParams();
    }

    void resetDirectDragParams()
    {
        delayTimer = wait_time_before_vanish;
        touchSuccess = false;
        curDirectDragResult = DirectDragResult.direct_drag_success;
        if (GlobalMemory.Instance)
        {
            GlobalMemory.Instance.tech1Target1DirectDragResult
                = GlobalMemory.Instance.tech1Target2DirectDragResult
                = DirectDragResult.direct_drag_success;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GlobalMemory.Instance && GlobalMemory.Instance.curDragType == DragType.direct_drag)
        {
            if (curDirectDragResult != DirectDragResult.direct_drag_success
                || GlobalMemory.Instance.tech1Target2DirectDragResult != DirectDragResult.direct_drag_success)
            {
                targetVisualizer.hideTarget();
                targetVisualizer.hideShadow();
                Debug.Log("Trial s failed: " + curDirectDragResult.ToString());
                Debug.Log("Trial c failed: " + GlobalMemory.Instance.tech1Target2DirectDragResult.ToString());
                uiController.updatePosInfo(curDirectDragResult.ToString());
                trialController.switchTrialPhase(PublicTrialParams.TrialPhase.a_failed_trial);
            }

            if (GlobalMemory.Instance.lab1Target1Status == TargetStatus.total_on_screen_1)
            {
                if (curTarget1DirectDragStatus == DirectDragStatus.inactive_on_screen_1)
                {
#if UNITY_ANDROID && UNITY_EDITOR
                    if (Input.GetMouseButtonDown(0))
                    {
                        touchSuccess = process1Touch4Target1(Input.mousePosition, 0);
                        if (touchSuccess)
                        {
                            targetVisualizer.activeTarget();
                            dragStartTouchPosInWorld = processScreenPosToGetWorldPosAtZeroZ(Input.mousePosition);
                            dragStartTargetPos = targetVisualizer.getTargetPosition();
                            curTarget1DirectDragStatus = DirectDragStatus.drag_phase1_on_screen_1;
                        }
                    }
                    else
                    {
                        touchSuccess = false;
                    }
#elif UNITY_IOS || UNITY_ANDROID
                    if (Input.touchCount == 1)
                    {
                        Touch touch = Input.GetTouch(0);
                        if (touch.phase == TouchPhase.Began)
                        {
                            touchSuccess = process1Touch4Target1(touch.position, 0);
                            if (touchSuccess)
                            {
                                targetVisualizer.activeTarget();
                                dragStartTouchPosInWorld = processScreenPosToGetWorldPosAtZeroZ(touch.position);
                                dragStartTargetPos = targetVisualizer.getTargetPosition();
                                curTarget1DirectDragStatus = DirectDragStatus.drag_phase1_on_screen_1;
                            }
                        }
                    }
                    else
                    {
                        touchSuccess = false;
                    }
#endif
                }
                else if (curTarget1DirectDragStatus == DirectDragStatus.drag_phase1_on_screen_1)
                {
#if UNITY_ANDROID && UNITY_EDITOR
                    if (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
                    {
                        if (touchSuccess)
                        {
                            Vector3 curTouchPosInWorld = processScreenPosToGetWorldPosAtZeroZ(Input.mousePosition);
                            Vector3 offset = curTouchPosInWorld - dragStartTouchPosInWorld;
                            Vector3 intentPos = dragStartTargetPos + offset;
                            if (intentPos.x > minX && intentPos.y > minY && intentPos.y < maxY)
                            {
                                targetVisualizer.moveTarget(intentPos);
                            }
                            if (Input.GetMouseButtonUp(0))
                            {
                                targetVisualizer.inactiveTarget();
                                // record wrong touch position
                                curDirectDragResult = DirectDragResult.drag_1_failed_to_arrive_junction;
                                curTarget1DirectDragStatus = DirectDragStatus.t1tot2_trial_failed;
                            }
                        }
                    }
                    else
                    {
                        touchSuccess = false;
                    }
#elif UNITY_IOS || UNITY_ANDROID
                    if (Input.touchCount == 1)
                    {
                        Touch touch = Input.GetTouch(0);
                        if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary
                            || touch.phase == TouchPhase.Ended)
                        {
                            if (touchSuccess)
                            {
                                Vector3 curTouchPosInWorld = processScreenPosToGetWorldPosAtZeroZ(touch.position);
                                Vector3 offset = curTouchPosInWorld - dragStartTouchPosInWorld;
                                Vector3 intentPos = dragStartTargetPos + offset;
                                if (intentPos.x > minX && intentPos.y > minY && intentPos.y < maxY)
                                {
                                    targetVisualizer.moveTarget(intentPos);
                                }
                                if (touch.phase == TouchPhase.Ended)
                                {
                                    targetVisualizer.inactiveTarget();
                                    // record wrong touch position
                                    curDirectDragResult = DirectDragResult.drag_1_failed_to_arrive_junction;
                                    curTarget1DirectDragStatus = DirectDragStatus.t1tot2_trial_failed;
                                }
                            }
                        }
                    }
                    else
                    {
                        touchSuccess = false;
                    }
#endif
                    if ((targetVisualizer.getTargetPosition().x + targetVisualizer.getTargetLocalScale().x / 2) > rightBound)
                    {
                        //GlobalMemory.Instance.tech1Target1DirectDragPosition = targetVisualizer.getTargetPosition();
                        curTarget1DirectDragStatus = DirectDragStatus.across_from_screen_1;
                    }
                }
                else if (curTarget1DirectDragStatus == DirectDragStatus.across_from_screen_1)
                {
#if UNITY_ANDROID && UNITY_EDITOR
                    if (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
                    {
                        if (touchSuccess)
                        {
                            Vector3 curTouchPosInWorld = processScreenPosToGetWorldPosAtZeroZ(Input.mousePosition);
                            Vector3 offset = curTouchPosInWorld - dragStartTouchPosInWorld;
                            Vector3 intentPos = dragStartTargetPos + offset;
                            if (intentPos.x > minX && intentPos.y > minY && intentPos.y < maxY)
                            {
                                targetVisualizer.moveTarget(intentPos);
                            }
                            if (Input.GetMouseButtonUp(0))
                            {
                                targetVisualizer.inactiveTarget();
                                if ((targetVisualizer.getTargetPosition().x + targetVisualizer.getTargetLocalScale().x / 2) > rightBound)
                                {
                                    //GlobalMemory.Instance.tech1Target1DirectDragPosition = targetVisualizer.getTargetPosition();
                                    curTarget1DirectDragStatus = DirectDragStatus.drag_phase1_end_on_screen_1;

                                } else
                                {
                                    curDirectDragResult = DirectDragResult.drag_1_left_junction_after_arrive;
                                    curTarget1DirectDragStatus = DirectDragStatus.t1tot2_trial_failed;
                                }
                            }
                        }
                    }
                    else
                    {
                        touchSuccess = false;
                    }
#elif UNITY_IOS || UNITY_ANDROID
                    if (Input.touchCount == 1)
                    {
                        Touch touch = Input.GetTouch(0);
                        if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary
                            || touch.phase == TouchPhase.Ended)
                        {
                            if (touchSuccess)
                            {
                                Vector3 curTouchPosInWorld = processScreenPosToGetWorldPosAtZeroZ(touch.position);
                                Vector3 offset = curTouchPosInWorld - dragStartTouchPosInWorld;
                                Vector3 intentPos = dragStartTargetPos + offset;
                                if (intentPos.x > minX && intentPos.y > minY && intentPos.y < maxY)
                                {
                                    targetVisualizer.moveTarget(intentPos);
                                }
                                if (Input.GetMouseButtonUp(0))
                                {
                                    targetVisualizer.inactiveTarget();
                                    if ((targetVisualizer.getTargetPosition().x + targetVisualizer.getTargetLocalScale().x / 2) > rightBound)
                                    {
                                        curTarget1DirectDragStatus = DirectDragStatus.drag_phase1_end_on_screen_1;
                                    }
                                    else
                                    {
                                        //GlobalMemory.Instance.tech1Target1DirectDragPosition = targetVisualizer.getTargetPosition();
                                        curDirectDragResult = DirectDragResult.drag_1_left_junction_after_arrive;
                                        curTarget1DirectDragStatus = DirectDragStatus.t1tot2_trial_failed;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        touchSuccess = false;
                    }
#endif

                }
                else if (curTarget1DirectDragStatus == DirectDragStatus.drag_phase1_end_on_screen_1)
                {
                    curTarget1DirectDragStatus = DirectDragStatus.wait_for_drag_on_2;
                }
                else if (curTarget1DirectDragStatus == DirectDragStatus.wait_for_drag_on_2)
                {
                    if (GlobalMemory.Instance.refreshTarget1
                        && GlobalMemory.Instance.tech1Target2DirectDragStatus == DirectDragStatus.drag_phase2_on_screen_2)
                    {
                        targetVisualizer.activeTarget();
                        targetVisualizer.moveTarget(GlobalMemory.Instance.tech1Target1DirectDragPosition);
                        GlobalMemory.Instance.refreshTarget1 = false;
                    }
                    if (GlobalMemory.Instance.tech1Target2DirectDragStatus == DirectDragStatus.across_end_from_screen_1)
                    {
                        targetVisualizer.hideTarget();
                    }
                    else if (GlobalMemory.Instance.tech1Target2DirectDragStatus == DirectDragStatus.drag_phase2_end_on_screen_2)
                    {
                        targetVisualizer.hideTarget();
                        targetVisualizer.hideShadow();
                        trialController.switchTrialPhase(PublicTrialParams.TrialPhase.a_successful_trial);
                    }
                }
            }
            else if (GlobalMemory.Instance.lab1Target1Status == TargetStatus.total_on_screen_2)
            {
                if (curTarget1DirectDragStatus == DirectDragStatus.inactive_on_screen_2
                    && GlobalMemory.Instance.tech1Target2DirectDragStatus == DirectDragStatus.across_from_screen_2)
                {
                    if (GlobalMemory.Instance.refreshTarget1)
                    {
                        targetVisualizer.moveTarget(GlobalMemory.Instance.tech1Target1DirectDragPosition);
                        targetVisualizer.activeTarget();
                        targetVisualizer.showTarget();
                    }
                }
                else if (curTarget1DirectDragStatus == DirectDragStatus.inactive_on_screen_2
                    && GlobalMemory.Instance.tech1Target2DirectDragStatus == DirectDragStatus.drag_phase1_end_on_screen_2)
                {
                    if (GlobalMemory.Instance.refreshTarget1)
                    {
                        targetVisualizer.moveTarget(GlobalMemory.Instance.tech1Target1DirectDragPosition);
                        targetVisualizer.inactiveTarget();
                        targetVisualizer.showTarget();
                        curTarget1DirectDragStatus = DirectDragStatus.drag_phase1_end_on_screen_2;
                    }
                }
                else if (curTarget1DirectDragStatus == DirectDragStatus.drag_phase1_end_on_screen_2)
                {
#if UNITY_ANDROID && UNITY_EDITOR
                    if (Input.GetMouseButtonDown(0))
                    {
                        touchSuccess = process1Touch4Target1(Input.mousePosition, 0);
                        if (touchSuccess)
                        {
                            targetVisualizer.activeTarget();
                            dragStartTouchPosInWorld = processScreenPosToGetWorldPosAtZeroZ(Input.mousePosition);
                            dragStartTargetPos = targetVisualizer.getTargetPosition();
                            curTarget1DirectDragStatus = DirectDragStatus.drag_phase2_on_screen_1;
                        }
                        else
                        {
                            curDirectDragResult = DirectDragResult.drag_2_failed_to_touch;
                            curTarget1DirectDragStatus = DirectDragStatus.t2tot1_trial_failed;
                        }
                    }
                    else
                    {
                        touchSuccess = false;
                    }
#elif UNITY_IOS || UNITY_ANDROID
                    if (Input.touchCount == 1)
                    {
                        Touch touch = Input.GetTouch(0);
                        if (touch.phase == TouchPhase.Began)
                        {
                            touchSuccess = process1Touch4Target1(touch.position, 0);
                            if (touchSuccess)
                            {
                                targetVisualizer.activeTarget();
                                dragStartTouchPosInWorld = processScreenPosToGetWorldPosAtZeroZ(touch.position);
                                dragStartTargetPos = targetVisualizer.getTargetPosition();
                                curTarget1DirectDragStatus = DirectDragStatus.drag_phase2_on_screen_1;
                            }
                            else
                            {
                                curDirectDragResult = DirectDragResult.drag_2_failed_to_touch;
                                curTarget1DirectDragStatus = DirectDragStatus.t2tot1_trial_failed;
                            }
                        }
                    }
                    else
                    {
                        touchSuccess = false;
                    }
#endif
                }
                else if (curTarget1DirectDragStatus == DirectDragStatus.drag_phase2_on_screen_1)
                {
#if UNITY_ANDROID && UNITY_EDITOR
                    if (touchSuccess && (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0)) )
                    {
                        Vector3 curTouchPosInWorld = processScreenPosToGetWorldPosAtZeroZ(Input.mousePosition);
                        Vector3 offset = curTouchPosInWorld - dragStartTouchPosInWorld;
                        Vector3 intentPos = dragStartTargetPos + offset;
                        if (intentPos.x > minX && intentPos.y > minY && intentPos.y < maxY)
                        {
                            targetVisualizer.moveTarget(intentPos);
                        }
                        if (Input.GetMouseButtonUp(0))
                        {
                            targetVisualizer.inactiveTarget();
                            curDirectDragResult = DirectDragResult.drag_2_failed_to_leave_junction;
                            curTarget1DirectDragStatus = DirectDragStatus.t2tot1_trial_failed;
                        }
                    }
#elif UNITY_IOS || UNITY_ANDROID
                    if (touchSuccess && Input.touchCount == 1)
                    {
                        Touch touch = Input.GetTouch(0);
                        if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved
                            || touch.phase == TouchPhase.Ended)
                        {
                            Vector3 curTouchPosInWorld = processScreenPosToGetWorldPosAtZeroZ(touch.position);
                            Vector3 offset = curTouchPosInWorld - dragStartTouchPosInWorld;
                            Vector3 intentPos = dragStartTargetPos + offset;
                            if (intentPos.x > minX && intentPos.y > minY && intentPos.y < maxY)
                            {
                                targetVisualizer.moveTarget(intentPos);
                            }
                            if (touch.phase == TouchPhase.Ended)
                            {
                                targetVisualizer.inactiveTarget();
                                curDirectDragResult = DirectDragResult.drag_2_failed_to_leave_junction;
                                curTarget1DirectDragStatus = DirectDragStatus.t2tot1_trial_failed;
                            }
                        }
                    }
#endif

                    if ( (targetVisualizer.getTargetPosition().x + targetVisualizer.getTargetLocalScale().x / 2) < rightBound)
                    {
                        //GlobalMemory.Instance.tech1Target1DirectDragPosition = targetVisualizer.getTargetPosition();
                        curTarget1DirectDragStatus = DirectDragStatus.across_end_from_screen_2;
                    }
                }
                else if (curTarget1DirectDragStatus == DirectDragStatus.across_end_from_screen_2
                || curTarget1DirectDragStatus == DirectDragStatus.drag_phase2_ongoing_on_screen_1)
                {
#if UNITY_ANDROID && UNITY_EDITOR
                    if (touchSuccess && (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0)) )
                    {
                        Vector3 curTouchPosInWorld = processScreenPosToGetWorldPosAtZeroZ(Input.mousePosition);
                        Vector3 offset = curTouchPosInWorld - dragStartTouchPosInWorld;
                        Vector3 intentPos = dragStartTargetPos + offset;
                        if (intentPos.x > minX && intentPos.y > minY && intentPos.y < maxY)
                        {
                            targetVisualizer.moveTarget(intentPos);
                        }
                        if (curTarget1DirectDragStatus == DirectDragStatus.across_end_from_screen_2)
                        {
                            curTarget1DirectDragStatus = DirectDragStatus.drag_phase2_ongoing_on_screen_1;
                        }
                        if (Input.GetMouseButtonUp(0))
                        {
                            targetVisualizer.inactiveTarget();
                            if ((targetVisualizer.getTargetPosition().x + targetVisualizer.getTargetLocalScale().x / 2) > rightBound)
                            {
                                //GlobalMemory.Instance.tech1Target1DirectDragPosition = targetVisualizer.getTargetPosition();
                                curDirectDragResult = DirectDragResult.drag_2_rearrived_junction_after_leave;
                                curTarget1DirectDragStatus = DirectDragStatus.t2tot1_trial_failed;
                            }
                            else
                            {
                                curTarget1DirectDragStatus = DirectDragStatus.drag_phase2_end_on_screen_1;
                            }
                        }
                    }
                    else
                    {
                        touchSuccess = false;
                    }
#elif UNITY_IOS || UNITY_ANDROID
                    if (touchSuccess && Input.touchCount == 1)
                    {
                        Touch touch = Input.GetTouch(0);
                        if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary
                            || touch.phase == TouchPhase.Ended)
                        {
                            Vector3 curTouchPosInWorld = processScreenPosToGetWorldPosAtZeroZ(touch.position);
                            Vector3 offset = curTouchPosInWorld - dragStartTouchPosInWorld;
                            Vector3 intentPos = dragStartTargetPos + offset;
                            if (intentPos.x > minX && intentPos.y > minY && intentPos.y < maxY)
                            {
                                targetVisualizer.moveTarget(intentPos);
                            }
                            if (curTarget1DirectDragStatus == DirectDragStatus.across_end_from_screen_2)
                            {
                                curTarget1DirectDragStatus = DirectDragStatus.drag_phase2_ongoing_on_screen_1;
                            }
                            if (touch.phase == TouchPhase.Ended)
                            {
                                targetVisualizer.inactiveTarget();
                                if ((targetVisualizer.getTargetPosition().x + targetVisualizer.getTargetLocalScale().x / 2) > rightBound)
                                {
                                    //GlobalMemory.Instance.tech1Target1DirectDragPosition = targetVisualizer.getTargetPosition();
                                    curDirectDragResult = DirectDragResult.drag_2_rearrived_junction_after_leave;
                                    curTarget1DirectDragStatus = DirectDragStatus.t2tot1_trial_failed;
                                }
                                else
                                {
                                    curTarget1DirectDragStatus = DirectDragStatus.drag_phase2_end_on_screen_1;
                                }
                            }
                        }
                    }
                    else
                    {
                        touchSuccess = false;
                    }
#endif 
                }
                else if (curTarget1DirectDragStatus == DirectDragStatus.drag_phase2_end_on_screen_1)
                {
                    if (delayTimer > 0f)
                    {
                        delayTimer -= Time.deltaTime;
                    }
                    else
                    {
                        targetVisualizer.hideTarget();
                        targetVisualizer.hideShadow();

                        if (checkTouchEndPosCorrect())
                        {
                            trialController.switchTrialPhase(PublicTrialParams.TrialPhase.a_successful_trial);
                        }
                        else
                        {
                            curDirectDragResult = DirectDragResult.drag_2_failed_to_arrive_pos;
                            curTarget1DirectDragStatus = DirectDragStatus.t2tot1_trial_failed;
                        }
                    }
                }
            }

            curTarget1Pos = targetVisualizer.getTargetPosition();
            GlobalMemory.Instance.tech1Target1DirectDragPosition = curTarget1Pos;
            GlobalMemory.Instance.tech1Target1DirectDragStatus = curTarget1DirectDragStatus;
            GlobalMemory.Instance.tech1Target1DirectDragResult = curDirectDragResult;
            if ( curDirectDragResult != DirectDragResult.direct_drag_success
                || (curTarget1DirectDragStatus == DirectDragStatus.across_from_screen_1 && prevTarget1Pos != curTarget1Pos)
                || (curTarget1DirectDragStatus == DirectDragStatus.drag_phase2_on_screen_1 && prevTarget1Pos != curTarget1Pos)
                || ((curTarget1DirectDragStatus != prevTarget1DirectDragStatus) &&
                (curTarget1DirectDragStatus == DirectDragStatus.drag_phase1_end_on_screen_1
                || curTarget1DirectDragStatus == DirectDragStatus.drag_phase2_on_screen_1
                || curTarget1DirectDragStatus == DirectDragStatus.across_end_from_screen_2
                || curTarget1DirectDragStatus == DirectDragStatus.drag_phase2_end_on_screen_1
                || curTarget1DirectDragStatus == DirectDragStatus.t1tot2_trial_failed
                || curTarget1DirectDragStatus == DirectDragStatus.t2tot1_trial_failed) ) )
            {
                GlobalMemory.Instance.server.prepareNewMessage4Client(MessageType.DirectDragInfo);
            }
            prevTarget1DirectDragStatus = curTarget1DirectDragStatus;
            prevTarget1Pos = curTarget1Pos;

            uiController.updateDebugInfo(curTarget1DirectDragStatus.ToString());
            uiController.updateStatusInfo(GlobalMemory.Instance.tech1Target2DirectDragStatus.ToString());
        }
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

    public void initParamsWhenTargetOnScreen1 (int id1)
    {
        Debug.Log("tech1-initS1()");
        targetVisualizer.moveTargetWithPosID(id1);
        targetVisualizer.showTarget();
        targetVisualizer.hideShadow();
        prevTarget1DirectDragStatus = curTarget1DirectDragStatus = DirectDragStatus.inactive_on_screen_1;
        prevTarget1Pos = curTarget1Pos = targetVisualizer.getTargetPosition();
        
        if (GlobalMemory.Instance)
        {
            GlobalMemory.Instance.tech1Target1DirectDragStatus
                = GlobalMemory.Instance.tech1Target2DirectDragStatus
                = DirectDragStatus.inactive_on_screen_1;
            GlobalMemory.Instance.tech1Target1DirectDragPosition = curTarget1Pos;
        }
        resetDirectDragParams();
    }
    public void initParamsWhenTargetOnScreen2 (int id2)
    {
        Debug.Log("tech1-initS2()");
        targetVisualizer.moveShadowWithPosID(id2);
        targetVisualizer.hideTarget();
        targetVisualizer.showShadow();
        prevTarget1DirectDragStatus = curTarget1DirectDragStatus = DirectDragStatus.inactive_on_screen_2;
        
        if (GlobalMemory.Instance)
        {
            GlobalMemory.Instance.tech1Target1DirectDragStatus
                = GlobalMemory.Instance.tech1Target2DirectDragStatus
                = DirectDragStatus.inactive_on_screen_2;
        }
        resetDirectDragParams();
    }
}
