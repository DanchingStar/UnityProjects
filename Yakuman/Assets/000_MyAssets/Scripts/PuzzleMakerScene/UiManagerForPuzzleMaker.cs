using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UiManagerForPuzzleMaker : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI totalText;
    [SerializeField] private TMP_Dropdown dropdownOya;
    [SerializeField] private TMP_Dropdown[] dropdownCpus;
    [SerializeField] private TMP_Dropdown[] dropdownCpuNakis;

    private List<string> optionsOya = new List<string>() { "自家", "下家", "対面", "上家" };
    //private MahjongManager.PlayerKind playerOya;

    //private CpuParent[] cpuChildren;

    private List<string> optionsCpuNaki = new List<string>() { "鳴きアリ", "鳴きナシ" };
    //private bool[] cpuNakis;

    private CpuContentGenerator cpuContentGenerator;

    private const string SCENE_NAME_PUZZLE_MAKER = "PuzzleMaker";

    private void Start()
    {
        cpuContentGenerator = GameModeManager.Instance.GetCpuContentGenerator();

        dropdownOya.options.Clear();
        foreach (var option in optionsOya)
        {
            dropdownOya.options.Add(new TMP_Dropdown.OptionData(option));
        }
        dropdownOya.onValueChanged.AddListener((value) => OnValueChangedOya(value));

        for (int i = 0; i < dropdownCpus.Length; i++)
        {
            dropdownCpus[i].options.Clear();
            for (int j = 0; j < cpuContentGenerator.GetLength(); j++)
            {
                dropdownCpus[i].options.Add(new TMP_Dropdown.OptionData(cpuContentGenerator.GetCpuName(j)));
            }
            int x = i;
            dropdownCpus[i].onValueChanged.AddListener((value) => OnValueChangedCpuName(x, value));
        }

        for (int i = 0; i < dropdownCpuNakis.Length; i++)
        {
            dropdownCpuNakis[i].options.Clear();
            foreach (var option in optionsCpuNaki)
            {
                dropdownCpuNakis[i].options.Add(new TMP_Dropdown.OptionData(option));
            }
            int x = i;
            dropdownCpuNakis[i].onValueChanged.AddListener((value) => OnValueChangedCpuNaki(x, value));
        }

        dropdownOya.captionText.text = optionsOya[0];
        foreach (var item in dropdownCpus) item.captionText.text = cpuContentGenerator.GetCpuName(0);
        foreach (var item in dropdownCpuNakis) item.captionText.text = optionsCpuNaki[0];
    }

    public void PushDeleteButton()
    {
        PuzzleMakerManager.Instance.ReceptionDeleteButton();
    }

    public void PushLogButton()
    {
        Debug.Log(PuzzleMakerManager.Instance.GetDataString());
    }

    public void ChangeTotalText(int _counter)
    {
        totalText.text = "残り : " + _counter.ToString("d3");
    }

    public void PushResetButton()
    {
        ReloadThisScene();
    }

    private void ReloadThisScene()
    {
        FadeManager.Instance.LoadScene(SCENE_NAME_PUZZLE_MAKER, 0.5f);
    }

    private void OnValueChangedOya(int _value)
    {
        var playerOya = MahjongManager.PlayerKind.Player + _value;
        PuzzleMakerManager.Instance.SetOyaPlayer(playerOya);
    }

    private void OnValueChangedCpuName(int _index ,int _value)
    {
        var item = cpuContentGenerator.GetCpuChild(_value);
        PuzzleMakerManager.Instance.SetCpu(_index, item);
    }

    private void OnValueChangedCpuNaki(int _index, int _value)
    {
        bool flg = _value == 0 ? true : false;
        PuzzleMakerManager.Instance.SetCpuNaki(_index, flg);
    }

    public void ChangeOyaForLoadText(int _value)
    {
        if(dropdownOya.value != _value)
        {
            dropdownOya.value = _value;
        }
    }

    public void ChangeCpuNameForLoadText(int _index, int _value)
    {
        if (dropdownCpus[_index].value != _value)
        {
            dropdownCpus[_index].value = _value;
        }
    }

    public void ChangeCpuNakiForLoadText(int _index, int _value)
    {
        if (dropdownCpuNakis[_index].value != _value)
        {
            dropdownCpuNakis[_index].value = _value;
        }
    }


}
