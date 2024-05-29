using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public class GachaFailurePrefab : MonoBehaviour
    {
        [SerializeField] private Text errorMessageText;

        public void SetMyMessage(string str)
        {
            errorMessageText.text = str;
        }
    }
}
