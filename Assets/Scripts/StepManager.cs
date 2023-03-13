using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepManager : MonoBehaviour
{
    public static StepManager instance;
    private List<List<StepInfo>> stepInfosList = new List<List<StepInfo>>();
    [SerializeField] private List<StepInfo> stepInfos = new List<StepInfo>();
    [SerializeField] private List<StepInfo> stepInfosNow = new List<StepInfo>();
    [SerializeField] private int stepOrder = 0;
    private int step = 0;
    private int preStep = 0;
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
    private bool isMovingSlowly = false;

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

        foreach (var item in StepInfoGenerator.StepInfoReader())
        {
            stepInfosList.Add(item);
        }
        print($"Step Infos List count : {stepInfosList.Count}");
        stepInfosNow = new List<StepInfo>(stepInfosList[0]);
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
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangeStepOrder(-1);
        }
        else if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeStepOrder(1);
        }
    }

    public void ChangeStepOrder(int changeOrder)
    {
        if((stepOrder + changeOrder) < 0 | (stepOrder + changeOrder) >= stepInfosList.Count)
        {
            throw new System.IndexOutOfRangeException($"{nameof(changeOrder)} is out of range");
        }
        changeOrder = changeOrder > 0 ? 1 : -1;
        stepOrder += changeOrder;

        ResetRobotArm();
        stepInfosNow.Clear();
        stepInfosNow = new List<StepInfo>(stepInfosList[stepOrder]);
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
        stepInfosNow[0].Ik.init_MoveTool();
    }

    public void ResetRobotArm()
    {
        InitCatchableItem();
        MoveToOrigin();
    }

    public void MoveDirectly(changeStepDirection stepDirection)
    {
        if (stepInfosNow.Count == 0) throw new UnityException("Didn't set any point");
        
        if (stepDirection == changeStepDirection.positive && moveIndex == 0)
        {
            step = step == stepInfosNow.Count ? 0 : step + 1;
        }
        else if (stepDirection == changeStepDirection.negative)
        {
            step = step == 0 ? stepInfosNow.Count : step - 1;
        }

        ResetRobotArm();

        for (int i = 0; i < step; i++)
        {
            StepInfo preStepInfo = i == 0 ? new StepInfo() : stepInfosNow[i - 1];
            StepInfo targetStepInfo = stepInfosNow[i];

            float deltaAngleX = targetStepInfo.MoveToolAngleX - preStepInfo.MoveToolAngleX;
            float deltaAngleY = targetStepInfo.MoveToolAngleY - preStepInfo.MoveToolAngleY;
            float deltaAngleZ = targetStepInfo.MoveToolAngleZ - preStepInfo.MoveToolAngleZ;

            targetStepInfo.Ik.MoveToolX(deltaAngleX);
            targetStepInfo.Ik.MoveToolY(deltaAngleY);
            targetStepInfo.Ik.MoveToolZ(deltaAngleZ);
            if (targetStepInfo.IsCatchPressed)
            {
                targetStepInfo.Ik.SearchItemCatchable(false);
            }
        }
        preStep = step;
        
        moveIndex = 0;
        isMovingSlowly = false;
    }

    public void MoveNextSlowly()
    {
        if(stepInfosNow.Count < 0) throw new UnityException("Didn't set any point");

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
            if (step == stepInfosNow.Count + 1)
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
    }

    private bool MoveSlowly(int step)
    {
        StepInfo preStepInfo = preStep == 0 ? new StepInfo() : stepInfosNow[preStep - 1];
        StepInfo targetStepInfo = stepInfosNow[step-1];
        float deltaAngleX = targetStepInfo.MoveToolAngleX - preStepInfo.MoveToolAngleX;
        float deltaAngleY = targetStepInfo.MoveToolAngleY - preStepInfo.MoveToolAngleY;
        float deltaAngleZ = targetStepInfo.MoveToolAngleZ - preStepInfo.MoveToolAngleZ;

        float rotateAngleThisTimeX = deltaAngleX / moveInterval;
        float rotateAngleThisTimeY = deltaAngleY / moveInterval;
        float rotateAngleThisTimeZ = deltaAngleZ / moveInterval;

        //print($"{deltaAngleX}, {deltaAngleY}, {deltaAngleZ}");
        //print($"index : {moveIndex}, ({rotateAngleThisTimeX}, {rotateAngleThisTimeY}, {rotateAngleThisTimeZ})");

        targetStepInfo.Ik.MoveToolX(rotateAngleThisTimeX);
        targetStepInfo.Ik.MoveToolY(rotateAngleThisTimeY);
        targetStepInfo.Ik.MoveToolZ(rotateAngleThisTimeZ);
        

        if(moveIndex == moveInterval)
        {
            moveIndex = 0;
            if (targetStepInfo.IsCatchPressed)
            {
                targetStepInfo.Ik.SearchItemCatchable(false);
            }
            preStep = step;
            return true;
        }
        moveIndex++;
        return false;
    }

}
