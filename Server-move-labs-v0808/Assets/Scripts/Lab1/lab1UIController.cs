using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static PublicInfo;
using static PublicLabParams;

public class lab1UIController : MonoBehaviour
{
    public Camera renderCamera;

    public lab1PhaseController phaseController;

    public Button btnBack;
    public Text txtFinishLab;
    public Text txtUniqueInfo;

    public Text txtDragMode;
    public Text txtSendInfo;
    public Text txtRcvInfo;
    public Text txtPhaseInfo;
    public Text txtDebugInfo;
    public Text txtStatusInfo;
    public Text txtPosInfo;
    public Text txtAngle;
    public Text txtTrial;
    public Text txtDragInfo;

    private bool isConnecting;
    private Color disconnectColor = new Color(0.8156f, 0.3529f, 0.4313f);
    private Color connectColor = new Color(0f, 0f, 0f);

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("dy-" + Camera.main.aspect.ToString());
        if (GlobalMemory.Instance && GlobalMemory.Instance.curLabInfos.labMode == LabMode.Full)
        {
            setDebugUIVisibility(false);
        }
        else if (GlobalMemory.Instance && GlobalMemory.Instance.curLabInfos.labMode == LabMode.Test)
        {
            setDebugUIVisibility(true);
        }
        btnBack.gameObject.SetActive(false);
        txtFinishLab.gameObject.SetActive(false);
        txtUniqueInfo.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        isConnecting = GlobalMemory.Instance.getConnectionStatus();
        renderCamera.backgroundColor = (isConnecting ? connectColor : disconnectColor);
        txtAngle.text = "Angle: " + Math.Round(GlobalMemory.Instance.curAngle, 1).ToString() + "°";
        updateSendInfo(GlobalMemory.Instance.sendInfo);
        updateRcvInfo(GlobalMemory.Instance.rcvInfo);
    }

    public void BackToEntrySceneSoon()
    {
        phaseController.moveToPhase(LabPhase.out_lab_scene);
    }

    public void ShowTheEndText()
    {
        btnBack.gameObject.SetActive(false);
        txtFinishLab.gameObject.SetActive(true);
    }

    public void setDebugUIVisibility(bool debugging)
    {
        txtDragMode.gameObject.SetActive(debugging);
        txtSendInfo.gameObject.SetActive(debugging);
        txtRcvInfo.gameObject.SetActive(debugging);
        txtDragMode.gameObject.SetActive(debugging);
        txtPhaseInfo.gameObject.SetActive(debugging);
        txtDebugInfo.gameObject.SetActive(debugging);
        txtStatusInfo.gameObject.SetActive(debugging);
        txtPosInfo.gameObject.SetActive(debugging);
        txtAngle.gameObject.SetActive(debugging);
        txtTrial.gameObject.SetActive(debugging);
        txtDragInfo.gameObject.SetActive(debugging);
    }

    public void setTrialInfo(string prefix, int id1, int id2)
    {
        txtTrial.text = string.Format("{0}: ({1:D2}, {2:D2})", prefix, id1, id2);
    }

    #region Public UI method
    public void updateDragMode(string str)
    {
        txtDragMode.text = str;
    }

    public void updateSendInfo(string str)
    {
        txtSendInfo.text = str;
    }
    public void updateRcvInfo(string str)
    {
        txtRcvInfo.text = str;
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

    public void updateDragInfo(string str)
    {
        txtDragInfo.text = str;
    }
    public void updatePhaseInfo(string str)
    {
        txtPhaseInfo.text = str;
    }
    #endregion
}
