using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudEnemy : Enemy
{
    #region GameSystem
    private void Start()
    {
        _DamageAngle = 1.0f;
        _IsDependsPlayerAngle = true;
    }

    #endregion

    public override Vector3 GenerateEnemyPos()
    {
        var rand = Random.Range(0.0f, 4.0f);
        return new Vector3(-2.0f + rand, 15.0f, 0.0f);
    }

    protected override Vector3 Direction
    {
        get
        {
            return Vector3.zero;
        }
    }

}
