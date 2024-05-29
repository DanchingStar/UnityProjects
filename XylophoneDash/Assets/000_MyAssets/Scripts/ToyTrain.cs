using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToyTrain : MonoBehaviour
{
    [SerializeField] private SoundManager.SoundSeType seType;

    private float power;

    private bool moveFlg;
    private Rigidbody myRigidbody;

    private const float SPEED_STOP_VALUE = 0.01f;
    private const float SPEED_MAX_VALUE = 65f;
    private const float SPEED_MIN_VALUE = 20f;

    private void Start()
    {
        moveFlg = false;
        myRigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (moveFlg)
        {
            if (myRigidbody.velocity.magnitude <= SPEED_STOP_VALUE)
            {
                moveFlg = false;
            }
        }
    }

    /// <summary>
    /// 自身のオブジェクトを叩いた時
    /// </summary>
    public void TapMyObject()
    {
        if (moveFlg) return;

        SoundManager.Instance.PlaySE(seType);
        power = Random.Range(SPEED_MIN_VALUE, SPEED_MAX_VALUE);
        moveFlg = true;
        myRigidbody.AddForce(transform.right * power);
    }
}
