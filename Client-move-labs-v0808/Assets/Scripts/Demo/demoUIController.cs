using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class demoUIController : MonoBehaviour
{
    public Camera renderCamera;

    public Text txtDragMode;
    public Text txtDebugInfo;
    public Text txtStatusInfo;
    public Text txtPosInfo;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateDragMode()
    {
        txtDragMode.text = "Mode: " + GlobalController.Instance.demoDragType.ToString();
    }

    public void updateDebugInfo(string str)
    {
        txtDebugInfo.text = str;
    }

    public void updateStatusInfo(string str)
    {
        txtStatusInfo.text = str;
    }

    public void updatePosInfo(string str)
    {
        txtPosInfo.text = str;
    }
}
