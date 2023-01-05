using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Joint : MonoBehaviour
{
    public enum RotateAxis
    {
        DontRotate,
        X,
        Y,
        Z
    }

    [Header("Joint Detail")]
    [Tooltip("Z means this joint determine the rise and down of end joint")]
    public RotateAxis rotateAxis;
    [Range(0, 100)]
    public int RotateSpeed = 1;
    [Range(0,360)]
    public int PositiveRotateLimit = 0;
    [Range(-360, 0)]
    public int NegativeRotateLimit = 0;

    [Tooltip("Child Joint")]
    public Joint m_child;
    private GameObject MoveAreaPrefab;
    private GameObject MoveArea;
    public float RotateRadius { get; private set; } = 1f;
    private float RotateAngleThreshold = 0.5f;
    private bool isRotating = false;
    private Vector3 startAngle;
    public float angleNow { get; private set; } = 0f;
    private Vector3 rotatingAxis = Vector3.zero;
    /*protected enum RotateDirection : short
    {
        Clockwise = 1,
        Counterclockwise = -1
    }
    RotateDirection rotateDirection = RotateDirection.Clockwise;*/
    private Vector3 preTargetPosition = Vector3.zero;
    private Quaternion preRotation;
    private Quaternion oriRotation;

    private void OnEnable()
    {
        oriRotation = this.transform.localRotation;
        if(m_child != null)
        {
            RotateRadius = GetDistanceToChild();
            MoveAreaPrefab = Resources.Load<GameObject>("Prefabs/MoveArea");
            if(RotateRadius > 0)
            {
                MoveArea = Instantiate(MoveAreaPrefab);
                MoveArea.transform.position = this.transform.position;
                MoveArea.transform.localScale = Vector3.one * RotateRadius * 2;
                MoveArea.transform.parent = this.transform;
                MoveArea.transform.SetSiblingIndex(0);
            }
            startAngle = this.transform.localEulerAngles;
        }
    }

    public void Init()
    {
        angleNow = 0;
    }

    /// <summary>
    /// Get the sum of distance from this joint to end
    /// </summary>
    /// <returns></returns>
    public float GetDistanceToChild()
    {
        if(m_child != null)
        {
            return Vector3.Distance(this.transform.position, m_child.transform.position) + m_child.GetDistanceToChild();
        }
        return 0;
    }

    public Joint GetChild()
    {
        return m_child;
    }
    /// <summary>
    /// Get all Joints from this joint to end
    /// </summary>
    /// <returns>return all joint array</returns>
    public Joint[] GetAllChild()
    {
        List<Joint> joints = new List<Joint>();
        joints.Add(this);
        m_child.GetAllChild(ref joints);
        return joints.ToArray();
    }
    /// <summary>
    /// Used to add new joint to list
    /// </summary>
    /// <param name="joints"></param>
    private void GetAllChild(ref List<Joint> joints)
    {
        joints.Add(this);
        if(m_child != null)
        {
            m_child.GetAllChild(ref joints);
        }
    }

    public int GetChainLength()
    {
        return (m_child == null ? 0 :1 + m_child.GetChainLength());
    }

    public void Rotate(float angle)
    {
        //print(angle);
        //print($"{this.name} rotate : {angle}");
        CheckIsLimit(angle);
        preRotation = this.transform.rotation;
        switch(rotateAxis)
        {
            case RotateAxis.X:
                //float _angleNow = Vector3.SignedAngle(this.transform.up, m_child.transform.position - this.transform.position, this.transform.right);
                //this.transform.rotation = Quaternion.AngleAxis(_angleNow + angle, this.transform.right) * preRotation;
                this.transform.Rotate(new Vector3(angle, 0, 0));
                break;
            case RotateAxis.Y:
                this.transform.Rotate(new Vector3(0, angle, 0));
                break;
            case RotateAxis.Z:
                this.transform.Rotate(new Vector3(0, 0, angle));
                break;
        }
        angleNow += angle;
    }

    public void Rotate(Vector3 angle)
    {
        this.transform.localEulerAngles = startAngle + angle;
    }

    public void Rotate(Quaternion a, Quaternion b, float t)
    {
        transform.rotation = Quaternion.Slerp(a, b, t * Time.deltaTime);
    }

    public void Rotate(Quaternion a, float angle)
    {
        if(angle < 0)
        {
            a = Quaternion.Inverse(a);
            angle *= -1;
        }
        transform.rotation = Quaternion.Slerp(this.transform.rotation, a, angle);
    }
    /// <summary>
    /// If Target is inside MoveArea,
    /// the Vector from this joint to child joint will face to Target
    /// </summary>
    /// <param name="Target"></param>
    public void Rotate(Transform Target, bool instant = false)
    {

        if (m_child == null || (rotateAxis == RotateAxis.DontRotate)) return;

        Vector3 vectorToChild = m_child.transform.position - this.transform.position;
        Vector3 vectorToTarget = Target.position - this.transform.position;

        if (Target.position != preTargetPosition) CrossInit(Target);

        Debug.DrawLine(this.transform.position, this.transform.position + rotatingAxis, Color.red);
        Debug.DrawLine(this.transform.position, Target.position, Color.green);

        if(instant)
        {
            vectorToChild = m_child.transform.position - this.transform.position;
            float angle = Vector3.SignedAngle(vectorToChild, vectorToTarget, rotatingAxis);
            if (Mathf.Abs(angle) > RotateAngleThreshold)
            {
                this.transform.rotation = Quaternion.AngleAxis(angle, rotatingAxis) * preRotation;
            }
            else if (Mathf.Abs(angle) <= RotateAngleThreshold)
            {
                isRotating = false;
            }
        }
        else
        {
            for (int i = 0; i < RotateSpeed; i++)
            {
                vectorToChild = m_child.transform.position - this.transform.position;
                float angle = Vector3.SignedAngle(vectorToChild, vectorToTarget, rotatingAxis);
                if (Mathf.Abs(angle) > RotateAngleThreshold)
                {
                    angleNow += Time.deltaTime;
                    this.transform.rotation = Quaternion.AngleAxis(angleNow, rotatingAxis) * preRotation;
                }
                else if (Mathf.Abs(angle) <= RotateAngleThreshold)
                {
                    isRotating = false;
                }
            }
        }
    }

    private void CrossInit(Transform Target)
    {
        Vector3 vectorToChild = m_child.transform.position - this.transform.position;
        Vector3 vectorToTarget = Target.position - this.transform.position;
        preTargetPosition = Target.position;
        preRotation = this.transform.rotation;
        rotatingAxis = Vector3.Cross(vectorToChild, vectorToTarget).normalized;
        angleNow = 0;
        isRotating = true;
    }

    public void CheckIsLimit(float angle)
    {
        if(angle > 0 && (Mathf.Abs(PositiveRotateLimit - angleNow) < 0.1))
        {
            throw new UnityException($"{this.name}'s positive angle has reached limit");
        }
        else if(angle < 0 && (Mathf.Abs(NegativeRotateLimit - angleNow) < 0.1))
        {
            throw new UnityException($"{this.name}'s negative angle has reached limit");
        }
    }
}
