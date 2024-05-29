using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneEnemy : Enemy
{
    private Vector3 _Direciton;

    // Prefab‘¤‚ÅŽw’è‚·‚é‚±‚Æ
    [SerializeField]
    private bool _IsMoveLeft = true;

    #region GameSystem
    private void Start()
    {
        if (_IsMoveLeft)
        {
            _Direciton = new Vector3(-2.0f, 0.0f, 0.0f);
            _DamageAngle = 30.0f;
        }
        else
        {
            _Direciton = new Vector3(2.0f, 0.0f, 0.0f);
            _DamageAngle = -30.0f;
        }
        _IsDependsPlayerAngle = false;
    }
    #endregion

    public override Vector3 GenerateEnemyPos()
    {
        float rand = Random.Range(0.0f, 2.0f);

        if (_IsMoveLeft)
        {
            return new Vector3(5.0f, 4.0f + rand, 0.0f);
        }
        return new Vector3(-5.0f, 5.0f + rand, 0.0f);
    }

    protected override Vector3 Direction
    {
        get
        {
            return _Direciton;
        }
    }
}
