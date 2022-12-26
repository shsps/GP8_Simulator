using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArrayExtension;
using UnityEngine.UI;

public class IKManager3D2 : MonoBehaviour
{
    public int ChainLength = 2;

    [Tooltip("For free mode")]
    public Transform Target;
    [Tooltip("For free mode")]
    public Transform Pole;

    /// <summary>
    /// Solver iterations per update
    /// </summary>
    [Header("Solver Parament")]
    public int Iterations = 10;

    public float Delta = 0.001f;

    [Range(0, 1)]
    public float SnapBackStrength = 1f;
    public enum OperationMode
    {
        //Default,
        Stop,
        SingleJoint,
        FreeMode,
        MoveTool,
        FixedToolDirection
    }
    public OperationMode mode = OperationMode.SingleJoint;
    private OperationMode preMode;


    protected Joint root;
    protected Joint end;
    public Joint[] joints;
    public Joint[] xjoints;
    public Joint[] yjoints;
    public Joint[] zjoints;
    public float[] BonesLength;
    protected float CompleteLength;
    private float angleThreshold = 0.05f;
    public Transform[] Bones;
    protected Vector3[] Positions;
    protected Vector3[] StartDirectionSucc;
    protected Quaternion[] StartRotationBone;
    protected Quaternion StartRotationTarget;
    protected Quaternion startRotationRoot;

    private void Awake()
    {
        Init();
    }

    public IKManager3D2()
    {
    }

    private void Init()
    {
        root = this.GetComponent<Joint>();
        joints = root.GetAllChild();
        end = joints[joints.Length - 1];
        
        xjoints = new Joint[0];
        yjoints = new Joint[0];
        zjoints = new Joint[0];
        for (int i = 0; i < joints.Length; i++)
        {
            switch(joints[i].rotateAxis)
            {
                case Joint.RotateAxis.X:
                    xjoints = xjoints.Resize(xjoints.Length + 1);
                    xjoints[xjoints.Length - 1] = joints[i];
                    break;
                case Joint.RotateAxis.Y:
                    yjoints = yjoints.Resize(yjoints.Length + 1);
                    yjoints[yjoints.Length - 1] = joints[i];
                    break;
                case Joint.RotateAxis.Z:
                    zjoints = zjoints.Resize(zjoints.Length + 1);
                    zjoints[zjoints.Length - 1] = joints[i];
                    break;
            }
        }

        BonesLength = new float[xjoints.Length];
        BonesLength[0] = Vector3.Distance(xjoints[0].transform.position, xjoints[1].transform.position);
        BonesLength[1] = Vector3.Distance(xjoints[1].transform.position, xjoints[2].transform.position);
        BonesLength[2] = Vector3.Distance(xjoints[2].transform.position, joints[joints.Length - 1].transform.position);
    }

    private void LateUpdate()
    {
        //RotateAroundAxisTutorial();
        /*RotateAroundAxis_ang += Time.deltaTime * 100;
        Pole.position = Quaternion.Euler(RotateAroundAxis_ang, 0, 0) * new Vector3(4, 0, -4);*/
        //ResolveIK();
        RobotArmIK();
    }

    private float RotateAroundAxis_ang = 0;
    private int RotateAroundAxis_modeCount = 0;
    /// <summary>
    /// This function is used to relize how does Quaternion multiply Vector3 mean,
    /// press A to change mode
    /// </summary>
    public void RotateAroundAxisTutorial()
    {
        Vector3 p = Vector3.zero;
        if (Input.GetKeyDown(KeyCode.A)) RotateAroundAxis_modeCount++;
        RotateAroundAxis_ang += Time.deltaTime * 100;
        RotateAroundAxis_ang = RotateAroundAxis_ang > 360 ? RotateAroundAxis_ang - 360 : RotateAroundAxis_ang;
        switch (RotateAroundAxis_modeCount)
        {
            case 0:
                p = Quaternion.Euler(RotateAroundAxis_ang, 0, 0) * Vector3.one;
                break;
            case 1:
                p = Quaternion.Euler(0, RotateAroundAxis_ang, 0) * Vector3.one;
                break;
            case 2:
                p = Quaternion.Euler(0, 0, RotateAroundAxis_ang) * Vector3.one;
                break;
            case 3:
                p = Quaternion.Euler(RotateAroundAxis_ang, 45, 0) * Vector3.one;
                break;
            case 4:
                p = Quaternion.Euler(RotateAroundAxis_ang, 90, 0) * Vector3.one;
                break;
            case 5:
                p = Quaternion.AngleAxis(RotateAroundAxis_ang, new Vector3(1, 1, 0)) * Vector3.one;
                break;
            default:
                RotateAroundAxis_modeCount = 0;
                break;
        }

        Debug.DrawLine(Vector3.zero, p + Vector3.up / 20, Color.red, 3f);
    }

    private void ResolveIK()
    {
        if (Target == null) return;
        if (BonesLength.Length != ChainLength) Init();

        //Fabric
        //   End                                                          Root
        //  (Bones0)(BoneLength0)(Bones1)(BoneLength1)(Bones2)(BoneLength2)...
        //   x---------------------x---------------------x----------------

        //get position before calculation
        for (int i = 0; i < Bones.Length; i++)
        {
            Positions[i] = Bones[i].position;
        }

        Quaternion RootRot = (Bones[0].parent != null) ? Bones[0].parent.rotation : Quaternion.identity;
        Quaternion RootRotDiff = RootRot * Quaternion.Inverse(startRotationRoot);

        #region Caculation
        #region Move toward target
        if ((Target.position - Bones[0].position).sqrMagnitude >= CompleteLength * CompleteLength)
        {
            Vector3 direction = (Target.position - Positions[0]).normalized;
            for (int i = 1; i < Positions.Length; i++)
            {
                Positions[i] = Positions[i - 1] + direction * BonesLength[i - 1];
            }
        }
        else
        {
            for (int i = 0; i < Iterations; i++)
            {
                //Doesn't change the root
                for (int j = Positions.Length - 1; j > 0; j--)
                {
                    if (j == Positions.Length - 1)
                    {
                        Positions[j] = Target.position;
                    }
                    else
                    {
                        Positions[j] = Positions[j + 1] + (Positions[j] - Positions[j + 1]).normalized * BonesLength[j];
                    }
                }

                for (int j = 1; j < Positions.Length; j++)
                {
                    Positions[j] = Positions[j - 1] + (Positions[j] - Positions[j - 1]).normalized * BonesLength[j - 1];
                }

                if ((Positions[Positions.Length - 1] - Target.position).sqrMagnitude < Delta * Delta)
                {
                    break;
                }
            }
        }
        #endregion
        #region Move toward pole
        if (Pole != null)
        {
            for (int i = 1; i < Positions.Length - 1; i++)
            {
                Plane plane = new Plane(Positions[i + 1] - Positions[i - 1], Positions[i - 1]);
                Vector3 projectPole = plane.ClosestPointOnPlane(Pole.position);
                Vector3 projectBone = plane.ClosestPointOnPlane(Positions[i]);

                //SignedAngle return how much degrees does Vector3 (from) need to rotate to Vector3 (to).
                float angle = Vector3.SignedAngle(projectBone - Positions[i - 1], projectPole - Positions[i - 1], plane.normal);

                //Quaternion multiply Vector3 means this GameObject is at Vector3 and rotates around Quaternion.
                //Example: Quaternion.Euler(0, 45, 0) * new Vector3(1,1,1)
                //It means the GameObject is at point(1,1,1) and rotates around y-axis 45 degrees.
                //So the example below is the GameObject is at (Positions[i] - Positions[i - 1])
                //and rotate (angle) degrees around (plane.normal) axis.
                //Use function RotateAroundAxisTutorial to know how does it works.
                Positions[i] = Quaternion.AngleAxis(angle, plane.normal) * (Positions[i] - Positions[i - 1]) + Positions[i - 1];
            }
        }
        #endregion
        #endregion

        //Set position and rotation after calculation
        for (int i = 0; i < Bones.Length; i++)
        {
            if (i == Positions.Length - 1)
            {
                Bones[i].rotation = Target.rotation * Quaternion.Inverse(StartRotationTarget) * StartRotationBone[i];
            }
            else
            {
                Bones[i].rotation = Quaternion.FromToRotation(StartDirectionSucc[i], Positions[i + 1] - Positions[i]) * StartRotationBone[i];
            }
            Bones[i].position = Positions[i];
        }
    }

    private void RobotArmIK()
    {
        switch(mode)
        {
            case OperationMode.SingleJoint:
                break;
            case OperationMode.FreeMode:
                FreeMode();
                break;
            case OperationMode.MoveTool:
                MoveTool();
                break;
            case OperationMode.FixedToolDirection:
                FixedToolDirection();
                break;
        }
        preMode = mode;
    }
    /// <summary>
    /// Every joint will not follow its rotate axis, it will rotate to Target directly
    /// </summary>
    private void FreeMode()
    {
        //end joint doesn't need to rotate
        for (int i = joints.Length - 2; i >= 0; i--)
        {
            if (Vector3.Distance(joints[i].transform.position, Target.position) <= joints[i].RotateRadius)
            {

                joints[i].Rotate(Target);
                return;
            }
        }
    }

    /// <summary>
    /// Move tool by WASD
    /// </summary>
    private void MoveTool()
    {
        if (preMode != OperationMode.MoveTool) init_MoveTool();

        if(Input.GetKeyDown(KeyCode.A))
        {
            MoveToolHorizon(Time.deltaTime * GetJointFromName('L').RotateSpeed);
        }
        else if(Input.GetKeyDown(KeyCode.D))
        {
            MoveToolHorizon(-Time.deltaTime * GetJointFromName('L').RotateSpeed);
        }

        if(Input.GetKeyDown(KeyCode.S))
        {
            MoveToolVertical(-Time.deltaTime * GetJointFromName('U').RotateSpeed);
        }
        else if(Input.GetKeyDown(KeyCode.W))
        {
            MoveToolVertical(Time.deltaTime * GetJointFromName('U').RotateSpeed);
        }
    }
    private void init_MoveTool()
    {
        /*ControllButton.SetActive(true);*/
    }
    public void MoveToolHorizon(float angle)
    {
        Vector3 vector_LE = GetVectorFromJoints('L', 'E');
        float length_LE = vector_LE.magnitude;
        float preLEAngle = Vector3.SignedAngle(Vector3.up, vector_LE, GetJointFromName('L').transform.right);

        float _LRotateAngle = angle * GetJointFromName('L').RotateSpeed;
        xjoints[0].Rotate(_LRotateAngle);

        float _LEAngleNow = Vector3.SignedAngle(Vector3.up, GetVectorFromJoints('L', 'E'), GetJointFromName('L').transform.right);
        Vector3 vector_UE = GetVectorFromJoints('U', 'E');
        float length_UE = vector_UE.magnitude;
        float preUEAngle = Vector3.SignedAngle(Vector3.up, vector_UE, xjoints[1].transform.right);
        //print("L delta y : " + (length_LE * (Mathf.Cos(_LEAngleNow * Mathf.Deg2Rad) - Mathf.Cos(preLEAngle * Mathf.Deg2Rad))));
        float _UCosPrime = (-length_LE) / length_UE * (Mathf.Cos(_LEAngleNow * Mathf.Deg2Rad) - Mathf.Cos(preLEAngle * Mathf.Deg2Rad))
                        + Mathf.Cos(preUEAngle * Mathf.Deg2Rad);
        float _UAnglePrime = Mathf.Acos(_UCosPrime) * Mathf.Rad2Deg;
        float _UDeltaAngle = _UAnglePrime - preUEAngle;
        float yb = GetJointFromName('E').transform.position.y;
        xjoints[1].Rotate(_UDeltaAngle);

        //print("U delta y : " + (GetJointFromName('E').transform.position.y - yb));
        //float _UEAngleNow = Vector3.SignedAngle(Vector3.up, GetVectorFromJoints('U', 'E'), GetJointFromName('U').transform.right);
        float _BDeltaAngle = -(_LRotateAngle + _UDeltaAngle);   
        xjoints[2].Rotate(_BDeltaAngle);

        //print(GetJointFromName('B').transform.position.y);
    }
    public void MoveToolVertical(float angle)
    {
        Vector3 vector_UE = GetVectorFromJoints('U', 'E');
        float length_UE = vector_UE.magnitude;
        float preUEAngle = Vector3.SignedAngle(Vector3.up, vector_UE, GetJointFromName('U').transform.right);

        float _URotateAngle = angle * GetJointFromName('U').RotateSpeed;
        GetJointFromName('U').Rotate(_URotateAngle);

        float _UEAngleNow = Vector3.SignedAngle(Vector3.up, GetVectorFromJoints('U', 'E'), GetJointFromName('U').transform.right);
        Vector3 vector_LE = GetVectorFromJoints('L', 'E');
        float length_LE = vector_LE.magnitude;
        float preLEAngle = Vector3.SignedAngle(Vector3.up, vector_LE, GetJointFromName('L').transform.right);
        //print("U delta x : " + (length_UE * (Mathf.Sin(_UEAngleNow * Mathf.Deg2Rad) - Mathf.Sin(preUEAngle * Mathf.Deg2Rad))));
        float _LSinPrime = (-length_UE) / length_LE * (Mathf.Sin(_UEAngleNow * Mathf.Deg2Rad) - Mathf.Sin(preUEAngle * Mathf.Deg2Rad))
                           + Mathf.Sin(preLEAngle * Mathf.Deg2Rad);
        float _LAnglePrime = Mathf.Asin(_LSinPrime) * Mathf.Rad2Deg;
        float _LDeltaAngle = _LAnglePrime - preLEAngle;
        float zb = GetJointFromName('E').transform.position.z;
        GetJointFromName('L').Rotate(_LDeltaAngle);

        //print("L delta x : " + (GetJointFromName('E').transform.position.z - zb));
        float _BDeltaAngle = -(_URotateAngle + _LDeltaAngle);
        GetJointFromName('B').Rotate(_BDeltaAngle);
        //print(GetJointFromName('E').transform.position.z);
    }
    private void FixedToolDirection()
    {
        //Vector3 _angle = CaculateAngleToTarget();

        Plane yPlane = new Plane(this.transform.up, this.transform.position);
        float yAngle = Vector3.SignedAngle((yPlane.ClosestPointOnPlane(end.transform.position) - yPlane.ClosestPointOnPlane(this.transform.position)),
                                           (yPlane.ClosestPointOnPlane(Target.transform.position) - yPlane.ClosestPointOnPlane(this.transform.position)), Vector3.up);
        if(Mathf.Abs(yAngle) > angleThreshold)
        {
            yjoints[0].transform.Rotate(new Vector3(0, yAngle, 0));
        }

        Vector3 bToE = joints[joints.Length - 1].transform.position - joints[joints.Length - 3].transform.position;
        Vector3 bToT = Target.position - joints[joints.Length - 3].transform.position;
        Vector3 eToT = Target.position - joints[joints.Length - 1].transform.position;
        Vector3 lToE = joints[6].transform.position - xjoints[0].transform.position;
        print("end before : " + joints[6].transform.position.z);
        print("z now : " + joints[6].transform.position.z);
        print("z : " + eToT.z);
        if(Mathf.Abs(eToT.z) > 0.01f)
        {
            float angleNow_LToE = Vector3.SignedAngle(Vector3.up, lToE, xjoints[0].transform.right);
            print("angleNow : " + angleNow_LToE);
            print("Length : " + lToE.magnitude);
            float afterAngle = Mathf.Asin((eToT.z + joints[6].transform.position.z)/lToE.magnitude) * Mathf.Rad2Deg;
            print("afterAngle : " + afterAngle);
            //print("Cos : " + Mathf.Cos(afterAngle * Mathf.Deg2Rad) * lToE.magnitude);
            //print("Unity Angle : " + MathfExtension.AngleConvert(Mathf.Acos(eToT.z / lToE.magnitude) *Mathf.Rad2Deg));
            xjoints[0].Rotate(afterAngle - angleNow_LToE);
            print("end after: " + joints[6].transform.position.z);
        }
        int _dir = 0;
        if (eToT.y > 0.01) _dir = -1;
        else if (eToT.y < -0.01) _dir = 1;
        else _dir = 0;
        int counter = 0;
        /*while(Mathf.Abs((Vector3.Distance(xjoints[2].transform.position, Target.position) - BonesLength[2])) > 0.01)
        {
            xjoints[1].Rotate(0.01f * _dir);
            if (Vector3.Distance(xjoints[1].transform.position, Target.position) > (BonesLength[1] + BonesLength[2]))
            {
                print("out of range");
                break;
            }
            if (counter++ > 100000)
            {
                print("over loop");
                break;
            }
        }
        //print("1 : " + Vector3.Distance(xjoints[2].transform.position, Target.position));
        //print("2 : " + BonesLength[2]);
        xjoints[2].Rotate(Target, true);*/

        //xjoints[2].Rotate(Vector3.SignedAngle(bToE, bToT, xjoints[2].transform.right));
        //print(Target.position - joints[joints.Length - 1].transform.position);
        //print(Vector3.SignedAngle(Vector3.up, xjoints[0].transform.up, Vector3.right));

        //xjoints[1].Rotate(new Vector3(-Vector3.SignedAngle(Vector3.up, xjoints[0].transform.up, Vector3.right), 0, 0));

        float xAngle = 0;
        float zAngle = 0;
    }
    /// <returns>This Vector3 means how much degrees end joint needs to rotate to Target</returns>
    private Vector3 CaculateAngleToTarget()
    {
        Vector3 endToTarget = Target.position - joints[joints.Length - 1].transform.position;

        float xAngle = 0;

        Plane yPlane = new Plane(this.transform.up, this.transform.position);
        float yAngle = Vector3.SignedAngle((yPlane.ClosestPointOnPlane(end.transform.position) - yPlane.ClosestPointOnPlane(this.transform.position)),
                                           (yPlane.ClosestPointOnPlane(Target.transform.position) - yPlane.ClosestPointOnPlane(this.transform.position)), Vector3.up);
        float zAngle = 0;

        return new Vector3(xAngle, yAngle, zAngle);
    }

    private Vector3 GetVectorFromJoints(char from, char to)
    {
        Joint _from = GetJointFromName(from);
        Joint _to = GetJointFromName(to);
        return (_to.transform.position - _from.transform.position);
    }

    private Joint GetJointFromName(char name)
    {
        char _name = char.ToUpper(name);
        switch(_name)
        {
            case 'S':
                return joints[0];
            case 'L':
                return joints[1];
            case 'U':
                return joints[2];
            case 'R':
                return joints[3];
            case 'B':
                return joints[4];
            case 'T':
                return joints[5];
            case 'E':
                return joints[6];
        }
        throw new UnityException($"There isn't any joint called {name}");
    }
}

class MathfExtension
{
    public static float AngleConvert(float f)
    {
        return (450 - f) >= 360 ? 450 - f - 360 : 450 - f;
    }
    public static float Cos(float f)
    {
        return Mathf.Cos(AngleConvert(f));
    }

    public static float Sin(float f)
    {
        return Mathf.Sin(AngleConvert(f));
    }
}