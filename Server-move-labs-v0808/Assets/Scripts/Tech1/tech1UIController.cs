using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class tech1UIController : MonoBehaviour
{
    public Camera renderCamera;

    //public demoTouchProcessor touchProcessor;

    public Button btnBack;
    public Text txtFinishLab;

    public Text txtDragMode;
    public Text txtDebugInfo;
    public Text txtStatusInfo;
    public Text txtPosInfo;
    public Text txtAngle;
    public Text txtTrial;

    private bool isConnecting;
    private Color disconnectColor = new Color(0.8156f, 0.3529f, 0.4313f);
    private Color connectColor = new Color(0f, 0f, 0f);

    // Start is called before the first frame update
    void Start()
    {
        setDebugUIVisibility(false);
        btnBack.gameObject.SetActive(false);
        txtFinishLab.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        isConnecting = GlobalController.Instance.getConnectionStatus();
        renderCamera.backgroundColor = (isConnecting ? connectColor : disconnectColor);
        txtAngle.text = "Angle: " + Math.Round(GlobalMemory.Instance.curAngle, 1).ToString() + "°";
    }

    public void BackToEntrySceneSoon()
    {
        //phaseController.moveToPhase(LabPhase.out_lab0_scene);
    }

    public void ShowTheEndText()
    {
        btnBack.gameObject.SetActive(false);
        txtFinishLab.gameObject.SetActive(true);
    }

    public void setDebugUIVisibility(bool debugging)
    {
        txtAngle.gameObject.SetActive(debugging);
        txtTrial.gameObject.SetActive(debugging);
    }

    public void setTrialInfo(string prefix, int id1, int id2)
    {
        txtTrial.text = string.Format("{0}: ({1:D2}, {2:D2})", prefix, id1, id2);
    }

    #region Public UI method
    public void updateDragMode(string str)
    {
        txtDragMode.text = "Mode: " + str;
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
    #endregion
}
