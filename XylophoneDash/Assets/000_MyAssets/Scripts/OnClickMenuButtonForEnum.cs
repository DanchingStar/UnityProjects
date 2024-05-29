using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnClickMenuButtonForEnum : MonoBehaviour
{
    [SerializeField] private MenuSceneManager.SelectMode myMode;
    private Button myButton;

    void Start()
    {
        myButton = GetComponent<Button>();
        myButton.onClick.AddListener(OnClickMyButton);
    }

    private void OnClickMyButton()
    {
        MenuSceneManager.Instance.OnTapMenuButton((int)myMode);
    }
}
