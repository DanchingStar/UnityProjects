using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rainbow : MonoBehaviour
{
    [SerializeField] float speed = 10f;

    private Text image; //�g���Ώۂɂ���Č^��ς���K�v�A��

    void Start()
    {
        //�C���[�W�R���|�[�l���g�擾
        image = GetComponent<Text>();
        //image�̃J���[��ύX����R���[�`�����X�^�[�g
        StartCoroutine(_rainbow());
    }

    //���F�ɕω�����R���[�`��
    IEnumerator _rainbow()
    {
        //�������[�v
        while (true)
        {
            //�J���[��ω������鏈��
            image.color = Color.HSVToRGB(Time.time / speed % 1, 1, 1);
            //1�t���[���҂�
            yield return new WaitForFixedUpdate();
        }
    }
}