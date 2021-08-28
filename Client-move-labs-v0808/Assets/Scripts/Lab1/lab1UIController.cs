using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static PublicInfo;

public class lab1UIController : MonoBehaviour
{
    public Camera renderCamera;
    public Text txtFinishLab;
    public Text txtUniqueInfo;

    public Text txtDragMode;
    public Text txtSendInfo;
    public Text txtRcvInfo;
    public Text txtPhaseInfo;
    public Text txtDebugInfo;
    public Text txtStatusInfo;
    public Text txtPosInfo;
    public Text txtTrial;
    public Text txtDragInfo;

    private bool isConnecting;
    private Color disconnectColor = new Color(0.8156f, 0.3529f, 0.4313f);
    private Color connectColor = new Color(0f, 0f, 0f);

    // Start is called before the first frame update
    void Start()
    {
        if (GlobalMemory.Instance && GlobalMemory.Instance.targetLabMode == LabMode.Full)
        {
            setDebugUIVisibility(false);
        }
        else if (GlobalMemory.Instance && GlobalMemory.Instance.targetLabMode == LabMode.Test)
        {
            setDebugUIVisibility(true);
            //setDebugUIVisibility(false);
        }
        txtFinishLab.gameObject.SetActive(false);
        txtUniqueInfo.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        isConnecting = GlobalMemory.Instance.getConnectionStatus();
        renderCamera.backgroundColor = (isConnecting ? connectColor : disconnectColor);
        updateSendInfo(GlobalMemory.Instance.sendInfo);
        updateRcvInfo(GlobalMemory.Instance.rcvInfo);
    }

    public void SwitchToIndexScene()
    {
        SceneManager.LoadScene("Entry");
    }

    public void setDebugUIVisibility(bool debugging)
    {
        txtDragMode.gameObject.SetActive(debugging);
        txtSendInfo.gameObject.SetActive(debugging);
        txtRcvInfo.gameObject.SetActive(debugging);
        txtPhaseInfo.gameObject.SetActive(debugging);
        txtDebugInfo.gameObject.SetActive(debugging);
        txtStatusInfo.gameObject.SetActive(debugging);
        txtPosInfo.gameObject.SetActive(debugging);
        txtTrial.gameObject.SetActive(debugging);
        txtDragInfo.gameObject.SetActive(debugging);
    }

    public void updateUniqueInfo (string str)
    {
        txtUniqueInfo.text = str;
    }

    public void setTrialInfo(string prefix, int id1, int id2)
    {
        txtTrial.text = string.Format("{0}: ({1:D2}, {2:D2})", prefix, id1, id2);
    }

    public void ShowTheEndText()
    {
        txtFinishLab.gameObject.SetActive(true);
    }

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
}
