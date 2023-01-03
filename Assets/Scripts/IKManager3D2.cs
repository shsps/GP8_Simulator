using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public Transform[] Bones;
    protected Vector3[] Positions;
    protected Vector3[] StartDirectionSucc;
    protected Quaternion[] StartRotationBone;
    protected Quaternion StartRotationTarget;
    protected Quaternion startRotationRoot;

    private const float Deg2Rad = Mathf.Deg2Rad;
    private const float Rad2Deg = Mathf.Rad2Deg;

    public IKManager3D2()
    {
        /*Plane p = new Plane(Vector3.up, Vector3.zero);
        Vector3 v1 = new Vector3(0, 2, 2);
        Vector3 v2 = new Vector3(0, -3, 12);
        Vector3 deltav = v1 - v2;
        Vector3 v3 = p.ClosestPointOnPlane(deltav);
        print(v3);
        print((v1.magnitude > v2.magnitude) ? v3.magnitude : -v3.magnitude);*/
    }

    private void Awake()
    {
        Init();
    }
    private void LateUpdate()
    {
        //RotateAroundAxisTutorial();
        /*RotateAroundAxis_ang += Time.deltaTime * 100;
        Pole.position = Quaternion.Euler(RotateAroundAxis_ang, 0, 0) * new Vector3(4, 0, -4);*/
        //ResolveIK();

        RobotArmIK();

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

        if(Input.GetKey(KeyCode.Q))
        {
            MoveToolX(-Time.deltaTime);
        }
        else if(Input.GetKey(KeyCode.A))
        {
            MoveToolX(Time.deltaTime);
        }

        if(Input.GetKey(KeyCode.W))
        {
            MoveToolY(-Time.deltaTime);
        }
        else if(Input.GetKey(KeyCode.S))
        {
            MoveToolY(Time.deltaTime);
        }

        if(Input.GetKey(KeyCode.E))
        {
            MoveToolZ(Time.deltaTime * 0.5f);
        }
        else if(Input.GetKey(KeyCode.D))
        {
            MoveToolZ(-Time.deltaTime * 0.5f);
        }

        SearchItemCatchable();
    }
    private void init_MoveTool()
    {
        /*ControllButton.SetActive(true);*/
    }
    public void MoveToolZ(float angle)
    {
        Vector3 v1 = GetJointFromName('E').transform.position;

        float _LRotateAngle = angle * GetJointFromName('L').RotateSpeed;
        GetJointFromName('L').Rotate(_LRotateAngle);
        GetJointFromName('B').Rotate(-_LRotateAngle);

        Vector3 v2 = GetJointFromName('E').transform.position;
        float deltaVerticalDistance = -(v2.y - v1.y);

        Vector3 vectorUE = GetVectorFromJoints('U', 'E');
        float lengthUE = vectorUE.magnitude;
        float thetaPUE = Vector3.SignedAngle(Vector3.up, vectorUE, GetJointFromName('U').transform.right);

        Vector3 vectorBE = GetVectorFromJoints('B', 'E');
        float lengthBE = vectorBE.magnitude;
        float thetaPBE = Vector3.SignedAngle(Vector3.up, vectorBE, GetJointFromName('B').transform.right);

        float paramentA = deltaVerticalDistance + lengthUE * Mathf.Cos(thetaPUE * Deg2Rad) - lengthBE * Mathf.Cos(thetaPBE * Deg2Rad) + lengthUE - lengthBE * Mathf.Cos((thetaPBE - thetaPUE) * Deg2Rad);
        float paramentB = -2 * (lengthBE * Mathf.Sin((thetaPBE - thetaPUE) * Deg2Rad));
        float paramentC = deltaVerticalDistance + lengthUE * Mathf.Cos(thetaPUE * Deg2Rad) - lengthBE * Mathf.Cos(thetaPBE * Deg2Rad) - lengthUE + lengthBE * Mathf.Cos((thetaPBE - thetaPUE) * Deg2Rad);

        (double, double) result = MathfExtension.AX2BXC(paramentA, paramentB, paramentC);
        double theta1 = System.Math.Atan(result.Item1) * Rad2Deg * 2;
        double theta2 = System.Math.Atan(result.Item2) * Rad2Deg * 2;

        float thetaNUE = System.Math.Abs(theta1 - thetaPUE) < System.Math.Abs(theta2 - thetaPUE) ? (float)theta1 : (float)theta2;
        float rotateValue = thetaNUE - thetaPUE;

        GetJointFromName('U').Rotate(rotateValue);
        GetJointFromName('B').Rotate(-rotateValue);
    }

    public void MoveToolY(float angle)
    {
        Vector3 v1 = GetJointFromName('E').transform.position;

        float _URotateAngle = angle * GetJointFromName('U').RotateSpeed;
        GetJointFromName('U').Rotate(_URotateAngle);
        GetJointFromName('B').Rotate(-_URotateAngle);

        Vector3 v2 = GetJointFromName('E').transform.position;
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        Vector3 projectV1 = plane.ClosestPointOnPlane(v1);
        Vector3 projectv2 = plane.ClosestPointOnPlane(v2);
        float deltaHorizontalDistance = -(projectv2.magnitude > projectV1.magnitude ? Vector3.Distance(projectv2, projectV1) : -Vector3.Distance(projectv2, projectV1));

        Vector3 vectorLE = GetVectorFromJoints('L', 'E');
        float lengthLE = vectorLE.magnitude;
        //preLE
        float thetaPLE = Vector3.SignedAngle(Vector3.up, vectorLE, GetJointFromName('L').transform.right);

        Vector3 vectorBE = GetVectorFromJoints('B', 'E');
        float lengthBE = vectorBE.magnitude;
        //preBE
        float thetaPBE = Vector3.SignedAngle(Vector3.up, vectorBE, GetJointFromName('B').transform.right);

        float paramentA = (deltaHorizontalDistance + lengthLE * Mathf.Sin(thetaPLE * Deg2Rad) - lengthBE * Mathf.Sin(thetaPBE * Deg2Rad) - lengthBE * Mathf.Sin((thetaPBE - thetaPLE) * Deg2Rad));
        float paramentB = (-2 * (lengthLE - lengthBE * Mathf.Cos((thetaPBE - thetaPLE) * Deg2Rad)));
        float paramentC = (deltaHorizontalDistance + lengthLE * Mathf.Sin(thetaPLE * Deg2Rad) - lengthBE * Mathf.Sin(thetaPBE * Deg2Rad) + lengthBE * Mathf.Sin((thetaPBE - thetaPLE) * Deg2Rad));

        (double, double) result = MathfExtension.AX2BXC(paramentA, paramentB, paramentC);

        double theta1 = System.Math.Atan(result.Item1) * Rad2Deg * 2;
        double theta2 = System.Math.Atan(result.Item2) * Rad2Deg * 2;

        //choose which result is closer to thetaPLE
        float thetaNLE = System.Math.Abs(theta1 - thetaPLE) < System.Math.Abs(theta2 - thetaPLE) ? (float)theta1 : (float)theta2;
        float thetaNBE = thetaPBE - (thetaNLE - thetaPLE);

        float rotateValue = thetaNLE - thetaPLE;
        GetJointFromName('L').Rotate(rotateValue);
        GetJointFromName('B').Rotate(-rotateValue);
    }

    public void MoveToolX(float angle)
    {
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        Vector3 project1 = plane.ClosestPointOnPlane(GetJointFromName('E').transform.position);
        Vector3 project2 = plane.ClosestPointOnPlane(GetJointFromName('S').transform.position);
        float radiusNow = Vector3.Distance(project1, project2);

        float rotateAngleS = angle * GetJointFromName('S').RotateSpeed;
        float preAngleT = GetJointFromName('T').angleNow;
        GetJointFromName('S').Rotate(rotateAngleS);
        GetJointFromName('T').Rotate(rotateAngleS);
        float afterAngleT = GetJointFromName('T').angleNow;

        float deltaHorizontalDistance = radiusNow * (Mathf.Cos(preAngleT * Deg2Rad) / Mathf.Cos((afterAngleT) * Deg2Rad) - 1);

        //-----------------------Rectification Z-Axis error-----------------------------------------------------
        Vector3 v1 = GetJointFromName('E').transform.position;

        Vector3 vectorLE = GetVectorFromJoints('L', 'E');
        float lengthLE = vectorLE.magnitude;
        //preLE
        float thetaPLE = Vector3.SignedAngle(Vector3.up, vectorLE, GetJointFromName('L').transform.right);

        Vector3 vectorBE = GetVectorFromJoints('B', 'E');
        float lengthBE = vectorBE.magnitude;
        //preBE
        float thetaPBE = Vector3.SignedAngle(Vector3.up, vectorBE, GetJointFromName('B').transform.right);

        float paramentA = (deltaHorizontalDistance + lengthLE * Mathf.Sin(thetaPLE * Deg2Rad) - lengthBE * Mathf.Sin(thetaPBE * Deg2Rad) - lengthBE * Mathf.Sin((thetaPBE - thetaPLE) * Deg2Rad));
        float paramentB = (-2 * (lengthLE - lengthBE * Mathf.Cos((thetaPBE - thetaPLE) * Deg2Rad)));
        float paramentC = (deltaHorizontalDistance + lengthLE * Mathf.Sin(thetaPLE * Deg2Rad) - lengthBE * Mathf.Sin(thetaPBE * Deg2Rad) + lengthBE * Mathf.Sin((thetaPBE - thetaPLE) * Deg2Rad));

        (double, double) result = MathfExtension.AX2BXC(paramentA, paramentB, paramentC);

        double theta1 = System.Math.Atan(result.Item1) * Rad2Deg * 2;
        double theta2 = System.Math.Atan(result.Item2) * Rad2Deg * 2;
        //print($"theta1 : {theta1}, theta2 : {theta2}");

        //choose which result is closer to thetaPLE
        float thetaNLE = System.Math.Abs(theta1 - thetaPLE) < System.Math.Abs(theta2 - thetaPLE) ? (float)theta1 : (float)theta2;

        float rotateValue = thetaNLE - thetaPLE;
        GetJointFromName('L').Rotate(rotateValue);
        GetJointFromName('B').Rotate(-rotateValue);

        Vector3 v2 = GetJointFromName('E').transform.position;

        project1 = plane.ClosestPointOnPlane(v1);
        project2 = plane.ClosestPointOnPlane(v2);
        //--------------------------------Rectification Y-Axis error-----------------------------------------------------------

        float deltaVerticalDistance = -(v2.y - v1.y);

        Vector3 vectorUE = GetVectorFromJoints('U', 'E');
        float lengthUE = vectorUE.magnitude;
        float thetaPUE = Vector3.SignedAngle(Vector3.up, vectorUE, GetJointFromName('U').transform.right);

        vectorBE = GetVectorFromJoints('B', 'E');
        lengthBE = vectorBE.magnitude;
        thetaPBE = Vector3.SignedAngle(Vector3.up, vectorBE, GetJointFromName('B').transform.right);

        paramentA = deltaVerticalDistance + lengthUE * Mathf.Cos(thetaPUE * Deg2Rad) - lengthBE * Mathf.Cos(thetaPBE * Deg2Rad) + lengthUE - lengthBE * Mathf.Cos((thetaPBE - thetaPUE) * Deg2Rad);
        paramentB = -2 * (lengthBE * Mathf.Sin((thetaPBE - thetaPUE) * Deg2Rad));
        paramentC = deltaVerticalDistance + lengthUE * Mathf.Cos(thetaPUE * Deg2Rad) - lengthBE * Mathf.Cos(thetaPBE * Deg2Rad) - lengthUE + lengthBE * Mathf.Cos((thetaPBE - thetaPUE) * Deg2Rad);

        result = MathfExtension.AX2BXC(paramentA, paramentB, paramentC);
        theta1 = System.Math.Atan(result.Item1) * Rad2Deg * 2;
        theta2 = System.Math.Atan(result.Item2) * Rad2Deg * 2;

        float thetaNUE = System.Math.Abs(theta1 - thetaPUE) < System.Math.Abs(theta2 - thetaPUE) ? (float)theta1 : (float)theta2;
        rotateValue = thetaNUE - thetaPUE;

        GetJointFromName('U').Rotate(rotateValue);
        GetJointFromName('B').Rotate(-rotateValue);
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

    /// <summary>
    /// Joint name needs to end with S,L,U,R,B,T,E
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
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

    public void SearchItemCatchable()
    {
        RaycastHit hit;
        Debug.DrawLine(joints[joints.Length - 2].transform.position,
                       joints[joints.Length - 1].transform.position, Color.red);
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (Physics.Raycast(joints[joints.Length - 2].transform.position,
                joints[joints.Length - 1].transform.position - joints[joints.Length - 2].transform.position, out hit, 1f))
            {
                if(hit.collider.TryGetComponent<ICatchable>(out ICatchable c))
                {
                    if (!c.IsCatching) c.Catch(joints[joints.Length - 2].gameObject);
                    else c.Release();
                }
            }
        }
    }
}

public static class MathfExtension
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

    public static (double,double) AX2BXC(float a, float b, float c)
    {
        double positive = (-b + Mathf.Sqrt((b * b - 4 * a * c))) / (2 * a);
        double negative = (-b - Mathf.Sqrt((b * b - 4 * a * c))) / (2 * a);

        return (positive, negative);
    }
}
