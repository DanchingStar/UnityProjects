using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class TalkEventManager : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI talkHumanNameText;
    [SerializeField] private TextMeshProUGUI talkingText;

    [SerializeField] private TextAsset testTextfile;

    private List<TalkLine> talkLines;

    private Coroutine nowLineCoroutine;

    private int nowLineIndex;

    private const float TIME_ONE_CHAR_DELAY = 0.02f;


    public class TalkLine
    {
        public string talkHumanName;
        public string talkString;

        public TalkLine(string _talkHumanName, string _talkString)
        {
            talkHumanName = _talkHumanName;
            talkString = _talkString;
        }
    }

    private void Start()
    {
        if (testTextfile != null)
        {
            ResetStrings();
            LoadTextFile(testTextfile);
            TalkNextLine(true);
        }
    }


    private void ResetStrings()
    {
        nowLineIndex = 0;
        talkingText.text = "";
        talkHumanNameText.text = "";
        talkLines = new List<TalkLine>();
    }

    /// <summary>
    /// テキストファイルを読み込む
    /// </summary>
    /// <param name="_textfile"></param>
    /// <returns></returns>
    public bool LoadTextFile(TextAsset _textfile)
    {
        //Debug.Log($"LoadPuzzle : Called , Now Scene = {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");

        StringReader stackLevelCostReader = new StringReader(_textfile.text);
        string text = stackLevelCostReader.ReadToEnd();

        string[] allText = text.Split(';');

        bool errorFlg = false;
        int lineCounter = 0;
        foreach (var item in allText)
        {
            // 改行をカットする
            string itemStr = CutEnter(item);

            if (errorFlg)
            {
                break;
            }
            else if (string.IsNullOrEmpty(itemStr))
            {
                // 何もしない
            }
            else if (itemStr.StartsWith("#"))
            {
                // 何もしない
            }
            else if (itemStr.StartsWith("!"))
            {
                string[] spritStr = CutChar(itemStr, "!").Split(',');

                if (spritStr[0] == "Select")
                {

                }
                else if(spritStr[0] == "Change")
                {

                }
            }
            else if (itemStr.StartsWith("&"))
            {
                string[] spritStr = CutChar(itemStr, "&").Split(',');

                if(spritStr.Length != 2)
                {
                    Debug.Log($"LoadTextFile : String Sprit Error\nspritStr[0] = {spritStr[0]}\ni = {lineCounter}");
                    errorFlg = true;
                    break;
                }

                if (spritStr[1].Contains("\\n")) spritStr[1] = spritStr[1].Replace(@"\n", Environment.NewLine);
                talkLines.Add(new TalkLine(spritStr[0], spritStr[1]));

                lineCounter++;
            }
        }

        if (errorFlg)
        {
            Debug.LogError($"LoadTextFile : Load is Canceled");
        }
        else
        {
            Debug.Log($"LoadTextFile : Load Success !\nlineCounter = {lineCounter}");
        }

        return !errorFlg;
    }

    /// <summary>
    /// 会話を次の行に移行する
    /// </summary>
    /// <param name="_firstFlg">初回はTrueで呼ぶ</param>
    public void TalkNextLine(bool _firstFlg)
    {
        if (!_firstFlg) nowLineIndex++;

        if (talkLines.Count > nowLineIndex)
        {
            nowLineCoroutine = StartCoroutine(TalkOneChar(talkLines[nowLineIndex]));

            //Debug.Log($"TalkNextLine : nowLineIndex = {nowLineIndex}");

        }
        else // すべて読み切った後
        {

        }
    }

    /// <summary>
    /// 話すコルーチン
    /// </summary>
    /// <param name="_talkStr"></param>
    /// <returns></returns>
    private IEnumerator TalkOneChar(TalkLine _talkLine)
    {
        int talkLength = _talkLine.talkString.Length;

        talkHumanNameText.text = _talkLine.talkHumanName;

        for (int i = 0; i < talkLength; i++)
        {
            if (i == 0)
            {
                talkingText.text = _talkLine.talkString;
            }
            else
            {
                talkingText.maxVisibleCharacters = i;
                yield return new WaitForSeconds(TIME_ONE_CHAR_DELAY);
            }
        }

        talkingText.maxVisibleCharacters = talkLength;
        nowLineCoroutine = null;

        //Debug.Log($"TalkOneChar : talkLength = {talkLength}\n{talkHumanNameText.text}「{talkingText.text}」");
    }

    /// <summary>
    /// 文字列の改行を消す
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    private string CutEnter(string str)
    {
        return str.Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace("\v", "").Replace("\0", "").Replace(" ", "").Trim();
    }

    /// <summary>
    /// 指定した文字を消す
    /// </summary>
    /// <param name="str"></param>
    /// <param name="_cutChar"></param>
    /// <returns></returns>
    private string CutChar(string str , string _cutChar)
    {
        return str.Replace(_cutChar, "").Trim();
    }

}
