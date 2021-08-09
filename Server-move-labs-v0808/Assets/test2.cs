using UnityEngine;

public class test2 : MonoBehaviour
{
    // Minimum and maximum values for the transition.
    float minimum = -5.0f;
    float maximum = 5.0f;

    // Time taken for the transition.
    float duration = 1f;

    float startTime;

    void Start()
    {
        // Make a note of the time the script started.
        startTime = Time.time;
    }

    void Update()
    {
        // Calculate the fraction of the total duration that has passed.
        float t = (Time.time - startTime) / duration;
        transform.position = new Vector3(Mathf.SmoothStep(1, -1, t), Mathf.SmoothStep(minimum, maximum, t), 0);
    }
}