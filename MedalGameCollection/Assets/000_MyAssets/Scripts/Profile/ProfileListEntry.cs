using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProfileListEntry", menuName = "Profile/Profile List Entry")]
public class ProfileListEntry : ScriptableObject
{
    public List<Profile> itemList = new List<Profile>();
}
