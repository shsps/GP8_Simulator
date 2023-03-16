using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialBoard : MonoBehaviour
{
    [SerializeField] private Text text;
    [SerializeField] private Text title;
    private string[][] textArray =
    {
        new string[] {"按下X+可向右移動", "按下X-可向左移動", "按下Y+可向上移動", "按下Y-可向下移動", "按下Z+可向前移動", "按下Z-可向後移動"},
        new string[] {"第一步移動到方塊正上方", "第二步向下移動到可夾取範圍", "第三部按下夾取按鈕"}
    };
    public int TextArrayOrder = 0;
    public int TextOrder = 0;
    public enum ChangeOrderDirection
    {
        Next,
        Previous
    }

    void Start()
    {
        title.text = "基礎教學";
        text.text = textArray[TextArrayOrder][TextOrder];
    }

    public void ChangeTextOrder(ChangeOrderDirection direction)//Step
    {
        if (direction == ChangeOrderDirection.Next)
        {
            if(TextOrder == textArray[TextArrayOrder].Length - 1)
            {
                throw new System.IndexOutOfRangeException("TextOrder is at the last.");
            }
            TextOrder++;
        }
        else if (direction == ChangeOrderDirection.Previous)
        {
            if(TextOrder == 0)
            {
                throw new System.IndexOutOfRangeException("TextOrder is at the first");
            }
            TextOrder--;
        }

        text.text = textArray[TextArrayOrder][TextOrder];
    }

    public void ChangeTextArrayOrder(ChangeOrderDirection direction)//Action
    {
        if (direction == ChangeOrderDirection.Next)
        {
            if (TextArrayOrder == textArray.Length - 1)
            {
                throw new System.IndexOutOfRangeException("TextArrayOrder is at the last.");
            }
            TextArrayOrder++;
        }
        else if (direction == ChangeOrderDirection.Previous)
        {
            if (TextArrayOrder == 0)
            {
                throw new System.IndexOutOfRangeException("TextArrayOrder is at the first");
            }
            TextArrayOrder--;
        }
        TextOrder = 0;

        text.text = textArray[TextArrayOrder][TextOrder];
    }

    public void ChangeTitle()//Title
    {
        switch (TextArrayOrder)
        {
            case 0:
                title.text = "基礎教學";
                break;
            case 1:
                title.text = "夾取及紀錄教學";
                break;
        }
    }
}
