using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PublicLabFactors;
using static PublicDragParams;
using System;

public class demoHoldTapProcessor : MonoBehaviour
{
    public GameObject target1;
    public demoUIController uiController;
    public demoTouchVisualizer touchVisualizer;
    public demoTargetVisualizer targetVisualizer;

    private HoldTapStatus curTarget1HoldTapStatus, prevTarget1HoldTapStatus;

    private bool touchSuccess;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GlobalController.Instance && GlobalController.Instance.demoDragType == DragType.hold_tap)
        {
            if (GlobalController.Instance.demoTarget1Status == TargetStatus.total_on_screen_1)
            {
                if ( curTarget1HoldTapStatus == HoldTapStatus.inactive_on_screen_1 )
                {
                    targetVisualizer.showTarget();
                    if (Input.touchCount == 1)
                    {
                        Touch touch = Input.GetTouch(0);
                        touchSuccess = process1Touch4Target1(touch.position, 0);
                        if( touchSuccess && touch.phase == TouchPhase.Began )
                        {
                            targetVisualizer.activeTarget();
                            curTarget1HoldTapStatus = HoldTapStatus.holding_on_screen_1;
                        }
                    } else
                    {
                        touchSuccess = false;
                    }
                }
                else if (curTarget1HoldTapStatus == HoldTapStatus.holding_on_screen_1)
                {
                    if (Input.touchCount == 1)
                    {
                        Touch touch = Input.GetTouch(0);
                        bool touchSuccess = process1Touch4Target1(touch.position, 0);
                        if (touchSuccess)
                        {
                            if (touch.phase == TouchPhase.Ended)
                            {
                                targetVisualizer.inactiveTarget();
                                if (GlobalController.Instance.demoTarget2HoldTapStatus == HoldTapStatus.tapped_on_screen_2)
                                {
                                    curTarget1HoldTapStatus = HoldTapStatus.tapped_on_screen_2;
                                } else
                                {
                                    curTarget1HoldTapStatus = HoldTapStatus.inactive_on_screen_1;
                                }
                            }
                        }
                        else
                        {
                            targetVisualizer.inactiveTarget();
                            curTarget1HoldTapStatus = HoldTapStatus.inactive_on_screen_1;
                        }
                    }
                    else
                    {
                        targetVisualizer.inactiveTarget();
                        touchSuccess = false;
                        curTarget1HoldTapStatus = HoldTapStatus.inactive_on_screen_1;
                    }
                }
                else if (curTarget1HoldTapStatus == HoldTapStatus.tapped_on_screen_2)
                {
                    targetVisualizer.hideTarget();
                    curTarget1HoldTapStatus = HoldTapStatus.inactive_on_screen_2;
                    GlobalController.Instance.demoTarget1Status = TargetStatus.total_on_screen_2;
                }
            }
            else if (GlobalController.Instance.demoTarget1Status == TargetStatus.total_on_screen_2)
            {
                if (GlobalController.Instance.demoTarget2HoldTapStatus == HoldTapStatus.holding_on_screen_2
                    && (curTarget1HoldTapStatus == HoldTapStatus.inactive_on_screen_2
                     || curTarget1HoldTapStatus == HoldTapStatus.tapped_on_screen_1))
                {
#if UNITY_ANDROID && UNITY_EDITOR
                    if (Input.GetMouseButtonDown(0))
                    {
                        touchVisualizer.adjustTouchOutlinePosition(Input.mousePosition);
                        touchVisualizer.showTouchOutline();
                        curTarget1HoldTapStatus = HoldTapStatus.tapped_on_screen_1;
                    }
                    else if (Input.GetMouseButton(0))
                    {
                        touchVisualizer.adjustTouchOutlinePosition(Input.mousePosition);
                        Vector3 intentPos = processScreenPosToGetWorldPosAtZeroZ(Input.mousePosition);
                        targetVisualizer.moveTarget(intentPos);
                    }
                    else if (Input.GetMouseButtonUp(0))
                    {
                        touchVisualizer.hideTouchOutline();
                        Vector3 intentPos = processScreenPosToGetWorldPosAtZeroZ(Input.mousePosition);
                        targetVisualizer.moveTarget(intentPos);
                    }
#elif UNITY_IOS || UNITY_ANDROID
                    if ( Input.touchCount == 1)
                    {
                        Touch touch = Input.GetTouch(0);
                        if (touch.phase == TouchPhase.Began)
                        {
                            touchVisualizer.adjustTouchOutlinePosition(touch.position);
                            touchVisualizer.showTouchOutline();
                            curTarget1HoldTapStatus = HoldTapStatus.tapped_on_screen_1;
                        }
                        else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                        {
                            touchVisualizer.adjustTouchOutlinePosition(touch.position);
                            Vector3 intentPos = processScreenPosToGetWorldPosAtZeroZ(touch.position);
                            targetVisualizer.moveTarget(intentPos);
                        }
                        else if (touch.phase == TouchPhase.Ended)
                        {
                            touchVisualizer.hideTouchOutline();
                            Vector3 intentPos = processScreenPosToGetWorldPosAtZeroZ(touch.position);
                            targetVisualizer.moveTarget(intentPos);
                        }
                    }
#endif
                }
                else if (GlobalController.Instance.demoTarget2HoldTapStatus == HoldTapStatus.inactive_on_screen_1)
                {
                    touchVisualizer.hideTouchOutline();
                    targetVisualizer.showTarget();
                    curTarget1HoldTapStatus = HoldTapStatus.inactive_on_screen_1;
                    GlobalController.Instance.demoTarget1Status = TargetStatus.total_on_screen_1;
                }
                else if (GlobalController.Instance.demoTarget2HoldTapStatus == HoldTapStatus.inactive_on_screen_2)
                {
                    touchVisualizer.hideTouchOutline();
                    targetVisualizer.hideTarget();
                    curTarget1HoldTapStatus = HoldTapStatus.inactive_on_screen_2;
                    GlobalController.Instance.demoTarget1Status = TargetStatus.total_on_screen_2;
                }
            }
            GlobalController.Instance.demoTarget1HoldTapStatus = curTarget1HoldTapStatus;
            if (curTarget1HoldTapStatus != prevTarget1HoldTapStatus
                && curTarget1HoldTapStatus != HoldTapStatus.holding_on_screen_2
                && curTarget1HoldTapStatus != HoldTapStatus.tapped_on_screen_2)
            {
                GlobalController.Instance.server.prepareNewMessage4Client(MessageType.HoldTapInfo);
            }
            prevTarget1HoldTapStatus = curTarget1HoldTapStatus;

            uiController.updateDebugInfo(curTarget1HoldTapStatus.ToString());
            uiController.updateStatusInfo(GlobalController.Instance.demoTarget2HoldTapStatus.ToString());
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

    public void initParamsWhenTargetOnScreen1()
    {
        prevTarget1HoldTapStatus = curTarget1HoldTapStatus = HoldTapStatus.inactive_on_screen_1;
        if (GlobalController.Instance)
        {
            GlobalController.Instance.demoTarget1HoldTapStatus
           = GlobalController.Instance.demoTarget2HoldTapStatus
           = HoldTapStatus.inactive_on_screen_1;
        }
    }
    public void initParamsWhenTargetOnScreen2()
    {
        prevTarget1HoldTapStatus = curTarget1HoldTapStatus = HoldTapStatus.inactive_on_screen_2;
        if (GlobalController.Instance)
        {
            GlobalController.Instance.demoTarget1HoldTapStatus
           = GlobalController.Instance.demoTarget2HoldTapStatus
           = HoldTapStatus.inactive_on_screen_2;
        }
    }
}
