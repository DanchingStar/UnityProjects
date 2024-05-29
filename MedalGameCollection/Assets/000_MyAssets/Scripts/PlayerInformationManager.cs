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
            // シーン遷移しても破棄されないようにする
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 二重で起動されないようにする
            Destroy(gameObject);
        }

    }

    private void Start()
    {
        // スタックトレースを有効にしてNativeArrayのメモリリークを探す
        NativeLeakDetection.Mode = NativeLeakDetectionMode.EnabledWithStackTrace;

        LoadProfileListDataBase();
        InitLoad();

        // test
        testMethod(0);
    }

    /// <summary>
    /// テストメソッド(デバッグ用)
    /// </summary>
    /// <param name="num"></param>
    private void testMethod(int num)
    {
        bool defaultFlg = false;
        switch (num)
        {
            case 1: // データの書き込み
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
            case 2: // データの読み込み
                LoadJson();
                break;
            case 3: // ファイルを削除
                DeleteSaveFile();
                break;
            case 4: // データを読み込んだ後、指定したデータのインクリメント
                LoadJson();
                IncrementField("dropBallsData", "perfectClearTimes");
                break;
            case 5: // ログを出力
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
    /// jsonファイルを読み込み、クラスの変数に値を代入する
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
                // 変換失敗
                lastLoginDay = DateTime.MinValue;
            }

            for (int i = 0; i < NUMBER_OF_PLOFILELIST; i++)
            {
                //設定している全Profile画像と位置と持っている全パーツをデータベースから変数に代入
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
    /// ProfileのListの全データベースを読み込む
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
    /// jsonファイルを保存する
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
    /// 復旧したJsonファイルを保存する
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
    /// jsonファイルを読み込む
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
    /// RarityがDefaultのパーツを取得する
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
    /// haveProfilePartsのサイズが合わないときにリサイズする
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
    /// ArraySizeCheckAndChange()の中身
    /// </summary>
    /// <param name="box">ref save.haveProfileParts.〇〇</param>
    /// <param name="number">パーツの種類の番号</param>
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
    /// jsonファイルを削除する
    /// </summary>
    public void DeleteSaveFile()
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    /// <summary>
    /// 全てのパーツ所持情報を初期化し、Defaultのアイテムだけ所持にする
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
    /// 指定したパーツ所持情報を初期化し、Defaultのアイテムだけ所持にする
    /// </summary>
    /// <param name="box">パーツ情報のアドレス</param>
    /// <param name="number">パーツの種類番号</param>
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
    /// 指定したパーツを取得する
    /// </summary>
    /// <param name="partsKinds">パーツの種類番号</param>
    /// <param name="partsNumber">パーツの要素番号</param>
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
    /// 指定した値を1増やす
    /// </summary>
    /// <param name="className">文字列 className</param>
    /// <param name="fieldName">文字列 fieldName</param>
    public void IncrementField(string className, string fieldName)
    {
        className = className.ToLower();
        fieldName = fieldName.ToLower();
        bool isValid = true;

        // 条件のフィールド名は全て小文字で記述する
        switch (className)
        {
            case "commondata":
                switch (fieldName)
                {
                    case "logindays":
                        save.commonData.logInDays++;
                        break;
                    // 他のフィールドについても同様に処理する
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
    /// 指定した値を指定した数だけ増やす
    /// </summary>
    /// <param name="className">文字列 className</param>
    /// <param name="fieldName">文字列 fieldName</param>
    /// <param name="addNumber">増やす値</param>
    public void AddField(string className, string fieldName,int addNumber)
    {
        className = className.ToLower();
        fieldName = fieldName.ToLower();
        bool isValid = true;

        // 条件のフィールド名は全て小文字で記述する
        switch (className)
        {
            case "commondata":
                switch (fieldName)
                {
                    case "playerexperience":
                        save.commonData.playerExperience += addNumber;
                        break;
                    // 他のフィールドについても同様に処理する
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
    /// 条件を満たしたとき、指定した値を指定した値に更新する
    /// </summary>
    /// <param name="className">文字列 className</param>
    /// <param name="fieldName">文字列 fieldName</param>
    /// <param name="updateNumber">更新する値</param>
    public void UpdateField(string className, string fieldName, int updateNumber)
    {
        className = className.ToLower();
        fieldName = fieldName.ToLower();
        bool isValid = true;

        // 条件のフィールド名は全て小文字で記述する
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
                    // 他のフィールドについても同様に処理する
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
    /// メダルを消費する。足りない場合はfalseを返す
    /// </summary>
    /// <param name="numberOfMedals">使うメダルの枚数</param>
    /// <returns>消費が可能ならtrue、メダル不足ならfalse</returns>
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
    /// メダルを獲得する
    /// </summary>
    /// <param name="numberOfMedals">獲得するメダルの枚数</param>
    public void AcquisitionMedal(int numberOfMedals)
    {
        if (numberOfMedals == 0) return;

        haveMedal += numberOfMedals;
        SaveHaveMedal();
    }

    /// <summary>
    /// SPコインを消費する。足りない場合はfalseを返す
    /// </summary>
    /// <param name="numberOfCoins">使うSPコインの枚数</param>
    /// <returns>消費が可能ならtrue、SPコイン不足ならfalse</returns>
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
    /// SPコインを獲得する
    /// </summary>
    /// <param name="numberOfCoins">獲得するSPコインの枚数</param>
    public void AcquisitionSPCoin(int numberOfCoins)
    {
        haveSPCoin += numberOfCoins;
        SaveHaveSPCoin();
    }

    /// <summary>
    /// プレイヤー名を変更して、jsonファイルに保存する
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
    /// メダル枚数を更新して、jsonファイルに保存する
    /// </summary>
    public void SaveHaveMedal()
    {
        save.commonData.haveMedal = haveMedal;
        SaveJson();
    }

    /// <summary>
    /// SPコインの枚数を更新して、jsonファイルに保存する
    /// </summary>
    public void SaveHaveSPCoin()
    {
        save.commonData.haveSPCoin = haveSPCoin;
        SaveJson();
    }

    /// <summary>
    /// プロフィール情報を更新して、jsonファイルに保存する
    /// </summary>
    /// <param name="spriteNumber">設定している画像のリスト番号の配列</param>
    /// <param name="posX">設定しているポジションX(H)の配列</param>
    /// <param name="posY">設定しているポジションX(H)の配列</param>
    public void SaveSetProfile(int[] spriteNumber, int[] posX, int[] posY)
    {
        for (int i = 0; i < NUMBER_OF_PLOFILELIST; i++)
        {
            // 読み込んだデータを更新
            settingPartsNumber[i] = spriteNumber[i];
            settingPartsPositionH[i] = posX[i];
            settingPartsPositionV[i] = posY[i];

            // jsonファイルの中身を更新
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

        // 更新を保存
        SaveJson();
    }

    /// <summary>
    /// 所持メダル枚数のゲッター
    /// </summary>
    /// <returns>所持メダル枚数</returns>
    public int GetHaveMedal()
    {
        return haveMedal;
    }

    /// <summary>
    /// 所持SPコイン枚数のゲッター
    /// </summary>
    /// <returns>所持SPコイン枚数</returns>
    public int GetSPCoin()
    {
        return haveSPCoin;
    }

    /// <summary>
    /// fPlofileListの要素数を返すゲッター
    /// </summary>
    /// <returns>NUNBER_OF_PLOFILELIST</returns>
    public int GetNumberOfPlofileList()
    {
        return NUMBER_OF_PLOFILELIST;
    }

    /// <summary>
    /// プレイヤーの名前を返すゲッター
    /// </summary>
    /// <returns></returns>
    public string GetPlayerName()
    {
        return playerName;
    }

    /// <summary>
    /// プレイヤーのレベルを返すゲッター
    /// </summary>
    /// <returns></returns>
    public int GetPlayerLevel()
    {
        return playerLevel;
    }

    /// <summary>
    /// プレイヤーの経験値を返すゲッター
    /// </summary>
    /// <returns></returns>
    public int GetPlayerExperience()
    {
        return playerExperience;
    }

    /// <summary>
    /// 新しいプレイヤーであるかのフラグを返すゲッター
    /// </summary>
    /// <returns></returns>
    public bool GetNewPlayerFlg()
    {
        return newPlayerFlg;
    }

    /// <summary>
    /// 指定したパーツの所持を調べて返す
    /// </summary>
    /// <param name="partsKinds">パーツの種類番号</param>
    /// <param name="partsNumber">パーツの要素番号</param>
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
    /// 最終ログイン日を返すゲッター
    /// </summary>
    /// <returns></returns>
    public DateTime GetLastLoginDay()
    {
        return lastLoginDay;
    }

    /// <summary>
    /// 本日のSNSシェア済みかのフラグを返すゲッター
    /// </summary>
    /// <returns></returns>
    public bool GetTodaySNSShareFlg()
    {
        return todaySNSShareFlg;
    }

    /// <summary>
    /// ログイン日数を返すゲッター
    /// </summary>
    /// <returns></returns>
    public int GetLoginDays()
    {
        return logInDays;
    }

    /// <summary>
    /// ログイン日数を更新し、最終ログイン日を今日にする
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
    /// 本日のSNSシェアを済みにし、更新して保存する
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
    /// Reward広告を見た回数をインクリメントして更新する
    /// </summary>
    public void UpdateRewardTimes()
    {
        rewardTimes++;
        save.commonData.rewardTimes = rewardTimes;
        SaveJson();
    }

    /// <summary>
    /// レベルと経験値の値を更新する
    /// </summary>
    /// <param name="level">レベルの値</param>
    /// <param name="exp">経験値(レベル分を除く)</param>
    public void UpdateLevelAndExp(int level, int exp)
    {
        playerLevel = level;
        playerExperience = exp;

        save.commonData.playerLevel = playerLevel;
        save.commonData.playerExperience = playerExperience;

        SaveJson();
    }

    /// <summary>
    /// saveのゲッター
    /// </summary>
    /// <returns></returns>
    public SaveData GetSaveData()
    {
        return save;
    }

    /// <summary>
    /// デバッグボタンを押したとき
    /// </summary>
    public void PushDebugButton()
    {
        testMethod(1);
    }

    /// <summary>
    /// 指定した項目のスコアを返す
    /// </summary>
    /// <param name="gameNumber">ゲームの種類番号 0:DropBall , 1:JanKen , 2:Pusher01 , 3:SmartBall01</param>
    /// <param name="kindNumber">達成度の番号 0:useMedals , 1:payMedals , 2:winNumber</param>
    /// <returns>saveに保存されている指定した項目のスコア</returns>
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
