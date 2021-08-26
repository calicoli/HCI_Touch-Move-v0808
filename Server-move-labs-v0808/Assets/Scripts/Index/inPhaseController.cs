using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static PublicInfo;


public class inPhaseController : MonoBehaviour
{
    public bool debugOnPC = false;

    public inUIController uiController;

    private ServerCenter sender;

    private WelcomePhase curPhase, prevPhase;
    private int userid;


    private bool updatedSceneToClient;

    // Start is called before the first frame update
    void Start()
    {
        debugOnPC = false;
        uiController.setStartUIInvisible();
        sender = GlobalMemory.Instance.server;
        GlobalMemory.Instance.curServerScene = LabScene.Index_scene;
        updatedSceneToClient = false;
        if (GlobalMemory.Instance.getConnectionStatus())
        {
            sender.prepareNewMessage4Client(MessageType.Scene);
            updatedSceneToClient = true;
        }

        curPhase = GlobalMemory.Instance.curIndexPhase;
        prevPhase = WelcomePhase.in_lab_scene;

    }

    // Update is called once per frame
    void Update()
    {
        if (!updatedSceneToClient && GlobalMemory.Instance.getConnectionStatus())
        {
            sender.prepareNewMessage4Client(MessageType.Scene);
            updatedSceneToClient = true;
        }

        uiController.debugText.text = curPhase.ToString();
        uiController.setServerip();

        if (curPhase != prevPhase)
        {
            GlobalMemory.Instance.curIndexPhase = curPhase;
            Debug.Log("IndexPhase changed: " + prevPhase + " -> " + curPhase);
            prevPhase = curPhase;
        }

        if (curPhase == WelcomePhase.in_entry_scene)
        {
            uiController.setEnterLabBtnVisibility(false);
            switchPhase(WelcomePhase.wait_for_input_information);

        }
        else if (curPhase == WelcomePhase.wait_for_input_information)
        {
            uiController.setUserLabInfoVisibility(false);
            uiController.setBlockInfoVisibility(false, false, false);
        }
        else if (curPhase == WelcomePhase.set_target_lab)
        {
            if (GlobalMemory.Instance.getConnectionStatus())
            {
                GlobalMemory.Instance.isUserLabInfoSet = true;
                sender.prepareNewMessage4Client(MessageType.Block);
                GlobalMemory.Instance.excuteCommand(ServerCommand.server_set_target_lab);
                switchPhase(WelcomePhase.check_client_scene);
            }
        }
        else if (curPhase == WelcomePhase.check_client_scene)
        {
            if (GlobalMemory.Instance.curClientScene == LabScene.Index_scene)
            {
                switchPhase(WelcomePhase.assign_block_conditions);
            }
        }
        else if (curPhase == WelcomePhase.assign_block_conditions)
        {
            uiController.setBlockInfoContent(GlobalMemory.Instance.curBlockid);
            switchPhase(WelcomePhase.accept_acc_from_now);
        }
        else if (curPhase == WelcomePhase.accept_acc_from_now)
        {
            GlobalMemory.Instance.excuteCommand(ServerCommand.server_begin_to_receive_acc);
            GlobalMemory.Instance.angleProcessor.setReceivingAccStatus(true);
            uiController.setUserLabInfoVisibility(true);
            uiController.setBlockInfoVisibility(true, false, false);
            uiController.setEnterLabBtnVisibility(false);
            switchPhase(WelcomePhase.adjust_block_conditions);
        }
        else if (curPhase == WelcomePhase.adjust_block_conditions)
        {
            uiController.setAngleInfoContent(GlobalMemory.Instance.curAngle);
            if (debugOnPC || checkAngleValidation())
            {
                uiController.setBlockInfoVisibility(true, true, false);
            }
            else
            {
                uiController.setBlockInfoVisibility(true, false, false);
            }
        }
        else if (curPhase == WelcomePhase.confirm_block_conditions)
        {
            GlobalMemory.Instance.excuteCommand(ServerCommand.server_confirm_block_conditions);
            GlobalMemory.Instance.angleProcessor.setReceivingAccStatus(false);
            uiController.setBlockInfoVisibility(true, true, true);
            uiController.btnEnterLab.gameObject.SetActive(true);
            switchPhase(WelcomePhase.ready_to_enter_lab);
        }
        else if (curPhase == WelcomePhase.ready_to_enter_lab)
        {

            // then wait to click the "Enter Lab" Button
        }
        else if (curPhase == WelcomePhase.in_lab_scene)
        {
            GlobalMemory.Instance.excuteCommand(ServerCommand.server_say_enter_lab);
            //GlobalMemory.Instance.writeCurrentBlockConditionToFile();
            string sceneToLoad = GlobalMemory.Instance.getLabSceneToEnter();
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    private bool checkAngleValidation()
    {
        float conditionAngle = 0;
        switch (GlobalMemory.Instance.curLabInfos.labName)
        {
            case LabName.Lab1_move_28:
                conditionAngle = GlobalMemory.Instance.curBlockCondition.getAngle();
                break;
            default:
                break;
        }
        float currentAngle = GlobalMemory.Instance.curAngle;
        if (Mathf.Abs(conditionAngle - currentAngle) < 5f)
        {
            return true;
        }
        return false;
    }

    private void switchPhase(WelcomePhase wp)
    {
        curPhase = wp;
    }

    #region Public Region
    public void moveToPhase(WelcomePhase wp)
    {
        switchPhase(wp);
    }
    public void setUserid(int id)
    {
        userid = id;
        GlobalMemory.Instance.userid = userid;
    }
    public void setLabInfo(LabName name, bool isFullMode)
    {
        bool finished = GlobalMemory.Instance.setLabParams(name, isFullMode);
        if (finished)
        {
            GlobalMemory.Instance.writeAllBlockConditionsToFile();
        }
        else
        {
            Debug.Log("Do not finish Func:writeAllBlockConditionsToFile()");
        }
    }
    public void setDragType (DragType type)
    {
        GlobalMemory.Instance.curDragType = type;
        Debug.Log("setDragType: " + type);
    }
    #endregion
}
