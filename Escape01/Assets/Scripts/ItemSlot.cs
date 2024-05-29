//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.UI;
//using static UnityEditor.Progress;

public class ItemSlot : MonoBehaviour
{
    private Item myItem;
    private Image image;

    private bool selectNow = false;

    private GameObject frame1,frame2;

    [SerializeField] Sprite noItemSprite;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void Start()
    {
        frame1 = this.transform.Find("ItemSlotFrame1").gameObject;
        frame2 = this.transform.Find("ItemSlotFrame2").gameObject;
        frame1.SetActive(true);
        frame2.SetActive(false);
    }

    private void Update()
    {
        if (selectNow)
        {
            frame1.SetActive(false);
            frame2.SetActive(true);
        }
        else
        {
            frame1.SetActive(true);
            frame2.SetActive(false);
        }
    }

    public bool IsEmpty()
    {
        if (myItem == null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetItem(Item item)
    {
        myItem = item;
        UpdateImage(item);
    }

    private void UpdateImage(Item item)
    {
        image.sprite = item.sprite;
    }

    public bool OnSelected()
    {
        if (myItem == null) //　アイテムを持っていない場合
        {
            return false; //選択は失敗
        }
        else
        {
            return true; //選択成功
        }
    }


    public bool GetSelectNow()
    {
        return selectNow;
    }

    public void SetSelectNow(bool a)
    {
        selectNow = a;
    }

    public Item GetHaveItem()
    {
        return myItem;
    }

    public void DeleteItem()
    {
        frame1.SetActive(true);
        frame2.SetActive(false);
        selectNow = false;
        image.sprite = noItemSprite;
        myItem = null;
    }

}
