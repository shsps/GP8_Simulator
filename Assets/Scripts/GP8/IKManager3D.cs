using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// This IK refers to https://www.youtube.com/watch?v=qqOAzn05fvk
/// </summary>
public class IKManager3D : MonoBehaviour
{
    public int ChainLength = 2;

    public Transform Target;
    public Transform Pole;

    /// <summary>
    /// Solver iterations per update
    /// </summary>
    [Header("Solver Parament")]
    public int Iterations = 10;

    public float Delta = 0.001f;

    [Range(0, 1)]
    public float SnapBackStrength = 1f;

    protected Joint root;
    protected float[] BonesLength;
    protected float CompleteLength;
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

    private void Init()
    {
        root = this.GetComponent<Joint>();
        ChainLength = root.GetChainLength();
        //initial array
        Bones = new Transform[ChainLength + 1];
        Positions = new Vector3[ChainLength + 1];
        BonesLength = new float[ChainLength];
        StartDirectionSucc = new Vector3[ChainLength + 1];
        StartRotationBone = new Quaternion[ChainLength + 1];

        //initial fields
        if(Target == null)
        {
            throw new UnassignedReferenceException($"The variable {nameof(Target)} is not assigned");
        }
        StartRotationTarget = Target.rotation;
        CompleteLength = 0;

        //initial data
        Joint current = root;
        for (int i = 0; i <= Bones.Length - 1; i++)
        {
            Bones[i] = current.transform;
            StartRotationBone[i] = current.transform.rotation;

            if(i != Bones.Length - 1)
            {
                StartDirectionSucc[i] = current.GetChild().transform.position - current.transform.position;
                BonesLength[i] = (current.GetChild().transform.position - current.transform.position).magnitude;
                CompleteLength += BonesLength[i];
            }
            else
            {
                StartDirectionSucc[i] = Target.position - current.transform.position;
            }

            current = current.GetChild();
        }

        if(Bones[0] == null)
        {
            throw new UnityException("The chain value is longer than the ancestor chain");
        }
    }
    private void LateUpdate()
    {
        //RotateAroundAxisTutorial();
        /*RotateAroundAxis_ang += Time.deltaTime * 100;
        Pole.position = Quaternion.Euler(RotateAroundAxis_ang, 0, 0) * new Vector3(4, 0, -4);*/
        ResolveIK();
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
        switch(RotateAroundAxis_modeCount)
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
                    if(j == Positions.Length -1)
                    {
                        Positions[j] = Target.position;
                    }
                    else
                    {
                        Positions[j] = Positions[j+1] + (Positions[j] - Positions[j+1]).normalized * BonesLength[j];
                    }
                }

                for (int j = 1; j < Positions.Length; j++)
                {
                    Positions[j] = Positions[j - 1] + (Positions[j] - Positions[j - 1]).normalized * BonesLength[j - 1];
                }

                if((Positions[Positions.Length - 1] - Target.position).sqrMagnitude < Delta * Delta)
                {
                    break;
                }
            }
        }
        #endregion
        #region Move toward pole
        if(Pole != null)
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
            if(i == Positions.Length - 1)
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

    /*private void OnDrawGizmos()
    {
        Transform current = this.transform;
        for (int i = 0; i < ChainLength && current != null && current.parent != null; i++)
        {
            float scale = Vector3.Distance(current.position, current.parent.position) * 0.1f;
            Handles.matrix = Matrix4x4.TRS(current.position, Quaternion.FromToRotation(Vector3.up, current.parent.position - current.position),
                new Vector3(scale, Vector3.Distance(current.parent.position, current.position), scale));
            Handles.color = Color.green;
            Handles.DrawWireCube(Vector3.up * 0.5f, Vector3.one);
            current = current.parent;
        }
    }*/
}
