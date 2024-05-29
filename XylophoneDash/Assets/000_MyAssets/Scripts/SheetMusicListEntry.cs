using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SheetMusicListEntry", menuName = "Sheet Music List Entry")]
public class SheetMusicListEntry : ScriptableObject
{
    public List<SheetMusicList> itemList = new List<SheetMusicList>();

}
