using System;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Collections;

public class PlayerInformationManager : MonoBehaviour
{
    private string playerName;
    private int haveMedal;
    private int haveSPCoin;
    private int playerLevel;
    private int playerExperience;
    private int logInDays;
    private DateTime lastLoginDay;
    private bool todaySNSShareFlg;
    private int rewardTimes;
    private int shareDays;

    private string filePath;
    private SaveData save;

    private int[] haveProfilePartsLength;
    public const int NUMBER_OF_PLOFILELIST = 11;

    private const int FIRST_HAVE_MEDAL = 100;
    private const int FIRST_HAVE_SP_COIN = 0;
    private const int FIRST_PLAYER_LEVEL = 1;

    [HideInInspector] public ProfileListEntry[] profileListEntries;
    [HideInInspector] public int[] settingPartsNumber;
    [HideInInspector] public int[] settingPartsPositionH;
    [HideInInspector] public int[] settingPartsPositionV;
    public HaveProfileParts haveProfileParts;

    private bool newPlayerFlg;

    public struct HaveProfileParts
    {
        public bool[] haveBackground;
        public bool[] haveOutline;
        public bool[] haveAccessory;
        public bool[] haveWrinkle;
        public bool[] haveEar;
        public bool[] haveMouth;
        public bool[] haveNose;
        public bool[] haveEyebrow;
        public bool[] haveEye;
        public bool[] haveGlasses;
        public bool[] haveHair;
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

        LoadProfileListDataBase();
        InitLoad();

        // test
        testMethod(0);
    }

    /// <summary>
    /// �e�X�g���\�b�h(�f�o�b�O�p)
    /// </summary>
    /// <param name="num"></param>
    private void testMethod(int num)
    {
        bool defaultFlg = false;
        switch (num)
        {
            case 1: // �f�[�^�̏�������
                save.commonData.playerName = "Danchi";
                save.commonData.logInDays = 3;
                save.commonData.playerLevel = 12;
                save.commonData.playerExperience = 26;
                save.commonData.haveMedal = 1000;
                save.commonData.haveSPCoin = 5;
                save.dropBallsData.perfectClearTimes = 1;

                save.profileData.myBackgroundNumber = 1;
                save.profileData.myEyeVerticalPosition = 100;
                save.profileData.myEyeHorizontalPosition = 50;

                InitializeHavePartsForDefaultAll();

                save.haveProfileParts.haveBackground[3] = true;

                SaveJson();
                break;
            case 2: // �f�[�^�̓ǂݍ���
                LoadJson();
                break;
            case 3: // �t�@�C�����폜
                DeleteSaveFile();
                break;
            case 4: // �f�[�^��ǂݍ��񂾌�A�w�肵���f�[�^�̃C���N�������g
                LoadJson();
                IncrementField("dropBallsData", "perfectClearTimes");
                break;
            case 5: // ���O���o��
                Debug.Log($"testMethod[5] : {save.haveProfileParts.haveBackground[0]}");
                Debug.Log($"testMethod[5] : {save.haveProfileParts.haveBackground[1]}");
                Debug.Log($"testMethod[5] : {save.haveProfileParts.haveBackground[2]}");
                Debug.Log($"testMethod[5] : {save.haveProfileParts.haveBackground[3]}");
                break;
            default:
                defaultFlg = true;
                break;
        }

        if (!defaultFlg)
        {
            Debug.Log($"PlayerInformationManager.testMethod : num {num}");
        }

        Debug.Log(JsonUtility.ToJson(save, true));
    }

    /// <summary>
    /// json�t�@�C����ǂݍ��݁A�N���X�̕ϐ��ɒl��������
    /// </summary>
    public void InitLoad()
    {
        filePath = Application.persistentDataPath + "/" + ".savedata.json";
        save = new SaveData();
        
        haveProfilePartsLength = new int[NUMBER_OF_PLOFILELIST];
        for (int i = 0; i < NUMBER_OF_PLOFILELIST; i++) 
        {
            haveProfilePartsLength[i] = profileListEntries[i].itemList.Count;

            // Debug.Log($"itemList.Count : {i} -> {haveProfilePartsLength[i]}");

        }
        save.InitializeHaveProfileParts(haveProfilePartsLength);

        bool loadFlg = LoadJson();

        if (loadFlg)
        {
            playerName = save.commonData.playerName;
            haveMedal = save.commonData.haveMedal;
            haveSPCoin = save.commonData.haveSPCoin;
            playerLevel = save.commonData.playerLevel;
            playerExperience = save.commonData.playerExperience;
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

            for (int i = 0; i < NUMBER_OF_PLOFILELIST; i++)
            {
                //�ݒ肵�Ă���SProfile�摜�ƈʒu�Ǝ����Ă���S�p�[�c���f�[�^�x�[�X����ϐ��ɑ��
                switch (i)
                {
                    case 0:
                        {
                            settingPartsNumber[i] = save.profileData.myBackgroundNumber;
                            settingPartsPositionH[i] = 0;
                            settingPartsPositionV[i] = 0;

                            haveProfileParts.haveBackground = new bool[haveProfilePartsLength[i]];
                            for (int j = 0; j < save.haveProfileParts.haveBackground.Length; j++)
                            {
                                haveProfileParts.haveBackground[j] = save.haveProfileParts.haveBackground[j];
                            }
                            break;
                        }
                    case 1:
                        {
                            settingPartsNumber[i] = save.profileData.myOutlineNumber;
                            settingPartsPositionH[i] = save.profileData.myOutlineHorizontalPosition;
                            settingPartsPositionV[i] = save.profileData.myOutlineVerticalPosition;

                            haveProfileParts.haveOutline = new bool[haveProfilePartsLength[i]];
                            for (int j = 0; j < save.haveProfileParts.haveOutline.Length; j++)
                            {
                                haveProfileParts.haveOutline[j] = save.haveProfileParts.haveOutline[j];
                            }
                            break;
                        }
                    case 2:
                        {
                            settingPartsNumber[i] = save.profileData.myAccessoryNumber;
                            settingPartsPositionH[i] = save.profileData.myAccessoryHorizontalPosition;
                            settingPartsPositionV[i] = save.profileData.myAccessoryVerticalPosition;

                            haveProfileParts.haveAccessory = new bool[haveProfilePartsLength[i]];
                            for (int j = 0; j < save.haveProfileParts.haveAccessory.Length; j++)
                            {
                                haveProfileParts.haveAccessory[j] = save.haveProfileParts.haveAccessory[j];
                            }
                            break;
                        }
                    case 3:
                        {
                            settingPartsNumber[i] = save.profileData.myWrinkleNumber;
                            settingPartsPositionH[i] = save.profileData.myWrinkleHorizontalPosition;
                            settingPartsPositionV[i] = save.profileData.myWrinkleVerticalPosition;

                            haveProfileParts.haveWrinkle = new bool[haveProfilePartsLength[i]];
                            for (int j = 0; j < save.haveProfileParts.haveWrinkle.Length; j++)
                            {
                                haveProfileParts.haveWrinkle[j] = save.haveProfileParts.haveWrinkle[j];
                            }
                            break;
                        }
                    case 4:
                        {
                            settingPartsNumber[i] = save.profileData.myEarNumber;
                            settingPartsPositionH[i] = save.profileData.myEarHorizontalPosition;
                            settingPartsPositionV[i] = save.profileData.myEarVerticalPosition;

                            haveProfileParts.haveEar = new bool[haveProfilePartsLength[i]];
                            for (int j = 0; j < save.haveProfileParts.haveEar.Length; j++)
                            {
                                haveProfileParts.haveEar[j] = save.haveProfileParts.haveEar[j];
                            }
                            break;
                        }
                    case 5:
                        {
                            settingPartsNumber[i] = save.profileData.myMouthNumber;
                            settingPartsPositionH[i] = save.profileData.myMouthHorizontalPosition;
                            settingPartsPositionV[i] = save.profileData.myMouthVerticalPosition;

                            haveProfileParts.haveMouth = new bool[haveProfilePartsLength[i]];
                            for (int j = 0; j < save.haveProfileParts.haveMouth.Length; j++)
                            {
                                haveProfileParts.haveMouth[j] = save.haveProfileParts.haveMouth[j];
                            }
                            break;
                        }
                    case 6:
                        {
                            settingPartsNumber[i] = save.profileData.myNoseNumber;
                            settingPartsPositionH[i] = save.profileData.myNoseHorizontalPosition;
                            settingPartsPositionV[i] = save.profileData.myNoseVerticalPosition;

                            haveProfileParts.haveNose = new bool[haveProfilePartsLength[i]];
                            for (int j = 0; j < save.haveProfileParts.haveNose.Length; j++)
                            {
                                haveProfileParts.haveNose[j] = save.haveProfileParts.haveNose[j];
                            }
                            break;
                        }
                    case 7:
                        {
                            settingPartsNumber[i] = save.profileData.myEyebrowNumber;
                            settingPartsPositionH[i] = save.profileData.myEyebrowHorizontalPosition;
                            settingPartsPositionV[i] = save.profileData.myEyebrowVerticalPosition;

                            haveProfileParts.haveEyebrow = new bool[haveProfilePartsLength[i]];
                            for (int j = 0; j < save.haveProfileParts.haveEyebrow.Length; j++)
                            {
                                haveProfileParts.haveEyebrow[j] = save.haveProfileParts.haveEyebrow[j];
                            }
                            break;
                        }
                    case 8:
                        {
                            settingPartsNumber[i] = save.profileData.myEyeNumber;
                            settingPartsPositionH[i] = save.profileData.myEyeHorizontalPosition;
                            settingPartsPositionV[i] = save.profileData.myEyeVerticalPosition;

                            haveProfileParts.haveEye = new bool[haveProfilePartsLength[i]];
                            for (int j = 0; j < save.haveProfileParts.haveEye.Length; j++)
                            {
                                haveProfileParts.haveEye[j] = save.haveProfileParts.haveEye[j];
                            }
                            break;
                        }
                    case 9:
                        {
                            settingPartsNumber[i] = save.profileData.myGlassesNumber;
                            settingPartsPositionH[i] = save.profileData.myGlassesHorizontalPosition;
                            settingPartsPositionV[i] = save.profileData.myGlassesVerticalPosition;

                            haveProfileParts.haveGlasses = new bool[haveProfilePartsLength[i]];
                            for (int j = 0; j < save.haveProfileParts.haveGlasses.Length; j++)
                            {
                                haveProfileParts.haveGlasses[j] = save.haveProfileParts.haveGlasses[j];
                            }
                            break;
                        }
                    case 10:
                        {
                            settingPartsNumber[i] = save.profileData.myHairNumber;
                            settingPartsPositionH[i] = save.profileData.myHairHorizontalPosition;
                            settingPartsPositionV[i] = save.profileData.myHairVerticalPosition;

                            haveProfileParts.haveHair = new bool[haveProfilePartsLength[i]];
                            for (int j = 0; j < save.haveProfileParts.haveHair.Length; j++)
                            {
                                haveProfileParts.haveHair[j] = save.haveProfileParts.haveHair[j];
                            }
                            break;
                        }
                    default:
                        {
                            Debug.LogWarning($"PlayerInformationManager.InitLoad : switch is default : i = {i}");
                            return;
                        }
                }
            }
        }
        else
        {
            playerName = "";
            haveMedal = FIRST_HAVE_MEDAL;
            haveSPCoin = FIRST_HAVE_SP_COIN;
            playerLevel = FIRST_PLAYER_LEVEL;

            save.commonData.playerName = playerName;
            save.commonData.haveMedal = haveMedal;
            save.commonData.haveSPCoin = haveSPCoin;
            save.commonData.playerLevel = playerLevel;

            InitHaveDefaultParts();
        }

        SaveJson();
    }

    /// <summary>
    /// Profile��List�̑S�f�[�^�x�[�X��ǂݍ���
    /// </summary>
    private void LoadProfileListDataBase()
    {
        string resourcePath;
        string databaseName;

        profileListEntries = new ProfileListEntry[NUMBER_OF_PLOFILELIST];
        settingPartsNumber = new int[NUMBER_OF_PLOFILELIST];
        settingPartsPositionH = new int[NUMBER_OF_PLOFILELIST];
        settingPartsPositionV = new int[NUMBER_OF_PLOFILELIST];

        for (int i = 0; i < NUMBER_OF_PLOFILELIST; i++)
        {
            switch (i)
            {
                case  0: databaseName = "00_Background"; break;
                case  1: databaseName = "01_Outline"; break;
                case  2: databaseName = "02_Accessory"; break;
                case  3: databaseName = "03_Wrinkle"; break;
                case  4: databaseName = "04_Ear"; break;
                case  5: databaseName = "05_Mouth"; break;
                case  6: databaseName = "06_Nose"; break;
                case  7: databaseName = "07_Eyebrow"; break;
                case  8: databaseName = "08_Eye"; break;
                case  9: databaseName = "09_Glasses"; break;
                case 10: databaseName = "10_Hair"; break;
                default:
                    Debug.LogWarning($"PlayerInformationManager.LoadProfileListDataBase : switch is default : i = {i}");
                    return;
            }
            resourcePath = "Profile/" + databaseName;

            profileListEntries[i] = Resources.Load<ProfileListEntry>(resourcePath);
            if (profileListEntries[i] == null)
            {
                Debug.LogWarning("PlayerInformationManager.LoadProfileListDataBase : ProfileListEntry not found at path : " + resourcePath);
            }
            else if(profileListEntries[i].itemList.Count == 0)
            {
                Debug.LogWarning($"PlayerInformationManager.LoadProfileListDataBase : itemList = 0 : {databaseName}");
            }
            else
            {
                // Debug.Log($"PlayerInformationManager.LoadProfileListDataBase : {databaseName} Size -> {profileListEntries[i].itemList.Count}");
            }
        }
    }

    /// <summary>
    /// json�t�@�C����ۑ�����
    /// </summary>
    public void SaveJson()
    {
        string json = JsonUtility.ToJson(save);
        StreamWriter streamWriter = new StreamWriter(filePath);
        streamWriter.Write(json);
        streamWriter.Flush();
        streamWriter.Close();
    }

    /// <summary>
    /// ��������Json�t�@�C����ۑ�����
    /// </summary>
    /// <param name="repairJsonString"></param>
    public async Task SaveRepairJson(SaveData repairSavedata)
    {
        string repairJsonString = JsonUtility.ToJson(repairSavedata);

        using (StreamWriter streamWriter = new StreamWriter(filePath))
        {
            await streamWriter.WriteAsync(repairJsonString);
            await streamWriter.FlushAsync();
        }
    }

    /// <summary>
    /// json�t�@�C����ǂݍ���
    /// </summary>
    public bool LoadJson()
    {
        bool flg;

        if (File.Exists(filePath))
        {
            newPlayerFlg = false;

            StreamReader streamReader;
            streamReader = new StreamReader(filePath);
            string data = streamReader.ReadToEnd();
            streamReader.Close();
            save = JsonUtility.FromJson<SaveData>(data);

            flg = true;
        }
        else
        {
            newPlayerFlg = true;

            // Debug.LogError("PlayerInformationManager.LoadJson : File not found: " + filePath);

            flg= false;
        }

        ArraySizeCheckAndChange();

        return flg;
    }

    /// <summary>
    /// Rarity��Default�̃p�[�c���擾����
    /// </summary>
    private void InitHaveDefaultParts()
    {
        for (int i = 0; i < NUMBER_OF_PLOFILELIST; i++)
        {
            settingPartsNumber[i] = 0;
            settingPartsPositionH[i] = 0;
            settingPartsPositionV[i] = 0;

            switch (i)
            {
                case 0:
                    {
                        haveProfileParts.haveBackground = new bool[haveProfilePartsLength[i]];
                        for (int j = 0; j < save.haveProfileParts.haveBackground.Length; j++)
                        {
                            if (profileListEntries[i].itemList[j].rarity == Profile.Rarity.Default)
                            {
                                haveProfileParts.haveBackground[j] = true;
                            }
                            else
                            {
                                haveProfileParts.haveBackground[j] = false;
                            }
                            save.haveProfileParts.haveBackground[j] = haveProfileParts.haveBackground[j];
                        }
                        save.profileData.myBackgroundNumber = settingPartsNumber[i];
                        break;
                    }
                case 1:
                    {
                        haveProfileParts.haveOutline = new bool[haveProfilePartsLength[i]];
                        for (int j = 0; j < save.haveProfileParts.haveOutline.Length; j++)
                        {
                            if (profileListEntries[i].itemList[j].rarity == Profile.Rarity.Default)
                            {
                                haveProfileParts.haveOutline[j] = true;
                            }
                            else
                            {
                                haveProfileParts.haveOutline[j] = false;
                            }
                            save.haveProfileParts.haveOutline[j] = haveProfileParts.haveOutline[j];
                        }
                        save.profileData.myOutlineNumber = settingPartsNumber[i];
                        save.profileData.myOutlineHorizontalPosition = settingPartsPositionH[i];
                        save.profileData.myOutlineVerticalPosition = settingPartsPositionV[i];
                        break;
                    }
                case 2:
                    {
                        haveProfileParts.haveAccessory = new bool[haveProfilePartsLength[i]];
                        for (int j = 0; j < save.haveProfileParts.haveAccessory.Length; j++)
                        {
                            if (profileListEntries[i].itemList[j].rarity == Profile.Rarity.Default)
                            {
                                haveProfileParts.haveAccessory[j] = true;
                            }
                            else
                            {
                                haveProfileParts.haveAccessory[j] = false;
                            }
                            save.haveProfileParts.haveAccessory[j] = haveProfileParts.haveAccessory[j];
                        }
                        save.profileData.myAccessoryNumber = settingPartsNumber[i];
                        save.profileData.myAccessoryHorizontalPosition = settingPartsPositionH[i];
                        save.profileData.myAccessoryVerticalPosition = settingPartsPositionV[i];
                        break;
                    }
                case 3:
                    {
                        haveProfileParts.haveWrinkle = new bool[haveProfilePartsLength[i]];
                        for (int j = 0; j < save.haveProfileParts.haveWrinkle.Length; j++)
                        {
                            if (profileListEntries[i].itemList[j].rarity == Profile.Rarity.Default)
                            {
                                haveProfileParts.haveWrinkle[j] = true;
                            }
                            else
                            {
                                haveProfileParts.haveWrinkle[j] = false;
                            }
                            save.haveProfileParts.haveWrinkle[j] = haveProfileParts.haveWrinkle[j];
                        }
                        save.profileData.myWrinkleNumber = settingPartsNumber[i];
                        save.profileData.myWrinkleHorizontalPosition = settingPartsPositionH[i];
                        save.profileData.myWrinkleVerticalPosition = settingPartsPositionV[i];
                        break;
                    }
                case 4:
                    {
                        haveProfileParts.haveEar = new bool[haveProfilePartsLength[i]];
                        for (int j = 0; j < save.haveProfileParts.haveEar.Length; j++)
                        {
                            if (profileListEntries[i].itemList[j].rarity == Profile.Rarity.Default)
                            {
                                haveProfileParts.haveEar[j] = true;
                            }
                            else
                            {
                                haveProfileParts.haveEar[j] = false;
                            }
                            save.haveProfileParts.haveEar[j] = haveProfileParts.haveEar[j];
                        }
                        save.profileData.myEarNumber = settingPartsNumber[i];
                        save.profileData.myEarHorizontalPosition = settingPartsPositionH[i];
                        save.profileData.myEarVerticalPosition = settingPartsPositionV[i];
                        break;
                    }
                case 5:
                    {
                        haveProfileParts.haveMouth = new bool[haveProfilePartsLength[i]];
                        for (int j = 0; j < save.haveProfileParts.haveMouth.Length; j++)
                        {
                            if (profileListEntries[i].itemList[j].rarity == Profile.Rarity.Default)
                            {
                                haveProfileParts.haveMouth[j] = true;
                            }
                            else
                            {
                                haveProfileParts.haveMouth[j] = false;
                            }
                            save.haveProfileParts.haveMouth[j] = haveProfileParts.haveMouth[j];
                        }
                        save.profileData.myMouthNumber = settingPartsNumber[i];
                        save.profileData.myMouthHorizontalPosition = settingPartsPositionH[i];
                        save.profileData.myMouthVerticalPosition = settingPartsPositionV[i];
                        break;
                    }
                case 6:
                    {
                        haveProfileParts.haveNose = new bool[haveProfilePartsLength[i]];
                        for (int j = 0; j < save.haveProfileParts.haveNose.Length; j++)
                        {
                            if (profileListEntries[i].itemList[j].rarity == Profile.Rarity.Default)
                            {
                                haveProfileParts.haveNose[j] = true;
                            }
                            else
                            {
                                haveProfileParts.haveNose[j] = false;
                            }
                            save.haveProfileParts.haveNose[j] = haveProfileParts.haveNose[j];
                        }
                        save.profileData.myNoseNumber = settingPartsNumber[i];
                        save.profileData.myNoseHorizontalPosition = settingPartsPositionH[i];
                        save.profileData.myNoseVerticalPosition = settingPartsPositionV[i];
                        break;
                    }
                case 7:
                    {
                        haveProfileParts.haveEyebrow = new bool[haveProfilePartsLength[i]];
                        for (int j = 0; j < save.haveProfileParts.haveEyebrow.Length; j++)
                        {
                            if (profileListEntries[i].itemList[j].rarity == Profile.Rarity.Default)
                            {
                                haveProfileParts.haveEyebrow[j] = true;
                            }
                            else
                            {
                                haveProfileParts.haveEyebrow[j] = false;
                            }
                            save.haveProfileParts.haveEyebrow[j] = haveProfileParts.haveEyebrow[j];
                        }
                        save.profileData.myEyebrowNumber = settingPartsNumber[i];
                        save.profileData.myEyebrowHorizontalPosition = settingPartsPositionH[i];
                        save.profileData.myEyebrowVerticalPosition = settingPartsPositionV[i];
                        break;
                    }
                case 8:
                    {
                        haveProfileParts.haveEye = new bool[haveProfilePartsLength[i]];
                        for (int j = 0; j < save.haveProfileParts.haveEye.Length; j++)
                        {
                            if (profileListEntries[i].itemList[j].rarity == Profile.Rarity.Default)
                            {
                                haveProfileParts.haveEye[j] = true;
                            }
                            else
                            {
                                haveProfileParts.haveEye[j] = false;
                            }
                            save.haveProfileParts.haveEye[j] = haveProfileParts.haveEye[j];
                        }
                        save.profileData.myEyeNumber = settingPartsNumber[i];
                        save.profileData.myEyeHorizontalPosition = settingPartsPositionH[i];
                        save.profileData.myEyeVerticalPosition = settingPartsPositionV[i];
                        break;
                    }
                case 9:
                    {
                        haveProfileParts.haveGlasses = new bool[haveProfilePartsLength[i]];
                        for (int j = 0; j < save.haveProfileParts.haveGlasses.Length; j++)
                        {
                            if (profileListEntries[i].itemList[j].rarity == Profile.Rarity.Default)
                            {
                                haveProfileParts.haveGlasses[j] = true;
                            }
                            else
                            {
                                haveProfileParts.haveGlasses[j] = false;
                            }
                            save.haveProfileParts.haveGlasses[j] = haveProfileParts.haveGlasses[j];
                        }
                        save.profileData.myGlassesNumber = settingPartsNumber[i];
                        save.profileData.myGlassesHorizontalPosition = settingPartsPositionH[i];
                        save.profileData.myGlassesVerticalPosition = settingPartsPositionV[i];
                        break;
                    }
                case 10:
                    {
                        haveProfileParts.haveHair = new bool[haveProfilePartsLength[i]];
                        for (int j = 0; j < save.haveProfileParts.haveHair.Length; j++)
                        {
                            if (profileListEntries[i].itemList[j].rarity == Profile.Rarity.Default)
                            {
                                haveProfileParts.haveHair[j] = true;
                            }
                            else
                            {
                                haveProfileParts.haveHair[j] = false;
                            }
                            save.haveProfileParts.haveHair[j] = haveProfileParts.haveHair[j];
                        }
                        save.profileData.myHairNumber = settingPartsNumber[i];
                        save.profileData.myHairHorizontalPosition = settingPartsPositionH[i];
                        save.profileData.myHairVerticalPosition = settingPartsPositionV[i];
                        break;
                    }
                default:
                    {
                        Debug.LogWarning($"PlayerInformationManager.InitLoad : switch is default : i = {i}");
                        return;
                    }
            }
        }

    }

    /// <summary>
    /// haveProfileParts�̃T�C�Y������Ȃ��Ƃ��Ƀ��T�C�Y����
    /// </summary>
    private void ArraySizeCheckAndChange()
    {
        ArraySizeCheckAndChange(ref save.haveProfileParts.haveBackground, 0);
        ArraySizeCheckAndChange(ref save.haveProfileParts.haveOutline, 1);
        ArraySizeCheckAndChange(ref save.haveProfileParts.haveAccessory, 2);
        ArraySizeCheckAndChange(ref save.haveProfileParts.haveWrinkle, 3);
        ArraySizeCheckAndChange(ref save.haveProfileParts.haveEar, 4);
        ArraySizeCheckAndChange(ref save.haveProfileParts.haveMouth, 5);
        ArraySizeCheckAndChange(ref save.haveProfileParts.haveNose, 6);
        ArraySizeCheckAndChange(ref save.haveProfileParts.haveEyebrow, 7);
        ArraySizeCheckAndChange(ref save.haveProfileParts.haveEye, 8);
        ArraySizeCheckAndChange(ref save.haveProfileParts.haveGlasses, 9);
        ArraySizeCheckAndChange(ref save.haveProfileParts.haveHair, 10);
    }

    /// <summary>
    /// ArraySizeCheckAndChange()�̒��g
    /// </summary>
    /// <param name="box">ref save.haveProfileParts.�Z�Z</param>
    /// <param name="number">�p�[�c�̎�ނ̔ԍ�</param>
    private void ArraySizeCheckAndChange(ref bool[] box , int number)
    {
        if (box.Length != haveProfilePartsLength[number])
        {
            Debug.Log($"Array.Resize : number.{number} : {box.Length} -> {haveProfilePartsLength[number]}");

            Array.Resize(ref box, haveProfilePartsLength[number]);

            for (int i = box.Length; i < haveProfilePartsLength[number]; i++)
            {
                box[i] = false;
            }
        }
    }

    /// <summary>
    /// json�t�@�C�����폜����
    /// </summary>
    public void DeleteSaveFile()
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    /// <summary>
    /// �S�Ẵp�[�c�����������������ADefault�̃A�C�e�����������ɂ���
    /// </summary>
    private void InitializeHavePartsForDefaultAll()
    {
        InitializeHavePartsForDefault(ref haveProfileParts.haveBackground, 0);
        InitializeHavePartsForDefault(ref haveProfileParts.haveOutline, 1);
        InitializeHavePartsForDefault(ref haveProfileParts.haveAccessory, 2);
        InitializeHavePartsForDefault(ref haveProfileParts.haveWrinkle, 3);
        InitializeHavePartsForDefault(ref haveProfileParts.haveEar, 4);
        InitializeHavePartsForDefault(ref haveProfileParts.haveMouth, 5);
        InitializeHavePartsForDefault(ref haveProfileParts.haveNose, 6);
        InitializeHavePartsForDefault(ref haveProfileParts.haveEyebrow, 7);
        InitializeHavePartsForDefault(ref haveProfileParts.haveEye, 8);
        InitializeHavePartsForDefault(ref haveProfileParts.haveGlasses, 9);
        InitializeHavePartsForDefault(ref haveProfileParts.haveHair, 10);

        Array.Copy(haveProfileParts.haveBackground, save.haveProfileParts.haveBackground, haveProfileParts.haveBackground.Length);
        Array.Copy(haveProfileParts.haveOutline, save.haveProfileParts.haveOutline, haveProfileParts.haveOutline.Length);
        Array.Copy(haveProfileParts.haveAccessory, save.haveProfileParts.haveAccessory, haveProfileParts.haveAccessory.Length);
        Array.Copy(haveProfileParts.haveWrinkle, save.haveProfileParts.haveWrinkle, haveProfileParts.haveWrinkle.Length);
        Array.Copy(haveProfileParts.haveEar, save.haveProfileParts.haveEar, haveProfileParts.haveEar.Length);
        Array.Copy(haveProfileParts.haveMouth, save.haveProfileParts.haveMouth, haveProfileParts.haveMouth.Length);
        Array.Copy(haveProfileParts.haveNose, save.haveProfileParts.haveNose, haveProfileParts.haveNose.Length);
        Array.Copy(haveProfileParts.haveEyebrow, save.haveProfileParts.haveEyebrow, haveProfileParts.haveEyebrow.Length);
        Array.Copy(haveProfileParts.haveEye, save.haveProfileParts.haveEye, haveProfileParts.haveEye.Length);
        Array.Copy(haveProfileParts.haveGlasses, save.haveProfileParts.haveGlasses, haveProfileParts.haveGlasses.Length);
        Array.Copy(haveProfileParts.haveHair, save.haveProfileParts.haveHair, haveProfileParts.haveHair.Length);
    }

    /// <summary>
    /// �w�肵���p�[�c�����������������ADefault�̃A�C�e�����������ɂ���
    /// </summary>
    /// <param name="box">�p�[�c���̃A�h���X</param>
    /// <param name="number">�p�[�c�̎�ޔԍ�</param>
    private void InitializeHavePartsForDefault(ref bool[] box,int number)
    {
        box = Enumerable.Repeat<bool>(false, haveProfilePartsLength[number]).ToArray();

        for (int i = 0; i < haveProfilePartsLength[number]; i++)
        {
            if (profileListEntries[number].itemList[i].rarity == Profile.Rarity.Default)
            {
                box[i] = true;
                Debug.Log($"PlayerInformationManager.InitializeHavePartsForDefault : {number} , {i}");
            }
            else
            {
                box[i] = false;
            }
        }
    }

    /// <summary>
    /// �w�肵���p�[�c���擾����
    /// </summary>
    /// <param name="partsKinds">�p�[�c�̎�ޔԍ�</param>
    /// <param name="partsNumber">�p�[�c�̗v�f�ԍ�</param>
    public void AcquisitionParts(int partsKinds,int partsNumber)
    {
        bool errorFlg = false;

        switch (partsKinds)
        {
            case 0:
                if(partsNumber > haveProfileParts.haveBackground.Length)
                {
                    errorFlg = true;
                }
                else if (partsNumber > save.haveProfileParts.haveBackground.Length)
                {
                    errorFlg = true;
                }
                haveProfileParts.haveBackground[partsNumber] = true;
                save.haveProfileParts.haveBackground[partsNumber] = true;
                break;
            case 1:
                if (partsNumber > haveProfileParts.haveOutline.Length)
                {
                    errorFlg = true;
                }
                else if (partsNumber > save.haveProfileParts.haveOutline.Length)
                {
                    errorFlg = true;
                }
                haveProfileParts.haveOutline[partsNumber] = true;
                save.haveProfileParts.haveOutline[partsNumber] = true;
                break;
            case 2:
                if (partsNumber > haveProfileParts.haveAccessory.Length)
                {
                    errorFlg = true;
                }
                else if (partsNumber > save.haveProfileParts.haveAccessory.Length)
                {
                    errorFlg = true;
                }
                haveProfileParts.haveAccessory[partsNumber] = true;
                save.haveProfileParts.haveAccessory[partsNumber] = true;
                break;
            case 3:
                if (partsNumber > haveProfileParts.haveWrinkle.Length)
                {
                    errorFlg = true;
                }
                else if (partsNumber > save.haveProfileParts.haveWrinkle.Length)
                {
                    errorFlg = true;
                }
                haveProfileParts.haveWrinkle[partsNumber] = true;
                save.haveProfileParts.haveWrinkle[partsNumber] = true;
                break;
            case 4:
                if (partsNumber > haveProfileParts.haveEar.Length)
                {
                    errorFlg = true;
                }
                else if (partsNumber > save.haveProfileParts.haveEar.Length)
                {
                    errorFlg = true;
                }
                haveProfileParts.haveEar[partsNumber] = true;
                save.haveProfileParts.haveEar[partsNumber] = true;
                break;
            case 5:
                if (partsNumber > haveProfileParts.haveMouth.Length)
                {
                    errorFlg = true;
                }
                else if (partsNumber > save.haveProfileParts.haveMouth.Length)
                {
                    errorFlg = true;
                }
                haveProfileParts.haveMouth[partsNumber] = true;
                save.haveProfileParts.haveMouth[partsNumber] = true;
                break;
            case 6:
                if (partsNumber > haveProfileParts.haveNose.Length)
                {
                    errorFlg = true;
                }
                else if (partsNumber > save.haveProfileParts.haveNose.Length)
                {
                    errorFlg = true;
                }
                haveProfileParts.haveNose[partsNumber] = true;
                save.haveProfileParts.haveNose[partsNumber] = true;
                break;
            case 7:
                if (partsNumber > haveProfileParts.haveEyebrow.Length)
                {
                    errorFlg = true;
                }
                else if (partsNumber > save.haveProfileParts.haveEyebrow.Length)
                {
                    errorFlg = true;
                }
                haveProfileParts.haveEyebrow[partsNumber] = true;
                save.haveProfileParts.haveEyebrow[partsNumber] = true;
                break;
            case 8:
                if (partsNumber > haveProfileParts.haveEye.Length)
                {
                    errorFlg = true;
                }
                else if (partsNumber > save.haveProfileParts.haveEye.Length)
                {
                    errorFlg = true;
                }
                haveProfileParts.haveEye[partsNumber] = true;
                save.haveProfileParts.haveEye[partsNumber] = true;
                break;
            case 9:
                if (partsNumber > haveProfileParts.haveGlasses.Length)
                {
                    errorFlg = true;
                }
                else if (partsNumber > save.haveProfileParts.haveGlasses.Length)
                {
                    errorFlg = true;
                }
                haveProfileParts.haveGlasses[partsNumber] = true;
                save.haveProfileParts.haveGlasses[partsNumber] = true;
                break;
            case 10:
                if (partsNumber > haveProfileParts.haveHair.Length)
                {
                    errorFlg = true;
                }
                else if (partsNumber > save.haveProfileParts.haveHair.Length)
                {
                    errorFlg = true;
                }
                haveProfileParts.haveHair[partsNumber] = true;
                save.haveProfileParts.haveHair[partsNumber] = true;
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
            SaveJson();
        }
    }

    /// <summary>
    /// �w�肵���l��1���₷
    /// </summary>
    /// <param name="className">������ className</param>
    /// <param name="fieldName">������ fieldName</param>
    public void IncrementField(string className, string fieldName)
    {
        className = className.ToLower();
        fieldName = fieldName.ToLower();
        bool isValid = true;

        // �����̃t�B�[���h���͑S�ď������ŋL�q����
        switch (className)
        {
            case "commondata":
                switch (fieldName)
                {
                    case "logindays":
                        save.commonData.logInDays++;
                        break;
                    // ���̃t�B�[���h�ɂ��Ă����l�ɏ�������
                    default:
                        isValid = false;
                        break;
                }
                break;
            case "dropballsdata":
                switch (fieldName)
                {
                    case "perfectcleartimes":
                        save.dropBallsData.perfectClearTimes++;
                        break;
                    case "shottimes":
                        save.dropBallsData.shotTimes++;
                        break;
                    case "lostballstotal":
                        save.dropBallsData.lostBallsTotal++;
                        break;
                    default:
                        isValid = false;
                        break;
                }
                break;
            case "jankendata":
                switch (fieldName)
                {
                    case "battletimes":
                        save.janKenData.battleTimes++;
                        break;
                    case "wintimes":
                        save.janKenData.winTimes++;
                        break;
                    default:
                        isValid = false;
                        break;
                }
                break;
            case "pusher01data":
                switch (fieldName)
                {
                    case "betmedaltotal":
                        save.pusher01Data.betMedalTotal++;
                        break;
                    case "getmedaltotal":
                        save.pusher01Data.getMedalTotal++;
                        break;
                    case "bonustimes":
                        save.pusher01Data.bonusTimes++;
                        break;
                    default:
                        isValid = false;
                        break;
                }
                break;
            default:
                isValid = false;
                break;
        }

        if (!isValid)
        {
            Debug.LogError($"PlayerInformationManager.IncrementField : Error : {className} {fieldName}");
        }
        else
        {
            SaveJson();
        }
    }

    /// <summary>
    /// �w�肵���l���w�肵�����������₷
    /// </summary>
    /// <param name="className">������ className</param>
    /// <param name="fieldName">������ fieldName</param>
    /// <param name="addNumber">���₷�l</param>
    public void AddField(string className, string fieldName,int addNumber)
    {
        className = className.ToLower();
        fieldName = fieldName.ToLower();
        bool isValid = true;

        // �����̃t�B�[���h���͑S�ď������ŋL�q����
        switch (className)
        {
            case "commondata":
                switch (fieldName)
                {
                    case "playerexperience":
                        save.commonData.playerExperience += addNumber;
                        break;
                    // ���̃t�B�[���h�ɂ��Ă����l�ɏ�������
                    default:
                        isValid = false;
                        break;
                }
                break;
            case "dropballsdata":
                switch (fieldName)
                {
                    case "getmedalstotal":
                        save.dropBallsData.getMedalsTotal += addNumber;
                        break;
                    case "lostballstotal":
                        save.dropBallsData.lostBallsTotal += addNumber;
                        break;
                    default:
                        isValid = false;
                        break;
                }
                break;
            case "jankendata":
                switch (fieldName)
                {
                    case "getmedalstotal":
                        save.janKenData.getMedalsTotal += addNumber;
                        break;
                    default:
                        isValid = false;
                        break;
                }
                break;
            case "smartball01data":
                switch (fieldName)
                {
                    case "betmedaltotal":
                        save.smartBall01Data.betMedalTotal += addNumber;
                        break;
                    case "getmedaltotal":
                        save.smartBall01Data.getMedalTotal += addNumber;
                        break;
                    default:
                        isValid = false;
                        break;
                }
                break;
            default:
                isValid = false;
                break;
        }

        if (!isValid)
        {
            Debug.LogError($"PlayerInformationManager.AddField : Error : {className} {fieldName}");
        }
        else
        {
            SaveJson();
        }
    }

    /// <summary>
    /// �����𖞂������Ƃ��A�w�肵���l���w�肵���l�ɍX�V����
    /// </summary>
    /// <param name="className">������ className</param>
    /// <param name="fieldName">������ fieldName</param>
    /// <param name="updateNumber">�X�V����l</param>
    public void UpdateField(string className, string fieldName, int updateNumber)
    {
        className = className.ToLower();
        fieldName = fieldName.ToLower();
        bool isValid = true;

        // �����̃t�B�[���h���͑S�ď������ŋL�q����
        switch (className)
        {
            case "smartball01data":
                switch (fieldName)
                {
                    case "maxlines":
                        if (save.smartBall01Data.maxLines < updateNumber)
                        {
                            save.smartBall01Data.maxLines = updateNumber;
                        }
                        break;
                    // ���̃t�B�[���h�ɂ��Ă����l�ɏ�������
                    default:
                        isValid = false;
                        break;
                }
                break;
            default:
                isValid = false;
                break;
        }

        if (!isValid)
        {
            Debug.LogError($"PlayerInformationManager.UpdateField : Error : {className} {fieldName}");
        }
        else
        {
            SaveJson();
        }
    }

    /// <summary>
    /// ���_���������B����Ȃ��ꍇ��false��Ԃ�
    /// </summary>
    /// <param name="numberOfMedals">�g�����_���̖���</param>
    /// <returns>����\�Ȃ�true�A���_���s���Ȃ�false</returns>
    public bool ConsumptionMedal(int numberOfMedals)
    {
        if (numberOfMedals <= haveMedal)
        {
            haveMedal -= numberOfMedals;
            SaveHaveMedal();
            return true;
        }
        else
        {
            Debug.Log($"PlayerInformationManager.UseMedal : Medal Low... : {haveMedal} - {numberOfMedals}");
            return false;
        }
    }

    /// <summary>
    /// ���_�����l������
    /// </summary>
    /// <param name="numberOfMedals">�l�����郁�_���̖���</param>
    public void AcquisitionMedal(int numberOfMedals)
    {
        if (numberOfMedals == 0) return;

        haveMedal += numberOfMedals;
        SaveHaveMedal();
    }

    /// <summary>
    /// SP�R�C���������B����Ȃ��ꍇ��false��Ԃ�
    /// </summary>
    /// <param name="numberOfCoins">�g��SP�R�C���̖���</param>
    /// <returns>����\�Ȃ�true�ASP�R�C���s���Ȃ�false</returns>
    public bool ConsumptionSPCoin(int numberOfCoins)
    {
        if (numberOfCoins <= haveSPCoin)
        {
            haveSPCoin -= numberOfCoins;
            SaveHaveSPCoin();
            return true;
        }
        else
        {
            Debug.Log($"PlayerInformationManager.UseSPCoin : SPCoin Low... : {haveSPCoin} - {numberOfCoins}");
            return false;
        }
    }

    /// <summary>
    /// SP�R�C�����l������
    /// </summary>
    /// <param name="numberOfCoins">�l������SP�R�C���̖���</param>
    public void AcquisitionSPCoin(int numberOfCoins)
    {
        haveSPCoin += numberOfCoins;
        SaveHaveSPCoin();
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
    /// ���_���������X�V���āAjson�t�@�C���ɕۑ�����
    /// </summary>
    public void SaveHaveMedal()
    {
        save.commonData.haveMedal = haveMedal;
        SaveJson();
    }

    /// <summary>
    /// SP�R�C���̖������X�V���āAjson�t�@�C���ɕۑ�����
    /// </summary>
    public void SaveHaveSPCoin()
    {
        save.commonData.haveSPCoin = haveSPCoin;
        SaveJson();
    }

    /// <summary>
    /// �v���t�B�[�������X�V���āAjson�t�@�C���ɕۑ�����
    /// </summary>
    /// <param name="spriteNumber">�ݒ肵�Ă���摜�̃��X�g�ԍ��̔z��</param>
    /// <param name="posX">�ݒ肵�Ă���|�W�V����X(H)�̔z��</param>
    /// <param name="posY">�ݒ肵�Ă���|�W�V����X(H)�̔z��</param>
    public void SaveSetProfile(int[] spriteNumber, int[] posX, int[] posY)
    {
        for (int i = 0; i < NUMBER_OF_PLOFILELIST; i++)
        {
            // �ǂݍ��񂾃f�[�^���X�V
            settingPartsNumber[i] = spriteNumber[i];
            settingPartsPositionH[i] = posX[i];
            settingPartsPositionV[i] = posY[i];

            // json�t�@�C���̒��g���X�V
            switch (i)
            {
                case 0: 
                    save.profileData.myBackgroundNumber = spriteNumber[i];
                    break;
                case 1:
                    save.profileData.myOutlineNumber = spriteNumber[i];
                    save.profileData.myOutlineHorizontalPosition = posX[i];
                    save.profileData.myOutlineVerticalPosition = posY[i];
                    break;
                case 2:
                    save.profileData.myAccessoryNumber = spriteNumber[i];
                    save.profileData.myAccessoryHorizontalPosition = posX[i];
                    save.profileData.myAccessoryVerticalPosition = posY[i];
                    break;
                case 3:
                    save.profileData.myWrinkleNumber = spriteNumber[i];
                    save.profileData.myWrinkleHorizontalPosition = posX[i];
                    save.profileData.myWrinkleVerticalPosition = posY[i];
                    break;
                case 4:
                    save.profileData.myEarNumber = spriteNumber[i];
                    save.profileData.myEarHorizontalPosition = posX[i];
                    save.profileData.myEarVerticalPosition = posY[i];
                    break;
                case 5:
                    save.profileData.myMouthNumber = spriteNumber[i];
                    save.profileData.myMouthHorizontalPosition = posX[i];
                    save.profileData.myMouthVerticalPosition = posY[i];
                    break;
                case 6:
                    save.profileData.myNoseNumber = spriteNumber[i];
                    save.profileData.myNoseHorizontalPosition = posX[i];
                    save.profileData.myNoseVerticalPosition = posY[i];
                    break;
                case 7:
                    save.profileData.myEyebrowNumber = spriteNumber[i];
                    save.profileData.myEyebrowHorizontalPosition = posX[i];
                    save.profileData.myEyebrowVerticalPosition = posY[i];
                    break;
                case 8:
                    save.profileData.myEyeNumber = spriteNumber[i];
                    save.profileData.myEyeHorizontalPosition = posX[i];
                    save.profileData.myEyeVerticalPosition = posY[i];
                    break;
                case 9:
                    save.profileData.myGlassesNumber = spriteNumber[i];
                    save.profileData.myGlassesHorizontalPosition = posX[i];
                    save.profileData.myGlassesVerticalPosition = posY[i];
                    break;
                case 10:
                    save.profileData.myHairNumber = spriteNumber[i];
                    save.profileData.myHairHorizontalPosition = posX[i];
                    save.profileData.myHairVerticalPosition = posY[i];
                    break;
                default:                                      
                    Debug.LogWarning($"PlayerInformationManager.SaveSetProfile : switch is default : i = {i}");
                    return;
            }
        }

        // �X�V��ۑ�
        SaveJson();
    }

    /// <summary>
    /// �������_�������̃Q�b�^�[
    /// </summary>
    /// <returns>�������_������</returns>
    public int GetHaveMedal()
    {
        return haveMedal;
    }

    /// <summary>
    /// ����SP�R�C�������̃Q�b�^�[
    /// </summary>
    /// <returns>����SP�R�C������</returns>
    public int GetSPCoin()
    {
        return haveSPCoin;
    }

    /// <summary>
    /// fPlofileList�̗v�f����Ԃ��Q�b�^�[
    /// </summary>
    /// <returns>NUNBER_OF_PLOFILELIST</returns>
    public int GetNumberOfPlofileList()
    {
        return NUMBER_OF_PLOFILELIST;
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
    /// �v���C���[�̃��x����Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public int GetPlayerLevel()
    {
        return playerLevel;
    }

    /// <summary>
    /// �v���C���[�̌o���l��Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public int GetPlayerExperience()
    {
        return playerExperience;
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
    /// �w�肵���p�[�c�̏����𒲂ׂĕԂ�
    /// </summary>
    /// <param name="partsKinds">�p�[�c�̎�ޔԍ�</param>
    /// <param name="partsNumber">�p�[�c�̗v�f�ԍ�</param>
    /// <returns></returns>
    public bool GetHaveProfileParts(int partsKinds, int partsNumber)
    {
        bool flg = false;

        switch (partsKinds)
        {
            case 0: flg = haveProfileParts.haveBackground[partsNumber]; break;
            case 1: flg = haveProfileParts.haveOutline[partsNumber]; break;
            case 2: flg = haveProfileParts.haveAccessory[partsNumber]; break;
            case 3: flg = haveProfileParts.haveWrinkle[partsNumber]; break;
            case 4: flg = haveProfileParts.haveEar[partsNumber]; break;
            case 5: flg = haveProfileParts.haveMouth[partsNumber]; break;
            case 6: flg = haveProfileParts.haveNose[partsNumber]; break;
            case 7: flg = haveProfileParts.haveEyebrow[partsNumber]; break;
            case 8: flg = haveProfileParts.haveEye[partsNumber]; break;
            case 9: flg = haveProfileParts.haveGlasses[partsNumber]; break;
            case 10: flg = haveProfileParts.haveHair[partsNumber]; break;
            default: break;
        }

        return flg;
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
    /// ���O�C���������X�V���A�ŏI���O�C�����������ɂ���
    /// </summary>
    public void UpdateLoginDays()
    {
        lastLoginDay = DateTime.Today;
        logInDays++;
        todaySNSShareFlg = false;

        save.commonData.lastLoginDay = lastLoginDay.ToString();
        save.commonData.logInDays = logInDays;
        save.commonData.todaySNSShareFlg = todaySNSShareFlg;

        SaveJson();
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
    /// ���x���ƌo���l�̒l���X�V����
    /// </summary>
    /// <param name="level">���x���̒l</param>
    /// <param name="exp">�o���l(���x����������)</param>
    public void UpdateLevelAndExp(int level, int exp)
    {
        playerLevel = level;
        playerExperience = exp;

        save.commonData.playerLevel = playerLevel;
        save.commonData.playerExperience = playerExperience;

        SaveJson();
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
    /// �f�o�b�O�{�^�����������Ƃ�
    /// </summary>
    public void PushDebugButton()
    {
        testMethod(1);
    }

    /// <summary>
    /// �w�肵�����ڂ̃X�R�A��Ԃ�
    /// </summary>
    /// <param name="gameNumber">�Q�[���̎�ޔԍ� 0:DropBall , 1:JanKen , 2:Pusher01 , 3:SmartBall01</param>
    /// <param name="kindNumber">�B���x�̔ԍ� 0:useMedals , 1:payMedals , 2:winNumber</param>
    /// <returns>save�ɕۑ�����Ă���w�肵�����ڂ̃X�R�A</returns>
    public int GetAchievementScore(int gameNumber, int kindNumber)
    {
        int score = 0;

        if(gameNumber == 0)
        {
            if (kindNumber == 0)
            {
                score = save.dropBallsData.shotTimes;
            }
            else if (kindNumber == 1)
            {
                score = save.dropBallsData.getMedalsTotal;
            }
            else if (kindNumber == 2)
            {
                score = save.dropBallsData.perfectClearTimes;
            }
            else
            {
                score = -1;
            }
        }
        else if (gameNumber == 1)
        {
            if (kindNumber == 0)
            {
                score = save.janKenData.battleTimes;
            }
            else if (kindNumber == 1)
            {
                score = save.janKenData.getMedalsTotal;
            }
            else if (kindNumber == 2)
            {
                score = save.janKenData.winTimes;
            }
            else
            {
                score = -1;
            }
        }
        else if (gameNumber == 2)
        {
            if (kindNumber == 0)
            {
                score = save.pusher01Data.betMedalTotal;
            }
            else if (kindNumber == 1)
            {
                score = save.pusher01Data.getMedalTotal;
            }
            else if (kindNumber == 2)
            {
                score = save.pusher01Data.bonusTimes;
            }
            else
            {
                score = -1;
            }
        }
        else if (gameNumber == 3)
        {
            if (kindNumber == 0)
            {
                score = save.smartBall01Data.betMedalTotal;
            }
            else if (kindNumber == 1)
            {
                score = save.smartBall01Data.getMedalTotal;
            }
            else if (kindNumber == 2)
            {
                score = save.smartBall01Data.maxLines;
            }
            else
            {
                score = -1;
            }
        }
        else
        {
            score = -1;
        }

        if (score < 0)
        {
            Debug.LogError($"PlayerInformationManager.GetAchievementScore :" +
                $" Error , gameNumber = {gameNumber} , kindNumber = {kindNumber}");
        }

        return score;
    }



}
