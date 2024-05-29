using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SimulationWaitingTextPrefab : MonoBehaviour
{
    private TextMeshProUGUI myText;
    private SimulationPanelPrefab panel;
    private float value;
    private bool finishStartFlg = false;

    void Start()
    {
        if (finishStartFlg) return;
        finishStartFlg = true;
        myText = gameObject.GetComponent<TextMeshProUGUI>();
        value = 0f;
        myText.text = "";
    }

    void Update()
    {
        if (value != panel.GetSimulationCompleteRate())
        {
            value = panel.GetSimulationCompleteRate();
            myText.text = $"シミュレーション中…\n{value}% 完了";
        }
    }

    public void SetMyStatus(SimulationPanelPrefab _panel)
    {
        Start();
        panel = _panel;
    }
}
