using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInformationManager : MonoBehaviour
{
    [SerializeField] private Sprite[] starSprite;

    /// <summary> �ϐ��̏��������������Ă��邩�̃t���O(false�Ȃ珀���ł��Ă��Ȃ�) </summary>
    private bool initReadyFlg = false;
    /// <summary> �Z�[�u�f�[�^���ǂݍ��߂Ă��邩�̃t���O(false�Ȃ珀���ł��Ă��Ȃ�) </summary>
    private bool saveReadyFlg = false;

    /// <summary> ���[�U�[�̐ݒ肵�����O </summary>
    private string playerName;
    /// <summary> ���O�C������ </summary>
    private int logInDays;
    /// <summary> �O��̃��O�C���� </summary>
    private DateTime lastLoginDay;
    /// <summary> SNS�Ŗ{���V�F�A�������̃t���O </summary>
    private bool todaySNSShareFlg;
    /// <summary> �����[�h�L�����l�������� </summary>
    private int rewardTimes;
    /// <summary> SNS�ŃV�F�A�������� </summary>
    private int shareDays;

    /// <summary> �ݒ肵�Ă���m�[�c�̕\���̎d���̎�� </summary>
    private DisplayNotesKind settingDisplayNotesKind;
    /// <summary> ���K���̃V�[����\�����邩�̃t���O </summary>
    private bool displayXylophoneStickerFlg;

    /// <summary> �ݒ肵���v���C���[�L�����N�^�[�̔ԍ� </summary>
    private int selectCharacterNumber;
    /// <summary> �ݒ肵���A�C�R���w�i�̔ԍ� </summary>
    private int selectIconBackgroundNumber;
    /// <summary> �ݒ肵���A�C�R���g�̔ԍ� </summary>
    private int selectIconFrameNumber;
    /// <summary> �uSP�R�C�������炤�v�őO���������Ƃ��̃X�^�[�� </summary>
    private int clearStarNumForObtainSpCoin;

    /// <summary> �Z�[�u�f�[�^�����[�h�����ϐ� </summary>
    private SaveData save;

    /// <summary> �V�K�v���C���[�̃t���O </summary>
    private bool newPlayerFlg;

    /// <summary> �X�e�[�W�N���A���[�h(Normal)�̃N���A�� </summary>
    [HideInInspector] public StageClearStarFlgs[] stageClearStarFlgsForNormal;
    /// <summary> �X�e�[�W�N���A���[�h(Hard)�̃N���A�� </summary>
    [HideInInspector] public StageClearStarFlgs[] stageClearStarFlgsForHard;
    /// <summary> �X�e�[�W�N���A���[�h(Veryhard)�̃N���A�� </summary>
    [HideInInspector] public StageClearStarFlgs[] stageClearStarFlgsForVeryHard;
    /// <summary> �����L���O���[�h�̎��g�̋L�^ </summary>
    [HideInInspector] public float[] rankingTimeRecord;
    /// <summary> �����A�C�e��(�L�����N�^�[)�̃t���O </summary>
    [HideInInspector] public int[] haveItemForCharacters;
    /// <summary> �����A�C�e��(�w�i)�̃t���O </summary>
    [HideInInspector] public int[] haveItemForBackgrounds;
    /// <summary> �����A�C�e��(�t���[��)�̃t���O </summary>
    [HideInInspector] public int[] haveItemForFlames;

    /// <summary> �X�e�[�W�N���A���[�h�̊l���X�^�[�̑��� (0:Normal , 1:Hard , 2:Veryhard) </summary>
    private int[] haveStarsForStageClearMode;

    /// <summary> �Q�[���V�[���ڍs�̍ۂɕK�v�ȏ�񂽂� </summary>
    private PlayGameInformation nextPlayGameInformation;

    /// <summary> �X�e�[�W�N���A���[�h�̃��X�g���쐬 </summary>
    private StageClearModeListGenerator stageClearModeListGenerator;
    /// <summary> StageClearModeListGenerator�̃t�@�C����(������) </summary>
    private const string NAME_OF_STAGE_CLEAR_MODE_LIST_GENERATOR = "StageClearModeListGenerators";

    /// <summary> �����L���O���[�h�̃��X�g���쐬 </summary>
    private RankingModeListGenerator rankingModeListGenerator;
    /// <summary> RankingModeListGenerator�̃t�@�C����(������) </summary>
    private const string NAME_OF_RANKING_MODE_LIST_GENERATOR = "RankingModeListGenerator";

    /// <summary> �L�����N�^�[�̃��X�g���쐬 </summary>
    private PlayerGenerator characterListGenerator;
    /// <summary> �L�����N�^�[���X�g�����̃t�@�C����(������) </summary>
    private const string NAME_OF_CHARACTER_LIST_GENERATOR = "CharactersGenerator";
    /// <summary> �A�C�R��(�w�i)���X�g���쐬 </summary>
    private IconItemListGenerator iconBackListGenerator;
    /// <summary> �A�C�R��(�w�i)���X�g�����̃t�@�C����(������) </summary>
    private const string NAME_OF_ICON_BACK_LIST_GENERATOR = "IconBackListGenerator";
    /// <summary> �A�C�R��(�g)���X�g���쐬 </summary>
    private IconItemListGenerator iconFrameListGenerator;
    /// <summary> �A�C�R��(�g)���X�g�����̃t�@�C����(������) </summary>
    private const string NAME_OF_ICON_FRAME_LIST_GENERATOR = "IconFrameListGenerator";

    /// <summary> PlayerPrefs�̃L�[(�\������m�[�c�̌���) </summary>
    private const string KEY_PLAYERPREFS_DISPLAY_NOTES_LANGUAGE = "KEY_PLAYERPREFS_DISPLAY_NOTES_LANGUAGE";
    /// <summary> PlayerPrefs�̃L�[(���Ղ̃V�[����\�����邩�̃t���O) </summary>
    private const string KEY_PLAYERPREFS_DISPLAY_STICKER_FLG = "KEY_PLAYERPREFS_DISPLAY_STICKER_FLG";

    /// <summary> StageClearStarFlgs�̃f�t�H���g�l </summary>
    public readonly StageClearStarFlgs DEFAULT_STAGE_CLEAR_STAR_FLGS = new StageClearStarFlgs(false, false, false, false, 0f);

    private readonly Color COLOR_NORMAL = new Color(1.0f, 1.0f, 0.0f);
    private readonly Color COLOR_HARD = new Color(0.0f, 1.0f, 0.0f);
    private readonly Color COLOR_VERYHARD = new Color(1.0f, 0.5f, 0.0f);
    private readonly Color COLOR_RANKING = new Color(1.0f, 0.0f, 1.0f);

    private const string SCENE_NAME_TITLE = "Title";
    private const string SCENE_NAME_MENU = "Menu";

    /// <summary>
    /// �Q�[���V�[���Ŏ�舵�����e
    /// </summary>
    public struct PlayGameInformation
    {
        /// <summary> �Q�[�����[�h </summary>
        public GameMode mode;
        /// <summary> �Q�[���̓�Փx </summary>
        public GameLevel level;
        /// <summary> �X�e�[�W�ԍ� </summary>
        public int stageNumber;
        /// <summary> �y���� </summary>
        public string sheetMusicName;
        /// <summary> �ڕW�^�C��(�����L���O���[�h���͎g�p���Ȃ�) </summary>
        public float targetTime;
        /// <summary> �L�����N�^�[�� </summary>
        public string playerCharacterName;
        /// <summary> ����{�X�L�b�v�@�\�̃t���O </summary>
        public bool skipForExampleFlg;
        /// <summary> �����L���O���[�h�̃L�[(�X�e�[�W�N���A���[�h���͎g�p���Ȃ�) </summary>
        public string rankingKey;
    }

    /// <summary>
    /// �K�`���A�C�e���̎��
    /// </summary>
    public enum GachaItemKind
    {
        None,
        Character,
        IconBackground,
        IconFrame,
    }

    /// <summary>
    /// �Q�[�����[�h
    /// </summary>
    public enum GameMode
    {
        None,
        StageClear,
        Ranking,
        FreeStyle,
    }

    /// <summary>
    /// �Q�[���̓�Փx(�ז��ȃI�u�W�F�N�g���o�Ă���)
    /// </summary>
    public enum GameLevel
    {
        None,
        Normal,
        Hard,
        VeryHard,
    }

    /// <summary>
    /// �m�[�c�̕\���̎d���̎��
    /// </summary>
    public enum DisplayNotesKind
    {
        None,
        KeyboardNumber,
        Japanese,
        English,
        //Deutsch,
    }


    public static PlayerInformationManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            // �V�[���J�ڂ��Ă��j������Ȃ��悤�ɂ���
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // ��d�ŋN������Ȃ��悤�ɂ���
            Destroy(gameObject);
        }

    }

    private void Start()
    {
        // �X�^�b�N�g���[�X��L���ɂ���NativeArray�̃��������[�N��T��
        NativeLeakDetection.Mode = NativeLeakDetectionMode.EnabledWithStackTrace;

        InitValues();
    }

    /// <summary>
    /// �S�Ă�ǂݍ���
    /// </summary>
    public void InitAndSettingPlayerInformation()
    {
        InitValues();
        PlayFabManager.Instance.GetUserSaveData();
    }

    /// <summary>
    /// �e�X�g���\�b�h(�f�o�b�O�p)
    /// </summary>
    /// <param name="num"></param>
    private void TestMethod(int num)
    {
        bool defaultFlg = false;
        switch (num)
        {
            case 1: // �f�[�^�̏�������
                //save.commonData.playerName = "Danchi";
                //save.commonData.logInDays = 3;
                //save.commonData.playerLevel = 12;
                //save.commonData.playerExperience = 26;
                //save.commonData.haveMedal = 1000;
                //save.commonData.haveSPCoin = 5;
                //save.dropBallsData.perfectClearTimes = 1;

                //save.profileData.myBackgroundNumber = 1;
                //save.profileData.myEyeVerticalPosition = 100;
                //save.profileData.myEyeHorizontalPosition = 50;

                //InitializeHavePartsForDefaultAll();

                //save.haveProfileParts.haveBackground[3] = true;

                //SaveJson();
                break;
            case 2: // �f�[�^�̓ǂݍ���
                //LoadJson();
                break;
            case 3: // �t�@�C�����폜
                //DeleteSaveFile();
                break;
            case 4: // �f�[�^��ǂݍ��񂾌�A�w�肵���f�[�^�̃C���N�������g
                //LoadJson();
                //IncrementField("dropBallsData", "perfectClearTimes");
                break;
            case 5: // ���O���o��
                //Debug.Log($"testMethod[5] : {save.haveProfileParts.haveBackground[0]}");
                //Debug.Log($"testMethod[5] : {save.haveProfileParts.haveBackground[1]}");
                //Debug.Log($"testMethod[5] : {save.haveProfileParts.haveBackground[2]}");
                //Debug.Log($"testMethod[5] : {save.haveProfileParts.haveBackground[3]}");
                break;
            default:
                defaultFlg = true;
                break;
        }

        if (!defaultFlg)
        {
            Debug.Log($"PlayerInformationManager.testMethod : num {num}");
        }

        Debug.Log("PlayerInformationManager.TestMethod : Json File ��\n" + JsonUtility.ToJson(save, true));
    }

    /// <summary>
    /// �ϐ��̏�����
    /// </summary>
    private void InitValues()
    {
        if (initReadyFlg) return;

        initReadyFlg = true;

        haveStarsForStageClearMode = new int[Enum.GetValues(typeof(GameLevel)).Length];
        stageClearModeListGenerator = transform.Find(NAME_OF_STAGE_CLEAR_MODE_LIST_GENERATOR)
            .gameObject.GetComponent<StageClearModeListGenerator>();

        rankingModeListGenerator = transform.Find(NAME_OF_RANKING_MODE_LIST_GENERATOR)
            .gameObject.GetComponent<RankingModeListGenerator>();

        characterListGenerator = transform.Find(NAME_OF_CHARACTER_LIST_GENERATOR)
            .gameObject.GetComponent<PlayerGenerator>();
        iconBackListGenerator = transform.Find(NAME_OF_ICON_BACK_LIST_GENERATOR)
            .gameObject.GetComponent<IconItemListGenerator>();
        iconFrameListGenerator = transform.Find(NAME_OF_ICON_FRAME_LIST_GENERATOR)
            .gameObject.GetComponent<IconItemListGenerator>();


        stageClearStarFlgsForNormal = new StageClearStarFlgs[stageClearModeListGenerator.GetListLength(GameLevel.Normal)];
        stageClearStarFlgsForHard = new StageClearStarFlgs[stageClearModeListGenerator.GetListLength(GameLevel.Hard)];
        stageClearStarFlgsForVeryHard = new StageClearStarFlgs[stageClearModeListGenerator.GetListLength(GameLevel.VeryHard)];
        rankingTimeRecord = new float[rankingModeListGenerator.GetLength()];
        haveItemForCharacters = new int[characterListGenerator.GetLength()]; 
        haveItemForBackgrounds = new int[iconBackListGenerator.GetLength()]; 
        haveItemForFlames = new int[iconFrameListGenerator.GetLength()];

        LoadPlayerPrefsForNotesLanguageAndStickerFlg();
    }

    /// <summary>
    /// json�t�@�C����ǂݍ��݁A�N���X�̕ϐ��ɒl��������
    /// </summary>
    public void InitLoad(SaveData saveFromPlayfab)
    {
        bool nullFlg = false;
        save = new SaveData();

        save.InitializeStageClearStarFlgsForNormal(GetStageClearModeListGenerator().GetListLength(GameLevel.Normal));
        save.InitializeStageClearStarFlgsForHard(GetStageClearModeListGenerator().GetListLength(GameLevel.Hard));
        save.InitializeStageClearStarFlgsForVeryHard(GetStageClearModeListGenerator().GetListLength(GameLevel.VeryHard));
        save.InitializeRankingTime(GetRankingModeListGenerator().GetLength());
        save.InitializeHaveCharacters(GetCharacterListGenerator().GetLength());
        save.InitializeHaveBackgrounds(GetIconBackListGenerator().GetLength());
        save.InitializeHaveFlames(GetIconFrameListGenerator().GetLength());

        //Debug.Log($"PlayerInformationManager.InitLoad : Length List\n" +
        //    $"save.stageClear.normal.Length = {save.stageClear.normal.Length}\n" +
        //    $"save.stageClear.hard.Length = {save.stageClear.hard.Length}\n" +
        //    $"save.stageClear.veryhard.Length = {save.stageClear.veryhard.Length}\n" +
        //    $"save.rankingTime.Length = {save.rankingTime.Length}\n" +
        //    $"save.haveItems.characters.Length = {save.haveItems.characters.Length}\n" +
        //    $"save.haveItems.backgrounds.Length = {save.haveItems.backgrounds.Length}\n" +
        //    $"save.haveItems.flames.Length = {save.haveItems.flames.Length}");

        if (saveFromPlayfab != null) 
        {
            save = saveFromPlayfab;

            playerName = save.commonData.playerName;
            logInDays = save.commonData.logInDays;
            todaySNSShareFlg = save.commonData.todaySNSShareFlg;
            rewardTimes = save.commonData.rewardTimes;
            shareDays = save.commonData.shareDays;
            if (DateTime.TryParse(save.commonData.lastLoginDay, out DateTime dateTime))
            {
                lastLoginDay = DateTime.Parse(save.commonData.lastLoginDay);
            }
            else
            {
                // �ϊ����s
                lastLoginDay = DateTime.MinValue;
            }
            selectCharacterNumber = save.commonData.selectCharacterNumber;
            selectIconBackgroundNumber = save.commonData.selectIconBackgroundNumber;
            selectIconFrameNumber = save.commonData.selectIconFrameNumber;
            clearStarNumForObtainSpCoin = save.commonData.clearStarNumForObtainSpCoin;

            //stageClearStarFlgsForNormal = new StageClearStarFlgs[stageClearModeListGenerator.GetListLength(GameLevel.Normal)];
            for (int i = 0; i < stageClearModeListGenerator.GetListLength(GameLevel.Normal); i++)
            {
                if (i < save.stageClear.normal.Length)
                {
                    stageClearStarFlgsForNormal[i] = save.stageClear.normal[i];
                }
                else
                {
                    stageClearStarFlgsForNormal[i] = DEFAULT_STAGE_CLEAR_STAR_FLGS;
                }
            }
            //stageClearStarFlgsForHard = new StageClearStarFlgs[stageClearModeListGenerator.GetListLength(GameLevel.Hard)];
            for (int i = 0; i < stageClearModeListGenerator.GetListLength(GameLevel.Hard); i++)
            {
                if (i < save.stageClear.hard.Length)
                {
                    stageClearStarFlgsForHard[i] = save.stageClear.hard[i];
                }
                else
                {
                    stageClearStarFlgsForHard[i] = DEFAULT_STAGE_CLEAR_STAR_FLGS;
                }
            }
            //stageClearStarFlgsForVeryHard = new StageClearStarFlgs[stageClearModeListGenerator.GetListLength(GameLevel.VeryHard)];
            for (int i = 0; i < stageClearModeListGenerator.GetListLength(GameLevel.VeryHard); i++)
            {
                if (i < save.stageClear.veryhard.Length)
                {
                    stageClearStarFlgsForVeryHard[i] = save.stageClear.veryhard[i];
                }
                else
                {
                    stageClearStarFlgsForVeryHard[i] = DEFAULT_STAGE_CLEAR_STAR_FLGS;
                }
            }

            //rankingTimeRecord = new float[save.rankingTime.Length]; // �T�C�Y�������L���O���[�h�̃X�e�[�W���ɂ���
            for (int i = 0; i < save.rankingTime.Length; i++)
            {
                rankingTimeRecord[i] = save.rankingTime[i];
            }
             
            //haveItemForCharacters = new bool[save.haveItems.characters.Length]; // �T�C�Y���L�������ɂ���
            for (int i = 0; i < save.haveItems.characters.Length; i++)
            {
                haveItemForCharacters[i] = save.haveItems.characters[i];
            }
            //haveItemForBackgrounds = new bool[save.haveItems.backgrounds.Length]; // �T�C�Y��w�i���ɂ���
            for (int i = 0; i < save.haveItems.backgrounds.Length; i++)
            {
                haveItemForBackgrounds[i] = save.haveItems.backgrounds[i];
            }
            //haveItemForFlames = new bool[save.haveItems.flames.Length]; // �T�C�Y���t���[�����ɂ���
            for (int i = 0; i < save.haveItems.flames.Length; i++)
            {
                haveItemForFlames[i] = save.haveItems.flames[i];
            }

            ArraySizeCheckAndChange();
        }
        else
        {
            nullFlg = true;
            Debug.Log($"PlayerInformationManager.InitLoad : Save is null");

            playerName = "";
            logInDays = 0;
            todaySNSShareFlg = false;
            rewardTimes = 0;
            shareDays = 0;
            lastLoginDay = DateTime.MinValue;
            selectCharacterNumber = 0;
            selectIconBackgroundNumber = 0;
            selectIconFrameNumber = 0;
            clearStarNumForObtainSpCoin = 0;

            haveItemForCharacters[0] = 1;
            haveItemForCharacters[1] = 1;
            haveItemForBackgrounds[0] = 1;
            haveItemForBackgrounds[1] = 1;
            haveItemForFlames[0] = 1;
            haveItemForFlames[1] = 1;

            save.commonData.playerName = playerName;
            save.commonData.logInDays = logInDays;
            save.commonData.todaySNSShareFlg = todaySNSShareFlg;
            save.commonData.rewardTimes = rewardTimes;
            save.commonData.shareDays = shareDays;
            save.commonData.lastLoginDay = lastLoginDay.ToString();
            save.commonData.selectCharacterNumber = selectCharacterNumber;
            save.commonData.selectIconBackgroundNumber = selectIconBackgroundNumber;
            save.commonData.selectIconFrameNumber = selectIconFrameNumber;
            save.commonData.clearStarNumForObtainSpCoin = clearStarNumForObtainSpCoin;

            save.haveItems.characters[0] = haveItemForCharacters[0];
            save.haveItems.characters[1] = haveItemForCharacters[1];
            save.haveItems.backgrounds[0] = haveItemForBackgrounds[0];
            save.haveItems.backgrounds[1] = haveItemForBackgrounds[1];
            save.haveItems.flames[0] = haveItemForFlames[0];
            save.haveItems.flames[1] = haveItemForFlames[1];

            PlayFabManager.Instance.UpdateUserDataForIcon();

            //InitHaveDefaultParts();
        }

        // test
        TestMethod(0);

        SaveJson();

        saveReadyFlg = true;

        if (SceneManager.GetActiveScene().name == SCENE_NAME_TITLE)
        {
            TitleSceneManager.Instance.AfterLoadSaveData(nullFlg);
        }
        else if (SceneManager.GetActiveScene().name == SCENE_NAME_MENU)
        {
            UpdateHaveStarsForStageClearMode();
        }
    }

    /// <summary>
    /// json�t�@�C����ۑ�����
    /// </summary>
    public void SaveJson()
    {
        PlayFabManager.Instance.UpdateUserDataForSave();

        Debug.Log($"PlayerInformationManager.SaveJson : Saved");
    }

    /// <summary>
    /// PlayFab����Save�f�[�^����M����
    /// </summary>
    /// <param name="saveData"></param>
    public void ReceptionSaveDataFromPlayFab(SaveData saveData)
    {
        InitLoad(saveData);
    }

    /// <summary>
    /// �z��̃T�C�Y�ɕύX���������ہA�T�C�Y�����T�C�Y����
    /// </summary>
    private void ArraySizeCheckAndChange()
    {
    //    Debug.Log($"ArraySizeCheckAndChange : Length :\n" +
    //$"save.stageClear.normal.Length -> {save.stageClear.normal.Length}\n" +
    //$"stageClearStarFlgsForNormal.Length -> {stageClearStarFlgsForNormal.Length}" +
    //$"");

        if (save.stageClear.normal.Length != stageClearStarFlgsForNormal.Length)
        {
            Debug.Log($"ArraySizeCheckAndChange : Resize StageClearNormal " +
                $"{save.stageClear.normal.Length} -> {stageClearStarFlgsForNormal.Length}");

            Array.Resize(ref save.stageClear.normal, stageClearStarFlgsForNormal.Length);

            for (int i = save.stageClear.normal.Length; i < stageClearStarFlgsForNormal.Length; i++)
            {
                //save.stageClear.normal[i].star1 = false;
                //save.stageClear.normal[i].star2 = false;
                //save.stageClear.normal[i].star3 = false;
                //save.stageClear.normal[i].starAll = false;
                //save.stageClear.normal[i].bestTime = 0f;
                save.stageClear.normal[i] = DEFAULT_STAGE_CLEAR_STAR_FLGS;
            }
            Array.Copy(save.stageClear.normal, stageClearStarFlgsForNormal, stageClearStarFlgsForNormal.Length);
        }

        if (save.stageClear.hard.Length != stageClearStarFlgsForHard.Length)
        {
            Debug.Log($"ArraySizeCheckAndChange : Resize StageClearHard " +
                $"{save.stageClear.hard.Length} -> {stageClearStarFlgsForHard.Length}");

            Array.Resize(ref save.stageClear.hard, stageClearStarFlgsForHard.Length);

            for (int i = save.stageClear.hard.Length; i < stageClearStarFlgsForHard.Length; i++)
            {
                //save.stageClear.hard[i].star1 = false;
                //save.stageClear.hard[i].star2 = false;
                //save.stageClear.hard[i].star3 = false;
                //save.stageClear.hard[i].starAll = false;
                //save.stageClear.hard[i].bestTime = 0f;
                save.stageClear.hard[i] = DEFAULT_STAGE_CLEAR_STAR_FLGS;
            }
            Array.Copy(save.stageClear.hard, stageClearStarFlgsForHard, stageClearStarFlgsForHard.Length);
        }

        if (save.stageClear.veryhard.Length != stageClearStarFlgsForVeryHard.Length)
        {
            Debug.Log($"ArraySizeCheckAndChange : Resize StageClearVeryhard " +
                $"{save.stageClear.veryhard.Length} -> {stageClearStarFlgsForVeryHard.Length}");

            Array.Resize(ref save.stageClear.veryhard, stageClearStarFlgsForVeryHard.Length);

            for (int i = save.stageClear.veryhard.Length; i < stageClearStarFlgsForVeryHard.Length; i++)
            {
                //save.stageClear.veryhard[i].star1 = false;
                //save.stageClear.veryhard[i].star2 = false;
                //save.stageClear.veryhard[i].star3 = false;
                //save.stageClear.veryhard[i].starAll = false;
                //save.stageClear.veryhard[i].bestTime = 0f;
                save.stageClear.veryhard[i] = DEFAULT_STAGE_CLEAR_STAR_FLGS;
            }
            Array.Copy(save.stageClear.veryhard, stageClearStarFlgsForVeryHard, stageClearStarFlgsForVeryHard.Length);
        }

        if (save.rankingTime.Length != rankingTimeRecord.Length)
        {
            Debug.Log($"ArraySizeCheckAndChange : Resize RankingTimeRecord " +
                $"{save.rankingTime.Length} -> {rankingTimeRecord.Length}");

            Array.Resize(ref save.rankingTime, rankingTimeRecord.Length);

            for (int i = save.rankingTime.Length; i < rankingTimeRecord.Length; i++)
            {
                save.rankingTime[i] = 0f;
            }
        }

        if (save.haveItems.characters.Length != haveItemForCharacters.Length)
        {
            Debug.Log($"ArraySizeCheckAndChange : Resize HaveItemForCharacters " +
                $"{save.haveItems.characters.Length} -> {haveItemForCharacters.Length}");

            Array.Resize(ref save.haveItems.characters, haveItemForCharacters.Length);

            for (int i = save.haveItems.characters.Length; i < haveItemForCharacters.Length; i++)
            {
                save.haveItems.characters[i] = 0;
            }
        }

        if (save.haveItems.backgrounds.Length != haveItemForBackgrounds.Length)
        {
            Debug.Log($"ArraySizeCheckAndChange : Resize HaveItemForBackgrounds " +
                $"{save.haveItems.backgrounds.Length} -> {haveItemForBackgrounds.Length}");

            Array.Resize(ref save.haveItems.backgrounds, haveItemForBackgrounds.Length);

            for (int i = save.haveItems.backgrounds.Length; i < haveItemForBackgrounds.Length; i++)
            {
                save.haveItems.backgrounds[i] = 0;
            }
        }

        if (save.haveItems.flames.Length != haveItemForFlames.Length)
        {
            Debug.Log($"ArraySizeCheckAndChange : Resize HaveItemForFlames " +
                $"{save.haveItems.flames.Length} -> {haveItemForFlames.Length}");

            Array.Resize(ref save.haveItems.flames, haveItemForFlames.Length);

            for (int i = save.haveItems.flames.Length; i < haveItemForFlames.Length; i++)
            {
                save.haveItems.flames[i] = 0;
            }
        }
    }

    /// <summary>
    /// �v���C���[����ύX���āAjson�t�@�C���ɕۑ�����
    /// </summary>
    /// <param name="newName"></param>
    public void SavePlayerName(string newName)
    {
        save.commonData.playerName = newName;
        playerName = newName;
        SaveJson();
        PlayFabManager.Instance.SetPlayerDisplayName(newName);
    }

    /// <summary>
    /// �Z�[�u�f�[�^�̏������ł��Ă��邩��Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public bool GetSaveDataReadyFlg()
    {
        return saveReadyFlg;
    }

    /// <summary>
    /// �v���C���[�̖��O��Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public string GetPlayerName()
    {
        return playerName;
    }

    /// <summary>
    /// �V�����v���C���[�ł��邩�̃t���O��Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public bool GetNewPlayerFlg()
    {
        return newPlayerFlg;
    }

    /// <summary>
    /// �ŏI���O�C������Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public DateTime GetLastLoginDay()
    {
        return lastLoginDay;
    }

    /// <summary>
    /// �{����SNS�V�F�A�ς݂��̃t���O��Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public bool GetTodaySNSShareFlg()
    {
        return todaySNSShareFlg;
    }

    /// <summary>
    /// ���O�C��������Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public int GetLoginDays()
    {
        return logInDays;
    }

    /// <summary>
    /// �uSP�R�C�������炤�v�őO���������Ƃ��̃X�^�[����Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public int GetClearStarNumForObtainSpCoin()
    {
        return clearStarNumForObtainSpCoin;
    }

    /// <summary>
    /// �ݒ肵�Ă���L�����N�^�[����Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public string GetPlayerCharacterName()
    {
        string name = characterListGenerator.GetCharacterName(selectCharacterNumber);

        return name;
    }

    /// <summary>
    /// �ݒ肵�Ă���L�����N�^�[�̃��X�g�ԍ���Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public int GetPlayerCharacterNumber()
    {
        return selectCharacterNumber;
    }

    /// <summary>
    /// �ݒ肵�Ă���A�C�R���w�i����Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public string GetSelectIconBackgroundName()
    {
        string name = iconBackListGenerator.GetName(selectIconBackgroundNumber);

        return name;
    }

    /// <summary>
    /// �ݒ肵�Ă���A�C�R���w�i�̃��X�g�ԍ���Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public int GetSelectIconBackgroundNumber()
    {
        return selectIconBackgroundNumber;
    }

    /// <summary>
    /// �ݒ肵�Ă���A�C�R���g����Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public string GetSelectIconFrameName()
    {
        string name = iconFrameListGenerator.GetName(selectIconFrameNumber);

        return name;
    }

    /// <summary>
    /// �ݒ肵�Ă���A�C�R���g�̃��X�g�ԍ���Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public int GetSelectIconFrameNumber()
    {
        return selectIconFrameNumber;
    }

    /// <summary>
    /// �w�肵���K�`���A�C�e�����擾���A���߂Ă̓���A�C�e������Ԃ�
    /// </summary>
    /// <param name="itemKinds">�A�C�e���̎��</param>
    /// <param name="itemNumber">�A�C�e���̗v�f�ԍ�</param>
    /// <returns>���߂ē���Ȃ�True</returns>
    public bool AcquisitionItemAndGetNewFlg(GachaItemKind itemKinds, int itemNumber)
    {
        bool errorFlg = false;
        bool newFlg = false;

        switch (itemKinds)
        {
            case GachaItemKind.Character:
                if (itemNumber > characterListGenerator.GetLength())
                {
                    errorFlg = true;
                }
                else if (itemNumber > save.haveItems.characters.Length)
                {
                    errorFlg = true;
                }
                haveItemForCharacters[itemNumber]++;
                save.haveItems.characters[itemNumber]++;
                newFlg = haveItemForCharacters[itemNumber] == 1 ? true : false;
                break;
            case GachaItemKind.IconBackground:
                if (itemNumber > GetIconBackListGenerator().GetLength())
                {
                    errorFlg = true;
                }
                else if (itemNumber > save.haveItems.backgrounds.Length)
                {
                    errorFlg = true;
                }
                haveItemForBackgrounds[itemNumber]++;
                save.haveItems.backgrounds[itemNumber]++;
                newFlg = haveItemForBackgrounds[itemNumber] == 1 ? true : false;
                break;
            case GachaItemKind.IconFrame:
                if (itemNumber > GetIconFrameListGenerator().GetLength())
                {
                    errorFlg = true;
                }
                else if (itemNumber > save.haveItems.flames.Length)
                {
                    errorFlg = true;
                }
                haveItemForFlames[itemNumber]++;
                save.haveItems.flames[itemNumber]++;
                newFlg = haveItemForFlames[itemNumber] == 1 ? true : false;
                break;
            default:
                errorFlg = true;
                break;
        }

        if (errorFlg)
        {
            Debug.LogError($"PlayerInformationManager.AcquisitionParts : Error");
        }
        else
        {
            //SaveJson();
        }

        return newFlg;
    }

    /// <summary>
    /// �L�����N�^�[�ƃA�C�R���ύX�Ƃ��̕ۑ�
    /// </summary>
    /// <param name="name"></param>
    public void UpdatePlayerCharacterAndIcon(int _characterNumber, int _backgroundNumber, int _frameNumber)
    {
        selectCharacterNumber = _characterNumber;
        save.commonData.selectCharacterNumber = _characterNumber;

        selectIconBackgroundNumber = _backgroundNumber;
        save.commonData.selectIconBackgroundNumber = _backgroundNumber;

        selectIconFrameNumber = _frameNumber;
        save.commonData.selectIconFrameNumber = _frameNumber;

        SaveJson();
    }

    /// <summary>
    /// �Q�[�����[�h�̓��e��Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public PlayGameInformation GetNextPlayGameInformation()
    {
        return nextPlayGameInformation;
    }

    /// <summary>
    /// �Q�[�����[�h�̓��e��ݒ肷��Z�b�^�[
    /// </summary>
    /// <param name="informations"></param>
    public void SetNextPlayGameInformation(PlayGameInformation informations)
    {
        nextPlayGameInformation = informations;
    }

    /// <summary>
    /// ���O�C���������X�V���A�ŏI���O�C�����������ɂ���
    /// </summary>
    public void UpdateLoginDays()
    {
        if (lastLoginDay == DateTime.Today) return;

        lastLoginDay = DateTime.Today;
        logInDays++;
        todaySNSShareFlg = false;

        save.commonData.lastLoginDay = lastLoginDay.ToString();
        save.commonData.logInDays = logInDays;
        save.commonData.todaySNSShareFlg = todaySNSShareFlg;

        Debug.Log($"UpdateLoginDays : Update Login Day\nlastLoginDay = {lastLoginDay} , logInDays = {logInDays}");

        //SaveJson();
    }

    /// <summary>
    /// �{����SNS�V�F�A���ς݂ɂ��A�X�V���ĕۑ�����
    /// </summary>
    public void UpdateAndSetTodaySNSShareFlgTrue()
    {
        todaySNSShareFlg = true;
        save.commonData.todaySNSShareFlg = todaySNSShareFlg;

        shareDays++;
        save.commonData.shareDays = shareDays;

        SaveJson();
    }

    /// <summary>
    /// Reward�L���������񐔂��C���N�������g���čX�V����
    /// </summary>
    public void UpdateRewardTimes()
    {
        rewardTimes++;
        save.commonData.rewardTimes = rewardTimes;
        SaveJson();
    }

    /// <summary>
    /// �X�e�[�W�N���A���[�h�̃N���A�󋵂��X�V���ăZ�[�u����
    /// </summary>
    public void UpdateSaveDataForStageClearMode()
    {
        //Debug.Log($"Length :\n" +
        //   $"stageClearStarFlgsForNormal -> {stageClearStarFlgsForNormal.Length}\n" +
        //   $"save.stageClear.normal -> {save.stageClear.normal.Length}" +
        //   $"");

        Array.Copy(stageClearStarFlgsForNormal, save.stageClear.normal, stageClearStarFlgsForNormal.Length);
        Array.Copy(stageClearStarFlgsForHard, save.stageClear.hard, stageClearStarFlgsForHard.Length);
        Array.Copy(stageClearStarFlgsForVeryHard, save.stageClear.veryhard, stageClearStarFlgsForVeryHard.Length);

        SaveJson();
    }

    /// <summary>
    /// �����L���O���[�h�̃X�R�A���X�V���ăZ�[�u����
    /// </summary>
    /// <param name="stageNumber"></param>
    /// <param name="score"></param>
    public void UpdateRankingRecordScore(int stageNumber,float score)
    {
        if (score < rankingTimeRecord[stageNumber] || rankingTimeRecord[stageNumber] <= 0f)
        {
            rankingTimeRecord[stageNumber] = score;
            save.rankingTime[stageNumber] = score;
            SaveJson();
        }
        else
        {
            Debug.LogError($"UpdateRankingRecordScore : Score Error!\n" +
                $"New Score = {score} , Old Score = {rankingTimeRecord[stageNumber]}");
        }
    }

    /// <summary>
    /// �uSP�R�C�������炤�v�őO���������Ƃ��̃X�^�[�����X�V���A�Z�[�u����
    /// </summary>
    public void UpdateClearStarNumAndObtainSpCoin()
    {
        int totalStar = GetTotalHaveStarsForStageClearMode();

        if (save.commonData.clearStarNumForObtainSpCoin < totalStar)
        {
            clearStarNumForObtainSpCoin = totalStar;
            save.commonData.clearStarNumForObtainSpCoin = totalStar;
            SaveJson();
        }
        else
        {
            Debug.LogError($"PlayerInformationManager.UpdateClearStarNumForObtainSpCoin : " +
                $"Error save.commonData.clearStarNumForObtainSpCoin >= totalStar\n" +
                $"{save.commonData.clearStarNumForObtainSpCoin} >= {totalStar}");

            return;
        }
    }

    /// <summary>
    /// �ݒ�L�����N�^�[���X�V���ĕۑ�����
    /// </summary>
    /// <param name="listNumber"></param>
    public void UpdateSelectCharacter(int listNumber)
    {
        selectCharacterNumber = listNumber;
        save.commonData.selectCharacterNumber = listNumber;
        //SaveJson();
    }

    /// <summary>
    /// �ݒ肵���A�C�R���w�i���X�V���ĕۑ�����
    /// </summary>
    /// <param name="listNumber"></param>
    public void UpdateSelectIconBackground(int listNumber)
    {
        selectIconBackgroundNumber = listNumber;
        save.commonData.selectIconBackgroundNumber = listNumber;
        //SaveJson();
    }

    /// <summary>
    /// �ݒ肵���A�C�R���g���X�V���ĕۑ�����
    /// </summary>
    /// <param name="listNumber"></param>
    public void UpdateSelectIconFrame(int listNumber)
    {
        selectIconFrameNumber = listNumber;
        save.commonData.selectIconFrameNumber = listNumber;
        //SaveJson();
    }

    /// <summary>
    /// �w�肵���N���A�X�^�[�̃X�v���C�g��Ԃ��Q�b�^�[
    /// </summary>
    /// <param name="number">0,1,2</param>
    /// <returns></returns>
    public Sprite GetStarSprite(int number)
    {
        if (0 <= number && number < starSprite.Length)
        {
            return starSprite[number];
        }
        else
        {
            Debug.LogError($"PlayerInformationManager.GetStarSprite : Number Error -> {number}");
            return null;
        }
    }

    /// <summary>
    /// �f�o�b�O�{�^�����������Ƃ�
    /// </summary>
    public void PushDebugButton()
    {
        TestMethod(1);
    }

    /// <summary>
    /// �X�^�[�l�������v�Z���čX�V����
    /// </summary>
    public void UpdateHaveStarsForStageClearMode()
    {
        //Debug.Log($"PlayerInformationManager.UpdateHaveStarsForStageClearMode : \n" +
        //    $"stageClearStarFlgsForNormal.Length = {stageClearStarFlgsForNormal.Length}");

        int counter = 0;
        //int testCount = -1;
        foreach (var hoge in stageClearStarFlgsForNormal)
        {
            //testCount++;
            //Debug.Log($"hoge[{testCount}] : hoge.star1 = {hoge.star1}");
            if (hoge.star1) counter++;
            if (hoge.star2) counter++;
            if (hoge.star3) counter++;
        }
        haveStarsForStageClearMode[0] = counter;

        counter = 0;
        foreach (var hoge in stageClearStarFlgsForHard)
        {
            if (hoge.star1) counter++;
            if (hoge.star2) counter++;
            if (hoge.star3) counter++;
        }
        haveStarsForStageClearMode[1] = counter;

        counter = 0;
        foreach (var hoge in stageClearStarFlgsForVeryHard)
        {
            if (hoge.star1) counter++;
            if (hoge.star2) counter++;
            if (hoge.star3) counter++;
        }
        haveStarsForStageClearMode[2] = counter;

        //Debug.Log($"PlayerInformationManager.UpdateHaveStarsForStageClearMode : Have Stars ��\n" +
        //    $"Normal = {haveStarsForStageClearMode[0]} , " +
        //    $"Hard  = {haveStarsForStageClearMode[1]} , " +
        //    $"VeryHard  = {haveStarsForStageClearMode[2]}");
    }

    /// <summary>
    /// ��Փx���Ƃ̊l���X�^�[�̑�����Ԃ��Q�b�^�[
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public int GetHaveStarsForStageClearMode(GameLevel level)
    {
        int index;
        switch (level)
        {
            case GameLevel.Normal: index = 0; break;
            case GameLevel.Hard: index = 1; break;
            case GameLevel.VeryHard: index = 2; break;
            default: Debug.LogError("MenuSceneManager.GetHaveStarsForStageClearMode : level Error"); return -1;
        }

        return haveStarsForStageClearMode[index];
    }

    /// <summary>
    /// �S��Փx�̊l���X�^�[�̑�����Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public int GetTotalHaveStarsForStageClearMode()
    {
        int totalStarNum = 0;

        foreach(int item in haveStarsForStageClearMode)
        {
            totalStarNum += item;
        }

        return totalStarNum;
    }

    /// <summary>
    /// �w�肵����Փx�̂��̃X�e�[�W���N���A�ς�(�X�^�[1���擾�ς�)����Ԃ��Q�b�^�[
    /// </summary>
    /// <param name="level"></param>
    /// <param name="stageNumber"></param>
    /// <returns></returns>
    public bool GetAlreadyStageClearFlg(GameLevel level,int stageNumber)
    {
        bool flg = false;
        switch (level)
        {
            case GameLevel.Normal: flg = stageClearStarFlgsForNormal[stageNumber].star1; break;
            case GameLevel.Hard: flg = stageClearStarFlgsForHard[stageNumber].star1; break;
            case GameLevel.VeryHard: flg = stageClearStarFlgsForVeryHard[stageNumber].star1; break;
            default: Debug.LogError("MenuSceneManager.GetAlreadyStageClearFlg : level Error"); return false;
        }

        return flg;
    }

    /// <summary>
    /// StageClearModeListGenerator��Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public StageClearModeListGenerator GetStageClearModeListGenerator()
    {
        return stageClearModeListGenerator;
    }

    /// <summary>
    /// RankingModeListGenerator��Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public RankingModeListGenerator GetRankingModeListGenerator()
    {
        return rankingModeListGenerator;
    }

    /// <summary>
    /// �L�����N�^�[���X�g��Generator��Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public PlayerGenerator GetCharacterListGenerator()
    {
        return characterListGenerator;
    }

    /// <summary>
    /// �A�C�R��(�w�i)��Generator��Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public IconItemListGenerator GetIconBackListGenerator()
    {
        return iconBackListGenerator;
    }

    /// <summary>
    /// �A�C�R��(�g)��Generator��Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public IconItemListGenerator GetIconFrameListGenerator()
    {
        return iconFrameListGenerator;
    }

    /// <summary>
    /// ��Փx�̃J���[��Ԃ��Q�b�^�[
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public Color GetLevelColor(GameLevel level)
    {
        Color col;
        if (level == GameLevel.Normal)
        {
            col = COLOR_NORMAL;
        }
        else if (level == GameLevel.Hard)
        {
            col = COLOR_HARD;
        }
        else if (level == GameLevel.VeryHard)
        {
            col = COLOR_VERYHARD;
        }
        else
        {
            Debug.LogError($"MenuSceneManager.GetLevelColor : Level Error {level}");
            col = Color.white;
        }
        return col;
    }

    /// <summary>
    /// �����L���O���[�h�̃J���[��Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public Color GetRankingModeColor()
    {
        return COLOR_RANKING;
    }

    /// <summary>
    /// save�̃Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public SaveData GetSaveData()
    {
        return save;
    }

    /// <summary>
    /// �ݒ肵�Ă��鉹�K���̌����Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public DisplayNotesKind GetSettingDisplayNotesKind()
    {
        return settingDisplayNotesKind;
    }

    /// <summary>
    /// ���ՃV�[����\�����邩�̃t���O��Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public bool GetDisplayXylophoneStickerFlg()
    {
        return displayXylophoneStickerFlg;
    }

    /// <summary>
    /// �m�[�c�\���̓��{�ꕶ�����Ԃ�
    /// </summary>
    /// <param name="kind"></param>
    /// <returns></returns>
    public string GetDisplayNotesKindToJapaneseString(DisplayNotesKind kind)
    {
        string str;
        switch (kind)
        {
            case DisplayNotesKind.KeyboardNumber:
                str = "�s�A�m�̌��Քԍ�";
                break;
            case DisplayNotesKind.Japanese:
                str = "���{��(�h���~�c)";
                break;
            case DisplayNotesKind.English:
                str = "�C�M���X��(CDE�c)";
                break;
            default:
                str = "";
                break;
        }

        return str;
    }

    /// <summary>
    /// �m�[�c�̌���ƌ��ՃV�[���̕\�����Ǘ�����l��PlayerPrefs�ɕۑ�����
    /// </summary>
    /// <param name="kind">�\�����錾��</param>
    /// <param name="flg">true : �\������</param>
    public void SavePlayerPrefsForNotesLanguageAndStickerFlg(DisplayNotesKind kind , bool flg)
    {
        settingDisplayNotesKind = kind;
        displayXylophoneStickerFlg = flg;
        string str = kind.ToString();
        int flgInt = flg ? 1 : 0;
        PlayerPrefs.SetString(KEY_PLAYERPREFS_DISPLAY_STICKER_FLG, str);
        PlayerPrefs.SetInt(KEY_PLAYERPREFS_DISPLAY_STICKER_FLG, flgInt);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// �m�[�c�̌���ƌ��ՃV�[���̕\�����Ǘ�����l��PlayerPrefs����ǂݍ���
    /// </summary>
    public void LoadPlayerPrefsForNotesLanguageAndStickerFlg()
    {
        string str = PlayerPrefs.GetString(KEY_PLAYERPREFS_DISPLAY_STICKER_FLG, "Japanese");
        settingDisplayNotesKind = Enum.TryParse(str, out DisplayNotesKind auth) ? auth : DisplayNotesKind.Japanese;
        displayXylophoneStickerFlg = PlayerPrefs.GetInt(KEY_PLAYERPREFS_DISPLAY_STICKER_FLG, 1) == 1 ? true : false;
    }


}
