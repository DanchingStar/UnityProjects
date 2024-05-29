using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Pusher01.GameManager;

namespace Pusher01
{
    public class HoryuuLamp : MonoBehaviour
    {
        public MeshRenderer horyuuRenderer;

        [SerializeField] private Material horyuuNormal;
        [SerializeField] private Material horyuuBlue;
        [SerializeField] private Material horyuuGreen;
        [SerializeField] private Material horyuuPurple;
        [SerializeField] private Material horyuuRed;
        [SerializeField] private Material horyuuNone;

        /// <summary>
        /// ï€óØÇÃêFÇïœÇ¶ÇÈ
        /// </summary>
        public void ChangeHoryuuColor(HoryuuColor horyuuColor)
        {
            switch (horyuuColor)
            {
                case HoryuuColor.Normal:
                    horyuuRenderer.material = horyuuNormal;
                    break;
                case HoryuuColor.Blue:
                    horyuuRenderer.material = horyuuBlue;
                    break;
                case HoryuuColor.Green:
                    horyuuRenderer.material = horyuuGreen;
                    break;
                case HoryuuColor.Purple:
                    horyuuRenderer.material = horyuuPurple;
                    break;
                case HoryuuColor.Red:
                    horyuuRenderer.material = horyuuRed;
                    break;
                case HoryuuColor.None:
                    horyuuRenderer.material = horyuuNone;
                    break;
                default:
                    break;
            }
        }
    }

}
