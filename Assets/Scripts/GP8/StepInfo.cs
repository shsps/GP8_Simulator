using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StepInfo
{
    public IKManager3D2.OperationMode Mode;
    public IKManager3D2 Ik;
    [System.NonSerialized]
    public Joint[] joints;
    public float MoveToolAngleX = 0f;
    public float MoveToolAngleY = 0f;
    public float MoveToolAngleZ = 0f;
    public bool IsCatchPressed = false;
    public IKManager3D2.CatchStatus CatchStatusNow;

    public StepInfo() { }
    public StepInfo(IKManager3D2 ik)
    {
        Mode = ik.mode;
        Ik = ik;
        joints = ik.joints;
        MoveToolAngleX = ik.MoveToolAngleX;
        MoveToolAngleY = ik.MoveToolAngleY;
        MoveToolAngleZ = ik.MoveToolAngleZ;
        IsCatchPressed = ik.IsCatchPressed;
        CatchStatusNow = ik.catchStatusNow;
    }
    public StepInfo(float moveToolAngleX, float moveToolAngleY, float moveToolAngleZ, bool isCatchPressed, IKManager3D2.CatchStatus catchStatus)
    {
        MoveToolAngleX = moveToolAngleX;
        MoveToolAngleY = moveToolAngleY;
        MoveToolAngleZ = moveToolAngleZ;
        IsCatchPressed = isCatchPressed;
        CatchStatusNow = catchStatus;
    }
    public StepInfo(IKManager3D2 ik, float moveToolAngleX, float moveToolAngleY, float moveToolAngleZ, bool isCatchPressed, IKManager3D2.CatchStatus catchStatus) : 
        this(moveToolAngleX, moveToolAngleY, moveToolAngleZ, isCatchPressed, catchStatus)
    {
        Ik = ik;
    }
    public override string ToString()
    {
        return (MoveToolAngleX, MoveToolAngleY, MoveToolAngleZ, IsCatchPressed, CatchStatusNow).ToString();
    }
}
