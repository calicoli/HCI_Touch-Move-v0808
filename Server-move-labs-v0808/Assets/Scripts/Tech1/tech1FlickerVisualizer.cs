using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PublicLabFactors;
using static PublicDragParams;

public class tech1FlickerVisualizer : MonoBehaviour
{
    public tech1TouchVisualizer touchVisualizer;
    public tech1TargetVisualizer targetVisualizer;

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
        if (GlobalController.Instance.demoTarget1Status == TargetStatus.total_on_screen_1)
        {
            if (flickerOn)
            {
                targetVisualizer.showTarget();
            }
            else
            {
                targetVisualizer.hideTarget();
            }
        }
        else if (GlobalController.Instance.demoTarget1Status == TargetStatus.total_on_screen_2)
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

    public void showFlickerObjects()
    {
        if (GlobalController.Instance && GlobalController.Instance.demoTarget1Status == TargetStatus.total_on_screen_1)
        {
            targetVisualizer.showTarget();
        }
        else if (GlobalController.Instance && GlobalController.Instance.demoTarget1Status == TargetStatus.total_on_screen_2)
        {
            touchVisualizer.showFrameLineLoop();
        }
    }

    public void hideFlickerObjects()
    {
        if (GlobalController.Instance && GlobalController.Instance.demoTarget1Status == TargetStatus.total_on_screen_1)
        {
            targetVisualizer.hideTarget();
        }
        else if (GlobalController.Instance && GlobalController.Instance.demoTarget1Status == TargetStatus.total_on_screen_2)
        {
            touchVisualizer.hideFrameLineLoop();
        }
    }
}
