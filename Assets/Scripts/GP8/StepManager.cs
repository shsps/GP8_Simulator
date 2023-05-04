using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepManager : MonoBehaviour
{
    public static StepManager instance;
    private List<List<StepInfo>> stepInfosList = new List<List<StepInfo>>();
    public List<StepInfo> stepInfos = new List<StepInfo>();
    [SerializeField] private List<StepInfo> stepInfosNow = new List<StepInfo>();
    [SerializeField] public int stepOrder = 0;
    public int step = 0;
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
    private bool isMovingSlowly = false;

    private void Start()
    {
        instance = this;
        if(moveInterval < 1)
        {
            throw new UnityException("MoveInterval can not less than 1");
        }
        moveInterval = ((int)(moveInterval / Time.fixedDeltaTime));

        ResetCatchableItemOrigin();

        ReImportStepInfosList();
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
        else if(Input.GetKeyDown(KeyCode.P))
        {
            ResetRobotArm();
        }
    }

    /// <summary>
    /// Re-setting all catchableItem's origin position and rotation
    /// </summary>
    public void ResetCatchableItemOrigin()
    {
        foreach (var item in catchableItems)
        {
            item.SetOrigin();
        }
    }

    /// <summary>
    /// If you generate a new StepInfosList you can use this function to reimport StepsInfoList
    /// </summary>
    public void ReImportStepInfosList()
    {
        stepInfosList.Clear();
        List<List<StepInfo>> get = StepInfoGenerator.StepInfoReader();
        if(get != null)
        {
            foreach (var item in StepInfoGenerator.StepInfoReader())
            {
                stepInfosList.Add(item);
            }
            print($"Step Infos List count : {stepInfosList?.Count}");
            stepInfosNow = new List<StepInfo>(stepInfosList[stepOrder]);
        }
        else
        {
            print("StepInfoPrefabs.txt doesn't have any stepInfo");
        }
    }

    /// <summary>
    /// Change step order to next or previous
    /// </summary>
    /// <param name="changeOrder">If this value bigger than 0, step order change to next, otherwise previous</param>
    public void ChangeStepOrder(int changeOrder)
    {
        if((stepOrder + changeOrder) < 0 | (stepOrder + changeOrder) >= stepInfosList.Count)
        {
            throw new System.IndexOutOfRangeException($"{nameof(changeOrder)} is out of range");
        }
        changeOrder = changeOrder > 0 ? 1 : -1;
        stepOrder += changeOrder;

        if(stepInfosNow[0].Ik.catchStatusNow == IKManager3D2.CatchStatus.Catch)
        {
            stepInfosNow[0].Ik.ForceReleaseItem();
        }

        ResetRobotArm();
        stepInfosNow.Clear();
        stepInfosNow = new List<StepInfo>(stepInfosList[stepOrder]);
        isMovingSlowly = false;
        step = 0;
        preStep = 0;
    }

    /// <summary>
    /// Add the step now to stepinfos
    /// </summary>
    /// <param name="ik"></param>
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

    /// <summary>
    /// Force release item on catching, 
    /// reset it to origin position , reset robot arm position
    /// and set step to 0
    /// </summary>
    public void ResetRobotArm()
    {
        stepInfosNow[0].Ik.ForceReleaseItem();
        InitCatchableItem();
        MoveToOrigin();
        step = 0;
    }

    //Bug : If your previous step catch an item that doesn't at the origin position,
    //      and you move to previous step. This function will move the item to origin position.
    /// <summary>
    /// Move to next or previous step directly
    /// </summary>
    /// <param name="stepDirection"></param>
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
            if(step > 0 && step < stepInfosNow.Count)
            {
                if (stepInfosNow[step].CatchStatusNow == IKManager3D2.CatchStatus.Catch &&
                   stepInfosNow[step - 1].CatchStatusNow == IKManager3D2.CatchStatus.None)
                {
                    stepInfosNow[step].Ik.ForceReleaseItem();
                    InitCatchableItem();
                }
            }
        }

        if(step == 0)
        {
            ResetRobotArm();
        }
        /*else
        {
            MoveToOrigin();
        }*/

        MoveToOrigin();
        #region Detect catchableItem need to be released or not

        #endregion
        //print($"step : {step}");

        #region move from 0 to step
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
            if (targetStepInfo.CatchStatusNow == IKManager3D2.CatchStatus.Catch && stepInfosNow[i].Ik.catchStatusNow != IKManager3D2.CatchStatus.Catch)
            {
                targetStepInfo.Ik.SearchItemCatchable(false);
            }
            else if(targetStepInfo.CatchStatusNow == IKManager3D2.CatchStatus.Release && stepInfosNow[i].Ik.catchStatusNow != IKManager3D2.CatchStatus.Release)
            {
                targetStepInfo.Ik.SearchItemCatchable(false);
            }
        }
        #endregion
        preStep = step;
        
        moveIndex = 0;
        isMovingSlowly = false;
    }

    /// <summary>
    /// Move to next or previous step solwly according to moveInterval
    /// </summary>
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
            if (targetStepInfo.CatchStatusNow == IKManager3D2.CatchStatus.Catch && stepInfosNow[step - 1].Ik.catchStatusNow != IKManager3D2.CatchStatus.Catch)
            {
                targetStepInfo.Ik.SearchItemCatchable(false);
            }
            else if (targetStepInfo.CatchStatusNow == IKManager3D2.CatchStatus.Release && stepInfosNow[step - 1].Ik.catchStatusNow != IKManager3D2.CatchStatus.Release)
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
