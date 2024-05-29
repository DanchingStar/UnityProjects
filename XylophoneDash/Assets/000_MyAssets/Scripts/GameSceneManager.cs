using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Text.RegularExpressions;

public class GameSceneManager : MonoBehaviour
{
    /// <summary> UIManager </summary>
    [SerializeField] private UIManager uIManager;
    /// <summary> Player�֘A�̐e�I�u�W�F�N�g </summary>
    [SerializeField] private GameObject playerObjects;
    /// <summary> Canvas��Transform </summary>
    [SerializeField] private Transform canvasTransform;
    /// <summary> �V�ѕ�Prefab </summary>
    [SerializeField] private GameObject howToPlayPrefab;

    [Space(10)]
    [Header("[Test Status]")]

    /// <summary> ����{���X�L�b�v���邩�̃t���O(true�ŃX�L�b�v) </summary>
    [SerializeField] private bool skipForExampleFlg;
    /// <summary> �X�e�[�W�̓�Փx </summary>
    [SerializeField] private PlayerInformationManager.GameLevel stageLevel;
    /// <summary> �v���C���[�L�����̖��O </summary>
    [SerializeField] private string playerCharactorName;
    /// <summary> �y���̖��O </summary>
    [SerializeField] private string sheetMusicName;
    /// <summary> �X�e�[�WBGM </summary>
    [SerializeField] private AudioClip stageBgmClip;

    /// <summary> PlayerInformationManager������������Ă������̃Q�[���̏�� </summary>
    private PlayerInformationManager.PlayGameInformation myGameInformation;

    /// <summary> �y���̃W�F�l���[�^�[ </summary>
    private SheetMusicListGenerator sheetMusicListGenerator;
    /// <summary> ��_�J���� </summary>
    private CinemachineVirtualCamera fixedPointCamera;
    /// <summary> �v���C���[����̃J����(�Q�[���I�[�o�[�̂Ƃ��Ɏg��) </summary>
    private CinemachineVirtualCamera playerTopCamera;
    /// <summary> �v���C���[��ǂ��J���� </summary>
    private CinemachineVirtualCamera playerFollowCamera;

    private ParticleSystem kamifubuki;

    /// <summary> �v���C���[�L������Transform </summary>
    private Transform playerArmatureTransform;
    /// <summary> �v���C����y���̃f�[�^ </summary>
    private SheetMusic sheetMusic;

    /// <summary> �\�����鉹�K�̎�� </summary>
    private PlayerInformationManager.DisplayNotesKind displayNotesKind;

    /// <summary> ���ʉ��t�̐i��(�m�[�c�����B�ŃN���A) </summary>
    private int progress;
    /// <summary> �v���C���Ă��錻�݂̃^�C�� </summary>
    private float playTimer;
    /// <summary> �~�X������ </summary>
    private int missCount;
    /// <summary> �X�^�[�g�O�̃J�E���g�_�E���̐��� </summary>
    private int countDownValue;

    /// <summary> ����{�Ō�����Ƃ��ɕς���J���[ </summary>
    private readonly Color COLOR_EXAMPLE = Color.yellow;

    /// <summary> ��_�J�����̃t���O </summary>
    private bool fixedPointCameraFlg;

    /// <summary> ���݂̃t�F�[�Y </summary>
    private NowPhase nowPhase;

    /// <summary> �|�[�Y�����̃t���O </summary>
    private bool pauseFlg;

    /// <summary> ����{�̃R���[�`��(Skip�Ɏg��) </summary>
    private Coroutine forExampleCoroutine;


    /// <summary>
    /// �ǂݍ��񂾊y�����o����\����
    /// </summary>
    public struct SheetMusic
    {
        /// <summary> �^�C�g�� </summary>
        public string title;
        /// <summary> ���q�̕��q </summary>
        public float hyoushiBunshi;
        /// <summary> ���q�̕��� </summary>
        public float hyoushiBunbo;
        /// <summary> �e���| </summary>
        public float tempo;
        /// <summary> �m�[�c�� </summary>
        public int notesNum;
        /// <summary> �������� </summary>
        public List<SingleNote> notes;
    }

    /// <summary>
    /// �����ЂƂ��Ƃ̏ڍ�
    /// </summary>
    public struct SingleNote
    {
        /// <summary> ���Ֆ� </summary>
        public XylophoneManager.Notes keyBoardName;
        /// <summary> �Z�Z�������ȂǁA�Z�Z�̐��� </summary>
        public float bu;

        public SingleNote(XylophoneManager.Notes keyBoardName, float bu)
        {
            this.keyBoardName = keyBoardName;
            this.bu = bu;
        }
    }

    /// <summary>
    /// ���݂̃t�F�[�Y
    /// </summary>
    public enum NowPhase
    {
        Start,
        HowToPlay,
        ForExample,
        StandBy,
        Play,
        GameOver,
        GameClear,
    }

    public static GameSceneManager Instance;
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

    private void Start()
    {
        LoadPlayGameInformation();

        InitVariable();
        InitChildrenComponent();
        InitPlayerPrefab();
        InitSheetMusic();
        InitSounds();

        ChangeFixedPointCamera(false);
        ChangeDisplayNotesKind();

        InitUI();

        SwitchOperatePlayerEnable(false);

        DecidePhaseForStart();
    }

    private void Update()
    {
        UpdateTimer();
    }

    /// <summary>
    /// �ϐ��̏�����
    /// </summary>
    private void InitVariable()
    {
        ChangeNowPhase(NowPhase.Start, 0f);
        playTimer = 0f;
        missCount = 0;
        countDownValue = 100;
    }

    /// <summary>
    /// �q�v�f�̃R���|�[�l���g���������߂̏�����
    /// </summary>
    private void InitChildrenComponent()
    {
        sheetMusicListGenerator = transform.Find("SheetMusicListGenerator").GetComponent<SheetMusicListGenerator>();

        fixedPointCamera = transform.Find("FixedPointCamera").GetComponent<CinemachineVirtualCamera>();
        playerTopCamera = transform.Find("PlayerTopCamera").GetComponent<CinemachineVirtualCamera>();
        playerFollowCamera = playerObjects.transform.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();

        kamifubuki = transform.Find("KamifubukiParticle").GetComponent<ParticleSystem>();
        DisplayKamifubuki(false);
    }

    /// <summary>
    /// �v���C���[Prefab�̏�����
    /// </summary>
    private void InitPlayerPrefab()
    {
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) // ���ł�Player�����݂��Ă���Ƃ�
        {
            playerArmatureTransform = playerObj.transform;

            Debug.LogError($"GameSceneManager.InitPlayerPrefab : Already Exist PlayerArmature ! ");
        }
        else
        {
            var prefab = PlayerInformationManager.Instance.GetCharacterListGenerator()
                .GetPlayerPrefab(myGameInformation.playerCharacterName);

            if (prefab != null)
            {
                playerObj = Instantiate(prefab, playerObjects.transform);

                playerArmatureTransform = playerObj.transform;
            }
            else
            {
                Debug.LogError($"GameSceneManager.InitPlayerPrefab : Prefab is Nothing ! ");
            }
        }

        if (playerArmatureTransform != null)
        {
            playerTopCamera.Follow = playerArmatureTransform;
            playerTopCamera.LookAt = playerArmatureTransform;
            playerFollowCamera.Follow = playerObj.transform.Find("PlayerCameraRoot");
        }
        else
        {
            Debug.LogError($"GameSceneManager.InitPlayerPrefab : PlayerArmatureTransform is Null ! ");
        }


    }

    /// <summary>
    /// �y���̓ǂݍ��݂⏉����
    /// </summary>
    private void InitSheetMusic()
    {
        if (myGameInformation.mode == PlayerInformationManager.GameMode.None ||
            myGameInformation.mode == PlayerInformationManager.GameMode.FreeStyle)
        {
            sheetMusic.notes = null;
            sheetMusic.title = "";
            sheetMusic.notesNum = 0;
            sheetMusic.tempo = 0;
            sheetMusic.hyoushiBunshi = 0;
            sheetMusic.hyoushiBunbo = 0;

            if (myGameInformation.mode == PlayerInformationManager.GameMode.None)
            {
                Debug.LogError($"GameSceneManager.InitSheetMusic : Mode is None !");
            }
        }
        else
        {
            sheetMusic.notes = new List<SingleNote>();

            TextAsset textfile = sheetMusicListGenerator.GetSheetMusicFile(myGameInformation.sheetMusicName);

            StringReader stackLevelCostReader = new StringReader(textfile.text);
            string text = stackLevelCostReader.ReadToEnd();

            string[] allText = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            int i = 0;
            foreach (var item in allText)
            {
                // ���s���J�b�g����
                string itemStr = CutEnter(item);

                if (string.IsNullOrEmpty(itemStr))
                {
                    //break;
                }
                else
                {
                    if (i == 0)
                    {
                        sheetMusic.title = itemStr;
                    }
                    else if (i == 1)
                    {
                        string[] str = itemStr.Split('/');
                        sheetMusic.hyoushiBunshi = float.Parse(str[0]);
                        sheetMusic.hyoushiBunbo = float.Parse(str[1]);
                    }
                    else if (i == 2)
                    {
                        sheetMusic.tempo = float.Parse(itemStr);
                    }
                    else
                    {
                        if (itemStr == "END") break;

                        string[] str = itemStr.Split(',');

                        XylophoneManager.Notes notes;

                        if (Enum.TryParse(str[0], out XylophoneManager.Notes auth))
                        {
                            notes = auth;
                        }
                        else if (Enum.TryParse(str[0], out XylophoneManager.NotesAnother authAnother))
                        {
                            notes = (XylophoneManager.Notes)authAnother;
                        }
                        else
                        {
                            Debug.LogError($"GameSceneManager.InitSheetMusic : Not Find Notes Name -> str[0] = {str[0]} , item = {itemStr} , i = {i}");
                            notes = XylophoneManager.Notes.None;
                        }

                        if (float.TryParse(str[1], out float result))
                        {
                            sheetMusic.notes.Add(new SingleNote(notes, result));
                        }
                        else
                        {
                            Debug.LogError($"GameSceneManager.InitSheetMusic : Cannot Be Float -> str[1] = {str[1]} , item = {itemStr} , i = {i}");
                            sheetMusic.notes.Add(new SingleNote(notes, 0f));
                        }
                    }

                    i++;
                }
            }
            sheetMusic.notesNum = sheetMusic.notes.Count;


            // �y���t�@�C���̃��[�h���m�F���郍�O
            string debugText = $"�^�C�g�� : {sheetMusic.title}\n���q : {sheetMusic.hyoushiBunshi}/{sheetMusic.hyoushiBunbo}\n" +
                $"�e���| : {sheetMusic.tempo}\n�m�[�c�� : {sheetMusic.notesNum}\n";
            foreach (var item in sheetMusic.notes)
            {
                debugText += $"{item.keyBoardName}({item.bu})\n";
            }
            Debug.Log(debugText);
        }

    }

    /// <summary>
    /// BGM�Ȃǂ̏����ݒ�
    /// </summary>
    private void InitSounds()
    {
        SoundManager.Instance.SetStageMusic(stageBgmClip);
        SoundManager.Instance.SetSuccessMusic(sheetMusicListGenerator.GetSuccessMusicClip(myGameInformation.sheetMusicName));
    }

    /// <summary>
    /// �e�L�X�g�Ȃǂ�UI�̏�����
    /// </summary>
    private void InitUI()
    {
        if (myGameInformation.mode == PlayerInformationManager.GameMode.FreeStyle)
        {
            uIManager.SetFreeMode(myGameInformation.level);
        }
        else
        {
            uIManager.SetTimerText(playTimer);
            uIManager.SetMissCountText(missCount);

            UpdateNextsText();
        }

    }

    /// <summary>
    /// ���ԂƂ���UI���X�V����
    /// </summary>
    private void UpdateTimer()
    {
        if (myGameInformation.mode == PlayerInformationManager.GameMode.None)
        {

        }
        else if (myGameInformation.mode == PlayerInformationManager.GameMode.FreeStyle)
        {
            if (nowPhase == NowPhase.Play)
            {
                if (!pauseFlg)
                {
                    playTimer += Time.deltaTime;
                }
            }
        }
        else
        {
            if (nowPhase == NowPhase.Play)
            {
                if (!pauseFlg)
                {
                    playTimer += Time.deltaTime;
                    uIManager.SetTimerText(playTimer);
                }
            }
        }
    }

    /// <summary>
    /// �Q�[������ǂݍ���
    /// </summary>
    private void LoadPlayGameInformation()
    {
        bool errorFlg = false;

        if (PlayerInformationManager.Instance == null)
        {
            errorFlg = true;
            Debug.LogError($"LoadPlayGameInformation : PlayerInformationManager.Instance is Nothing !!");
        }
        else if (PlayerInformationManager.Instance.GetNextPlayGameInformation().mode == PlayerInformationManager.GameMode.None)
        {
            errorFlg = true;
            Debug.LogError($"LoadPlayGameInformation : GameMode is None !!");
        }
        else
        {
            myGameInformation = PlayerInformationManager.Instance.GetNextPlayGameInformation();
        }

        if (errorFlg)
        {
            myGameInformation.sheetMusicName = sheetMusicName;
            myGameInformation.skipForExampleFlg = skipForExampleFlg;
            myGameInformation.playerCharacterName = playerCharactorName;
            myGameInformation.level = stageLevel;

            myGameInformation.mode = PlayerInformationManager.GameMode.StageClear;
            myGameInformation.stageNumber = -99;
            myGameInformation.targetTime = 60f;
        }

    }

    /// <summary>
    /// ��Փx�ɉ����ēG��z�u����
    /// </summary>
    private void SpawnEnemies()
    {
        if (myGameInformation.level == PlayerInformationManager.GameLevel.Normal)
        {

        }
        else if (myGameInformation.level == PlayerInformationManager.GameLevel.Hard)
        {
            SpawnEnemiesForHard();
        }
        else if (myGameInformation.level == PlayerInformationManager.GameLevel.VeryHard)
        {
            SpawnEnemiesForHard();
            SpawnEnemiesForVeryhard();
        }
        else
        {
            Debug.LogError($"GameSceneManager.SpawnEnemys : Level Error {myGameInformation.level}");
        }
    }

    /// <summary>
    /// Hard�p�̓G��z�u����
    /// </summary>
    private void SpawnEnemiesForHard()
    {
        EnemyManager.Instance.SpawnFlyEnemy(0, 0.8f);
        EnemyManager.Instance.SpawnFlyEnemy(1, 0.5f);
        EnemyManager.Instance.SpawnFlyEnemy(2, 0.2f);
        EnemyManager.Instance.SpawnFlyEnemy(3, 0.5f);
        EnemyManager.Instance.SpawnFlyEnemy(4, 0.8f);
    }

    /// <summary>
    /// VeryHard�p�̓G��z�u����
    /// </summary>
    private void SpawnEnemiesForVeryhard()
    {
        EnemyManager.Instance.SpawnWalkUnderEnemy(0, 0.2f);
        EnemyManager.Instance.SpawnWalkUnderEnemy(0, 0.8f);
        EnemyManager.Instance.SpawnWalkUnderEnemy(2, 0.2f);
        EnemyManager.Instance.SpawnWalkUnderEnemy(2, 0.8f);

        EnemyManager.Instance.SpawnWalkOverEnemy(0, 0.2f);
        EnemyManager.Instance.SpawnWalkOverEnemy(0, 0.8f);
        EnemyManager.Instance.SpawnWalkOverEnemy(2, 0.2f);
        EnemyManager.Instance.SpawnWalkOverEnemy(2, 0.8f);
    }

    /// <summary>
    /// Start���ŁA���[�h�ɂ���ăt�F�[�Y�����߂�
    /// </summary>
    private void DecidePhaseForStart()
    {
        if (myGameInformation.mode == PlayerInformationManager.GameMode.None)
        {
            Debug.LogError($"GameSceneManager.DecidePhaseForStart : GameMode is None ! ");
            ChangeNowPhase(NowPhase.Play, 0f);
        }
        else if (myGameInformation.mode == PlayerInformationManager.GameMode.FreeStyle)
        {
            SwitchOperatePlayerEnable(true);
            ChangeNowPhase(NowPhase.Play, 0f);
            SpawnEnemies();
        }
        else if (myGameInformation.mode == PlayerInformationManager.GameMode.StageClear && myGameInformation.skipForExampleFlg == false &&
            PlayerInformationManager.Instance.GetTotalHaveStarsForStageClearMode() <= 0)
        {
            ChangeNowPhase(NowPhase.HowToPlay, 0.5f);
        }
        else
        {
            ChangeNowPhase(NowPhase.ForExample, 0.5f);
        }
    }

    /// <summary>
    /// ���Ղ𓥂񂾎��Ɏ�M����
    /// </summary>
    /// <param name="notesKeyboard"></param>
    public void ReceptionStepKeyboard(NotesKeyboard notesKeyboard)
    {
        var stepNotes = notesKeyboard.GetMyNotes();

        XylophoneManager.Instance.PlayNotes(stepNotes);

        //Debug.Log($"GameSceneManager.StepKeyboard : Step {stepNotes} ");


        if (myGameInformation.mode == PlayerInformationManager.GameMode.None ||
            myGameInformation.mode == PlayerInformationManager.GameMode.FreeStyle)
        {
            notesKeyboard.ChangeMaterialColorAnother();
        }
        else
        {
            if (stepNotes == sheetMusic.notes[progress].keyBoardName)
            {
                notesKeyboard.ChangeMaterialColorSuccess();
                progress++;
                UpdateNextsText();
                if (progress == sheetMusic.notesNum)
                {
                    ChangeNowPhase(NowPhase.GameClear, 0f);
                    XylophoneManager.Instance.UpdateNextNotesFlg(XylophoneManager.Notes.None);
                }
                else
                {
                    XylophoneManager.Instance.UpdateNextNotesFlg(sheetMusic.notes[progress].keyBoardName);
                }
            }
            else
            {
                notesKeyboard.ChangeMaterialColorFailure();
                missCount++;
                uIManager.SetMissCountText(missCount);
            }
        }
    }

    /// <summary>
    /// �����̕\�����[�h��ς���
    /// </summary>
    /// <param name="mode"></param>
    public void ChangeDisplayNotesKind()
    {
        displayNotesKind = PlayerInformationManager.Instance.GetSettingDisplayNotesKind();
    }

    /// <summary>
    /// �J�������_�J�����ɂ��邩��؂�ւ���
    /// </summary>
    /// <param name="flg">true:��_�J�����ɂ��� , false:���Ȃ�</param>
    public void ChangeFixedPointCamera(bool flg)
    {
        fixedPointCameraFlg = flg;
        FixedPointCameraPriority(fixedPointCameraFlg);
    }

    /// <summary>
    /// ��_�J�����ƒǐՃJ������؂�ւ���
    /// </summary>
    public void SwitchFixedPointCamera()
    {
        fixedPointCameraFlg = !fixedPointCameraFlg;
        FixedPointCameraPriority(fixedPointCameraFlg);
    }

    /// <summary>
    /// ��_�J������Priority��؂�ւ���
    /// </summary>
    /// <param name="flg">true:��_�J�����ɂ��� , false:���Ȃ�</param>
    private void FixedPointCameraPriority(bool flg)
    {
        if (flg)
        {
            fixedPointCamera.Priority = 15;
            playerArmatureTransform.gameObject.GetComponent<StarterAssets.ThirdPersonController>().LockCameraPosition = true;
        }
        else
        {
            playerArmatureTransform.gameObject.GetComponent<StarterAssets.ThirdPersonController>().LockCameraPosition = false;
            fixedPointCamera.Priority = 5;
        }
    }

    /// <summary>
    /// �Q�[���I�[�o�[����M����
    /// </summary>
    public void ReceptionGameOver()
    {
        if (myGameInformation.mode == PlayerInformationManager.GameMode.None)
        {
            PlayerFallforFreeStyleMode();
        }
        else if (myGameInformation.mode == PlayerInformationManager.GameMode.FreeStyle)
        {
            PlayerFallforFreeStyleMode();
        }
        else
        {
            ChangeNowPhase(NowPhase.GameOver, 0);
        }
    }

    /// <summary>
    /// �t���[�X�^�C�����[�h�Ńv���C���[�������̏���
    /// </summary>
    private void PlayerFallforFreeStyleMode()
    {
        //playerArmatureTransform.localPosition = new Vector3(0, 10, 0);

        ChangeNowPhase(NowPhase.GameOver, 0);



    }

    /// <summary>
    /// �v���C���[�̑�����ł���悤�ɂ��邩�ǂ�����؂�ւ���
    /// </summary>
    /// <param name="flg">true : ������󂯕t���� , false : ������󂯕t���Ȃ�</param>
    public void SwitchOperatePlayerEnable(bool flg)
    {
        playerArmatureTransform.gameObject.GetComponent<UnityEngine.InputSystem.PlayerInput>().enabled = flg;
    }

    /// <summary>
    /// �v���C���[���S�Ă̓������ł���悤�ɂ��邩�ǂ�����؂�ւ���
    /// </summary>
    /// <param name="flg">true : ������ , false : �����Ȃ�</param>
    public void SwitchPlayerControllerEnable(bool flg)
    {
        playerArmatureTransform.gameObject.GetComponent<StarterAssets.ThirdPersonController>().enabled = flg;
    }

    /// <summary>
    /// ����{�̃R���[�`��
    /// </summary>
    /// <returns></returns>
    private IEnumerator ExampleShowCoroutine()
    {
        float waitTime = 2f;

        uIManager.SetTestText($"{sheetMusic.title}\n(����{)", 2);

        FixedPointCameraPriority(true);
        yield return new WaitForSeconds(waitTime);

        foreach (var item in sheetMusic.notes)
        {
            float timeSecond = sheetMusic.hyoushiBunbo * 60f / sheetMusic.tempo / (item.bu * sheetMusic.hyoushiBunbo / 4f);

            RingKeyboardNyGameManager(item.keyBoardName);
            yield return new WaitForSeconds(timeSecond);
        }

        FixedPointCameraPriority(false);
        yield return new WaitForSeconds(waitTime);

        ChangeNowPhase(NowPhase.StandBy, 0.5f);
    }

    /// <summary>
    /// �Q�[���X�^�[�g�O�̃J�E���g�_�E���̃R���[�`��
    /// </summary>
    /// <returns></returns>
    private IEnumerator StandByCountDownCoroutine()
    {
        uIManager.SetTestText(sheetMusic.title, 2);

        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.GameCountDounReady);
        countDownValue = 3;
        uIManager.SetCenterText($"{countDownValue}...");
        SpawnEnemies();
        yield return new WaitForSeconds(1);

        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.GameCountDounReady);
        countDownValue--;
        uIManager.SetCenterText($"{countDownValue}...");
        XylophoneManager.Instance.UpdateNextNotesFlg(sheetMusic.notes[progress].keyBoardName);
        yield return new WaitForSeconds(1);

        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.GameCountDounReady);
        countDownValue--;
        uIManager.SetCenterText($"{countDownValue}...");
        yield return new WaitForSeconds(1);

        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.GameCountDounStart);
        countDownValue--;
        uIManager.SetCenterText("START !!");

        SwitchOperatePlayerEnable(true);
        ChangeNowPhase(NowPhase.Play, 0f);

        SoundManager.Instance.PlayStageMusic();
    }

    /// <summary>
    /// �Q�[���I�[�o�[���o�̃R���[�`��
    /// </summary>
    /// <param name="retryFlg">�v���C���[�����g���C�{�^����I�������Ƃ���true</param>
    /// <returns></returns>
    private IEnumerator GameOverCoroutine(bool retryFlg)
    {
        SwitchOperatePlayerEnable(false);

        if (!retryFlg)
        {
            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.GameOver);
            uIManager.SetCenterText("GAME OVER");
            playerTopCamera.Priority = 100;

            yield return new WaitForSeconds(0.5f);
        }

        if (PlayerInformationManager.Instance != null)
        {
            var info = PlayerInformationManager.Instance.GetNextPlayGameInformation();
            if (!info.skipForExampleFlg)
            {
                info.skipForExampleFlg = true;
                PlayerInformationManager.Instance.SetNextPlayGameInformation(info);
            }
        }
        else
        {
            Debug.LogError($"GameSceneManager.GameOverCoroutine : PlayerInformationManager.Instance is Null ! ");
        }

        yield return new WaitForSeconds(0.5f);

        FadeManager.Instance.LoadScene("Game", 0.3f);

    }

    /// <summary>
    /// ���g���C����M�����Ƃ�
    /// </summary>
    public void ReceptionRetry()
    {
        StartCoroutine(GameOverCoroutine(true));
    }

    /// <summary>
    /// ���j���[�V�[���ֈڍs����M�����Ƃ�
    /// </summary>
    public void ReceptionGoToMenuScene()
    {
        if (AdMobManager.Instance.GetNoAdFlg())
        {
            ReceptionAdMobManagerForInterstitial(false);
        }
        else
        {
            AdMobManager.Instance.ShowInterstitialAd();
        }
    }

    /// <summary>
    /// �L����\�����邩�̎|���󂯎��
    /// </summary>
    /// <param name="adFlg">�L���\������Ȃ�True , �\�����Ȃ�or�\�����I������Ȃ�False</param>
    public void ReceptionAdMobManagerForInterstitial(bool adFlg)
    {
        Debug.Log($"GameSceneManager.ReceptionAdMobManagerForInterstitial : AdFlg = {adFlg}");

        if (adFlg)
        {

        }
        else
        {
            FadeManager.Instance.LoadScene("Menu", 0.3f);
        }
    }

    /// <summary>
    /// �Q�[���N���A���o�̃R���[�`��
    /// </summary>
    /// <returns></returns>
    private IEnumerator GameClearCoroutine()
    {
        SwitchOperatePlayerEnable(false);

        uIManager.SetCenterText("GAME CLEAR");

        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.GameClear);
        DisplayKamifubuki(true);

        yield return new WaitForSeconds(1);

        SoundManager.Instance.StopStageMusic();
        SoundManager.Instance.PlaySuccessMusic();

        uIManager.DisplayResultUI();
    }

    /// <summary>
    /// ���݂̃t�F�[�Y��ύX����
    /// </summary>
    /// <param name="phase"></param>
    /// <param name="waitTime"></param>
    private void ChangeNowPhase(NowPhase phase, float waitTime)
    {
        if (nowPhase == NowPhase.GameClear)
        {
            Debug.LogError($"GameSceneManager.ChangeNowPhase : Already GameClear");
            return;
        }

        if (phase == NowPhase.Start)
        {
            nowPhase = phase;
        }
        else if (phase == NowPhase.ForExample && myGameInformation.skipForExampleFlg)
        {
            ChangeNowPhase(NowPhase.StandBy, waitTime);
        }
        else
        {
            StartCoroutine(ChangeNowPhaseCoroutine(phase, waitTime));
        }
    }

    /// <summary>
    /// �t�F�[�Y�ύX�̃R���[�`��
    /// </summary>
    /// <param name="phase"></param>
    /// <param name="waitTime"></param>
    /// <returns></returns>
    private IEnumerator ChangeNowPhaseCoroutine(NowPhase phase, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        nowPhase = phase;

        if (phase == NowPhase.ForExample)
        {
            forExampleCoroutine = StartCoroutine(ExampleShowCoroutine());
        }
        else if (phase == NowPhase.HowToPlay)
        {
            InstantiateHowToPlayPrefab();
        }
        else if (phase == NowPhase.StandBy)
        {
            StartCoroutine(StandByCountDownCoroutine());
        }
        else if (phase == NowPhase.GameOver)
        {
            StartCoroutine(GameOverCoroutine(false));
        }
        else if (phase == NowPhase.GameClear)
        {
            StartCoroutine(GameClearCoroutine());
        }

        Debug.Log($"GameSceneManager.ChangeNowPhaseCoroutine : NowPhase = {nowPhase}");
    }

    /// <summary>
    /// ���Ղ��Q�[���}�l�[�W���[����炷
    /// </summary>
    /// <param name="key"></param>
    private void RingKeyboardNyGameManager(XylophoneManager.Notes key)
    {
        XylophoneManager.Instance.PlayNotes(key);
        XylophoneManager.Instance.ChangeColorNotesKeyboad(key, COLOR_EXAMPLE);
    }

    /// <summary>
    /// �i���ɍ��킹�ăl�N�X�g�ƃl�N�l�N�̃e�L�X�g���X�V����
    /// </summary>
    private void UpdateNextsText()
    {
        if (sheetMusic.notesNum == progress)
        {
            uIManager.SetNext1Text(XylophoneManager.Notes.None);
            uIManager.SetNext2Text(XylophoneManager.Notes.None);
        }
        else if (sheetMusic.notesNum == progress + 1)
        {
            uIManager.SetNext1Text(sheetMusic.notes[progress].keyBoardName);
            uIManager.SetNext2Text(XylophoneManager.Notes.None);
        }
        else
        {
            uIManager.SetNext1Text(sheetMusic.notes[progress].keyBoardName);
            uIManager.SetNext2Text(sheetMusic.notes[progress + 1].keyBoardName);
        }

        uIManager.SetRestNotesText(sheetMusic.notesNum - progress);
    }

    /// <summary>
    /// Skip�{�^�������M
    /// </summary>
    public void ReceptionSkipForExample()
    {
        if (nowPhase == NowPhase.ForExample)
        {
            StopCoroutine(forExampleCoroutine);
            FixedPointCameraPriority(false);
            ChangeNowPhase(NowPhase.StandBy, 0.5f);
        }
    }

    /// <summary>
    /// �V�ѕ���Prefab�����ʂ��������Ƃ���M����
    /// </summary>
    public void ReceptionHowToPlayPrefab()
    {
        if (nowPhase == NowPhase.HowToPlay) ChangeNowPhase(NowPhase.ForExample, 0.5f);
    }

    /// <summary>
    /// �V�ѕ�Prefab�𐶐�����
    /// </summary>
    public void InstantiateHowToPlayPrefab()
    {
        Instantiate(howToPlayPrefab, canvasTransform);
    }

    /// <summary>
    /// PlayFabManager���璷���ԃo�b�N�O���E���h�ɂ������Ƃ���M�����Ƃ�
    /// </summary>
    public void ReceptionLongTimeBackGroundFromPlayFabManager()
    {
        ChangePauseFlg(true);
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.No);
        uIManager.InstantiateLongTimeBackGroundPrefab();
    }

    /// <summary>
    /// ���݂̃t�F�[�Y��Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public NowPhase GetNowPhase()
    {
        return nowPhase;

    }

    /// <summary>
    /// �|�[�Y�ɂ���
    /// </summary>
    /// <param name="flg"></param>
    public void ChangePauseFlg(bool flg)
    {
        pauseFlg = flg;
        SwitchPlayerControllerEnable(!flg);
        SwitchOperatePlayerEnable(!flg);
    }

    /// <summary>
    /// ���݂̃v���C�^�C����Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public float GetPlayTime()
    {
        return playTimer;
    }

    /// <summary>
    /// ���݂̃~�X�J�E���g��Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public int GetMissCount()
    {
        return missCount;
    }

    /// <summary>
    /// �|�[�Y�̃t���O���擾����Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public bool GetPauseFlg()
    {
        return pauseFlg;
    }

    /// <summary>
    /// myGameInformation��Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public PlayerInformationManager.PlayGameInformation GetGameInformation()
    {
        return myGameInformation;
    }

    /// <summary>
    /// �Q�[���X�^�[�g�O�̃J�E���g�_�E���̒l��Ԃ�
    /// </summary>
    /// <returns></returns>
    public int GetCountDownValue()
    {
        return countDownValue;
    }

    /// <summary>
    /// �����቉�o�̕\����؂�ւ���
    /// </summary>
    /// <param name="flg"></param>
    private void DisplayKamifubuki(bool flg)
    {
        if (flg)
        {
            kamifubuki.Play();
        }
        else
        {
            kamifubuki.Stop();
        }
    }

    /// <summary>
    /// Canvas��Transform��Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public Transform GetCanvasTransform()
    {
        return canvasTransform;
    }

    /// <summary>
    /// ������̉��s������
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    private string CutEnter(string str)
    {
        return str.Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace("\v", "").Replace("\0", "").Replace(" ", "").Trim();
    }

}
