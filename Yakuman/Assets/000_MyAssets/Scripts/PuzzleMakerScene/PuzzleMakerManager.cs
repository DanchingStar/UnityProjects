using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class PuzzleMakerManager : MonoBehaviour
{
    [SerializeField] private UiManagerForPuzzleMaker uiManager;

    [SerializeField] private PositionButtonForPuzzleMaker[] positionButtonForPuzzleMakers;

    [Header("\n[Load Text File]")]
    [SerializeField] private TextAsset loadTextFile;

    [Header("\n[Save Text File]")]
    [SerializeField] private TextAsset saveTextFile;

    private PaiButtonForPuzzleMaker[] paiButtonForPuzzleMakers;

    private int[] zaikoOfPaiKinds;

    private int activeIndex;

    private MahjongManager.PlayerKind oyaPlayer;
    private CpuParent[] cpus;
    private bool[] cpuNakiActives;

    private bool startFinifhFlg = false;

    private const int NO_SELECT = -100;

    public static PuzzleMakerManager Instance;
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
        if (startFinifhFlg) return;
        startFinifhFlg = true;

        int totalCounter = GetZaikoTotalCounter();

        for (int i = 0; i < positionButtonForPuzzleMakers.Length; i++)
        {
            positionButtonForPuzzleMakers[i].SetMyIndex(i);
            positionButtonForPuzzleMakers[i].SetMyPaiKind(MahjongManager.PaiKinds.None_00);
        }

        ReceptionPositionButton(NO_SELECT);

        paiButtonForPuzzleMakers = new PaiButtonForPuzzleMaker[38];

        uiManager.ChangeTotalText(totalCounter);

        SetOyaPlayer(MahjongManager.PlayerKind.Player);
        cpus = new CpuParent[3];
        cpuNakiActives = new bool[3];
        for(int i = 0;i < cpus.Length; i++)
        {
            SetCpu(i, GameModeManager.Instance.GetCpuContentGenerator().GetCpuChild(0));
            SetCpuNaki(i, true);
        }
    }

    public void InitPaiButton(PaiButtonForPuzzleMaker _paiButton, MahjongManager.PaiKinds _paiKind)
    {
        Start();

        paiButtonForPuzzleMakers[(int)_paiKind] = _paiButton;
    }

    public void ReceptionPositionButton(int _index)
    {
        Start();

        activeIndex = _index;
        for (int i = 0; i < positionButtonForPuzzleMakers.Length; i++)
        {
            positionButtonForPuzzleMakers[i].ReceptionPuzzleMakerManager(activeIndex);
        }

    }

    public void ReceptionPositionNoSelectButton()
    {
        ReceptionPositionButton(NO_SELECT);
    }

    public void ReceptionPaiButton(PaiButtonForPuzzleMaker _paiButtonForPuzzleMaker)
    {
        MahjongManager.PaiKinds newPaiKind = _paiButtonForPuzzleMaker.getMyPaiKinds();

        SettingPai(newPaiKind);
    }

    private void SettingPai(MahjongManager.PaiKinds _newPaiKind)
    {
        if (activeIndex == NO_SELECT) // �ǂ����w�肳��Ă��Ȃ��Ƃ�
        {

        }
        else // �ǂ�������w�肳��Ă���Ƃ�
        {
            MahjongManager.PaiKinds positionSelectKind = positionButtonForPuzzleMakers[activeIndex].GetMySelectPaiKind();

            if (zaikoOfPaiKinds[(int)_newPaiKind] > 0)
            {
                positionButtonForPuzzleMakers[activeIndex].SetMyPaiKind(_newPaiKind);
                zaikoOfPaiKinds[(int)_newPaiKind] -= 1;

                if (positionSelectKind != MahjongManager.PaiKinds.None_00)
                {
                    zaikoOfPaiKinds[(int)positionSelectKind] += 1;
                }
            }
            else // �v�̍݌ɂ��Ȃ��Ƃ�
            {
                // ToDo
            }
            UpdatePaiCounterTexts();
        }
    }

    public void ReceptionDeleteButton()
    {
        if (activeIndex == NO_SELECT) // �ǂ����w�肳��Ă��Ȃ��Ƃ�
        {

        }
        else // �ǂ�������w�肳��Ă���Ƃ�
        {
            MahjongManager.PaiKinds positionSelectKind = positionButtonForPuzzleMakers[activeIndex].GetMySelectPaiKind();

            if (positionSelectKind != MahjongManager.PaiKinds.None_00)
            {
                positionButtonForPuzzleMakers[activeIndex].SetMyPaiKind(MahjongManager.PaiKinds.None_00);
                zaikoOfPaiKinds[(int)positionSelectKind] += 1;
            }
            UpdatePaiCounterTexts();
        }
    }

    private void UpdatePaiCounterTexts()
    {
        int totalCounter = 0;
        for (int i=0;i<zaikoOfPaiKinds.Length;i++)
        {
            if (i % 10 != 0)
            {
                int zaiko = zaikoOfPaiKinds[i];
                paiButtonForPuzzleMakers[i].ChangeZaikoText(zaiko);
                totalCounter += zaiko;
            }
        }
        uiManager.ChangeTotalText(totalCounter);
    }

    public int GetZaikoCounter(MahjongManager.PaiKinds _paiKinds)
    {
        Start();

        return zaikoOfPaiKinds[(int)_paiKinds];
    }

    public int GetZaikoTotalCounter()
    {
        int totalCounter = 0;
        zaikoOfPaiKinds = new int[38];
        for (int i = 0; i < zaikoOfPaiKinds.Length; i++)
        {
            int num = i % 10 == 0 ? 0 : 4;
            zaikoOfPaiKinds[i] = num;
            totalCounter += num;
        }

        return totalCounter;
    }

    public string GetDataString()
    {
        string resultStr = "";

        resultStr += $"{oyaPlayer}";
        resultStr += ";\n";

        for (int i = 0; i < cpus.Length; i++)
        {
            resultStr += $"{cpus[i].GetName()},";
        }
        resultStr += ";\n";

        for (int i = 0; i < cpuNakiActives.Length; i++)
        {
            resultStr += $"{cpuNakiActives[i]},";
        }
        resultStr += ";\n";

        for (int i=0; i< positionButtonForPuzzleMakers.Length; i++)
        {
            var paiKind = positionButtonForPuzzleMakers[i].GetMySelectPaiKind();
            resultStr += $"{paiKind},";
        }
        resultStr += ";\n";

        return resultStr;
    }

    public void SetOyaPlayer(MahjongManager.PlayerKind _oyaPlayer)
    {
        oyaPlayer = _oyaPlayer;
    }

    public void SetCpu(int _cpuIndex, CpuParent _cpu)
    {
        cpus[_cpuIndex] = _cpu;
    }

    public void SetCpuNaki(int _cpuIndex, bool _flg)
    {
        cpuNakiActives[_cpuIndex] = _flg;
    }

    /// <summary>
    /// �e�L�X�g�t�@�C����ǂݍ��݁AUI�ɔ��f������
    /// </summary>
    public void LoadTextFile()
    {
        StringReader stackLevelCostReader = new StringReader(loadTextFile.text);
        string text = stackLevelCostReader.ReadToEnd();

        string[] allText = text.Split(';');

        int i = 0;
        foreach (var item in allText)
        {
            // ���s���J�b�g����
            string itemStr = CutEnter(item);

            if (string.IsNullOrEmpty(itemStr))
            {
                // �������Ȃ�
            }
            else if (itemStr.StartsWith("#"))
            {
                // �������Ȃ�
            }
            else
            {
                if (i == 0) // �e
                {
                    if (Enum.TryParse(itemStr, out MahjongManager.PlayerKind oyaKouho))
                    {
                        if (MahjongManager.PlayerKind.Player <= oyaKouho && oyaKouho <= MahjongManager.PlayerKind.Kamicha)
                        {
                            SetOyaPlayer(oyaKouho);
                            uiManager.ChangeOyaForLoadText((int)(oyaKouho) - 1);
                        }
                        else
                        {
                            Debug.LogError($"LoadPuzzle : Error , i = {i}");
                        }
                    }
                }
                else if (i == 1) // CPU�̖��O
                {
                    string[] str = itemStr.Split(',');

                    int counter = 0;
                    foreach (var value in str)
                    {
                        if (string.IsNullOrEmpty(value))
                        {
                            continue;
                        }
                        else
                        {
                            CpuContentGenerator cpuContentGenerator = GameModeManager.Instance.GetCpuContentGenerator();
                            bool errorFlg = true;
                            for (int j = 0; j < cpuContentGenerator.GetLength(); j++)
                            {
                                CpuParent cpuChild = cpuContentGenerator.GetCpuChild(j);
                                if (cpuChild.GetName() == value)
                                {
                                    SetCpu(counter, cpuChild);
                                    uiManager.ChangeCpuNameForLoadText(counter, j);
                                    errorFlg = false;
                                    break;
                                }
                            }

                            if (errorFlg)
                            {
                                Debug.LogError($"LoadPuzzle : Error , i = {i}\ncounter = {counter} , value = {value}");
                            }
                            else
                            {
                                counter++;
                            }
                        }
                    }
                    if (counter != 3)
                    {
                        Debug.LogError($"LoadPuzzle : Error , i = {i}\ncounter = {counter}");
                    }
                }
                else if (i == 2) // CPU������
                {
                    string[] str = itemStr.Split(',');

                    int counter = 0;
                    foreach (var value in str)
                    {
                        if (string.IsNullOrEmpty(value))
                        {
                            continue;
                        }
                        else
                        {
                            bool result;
                            if (Boolean.TryParse(value, out result))
                            {
                                SetCpuNaki(counter, result);
                                uiManager.ChangeCpuNakiForLoadText(counter, result ? 0 : 1);
                                counter++;
                            }
                            else
                            {
                                Debug.LogError($"LoadPuzzle : Error , i = {i}\value = {value}");
                            }
                        }
                    }

                    if (counter != 3)
                    {
                        Debug.LogError($"LoadPuzzle : Error , i = {i}\ncounter = {counter}");
                    }
                }
                else if (i == 3) // �R
                {
                    //if (itemStr == "END") break;

                    List<MahjongManager.PaiKinds> loadList = new List<MahjongManager.PaiKinds>();

                    string[] str = itemStr.Split(',');

                    foreach (var value in str)
                    {
                        if (Enum.TryParse(value, out MahjongManager.PaiKinds auth))
                        {
                            loadList.Add(auth);
                        }
                    }

                    if (loadList.Count == 136)
                    {
                        for(int x = 0; x < 136; x++)
                        {
                            ReceptionPositionButton(x);
                            SettingPai(loadList[x]);
                        }
                        ReceptionPositionNoSelectButton();
                    }
                    else
                    {
                        Debug.LogError($"LoadPuzzle : Error , i = {i}\nloadList.Count = {loadList.Count}");
                    }
                }
                else
                {
                    break;
                }
                i++;
            }
        }

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

    /// <summary>
    /// �C���X�y�N�^�[�̃{�^�����������Ƃ�(�ǂݍ���)
    /// </summary>
    public void InspectorButtonFunctionForLoad()
    {
        if (loadTextFile == null) return;
        if (GetZaikoTotalCounter() != 136) return;

        LoadTextFile();
    }

    /// <summary>
    /// �C���X�y�N�^�[�̃{�^�����������Ƃ�(��������)
    /// </summary>
    public void InspectorButtonFunctionForSave()
    {
        if (saveTextFile == null) return;

        string filePath = AssetDatabase.GetAssetPath(saveTextFile);

        // �t�@�C���ɏ������݁i�㏑���j
        try
        {
            File.WriteAllText(filePath, GetDataString());
            Debug.Log($"InspectorButtonFunctionForSave : Success !\n filePath = {filePath}");
        }
        catch (Exception ex)
        {
            Debug.Log($"InspectorButtonFunctionForSave : Failed !\n filePath = {filePath} , Message = {ex.Message}");
        }

        // �A�Z�b�g�����t���b�V�����āA�G�f�B�^�ōŐV�̏�Ԃ𔽉f
        AssetDatabase.Refresh();
    }

}

/// <summary>
/// PuzzleMakerManager���g������
/// </summary>
[CustomEditor(typeof(PuzzleMakerManager))] 
public class PuzzleMakerManagerDisplayLog : Editor
{
    public override void OnInspectorGUI()
    {
        // �^�[�Q�b�g�X�N���v�g���擾
        PuzzleMakerManager yourScript = (PuzzleMakerManager)target;

        // ���ׂẴv���p�e�B���擾
        SerializedProperty property = serializedObject.GetIterator();
        property.NextVisible(true); // �ŏ��̃v���p�e�B�iScript�j���X�L�b�v

        // �C���X�y�N�^�[���̃v���p�e�B�����[�v���ĕ\��
        while (property.NextVisible(false))
        {
            // "loadTextFile"�v���p�e�B��`��
            if (property.name == "loadTextFile")
            {
                EditorGUILayout.PropertyField(property, true);

                // "Load Text File"�{�^�������̌�ɕ\��
                if (GUILayout.Button("Load Text File"))
                {
                    yourScript.InspectorButtonFunctionForLoad();
                }
            }
            // "saveTextFile"�v���p�e�B��`��
            else if (property.name == "saveTextFile")
            {
                EditorGUILayout.PropertyField(property, true);

                // "Save Text File"�{�^�������̌�ɕ\��
                if (GUILayout.Button("Save Text File"))
                {
                    yourScript.InspectorButtonFunctionForSave();
                }
            }
            else
            {
                // ���̂��ׂẴv���p�e�B��`��
                EditorGUILayout.PropertyField(property, true);
            }
        }

        // �ύX��K�p
        serializedObject.ApplyModifiedProperties();
    }
}
