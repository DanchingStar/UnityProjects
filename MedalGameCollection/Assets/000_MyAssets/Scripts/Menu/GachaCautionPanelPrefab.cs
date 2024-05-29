using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Menu
{
    public class GachaCautionPanelPrefab : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI cautionText;

        public void SetMyText()
        {
            string resourcePath = "Gacha/CautionText";

            TextAsset textAsset = Resources.Load<TextAsset>(resourcePath);

            string fileContents = textAsset.text;

            cautionText.text = fileContents;
        }

        public void PushCloseButton()
        {
            Destroy(this.gameObject);
        }
    }
}
