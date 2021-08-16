using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lab1TouchVisualizer : MonoBehaviour
{
    public GameObject frameLineLoop;

    // Start is called before the first frame update
    void Start()
    {
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

    public void hideFrameLineLoop()
    {
        updateFrameLineLoopVisibility(false);
    }

    public void showFrameLineLoop()
    {
        updateFrameLineLoopVisibility(true);
    }
}
