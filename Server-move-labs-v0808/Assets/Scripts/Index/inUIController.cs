using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PublicInfo;
using static PublicLabParams;

public class inUIController : MonoBehaviour
{
    public Text debugText;

    public inPhaseController phaseController;
    public Camera renderCamera;

    public InputField inputUserid;
    public Text txtUserid;

    public Dropdown dpLabOptions;
    public Text txtLabInfo;

    public Toggle tgLabMode;
    public Text txtLabMode;

    public GameObject blockConditions;
    public Text txtBlockInfo;
    public Text txtCurrentBlockTitle, txtAngleInfo, txtAngleValid;

    public Text txtServerip;

    public Button btnConfirmNameAndLab;
    public Button btnConfirmBlockCondition;
    public Button btnEnterLab;

    private bool isConnecting;

    private Color disconnectColor = new Color(0.8156f, 0.3529f, 0.4313f);
    private Color connectColor = new Color(0f, 0f, 0f);

    // Start is called before the first frame update
    void Start()
    {
        //txtLabName.text = "in " + ((LabScene)1).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        isConnecting = GlobalMemory.Instance.getConnectionStatus();
        renderCamera.backgroundColor = (isConnecting ? connectColor : disconnectColor);
    }

    #region Public Method
    public void setUserLabInfoVisibility(bool isInfoSet)
    {
        inputUserid.gameObject.SetActive(!isInfoSet);
        dpLabOptions.gameObject.SetActive(!isInfoSet);
        tgLabMode.gameObject.SetActive(!isInfoSet);
        btnConfirmNameAndLab.gameObject.SetActive(!isInfoSet);
        txtLabInfo.gameObject.SetActive(isInfoSet);
        txtUserid.gameObject.SetActive(isInfoSet);

        if (isInfoSet)
        {
            setUserLabInfoContent();
        }
    }

    public void setBlockInfoVisibility(bool inPhase, bool isAngleVaild, bool isConditionConfirmed)
    {
        if (!inPhase)
        {
            blockConditions.SetActive(false);
            btnConfirmBlockCondition.gameObject.SetActive(false);
        }
        else
        {
            blockConditions.SetActive(true);
            if (isConditionConfirmed)
            {
                txtAngleValid.text = "Confirmed";
                txtAngleValid.color = Color.green;
                btnConfirmBlockCondition.gameObject.SetActive(false);
            }
            else if (isAngleVaild && !isConditionConfirmed)
            {
                txtAngleValid.text = "Valid";
                txtAngleValid.color = Color.green;
                btnConfirmBlockCondition.gameObject.SetActive(true);
            }
            else if (!isAngleVaild && !isConditionConfirmed)
            {
                txtAngleValid.text = "Invalid";
                txtAngleValid.color = Color.red;
                btnConfirmBlockCondition.gameObject.SetActive(false);
            }
        }




    }

    public void setEnterLabBtnVisibility(bool readyToEnterLab)
    {
        btnEnterLab.gameObject.SetActive(readyToEnterLab);
    }

    public void setBlockInfoContent(int blockid)
    {
        string strCondition = "";
        switch (GlobalMemory.Instance.curLabInfos.labName)
        {
            case LabName.Lab1_move_28:
                strCondition = GlobalMemory.Instance.curBlockCondition.getAllDataForSceneDisplay();
                break;
            default:
                break;
        }
        txtBlockInfo.text = strCondition;

        string strBlockid = "#" + blockid.ToString() + " Block Condition";
        txtCurrentBlockTitle.text = strBlockid;
    }

    public void setAngleInfoContent(float angle)
    {
        string strAngle = Math.Round(angle, 1).ToString();
        txtAngleInfo.text = "Current angle: " + strAngle + "°";
    }

    public void setUserLabInfoContent()
    {
        txtUserid.text = "Hi, user" + GlobalMemory.Instance.userid.ToString();
        txtLabInfo.text = GlobalMemory.Instance.curLabInfos.labMode == LabMode.Full
            ? "in Full Mode" + Environment.NewLine
            : "in Test Mode" + Environment.NewLine;
        txtLabInfo.text += "of Tech " + ((int)GlobalMemory.Instance.curDragType + 1).ToString() + ": "
            + GlobalMemory.Instance.curDragType.ToString();
    }
    #endregion

    #region Public UI Method

    public void ConfirmUserAndLabInfo()
    {
        bool flag = GlobalMemory.Instance.getConnectionStatus();
        if (flag)
        {
            // user info
            int userid = int.Parse(inputUserid.text);
            phaseController.setUserid(userid);
            // lab info
            LabName labName = LabName.Lab1_move_28;
            int dragTypeid = dpLabOptions.value;
            DragType dragType = (DragType)dragTypeid;
            phaseController.setDragType(dragType);
            phaseController.setLabInfo(labName, tgLabMode.isOn);
            // move to next phase
            phaseController.moveToPhase(WelcomePhase.set_target_lab);
        }
    }

    public void ChangeLabModeText()
    {
        if (tgLabMode.isOn)
        {
            txtLabMode.text = "Full Mode";
        }
        else
        {
            txtLabMode.text = "Test Mode";
        }
    }

    public void ConfirmLabConditions()
    {
        phaseController.moveToPhase(WelcomePhase.confirm_block_conditions);
    }

    public void EnterSelectedLab()
    {
        phaseController.moveToPhase(WelcomePhase.in_lab_scene);
    }

    public void setServerip()
    {
        txtServerip.text = GlobalMemory.Instance.serverip;
    }

    public void setStartUIInvisible()
    {
        setUserLabInfoVisibility(GlobalMemory.Instance.isUserLabInfoSet);
        setBlockInfoVisibility(false, false, false);
        setEnterLabBtnVisibility(false);
    }
    #endregion
}
