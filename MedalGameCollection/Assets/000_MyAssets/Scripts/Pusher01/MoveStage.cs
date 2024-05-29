using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pusher01
{
    public class MoveStage : MonoBehaviour
    {
        [SerializeField] private float speed = 0.25f;

        private Rigidbody myRigidbody;
        private Vector3 defaultPosition;

        private float amplitude = 2f;

        private float myTime = 0f;

        private void Start()
        {
            myRigidbody = this.GetComponent<Rigidbody>();
            defaultPosition = myRigidbody.position;
        }

        private void Update()
        {

            //myRigidbody.position = new Vector3(0, 0, MakePosition() + amplitude) + defaultPosition;
            myRigidbody.MovePosition(new Vector3(0, 0, MakePosition() + amplitude) + defaultPosition);

        }


        private float MakePosition()
        {
            float num1, num2;
            myTime += Time.deltaTime;

            num1 = myTime;

            num2 = amplitude * Mathf.Sin(Mathf.PI * num1 * speed);

            return num2;
        }
    }

}
