using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class demoTouchVisualizer : MonoBehaviour
{
    public GameObject touchOutline;
    public GameObject touchPath;
    public GameObject locationLine;
    public GameObject frameLineLoop;

    // Start is called before the first frame update
    void Start()
    {
        hideTouchPath();
        hideTouchOutline();
        hideLocationLine();
        hideFrameLineLoop();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void updateFrameLineLoopVisibility(bool isVis)
    {
        frameLineLoop.GetComponent<LineRenderer>().enabled = isVis;
    }

    private void updateLocationLineVisibility(bool isVis)
    {
        locationLine.GetComponent<LineRenderer>().enabled = isVis;
    }

    private void updateOutlinePosition ( Vector3 v3 )
    {
        touchOutline.transform.position = v3;
    }

    private void updateOutlineVisibility ( bool isVis )
    {
        touchOutline.GetComponent<LineRenderer>().enabled = isVis;
    }

    private void updatePathStartNodePostion(Vector3 startPos)
    {
        touchPath.GetComponent<LineRenderer>().SetPosition(0, startPos);
    }

    private void updatePathEndNodePostion(Vector3 endPos)
    {
        touchPath.GetComponent<LineRenderer>().SetPosition(1, endPos);
    }

    private void updatePathVisibility(bool isVis)
    {
        touchPath.GetComponent<LineRenderer>().enabled = isVis;
    }

    public void hideFrameLineLoop()
    {
        updateFrameLineLoopVisibility(false);
    }

    public void showFrameLineLoop()
    {
        updateFrameLineLoopVisibility(true);
    }

    public void hideLocationLine()
    {
        updateLocationLineVisibility(false);
    }

    public void showLocationLine()
    {
        updateLocationLineVisibility(true);
    }

    public void showTouchOutline ()
    {
        
        updateOutlineVisibility(true);
    }

    public void hideTouchOutline()
    {
        updateOutlineVisibility(false);
    }

    public void adjustTouchOutlinePosition(Vector2 touchPoint)
    {
        Vector3 v3 = Camera.main.ScreenToWorldPoint(touchPoint);
        updateOutlinePosition(new Vector3(v3.x, v3.y, 0f));
    }

    public void showTouchPath()
    {
        updatePathVisibility(true);
    }

    public void hideTouchPath()
    {
        updatePathVisibility(false);
    }

    public void adjustPathBothNode(Vector2 startPos, Vector2 endPos)
    {
        Vector3 vStart = Camera.main.ScreenToWorldPoint(startPos);
        Vector3 vEnd = Camera.main.ScreenToWorldPoint(endPos);
        updatePathStartNodePostion(new Vector3(vStart.x, vStart.y, 0f));
        updatePathEndNodePostion(new Vector3(vEnd.x, vEnd.y, 0f));
    }

    public void adjustPathStartNode(Vector2 startPos)
    {
        Vector3 vStart = Camera.main.ScreenToWorldPoint(startPos);
        updatePathStartNodePostion(new Vector3(vStart.x, vStart.y, 0f));
    }

    public void adjustPathEndNode(Vector2 endPos)
    {
        Vector3 vEnd = Camera.main.ScreenToWorldPoint(endPos);
        updatePathEndNodePostion(new Vector3(vEnd.x, vEnd.y, 0f));
    }

    public void adjustPathStartNodeWithWorldPos(Vector3 startPos)
    {
        updatePathStartNodePostion(startPos);
    }

    public void adjustPathEndNodeWithWorldPos(Vector3 endPos)
    {
        updatePathEndNodePostion(endPos);
    }
}