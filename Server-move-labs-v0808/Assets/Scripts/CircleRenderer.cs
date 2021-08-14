using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class CircleRenderer : MonoBehaviour
{

    [Range(0.1f, 100f)]
    public float radius = 0.9f;

    [Range(3, 256)]
    public int numSegments = 64;

    private Color lineColor = new Color(1f, 1f, 1f, 1);
    private float lineWidth = 0.1f;
    private Material lineMaterial;

    private void Start()
    {
        lineMaterial = new Material(Shader.Find("Sprites/Default"));
        RotateCircle();
        RenderCircle();
    }

    private void Update()
    {
        
    }

    public void RotateCircle()
    {
        gameObject.transform.Rotate(new Vector3(90f, 90f, 90f));
    }

    // https://gamedev.stackexchange.com/questions/126427/draw-circle-around-gameobject-to-indicate-radius
    public void RenderCircle()
    {
        LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer>();
        //lineRenderer.alignment = LineAlignment.TransformZ;
        lineRenderer.material = lineMaterial;
        lineRenderer.startColor = lineRenderer.endColor = lineColor;
        lineRenderer.startWidth = lineRenderer.endWidth = lineWidth;
        lineRenderer.positionCount = numSegments + 1;
        lineRenderer.useWorldSpace = false;

        float deltaTheta = (float)(2.0 * Mathf.PI) / numSegments;
        float theta = 0f;

        for (int i = 0; i < numSegments + 1; i++)
        {
            float x = radius * Mathf.Cos(theta);
            float z = radius * Mathf.Sin(theta);
            Vector3 pos = new Vector3(x, 0, z);
            lineRenderer.SetPosition(i, pos);
            theta += deltaTheta;
        }
    }
}