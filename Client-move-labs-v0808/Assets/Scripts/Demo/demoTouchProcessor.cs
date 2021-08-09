using UnityEngine;
using System;
using static PublicLabFactors;
using UnityEngine.SceneManagement;
using static PublicDragParams;

public class demoTouchProcessor : MonoBehaviour
{
    public GameObject target2;
    public demoUIController uiController;
    public demoTouchVisualizer touchVisualizer;
    public demoTargetVisualizer targetVisualizer;
    public GameObject directDragProcessor;
    public GameObject holdTapProcessor;
    public GameObject throwCatchProcessor;

    private DragType curDragType, prevDragType;

    // Start is called before the first frame update
    void Start()
    {
        curDragType = prevDragType = DragType.direct_drag;
        if (GlobalController.Instance)
        {
            GlobalController.Instance.demoDragType = DragType.direct_drag;
        }
        initDifferentDragParams();
        Debug.Log("ScreenHeight: " + Screen.height + "; ScreenWidth: " + Screen.width);
        targetVisualizer.hideTarget();
        switchDragMode(curDragType);
    }

    void initDifferentDragParams()
    {
        directDragProcessor.GetComponent<demoDirectDragProcessor>().enabled = true;
        holdTapProcessor.GetComponent<demoHoldTapProcessor>().enabled = false;
        throwCatchProcessor.GetComponent<demoThrowCatchProcessor>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (GlobalController.Instance.serverCmdQueue.Count != 0 &&
                GlobalController.Instance.serverCmdQueue.Peek() == ServerCommand.server_say_reset_drag_type_and_position)
        {
            GlobalController.Instance.serverCmdQueue.Dequeue();
            resetDragTypeAndPosition();
        }

        curDragType = GlobalController.Instance.demoDragType;
        if ( curDragType != prevDragType )
        {
            switchDragMode(curDragType);
            Debug.Log("DragMode changed: " + prevDragType + " -> " + curDragType);
            prevDragType = curDragType;
            uiController.updateDragMode();
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

    private void resetDragTypeAndPosition()
    {
        targetVisualizer.hideTarget();
        targetVisualizer.moveTarget(Vector3.zero);
        if (GlobalController.Instance)
        {
            GlobalController.Instance.demoTarget2DirectDragStatus = DirectDragStatus.inactive_on_screen_1;
            GlobalController.Instance.demoTarget2Status = TargetStatus.total_on_screen_1;
            GlobalController.Instance.switchDragType(DragType.direct_drag);
        }
        switchDragMode(DragType.direct_drag);
    }

    private void switchDragMode(DragType dt)
    {
        if (dt == DragType.direct_drag)
        {
            if (GlobalController.Instance &&
                GlobalController.Instance.demoTarget2Status == TargetStatus.total_on_screen_1)
            {
                directDragProcessor.GetComponent<demoDirectDragProcessor>().initParamsWhenTargetOnScreen1();
            }
            else if (GlobalController.Instance &&
                     GlobalController.Instance.demoTarget2Status == TargetStatus.total_on_screen_2)
            {
                directDragProcessor.GetComponent<demoDirectDragProcessor>().initParamsWhenTargetOnScreen2();
            }
            directDragProcessor.GetComponent<demoDirectDragProcessor>().enabled = true;
        }
        else if (dt == DragType.hold_tap)
        {
            if (GlobalController.Instance &&
                GlobalController.Instance.demoTarget2Status == TargetStatus.total_on_screen_1)
            {
                holdTapProcessor.GetComponent<demoHoldTapProcessor>().initParamsWhenTargetOnScreen1();
            }
            else if (GlobalController.Instance &&
                     GlobalController.Instance.demoTarget2Status == TargetStatus.total_on_screen_2)
            {
                holdTapProcessor.GetComponent<demoHoldTapProcessor>().initParamsWhenTargetOnScreen2();
            }
            holdTapProcessor.GetComponent<demoHoldTapProcessor>().enabled = true;
        }
        else if (dt == DragType.throw_catch)
        {
            if (GlobalController.Instance.demoTarget2Status == TargetStatus.total_on_screen_1)
            {
                throwCatchProcessor.GetComponent<demoThrowCatchProcessor>().initParamsWhenTargetOnScreen1();
            }
            else if (GlobalController.Instance.demoTarget2Status == TargetStatus.total_on_screen_2)
            {
                throwCatchProcessor.GetComponent<demoThrowCatchProcessor>().initParamsWhenTargetOnScreen2();
            }
            throwCatchProcessor.GetComponent<demoThrowCatchProcessor>().enabled = true;
        }
    }
}
