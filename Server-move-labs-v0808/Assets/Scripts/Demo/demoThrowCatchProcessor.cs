using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PublicLabFactors;
using static PublicDragParams;

public class demoThrowCatchProcessor : MonoBehaviour
{
    public GameObject target1;
    public demoUIController uiController;
    public demoTouchVisualizer touchVisualizer;
    public demoTargetVisualizer targetVisualizer;
    public demoFlickerVisualizer flickerVisualizer;

    private ThrowCatchStatus curTarget1ThrowCatchStatus, prevTarget1ThrowCatchStatus;
    private bool touchSuccess;
    private ThrowCatchPhase1Result throwResult;
    private Vector3 throwStartPos, throwLastPos, throwEndPos;
    private Vector3 moveStartPos, moveEndPos, actualEndPos;
    private float throwStartTime, throwEndTime, catchEndTime;
    private float thisMoveDuration;
    private Vector2 throwStartTouchPoint, throwEndTouchPoint;
    private float throwTouchVelocity, throwTouchDistance;
    private Vector3 prevMovingPos, curMovingPos;
    private Vector3 startTouchPointInWorld;

    private float rightBound;

    private const float x3rdPoint = 4f;
    private const float unitMoveDuration = 0.3f; // The time the target moves one screen width as the unit time

    private const float minX = DRAG_MIN_X, maxX = DRAG_MAX_X, minY = DRAG_MIN_Y, maxY = DRAG_MAX_Y;
    private const float minFlickDistance = 60f;
    private const float junctionX = maxX;

    //private const int SWIPE_MIN_DISTANCE = 120;
    //private const int SWIPE_MAX_OFF_PATH = 250;
    //private const int SWIPE_THRESHOLD_VELOCITY = 200;

    private const float FLING_FRICTION = 1.1f;
    private const float FLING_MIN_VELOCITY = 200f; // 移动最大速度
    private const float FLING_MIN_DISTANCE = 6f;  // ios 120

    private const bool isSecondVersion = true;

    // Start is called before the first frame update
    void Start()
    {
        touchSuccess = false;
        rightBound = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0)).x;
        initParamsWhenTargetOnScreen1();
    }

    // Update is called once per frame
    void Update()
    {
        if (GlobalController.Instance && GlobalController.Instance.demoDragType == DragType.throw_catch)
        {
            if (GlobalController.Instance.demoTarget1Status == TargetStatus.total_on_screen_1)
            {
                if (curTarget1ThrowCatchStatus == ThrowCatchStatus.inactive_on_screen_1)
                {
#if UNITY_ANDROID && UNITY_EDITOR
                    if (Input.GetMouseButtonDown(0))
                    {
                        //throwResult = ThrowCatchPhase1Result.tc_throw_drag;
                        touchSuccess = process1Touch4Target1(Input.mousePosition, 0);
                        if (touchSuccess)
                        {
                            throwStartTime = Time.time;
                            throwStartPos = throwLastPos = target1.transform.position;
                            throwStartTouchPoint = Input.mousePosition;
                            startTouchPointInWorld = processScreenPosToGetWorldPosAtZeroZ(Input.mousePosition);
                            targetVisualizer.activeTarget();
                            touchVisualizer.adjustPathStartNodeWithWorldPos(target1.transform.position);
                        }
                    }
                    else if (Input.GetMouseButton(0))
                    {
                        if (touchSuccess)
                        {
                            Vector3 curTouchPointInWorld = processScreenPosToGetWorldPosAtZeroZ(Input.mousePosition);
                            //Vector3 offset = curTouchPointInWorld - throwStartPos;
                            Vector3 offset = curTouchPointInWorld - startTouchPointInWorld;
                            Vector3 intentPos = throwStartPos + offset;
                            if (   intentPos.x > minX && intentPos.x < maxX
                                && intentPos.y > minY && intentPos.y < maxY )
                            {
                                targetVisualizer.moveTarget(throwStartPos + offset);
                                //touchVisualizer.adjustPathEndNodeWithWorldPos(target1.transform.position);
                                throwLastPos = target1.transform.position;
                            }
                        }
                    }
                    else if (Input.GetMouseButtonUp(0))
                    {
                        if (touchSuccess)
                        {
                            Vector3 curTouchPointInWorld = processScreenPosToGetWorldPosAtZeroZ(Input.mousePosition);
                            Vector3 offset = curTouchPointInWorld - startTouchPointInWorld;
                            Vector3 intentPos = throwStartPos + offset;
                            if (intentPos.x > minX && intentPos.x < maxX
                                && intentPos.y > minY && intentPos.y < maxY)
                            {
                                targetVisualizer.moveTarget(throwStartPos + offset);
                                //touchVisualizer.adjustPathEndNodeWithWorldPos(target1.transform.position);
                            }

                            throwEndTime = Time.time;
                            throwEndPos = target1.transform.position;
                            throwEndTouchPoint = Input.mousePosition;

                            throwTouchDistance = Mathf.Abs((throwEndTouchPoint - throwStartTouchPoint).magnitude);
                            throwTouchVelocity = throwTouchDistance / (throwEndTime - throwStartTime);
                            

                            uiController.updateStatusInfo("PC-D/V:" + throwTouchDistance.ToString()+ "/" + throwTouchVelocity.ToString());

                            if ( throwTouchDistance > FLING_MIN_DISTANCE
                                && throwTouchVelocity > FLING_MIN_VELOCITY)
                            {
                                //throwResult = ThrowCatchPhase1Result.tc_throw_flick;
                                curTarget1ThrowCatchStatus = ThrowCatchStatus.throw_flicked_on_screen_1;
                            } else
                            {
                                targetVisualizer.inactiveTarget();
                                //throwResult = ThrowCatchPhase1Result.tc_throw_drag;
                                curTarget1ThrowCatchStatus = ThrowCatchStatus.throw_dragged_on_screen_1;
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
                        if (touch.phase == TouchPhase.Began)
                        {
                            throwResult = ThrowCatchPhase1Result.tc_throw_drag;
                            touchSuccess = process1Touch4Target1(touch.position, 0);
                            if (touchSuccess)
                            {
                                throwStartTime = Time.time;
                                throwStartPos = throwLastPos = target1.transform.position;
                                startTouchPointInWorld = processScreenPosToGetWorldPosAtZeroZ(touch.position);
                                targetVisualizer.activeTarget();
                                touchVisualizer.adjustPathStartNodeWithWorldPos(target1.transform.position);
                            }
                        }
                        else if (touch.phase == TouchPhase.Moved)
                        {
                            if (touchSuccess)
                            {
                                Vector3 curTouchPointInWorld = processScreenPosToGetWorldPosAtZeroZ(touch.position);
                                Vector3 offset = curTouchPointInWorld - startTouchPointInWorld;
                                Vector3 intentPos = throwStartPos + offset;
                                if (intentPos.x > minX && intentPos.x < maxX
                                    && intentPos.y > minY && intentPos.y < maxY)
                                {
                                    targetVisualizer.moveTarget(throwStartPos + offset);
                                    //touchVisualizer.adjustPathEndNodeWithWorldPos(target1.transform.position);
                                    throwLastPos = target1.transform.position;
                                }
                            }
                        }
                        else if (touch.phase == TouchPhase.Ended)
                        {
                            if (touchSuccess)
                            {
                                Vector3 curTouchPointInWorld = processScreenPosToGetWorldPosAtZeroZ(touch.position);
                                Vector3 offset = curTouchPointInWorld - startTouchPointInWorld;
                                Vector3 intentPos = throwStartPos + offset;
                                if (intentPos.x > minX && intentPos.x < maxX
                                    && intentPos.y > minY && intentPos.y < maxY)
                                {
                                    targetVisualizer.moveTarget(throwStartPos + offset);
                                    //touchVisualizer.adjustPathEndNodeWithWorldPos(target1.transform.position);
                                }

                                throwEndTime = Time.time;
                                throwEndPos = target1.transform.position;
                                throwEndTouchPoint = touch.position;

                                //throwTouchDistance = Mathf.Abs((throwEndTouchPoint - throwStartTouchPoint).magnitude);
                                //throwTouchVelocity = throwTouchDistance / (throwEndTime - throwStartTime);
                                throwTouchDistance = touch.deltaPosition.magnitude;
                                throwTouchVelocity = throwTouchDistance / touch.deltaTime;

                                uiController.updateStatusInfo("TD-D/V:" + throwTouchDistance.ToString() + "/" + throwTouchVelocity.ToString());

                                if (throwTouchDistance > FLING_MIN_DISTANCE
                                    && throwTouchVelocity > FLING_MIN_VELOCITY)
                                {
                                    throwResult = ThrowCatchPhase1Result.tc_throw_flick;
                                    curTarget1ThrowCatchStatus = ThrowCatchStatus.throw_flicked_on_screen_1;
                                }
                                else
                                {
                                    targetVisualizer.inactiveTarget();
                                    throwResult = ThrowCatchPhase1Result.tc_throw_drag;
                                    curTarget1ThrowCatchStatus = ThrowCatchStatus.throw_dragged_on_screen_1;
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
                else if (curTarget1ThrowCatchStatus == ThrowCatchStatus.throw_flicked_on_screen_1)
                {
                    moveStartPos = throwEndPos;
                    moveEndPos = calculate3rdPointOnLine(throwStartPos, throwEndPos);
                    prevMovingPos = curMovingPos = moveStartPos;
                    //thisMoveDuration = (moveEndPos - moveStartPos).magnitude / (maxX - minX) * unitMoveDuration;
                    Vector2 moveStartTouchPoint = uiController.renderCamera.WorldToScreenPoint(moveStartPos);
                    Vector2 moveEndTouchPoint = uiController.renderCamera.WorldToScreenPoint(moveEndPos);
                    float moveDistance = (moveEndTouchPoint - moveStartTouchPoint).magnitude;
                    thisMoveDuration = calculateMoveTime(-FLING_FRICTION, moveDistance, throwTouchVelocity);
                    uiController.updatePosInfo(thisMoveDuration.ToString());
                    curTarget1ThrowCatchStatus = ThrowCatchStatus.wait_for_t1_move_phase1;
                }
                else if (curTarget1ThrowCatchStatus == ThrowCatchStatus.wait_for_t1_move_phase1)
                {
                    float t = (Time.time - throwEndTime) / thisMoveDuration;
                    target1.transform.position = new Vector3(
                        Mathf.SmoothStep(moveStartPos.x, moveEndPos.x, t),
                        Mathf.SmoothStep(moveStartPos.y, moveEndPos.y, t),
                        0f);
                    curMovingPos = target1.transform.position;
                    if (curMovingPos != prevMovingPos)
                    {
                        prevMovingPos = curMovingPos;
                    }
                    else
                    {
                        if (curMovingPos.x == junctionX)
                        {
                            //touchVisualizer.showLocationLine();
                            curTarget1ThrowCatchStatus = ThrowCatchStatus.throw_successed_on_screen_1;
                            // send message later
                        } else
                        {
                            targetVisualizer.inactiveTarget();
                            curTarget1ThrowCatchStatus = ThrowCatchStatus.throw_failed_on_screen_1;
                        }
                    }
                }
                else if (curTarget1ThrowCatchStatus == ThrowCatchStatus.throw_dragged_on_screen_1)
                {
                     curTarget1ThrowCatchStatus = ThrowCatchStatus.throw_failed_on_screen_1;
                }
                else if (curTarget1ThrowCatchStatus == ThrowCatchStatus.throw_successed_on_screen_1)
                {
                    flickerVisualizer.startFlicker();
                    curTarget1ThrowCatchStatus = ThrowCatchStatus.wait_for_catch_on_2;
                }
                else if (curTarget1ThrowCatchStatus == ThrowCatchStatus.throw_failed_on_screen_1)
                {
                    targetVisualizer.inactiveTarget();
                    curTarget1ThrowCatchStatus = ThrowCatchStatus.inactive_on_screen_1;
                }
                else if (curTarget1ThrowCatchStatus == ThrowCatchStatus.wait_for_catch_on_2)
                {
                    if ( GlobalController.Instance.demoTarget2ThrowCatchStatus == ThrowCatchStatus.catch_start_on_screen_2
                        || GlobalController.Instance.demoTarget2ThrowCatchStatus == ThrowCatchStatus.t1_move_phase2_ongoing
                        || GlobalController.Instance.demoTarget2ThrowCatchStatus == ThrowCatchStatus.t1_move_phase2_acrossing_over
                        || GlobalController.Instance.demoTarget2ThrowCatchStatus == ThrowCatchStatus.catch_end_on_screen_2)
                    {
                        flickerVisualizer.stopFlicker();
                        flickerVisualizer.showFlickerObjects();
                    }
                    if (GlobalController.Instance.demoTarget2ThrowCatchStatus == ThrowCatchStatus.t1_move_phase2_ongoing)
                    {
                        targetVisualizer.moveTarget(GlobalController.Instance.demoTarget1ThrowCatchPosition);
                    }
                    else if (GlobalController.Instance.demoTarget2ThrowCatchStatus == ThrowCatchStatus.t1_move_phase2_acrossing_over)
                    {
                        targetVisualizer.hideTarget();
                    }
                    else if (GlobalController.Instance.demoTarget2ThrowCatchStatus == ThrowCatchStatus.catch_end_on_screen_2)
                    {
                        targetVisualizer.hideTarget();
                        curTarget1ThrowCatchStatus = ThrowCatchStatus.catch_end_on_screen_2;
                    }
                }
                else if (curTarget1ThrowCatchStatus == ThrowCatchStatus.catch_end_on_screen_2)
                {
                    flickerVisualizer.hideFlickerObjects();
                    targetVisualizer.hideTarget();
                    touchVisualizer.hideLocationLine();
                    curTarget1ThrowCatchStatus = ThrowCatchStatus.inactive_on_screen_2;
                    GlobalController.Instance.demoTarget1Status = TargetStatus.total_on_screen_2;
                }
            }
            else if (GlobalController.Instance.demoTarget1Status == TargetStatus.total_on_screen_2)
            {
                if (curTarget1ThrowCatchStatus == ThrowCatchStatus.inactive_on_screen_2
                    && GlobalController.Instance.demoTarget2ThrowCatchStatus == ThrowCatchStatus.throw_successed_on_screen_2)
                {
                    flickerVisualizer.startFlicker();
                    targetVisualizer.moveTarget(GlobalController.Instance.demoTarget1ThrowCatchPosition);
                    uiController.updateStatusInfo(targetVisualizer.getTargetPosition().ToString());
                    curTarget1ThrowCatchStatus = ThrowCatchStatus.throw_successed_on_screen_2;
                }
                else if ( ( curTarget1ThrowCatchStatus == ThrowCatchStatus.throw_successed_on_screen_2 ||
                       curTarget1ThrowCatchStatus == ThrowCatchStatus.catch_start_on_screen_1 )
                    && GlobalController.Instance.demoTarget2ThrowCatchStatus == ThrowCatchStatus.throw_successed_on_screen_2)
                {
#if UNITY_ANDROID && UNITY_EDITOR
                    if (Input.GetMouseButtonDown(0))
                    {
                        flickerVisualizer.stopFlicker();
                        flickerVisualizer.showFlickerObjects();
                        touchVisualizer.adjustTouchOutlinePosition(Input.mousePosition);
                        touchVisualizer.showTouchOutline();
                        curTarget1ThrowCatchStatus = ThrowCatchStatus.catch_start_on_screen_1;
                    }
                    else if (Input.GetMouseButton(0))
                    {
                        touchVisualizer.adjustTouchOutlinePosition(Input.mousePosition);
                        moveStartPos = targetVisualizer.getTargetPosition();
                        moveEndPos = processScreenPosToGetWorldPosAtZeroZ(Input.mousePosition);
                    }
                    else if (Input.GetMouseButtonUp(0))
                    {
                        touchVisualizer.hideTouchOutline();
                        moveStartPos = targetVisualizer.getTargetPosition();
                        moveEndPos = processScreenPosToGetWorldPosAtZeroZ(Input.mousePosition);
                        catchEndTime = Time.time;
                        thisMoveDuration = (moveEndPos - moveStartPos).magnitude / (maxX - minX) * unitMoveDuration;
                        uiController.updatePosInfo(thisMoveDuration.ToString());
                        curTarget1ThrowCatchStatus = ThrowCatchStatus.t2_move_phase2_ongoing;
                    }
#elif UNITY_IOS || UNITY_ANDROID
                    if ( Input.touchCount == 1 )
                    {
                        Touch touch = Input.GetTouch(0);
                        if (touch.phase == TouchPhase.Began)
                        {
                            flickerVisualizer.stopFlicker();
                            flickerVisualizer.showFlickerObjects();
                            touchVisualizer.adjustTouchOutlinePosition(touch.position);
                            touchVisualizer.showTouchOutline();
                            curTarget1ThrowCatchStatus = ThrowCatchStatus.catch_start_on_screen_1;
                        }
                        else if (touch.phase == TouchPhase.Moved)
                        {
                            touchVisualizer.adjustTouchOutlinePosition(touch.position);
                            moveStartPos = targetVisualizer.getTargetPosition();
                            moveEndPos = processScreenPosToGetWorldPosAtZeroZ(touch.position);
                        }
                        else if ( touch.phase == TouchPhase.Ended )
                        {
                            touchVisualizer.hideTouchOutline();
                            moveStartPos = targetVisualizer.getTargetPosition();
                            moveEndPos = processScreenPosToGetWorldPosAtZeroZ(touch.position);
                            catchEndTime = Time.time;
                            thisMoveDuration = (moveEndPos - moveStartPos).magnitude / (maxX - minX) * unitMoveDuration;
                            uiController.updatePosInfo(thisMoveDuration.ToString());
                            curTarget1ThrowCatchStatus = ThrowCatchStatus.t2_move_phase2_ongoing;
                        }
                    } 
#endif
                }
                else if (curTarget1ThrowCatchStatus == ThrowCatchStatus.t2_move_phase2_ongoing
                    || curTarget1ThrowCatchStatus == ThrowCatchStatus.t2_move_phase2_acrossing_over)
                {
                    float t = (Time.time - catchEndTime) / thisMoveDuration;
                    targetVisualizer.moveTarget(new Vector3(
                        Mathf.SmoothStep(moveStartPos.x, moveEndPos.x, t),
                        Mathf.SmoothStep(moveStartPos.y, moveEndPos.y, t),
                        0f));
                    curMovingPos = targetVisualizer.getTargetPosition();
                    targetVisualizer.activeTarget();
                    targetVisualizer.showTarget();
                    if (curMovingPos != prevMovingPos &&
                        targetVisualizer.getTargetPosition().x + targetVisualizer.getTargetLocalScale().x / 2 >= rightBound)
                    {
                        prevMovingPos = curMovingPos;
                    }
                    else if (curMovingPos != prevMovingPos &&
                        targetVisualizer.getTargetPosition().x + targetVisualizer.getTargetLocalScale().x / 2 < rightBound)
                    {
                        prevMovingPos = curMovingPos;
                        curTarget1ThrowCatchStatus = ThrowCatchStatus.t2_move_phase2_acrossing_over;
                    }
                    else if (curMovingPos == prevMovingPos && curMovingPos.x == moveEndPos.x)
                    {
                        targetVisualizer.inactiveTarget();
                        curTarget1ThrowCatchStatus = ThrowCatchStatus.catch_end_on_screen_1;
                        // send message later
                    }
                }
                else if (curTarget1ThrowCatchStatus == ThrowCatchStatus.catch_end_on_screen_1)
                {
                    flickerVisualizer.hideFlickerObjects();
                    targetVisualizer.showTarget();
                    curTarget1ThrowCatchStatus = ThrowCatchStatus.inactive_on_screen_1;
                    GlobalController.Instance.demoTarget1Status = TargetStatus.total_on_screen_1;
                }
            }
        }

        uiController.updateDebugInfo(curTarget1ThrowCatchStatus.ToString());
        uiController.updateStatusInfo(GlobalController.Instance.demoTarget2ThrowCatchStatus.ToString());
        //uiController.updatePosInfo(throwResult.ToString());
        GlobalController.Instance.demoTarget1ThrowCatchStatus = curTarget1ThrowCatchStatus;
        GlobalController.Instance.demoTarget1ThrowCatchPosition = targetVisualizer.getTargetPosition();
        // keep with t1 st-status
        if (curTarget1ThrowCatchStatus == ThrowCatchStatus.t2_move_phase2_ongoing
            || (curTarget1ThrowCatchStatus != prevTarget1ThrowCatchStatus &&
                 ( curTarget1ThrowCatchStatus == ThrowCatchStatus.throw_successed_on_screen_1
                || curTarget1ThrowCatchStatus == ThrowCatchStatus.catch_start_on_screen_1
                || curTarget1ThrowCatchStatus == ThrowCatchStatus.t2_move_phase2_acrossing_over
                || curTarget1ThrowCatchStatus == ThrowCatchStatus.catch_end_on_screen_1) )
           )
        {
            GlobalController.Instance.server.prepareNewMessage4Client(MessageType.ThrowCatchInfo);
        }

        prevTarget1ThrowCatchStatus = curTarget1ThrowCatchStatus;
    }


    public void initParamsWhenTargetOnScreen1()
    {
        prevTarget1ThrowCatchStatus = curTarget1ThrowCatchStatus = ThrowCatchStatus.inactive_on_screen_1;
        if (GlobalController.Instance)
        {
            GlobalController.Instance.demoTarget1ThrowCatchStatus
           = GlobalController.Instance.demoTarget2ThrowCatchStatus
           = ThrowCatchStatus.inactive_on_screen_1;
        }
    }
    public void initParamsWhenTargetOnScreen2()
    {
        prevTarget1ThrowCatchStatus = curTarget1ThrowCatchStatus = ThrowCatchStatus.inactive_on_screen_2;
        if (GlobalController.Instance)
        {
            GlobalController.Instance.demoTarget1ThrowCatchStatus
           = GlobalController.Instance.demoTarget2ThrowCatchStatus
           = ThrowCatchStatus.inactive_on_screen_2;
        }
    }

    private Vector3 processScreenPosToGetWorldPosAtZeroZ(Vector2 tp)
    {
        Vector3 pos = Vector3.zero;
        pos = Camera.main.ScreenToWorldPoint(new Vector3(tp.x, tp.y, 0));
        pos.z = 0f;
        return pos;
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

    private Vector3 calculate3rdPointOnLine(Vector2 p1, Vector2 p2)
    {
        // p1-start, p2-end
        Vector3 res = Vector3.zero;
        bool isMovingRight = p2.x - p1.x > 0;
        bool isMovingDown = p2.y - p1.y > 0;
        if (p2.x - p1.x == 0 && p2.y - p1.y == 0)
        {
            // Impossible condition
            res.x = p1.x;
            res.y = p2.y;
        }
        else if (p2.x - p1.x == 0 && isMovingDown)
        {
            res.x = p1.x;
            res.y = minY;
        }
        else if (p2.x - p1.x == 0 && !isMovingDown)
        {
            res.x = p1.x;
            res.y = maxY;
        }
        else if (p2.y - p1.y == 0 && isMovingRight)
        {
            res.x = maxX;
            res.y = p1.y;
        }
        else if (p2.y - p1.y == 0 && !isMovingRight)
        {
            res.x = minX;
            res.y = p1.y;
        }
        else if (isMovingRight)
        {
            res.x = maxX;
            res.y = (res.x - p1.x) * (p2.y - p1.y) / (p2.x - p1.x) + p1.y;

        }
        else if (!isMovingRight)
        {
            res.x = minX;
            res.y = (res.x - p1.x) * (p2.y - p1.y) / (p2.x - p1.x) + p1.y;
        }

        if (res.y > maxY)
        {
            res.y = maxY;
            res.x = (res.y - p1.y) * (p2.x - p1.x) / (p2.y - p1.y) + p1.x;
        }
        else if (res.y < minY)
        {
            res.y = minY;
            res.x = (res.y - p1.y) * (p2.x - p1.x) / (p2.y - p1.y) + p1.x;
        }
        return res;
    }


    private float calculateMoveTime(float a, float s, float v0)
    {
        float coe_a = 0.5f * a;
        float coe_b = v0;
        float coe_c = -s;

        float x1 = 0f, x2 = 0f;
        if (coe_b * coe_b - 4 * coe_a * coe_c > 0)
        {

            x1 = (-coe_b + Mathf.Sqrt(coe_b * coe_b - 4f * coe_a * coe_c)) / 2f * coe_a;
            x2 = (-coe_b - Mathf.Sqrt(coe_b * coe_b - 4f * coe_a * coe_c)) / 2f * coe_a;
            Debug.Log(String.Format("一元二次方程{0}*x*x+{1}*x+{2}=0的根为：{3}\t{4}", coe_a, coe_b, coe_c, x1, x2));
        }
        else if (coe_b * coe_b - 4f * coe_a * coe_c == 0f)
        {
            x1 = (-coe_b + Mathf.Sqrt(coe_b * coe_b - 4f * coe_a * coe_c)) / 2f * coe_a;
            Debug.Log(String.Format("一元二次方程{0}*x*x+{1}*x+{2}=0的根为：{3}", coe_a, coe_b, coe_c, x1));
        }
        else
        {
            Debug.Log(String.Format("一元二次方程{0}*x*x+{1}*x+{2}=0无解！", coe_a, coe_b, coe_c));
        }

        if (x1 > 0) return x1;
        if (x2 > 0) return x2;
        return 0f;
    }
}
