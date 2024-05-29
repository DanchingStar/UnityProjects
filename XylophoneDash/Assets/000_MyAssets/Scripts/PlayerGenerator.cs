using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGenerator : MonoBehaviour
{
    [SerializeField] private CharactorsEntry charactersEntry;

    /// <summary>
    /// キャラ名を返す
    /// </summary>
    /// <param name="elementNumber"></param>
    /// <returns></returns>
    public string GetCharacterName(int elementNumber)
    {
        return charactersEntry.itemList[elementNumber].name;
    }

    /// <summary>
    /// キャラ名と一致するPlayerPrefabを返す
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
    /// PlayerPrefabを返す
    /// </summary>
    /// <param name="elementNumber"></param>
    /// <returns></returns>
    public GameObject GetPlayerPrefab(int elementNumber)
    {
        return charactersEntry.itemList[elementNumber].playerPrefab;
    }

    /// <summary>
    /// キャラ名と一致するSpriteを返す
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
    /// Spriteを返す
    /// </summary>
    /// <param name="elementNumber"></param>
    /// <returns></returns>
    public Sprite GetCharacterSprite(int elementNumber)
    {
        return charactersEntry.itemList[elementNumber].sprite;
    }

    /// <summary>
    /// リストの長さを返す
    /// </summary>
    /// <returns></returns>
    public int GetLength()
    {
        return charactersEntry.itemList.Count;
    }
}
