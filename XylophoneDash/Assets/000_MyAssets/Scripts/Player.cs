using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float jumpHeight = 0.7f;
    private float gameoverHeight = -3f;
    private float disappearHeight = -500f;

    private bool jumpFlg;
    private bool gameoverFlg;

    private Vector3 myPosition;

    //private CharacterController controller;
    //private ThirdPersonController characterController;
    //private StarterAssetsInputs starterAssetsInputs;
    //private PlayerInput playerInput;
    //private Vector3 externalForce;

    private void Start()
    {
        ChangeJumpFlg(false);
        ChangeGameoverFlg(false);
        //controller = GetComponent<CharacterController>();
        //characterController = GetComponent<ThirdPersonController>();
        //starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        //playerInput = GetComponent<PlayerInput>();
        //externalForce = Vector3.zero;
    }

    private void Update()
    {
        UpdateMyPosition();
        CheckJump();
        CheckGameover();
        CheckDisappearHeight();

        //// �O������̗͂�������
        //controller.Move(externalForce);

        //// ���̃t���[���̂��߂ɊO������̗͂����Z�b�g
        //externalForce = Vector3.zero;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.gameObject.transform.parent.gameObject.tag == "Keyboard")
        {
            if (!jumpFlg) return;
            hit.gameObject.transform.parent.gameObject.GetComponent<NotesKeyboard>().StepMe(GetComponent<Player>());
        }
        //else if(hit.gameObject.tag == "Enemy")
        //{
        //    Vector3 damageVector = hit.gameObject.GetComponent<EnemyBase>().HitPlayer(GetComponent<Player>());

        //    StartCoroutine(DamageMe(damageVector));

        //    Debug.Log($"Damage {damageVector}");
        //}
        //else
        //{
        //    Debug.Log($"collision ! {hit.gameObject.name}");
        //}
    }

    //private IEnumerator DamageMe(Vector3 powor)
    //{
    //    GameSceneManager.Instance.SwitchOperatePlayerEnable(false);
    //    //GetComponent<CharacterController>().enabled = false;

    //    // �ǂ��ɂ����ė͂��������� !!!

    //    //AddForce(powor);
    //    //GetComponent<Rigidbody>().AddForce(powor,ForceMode.VelocityChange);

    //    float roopCount = 60;
    //    Vector3 firstPos = transform.position;
    //    for (int i = 0; i < roopCount; i++)
    //    {
    //        transform.position = Vector3.Lerp(firstPos, firstPos + powor, (float)(i + 1) / (float)roopCount);
    //        yield return new WaitForSeconds((float)1 / (float)roopCount);
    //    }
    //    Debug.Log($"{firstPos} ->{firstPos + powor}");

    //    //yield return new WaitForSeconds(1f);

    //    GameSceneManager.Instance.SwitchOperatePlayerEnable(true);
    //    //GetComponent<CharacterController>().enabled = true;
    //}


    /// <summary>
    /// �W�����v�������`�F�b�N����
    /// </summary>
    private void CheckJump()
    {
        if (jumpFlg) return; // ���łɃW�����v���Ȃ珜�O
        if (jumpHeight > gameObject.transform.position.y) return; // �����Ŕ��f
        ChangeJumpFlg(true);
    }

    /// <summary>
    /// GameOver�������`�F�b�N����
    /// </summary>
    private void CheckGameover()
    {
        if (gameoverFlg) return; // ���ł�GameOver�Ȃ珜�O
        if (gameoverHeight < gameObject.transform.position.y) return; // �����Ŕ��f
        ChangeGameoverFlg(true);
    }

    /// <summary>
    /// GameOver�������`�F�b�N����
    /// </summary>
    private void CheckDisappearHeight()
    {
        if (!gameoverFlg) return; // GameOver�łȂ��Ȃ珜�O
        if (disappearHeight < gameObject.transform.position.y) return; // �����Ŕ��f
        Destroy(gameObject);
    }

    /// <summary>
    /// myPosition���X�V����
    /// </summary>
    private void UpdateMyPosition()
    {
        myPosition = gameObject.transform.position;
    }

    // �W�����v�����Ƃ��ɌĂ�(�폜)
    //public void Jumping()
    //{
    //    ChangeJumpFlg(true);
    //}

    /// <summary>
    /// ���n�����Ƃ��A�����\�b�h�����M����
    /// </summary>
    /// <param name="notes">�炵�������̃m�[�c��</param>
    public void Landing()
    {
        if (!jumpFlg) return;

        ChangeJumpFlg(false);
    }

    /// <summary>
    /// �W�����v���Ă��邩�ǂ����̃t���O��؂�ւ���
    /// </summary>
    /// <param name="flg"></param>
    private void ChangeJumpFlg(bool flg)
    {
        if (jumpFlg == flg) return;

        jumpFlg = flg;

        //if (flg)
        //{
        //    Debug.Log($"Player.ChangeJumpFlg : Player Jump");
        //}
        //else
        //{
        //    Debug.Log($"Player.ChangeJumpFlg : Player Land");
        //}
    }

    /// <summary>
    /// GameOver���ǂ����̃t���O��؂�ւ���
    /// </summary>
    /// <param name="flg"></param>
    private void ChangeGameoverFlg(bool flg)
    {
        if (gameoverFlg == flg) return;

        gameoverFlg = flg;

        if (flg)
        {
            Debug.Log($"Player.ChangeGameoverFlg : Player GameOver");

            GameSceneManager.Instance.ReceptionGameOver();
        }
    }

    /// <summary>
    /// �W�����v���Ă��邩�ǂ����̃t���O��Ԃ��Q�b�^�[
    /// </summary>
    /// <returns>jumpFlg</returns>
    public bool GetJumpFlg()
    {
        return jumpFlg;
    }

    /// <summary>
    /// Player�̈ʒu��Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public Vector3 GetPlayerPosition()
    {
        return myPosition;
    }

    ///// <summary>
    ///// �O������̗͂�ݒ肷�郁�\�b�h
    ///// </summary>
    ///// <param name="force"></param>
    //public void AddForce(Vector3 force)
    //{
    //    externalForce = force;
    //}

}
