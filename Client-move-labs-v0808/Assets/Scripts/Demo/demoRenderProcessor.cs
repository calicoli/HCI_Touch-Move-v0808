using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class demoRenderProcessor : MonoBehaviour
{
    // line var
    private float lineWidth = 0.1f;

    void Start()
    {

    }

    void Update()
    {

    }

    public LineRenderer initLineRenderer(LineRenderer lr)
    {
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.alignment = LineAlignment.TransformZ;
        lr.startWidth = lr.endWidth = lineWidth;
        lr.startColor = new Color(1, 1, 1, 0.8f);
        lr.endColor = new Color(1, 1, 1, 0.8f);
        lr.enabled = false;
        return lr;
    }

    public void updateLine(bool flag, int cntPos, Vector3[] vertics,
        out LineRenderer outLr)
    {
        LineRenderer lr = new LineRenderer();
        lr.positionCount = cntPos;
        for (int i = 0; i < cntPos; i++)
        {
            lr.SetPosition(i, vertics[i]);
        }
        outLr = lr;
    }
}
