using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static PublicLabFactors;

public class enPhaseController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GlobalController.Instance.serverCmdQueue.Count != 0 &&
                GlobalController.Instance.serverCmdQueue.Peek() == ServerCommand.server_say_enter_lab)
        {
            GlobalController.Instance.serverCmdQueue.Dequeue();
            SceneManager.LoadScene(LabScene.Demo.ToString());
        }
    }
}
