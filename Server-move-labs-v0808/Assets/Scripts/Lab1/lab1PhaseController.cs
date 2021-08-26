using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PublicInfo;
using static PublicLabParams;

public class lab1PhaseController : MonoBehaviour
{
    public lab1UIController uiController;

    public lab1TrialController trialController;

    private ServerCenter sender;
    private LabPhase curPhase;
    private bool updatedSceneToClient;

    // Start is called before the first frame update
    void Start()
    {
        sender = GlobalMemory.Instance.server;
        GlobalMemory.Instance.curServerScene = LabScene.Lab1_move_3techs;
        uiController.updateDragMode("Mode: " + GlobalMemory.Instance.curDragType.ToString());
        updatedSceneToClient = false;
        if (GlobalMemory.Instance.getConnectionStatus())
        {
            sender.prepareNewMessage4Client(MessageType.Scene);
            updatedSceneToClient = true;
        }
        switchPhase(LabPhase.in_lab_scene);

        bool inDebugMode = GlobalMemory.Instance.curLabInfos.labMode == LabMode.Test ? true : false;
        uiController.setDebugUIVisibility(inDebugMode);
    }

    // Update is called once per frame
    void Update()
    {
        if (!updatedSceneToClient && GlobalMemory.Instance.getConnectionStatus())
        {
            sender.prepareNewMessage4Client(MessageType.Scene);
            updatedSceneToClient = true;
        }



        if (curPhase == LabPhase.in_lab_scene)
        {
            switchPhase(LabPhase.check_connection);
        }
        else if (curPhase == LabPhase.check_connection)
        {
            // check if client is in the lab scene
            if (GlobalMemory.Instance.getConnectionStatus())
            {
                trialController.setConnectionStatus(true);
                switchPhase(LabPhase.check_client_scene);
            }
        }
        else if (curPhase == LabPhase.check_client_scene)
        {
            if (GlobalMemory.Instance.curClientScene == LabScene.Lab1_move_3techs)
            {
                switchPhase(LabPhase.in_experiment);
            }
        }
        else if (curPhase == LabPhase.in_experiment)
        {
            trialController.setExperimentStatus(true);
        }
        else if (curPhase == LabPhase.end_experiment)
        {
            trialController.setExperimentStatus(false);
            curPhase = LabPhase.wait_to_back_to_entry;
        }
        else if (curPhase == LabPhase.wait_to_back_to_entry)
        {
            uiController.btnBack.gameObject.SetActive(true);
        }
        else if (curPhase == LabPhase.out_lab_scene)
        {
            if (GlobalMemory.Instance.haveNextBlock())
            {
                GlobalMemory.Instance.moveToNextBlock();
            }
            else
            {
                sender.prepareNewMessage4Client(MessageType.Command, ServerCommand.server_say_end_lab);
                uiController.ShowTheEndText();
                //GlobalMemory.Instance.writeAllBlocksFinishedFlagToFile();
            }
        }
    }


    private void switchPhase(LabPhase ph)
    {
        curPhase = ph;
        GlobalMemory.Instance.curLabPhase = ph;
    }

    #region Public Method
    public void moveToPhase(LabPhase ph)
    {
        switchPhase(ph);
    }
    #endregion
}
