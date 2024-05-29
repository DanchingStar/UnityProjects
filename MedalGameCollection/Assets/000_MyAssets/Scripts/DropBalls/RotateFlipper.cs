using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DropBalls
{
    public class RotateFlipper : MonoBehaviour
    {
        [SerializeField] private Transform[] Flippers;
        [SerializeField] private float speed = 0.25f;


        private float myTime = 0f;

        private void Start()
        {

        }

        private void Update()
        {
            for (int i = 0; i < Flippers.Length; i++)
            {
                Flippers[i].localRotation = (Quaternion.Euler(new Vector3(0, 0, MakeAngle())));
            }
        }


        private float MakeAngle()
        {
            float num1, num2;
            myTime += Time.deltaTime;

            num1 = myTime;

            num2 = 40f * Mathf.Sin(Mathf.PI * num1 * speed);

            return num2;
        }
    }
}