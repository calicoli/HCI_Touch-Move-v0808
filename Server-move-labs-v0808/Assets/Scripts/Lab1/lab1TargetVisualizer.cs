﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lab1TargetVisualizer : MonoBehaviour
{
    public GameObject target1;
    public GameObject shadow1;
    public GameObject markers;
    public Camera renderCamera;

    private Vector3[] posMarkers;

    private static Color defaultColor = new Color32(255, 255, 255, 255);
    private static Color activeColor = new Color32(255, 255, 0, 255);
    private static Color outlineColor = new Color32(0, 0, 255, 255);
    private static Color shadowColorInFullLab = new Color32(0, 0, 0, 255);
    //private static Color shadowColorInTestLab = new Color32(0, 0, 255, 255);
    private static Color shadowColorInTestLab = new Color32(0, 0, 0, 255);

    private int t1cnt = 0;

    // Start is called before the first frame update
    void Start()
    {
        setPosMarkers();
        hideTarget();
        hideShadow();
        if( GlobalMemory.Instance && GlobalMemory.Instance.curLabInfos.labMode == PublicInfo.LabMode.Full)
        {
            updateShadowColor(shadowColorInFullLab);
            hideMarkers();
        }
        else if (GlobalMemory.Instance && GlobalMemory.Instance.curLabInfos.labMode == PublicInfo.LabMode.Test)
        {
            updateShadowColor(shadowColorInTestLab);
            showMarkers();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void setPosMarkers()
    {
        posMarkers = new Vector3[markers.transform.childCount];
        for ( int i = 0; i < markers.transform.childCount; i++ )
        {
            posMarkers[i] = markers.transform.GetChild(i).position;
            Debug.Log(i.ToString() + " " + markers.transform.GetChild(i).gameObject.name + " " 
                + posMarkers[i].ToString() + " "
                + renderCamera.WorldToScreenPoint(posMarkers[i]).ToString());
        }
    }

    private void updateMarkersVisibility(bool isVis)
    {
        markers.SetActive(isVis);
    }

    private void updateShadowColor(Color colorid)
    {
        shadow1.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = colorid;
    }

    private void updateShadowPosition(Vector3 pos)
    {
        shadow1.transform.position = pos;
    }

    private void updateShadowVisibility(bool isVis)
    {
        shadow1.gameObject.SetActive(isVis);
    }

    private void updateTargetColor(Color colorid)
    {
        target1.GetComponent<MeshRenderer>().material.color = colorid;
    }

    private void updateTargetPosition(Vector3 pos)
    {
        target1.transform.localPosition = pos;
    }

    private void updateTargetVisibility(bool isVis)
    {
        target1.GetComponent<MeshRenderer>().enabled = isVis;
        t1cnt++;
        //Debug.Log("t1 vis changed: " + t1cnt);
    }


    #region Public Method
    public void showMarkers()
    {
        updateMarkersVisibility(true);
    }

    public void hideMarkers()
    {
        updateMarkersVisibility(false);
    }

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

    public void moveTarget(Vector3 pos)
    {
        updateTargetPosition(pos);
    }

    public void moveTargetWithPosID (int id)
    {
        Debug.Log("Target: " + id + " " + (id - 100) + " " + posMarkers[id - 100]);
        updateTargetPosition(posMarkers[id-100]);
    }

    public bool getTargetVisibility()
    {
        return target1.GetComponent<MeshRenderer>().enabled;
    }

    public Vector3 getTargetPosition()
    {
        return target1.transform.position;
    }

    public Vector3 getTargetLocalScale()
    {
        return target1.transform.localScale;
    }

    public Vector2 getTargetScreenPosition()
    {
        return renderCamera.WorldToScreenPoint(getTargetPosition());
    }

    public void showShadow()
    {
        updateShadowVisibility(true);
    }

    public void hideShadow()
    {
        updateShadowVisibility(false);
    }

    public void moveShadow(Vector3 pos)
    {
        updateShadowPosition(pos);
    }

    public void moveShadowWithPosID(int id)
    {
        Debug.Log("Shadow: " + id + " " + (id - 100) + " " + posMarkers[id - 100]);
        updateShadowPosition(posMarkers[id - 100]);
    }

    public bool getShadowVisibility()
    {
        return shadow1.activeInHierarchy;
    }

    public Vector3 getShadowPosition()
    {
        return shadow1.transform.position;
    }

    public Vector3 getShadowLocalScale()
    {
        return shadow1.transform.GetChild(0).transform.localScale;
    }

    public Vector2 getShadowScreenPosition()
    {
        return renderCamera.WorldToScreenPoint(getShadowPosition());
    }
    #endregion
}
