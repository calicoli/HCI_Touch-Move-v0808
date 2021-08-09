using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PublicLabFactors;
using static PublicDragParams;

public class demoDirectDragProcessor : MonoBehaviour
{
    public GameObject target2;
    public demoUIController uiController;
    public demoTouchVisualizer touchVisualizer;
    public demoTargetVisualizer targetVisualizer;

    private DirectDragStatus curTarget2DirectDragStatus, prevTarget2DirectDragStatus;
    private Vector3 prevTarget2Pos, curTarget2Pos;

    private bool dragSuccess;
    private Vector3 dragStartTouchPosInWorld;
    private Vector3 dragStartTargetPos;

    private bool directDragSuccess;

    private float leftBound;

    private const float minX = DRAG_MIN_X, maxX = DRAG_MAX_X, minY = DRAG_MIN_Y, maxY = DRAG_MAX_Y;

    // Start is called before the first frame update
    void Start()
    {
        leftBound = Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, 0f)).x;
    }

    // Update is called once per frame
    void Update()
    {
        if (GlobalController.Instance && GlobalController.Instance.demoDragType == DragType.direct_drag)
        {
            if (GlobalController.Instance.demoTarget2Status == TargetStatus.total_on_screen_1)
            {
                if (curTarget2DirectDragStatus == DirectDragStatus.inactive_on_screen_1
                    && GlobalController.Instance.demoTarget1DirectDragStatus == DirectDragStatus.across_from_screen_1)
                {
                    if(GlobalController.Instance.refreshTarget2)
                    {
                        targetVisualizer.moveTarget(GlobalController.Instance.demoTarget2DirectDragPosition);
                        targetVisualizer.activeTarget();
                        targetVisualizer.showTarget();
                    }
                }
                else if (curTarget2DirectDragStatus == DirectDragStatus.inactive_on_screen_1
                    && GlobalController.Instance.demoTarget1DirectDragStatus == DirectDragStatus.drag_phase1_end_on_screen_1)
                {
                    if (GlobalController.Instance.refreshTarget2)
                    {
                        targetVisualizer.moveTarget(GlobalController.Instance.demoTarget2DirectDragPosition);
                        targetVisualizer.inactiveTarget();
                        targetVisualizer.showTarget();
                        curTarget2DirectDragStatus = DirectDragStatus.drag_phase1_end_on_screen_1;
                    }
                }
                else if (curTarget2DirectDragStatus == DirectDragStatus.drag_phase1_end_on_screen_1)
                {
#if UNITY_ANDROID && UNITY_EDITOR
                    if (Input.GetMouseButtonDown(0))
                    {
                        dragSuccess = process1Touch4Target2(Input.mousePosition, 0);
                        if (dragSuccess)
                        {
                            curTarget2DirectDragStatus = DirectDragStatus.drag_phase2_on_screen_2;
                            targetVisualizer.activeTarget();
                            dragStartTouchPosInWorld = processScreenPosToGetWorldPosAtZeroZ(Input.mousePosition);
                            dragStartTargetPos = targetVisualizer.getTargetPosition();
                        }
                    }
                    else
                    {
                        dragSuccess = false;
                    }
#elif UNITY_IOS || UNITY_ANDROID
                    if (Input.touchCount == 1)
                    {
                        Touch touch = Input.GetTouch(0);
                        if (touch.phase == TouchPhase.Began)
                        {
                            dragSuccess = process1Touch4Target2(touch.position, 0);
                            if (dragSuccess)
                            {
                                curTarget2DirectDragStatus = DirectDragStatus.drag_phase2_on_screen_2;
                                targetVisualizer.activeTarget();
                                dragStartTouchPosInWorld = processScreenPosToGetWorldPosAtZeroZ(touch.position);
                                dragStartTargetPos = targetVisualizer.getTargetPosition();
                            }
                        }
                    }
                    else
                    {
                        dragSuccess = false;
                    }
#endif
                }
                else if (curTarget2DirectDragStatus == DirectDragStatus.drag_phase2_on_screen_2
                    || curTarget2DirectDragStatus == DirectDragStatus.across_end_from_screen_1
                    || curTarget2DirectDragStatus == DirectDragStatus.drag_phase2_ongoing_on_screen_2)
                {
#if UNITY_ANDROID && UNITY_EDITOR
                    if (Input.GetMouseButtonUp(0))
                    {
                        if (dragSuccess)
                        {
                            Vector3 curTouchPosInWorld = processScreenPosToGetWorldPosAtZeroZ(Input.mousePosition);
                            Vector3 offset = curTouchPosInWorld - dragStartTouchPosInWorld;
                            Vector3 intentPos = dragStartTargetPos + offset;
                            if (intentPos.x < maxX && intentPos.y > minY && intentPos.y < maxY)
                            {
                                targetVisualizer.moveTarget(intentPos);
                            }
                            targetVisualizer.inactiveTarget();
                            if ( curTarget2DirectDragStatus == DirectDragStatus.drag_phase2_ongoing_on_screen_2
                                || curTarget2DirectDragStatus == DirectDragStatus.across_end_from_screen_1)
                            {
                                curTarget2DirectDragStatus = DirectDragStatus.drag_phase2_end_on_screen_2;
                            }
                            else
                            {
                                curTarget2DirectDragStatus = DirectDragStatus.wait_for_drag_on_2;
                            }
                        }
                    }
                    else if (Input.GetMouseButton(0))
                    {
                        if (dragSuccess)
                        {
                            Vector3 curTouchPosInWorld = processScreenPosToGetWorldPosAtZeroZ(Input.mousePosition);
                            Vector3 offset = curTouchPosInWorld - dragStartTouchPosInWorld;
                            Vector3 intentPos = dragStartTargetPos + offset;
                            if (intentPos.x < maxX && intentPos.y > minY && intentPos.y < maxY)
                            {
                                targetVisualizer.moveTarget(intentPos);
                            }
                            if (curTarget2DirectDragStatus == DirectDragStatus.across_end_from_screen_1)
                            {
                                curTarget2DirectDragStatus = DirectDragStatus.drag_phase2_ongoing_on_screen_2;
                            }
                        }
                    }
                    else
                    {
                        dragSuccess = false;
                    }
#elif UNITY_IOS || UNITY_ANDROID
                    if (Input.touchCount == 1)
                    {
                        Touch touch = Input.GetTouch(0);
                        if (touch.phase == TouchPhase.Ended)
                        {
                            if (dragSuccess)
                            {
                                Vector3 curTouchPosInWorld = processScreenPosToGetWorldPosAtZeroZ(touch.position);
                                Vector3 offset = curTouchPosInWorld - dragStartTouchPosInWorld;
                                Vector3 intentPos = dragStartTargetPos + offset;
                                if (intentPos.x > minX && intentPos.y > minY && intentPos.y < maxY)
                                {
                                    targetVisualizer.moveTarget(intentPos);
                                }
                                targetVisualizer.inactiveTarget();
                                if (curTarget2DirectDragStatus == DirectDragStatus.drag_phase2_ongoing_on_screen_2
                                    || curTarget2DirectDragStatus == DirectDragStatus.across_end_from_screen_1)
                                {
                                    curTarget2DirectDragStatus = DirectDragStatus.drag_phase2_end_on_screen_2;
                                }
                                else
                                {
                                    curTarget2DirectDragStatus = DirectDragStatus.wait_for_drag_on_2;
                                }
                            }
                        }
                        else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                        {
                            if (dragSuccess)
                            {
                                Vector3 curTouchPosInWorld = processScreenPosToGetWorldPosAtZeroZ(touch.position);
                                Vector3 offset = curTouchPosInWorld - dragStartTouchPosInWorld;
                                Vector3 intentPos = dragStartTargetPos + offset;
                                if (intentPos.x > minX && intentPos.y > minY && intentPos.y < maxY)
                                {
                                    targetVisualizer.moveTarget(intentPos);
                                }
                                if (curTarget2DirectDragStatus == DirectDragStatus.across_end_from_screen_1)
                                {
                                    curTarget2DirectDragStatus = DirectDragStatus.drag_phase2_ongoing_on_screen_2;
                                }
                            }
                        }
                    }
                    else
                    {
                        dragSuccess = false;
                    }
#endif
                    if (curTarget2DirectDragStatus == DirectDragStatus.drag_phase2_on_screen_2
                        && (targetVisualizer.getTargetPosition().x - targetVisualizer.getTargetLocalScale().x / 2) > leftBound)
                    {
                        GlobalController.Instance.demoTarget1DirectDragPosition = targetVisualizer.getTargetPosition();
                        curTarget2DirectDragStatus = DirectDragStatus.across_end_from_screen_1;
                    }
                }
                else if (curTarget2DirectDragStatus == DirectDragStatus.drag_phase2_end_on_screen_2)
                {
                    curTarget2DirectDragStatus = DirectDragStatus.inactive_on_screen_2;
                    GlobalController.Instance.demoTarget2Status = TargetStatus.total_on_screen_2;
                }
            }
            else if (GlobalController.Instance.demoTarget2Status == TargetStatus.total_on_screen_2)
            {
                if (curTarget2DirectDragStatus == DirectDragStatus.inactive_on_screen_2)
                {
#if UNITY_ANDROID && UNITY_EDITOR
                    if (Input.GetMouseButtonDown(0))
                    {
                        dragSuccess = process1Touch4Target2(Input.mousePosition, 0);
                        if (dragSuccess)
                        {
                            targetVisualizer.activeTarget();
                            dragStartTouchPosInWorld = processScreenPosToGetWorldPosAtZeroZ(Input.mousePosition);
                            dragStartTargetPos = targetVisualizer.getTargetPosition();
                            curTarget2DirectDragStatus = DirectDragStatus.drag_phase1_on_screen_2;
                        }
                    }
                    else
                    {
                        dragSuccess = false;
                    }
#elif UNITY_IOS || UNITY_ANDROID
                    if (Input.touchCount == 1)
                    {
                        Touch touch = Input.GetTouch(0);
                        if (touch.phase == TouchPhase.Began)
                        {
                            dragSuccess = process1Touch4Target2(touch.position, 0);
                            if (dragSuccess)
                            {
                                targetVisualizer.activeTarget();
                                dragStartTouchPosInWorld = processScreenPosToGetWorldPosAtZeroZ(touch.position);
                                dragStartTargetPos = targetVisualizer.getTargetPosition();
                                curTarget2DirectDragStatus = DirectDragStatus.drag_phase1_on_screen_2;
                            }
                        }
                    }
                    else
                    {
                        dragSuccess = false;
                    }
#endif
                }
                else if (curTarget2DirectDragStatus == DirectDragStatus.drag_phase1_on_screen_2
                       || curTarget2DirectDragStatus == DirectDragStatus.across_from_screen_2)
                {
#if UNITY_ANDROID && UNITY_EDITOR
                    if (Input.GetMouseButtonUp(0))
                    {
                        if (dragSuccess)
                        {
                            Vector3 curTouchPosInWorld = processScreenPosToGetWorldPosAtZeroZ(Input.mousePosition);
                            Vector3 offset = curTouchPosInWorld - dragStartTouchPosInWorld;
                            Vector3 intentPos = dragStartTargetPos + offset;
                            if (intentPos.x > minX && intentPos.y > minY && intentPos.y < maxY)
                            {
                                targetVisualizer.moveTarget(intentPos);
                            }
                            targetVisualizer.inactiveTarget();
                            if (curTarget2DirectDragStatus == DirectDragStatus.across_from_screen_2)
                            {
                                curTarget2DirectDragStatus = DirectDragStatus.drag_phase1_end_on_screen_2;
                            }
                            else
                            {
                                curTarget2DirectDragStatus = DirectDragStatus.inactive_on_screen_2;
                            }
                        }
                    }
                    else if (Input.GetMouseButton(0))
                    {
                        if (dragSuccess)
                        {
                            Vector3 curTouchPosInWorld = processScreenPosToGetWorldPosAtZeroZ(Input.mousePosition);
                            Vector3 offset = curTouchPosInWorld - dragStartTouchPosInWorld;
                            Vector3 intentPos = dragStartTargetPos + offset;
                            if (intentPos.x < maxX && intentPos.y > minY && intentPos.y < maxY)
                            {
                                targetVisualizer.moveTarget(intentPos);
                            }
                        }
                    }
                    else
                    {
                        dragSuccess = false;
                    }
#elif UNITY_IOS || UNITY_ANDROID
                    if (Input.touchCount == 1)
                    {
                        Touch touch = Input.GetTouch(0);
                        if (touch.phase == TouchPhase.Ended)
                        {
                            if (dragSuccess)
                            {
                                Vector3 curTouchPosInWorld = processScreenPosToGetWorldPosAtZeroZ(touch.position);
                                Vector3 offset = curTouchPosInWorld - dragStartTouchPosInWorld;
                                Vector3 intentPos = dragStartTargetPos + offset;
                                if (intentPos.x > minX && intentPos.y > minY && intentPos.y < maxY)
                                {
                                    targetVisualizer.moveTarget(intentPos);
                                }
                                targetVisualizer.inactiveTarget();
                                if (curTarget2DirectDragStatus == DirectDragStatus.across_from_screen_2)
                                {
                                    curTarget2DirectDragStatus = DirectDragStatus.drag_phase1_end_on_screen_2;
                                }
                                else
                                {
                                    curTarget2DirectDragStatus = DirectDragStatus.inactive_on_screen_2;
                                }
                            }
                        }
                        else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                        {
                            if (dragSuccess)
                            {
                                Vector3 curTouchPosInWorld = processScreenPosToGetWorldPosAtZeroZ(touch.position);
                                Vector3 offset = curTouchPosInWorld - dragStartTouchPosInWorld;
                                Vector3 intentPos = dragStartTargetPos + offset;
                                if (intentPos.x < maxX && intentPos.y > minY && intentPos.y < maxY)
                                {
                                    targetVisualizer.moveTarget(intentPos);
                                }
                            }
                        }
                    }
                    else
                    {
                        dragSuccess = false;
                    }
#endif
                    if (curTarget2DirectDragStatus == DirectDragStatus.drag_phase1_on_screen_2
                        && (targetVisualizer.getTargetPosition().x - targetVisualizer.getTargetLocalScale().x / 2) < leftBound)
                    {
                        GlobalController.Instance.demoTarget2DirectDragPosition = targetVisualizer.getTargetPosition();
                        curTarget2DirectDragStatus = DirectDragStatus.across_from_screen_2;
                    }
                }
                else if (curTarget2DirectDragStatus == DirectDragStatus.drag_phase1_end_on_screen_2)
                {
                    curTarget2DirectDragStatus = DirectDragStatus.wait_for_drag_on_1;
                }
                else if (curTarget2DirectDragStatus == DirectDragStatus.wait_for_drag_on_1)
                {
                    if (GlobalController.Instance.refreshTarget2
                        && GlobalController.Instance.demoTarget1DirectDragStatus == DirectDragStatus.drag_phase2_on_screen_1)
                    {
                        targetVisualizer.activeTarget();
                        targetVisualizer.moveTarget(GlobalController.Instance.demoTarget2DirectDragPosition);
                        GlobalController.Instance.refreshTarget2 = false;
                    }
                    if (GlobalController.Instance.demoTarget1DirectDragStatus == DirectDragStatus.across_end_from_screen_2)
                    {
                        targetVisualizer.hideTarget();
                    }
                    else if (GlobalController.Instance.demoTarget1DirectDragStatus == DirectDragStatus.drag_phase2_end_on_screen_1)
                    {
                        GlobalController.Instance.demoTarget2Status = TargetStatus.total_on_screen_1;
                        curTarget2DirectDragStatus = DirectDragStatus.inactive_on_screen_1;
                    }
                }
            }

            curTarget2Pos = targetVisualizer.getTargetPosition();
            GlobalController.Instance.demoTarget2DirectDragPosition = curTarget2Pos;
            GlobalController.Instance.demoTarget2DirectDragStatus = curTarget2DirectDragStatus;
            if ((curTarget2DirectDragStatus == DirectDragStatus.across_from_screen_2 && prevTarget2Pos != curTarget2Pos)
                || (curTarget2DirectDragStatus == DirectDragStatus.drag_phase2_on_screen_2 && prevTarget2Pos != curTarget2Pos)
                || ( (curTarget2DirectDragStatus != prevTarget2DirectDragStatus) &&
                ( curTarget2DirectDragStatus == DirectDragStatus.drag_phase1_end_on_screen_2
                || curTarget2DirectDragStatus == DirectDragStatus.drag_phase2_on_screen_2
                || curTarget2DirectDragStatus == DirectDragStatus.across_end_from_screen_1
                || curTarget2DirectDragStatus == DirectDragStatus.drag_phase2_end_on_screen_2) ) )
            {
                GlobalController.Instance.client.prepareNewMessage4Server(MessageType.DirectDragInfo);
            }
            prevTarget2DirectDragStatus = curTarget2DirectDragStatus;
            prevTarget2Pos = curTarget2Pos;

            uiController.updateDebugInfo(curTarget2DirectDragStatus.ToString());
            uiController.updateStatusInfo(GlobalController.Instance.demoTarget1DirectDragStatus.ToString());
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

    public void initParamsWhenTargetOnScreen1()
    {
        prevTarget2DirectDragStatus = curTarget2DirectDragStatus = DirectDragStatus.inactive_on_screen_1;
        //prevTarget2Pos = curTarget2Pos = target2.transform.position;
        if (GlobalController.Instance)
        {
            GlobalController.Instance.demoTarget1DirectDragStatus
           = GlobalController.Instance.demoTarget2DirectDragStatus
           = DirectDragStatus.inactive_on_screen_1;
            
        }
    }
    public void initParamsWhenTargetOnScreen2()
    {
        prevTarget2DirectDragStatus = curTarget2DirectDragStatus = DirectDragStatus.inactive_on_screen_2;
        //prevTarget2Pos = curTarget2Pos = target2.transform.position;
        if (GlobalController.Instance)
        {
            GlobalController.Instance.demoTarget1DirectDragStatus
           = GlobalController.Instance.demoTarget2DirectDragStatus
           = DirectDragStatus.inactive_on_screen_2;
            GlobalController.Instance.demoTarget2DirectDragPosition = curTarget2Pos;
        }
    }
}
