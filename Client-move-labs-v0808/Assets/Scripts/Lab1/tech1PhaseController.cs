using UnityEngine;
using UnityEngine.SceneManagement;
using static PublicInfo;
using static PublicLabParams;


public class tech1PhaseController : MonoBehaviour
{
    public tech1UIController uiController;
    public tech1TrialController trialController;

    private ClientCenter sender;

    private LabPhase curPhase, prevPhase;

    private bool updatedSceneToServer;

    // Start is called before the first frame update
    void Start()
    {
        sender = GlobalMemory.Instance.client;
        GlobalMemory.Instance.curClientScene = LabScene.Lab1_move_3techs;
        uiController.updateDragMode();
        updatedSceneToServer = false;
        if (GlobalMemory.Instance.getConnectionStatus())
        {
            sender.prepareNewMessage4Server(MessageType.Scene);
            updatedSceneToServer = true;
        }
        prevPhase = LabPhase.out_lab_scene;
        switchPhase(LabPhase.in_lab_scene);
    }

    // Update is called once per frame
    void Update()
    {
        if (!updatedSceneToServer && GlobalMemory.Instance.getConnectionStatus())
        {
            sender.prepareNewMessage4Server(MessageType.Scene);
            updatedSceneToServer = true;
        }

        if (curPhase != prevPhase)
        {
            GlobalMemory.Instance.curLabPhase = curPhase;
            Debug.Log("LabPhase changed: " + prevPhase + " -> " + curPhase);
            prevPhase = curPhase;
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
            if (GlobalMemory.Instance.curServerScene == LabScene.Lab1_move_3techs)
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
            if (GlobalMemory.Instance.serverCmdQueue.Count != 0 &&
                GlobalMemory.Instance.serverCmdQueue.Peek() == ServerCommand.server_say_exit_lab)
            {
                finishCurrentServerCmdExcution();
                switchPhase(LabPhase.out_lab_scene);
            }
            else if (GlobalMemory.Instance.serverCmdQueue.Count != 0 &&
              GlobalMemory.Instance.serverCmdQueue.Peek() == ServerCommand.server_say_end_lab)
            {
                finishCurrentServerCmdExcution();
                uiController.ShowTheEndText();
            }
        }
        else if (curPhase == LabPhase.out_lab_scene)
        {
            GlobalMemory.Instance.curIndexPhase = WelcomePhase.check_server_scene;
            Debug.Log("lab0Phase: back to entry scene soon");
            string indexSceneName = (LabScene.Index_scene).ToString();
            SceneManager.LoadScene(indexSceneName);
        }
    }

    private void switchPhase(LabPhase ph)
    {
        curPhase = ph;
        GlobalMemory.Instance.curLabPhase = curPhase;
    }

    private void finishCurrentServerCmdExcution()
    {
        ServerCommand sc = GlobalMemory.Instance.serverCmdQueue.Dequeue();
        Debug.Log("C excuted: " + sc.ToString());
    }

    #region Public Method
    public void moveToPhase(LabPhase ph)
    {
        switchPhase(ph);
    }
    #endregion
}
