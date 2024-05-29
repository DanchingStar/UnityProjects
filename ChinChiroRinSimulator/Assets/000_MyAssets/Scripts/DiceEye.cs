using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceEye : MonoBehaviour
{
    [SerializeField] private DiceController.DiceEyeKinds myOriginalKind = DiceController.DiceEyeKinds.None;
    private DiceController.DiceEyeKinds myChangedKind = DiceController.DiceEyeKinds.None;
    private DiceController.DiceEyeKinds myFinalKind = DiceController.DiceEyeKinds.None;

    private MeshRenderer myRenderer;

    private bool finishStartFlg = false;
    private bool rainbowFlg = false;

    private const float SPEED_RAINBOW = 0.5f;

    private void Start()
    {
        if (finishStartFlg) return;

        finishStartFlg = true;

        myRenderer = GetComponent<MeshRenderer>();

        DecideMyFinalKind(true);
        ChangeMyColor();
    }

    private void Update()
    {
        if (rainbowFlg)
        {
            myRenderer.material.color = Color.HSVToRGB((Time.time * SPEED_RAINBOW) % 1, 1, 1);
        }
    }

    /// <summary>
    /// 自身の目を変える
    /// </summary>
    /// <param name="_changedKind"></param>
    public void ChangeMyKind(DiceController.DiceEyeKinds _changedKind)
    {
        if (!finishStartFlg) Start();

        myChangedKind = _changedKind;
        DecideMyFinalKind(false);
        ChangeMyColor();
    }

    /// <summary>
    /// 自身の最終的な目を決める
    /// </summary>
    /// <param name="startMethodFlg">Start関数から呼ぶときはtrue</param>
    private void DecideMyFinalKind(bool startMethodFlg)
    {
        if (startMethodFlg)
        {
            myFinalKind = myOriginalKind;

            //myRenderer.material = DiceController.Instance.GetEyeMaterial(myFinalKind);
        }
        else
        {
            if (myChangedKind == DiceController.DiceEyeKinds.None)
            {
                myFinalKind = myOriginalKind;
            }
            else
            {
                myFinalKind = myChangedKind;

                myRenderer.material = DiceController.Instance.GetEyeMaterial(myFinalKind);
            }
        }
    }

    /// <summary>
    /// 自身の色を決める
    /// </summary>
    private void ChangeMyColor()
    {
        if(myFinalKind == myOriginalKind)
        {
            bool flg = false; // 1の目の色を変える場合にtrue

            if (myFinalKind == DiceController.DiceEyeKinds.Eye1 && flg)
            {
                int diceNumber = transform.parent.gameObject.GetComponent<DiceMain>().GetMyDiceNumber();
                switch (diceNumber)
                {
                    case 0: myRenderer.material.color = Color.blue; break;
                    case 1: myRenderer.material.color = Color.red; break;
                    case 2: myRenderer.material.color = Color.green; break;
                    default: myRenderer.material.color = Color.white; break;
                }
            }
            else
            {
                myRenderer.material.color = Color.white;
            }
        }
        else
        {
            rainbowFlg = true;
        }
    }

    /// <summary>
    /// 自身の最終的な目を返すゲッター
    /// </summary>
    /// <returns></returns>
    public DiceController.DiceEyeKinds GetMyFinalKind()
    {
        return myFinalKind;
    }

}
