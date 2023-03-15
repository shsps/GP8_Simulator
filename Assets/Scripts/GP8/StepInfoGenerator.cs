using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepInfoGenerator : MonoBehaviour
{
    //private List<StepInfo> stepInfoList = new List<StepInfo>();

    private static Regex regex1 = new Regex(@"{[^{^}]+}", RegexOptions.Multiline);
    private static Regex regex2 = new Regex(@"((-?\d+\.?\d*,)|[A-Za-z]+,?)+", RegexOptions.Multiline);

    public static List<List<StepInfo>> StepInfoReader()
    {
        List<List<StepInfo>> result = new List<List<StepInfo>>();
        string readText = File.ReadAllText($"{Application.dataPath}/Resources/StepInfoPrefabs.txt");
        if(readText.Length <= 2)
        {
            print("StepInfoPrefabs.txt doesn't have any stepInfo");
            return null;
        }

        MatchCollection matchCollection1 = regex1.Matches(readText);
        //print(readText);
        foreach (Match match1 in matchCollection1)
        {
            List<string> group1 = new List<string>();
            MatchCollection matchCollection2 = regex2.Matches(match1.Value);
            List<StepInfo> tmpStepInfo = new List<StepInfo>();

            //Debug.Log(match1.Value);
            foreach (Match match2 in matchCollection2)
            {
                group1.Add(match2.Value);
            }

            GameObject robotArm = GameObject.Find(group1[0].Replace(",", ""));
            if(robotArm == null)
            {
                print($"Can not find robot arm named {group1[0]}");
                continue;
            }
            robotArm.TryGetComponent<IKManager3D2>(out IKManager3D2 _ik);
            for (int i = 1; i < group1.Count; i++)
            {
                string[] sp = group1[i].Split(','); //rotate angle and isCatch
                tmpStepInfo.Add(new StepInfo(_ik,
                    float.Parse(sp[0]),
                    float.Parse(sp[1]),
                    float.Parse(sp[2]),
                    bool.Parse(sp[3]),
                    CatchStatusParse(sp[4])));
            }
            result.Add(tmpStepInfo);
        }
        return result;
    }

    public static void StepInfoWriter()
    {
        if(StepManager.instance.stepInfos.Count <= 0)
        {
            throw new UnityException("Generate failed, didn't save any point");
        }
        List<StepInfo> getStepInfos = new List<StepInfo>(StepManager.instance.stepInfos);

        string readText = File.ReadAllText($"{Application.dataPath}/Resources/StepInfoPrefabs.txt");

        string appendText = "{}";
        if(readText.Length > 2)
        {
            appendText = appendText.Insert(0, ",");
        }
        appendText = appendText.Insert(appendText.Length - 1, $"{getStepInfos[0].Ik.name},");

        for (int i = 0; i < getStepInfos.Count; i++)
        {
            appendText = appendText.Insert(appendText.Length - 1,
                $"[{getStepInfos[i].MoveToolAngleX}," +
                $"{getStepInfos[i].MoveToolAngleY}," +
                $"{getStepInfos[i].MoveToolAngleZ}," +
                $"{getStepInfos[i].IsCatchPressed}," +
                $"{getStepInfos[i].CatchStatusNow}]");
            if(i < getStepInfos.Count - 1)
            {
                appendText = appendText.Insert(appendText.Length - 1, ",");
            }
        }

        readText = readText.Insert(readText.Length - 1, appendText);

        File.WriteAllText($"{Application.dataPath}/Resources/StepInfoPrefabs.txt", readText);


        StepManager.instance.ReImportStepInfosList();

        print($"Generate sucess");
    }

    private static IKManager3D2.CatchStatus CatchStatusParse(string s)
    {
        switch(s)
        {
            case "None":
                return IKManager3D2.CatchStatus.None;
            case "Catch":
                return IKManager3D2.CatchStatus.Catch;
            case "Catching":
                return IKManager3D2.CatchStatus.Catching;
            case "Release":
                return IKManager3D2.CatchStatus.Release;
        }
        return IKManager3D2.CatchStatus.None;
    }
}
