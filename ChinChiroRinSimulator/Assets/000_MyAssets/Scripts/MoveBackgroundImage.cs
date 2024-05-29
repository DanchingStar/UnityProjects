using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBackgroundImage : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float moveAmplitude = 1f;
    private Vector3 defaultPosition;

    private void Start()
    {
        defaultPosition = transform.localPosition;
    }

    private void Update()
    {
        transform.localPosition = defaultPosition + new Vector3(Mathf.Sin(Time.time * moveSpeed) * moveAmplitude, 0, 0);
    }
}
