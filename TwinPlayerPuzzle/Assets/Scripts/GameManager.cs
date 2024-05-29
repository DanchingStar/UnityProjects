using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : ManagerParent 
{
    public Player1 myPlayer1;
    public Player2 myPlayer2;

    public GameMenu myMenuManager;

    public GameObject gameOverCanvas;
    public GameObject gameClearCanvas;

    public Camera mainCamera;
    public GameObject anotherCamera;

    public Button nextStageButton;
    public TextMeshProUGUI troubleTextObject;

    Vector3 startCameraMaker, endCameraMaker; //�J�����̃X�^�[�g�n�_�ƃS�[���n�_
    Quaternion startCameraQuaternion, endCameraQuaternion;//�J�����̃X�^�[�g�p�x�ƃS�[���p�x

    float presentCameraLocation = 0f; //���ݒn������[ 0 <= x <= 1 ]

    bool firstFlag = true; //�J�����p�t���O

    const float gameOverDelayTime = 0.7f; //�Q�[���I�[�o�[��ʂ��o���܂ł̎���
    const float gameClearDelayTime = 2.0f; //�Q�[���N���A��ʂ��o���܂ł̎���
    float countTime = 0f; //�ݒ肵�����Ԃ܂ł̎��Ԃ��Ǘ�

    bool gameOverFlag = false; //�Q�[���I�[�o�[���m�肵����true
    bool gameClearFlag = false; //�Q�[���N���A���m�肵����true

    bool clearMissSEFlag = true; //�N���A�܂��̓~�X�̂Ƃ��Ɉ�x����SE���Đ�����邽�߂̃t���O

    int nowStage = 0; //���݂̃X�e�[�W�����擾
    int highStage = 0; //�ō����B�X�e�[�W�����擾

    int countTrouble = 0; //�萔���J�E���g
    bool countTroubleFlag = false; //�萔���J�E���g���邽�߂̃t���O

    // Start is called before the first frame update
    void Start()
    {
        if (myBanner)
        {
            myBanner.RequestBanner(); //�o�i�[�L�����Ăяo��
        }

        OnSoundPlay(SoundManager.BGM_Type.GameBGM);

        startCameraMaker = mainCamera.transform.position;
        endCameraMaker = startCameraMaker;

        startCameraQuaternion = mainCamera.transform.rotation;
        endCameraQuaternion = startCameraQuaternion;

        presentCameraLocation = 0f;

        nowStage = int.Parse(Regex.Replace(SceneManager.GetActiveScene().name, @"[^0-9]", ""));
        highStage = PlayerPrefs.GetInt("ClearStage", 0);

        //Debug.Log($"nowStage : {nowStage}");
        //Debug.Log($"highStage : {highStage}");

        if (nowStage >= maxStage) 
        {
            nextStageButton.interactable = false; //�{�^���𖳌���
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (myPlayer1.WalkToHole() == 1) //���Ɍ������ĕ����Ă���Ƃ�
        {
            MoveCamera(myPlayer1.transform.position, mainCamera);
        }
        else if (myPlayer1.WalkToHole() == 2) //���ɓ��B�����Ƃ�
        {
            GameOverDisplay();
        }
        else if (myPlayer2.WalkToHole() == 1) //���Ɍ������ĕ����Ă���Ƃ�
        {
            MoveCamera(myPlayer2.transform.position, mainCamera);
        }
        else if (myPlayer2.WalkToHole() == 2) //���ɓ��B�����Ƃ�
        {
            GameOverDisplay();
        }
        else if(gameClearFlag == true) //�Q�[���N���A�����Ƃ�
        {
            GameClearDisplay();
            MoveCamera(myPlayer1.transform.position, mainCamera);
            //if(presentCameraLocation >= 1.0f)
            //{
            //    anotherCamera.SetActive(true);
            //}
        }

        if (gameOverFlag == false && myPlayer1.WalkToHole() >= 1) //Player1��������̊m�肵���Ƃ�
        {
            gameOverFlag = true;
        }
        else if (gameOverFlag == false && myPlayer2.WalkToHole() >= 1) //Player2��������̊m�肵���Ƃ�
        {
            gameOverFlag = true;
        }

        if (gameClearFlag == false && myPlayer1.GoalCheck() == true && myPlayer2.GoalCheck() == true)  //�Q�[���N���A���m�肵���Ƃ�
        {
            gameClearFlag = true;
            if (nowStage >= highStage) //���߂Ă��̃X�e�[�W���N���A�����Ƃ�
            {
                PlayerPrefs.SetInt("ClearStage", nowStage);
                PlayerPrefs.Save();
            }
        }

        CountTrouble(); //�萔���J�E���g
        troubleTextObject.text = "�萔 : " + countTrouble.ToString(); //�萔��\������e�L�X�g��ύX
    }


    void MoveCamera(Vector3 maker , Camera camera)
    {
        if (firstFlag == true) //���Ɍ����������ɏ���ɌĂ΂�A�J�����̖ړI�n��ݒ肷��
        {
            endCameraMaker = new Vector3(maker.x, 6f, maker.z - 5f); //�J�����̖ړI�n�̐ݒ�
            endCameraQuaternion = Quaternion.Euler(45f,0f,0f); //�J�����̊p�x�ύX�̐ݒ�
            firstFlag = false;
        }
        else //�ݒ肵���J�����̖ړI�n�Ɍ������Ċ��
        {
            presentCameraLocation += Time.deltaTime * 1.1f;   // ���݂̃J�����̈ʒu
            camera.transform.position = Vector3.Lerp(startCameraMaker, endCameraMaker, presentCameraLocation);// �J�����I�u�W�F�N�g�̈ړ�
            camera.transform.rotation = Quaternion.Lerp(startCameraQuaternion, endCameraQuaternion, presentCameraLocation);// �J�����̉�]
        }
    }

    void GameOverDisplay() //�Q�[���I�[�o�[�̉�ʂ��o���֐�
    {
        if (countTime < gameOverDelayTime)
        {
            countTime += Time.deltaTime;
        }
        else
        {
            if (clearMissSEFlag) //��x����SE���Đ�����
            {
                OnSoundPlay(SoundManager.SE_Type.Miss);
                clearMissSEFlag = false;
            }
            gameOverCanvas.SetActive(true);
        }

    }
    void GameClearDisplay() //�Q�[���N���A�̉�ʂ��o���֐�
    {
        if (countTime < gameClearDelayTime)
        {
            countTime += Time.deltaTime;
        }
        else
        {
            if (clearMissSEFlag) //��x����SE���Đ�����
            {
                OnSoundPlay(SoundManager.SE_Type.Clear);
                clearMissSEFlag = false;
            }
            gameClearCanvas.SetActive(true);
            anotherCamera.SetActive(true);
        }

    }


    void CountTrouble() //�萔���J�E���g���邽�߂̊֐�
    {
        if (myPlayer1.inputManagerFlag == true || myPlayer2.inputManagerFlag == true)
        {
            countTroubleFlag = true;
        }
        else
        {
            if (countTroubleFlag == true && (myPlayer1.IsGetKeyOK() == false || myPlayer2.IsGetKeyOK() == false)) 
            {
                countTrouble++;
                countTroubleFlag = false;
                OnSoundPlay(SoundManager.SE_Type.Walk);
            }
        }
    }


    public void PushNextStage() //�N���A���NexrStage���N���b�N�����Ƃ�
    {
        OnSoundPlay(SoundManager.SE_Type.Yes);

        int n = nowStage + 1;
        string text = "Game" + n;
        ChangeScene(text,30); //30%�ŃC���^�[�X�e�B�V�����L�����Ăяo���āA�V�[���ύX
    }

    public void ClickSNS() //SNS�{�^�����N���b�N�����Ƃ��ɌĂ΂��֐�
    {
        string url = "";
        string image_path = "";
        string text = "";

        string aaa = "�X�e�[�W";
        string stage = nowStage.ToString();
        string bbb = "��";
        string score = countTrouble.ToString();
        string ccc = "��ŃN���A!!\n�����A���Ȃ�������Ă݂悤(^^)/\n#�^�������Z�� #UKK ";

        text = aaa + stage + bbb + score + ccc;

        if (Application.platform == RuntimePlatform.Android)
        {
            url = "https://play.google.com/store/apps/details?id=com.DanchingStar";
            image_path = Application.persistentDataPath + "/SS.png";
            text = text + "#Android\n";
            Debug.Log("Android");
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            url = "https://www.google.com";
            image_path = Application.persistentDataPath + "/SS.png";
            text = text + "#iPhone\n";
            Debug.Log("iPhone");
        }
        else
        {
            url = "https://www.google.com";
            image_path = Application.persistentDataPath + "/SS.png";
            //text = text + "���̑��̋@��\n";
            Debug.Log("Other OS");
        }

        //SocialConnector.SocialConnector.Share(text); //��1����:�e�L�X�g,��2����:URL,��3����:�摜
        SocialConnector.SocialConnector.Share(text, url); //��1����:�e�L�X�g,��2����:URL,��3����:�摜
        //Debug.Log("SNS");
    }


    public bool PauseCheck() //���݃|�[�Y�����`�F�b�N����֐�
    {
        if (myMenuManager.MenuPauseCheck() == true)
        {
            return true;
        }

        return false;
    }
    public bool GameOverCheck() //GameOver���m�肵�����`�F�b�N����֐�
    {
        return gameOverFlag;
    }
    public bool GameClearCheck() //Game�N���A���m�肵�����`�F�b�N����֐�
    {
        return gameClearFlag;
    }

    public int NowStageCheck() //���݂̃X�e�[�W����Ԃ��֐�
    {
        return nowStage;
    }

    public int GetMaxStage() //maxStage��Ԃ������̊֐�
    {
        return maxStage;
    }
    public bool GetDebugMuteMode() //debugMuteMode��Ԃ������̊֐�
    {
        return debugMuteMode;
    }



}
