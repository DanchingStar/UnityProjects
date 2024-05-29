using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menu
{
    [CreateAssetMenu]
    public class TextListEntry : ScriptableObject
    {
        public List<TextList> textList = new List<TextList>();
    }
}