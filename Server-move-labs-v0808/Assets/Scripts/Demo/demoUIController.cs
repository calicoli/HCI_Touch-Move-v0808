using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class demoUIController : MonoBehaviour
{
    public Camera renderCamera;

    public demoTouchProcessor touchProcessor;

    public Text txtDragMode;
    public Text txtDebugInfo;
    public Text txtStatusInfo;
    public Text txtPosInfo;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Public UI Method
    public void SwitchDragMode ()
    {
        
        string btnContent = EventSystem.current.currentSelectedGameObject.transform.
            Find("Text").gameObject.GetComponent<Text>().text;
        switch (btnContent)
        {
            case "Direct Drag":
                touchProcessor.switchDragMode(PublicLabFactors.DragType.direct_drag);
                break;
            case "Hold Tap":
                touchProcessor.switchDragMode(PublicLabFactors.DragType.hold_tap);
                break;
            case "Throw Catch":
                touchProcessor.switchDragMode(PublicLabFactors.DragType.throw_catch);
                break;
            default:
                break;
        }
    }

    public void ResetDrag()
    {
        touchProcessor.resetTypeAndPosition();
    }

    public void updateDragMode (string str)
    {
        txtDragMode.text = "Mode: " + str;
    }

    public void updateDebugInfo (string str)
    {
        txtDebugInfo.text = str;
    }

    public void updateStatusInfo (string str)
    {
        txtStatusInfo.text = str;
    }

    public void updatePosInfo(string str)
    {
        txtPosInfo.text = str;
    }

    #endregion
}
