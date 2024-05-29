using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using TMPro;

public class TestSceneManager : MonoBehaviour
{
    [SerializeField] private SheetMusicListGenerator sheetMusicListGenerator;
    [SerializeField] private StageClearModeListGenerator stageClearModeListGenerator;
    [SerializeField] private TextMeshProUGUI musicTitleText;

    private int listIndex;
    private int listLength;

    private AudioSource audioSource;

    public static TestSceneManager Instance;
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
        audioSource = GetComponent<AudioSource>();
        listLength = sheetMusicListGenerator.GetLength();
        TestBgm(0);
    }

    /// <summary>
    /// テストボタンを押したとき
    /// </summary>
    /// <param name="number">テスト番号</param>
    public void PushTestButton(int number)
    {
        switch (number)
        {
            case 0:
                CheckSheetMusicFiles();
                break;
            case 1:
                CheckSeetMusicTargetTime();
                break;
            default:
                break;
        }
    }


    public void PushTestLeftButton()
    {
        listIndex = listIndex - 1 < 0 ? listLength - 1 : listIndex - 1;
        TestBgm(listIndex);
    }

    public void PushTestRightButton()
    {
        listIndex = listIndex + 1 >= listLength ? 0 : listIndex + 1;
        TestBgm(listIndex);
    }

    private void TestBgm(int num)
    {
        audioSource.Stop();
        if (num == 0)
        {
            musicTitleText.text = "None";
            audioSource.clip = null;
        }
        else
        {
            musicTitleText.text = $"[{listIndex}]{sheetMusicListGenerator.GetTitleName(num)}";
            audioSource.clip = sheetMusicListGenerator.GetSuccessMusicClip(num);
            audioSource.Play();
        }
    }

    /// <summary>
    /// 全ての楽譜の詳細を表示する
    /// </summary>
    private void CheckSheetMusicFiles()
    {
        string str = "";

        for (int j = 0; j < sheetMusicListGenerator.GetLength(); j++) 
        {
            TextAsset textfile = sheetMusicListGenerator.GetSheetMusicFile(j);
            StringReader stackLevelCostReader = new StringReader(textfile.text);
            string text = stackLevelCostReader.ReadToEnd();
            string[] allText = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            List<XylophoneManager.NotesAnother> notes2 = new List<XylophoneManager.NotesAnother>();
            List<XylophoneManager.NotesAnother> notes3 = new List<XylophoneManager.NotesAnother>();
            int notesCount = 0;
            int targetTime = 0;
            int i = 0;

            foreach (var item in allText)
            {
                if (item == "")
                {
                }
                else
                {
                    if (i == 0)
                    {
                        str += $"[{j.ToString().PadLeft(3, '0')}] {item}";
                    }
                    else if (i == 1)
                    {
                        str += $" ( Beat : {item} , ";
                    }
                    else if (i == 2)
                    {
                        str += $"Tempo : {item} )\n";
                    }
                    else
                    {
                        if (item == "END") break;

                        string[] nowNoteString = item.Split(',');

                        XylophoneManager.NotesAnother nowNotes;

                        if (Enum.TryParse(nowNoteString[0], out XylophoneManager.Notes auth))
                        {
                            nowNotes = (XylophoneManager.NotesAnother)auth;
                        }
                        else if (Enum.TryParse(nowNoteString[0], out XylophoneManager.NotesAnother authAnother))
                        {
                            nowNotes = authAnother;
                        }
                        else
                        {
                            Debug.LogError($"Not Find Notes Name -> {nowNoteString[0]}");
                            nowNotes = XylophoneManager.NotesAnother.Rest;
                        }

                        if (nowNotes.ToString().Length ==2)
                        {
                            bool flg = false;
                            foreach(var xxx in notes2)
                            {
                                if(xxx == nowNotes)
                                {
                                    flg = true;
                                    break;
                                }
                            }
                            if (!flg)
                            {
                                notes2.Add(nowNotes);
                            }
                        }
                        else if (nowNotes.ToString().Length == 3)
                        {
                            bool flg = false;
                            foreach (var xxx in notes3)
                            {
                                if (xxx == nowNotes)
                                {
                                    flg = true;
                                    break;
                                }
                            }
                            if (!flg)
                            {
                                notes3.Add(nowNotes);
                            }
                        }
                        else
                        {
                            Debug.LogError($"LENGTH ERROR : nowNotes = {nowNotes} ({nowNotes.ToString().Length})");
                        }

                        notesCount++;
                    }

                    i++;
                }

            }

            if (notesCount > 45) targetTime = 100;
            else if (notesCount > 40) targetTime = 90;
            else if (notesCount > 35) targetTime = 80;
            else if (notesCount > 30) targetTime = 70;
            else if (notesCount > 25) targetTime = 60;
            else if (notesCount > 20) targetTime = 50;
            else if (notesCount > 15) targetTime = 40;
            else targetTime = 30;

            notes2.Sort();
            notes3.Sort();
            str += $"\tNotesCount : {notesCount}\n";
            str += $"\tTargetTime : {targetTime}\n";
            str += $"\tNotesKinds : {notes2.Count + notes3.Count}\n";
            XylophoneManager.NotesAnother rangeMax;
            XylophoneManager.NotesAnother rangeMin;
            if (notes2.Count == 0 && notes3.Count != 0)
            {
                rangeMax = notes3[notes3.Count - 1];
                rangeMin = notes3[0];
            }
            else if (notes2.Count != 0 && notes3.Count == 0)
            {
                rangeMax = notes2[notes2.Count - 1];
                rangeMin = notes2[0];
            }
            else
            {
                rangeMax = notes2[notes2.Count - 1] > notes3[notes3.Count - 1] ? notes2[notes2.Count - 1] : notes3[notes3.Count - 1];
                rangeMin = notes2[0] < notes3[0] ? notes2[0] : notes3[0];
            }
            str += $"\tNotesRange : {(XylophoneManager.Notes)rangeMin} - {(XylophoneManager.Notes)rangeMax}\n";
            str += $"\tNotes[Under] : ";
            str += notes2.Count == 0 ? $"[Nothing]" : string.Join(", ", notes2);
            str += $"\n\tNotes[Over] : ";
            str += notes3.Count == 0 ? $"[Nothing]" : string.Join(", ", notes3);
            str += $"\n\n";
        }

        Debug.Log(str);
    }

    /// <summary>
    /// 各スコアの目標タイムをチェックする
    /// </summary>
    private void CheckSeetMusicTargetTime()
    {
        string str = "";

        for (int i = 0; i < stageClearModeListGenerator.GetListLength(PlayerInformationManager.GameLevel.Normal); i++)
        {
            string title = stageClearModeListGenerator.GetSheetMusicName(PlayerInformationManager.GameLevel.Normal, i);
            int valueN = (int)stageClearModeListGenerator.GetTargetTime(PlayerInformationManager.GameLevel.Normal, i);
            int valueH = (int)stageClearModeListGenerator.GetTargetTime(PlayerInformationManager.GameLevel.Hard, i);
            int valueV = (int)stageClearModeListGenerator.GetTargetTime(PlayerInformationManager.GameLevel.VeryHard, i);
            bool flg1 = valueN == valueH ? true : false;
            bool flg2 = valueN - valueV == 10 ? true : false;
            str += $"[{(i + 1).ToString().PadLeft(3, '0')}] {title}\n\tN : {valueN} , H : {valueH} , V : {valueV}\n\t{flg1} , {flg2}\n\n";
        }

        Debug.Log(str);
    }


}
