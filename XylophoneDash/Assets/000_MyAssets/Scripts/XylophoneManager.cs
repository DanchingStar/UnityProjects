using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XylophoneManager : MonoBehaviour
{
    [SerializeField] private Transform xylophoneSoundsTransform;

    [SerializeField] private NotesKeyboard[] keyboards;

    private AudioSource[] notesSound;

    private int notesSize;

    private string[,] notesName;

    /// <summary>
    /// �m�[�c�̖��O
    /// </summary>
    public enum Notes
    {
        None,
        Xy45,
        Xy46,
        Xy47,
        Xy48,
        Xy49,
        Xy50,
        Xy51,
        Xy52,
        Xy53,
        Xy54,
        Xy55,
        Xy56,
        Xy57,
        Xy58,
        Xy59,
        Xy60,
        Xy61,
        Xy62,
        Xy63,
        Xy64,
        Xy65,
        Xy66,
        Xy67,
        Xy68,
        Xy69,
        Xy70,
        Xy71,
        Xy72,
        Xy73,
        Xy74,
        Xy75,
        Xy76,
    }

    /// <summary>
    /// �m�[�c�̖��O(�y���t�@�C���p�̕ʃo�[�W����)
    /// </summary>
    public enum NotesAnother
    {
        Rest,
        F4,
        Fs4,
        G4,
        Gs4,
        A4,
        As4,
        B4,
        C5,
        Cs5,
        D5,
        Ds5,
        E5,
        F5,
        Fs5,
        G5,
        Gs5,
        A5,
        As5,
        B5,
        C6,
        Cs6,
        D6,
        Ds6,
        E6,
        F6,
        Fs6,
        G6,
        Gs6,
        A6,
        As6,
        B6,
        C7,
    }


    public static XylophoneManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // ��d�ŋN������Ȃ��悤�ɂ���
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        notesSize = Enum.GetValues(typeof(Notes)).Length;
        notesSound = new AudioSource[notesSize];
        notesName = new string[notesSize, Enum.GetValues(typeof(PlayerInformationManager.DisplayNotesKind)).Length];

        for (int i = 0; i < notesSize; i++)
        {
            string itemName = Enum.GetName(typeof(Notes), (Notes)i);
            Transform tf = xylophoneSoundsTransform.Find(itemName);
            if (tf != null)
            {
                notesSound[i] = tf.gameObject.GetComponent<AudioSource>();
            }
            else
            {
                Debug.LogError($"XylophoneManager.Start : Not Find Item Name -> {itemName}");
            }
        }

        InitNotesName();
    }

    /// <summary>
    /// �w�肵���m�[�c�̉���炷
    /// </summary>
    /// <param name="notes"></param>
    public void PlayNotes(Notes notes)
    {
        if (notesSound[(int)notes] == null) return;

        notesSound[(int)notes].Play();
    }

    /// <summary>
    /// �m�[�c�̃T�C�Y��Ԃ�
    /// </summary>
    /// <returns></returns>
    public int GetNotesSize()
    {
        return notesSize;
    }

    /// <summary>
    /// ���K���̃��X�g�����(������)
    /// </summary>
    private void InitNotesName()
    {
        string testStr = "< Notes Name List >\n";

        for (int i = 0; i < notesSize; i++)
        {
            for (int j = 0; j < Enum.GetValues(typeof(PlayerInformationManager.DisplayNotesKind)).Length; j++)
            {
                if ((Notes)i == Notes.None)
                {
                    notesName[i, j] = "";
                }
                else if ((PlayerInformationManager.DisplayNotesKind)j == PlayerInformationManager.DisplayNotesKind.None)
                {
                    notesName[i, j] = "";
                }
                else if ((PlayerInformationManager.DisplayNotesKind)j == PlayerInformationManager.DisplayNotesKind.KeyboardNumber)
                {
                    string str = Enum.GetName(typeof(Notes), (Notes)i);
                    notesName[i, j] = str.Substring(2, 2);
                }
                else if ((PlayerInformationManager.DisplayNotesKind)j == PlayerInformationManager.DisplayNotesKind.Japanese)
                {
                    string str = "";

                    if ((Notes)i == Notes.Xy45) str = "�t�@4";
                    else if ((Notes)i == Notes.Xy46) str = "�t�@#4";
                    else if ((Notes)i == Notes.Xy47) str = "�\4";
                    else if ((Notes)i == Notes.Xy48) str = "�\#4";
                    else if ((Notes)i == Notes.Xy49) str = "��4";
                    else if ((Notes)i == Notes.Xy50) str = "��#4";
                    else if ((Notes)i == Notes.Xy51) str = "�V4";
                    else if ((Notes)i == Notes.Xy52) str = "�h5";
                    else if ((Notes)i == Notes.Xy53) str = "�h#5";
                    else if ((Notes)i == Notes.Xy54) str = "��5";
                    else if ((Notes)i == Notes.Xy55) str = "��#5";
                    else if ((Notes)i == Notes.Xy56) str = "�~5";
                    else if ((Notes)i == Notes.Xy57) str = "�t�@5";
                    else if ((Notes)i == Notes.Xy58) str = "�t�@#5";
                    else if ((Notes)i == Notes.Xy59) str = "�\5";
                    else if ((Notes)i == Notes.Xy60) str = "�\#5";
                    else if ((Notes)i == Notes.Xy61) str = "��5";
                    else if ((Notes)i == Notes.Xy62) str = "��#5";
                    else if ((Notes)i == Notes.Xy63) str = "�V5";
                    else if ((Notes)i == Notes.Xy64) str = "�h6";
                    else if ((Notes)i == Notes.Xy65) str = "�h#6";
                    else if ((Notes)i == Notes.Xy66) str = "��6";
                    else if ((Notes)i == Notes.Xy67) str = "��#6";
                    else if ((Notes)i == Notes.Xy68) str = "�~6";
                    else if ((Notes)i == Notes.Xy69) str = "�t�@6";
                    else if ((Notes)i == Notes.Xy70) str = "�t�@#6";
                    else if ((Notes)i == Notes.Xy71) str = "�\6";
                    else if ((Notes)i == Notes.Xy72) str = "�\#6";
                    else if ((Notes)i == Notes.Xy73) str = "��6";
                    else if ((Notes)i == Notes.Xy74) str = "��#6";
                    else if ((Notes)i == Notes.Xy75) str = "�V6";
                    else if ((Notes)i == Notes.Xy76) str = "�h7";
                    else str = "Error !!";

                    notesName[i, j] = str;
                }
                else if ((PlayerInformationManager.DisplayNotesKind)j == PlayerInformationManager.DisplayNotesKind.English)
                {
                    string str = "";

                    if ((Notes)i == Notes.Xy45) str = "F4";
                    else if ((Notes)i == Notes.Xy46) str = "F#4";
                    else if ((Notes)i == Notes.Xy47) str = "G4";
                    else if ((Notes)i == Notes.Xy48) str = "G#4";
                    else if ((Notes)i == Notes.Xy49) str = "A4";
                    else if ((Notes)i == Notes.Xy50) str = "A#4";
                    else if ((Notes)i == Notes.Xy51) str = "B4";
                    else if ((Notes)i == Notes.Xy52) str = "C5";
                    else if ((Notes)i == Notes.Xy53) str = "C#5";
                    else if ((Notes)i == Notes.Xy54) str = "D5";
                    else if ((Notes)i == Notes.Xy55) str = "D#5";
                    else if ((Notes)i == Notes.Xy56) str = "E5";
                    else if ((Notes)i == Notes.Xy57) str = "F5";
                    else if ((Notes)i == Notes.Xy58) str = "F#5";
                    else if ((Notes)i == Notes.Xy59) str = "G5";
                    else if ((Notes)i == Notes.Xy60) str = "G#5";
                    else if ((Notes)i == Notes.Xy61) str = "A5";
                    else if ((Notes)i == Notes.Xy62) str = "A#5";
                    else if ((Notes)i == Notes.Xy63) str = "B5";
                    else if ((Notes)i == Notes.Xy64) str = "C6";
                    else if ((Notes)i == Notes.Xy65) str = "C#6";
                    else if ((Notes)i == Notes.Xy66) str = "D6";
                    else if ((Notes)i == Notes.Xy67) str = "D#6";
                    else if ((Notes)i == Notes.Xy68) str = "E6";
                    else if ((Notes)i == Notes.Xy69) str = "F6";
                    else if ((Notes)i == Notes.Xy70) str = "F#6";
                    else if ((Notes)i == Notes.Xy71) str = "G6";
                    else if ((Notes)i == Notes.Xy72) str = "G#6";
                    else if ((Notes)i == Notes.Xy73) str = "A6";
                    else if ((Notes)i == Notes.Xy74) str = "A#6";
                    else if ((Notes)i == Notes.Xy75) str = "B6";
                    else if ((Notes)i == Notes.Xy76) str = "C7";
                    else str = "Error !!";

                    notesName[i, j] = str;
                }
                //else if ((GameSceneManager.DisplayNotesKind)j == GameSceneManager.DisplayNotesKind.Deutsch)
                //{
                //    string str = "";

                //    if ((Notes)i == Notes.Xy45) str = "f1";
                //    else if ((Notes)i == Notes.Xy46) str = "fis1";
                //    else if ((Notes)i == Notes.Xy47) str = "g1";
                //    else if ((Notes)i == Notes.Xy48) str = "gis1";
                //    else if ((Notes)i == Notes.Xy49) str = "a1";
                //    else if ((Notes)i == Notes.Xy50) str = "ais1";
                //    else if ((Notes)i == Notes.Xy51) str = "h1";
                //    else if ((Notes)i == Notes.Xy52) str = "c2";
                //    else if ((Notes)i == Notes.Xy53) str = "cis2";
                //    else if ((Notes)i == Notes.Xy54) str = "d2";
                //    else if ((Notes)i == Notes.Xy55) str = "dis2";
                //    else if ((Notes)i == Notes.Xy56) str = "e2";
                //    else if ((Notes)i == Notes.Xy57) str = "f2";
                //    else if ((Notes)i == Notes.Xy58) str = "fis2";
                //    else if ((Notes)i == Notes.Xy59) str = "g2";
                //    else if ((Notes)i == Notes.Xy60) str = "gis2";
                //    else if ((Notes)i == Notes.Xy61) str = "a2";
                //    else if ((Notes)i == Notes.Xy62) str = "ais2";
                //    else if ((Notes)i == Notes.Xy63) str = "h2";
                //    else if ((Notes)i == Notes.Xy64) str = "c3";
                //    else if ((Notes)i == Notes.Xy65) str = "cis3";
                //    else if ((Notes)i == Notes.Xy66) str = "d3";
                //    else if ((Notes)i == Notes.Xy67) str = "dis3";
                //    else if ((Notes)i == Notes.Xy68) str = "e3";
                //    else if ((Notes)i == Notes.Xy69) str = "f3";
                //    else if ((Notes)i == Notes.Xy70) str = "fis3";
                //    else if ((Notes)i == Notes.Xy71) str = "g3";
                //    else if ((Notes)i == Notes.Xy72) str = "gis3";
                //    else if ((Notes)i == Notes.Xy73) str = "a3";
                //    else if ((Notes)i == Notes.Xy74) str = "ais3";
                //    else if ((Notes)i == Notes.Xy75) str = "h3";
                //    else if ((Notes)i == Notes.Xy76) str = "c4";
                //    else str = "Error !!";

                //    notesName[i, j] = str;
                //}
                else
                {
                    notesName[i, j] = "Error";
                    Debug.LogError($"XylophoneManager.InitNotesName : Number Error -> {i},{j}");
                }

                testStr += $"  {i.ToString().PadLeft(2, '0')},{j.ToString().PadLeft(2, '0')} : {notesName[i, j]}\n";
            }
        }

        // ���K���̃��X�g���m�F���邽�߂̃��O
        // Debug.Log($"XylophoneManager.InitNotesName\n{testStr}");
    }

    /// <summary>
    /// �w�肳�ꂽ���K����Ԃ�
    /// </summary>
    /// <param name="notes"></param>
    /// <param name="notesKind"></param>
    /// <returns></returns>
    public string GetNotesName(Notes notes)
    {
        return notesName[(int)notes, (int)(PlayerInformationManager.Instance.GetSettingDisplayNotesKind())];
    }

    /// <summary>
    /// �w�肵���m�[�c�̐F��ς���
    /// </summary>
    /// <param name="notes"></param>
    public void ChangeColorNotesKeyboad(Notes notes,Color color)
    {
        if (keyboards[(int)notes] == null) return;

        keyboards[(int)notes].ChangeMaterialColor(color);
    }

    /// <summary>
    /// �S�Ẵl�N�X�g�t���O��False�ɕς�����A�w�肵���m�[�c�̃l�N�X�g�t���O��True�ɕς���
    /// </summary>
    /// <param name="notes"></param>
    public void UpdateNextNotesFlg(Notes notes)
    {
        foreach (var item in keyboards)
        {
            if (item != null)
            {
                item.SetNextNotesFlg(false);
            }
        }

        if (keyboards[(int)notes] != null)
        {
            keyboards[(int)notes].SetNextNotesFlg(true);
        }
    }


}
