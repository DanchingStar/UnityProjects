using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameSceneUIController : MonoBehaviour
{
    [SerializeField] private GameObject menuPanelForGameScenePrefab;
    [SerializeField] private GameObject settingVolumePanelPrefab;
    [SerializeField] private GameObject simulationPanelPrefab;

    [SerializeField] private TextMeshProUGUI yakuText;

    private Transform canvasTF;

    private GameObject activePanelPrefab;


#region Awake・インスタンス

    public static GameSceneUIController Instance;
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

    private void Start()
    {
        canvasTF = transform;
    }

    /// <summary>
    /// メニューボタンを押したとき
    /// </summary>
    public void PushMenuButton()
    {
        if (activePanelPrefab != null) return;
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes);
        activePanelPrefab = Instantiate(menuPanelForGameScenePrefab, canvasTF);
    }

    /// <summary>
    /// ボリューム変更ボタンを押したとき
    /// </summary>
    public void PushSettingVolumeButton()
    {
        if (activePanelPrefab != null) return;
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes);
        activePanelPrefab = Instantiate(settingVolumePanelPrefab, canvasTF);
    }

    /// <summary>
    /// スタンバイボタンを押したとき
    /// </summary>
    /// <param name="number"></param>
    public void Test_PushStandByButton(int number)
    {
        DiceController.Instance.Test_ReceptionStandByButton(number);
    }

    /// <summary>
    /// スタンバイボタンを押したとき
    /// </summary>
    /// <param name="number"></param>
    public void PushStandByButton()
    {
        DiceController.Instance.ReceptionStandByButton();

        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.StandBy);
    }

    /// <summary>
    /// 賽を振るボタンを押したとき
    /// </summary>
    public void PushFireButton()
    {
        bool flg = DiceController.Instance.ReceptionFireButton();

        if (flg)
        {
            SoundManager.Instance.PlayDiceRandomSE();
        }
    }

    /// <summary>
    /// シミュレーションボタンを押したとき
    /// </summary>
    public void PushSimulationButton()
    {
        if (activePanelPrefab != null) return;
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes);
        activePanelPrefab = Instantiate(simulationPanelPrefab, canvasTF);
    }

    /// <summary>
    /// 役を表示するテキストを変更する
    /// </summary>
    /// <param name="str">文字列</param>
    public void ChangeYakuText(string str)
    {
        yakuText.text = str;
    }

    /// <summary>
    /// 役を表示するテキストを変更する
    /// </summary>
    /// <param name="str">文字列</param>
    /// <param name="color">色</param>
    public void ChangeYakuText(string str, Color color)
    {
        ChangeYakuText(str);
        yakuText.color = color;
    }

    /// <summary>
    /// 役を表示するテキストを変更する
    /// </summary>
    /// <param name="yaku">役名</param>
    public void ChangeYakuText(DiceController.Yaku yaku)
    {
        switch (yaku)
        {
            case DiceController.Yaku.Hifumi:
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.ResultE);
                ChangeYakuText("ヒフミ!!!", Color.red);
                break;
            case DiceController.Yaku.Menashi:
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.ResultD);
                ChangeYakuText("目無し!");
                break;
            case DiceController.Yaku.Deme1:
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.ResultC);
                ChangeYakuText("出目 壱!!", Color.red); 
                break;
            case DiceController.Yaku.Deme2:
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.ResultC);
                ChangeYakuText("出目 弐!");
                break;
            case DiceController.Yaku.Deme3:
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.ResultC);
                ChangeYakuText("出目 参!");
                break;
            case DiceController.Yaku.Deme4:
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.ResultC);
                ChangeYakuText("出目 肆!");
                break;
            case DiceController.Yaku.Deme5:
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.ResultC);
                ChangeYakuText("出目 伍!"); 
                break;
            case DiceController.Yaku.Deme6:
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.ResultC);
                ChangeYakuText("出目 陸!!", Color.blue);
                break;
            case DiceController.Yaku.Shigoro:
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.ResultB);
                ChangeYakuText("シゴロ!!!", Color.blue); 
                break;
            case DiceController.Yaku.Arashi2:
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.ResultA);
                ChangeYakuText("弐ゾロの\nアラシ!!!", Color.blue);
                break;
            case DiceController.Yaku.Arashi3:
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.ResultA);
                ChangeYakuText("参ゾロの\nアラシ!!!", Color.blue);
                break;
            case DiceController.Yaku.Arashi4:
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.ResultA);
                ChangeYakuText("肆ゾロの\nアラシ!!!", Color.blue); 
                break;
            case DiceController.Yaku.Arashi5:
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.ResultA);
                ChangeYakuText("伍ゾロの\nアラシ!!!", Color.blue);
                break;
            case DiceController.Yaku.Arashi6:
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.ResultA);
                ChangeYakuText("陸ゾロの\nアラシ!!!", Color.blue);
                break;
            case DiceController.Yaku.Arashi1:
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.ResultS);
                ChangeYakuText("ピンゾロの\nアラシ!!!!!", Color.blue);
                break;
            default: ChangeYakuText("エラー");
                break;
        }
    }

    /// <summary>
    /// CanvasのTransformを返すゲッター
    /// </summary>
    /// <returns></returns>
    public Transform GetCanvasTransform()
    {
        return canvasTF;
    }

}
