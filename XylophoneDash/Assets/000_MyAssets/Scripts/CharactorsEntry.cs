using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharactorsEntry", menuName = "Charactors Entry")]
public class CharactorsEntry : ScriptableObject
{
    public List<Charactors> itemList = new List<Charactors>();

}
