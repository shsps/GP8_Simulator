using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepManager : MonoBehaviour
{
    public static StepManager instance;

    [SerializeField] private List<StepInfo> stepInfos = new List<StepInfo>();
    private int step = 0;
    [SerializeField] private float moveInterval = 1;
    private int moveIndex = 1;
    private bool isRepeat = false;
    [SerializeField] private Catchable[] catchableItems;
    [SerializeField] private bool useKeyboard = false;

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
            MoveNext();
        }
        else if(Input.GetKey(KeyCode.X) && !isRepeat)
        {
            MoveNextSlowly();
        }
        else if(Input.GetKeyDown(KeyCode.C))
        {
            isRepeat = isRepeat ? false : true;
        }
        else if(isRepeat)
        {
            MoveNextSlowly();
        }
    }

    public void AddStep(IKManager3D2 ik)
    {
        StepInfo newInfo = new StepInfo(ik);
        stepInfos.Add(newInfo);
        print(newInfo);
    }

    private void MoveToOrigin()
    {
        stepInfos[0].ik.init_MoveTool();
    }

    private void MoveNext()
    {
        if (step < 0)
        {
            throw new UnityException("Didn't set any point");
        }

        if (step == 0)
        {
            InitCatchableItem();
            MoveToOrigin();
            step++;
            return;
        }

        MoveDirectly(step - 1);
        step++;

        if(step > stepInfos.Count)
        {
            step = 0;
        }
    }

    private void MoveDirectly(int step)
    {
        StepInfo stepPre = step == 0 ?new StepInfo(): stepInfos[step - 1];
        StepInfo stepNow = stepInfos[step];
        float deltaAngleX = stepNow.MoveToolAngleX - stepPre.MoveToolAngleX;
        float deltaAngleY = stepNow.MoveToolAngleY - stepPre.MoveToolAngleY;
        float deltaAngleZ = stepNow.MoveToolAngleZ - stepPre.MoveToolAngleZ;
        stepNow.ik.MoveToolX(deltaAngleX);
        stepNow.ik.MoveToolY(deltaAngleY);
        stepNow.ik.MoveToolZ(deltaAngleZ);
        if(stepNow.IsCatchPressed)
        {
            stepNow.ik.SearchItemCatchable(false);
        }
    }

    private void MoveNextSlowly()
    {
        if(step < 0)
        {
            throw new UnityException("Didn't set any point");
        }

        if (step == 0)
        {
            InitCatchableItem();
            MoveToOrigin();
            step++;
            return;
        }

        if(MoveSlowly(step - 1))
        {
            step++;
        }

        if (step > stepInfos.Count)
        {
            step = 0;
        }
    }

    private bool MoveSlowly(int step)
    {
        StepInfo stepPre = step == 0 ? new StepInfo() : stepInfos[step - 1];
        StepInfo stepNow = stepInfos[step];
        float deltaAngleX = stepNow.MoveToolAngleX - stepPre.MoveToolAngleX;
        float deltaAngleY = stepNow.MoveToolAngleY - stepPre.MoveToolAngleY;
        float deltaAngleZ = stepNow.MoveToolAngleZ - stepPre.MoveToolAngleZ;

        float rotateAngleThisTimeX = deltaAngleX / moveInterval;
        float rotateAngleThisTimeY = deltaAngleY / moveInterval;
        float rotateAngleThisTimeZ = deltaAngleZ / moveInterval;

        //print($"{deltaAngleX}, {deltaAngleY}, {deltaAngleZ}");
        //print($"index : {moveIndex}, ({rotateAngleThisTimeX}, {rotateAngleThisTimeY}, {rotateAngleThisTimeZ})");

        stepNow.ik.MoveToolX(rotateAngleThisTimeX);
        stepNow.ik.MoveToolY(rotateAngleThisTimeY);
        stepNow.ik.MoveToolZ(rotateAngleThisTimeZ);
        

        if(moveIndex == moveInterval)
        {
            moveIndex = 1;
            if (stepNow.IsCatchPressed)
            {
                stepNow.ik.SearchItemCatchable(false);
            }
            return true;
        }
        moveIndex++;
        return false;
    }

    private void InitCatchableItem()
    {
        foreach (var item in catchableItems)
        {
            item.Init();
        }
    }
}
