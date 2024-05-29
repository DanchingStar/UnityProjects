using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviour
{
    public ItemSlot[] itemSlots;

    public static ItemBox instance;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    /// <summary>
    /// スロットが選択された時に実行する関数
    /// </summary>
    /// <param name="position">アイテムを格納する場所</param>
    public void OnSelectSlot(int position)
    {
        if (ControlStopper.instance.GetIsControlStop()) return;

        foreach (ItemSlot slot in itemSlots) //slotsの数だけ繰り返す
        {
            slot.SetSelectNow(false); //選択状態を解除
        }

        //選択されたスロットのフラグを変える
        if (itemSlots[position].OnSelected()) // もしアイテムの選択が成功したなら
        {
            itemSlots[position].SetSelectNow(true);
            ItemUnion.instance.Union(itemSlots[position].GetHaveItem());
        }
    }

    public void SetItem(Item item)
    {
        foreach (var slot in itemSlots)
        {
            if (slot.IsEmpty())
            {
                slot.SetItem(item);
                break;
            }
        }
    }

    public void DeleteItem(ItemSlot itemSlot)
    {
        foreach (ItemSlot slot in itemSlots)
        {
            if (itemSlot == slot)
            {
                slot.DeleteItem();
                return;
            }
        }
    }

    public ItemSlot GetActiveSlot()
    {
        foreach (ItemSlot slot in itemSlots) //slotsの数だけ繰り返す
        {
            if (slot.GetSelectNow() == true)
            {
                return slot;
            }
        }

        return null;
    }
}
