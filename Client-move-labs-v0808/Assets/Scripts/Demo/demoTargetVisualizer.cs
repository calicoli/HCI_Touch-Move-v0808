using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class demoTargetVisualizer : MonoBehaviour
{
    public GameObject target;

    private static Color defaultColor = new Color32(255, 255, 255, 255);
    private static Color activeColor = new Color32(255, 255, 0, 255);

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void updateTargetColor(Color colorid)
    {
        target.GetComponent<MeshRenderer>().material.color = colorid;
    }

    private void updateTargetPosition(Vector3 pos)
    {
        target.transform.localPosition = pos;
    }

    private void updateTargetVisibility(bool isVis)
    {
        target.GetComponent<MeshRenderer>().enabled = isVis;
    }


    #region Public Method
    public void activeTarget()
    {
        updateTargetColor(activeColor);
    }

    public void inactiveTarget()
    {
        updateTargetColor(defaultColor);
    }

    public void showTarget()
    {
        updateTargetVisibility(true);
    }

    public void hideTarget()
    {
        updateTargetVisibility(false);
        updateTargetColor(defaultColor);
    }

    public void moveTarget( Vector3 pos )
    {
        updateTargetPosition(pos);
    }

    public bool getTargetVisibility()
    {
        return target.GetComponent<MeshRenderer>().enabled;
    }

    public Vector3 getTargetPosition()
    {
        return target.transform.position;
    }

    public Vector3 getTargetLocalScale()
    {
        return target.transform.localScale;
    }
    #endregion
}
