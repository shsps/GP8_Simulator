using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepInfo : MonoBehaviour
{
    public IKManager3D2.OperationMode mode;
    public IKManager3D2 ik;
    public Joint[] joints;
    public float MoveToolAngleX = 0f;
    public float MoveToolAngleY = 0f;
    public float MoveToolAngleZ = 0f;
    public bool IsCatchPressed = false;

    public StepInfo() { }
    public StepInfo(IKManager3D2 ik)
    {
        mode = ik.mode;
        this.ik = ik;
        joints = ik.joints;
        MoveToolAngleX = ik.MoveToolAngleX;
        MoveToolAngleY = ik.MoveToolAngleY;
        MoveToolAngleZ = ik.MoveToolAngleZ;
        IsCatchPressed = ik.IsCatchPressed;
    }

    public override string ToString()
    {
        return (MoveToolAngleX, MoveToolAngleY, MoveToolAngleZ, IsCatchPressed).ToString();
    }
}
