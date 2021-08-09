using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PublicLabFactors;
using static PublicDragParams;

public class demoFlickerVisualizer : MonoBehaviour
{
    public demoTouchVisualizer touchVisualizer;
    public demoTargetVisualizer targetVisualizer;

    private bool flickerOn;
    private float remainFlickerSwitchingTime = 0f;

    private const float flickerFrequency = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        stopFlicker();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (remainFlickerSwitchingTime > 0f)
        {
            remainFlickerSwitchingTime -= Time.deltaTime;
        }
        else
        {
            changeFlickerStatus();
            resetRemainTime();
        }
    }

    private void resetFlickerStatus()
    {
        flickerOn = true;
    }

    private void resetRemainTime()
    {
        remainFlickerSwitchingTime = flickerFrequency;
    }

    private void changeFlickerStatus()
    {
        flickerOn = !flickerOn;
        if (GlobalController.Instance && GlobalController.Instance.demoTarget2Status == TargetStatus.total_on_screen_2)
        {
            if (flickerOn)
            {
                //targetVisualizer.showTarget();
                //touchVisualizer.showLocationLine();
            }
            else
            {
                //targetVisualizer.hideTarget();
                //touchVisualizer.hideLocationLine();
            }
        }
        else if (GlobalController.Instance && GlobalController.Instance.demoTarget2Status == TargetStatus.total_on_screen_1)
        {
            if (flickerOn)
            {
                touchVisualizer.showFrameLineLoop();
            }
            else
            {
                touchVisualizer.hideFrameLineLoop();
            }
        }
    }

    public void startFlicker()
    {
        resetFlickerStatus();
        resetRemainTime();
        this.gameObject.GetComponent<demoFlickerVisualizer>().enabled = true;
    }

    public void stopFlicker()
    {
        this.gameObject.GetComponent<demoFlickerVisualizer>().enabled = false;
    }

    public void showFlickerObjects ()
    {
        if (GlobalController.Instance && GlobalController.Instance.demoTarget2Status == TargetStatus.total_on_screen_2)
        {
            //targetVisualizer.showTarget();
            //touchVisualizer.showLocationLine();
        }
        else if (GlobalController.Instance && GlobalController.Instance.demoTarget2Status == TargetStatus.total_on_screen_1)
        {
            touchVisualizer.showFrameLineLoop();
        }
    }

    public void hideFlickerObjects ()
    {
        if (GlobalController.Instance && GlobalController.Instance.demoTarget2Status == TargetStatus.total_on_screen_2)
        {
            //targetVisualizer.hideTarget();
            //touchVisualizer.hideLocationLine();
        }
        else if (GlobalController.Instance && GlobalController.Instance.demoTarget2Status == TargetStatus.total_on_screen_1)
        {
            touchVisualizer.hideFrameLineLoop();
        }
    }
}
