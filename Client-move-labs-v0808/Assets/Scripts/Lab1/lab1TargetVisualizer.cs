using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lab1TargetVisualizer : MonoBehaviour
{
    public Camera renderCamera;
    public GameObject target2;
    public GameObject shadow2;
    public GameObject markers;

    private Vector3[] posMarkers;

    private static Color defaultColor = new Color32(255, 255, 255, 255);
    private static Color activeColor = new Color32(255, 255, 0, 255);
    private static Color correctColor = new Color32(0, 255, 0, 255);
    private static Color wrongColor = new Color32(255, 0, 0, 255);
    private static Color shadowColorInFullLab = new Color32(0, 0, 0, 255);
    //private static Color shadowColorInTestLab = new Color32(0, 0, 255, 255);
    private static Color shadowColorInTestLab = new Color32(0, 0, 0, 255);

    private static Vector3 normalScale = new Vector3(1.6f, 1.6f, 1f);
    private static Vector3 largeScale = new Vector3(2.4f, 2.4f, 1f);

    // Start is called before the first frame update
    void Start()
    {
        setPosMarkers();
        hideTarget();
        hideShadow();
        if (GlobalMemory.Instance && GlobalMemory.Instance.targetLabMode == PublicInfo.LabMode.Full)
        {
            updateShadowColor(shadowColorInFullLab);
            hideMarkers();
        }
        else if (GlobalMemory.Instance && GlobalMemory.Instance.targetLabMode == PublicInfo.LabMode.Test)
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
        for (int i = 0; i < markers.transform.childCount; i++)
        {
            posMarkers[i] = markers.transform.GetChild(i).position;
            Debug.Log(i.ToString() + " " + markers.transform.GetChild(i).gameObject.name + " "
               + posMarkers[i].ToString() + " "
               + renderCamera.WorldToScreenPoint(posMarkers[i]).ToString() + " "
               + Vector3.Distance(posMarkers[i], renderCamera.ScreenToWorldPoint(new Vector2(0f, 1600f))).ToString());
        }
    }

    private void updateMarkersVisibility(bool isVis)
    {
        markers.SetActive(isVis);
    }

    private void updateShadowColor(Color colorid)
    {
        shadow2.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = colorid;
    }

    private void updateShadowPosition(Vector3 pos)
    {
        shadow2.transform.position = pos;
    }

    private void updateShadowVisibility(bool isVis)
    {
        shadow2.gameObject.SetActive(isVis);
    }

    private void updateTargetColor(Color colorid)
    {
        target2.GetComponent<MeshRenderer>().material.color = colorid;
    }

    private void updateTargetPosition(Vector3 pos)
    {
        target2.transform.localPosition = pos;
    }

    private void updateTargetLocalScale(Vector3 sca)
    {
        target2.transform.localScale = sca;
    }

    private void updateTargetVisibility(bool isVis)
    {
        target2.GetComponent<MeshRenderer>().enabled = isVis;
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

    public void correctTarget()
    {
        updateTargetColor(correctColor);
    }
    public void wrongTarget()
    {
        updateTargetColor(wrongColor);
    }

    public void zoominTarget()
    {
        updateTargetLocalScale(largeScale);
    }

    public void zoomoutTarget()
    {
        updateTargetLocalScale(normalScale);
    }

    public void showTarget()
    {
        updateTargetVisibility(true);
    }

    public void hideTarget()
    {
        updateTargetVisibility(false);
        updateTargetColor(defaultColor);
        updateTargetLocalScale(normalScale);
    }

    public void moveTarget(Vector3 pos)
    {
        updateTargetPosition(pos);
    }

    public void moveTargetWithPosID(int id)
    {
        Debug.Log("Target: " + id.ToString() + " " + (id - 200).ToString() + posMarkers[id - 200].ToString());
        updateTargetPosition(posMarkers[id - 200]);
    }

    public bool getTargetVisibility()
    {
        return target2.GetComponent<MeshRenderer>().enabled;
    }

    public bool getTargetColliderEnabled()
    {
        return target2.GetComponent<SphereCollider>().enabled;
    }

    public Vector3 getTargetPosition()
    {
        return target2.transform.position;
    }

    public Vector3 getTargetLocalScale()
    {
        return target2.transform.localScale;
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
        Debug.Log("Shadow: " + id.ToString() + " " + (id - 200).ToString() + posMarkers[id - 200].ToString());
        updateShadowPosition(posMarkers[id - 200]);
    }

    public bool getShadowVisibility()
    {
        return shadow2.activeInHierarchy;
    }

    public Vector3 getShadowPosition()
    {
        return shadow2.transform.position;
    }

    public Vector3 getShadowLocalScale()
    {
        return shadow2.transform.GetChild(0).transform.localScale;
    }

    public Vector2 getShadowScreenPosition()
    {
        return renderCamera.WorldToScreenPoint(getShadowPosition());
    }
    #endregion
}
