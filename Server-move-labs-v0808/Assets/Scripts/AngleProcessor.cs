using UnityEngine;
using static PublicInfo;
using static PublicLabParams;

public class AngleProcessor : MonoBehaviour
{
    const float defaultAngle = Mathf.PI;
    private float angle;

    private Vector3 accThis;
    private Vector3 accOther;

    private bool inReceivingAccStatus;
    private bool inTrial;

    // Start is called before the first frame update
    void Start()
    {
        inTrial = false;
        inReceivingAccStatus = false;
        angle = defaultAngle;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (inReceivingAccStatus || inTrial)
        {
            accThis = Input.acceleration;
            accThis.y = 0f;
            accOther = GlobalMemory.Instance.accClient;
            accOther.y = 0f;

            angle = Vector3.Angle(accThis, accOther);

            if(
                GlobalMemory.Instance.curLabInfos.labName == LabName.Lab1_move_28 &&
                GlobalMemory.Instance.curBlockCondition.getShape() == Lab1_move_28.Shape.concave)
            {
                GlobalMemory.Instance.curAngle = 180 - angle;
            }
            else if(
                GlobalMemory.Instance.curLabInfos.labName == LabName.Lab1_move_28 &&
                GlobalMemory.Instance.curBlockCondition.getShape() == Lab1_move_28.Shape.convex)
            {
                GlobalMemory.Instance.curAngle = 180 + angle;
            }
            else if (
                GlobalMemory.Instance.curLabInfos.labName == LabName.Lab1_move_28 &&
                GlobalMemory.Instance.curBlockCondition.getShape() == Lab1_move_28.Shape.flat)
            {
                GlobalMemory.Instance.curAngle = 180 - angle;
            }
            else
            {
                GlobalMemory.Instance.curAngle = float.MaxValue;
            }

            if(inTrial)
            {
                inTrial = false;
            }
        }
        else
        {
            angle = defaultAngle;
        }
        
    }



    public void setReceivingAccStatus(bool open)
    {
        inReceivingAccStatus = open;
    }

    public void setTrialStatus(bool open)
    {
        inTrial = open;
    }
}
