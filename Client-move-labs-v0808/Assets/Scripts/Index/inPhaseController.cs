using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static PublicInfo;

public class inPhaseController : MonoBehaviour
{
    public inUIController uiController;

    private ClientCenter sender;
    private WelcomePhase curPhase, prevPhase;
    private bool updatedSceneToServer;

    // Start is called before the first frame update
    void Start()
    {
        uiController.setStartUIInvisible();
        sender = GlobalMemory.Instance.client;
        GlobalMemory.Instance.curClientScene = LabScene.Index_scene;
        updatedSceneToServer = false;
        if (GlobalMemory.Instance.getConnectionStatus())
        {
            sender.prepareNewMessage4Server(MessageType.Scene);
            updatedSceneToServer = true;
        }

        curPhase = GlobalMemory.Instance.curIndexPhase;
        prevPhase = WelcomePhase.out_entry_scene;
    }

    // Update is called once per frame
    void Update()
    {
        if (!updatedSceneToServer && GlobalMemory.Instance.getConnectionStatus())
        {
            sender.prepareNewMessage4Server(MessageType.Scene);
            updatedSceneToServer = true;
        }

        uiController.debugText.text = curPhase.ToString();
        if (curPhase != prevPhase)
        {
            GlobalMemory.Instance.curIndexPhase = curPhase;
            Debug.Log("IndexPhase changed: " + prevPhase + " -> " + curPhase);
            prevPhase = curPhase;
        }

        if (curPhase == WelcomePhase.in_entry_scene)
        {
            uiController.setConnectionInfoVisibility(false);
            uiController.setLabInfoVisibility(false, false);
            switchPhase(WelcomePhase.wait_for_input_serverip);

        }
        else if (curPhase == WelcomePhase.wait_for_input_serverip)
        {
            // wait
        }
        else if (curPhase == WelcomePhase.detect_connect_status)
        {
            if (GlobalMemory.Instance.getConnectionStatus())
            {
                uiController.setConnectionInfoVisibility(true);
                switchPhase(WelcomePhase.wait_for_server_set_lab);
                uiController.setLabInfoVisibility(true, false);
            }
        }
        else if (curPhase == WelcomePhase.wait_for_server_set_lab)
        {
            if (GlobalMemory.Instance.serverCmdQueue.Count != 0 &&
                GlobalMemory.Instance.serverCmdQueue.Peek() == ServerCommand.server_set_target_lab)
            {
                uiController.setLabInfoVisibility(true, true);
                finishCurrentServerCmdExcution();
                GlobalMemory.Instance.isLabInfoSet = true;
                switchPhase(WelcomePhase.check_server_scene);
            }
        }
        else if (curPhase == WelcomePhase.check_server_scene)
        {
            uiController.setLabInfoVisibility(true, true);
            if (GlobalMemory.Instance.curServerScene == LabScene.Index_scene)
            {
                switchPhase(WelcomePhase.wait_for_server_accept_acc);
            }
        }
        else if (curPhase == WelcomePhase.wait_for_server_accept_acc)
        {
            if (GlobalMemory.Instance.serverCmdQueue.Count != 0 &&
                GlobalMemory.Instance.serverCmdQueue.Peek() == ServerCommand.server_begin_to_receive_acc)
            {
                finishCurrentServerCmdExcution();
                switchPhase(WelcomePhase.deliver_angle_info);
            }
        }
        else if (curPhase == WelcomePhase.deliver_angle_info)
        {
            GlobalMemory.Instance.setAngleDetectStatus(true);
            if (GlobalMemory.Instance.serverCmdQueue.Count != 0 &&
                GlobalMemory.Instance.serverCmdQueue.Peek() == ServerCommand.server_confirm_block_conditions)
            {
                finishCurrentServerCmdExcution();
                GlobalMemory.Instance.setAngleDetectStatus(false);
                switchPhase(WelcomePhase.ready_to_enter_lab);
            }
        }
        else if (curPhase == WelcomePhase.ready_to_enter_lab)
        {
            if (GlobalMemory.Instance.serverCmdQueue.Count != 0 &&
                GlobalMemory.Instance.serverCmdQueue.Peek() == ServerCommand.server_say_enter_lab)
            {
                finishCurrentServerCmdExcution();
                switchPhase(WelcomePhase.out_entry_scene);
            }
        }
        else if (curPhase == WelcomePhase.out_entry_scene)
        {
            Debug.Log("Enter scene:" + GlobalMemory.Instance.targetLabScene.ToString());
            SceneManager.LoadScene(GlobalMemory.Instance.targetLabScene.ToString());
        }
    }

    private void switchPhase(WelcomePhase wp)
    {
        curPhase = wp;
    }

    private void finishCurrentServerCmdExcution()
    {
        ServerCommand sc = GlobalMemory.Instance.serverCmdQueue.Dequeue();
        Debug.Log("C excuted: " + sc.ToString());
    }


    #region public method
    public void tryToConnectServer(string ip)
    {

        GlobalMemory.Instance.serverip = ip;
        GlobalMemory.Instance.connectServer();
        switchPhase(WelcomePhase.detect_connect_status);
    }

    #endregion
}
