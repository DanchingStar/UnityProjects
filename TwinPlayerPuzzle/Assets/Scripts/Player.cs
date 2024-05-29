using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject[] walls; //�ǂ̈ʒu���擾�p 
    public GameObject[] holes; //���̈ʒu���擾�p 
    public GameObject[] reverses; //�t�}�X�̈ʒu���擾�p 
    public GameObject[] goals; //�S�[���̈ʒu���擾�p 

    public GameManager myGameManager;
    public InputManager myInputManager;

    protected Animator animator; //�A�j���[�^�[
    protected const int walk = 1, idle = 5; //�A�j���[�V����"legs"�̃p�����[�^
    protected Vector3 startMaker, endMaker; //�����X�^�[�g�n�_�ƃS�[���n�_
    protected float presentLocation = 0f; //���ݒn������[ 0 <= x <= 1 ]
    protected bool isGetKeyOK = true; //�L�[���󂯕t���邩���肷��

    protected const float speed = 1.0F; //�����X�s�[�h
    protected const int floorSizeX = 9; //���̃T�C�Y
    protected const int floorSizeZ = 9;
    protected const int zeroPositionX = 4; //���W0���z��̉��Ԗڂ�
    protected const int zeroPositionZ = 4;
    protected const int neutral = 0, wall = 1, hole = 2, reverse = 3, goal = 9; //wallHoleCheck�p�̒萔
    protected int[,] wallHoleCheck = new int[floorSizeX, floorSizeZ]; //�ǂ��������邩���ׂ邽�߂̔z��

    protected int walkToHole = 0; // 0:������Ȃ��Ƃ���,1:���Ɍ������Ă���,2;���ɓ��B
    protected bool reverseFlag = false; //�t�}�X�̃t���O
    protected bool goalFlag = false; //�S�[���̃t���O
    protected float goalActionTime = 0f; //�S�[��������̃|�[�Y���Ǘ�����

    [System.NonSerialized] public bool inputManagerFlag = false; //InputManager�Ƃ���肷�邽�߂̃t���O
    protected bool waitFlag = false; //1�t���[�����͂��󂯕t���Ȃ����߂̃t���O

    protected bool keyInputFlag = false; //�L�[�����͂��ꂽ�Ƃ��̊m�F�t���O
    protected int xDistance = 0, zDistance = 0; //���̓L�[���Ƃɐi�ނׂ��������Z

    protected void PositionStatusReset() //�X�^�[�g�ʒu�ƃG���h�ʒu�����Z�b�g����
    {
        animator = GetComponent<Animator>();
        animator.SetInteger("legs", idle);//�A�j���[�V����[�����~�܂�]
        animator.SetInteger("arms", idle);//�A�j���[�V����[�����~�܂�]

        startMaker = new Vector3(this.transform.localPosition.x, 0.5f, this.transform.localPosition.z);
        endMaker = startMaker;
        presentLocation = 0f;
    }


    protected void WalkToHoleAction() //���֌������Ƃ��̏����ƃA�N�V����
    {
        walkToHole = 1; //���Ɍ������Ă���t���O
        isGetKeyOK = false;//�L�[���͂��󂯕t���Ȃ��悤�ɂ���
        animator.SetInteger("arms", 12);//�A�j���[�V����[����]
    }

    protected void DoStart()
    {
        PositionStatusReset();

        for (int x = 0; x < floorSizeX; x++)  //wallHoleCheck�̏�����
        {
            for (int z = 0; z < floorSizeZ; z++)
            {
                wallHoleCheck[x, z] = neutral;
            }
        }

        if (walls != null)
        {
            foreach (var i in walls) //�ǂ�����ꏊ��wallHoleCheck���o����
            {
                int x = (int)i.transform.localPosition.x;
                int z = (int)i.transform.localPosition.z;

                wallHoleCheck[x + zeroPositionX, z + zeroPositionZ] = wall;
            }
        }
        if (holes != null)
        {
            foreach (var i in holes) //��������ꏊ��wallHoleCheck���o����
            {
                int x = (int)i.transform.localPosition.x;
                int z = (int)i.transform.localPosition.z;

                wallHoleCheck[x + zeroPositionX, z + zeroPositionZ] = hole;
            }
        }
        if (reverses != null)
        {
            foreach (var i in reverses) //�t�}�X������ꏊ��wallHoleCheck���o����
            {
                int x = (int)i.transform.localPosition.x;
                int z = (int)i.transform.localPosition.z;

                wallHoleCheck[x + zeroPositionX, z + zeroPositionZ] = reverse;
            }
        }
        foreach (var i in goals) //�S�[��������ꏊ��wallHoleCheck���o����
        {
            int x = (int)i.transform.localPosition.x;
            int z = (int)i.transform.localPosition.z;

            wallHoleCheck[x + zeroPositionX, z + zeroPositionZ] = goal;
        }
    }

    protected void DoIsGetKeyTrue()
    {

        if (myInputManager.CheckInputUp())  //�L�[���󂯕t���Ă�Ƃ��ɏ�������ꂽ�ꍇ
        {
            keyInputFlag = true;
            xDistance = 0;
            zDistance = 1;
            this.transform.rotation = Quaternion.Euler(0f, 90f, 0f);//�������
        }
        else if (myInputManager.CheckInputDown())  //�L�[���󂯕t���Ă�Ƃ��ɉ��������ꂽ�ꍇ
        {
            keyInputFlag = true;
            xDistance = 0;
            zDistance = -1;
            this.transform.rotation = Quaternion.Euler(0f, -90f, 0f);//��������
        }
        else if (myInputManager.CheckInputRight())  //�L�[���󂯕t���Ă�Ƃ��ɉE�������ꂽ�ꍇ
        {
            keyInputFlag = true;
            xDistance = 1;
            zDistance = 0;
            this.transform.rotation = Quaternion.Euler(0f, 180f, 0f);//�E������
        }
        else if (myInputManager.CheckInputLeft())  //�L�[���󂯕t���Ă�Ƃ��ɍ��������ꂽ�ꍇ

        {
            keyInputFlag = true;
            xDistance = -1;
            zDistance = 0;
            this.transform.rotation = Quaternion.Euler(0f, 0f, 0f);//��������
        }
        else
        {
            keyInputFlag = false;
            xDistance = 0;
            zDistance = 0;
        }

        if (keyInputFlag)
        {
            if(reverseFlag) // �t�}�X�ɂ���ꍇ
            {
                xDistance *= -1;
                zDistance *= -1;
            }

            if ((int)startMaker.z + zeroPositionZ + zDistance >= floorSizeZ || (int)startMaker.z + zeroPositionZ + zDistance < 0
                || (int)startMaker.x + zeroPositionX + xDistance >= floorSizeX || (int)startMaker.x + zeroPositionX + xDistance < 0) //�g�O�֕������ꍇ
            {
                WalkToHoleAction(); //���֕��������̏���
                endMaker = new Vector3(startMaker.x + xDistance, startMaker.y, startMaker.z + zDistance); //�ړI�n�̐ݒ�
                reverseFlag = false;
            }
            else if (wallHoleCheck[(int)startMaker.x + zeroPositionX + xDistance, (int)startMaker.z + zeroPositionZ + zDistance] == hole) //���֕������ꍇ
            {
                WalkToHoleAction(); //���֕��������̏���
                endMaker = new Vector3(startMaker.x + xDistance, startMaker.y, startMaker.z + zDistance); //�ړI�n�̐ݒ�
                reverseFlag = false;
            }
            else if (wallHoleCheck[(int)startMaker.x + zeroPositionX + xDistance, (int)startMaker.z + zeroPositionZ + zDistance] == neutral) //���ʂ̓��֕������ꍇ
            {
                isGetKeyOK = false;//�L�[���͂��󂯕t���Ȃ��悤�ɂ���
                endMaker = new Vector3(startMaker.x + xDistance, startMaker.y, startMaker.z + zDistance); //�ړI�n�̐ݒ�
                goalFlag = false;
                reverseFlag = false;
            }
            else if (wallHoleCheck[(int)startMaker.x + zeroPositionX + xDistance, (int)startMaker.z + zeroPositionZ + zDistance] == reverse) //�t�}�X�֕������ꍇ
            {
                isGetKeyOK = false;//�L�[���͂��󂯕t���Ȃ��悤�ɂ���
                endMaker = new Vector3(startMaker.x + xDistance, startMaker.y, startMaker.z + zDistance); //�ړI�n�̐ݒ�
                goalFlag = false;
                reverseFlag = true;
            }
            else if (wallHoleCheck[(int)startMaker.x + zeroPositionX + xDistance, (int)startMaker.z + zeroPositionZ + zDistance] == wall) //�ǂ֕������Ƃ����ꍇ
            {
                //Debug.Log("wall");
            }
            else if (wallHoleCheck[(int)startMaker.x + zeroPositionX + xDistance, (int)startMaker.z + zeroPositionZ + zDistance] == goal) //�S�[���֕������Ƃ����ꍇ
            {
                isGetKeyOK = false;//�L�[���͂��󂯕t���Ȃ��悤�ɂ���
                endMaker = new Vector3(startMaker.x + xDistance, startMaker.y, startMaker.z + zDistance); //�ړI�n�̐ݒ�
                goalFlag = true;
                reverseFlag = false;
            }
            inputManagerFlag = false;
        }


        //�ȉ��Â�����
        /********************************************************************************************************************************************************
        //�L�[���󂯕t���Ă�Ƃ��ɏ�������ꂽ�ꍇ
        if (myInputManager.CheckInputUp())
        {
            this.transform.rotation = Quaternion.Euler(0f, 90f, 0f);//�������
            if ((int)startMaker.z + zeroPositionZ + 1 >= floorSizeZ) //�g�O�֕������ꍇ
            {
                WalkToHoleAction(); //���֕��������̏���
                endMaker = new Vector3(startMaker.x, startMaker.y, startMaker.z + 1.0f); //�ړI�n�̐ݒ�
            }
            else if (wallHoleCheck[(int)startMaker.x + zeroPositionX, (int)startMaker.z + zeroPositionZ + 1] == hole) //���֕������ꍇ
            {
                WalkToHoleAction(); //���֕��������̏���
                endMaker = new Vector3(startMaker.x, startMaker.y, startMaker.z + 1.0f); //�ړI�n�̐ݒ�
            }
            else if (wallHoleCheck[(int)startMaker.x + zeroPositionX, (int)startMaker.z + zeroPositionZ + 1] == neutral) //���ʂ̓��֕������ꍇ
            {
                isGetKeyOK = false;//�L�[���͂��󂯕t���Ȃ��悤�ɂ���
                endMaker = new Vector3(startMaker.x, startMaker.y, startMaker.z + 1.0f); //�ړI�n�̐ݒ�
                goalFlag = false;
            }
            else if (wallHoleCheck[(int)startMaker.x + zeroPositionX, (int)startMaker.z + zeroPositionZ + 1] == wall) //�ǂ֕������Ƃ����ꍇ
            {
                //Debug.Log("wall ��");
            }
            else if (wallHoleCheck[(int)startMaker.x + zeroPositionX, (int)startMaker.z + zeroPositionZ + 1] == goal) //�S�[���֕������Ƃ����ꍇ
            {
                isGetKeyOK = false;//�L�[���͂��󂯕t���Ȃ��悤�ɂ���
                endMaker = new Vector3(startMaker.x, startMaker.y, startMaker.z + 1.0f); //�ړI�n�̐ݒ�
                goalFlag = true;
            }
            inputManagerFlag = false;
        }
        //�L�[���󂯕t���Ă�Ƃ��ɉ��������ꂽ�ꍇ
        else if (myInputManager.CheckInputDown())
        {
            this.transform.rotation = Quaternion.Euler(0f, -90f, 0f);//��������
            if ((int)startMaker.z + zeroPositionZ - 1 < 0) //�g�O�֕������ꍇ
            {
                WalkToHoleAction(); //���֕��������̏���
                endMaker = new Vector3(startMaker.x, startMaker.y, startMaker.z - 1.0f); //�ړI�n�̐ݒ�
            }
            else if (wallHoleCheck[(int)startMaker.x + zeroPositionX, (int)startMaker.z + zeroPositionZ - 1] == hole) //���֕������ꍇ
            {
                WalkToHoleAction(); //���֕��������̏���
                endMaker = new Vector3(startMaker.x, startMaker.y, startMaker.z - 1.0f); //�ړI�n�̐ݒ�
            }
            else if (wallHoleCheck[(int)startMaker.x + zeroPositionX, (int)startMaker.z + zeroPositionZ - 1] == neutral) //���ʂ̓��֕������ꍇ
            {
                isGetKeyOK = false;//�L�[���͂��󂯕t���Ȃ��悤�ɂ���
                endMaker = new Vector3(startMaker.x, startMaker.y, startMaker.z - 1.0f); //�ړI�n�̐ݒ�
                goalFlag = false;
            }
            else if (wallHoleCheck[(int)startMaker.x + zeroPositionX, (int)startMaker.z + zeroPositionZ - 1] == wall) //�ǂ֕������Ƃ����ꍇ
            {
                //Debug.Log("wall ��");
            }
            else if (wallHoleCheck[(int)startMaker.x + zeroPositionX, (int)startMaker.z + zeroPositionZ - 1] == goal) //�S�[���֕������Ƃ����ꍇ
            {
                isGetKeyOK = false;//�L�[���͂��󂯕t���Ȃ��悤�ɂ���
                endMaker = new Vector3(startMaker.x, startMaker.y, startMaker.z - 1.0f); //�ړI�n�̐ݒ�
                goalFlag = true;
            }
            inputManagerFlag = false;
        }
        //�L�[���󂯕t���Ă�Ƃ��ɉE�������ꂽ�ꍇ
        else if (myInputManager.CheckInputRight())
        {
            this.transform.rotation = Quaternion.Euler(0f, 180f, 0f);//�E������
            if ((int)startMaker.x + zeroPositionX + 1 >= floorSizeX) //�g�O�֕������ꍇ
            {
                WalkToHoleAction(); //���֕��������̏���
                endMaker = new Vector3(startMaker.x + 1.0f, startMaker.y, startMaker.z); //�ړI�n�̐ݒ�
            }
            else if (wallHoleCheck[(int)startMaker.x + zeroPositionX + 1, (int)startMaker.z + zeroPositionZ] == hole) //���֕������ꍇ
            {
                WalkToHoleAction(); //���֕��������̏���
                endMaker = new Vector3(startMaker.x + 1.0f, startMaker.y, startMaker.z); //�ړI�n�̐ݒ�
            }
            else if (wallHoleCheck[(int)startMaker.x + zeroPositionX + 1, (int)startMaker.z + zeroPositionZ] == neutral) //���ʂ̓��֕������ꍇ
            {
                isGetKeyOK = false;//�L�[���͂��󂯕t���Ȃ��悤�ɂ���
                endMaker = new Vector3(startMaker.x + 1.0f, startMaker.y, startMaker.z); //�ړI�n�̐ݒ�
                goalFlag = false;
            }
            else if (wallHoleCheck[(int)startMaker.x + zeroPositionX + 1, (int)startMaker.z + zeroPositionZ] == wall) //�ǂ֕������Ƃ����ꍇ
            {
                //Debug.Log("wall ��");
            }
            else if (wallHoleCheck[(int)startMaker.x + zeroPositionX + 1, (int)startMaker.z + zeroPositionZ] == goal) //�S�[���֕������Ƃ����ꍇ
            {
                isGetKeyOK = false;//�L�[���͂��󂯕t���Ȃ��悤�ɂ���
                endMaker = new Vector3(startMaker.x + 1.0f, startMaker.y, startMaker.z); //�ړI�n�̐ݒ�
                goalFlag = true;
            }
            inputManagerFlag = false;
        }
        //�L�[���󂯕t���Ă�Ƃ��ɍ��������ꂽ�ꍇ
        else if (myInputManager.CheckInputLeft())
        {
            this.transform.rotation = Quaternion.Euler(0f, 0f, 0f);//��������
            if ((int)startMaker.x + zeroPositionX - 1 < 0) //�g�O�֕������ꍇ
            {
                WalkToHoleAction(); //���֕��������̏���
                endMaker = new Vector3(startMaker.x - 1.0f, startMaker.y, startMaker.z); //�ړI�n�̐ݒ�
            }
            else if (wallHoleCheck[(int)startMaker.x + zeroPositionX - 1, (int)startMaker.z + zeroPositionZ] == hole) //���֕������ꍇ
            {
                WalkToHoleAction(); //���֕��������̏���
                endMaker = new Vector3(startMaker.x - 1.0f, startMaker.y, startMaker.z); //�ړI�n�̐ݒ�
            }
            else if (wallHoleCheck[(int)startMaker.x + zeroPositionX - 1, (int)startMaker.z + zeroPositionZ] == neutral) //���ʂ̓��֕������ꍇ
            {
                isGetKeyOK = false;//�L�[���͂��󂯕t���Ȃ��悤�ɂ���
                endMaker = new Vector3(startMaker.x - 1.0f, startMaker.y, startMaker.z); //�ړI�n�̐ݒ�
                goalFlag = false;
            }
            else if (wallHoleCheck[(int)startMaker.x + zeroPositionX - 1, (int)startMaker.z + zeroPositionZ] == wall) //�ǂ֕������Ƃ����ꍇ
            {
                //Debug.Log("wall ��");
            }
            else if (wallHoleCheck[(int)startMaker.x + zeroPositionX - 1, (int)startMaker.z + zeroPositionZ] == goal) //�S�[���֕������Ƃ����ꍇ
            {
                isGetKeyOK = false;//�L�[���͂��󂯕t���Ȃ��悤�ɂ���
                endMaker = new Vector3(startMaker.x - 1.0f, startMaker.y, startMaker.z); //�ړI�n�̐ݒ�
                goalFlag = true;
            }
            inputManagerFlag = false;
        }
        ********************************************************************************************************************************************************/
    }

    protected void DoIsGetKeyFalse()
    {
        presentLocation += Time.deltaTime * speed;   // ���݂̈ʒu
        animator.SetInteger("legs", walk); //�A�j���[�V����[����]
        animator.SetInteger("arms", walk); //�A�j���[�V����[����]
        transform.localPosition = Vector3.Lerp(startMaker, endMaker, presentLocation);// �I�u�W�F�N�g�̈ړ�

        if (presentLocation >= 1f) //�ڕW�n�_�ɓ��B�����Ƃ�
        {
            isGetKeyOK = true;//�L�[���͂��󂯕t����悤�ɂ���
            PositionStatusReset();//���ݒn��ݒ肵�Ȃ���

            if (walkToHole == 1) //���ɓ��B�����ꍇ
            {
                walkToHole = 2; //���ɓ��B�t���O
                isGetKeyOK = false; //�L�[���͂��󂯕t���Ȃ��悤�ɂ���
                endMaker = new Vector3(startMaker.x, startMaker.y - 100f, startMaker.z); //�ړI�n(����)�̐ݒ�
            }
        }
    }

    protected void FallToAbyss() //�ޗ��ւƗ����Ă����Ƃ�
    {
        if(presentLocation <= 0f)
        {
            myGameManager.OnSoundPlay(SoundManager.SE_Type.Fall);
        }
        presentLocation += Time.deltaTime * speed * 0.1f;   // ���݂̈ʒu
        animator.SetInteger("arms", 17); //�A�j���[�V����[������グ��]
        transform.localPosition = Vector3.Lerp(startMaker, endMaker, presentLocation);// �I�u�W�F�N�g�̈ړ�
    }
    protected void GameClearAction() //�Q�[���N���A�����Ƃ��̃A�N�V����
    {
        goalActionTime += Time.deltaTime;

        if (goalActionTime < 1f)
        {
            animator.SetInteger("arms", 17); //�A�j���[�V����[������グ��]
        }
        else
        {
            animator.SetInteger("arms", idle); //�A�j���[�V����[�����������]
        }

        if(goalActionTime >= 2f)
        {
            goalActionTime -= 2f;
        }
    }

    public int WalkToHole() //���֌������Ă����Ԃ�Ԃ������̊֐�
    {
        return walkToHole;
    }
    public bool GoalCheck() //�S�[���n�_�𓥂�ł��邩�Ԃ������̊֐�
    {
        return goalFlag ;
    }

    public bool IsGetKeyOK() //���͂��󂯕t���Ă��邩�Ԃ������̊֐�
    {
        return isGetKeyOK;
    }
}
