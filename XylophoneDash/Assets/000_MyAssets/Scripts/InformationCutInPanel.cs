using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InformationCutInPanel : MonoBehaviour
{
    private List<string> stringList = new List<string>();

    private GameObject informationCutInPanel;
    [SerializeField] private TextMeshProUGUI informationCutInText;

    private const int INFORMATION_CUT_IN_PANEL_DEFAULT_POSITION_X = 910;
    private const int INFORMATION_CUT_IN_PANEL_MOVE_POSITION_X = 180;
    private const int INFORMATION_CUT_IN_PANEL_POSITION_Y = 650;

    private const float CUT_IN_SPEED = 3f;
    private const float CUT_IN_DISPLAY_TIME = 3f;
    private const float CUT_IN_INTERVAL_TIME = 0.2f;

    private bool waitingFlg;

    private void Start()
    {
        informationCutInPanel = this.gameObject;
        waitingFlg = false;

        informationCutInPanel.GetComponent<Transform>().localPosition =
             new Vector3(INFORMATION_CUT_IN_PANEL_DEFAULT_POSITION_X, INFORMATION_CUT_IN_PANEL_POSITION_Y, 0);
        informationCutInText.text = "";

        informationCutInPanel.transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        if (waitingFlg) return;
        if (stringList.Count == 0) return;

        StartCoroutine(DisplayInformationCutInPanel());
    }

    /// <summary>
    /// テキストを追加する
    /// </summary>
    /// <param name="text"></param>
    public void AddInformationText(string text)
    {
        stringList.Add(text);
    }

    /// <summary>
    /// 情報のカットインを表示する
    /// </summary>
    /// <param name="isSuccess"></param>
    /// <returns></returns>
    public IEnumerator DisplayInformationCutInPanel()
    {
        waitingFlg = true;

        informationCutInPanel.transform.localScale = Vector3.one;
        informationCutInText.text = stringList[0];

        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.InformationCutIn);

        Transform panelTF = informationCutInPanel.GetComponent<Transform>();

        Vector3 startPosition = new Vector3(INFORMATION_CUT_IN_PANEL_DEFAULT_POSITION_X, INFORMATION_CUT_IN_PANEL_POSITION_Y, 0);
        Vector3 goalPosition = new Vector3(INFORMATION_CUT_IN_PANEL_MOVE_POSITION_X, INFORMATION_CUT_IN_PANEL_POSITION_Y, 0);

        float value = 0;
        while (value < 1)
        {
            value += Time.deltaTime * CUT_IN_SPEED;
            panelTF.localPosition = Vector3.Lerp(startPosition, goalPosition, value);
            yield return null;
        }

        yield return new WaitForSeconds(CUT_IN_DISPLAY_TIME);

        value = 0;
        while (value < 1)
        {
            value += Time.deltaTime * CUT_IN_SPEED;
            panelTF.localPosition = Vector3.Lerp(goalPosition, startPosition, value);
            yield return null;
        }

        informationCutInPanel.transform.localScale = Vector3.zero;

        yield return new WaitForSeconds(CUT_IN_INTERVAL_TIME);

        stringList.RemoveAt(0);
        waitingFlg = false;
        yield return null;
    }
}
