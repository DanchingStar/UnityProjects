using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteoriteEnemy : Enemy
{
    private Vector3 _Direciton;

    #region GameSystem
    private void Start()
    {
        _DamageAngle = 50.0f;
        var rand = Random.Range(0.0f, 0.6f);
        _Direciton = new Vector3(-0.3f + rand, -2.0f, 0.0f);
        _IsDependsPlayerAngle = true;
    }

    #endregion

    public override Vector3 GenerateEnemyPos()
    {
        return new Vector3(0.0f, 15.0f, 0.0f);
    }

    protected override Vector3 Direction
    {
        get
        {
            return _Direciton;
        }
    }
}
