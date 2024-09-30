using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManagerForMenuScene : MonoBehaviour
{
    [SerializeField] private GameObject puzzleStageButtonPrefab;
    [SerializeField] private Transform puzzleContentTF;
    [SerializeField] private Transform canvasFrontTF;

    private List<Button> buttonsForPuzzleMode;

    public static UiManagerForMenuScene Instance;
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
        InstantiatePuzzleButtons();
    }

    /// <summary>
    /// タイムアタックを始めるボタンを押したとき
    /// </summary>
    /// <param name="_functionNumber"></param>
    public void PushGameStartToTimeAttack(int _functionNumber)
    {
        bool _supportFlg;
        int _needClearCoun;

        switch (_functionNumber)
        {
            case 1:
                {
                    _supportFlg = false;
                    _needClearCoun = 1;
                }
                break;
            default:
                {
                    _supportFlg = true;
                    _needClearCoun = 1;
                }
                break;
        }

        GameModeManager.Instance.SetGameModeTimeAttack(_supportFlg, _needClearCoun);
        MoveGameScene();
    }

    /// <summary>
    /// パズルを始めるボタンを押したとき
    /// </summary>
    /// <param name="_stageNumber"></param>
    public void PushGameStartToPuzzle(int _stageNumber)
    {
        if (GameModeManager.Instance.SetGameModePuzzle(_stageNumber))
        {
            MoveGameScene();
        }
        else
        {
            ChangeInteractableForPuzzleFileLoadIsNoGood(_stageNumber);
        }
    }

    /// <summary>
    /// フリーを始めるボタンを押したとき
    /// </summary>
    /// <param name="_functionNumber"></param>
    public void PushGameStartToFree(int _functionNumber)
    {
        bool _supportFlg = _functionNumber % 2 == 0 ? true : false;
        bool _tehaiOpenFlg = _functionNumber / 2 == 0 ? false : true;

        GameModeManager.Instance.SetGameModeFree(_supportFlg , _tehaiOpenFlg);
        MoveGameScene();
    }

    /// <summary>
    /// デバッグを始めるボタンを押したとき
    /// </summary>
    /// <param name="_number"></param>
    public void PushGameStartToDebug(int _number)
    {
        int _yama = -1;
        int _rinshan = -1;

        switch (_number)
        {
            case 11:
                {
                    _yama = 0;
                }
                break;
            case 12:
                {
                    _yama = 1;
                }
                break;
            case 20:
                {
                    _rinshan = 0;
                }
                break;
        }

        GameModeManager.Instance.SetGameModeDebug(_number, _yama, _rinshan);
        MoveGameScene();
    }

    /// <summary>
    /// ゲームシーンへ移動する
    /// </summary>
    private void MoveGameScene()
    {
        FadeManager.Instance.LoadScene("Game", 0.5f);
    }

    /// <summary>
    /// パズルモードのボタンを生成する
    /// </summary>
    private void InstantiatePuzzleButtons()
    {
        int stageItemCount = GameModeManager.Instance.GetPuzzleStageGenerator().GetStageItemCount();
        buttonsForPuzzleMode = new List<Button>();

        for (int i = 0; i < stageItemCount; i++)
        {
            PuzzleStageButtonPrefab prefab = Instantiate(puzzleStageButtonPrefab, puzzleContentTF).GetComponent<PuzzleStageButtonPrefab>();
            prefab.SetMyStatus(i);
            buttonsForPuzzleMode.Add(prefab.GetMyButton());
        }


    }

    /// <summary>
    /// 正しくない書式のパズルファイルを読もうとした際に呼び出して、ボタンを押せなくする
    /// </summary>
    /// <param name="_stageNumber"></param>
    private void ChangeInteractableForPuzzleFileLoadIsNoGood(int _stageNumber)
    {
        buttonsForPuzzleMode[_stageNumber].interactable = false;
    }

    public void ReceptionPuzzleStageButton(int _stageNumber)
    {
        PushGameStartToPuzzle(_stageNumber);
    }

}
