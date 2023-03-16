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
        new string[] {"���UX+�i�V�k����", "���UX-�i�V������", "���UY+�i�V�W����", "���UY-�i�V�U����", "���UZ+�i�V�e����", "���UZ-�i�V�Ჾ��"},
        new string[] {"�Ĥ@�B���ʨ������W��", "�ĤG�B�V�U���ʨ�i�����d��", "�ĤT�����U�������s"}
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
        title.text = "��¦�о�";
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
                title.text = "��¦�о�";
                break;
            case 1:
                title.text = "�����ά����о�";
                break;
        }
    }
}
