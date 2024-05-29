using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdEnemy : Enemy
{
    #region GameSystem
    private void Start()
    {
        _DamageAngle = 10.0f;
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
            return Vector3.zero;
        }
    }
}
