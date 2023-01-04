using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepInfo : MonoBehaviour
{
    private IKManager3D2.OperationMode mode;
    private Joint[] joints;

    public StepInfo() { }
    public StepInfo(IKManager3D2 iK)
    {
        mode = iK.mode;
        joints = iK.joints;
    }
}
