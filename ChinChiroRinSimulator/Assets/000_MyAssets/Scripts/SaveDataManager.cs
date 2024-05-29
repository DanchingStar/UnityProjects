using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveDataManager : MonoBehaviour
{
    private SaveData save;

    private string filePath;

    private const string DEFAULT_MYSET_NAME = "マイセット";

    public static SaveDataManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadJson();
    }

    private void SaveJson()
    {
        string json = JsonUtility.ToJson(save);
        StreamWriter streamWriter = new StreamWriter(filePath);
        streamWriter.Write(json);
        streamWriter.Flush();
        streamWriter.Close();
    }

    private void LoadJson()
    {
        filePath = Application.persistentDataPath + "/" + ".savedata.json";
        save = new SaveData();

        if (File.Exists(filePath))
        {
            StreamReader streamReader;
            streamReader = new StreamReader(filePath);
            string data = streamReader.ReadToEnd();
            streamReader.Close();
            save = JsonUtility.FromJson<SaveData>(data);

            Debug.Log($"SaveDataManager.LoadJson : Load SaveData\n{JsonUtility.ToJson(save, true)}");
        }
        else
        {
            save.saikoroMySet = new SaikoroMySet();
            save.saikoroMySet.myDice = new List<MyDice> { new MyDice() };
            save.saikoroMySet.myDice[0].name = DEFAULT_MYSET_NAME + "1";
            for (int i = 0; i < save.saikoroMySet.myDice[0].changeEyes.Length; i++)
            {
                save.saikoroMySet.myDice[0].changeEyes[i] = DiceController.DiceEyeKinds.None;
            }

            SaveJson();
            Debug.Log($"SaveDataManager.LoadJson : Not Found SaveData.\n{JsonUtility.ToJson(save, true)}");
        }
    }

    public void AddSaikoroMyset(string _mySetName)
    {
        MyDice addSaikoroOne = new MyDice();

        if (_mySetName == "")
        {
            addSaikoroOne.name = $"{DEFAULT_MYSET_NAME}{GetMyDiceCount() + 1}";
        }
        else
        {
            addSaikoroOne.name = _mySetName;
        }

        for (int i = 0; i < addSaikoroOne.changeEyes.Length; i++)
        {
            addSaikoroOne.changeEyes[i] = DiceController.DiceEyeKinds.None;
        }

        save.saikoroMySet.myDice.Add(addSaikoroOne);

        SaveJson();
    }

    public void SaveDice(int _diceNumber, MyDice _myDice)
    {
        save.saikoroMySet.myDice[_diceNumber] = _myDice;
        SaveJson();
    }

    /// <summary>
    /// サイコロのマイセットを返すゲッター
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public MyDice GetMyDice(int num)
    {
        if (num < save.saikoroMySet.myDice.Count && num >= 0)
        {
            return save.saikoroMySet.myDice[num];
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// サイコロマイセットの保存数を返すゲッター
    /// </summary>
    /// <returns></returns>
    public int GetMyDiceCount()
    {
        return save.saikoroMySet.myDice.Count;
    }

}
