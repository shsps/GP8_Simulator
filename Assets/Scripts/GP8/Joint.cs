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

    public float JointAngle
    {
        get
        {
            return _jointAngle;
        }
    }
    [SerializeField] private float _jointAngle;
    private float oriJointAngle;
    public float AngleForCaculate
    {
        get
        {
            return _angleForCaculate;
        }
    }
    [SerializeField] private float _angleForCaculate;
    private Vector3 rotatingAxis = Vector3.zero;
    private Vector3 preTargetPosition = Vector3.zero;
    private Quaternion preRotation;
    private Quaternion oriRotation;

    private void OnEnable()
    {
        oriRotation = this.transform.localRotation;

        switch(rotateAxis)
        {
            case RotateAxis.X:
                oriJointAngle = this.transform.localRotation.eulerAngles.x;
                break;
            case RotateAxis.Y:
                oriJointAngle = this.transform.localRotation.eulerAngles.y;
                break;
            case RotateAxis.Z:
                oriJointAngle = this.transform.localRotation.eulerAngles.z;
                break;
        }
        Init();

        if(m_child != null)
        {
            RotateRadius = GetDistanceToChild();
            /*MoveAreaPrefab = Resources.Load<GameObject>("Prefabs/MoveArea");
            if(RotateRadius > 0)
            {
                MoveArea = Instantiate(MoveAreaPrefab);
                MoveArea.transform.position = this.transform.position;
                MoveArea.transform.localScale = Vector3.one * RotateRadius * 2;
                MoveArea.transform.parent = this.transform;
                MoveArea.transform.SetSiblingIndex(0);
            }*/
            startAngle = this.transform.localEulerAngles;
        }
    }

    public void Init()
    {
        _angleForCaculate = 0;
        _jointAngle = oriJointAngle;
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
        if(float.IsNaN(angle))
        {
            throw new UnityException("Rotate angle is not a number");
        }

        CheckIsLimit(angle);
        preRotation = this.transform.rotation;
        switch(rotateAxis)
        {
            case RotateAxis.X:
                this.transform.Rotate(new Vector3(angle, 0, 0));
                break;
            case RotateAxis.Y:
                this.transform.Rotate(new Vector3(0, angle, 0));
                break;
            case RotateAxis.Z:
                this.transform.Rotate(new Vector3(0, 0, angle));
                break;
        }
        _angleForCaculate += angle;
        _jointAngle += angle;
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
                    _angleForCaculate += Time.deltaTime;
                    this.transform.rotation = Quaternion.AngleAxis(_angleForCaculate, rotatingAxis) * preRotation;
                }
                else if (Mathf.Abs(angle) <= RotateAngleThreshold)
                {
                    isRotating = false;
                }
            }
        }
    }

    private void CrossInit(Transform Target)// use at MoveArea Rotate
    {
        Vector3 vectorToChild = m_child.transform.position - this.transform.position;
        Vector3 vectorToTarget = Target.position - this.transform.position;
        preTargetPosition = Target.position;
        preRotation = this.transform.rotation;
        rotatingAxis = Vector3.Cross(vectorToChild, vectorToTarget).normalized;
        _angleForCaculate = 0;
        isRotating = true;
    }

    public void CheckIsLimit(float angle)
    {
        if(angle > 0 && (Mathf.Abs(PositiveRotateLimit - _jointAngle) < 0.1))//reach the positive angle limit
        {
            throw new JointLimitException($"{this.name} reach the positive angle limit");
        }
        else if(angle < 0 && (Mathf.Abs(NegativeRotateLimit - _jointAngle) < 0.1))
        {
            throw new JointLimitException($"{this.name} reach the negative angle limit");
        }
    }

    public void ChangeAngleValue(float value)
    {
        _angleForCaculate += value;
        _jointAngle += value;
    }
}
