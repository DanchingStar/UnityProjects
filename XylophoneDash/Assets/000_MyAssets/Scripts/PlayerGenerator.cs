using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGenerator : MonoBehaviour
{
    [SerializeField] private CharactorsEntry charactersEntry;

    /// <summary>
    /// �L��������Ԃ�
    /// </summary>
    /// <param name="elementNumber"></param>
    /// <returns></returns>
    public string GetCharacterName(int elementNumber)
    {
        return charactersEntry.itemList[elementNumber].name;
    }

    /// <summary>
    /// �L�������ƈ�v����PlayerPrefab��Ԃ�
    /// </summary>
    /// <param name="characterName"></param>
    /// <returns></returns>
    public GameObject GetPlayerPrefab(string characterName)
    {
        foreach (var item in charactersEntry.itemList)
        {
            if (item.name == characterName)
            {
                return item.playerPrefab;
            }
        }

        if (charactersEntry.itemList[0] != null)
        {
            return charactersEntry.itemList[0].playerPrefab;
        }

        return null;
    }

    /// <summary>
    /// PlayerPrefab��Ԃ�
    /// </summary>
    /// <param name="elementNumber"></param>
    /// <returns></returns>
    public GameObject GetPlayerPrefab(int elementNumber)
    {
        return charactersEntry.itemList[elementNumber].playerPrefab;
    }

    /// <summary>
    /// �L�������ƈ�v����Sprite��Ԃ�
    /// </summary>
    /// <param name="characterName"></param>
    /// <returns></returns>
    public Sprite GetCharacterSprite(string characterName)
    {
        foreach (var item in charactersEntry.itemList)
        {
            if (item.name == characterName)
            {
                return item.sprite;
            }
        }

        if (charactersEntry.itemList[0] != null)
        {
            return charactersEntry.itemList[0].sprite;
        }

        return null;
    }

    /// <summary>
    /// Sprite��Ԃ�
    /// </summary>
    /// <param name="elementNumber"></param>
    /// <returns></returns>
    public Sprite GetCharacterSprite(int elementNumber)
    {
        return charactersEntry.itemList[elementNumber].sprite;
    }

    /// <summary>
    /// ���X�g�̒�����Ԃ�
    /// </summary>
    /// <returns></returns>
    public int GetLength()
    {
        return charactersEntry.itemList.Count;
    }
}
