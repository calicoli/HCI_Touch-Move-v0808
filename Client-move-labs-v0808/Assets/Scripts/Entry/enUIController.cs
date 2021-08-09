using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PublicLabFactors;

public class enUIController : MonoBehaviour
{
    public Camera renderCamera;
    private bool isConnecting;
    private Color disconnectColor = new Color(0.8156f, 0.3529f, 0.4313f);
    private Color connectColor = new Color(0f, 0f, 0f);

    public InputField inputServerip;
    public Text txtConnectStatus;
    public Button btnConnect;

    // Update is called once per frame
    void Update()
    {
        isConnecting = GlobalController.Instance.getConnectionStatus();
        renderCamera.backgroundColor = (isConnecting ? connectColor : disconnectColor);
        setConnectionInfoVisibility(isConnecting);
    }

    #region Public Method
    public void setConnectionInfoVisibility(bool isConnecting)
    {
        if (isConnecting)
        {
            txtConnectStatus.text = "Connecting with" + Environment.NewLine +
                GlobalController.Instance.serverip;
        } else
        {
            txtConnectStatus.text = "Unconnecting..";
        }
        inputServerip.gameObject.SetActive(!isConnecting);
        btnConnect.gameObject.SetActive(!isConnecting);
        txtConnectStatus.gameObject.SetActive(isConnecting);
    }
    #endregion

    #region Public UI Method
    public void ConfirmServerip()
    {
        string serverip = inputServerip.text;
        GlobalController.Instance.serverip = serverip;
        GlobalController.Instance.connectServer();
    }
    #endregion
}