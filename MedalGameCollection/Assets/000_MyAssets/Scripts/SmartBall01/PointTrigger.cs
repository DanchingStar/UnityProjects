using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SmartBall01
{
    public class PointTrigger : MonoBehaviour
    {
        [SerializeField] private int myNumber;

        /// <summary>
        /// ポイント穴の番号を返すゲッター
        /// </summary>
        /// <returns>ポイント穴の番号</returns>
        public int GetPointNumber()
        {
            return myNumber;
        }
    }
}
