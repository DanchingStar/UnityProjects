using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DiceController : MonoBehaviour
{
    [SerializeField] private Material[] diceMaterials;
    [SerializeField] private Material[] eyeMaterials;
    [SerializeField] private Sprite[] eyeSprites;
    [SerializeField] private Transform[] diceParentTransforms;

    [SerializeField] private GameObject dicePrefab;

    private List<DiceEyeKinds> finalEyesList;

    private MyDice[] settingDices;

    private Yaku myYaku;

    private bool standByFlg;
    private bool rollingFlg;
    private List<DiceMain> diceList;

    private const string MENU_SCENE_NAME = "Menu";
    private const string GAME_SCENE_NAME = "Game";

    private const int DICE_COUNT = 3;
    private const string DEFAULT_DICE_NAME = "���ʂ̃T�C�R��";

#region Awake�E�C���X�^���X

    public static DiceController Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

#endregion

    public enum DiceEyeKinds
    {
        None,
        Eye1,
        Eye2,
        Eye3,
        Eye4,
        Eye5,
        Eye6,
    }

    public enum Yaku
    {
        None,
        Hifumi,
        Menashi,
        Deme1,
        Deme2,
        Deme3,
        Deme4,
        Deme5,
        Deme6,
        Shigoro,
        Arashi2,
        Arashi3,
        Arashi4,
        Arashi5,
        Arashi6,
        Arashi1,
    }

    private void Start()
    {
        diceList = new List<DiceMain>();
        finalEyesList = new List<DiceEyeKinds>();
        settingDices = new MyDice[DICE_COUNT];

        standByFlg = false;
        rollingFlg = false;

        myYaku = Yaku.None;

        if (SceneManager.GetActiveScene().name == GAME_SCENE_NAME)
        {
            GameSceneUIController.Instance.ChangeYakuText("�X�^���o�C�҂�");

            ReceptionCustomDicePanelPrefab(-1);
        }

    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == GAME_SCENE_NAME)
        {
            RollingCheck();
            StandByRolling();
        }
    }

    /// <summary>
    /// �T�C�R��������Ă��邩���ׁA�~�܂��Ă�������𒲂ׂ�
    /// </summary>
    private void RollingCheck()
    {
        if (!rollingFlg) return;

        foreach (DiceMain dice in diceList)
        {
            if (!dice.GetStopFlg()) return;
        }

        // �����ɓ��B������A�S���̃T�C�R�����~�܂��Ă���

        rollingFlg = false;

        foreach (DiceMain dice in diceList)
        {
            finalEyesList.Add(dice.GetMyFinalEye());
        }

        CheckYakuForPlay();
    }

    /// <summary>
    /// �X�^���o�C���ɃT�C�R������]���鉉�o
    /// </summary>
    private void StandByRolling()
    {
        if (standByFlg)
        {
            foreach (var dice in diceParentTransforms)
            {
                dice.transform.Rotate(new Vector3(-1f, 0.6f, 1f));
            }
        }
    }

    /// <summary>
    /// �����`�F�b�N����(������)
    /// </summary>
    private void CheckYakuForPlay()
    {
        myYaku = CheckYaku(finalEyesList);

        if (myYaku == Yaku.None)
        {
            Debug.LogError($"CheckYaku : Error {finalEyesList[0]},{finalEyesList[1]},{finalEyesList[2]}");
        }
        else
        {
            GameSceneUIController.Instance.ChangeYakuText(myYaku);
            //Debug.Log($"CheckYaku : {(int)finalEyesList[0]},{(int)finalEyesList[1]},{(int)finalEyesList[2]} -> {myYaku}");
        }
    }

    /// <summary>
    /// [�e�X�g]�X�^���o�C�{�^�����󂯎�����Ƃ�
    /// </summary>
    /// <param name="number"></param>
    public void Test_ReceptionStandByButton(int number)
    {
        foreach (var dice in diceList)
        {
            dice.DeleteMyObject();
        }
        diceList.Clear();
        finalEyesList.Clear();

        for (int i = 0; i < DICE_COUNT; i++) 
        {
            DiceMain newDice = Instantiate(dicePrefab, diceParentTransforms[i]).GetComponent<DiceMain>();
            newDice.SetMyStatus(i);
            diceList.Add(newDice);
        }

        switch (number)
        {
            case 1: // test
                for (int i = 0; i < DICE_COUNT; i++)
                {
                    diceList[i].ChangeEyes((DiceEyeKinds)(i + 1), DiceEyeKinds.Eye6);
                }
                break;
            case 2: // �V�S����
                for (int i = 0; i < DICE_COUNT; i++)
                {
                    diceList[i].ChangeEyes(DiceEyeKinds.Eye1, DiceEyeKinds.Eye6);
                    diceList[i].ChangeEyes(DiceEyeKinds.Eye2, DiceEyeKinds.Eye5);
                    diceList[i].ChangeEyes(DiceEyeKinds.Eye3, DiceEyeKinds.Eye4);
                }
                break;
            case 3: // �s���]����
                for (int i = 0; i < DICE_COUNT; i++)
                {
                    diceList[i].ChangeEyes(DiceEyeKinds.Eye2, DiceEyeKinds.Eye1);
                    diceList[i].ChangeEyes(DiceEyeKinds.Eye3, DiceEyeKinds.Eye1);
                    diceList[i].ChangeEyes(DiceEyeKinds.Eye4, DiceEyeKinds.Eye1);
                    diceList[i].ChangeEyes(DiceEyeKinds.Eye5, DiceEyeKinds.Eye1);
                    diceList[i].ChangeEyes(DiceEyeKinds.Eye6, DiceEyeKinds.Eye1);
                }
                break;
            default: 
                break;
        }

        standByFlg = true;
        rollingFlg = false;
        myYaku = Yaku.None;

        GameSceneUIController.Instance.ChangeYakuText("�΂�U��!!", Color.black);
    }

    /// <summary>
    /// �X�^���o�C�{�^�����󂯎�����Ƃ�
    /// </summary>
    public void ReceptionStandByButton()
    {
        foreach (var dice in diceList)
        {
            dice.DeleteMyObject();
        }
        diceList.Clear();
        finalEyesList.Clear();

        for (int i = 0; i < DICE_COUNT; i++)
        {
            DiceMain newDice = Instantiate(dicePrefab, diceParentTransforms[i]).GetComponent<DiceMain>();
            newDice.SetMyStatus(i);
            diceList.Add(newDice);
        }

        for (int i = 0; i < DICE_COUNT; i++)
        {
            if (settingDices[i] == null) 
            {

            }
            else
            {
                for (int j = 0; j < System.Enum.GetValues(typeof(DiceEyeKinds)).Length; j++)
                {
                    if (j != 0)
                    {
                        if (settingDices[i].changeEyes[j] != DiceEyeKinds.None)
                        {
                            diceList[i].ChangeEyes((DiceEyeKinds)j, settingDices[i].changeEyes[j]);
                        }
                    }
                }
            }
        }

        standByFlg = true;
        rollingFlg = false;
        myYaku = Yaku.None;

        GameSceneUIController.Instance.ChangeYakuText("�΂�U��!!", Color.black);
    }

    /// <summary>
    /// �΂�U��{�^�����󂯎�����Ƃ�
    /// </summary>
    /// <returns>true : �T�C�R����U�����Ƃ�</returns>
    public bool ReceptionFireButton()
    {
        if (!standByFlg) return false;

        GameSceneUIController.Instance.ChangeYakuText("");

        standByFlg = false;
        rollingFlg = true;

        foreach (var dice in diceList)
        {
            dice.Fire();
        }
        return true;
    }

    /// <summary>
    /// CustomDicePanelPrefab����ݒ肷��}�C�Z�b�g�̔ԍ����󂯎��
    /// </summary>
    /// <param name="_mysetNumber"></param>
    public void ReceptionCustomDicePanelPrefab(int _mysetNumber)
    {
        if (_mysetNumber == -1)
        {
            for (int i = 0; i < DICE_COUNT; i++)
            {
                settingDices[i] = null;
            }
        }
        else
        {
            for (int i = 0; i < DICE_COUNT; i++)
            {
                settingDices[i] = SaveDataManager.Instance.GetMyDice(_mysetNumber);
            }
        }

        ReceptionStandByButton();
    }

    /// <summary>
    /// �V�~�����[�V�������s���󂯎�����Ƃ�
    /// </summary>
    /// <param name="_diceNumber">�Ώۂ̃T�C�R��</param>
    /// <returns></returns>
    public DiceEyeKinds ReceptionSimulation(int _diceNumber)
    {
        if (_diceNumber < diceMaterials.Length && _diceNumber >= 0)
        {
            int randNum = Random.Range(0, 6) + 1;
            return diceList[_diceNumber].GetEyeForSimulation((DiceEyeKinds)randNum);
        }
        else
        {
            Debug.LogWarning($"DiceController.ReceptionSimulation : Return Null , DiceNumber = {_diceNumber}");
            return DiceEyeKinds.None;
        }
    }

    /// <summary>
    /// �󂯎�������X�g���琬�����Ă������Ԃ�(�V�~�����[�V�����p)
    /// </summary>
    /// <param name="_listEyes"></param>
    /// <returns></returns>
    public Yaku CheckYakuForSimulation(List<DiceEyeKinds> _listEyes)
    {
        return CheckYaku(_listEyes);
    }

    /// <summary>
    /// �������𔻒肷��
    /// </summary>
    /// <param name="_listEyes"></param>
    /// <returns></returns>
    private Yaku CheckYaku(List<DiceEyeKinds> _listEyes)
    {
        // �Ԃ�l
        Yaku resultYaku;

        // �𔻒�p�̃��X�g
        List<DiceEyeKinds> sortList = new List<DiceEyeKinds>(_listEyes);

        // �𔻒�̂��߂ɎႢ���Ń\�[�g
        sortList.Sort();

        if (sortList.Count != DICE_COUNT)
        {
            resultYaku = Yaku.None;
        }
        else if(sortList[0] == DiceEyeKinds.None)
        {
            resultYaku = Yaku.None;
        }
        else if (sortList[0] == sortList[1] && sortList[1] == sortList[2])
        {
            switch (sortList[0])
            {
                case DiceEyeKinds.Eye1: resultYaku = Yaku.Arashi1; break;
                case DiceEyeKinds.Eye2: resultYaku = Yaku.Arashi2; break;
                case DiceEyeKinds.Eye3: resultYaku = Yaku.Arashi3; break;
                case DiceEyeKinds.Eye4: resultYaku = Yaku.Arashi4; break;
                case DiceEyeKinds.Eye5: resultYaku = Yaku.Arashi5; break;
                case DiceEyeKinds.Eye6: resultYaku = Yaku.Arashi6; break;
                default: resultYaku = Yaku.None; break;
            }
        }
        else if (sortList[0] != sortList[1] && sortList[1] == sortList[2])
        {
            switch (sortList[0])
            {
                case DiceEyeKinds.Eye1: resultYaku = Yaku.Deme1; break;
                case DiceEyeKinds.Eye2: resultYaku = Yaku.Deme2; break;
                case DiceEyeKinds.Eye3: resultYaku = Yaku.Deme3; break;
                case DiceEyeKinds.Eye4: resultYaku = Yaku.Deme4; break;
                case DiceEyeKinds.Eye5: resultYaku = Yaku.Deme5; break;
                case DiceEyeKinds.Eye6: resultYaku = Yaku.Deme6; break;
                default: resultYaku = Yaku.None; break;
            }
        }
        else if (sortList[0] == sortList[1] && sortList[1] != sortList[2])
        {
            switch (sortList[2])
            {
                case DiceEyeKinds.Eye1: resultYaku = Yaku.Deme1; break;
                case DiceEyeKinds.Eye2: resultYaku = Yaku.Deme2; break;
                case DiceEyeKinds.Eye3: resultYaku = Yaku.Deme3; break;
                case DiceEyeKinds.Eye4: resultYaku = Yaku.Deme4; break;
                case DiceEyeKinds.Eye5: resultYaku = Yaku.Deme5; break;
                case DiceEyeKinds.Eye6: resultYaku = Yaku.Deme6; break;
                default: resultYaku = Yaku.None; break;
            }
        }
        else if (sortList[0] == DiceEyeKinds.Eye1 && sortList[1] == DiceEyeKinds.Eye2 && sortList[2] == DiceEyeKinds.Eye3)
        {
            resultYaku = Yaku.Hifumi;
        }
        else if (sortList[0] == DiceEyeKinds.Eye4 && sortList[1] == DiceEyeKinds.Eye5 && sortList[2] == DiceEyeKinds.Eye6)
        {
            resultYaku = Yaku.Shigoro;
        }
        else
        {
            resultYaku = Yaku.Menashi;
        }

        return resultYaku;
    }

    /// <summary>
    /// �T�C�R���̃}�e���A����Ԃ��Q�b�^�[
    /// </summary>
    /// <param name="_diceNumber"></param>
    /// <returns></returns>
    public Material GetDiceMaterial(int _diceNumber)
    {
        if (_diceNumber < diceMaterials.Length && _diceNumber >= 0)
        {
            return diceMaterials[_diceNumber];
        }
        else
        {
            Debug.LogWarning($"DiceController.GetDiceMaterial : Return Null , DiceNumber = {_diceNumber}");
            return null;
        } 
    }

    /// <summary>
    /// �T�C�R���̖ڂ̃}�e���A����Ԃ��Q�b�^�[
    /// </summary>
    /// <param name="eye"></param>
    /// <returns></returns>
    public Material GetEyeMaterial(DiceEyeKinds eye)
    {
        int _eyeNumber = (int)eye - 1;
        if (_eyeNumber < eyeMaterials.Length && _eyeNumber >= 0)
        {
            return eyeMaterials[_eyeNumber];
        }
        else
        {
            Debug.LogWarning($"DiceController.GetEyeMaterial : Return Null , Eye = {eye}");
            return null;
        }
    }

    /// <summary>
    /// �T�C�R���̖ڂ̃X�v���C�g��Ԃ��Q�b�^�[
    /// </summary>
    /// <param name="eye"></param>
    /// <returns></returns>
    public Sprite GetEyeSprite(DiceEyeKinds eye)
    {
        int _eyeNumber = (int)eye - 1;
        if (_eyeNumber < eyeSprites.Length && _eyeNumber >= 0)
        {
            return eyeSprites[_eyeNumber];
        }
        else
        {
            //Debug.LogWarning($"DiceController.GetEyeSprite : Return Null , Eye = {eye}");
            return null;
        }
    }

    /// <summary>
    /// �X�^���o�C�����̃t���O��Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public bool GetStandByFlg()
    {
        return standByFlg;
    }

    /// <summary>
    /// �T�C�R������]�����̃t���O��Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public bool GetRollingFlg()
    {
        return rollingFlg;
    }

    /// <summary>
    /// �T�C�R���̐���Ԃ�(��T3��Ԃ�)
    /// </summary>
    /// <returns></returns>
    public int GetDiceCount()
    {
        return DICE_COUNT;
    }

    /// <summary>
    /// ��(���{��)��Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public string GetYakuName(Yaku yaku)
    {
        string resultStr;
        switch (yaku)
        {
            case Yaku.Hifumi: resultStr="�q�t�~"; break;
            case Yaku.Menashi: resultStr = "�ږ���"; break;
            case Yaku.Deme1: resultStr = "�o��1"; break;
            case Yaku.Deme2: resultStr = "�o��2"; break;
            case Yaku.Deme3: resultStr = "�o��3"; break;
            case Yaku.Deme4: resultStr = "�o��4"; break;
            case Yaku.Deme5: resultStr = "�o��5"; break;
            case Yaku.Deme6: resultStr = "�o��6"; break;
            case Yaku.Shigoro: resultStr = "�V�S��"; break;
            case Yaku.Arashi2: resultStr = "�A���V2"; break;
            case Yaku.Arashi3: resultStr = "�A���V3"; break;
            case Yaku.Arashi4: resultStr = "�A���V4"; break;
            case Yaku.Arashi5: resultStr = "�A���V5"; break;
            case Yaku.Arashi6: resultStr = "�A���V6"; break;
            case Yaku.Arashi1: resultStr = "�A���V1"; break;
            default: resultStr = ""; break;
        }

        return resultStr;
    }

    /// <summary>
    /// �f�t�H���g(�ʏ�)�̃T�C�R���̖��O��Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public string GetDefaultDiceName()
    {
        return DEFAULT_DICE_NAME;
    }

}
