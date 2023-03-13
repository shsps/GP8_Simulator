using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepManager : MonoBehaviour
{
    public static StepManager instance;

    [SerializeField] private List<StepInfo> stepInfos = new List<StepInfo>();
    [SerializeField] private int step = 0;
    [SerializeField] private int preStep = 0;
    public enum changeStepDirection
    {
        positive,
        negative
    }
    private bool hasReset = false;
    [Tooltip("How many second does it need from this step to next")]
    [SerializeField] private float moveInterval = 1;
    private int moveIndex = 0;
    private bool isRepeat = false;
    [SerializeField] private Catchable[] catchableItems;
    [SerializeField] private bool useKeyboard = false;
    public bool isThisStepRepeat = false;
    [SerializeField] private bool isMovingSlowly = false;

    private void Start()
    {
        instance = this;
        if(moveInterval < 1)
        {
            throw new UnityException("MoveInterval can not less than 1");
        }
        moveInterval = ((int)(moveInterval / Time.fixedDeltaTime));

        foreach (var item in catchableItems)
        {
            item.SetOrigin();
        }
    }

    private void Update()
    {
        if (!useKeyboard) return;

        if(Input.GetKeyDown(KeyCode.Z))
        {
            MoveDirectly(changeStepDirection.positive);
        }
        else if(Input.GetKeyDown(KeyCode.X))
        {
            MoveDirectly(changeStepDirection.negative);
        }
        else if(Input.GetKey(KeyCode.C) && !isRepeat)
        {
            MoveNextSlowly();
        }
        else if(Input.GetKeyDown(KeyCode.V))
        {
            isRepeat = isRepeat ? false : true;
        }
        else if(isRepeat)
        {
            MoveNextSlowly();
        }
        else if(Input.GetKeyDown(KeyCode.B))
        {
            isThisStepRepeat = isThisStepRepeat ? false : true;
        }
    }

    public void AddStep(IKManager3D2 ik)
    {
        StepInfo newInfo = new StepInfo(ik);
        stepInfos.Add(newInfo);
        step = stepInfos.Count;
        print(newInfo);
    }

    private void InitCatchableItem()
    {
        foreach (var item in catchableItems)
        {
            item.Init();
        }
    }

    private void MoveToOrigin()
    {
        stepInfos[0].ik.init_MoveTool();
    }

    public void ResetRobotArm()
    {
        InitCatchableItem();
        MoveToOrigin();
    }

    public void MoveDirectly(changeStepDirection stepDirection)
    {
        if (stepInfos.Count == 0) throw new UnityException("Didn't set any point");
        
        if (stepDirection == changeStepDirection.positive && moveIndex == 0)
        {
            step = step == stepInfos.Count ? 0 : step + 1;
        }
        else if (stepDirection == changeStepDirection.negative)
        {
            step = step == 0 ? stepInfos.Count : step - 1;
        }

        ResetRobotArm();

        for (int i = 0; i < step; i++)
        {
            StepInfo preStepInfo = i == 0 ? new StepInfo() : stepInfos[i - 1];
            StepInfo targetStepInfo = stepInfos[i];

            float deltaAngleX = targetStepInfo.MoveToolAngleX - preStepInfo.MoveToolAngleX;
            float deltaAngleY = targetStepInfo.MoveToolAngleY - preStepInfo.MoveToolAngleY;
            float deltaAngleZ = targetStepInfo.MoveToolAngleZ - preStepInfo.MoveToolAngleZ;

            targetStepInfo.ik.MoveToolX(deltaAngleX);
            targetStepInfo.ik.MoveToolY(deltaAngleY);
            targetStepInfo.ik.MoveToolZ(deltaAngleZ);
            if (targetStepInfo.IsCatchPressed)
            {
                targetStepInfo.ik.SearchItemCatchable(false);
            }
        }
        preStep = step;
        
        moveIndex = 0;
        isMovingSlowly = false;
    }//呼叫來進入下一步

    public void MoveNextSlowly()
    {
        if(stepInfos.Count < 0) throw new UnityException("Didn't set any point");

        if (!hasReset)
        {
            ResetRobotArm();
            hasReset = true;
            return;
        }
        
        if(!isMovingSlowly)
        {
            step++;
            isMovingSlowly = true;
            if (step == stepInfos.Count + 1)
            {
                ResetRobotArm();
                step = 1;
                preStep = 0;
                return;
            }
        }


        if (MoveSlowly(step))
        {
            isMovingSlowly = false;
            if(isThisStepRepeat)
            {
                MoveDirectly(changeStepDirection.negative);
            }
        }
    }//重複播放

    private bool MoveSlowly(int step)
    {
        StepInfo preStepInfo = preStep == 0 ? new StepInfo() : stepInfos[preStep - 1];
        StepInfo targetStepInfo = stepInfos[step-1];
        float deltaAngleX = targetStepInfo.MoveToolAngleX - preStepInfo.MoveToolAngleX;
        float deltaAngleY = targetStepInfo.MoveToolAngleY - preStepInfo.MoveToolAngleY;
        float deltaAngleZ = targetStepInfo.MoveToolAngleZ - preStepInfo.MoveToolAngleZ;

        float rotateAngleThisTimeX = deltaAngleX / moveInterval;
        float rotateAngleThisTimeY = deltaAngleY / moveInterval;
        float rotateAngleThisTimeZ = deltaAngleZ / moveInterval;

        //print($"{deltaAngleX}, {deltaAngleY}, {deltaAngleZ}");
        //print($"index : {moveIndex}, ({rotateAngleThisTimeX}, {rotateAngleThisTimeY}, {rotateAngleThisTimeZ})");

        targetStepInfo.ik.MoveToolX(rotateAngleThisTimeX);
        targetStepInfo.ik.MoveToolY(rotateAngleThisTimeY);
        targetStepInfo.ik.MoveToolZ(rotateAngleThisTimeZ);
        

        if(moveIndex == moveInterval)
        {
            moveIndex = 0;
            if (targetStepInfo.IsCatchPressed)
            {
                targetStepInfo.ik.SearchItemCatchable(false);
            }
            preStep = step;
            return true;
        }
        moveIndex++;
        return false;
    }

}
