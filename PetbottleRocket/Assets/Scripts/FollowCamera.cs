using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FollowCamera : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] GameObject playerObj;


    void Update()
    {
        if(!player.GetGameOverFlg())
        {
            MoveCamera();
        }
    }

    void MoveCamera()
    {
        Vector3 playerPos = playerObj.transform.position;
        //カメラとプレイヤーの位置を同じにする
        transform.position = new Vector3(playerPos.x, playerPos.y + 3.0f, -10);
    }

}