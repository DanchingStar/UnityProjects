using Menu;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextListGenerater : MonoBehaviour
{
    [SerializeField] TextListEntry textListEntity;

    public static TextListGenerater instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public TextList Spawn(TextList.Stage stage)
    {
        foreach (var item in textListEntity.textList)
        {
            if (item.stage == stage)
            {
                return new TextList(item.stage, item.stageNameTextLine1, item.stageNameTextLine2,
                    item.stageIntroductionTextLine1, item.stageIntroductionTextLine2, item.stageIntroductionTextLine3);
            }
        }
        return null;
    }

    public string StageToStageNameText(TextList.Stage stage)
    {
        if (stage == TextList.Stage.None)
        {
            Debug.Log("TextListGenerater.StageToStageNameText : stage is None");
            return null;
        }

        foreach (var item in textListEntity.textList)
        {
            if (item.stage == stage)
            {
                if (item.stageNameTextLine2 != "")
                {
                    return item.stageNameTextLine1 + "\n" + item.stageNameTextLine2;
                }
                else
                {
                    return item.stageNameTextLine1;
                }                
            }
        }
        return null;
    }

    public string StageToStageIntroductionText(TextList.Stage stage)
    {
        if (stage == TextList.Stage.None)
        {
            Debug.Log("TextListGenerater.StageToStageIntroductionText : stage is None");
            return null;
        }

        foreach (var item in textListEntity.textList)
        {
            if (item.stage == stage)
            {
                string str = item.stageIntroductionTextLine1;

                if (item.stageIntroductionTextLine2 != "")
                {
                    str = str + "\n" + item.stageIntroductionTextLine2;
                }
                if (item.stageIntroductionTextLine3 != "")
                {
                    str = str + "\n" + item.stageIntroductionTextLine3;
                }

                return str;
            }
        }
        return null;
    }
}
