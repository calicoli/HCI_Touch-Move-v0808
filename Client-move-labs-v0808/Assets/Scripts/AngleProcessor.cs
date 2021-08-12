using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PublicInfo;
using static PublicLabParams;

public class AngleProcessor : MonoBehaviour
{
    [HideInInspector]
    private bool inConveryAccStatus;

    //private Vector3 accPrev;
    //private Vector3 accThis;

    private float sendTimer = -1;

    // Start is called before the first frame update
    void Start()
    {
        //accPrev = accThis = Vector3.zero;
        inConveryAccStatus = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (inConveryAccStatus)
        {
            if (sendTimer < 0)
            {
                GlobalMemory.Instance.client.GetComponent<ClientCenter>().prepareNewMessage4Server(MessageType.Angle);
                sendTimer = 0.05f;
            }
            else
            {
                sendTimer -= Time.deltaTime;
            }
        }


    }

    public void setConveyAccStatus(bool open)
    {
        inConveryAccStatus = open;
    }
}
