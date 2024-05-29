using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Menu
{
    public class UserFailurePrefab : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;

        public void SetText(string str)
        {
            text.text = str;
        }
    }
}
