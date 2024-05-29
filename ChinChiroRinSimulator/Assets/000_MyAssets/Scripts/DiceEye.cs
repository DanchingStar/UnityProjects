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
    /// ���g�̖ڂ�ς���
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
    /// ���g�̍ŏI�I�Ȗڂ����߂�
    /// </summary>
    /// <param name="startMethodFlg">Start�֐�����ĂԂƂ���true</param>
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
    /// ���g�̐F�����߂�
    /// </summary>
    private void ChangeMyColor()
    {
        if(myFinalKind == myOriginalKind)
        {
            bool flg = false; // 1�̖ڂ̐F��ς���ꍇ��true

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
    /// ���g�̍ŏI�I�Ȗڂ�Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public DiceController.DiceEyeKinds GetMyFinalKind()
    {
        return myFinalKind;
    }

}
