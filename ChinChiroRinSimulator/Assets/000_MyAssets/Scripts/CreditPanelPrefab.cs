using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreditPanelPrefab : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI creditText;
    [SerializeField] private TextAsset creditTextFile;

    private void Start()
    {
        creditText.text = creditTextFile.text;
    }

    /// <summary>
    /// ‚±‚ÌPrefab‚ð•Â‚¶‚½‚Æ‚«
    /// </summary>
    public void PushCloseButton()
    {
        SoundManager.Instance.PlaySE(SoundManager.SoundSeType.No);
        Destroy(gameObject);
    }

}
