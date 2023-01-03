using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepInfo : MonoBehaviour
{
    IKManager3D2.OperationMode mode;

    public StepInfo() { }
    public StepInfo(IKManager3D2 iK)
    {
        mode = iK.mode;
    }
}
