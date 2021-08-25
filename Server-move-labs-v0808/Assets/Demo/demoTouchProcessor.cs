using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PublicDragParams;
using static PublicLabFactors;

public class demoTouchProcessor : MonoBehaviour
{
    public GameObject target1;
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
        if (GlobalController.Instance) {
            GlobalController.Instance.demoDragType = DragType.direct_drag;
        }
        initDifferentDragParams();
        Debug.Log("ScreenHeight: " + Screen.height + "; ScreenWidth: " + Screen.width);
        targetVisualizer.showTarget();
        switchDragMode(curDragType);
    }

    void initDifferentDragParams()
    {
        directDragProcessor.GetComponent<demoDirectDragProcessor>().enabled = false;
        holdTapProcessor.GetComponent<demoHoldTapProcessor>().enabled = false;
        throwCatchProcessor.GetComponent<demoThrowCatchProcessor>().enabled = false;
    }

    void Update()
    {
        if (curDragType != prevDragType)
        {
            if(GlobalController.Instance)
            {
                GlobalController.Instance.demoDragType = curDragType;
                GlobalController.Instance.server.
                    prepareNewMessage4Client(MessageType.DragMode);
            }
            Debug.Log("DragMode changed: " + prevDragType + " -> " + curDragType);
            prevDragType = curDragType;
            uiController.updateDragMode(curDragType.ToString());
        }
    }

    public void switchDragMode(DragType dt)
    {
        if ( dt == DragType.direct_drag )
        {
            if (GlobalController.Instance && 
                GlobalController.Instance.demoTarget1Status == TargetStatus.total_on_screen_1)
            {
                directDragProcessor.GetComponent<demoDirectDragProcessor>().initParamsWhenTargetOnScreen1();
            }
            else if (GlobalController.Instance && 
                GlobalController.Instance.demoTarget1Status == TargetStatus.total_on_screen_2)
            {
                directDragProcessor.GetComponent<demoDirectDragProcessor>().initParamsWhenTargetOnScreen2();
            }
            directDragProcessor.GetComponent<demoDirectDragProcessor>().enabled = true;
        }
        else if ( dt == DragType.hold_tap )
        {
            if (GlobalController.Instance.demoTarget1Status == TargetStatus.total_on_screen_1)
            {
                holdTapProcessor.GetComponent<demoHoldTapProcessor>().initParamsWhenTargetOnScreen1();
            }
            else if (GlobalController.Instance.demoTarget1Status == TargetStatus.total_on_screen_2)
            {
                holdTapProcessor.GetComponent<demoHoldTapProcessor>().initParamsWhenTargetOnScreen2();
            }
            holdTapProcessor.GetComponent<demoHoldTapProcessor>().enabled = true;
        }
        else if ( dt == DragType.throw_catch )
        {
            if (GlobalController.Instance.demoTarget1Status == TargetStatus.total_on_screen_1)
            {
                throwCatchProcessor.GetComponent<demoThrowCatchProcessor>().initParamsWhenTargetOnScreen1();
            }
            else if (GlobalController.Instance.demoTarget1Status == TargetStatus.total_on_screen_2)
            {
                throwCatchProcessor.GetComponent<demoThrowCatchProcessor>().initParamsWhenTargetOnScreen2();
            }
            throwCatchProcessor.GetComponent<demoThrowCatchProcessor>().enabled = true;
        }
        curDragType = dt;
    }


    public void resetTypeAndPosition()
    {
        targetVisualizer.inactiveTarget();
        targetVisualizer.moveTarget(Vector3.zero);
        targetVisualizer.showTarget();
        
        if (GlobalController.Instance)
        {
            GlobalController.Instance.demoDragType = DragType.direct_drag;
            GlobalController.Instance.demoTarget1DirectDragStatus = DirectDragStatus.inactive_on_screen_1;
            GlobalController.Instance.demoTarget1Status = TargetStatus.total_on_screen_1;
            GlobalController.Instance.server.prepareNewMessage4Client(MessageType.Command, ServerCommand.server_say_reset_drag_type_and_position);
        }
        switchDragMode(DragType.direct_drag);
    }
}