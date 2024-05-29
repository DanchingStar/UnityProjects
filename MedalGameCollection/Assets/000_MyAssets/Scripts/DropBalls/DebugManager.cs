using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DropBalls
{
    public class DebugManager : MonoBehaviour
    {
        [SerializeField] private bool debugMode = false;
        [SerializeField] private GameObject UIDebugPanel;


        public static DebugManager Instance;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        private void Update()
        {
            if (UIDebugPanel.activeSelf != debugMode)
            {
                UIDebugPanel.SetActive(debugMode);
            }
        }

    }
}