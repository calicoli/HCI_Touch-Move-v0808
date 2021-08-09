using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static PublicLabFactors;

public class enUIController : MonoBehaviour
{
    public Camera renderCamera;
    public GameObject phaseController;

    public Text txtServerip;

    private bool isConnecting;

    private Color disconnectColor = new Color(0.8156f, 0.3529f, 0.4313f);
    private Color connectColor = new Color(0f, 0f, 0f);

    public ScrollRect rect;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        isConnecting = GlobalController.Instance.getConnectionStatus();
        renderCamera.backgroundColor = (isConnecting ? connectColor : disconnectColor);
        setServerip();
    }

    public void EnterSelectedLab()
    {
        if(isConnecting)
        {
            string sceneToLoad = "Demo";
            SceneManager.LoadScene(sceneToLoad);
            GlobalController.Instance.excuteCommand(ServerCommand.server_say_enter_lab);
        }
    }

    public void setServerip()
    {
        txtServerip.text = GlobalController.Instance.serverip;
    }
}
