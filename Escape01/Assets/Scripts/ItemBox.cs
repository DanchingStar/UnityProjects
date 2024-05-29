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
    /// �X���b�g���I�����ꂽ���Ɏ��s����֐�
    /// </summary>
    /// <param name="position">�A�C�e�����i�[����ꏊ</param>
    public void OnSelectSlot(int position)
    {
        if (ControlStopper.instance.GetIsControlStop()) return;

        foreach (ItemSlot slot in itemSlots) //slots�̐������J��Ԃ�
        {
            slot.SetSelectNow(false); //�I����Ԃ�����
        }

        //�I�����ꂽ�X���b�g�̃t���O��ς���
        if (itemSlots[position].OnSelected()) // �����A�C�e���̑I�������������Ȃ�
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
        foreach (ItemSlot slot in itemSlots) //slots�̐������J��Ԃ�
        {
            if (slot.GetSelectNow() == true)
            {
                return slot;
            }
        }

        return null;
    }
}
