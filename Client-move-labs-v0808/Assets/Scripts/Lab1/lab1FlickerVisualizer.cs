using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PublicDragParams;

public class lab1FlickerVisualizer : MonoBehaviour
{
    public lab1TouchVisualizer touchVisualizer;
    public lab1TargetVisualizer targetVisualizer;

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
        if (GlobalMemory.Instance && GlobalMemory.Instance.lab1Target2Status == TargetStatus.total_on_screen_2)
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
        else if (GlobalMemory.Instance && GlobalMemory.Instance.lab1Target2Status == TargetStatus.total_on_screen_1)
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
        this.gameObject.GetComponent<lab1FlickerVisualizer>().enabled = true;
    }

    public void stopFlicker()
    {
        this.gameObject.GetComponent<lab1FlickerVisualizer>().enabled = false;
    }

    public void showFlickerObjects()
    {
        if (GlobalMemory.Instance && GlobalMemory.Instance.lab1Target2Status == TargetStatus.total_on_screen_2)
        {
            targetVisualizer.showTarget();
        }
        else if (GlobalMemory.Instance && GlobalMemory.Instance.lab1Target2Status == TargetStatus.total_on_screen_1)
        {
            touchVisualizer.showFrameLineLoop();
        }
    }

    public void hideFlickerObjects()
    {
        if (GlobalMemory.Instance && GlobalMemory.Instance.lab1Target2Status == TargetStatus.total_on_screen_2)
        {
            targetVisualizer.hideTarget();
        }
        else if (GlobalMemory.Instance && GlobalMemory.Instance.lab1Target2Status == TargetStatus.total_on_screen_1)
        {
            touchVisualizer.hideFrameLineLoop();
        }
    }
}
