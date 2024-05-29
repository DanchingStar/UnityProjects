using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SmartBall01
{
    public class FireHandle : MonoBehaviour
    {
        [SerializeField] private ButtonHold fireButton;

        private const float HANDLE_SPEED = 580f;

        private Transform myTransform;
        private Vector3 defaultPosition;

        private void Start()
        {
            myTransform = this.gameObject.GetComponent<Transform>();
            defaultPosition = myTransform.localPosition;
        }
        private void Update()
        {
            if (fireButton.GetIsButtonOn())
            {
                MoveMyPosition();
            }
            else if (myTransform.localPosition != defaultPosition)
            {
                myTransform.localPosition = defaultPosition;
            }
        }

        /// <summary>
        /// é©êgÇÃà íuÇìÆÇ©Ç∑
        /// </summary>
        private void MoveMyPosition()
        {
            float power = (fireButton.GetPower() - fireButton.GetBasePower()) / HANDLE_SPEED;
            myTransform.localPosition = defaultPosition - new Vector3(0, 0, power);
        }

    }
}
