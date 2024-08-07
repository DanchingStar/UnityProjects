using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵共通Behavior
/// </summary>
public class Enemy : MonoBehaviour
{
    //[SerializeField]
    protected float _DamageAngle = 0.0f;

    /// <summary>
    /// 当たった時に傾く角度
    /// </summary>
    public float DamageAngle
    {
        get
        {
            return _DamageAngle;
        }
    }

    //[SerializeField]
    protected bool _IsDependsPlayerAngle = false;

    /// <summary>
    /// Playerの角度に依存するかどうか
    /// Playerとぶつかった時にEnemyのDamageAngleのみで完結しない
    /// True = Player側でどっちに向くか考慮する
    /// False = Player側はDamageAngleを足すだけ
    /// </summary>
    public bool IsDependsPlayerAngle
    {
        get
        {
            return _IsDependsPlayerAngle;
        }
    }

    #region GameSystem
    private void Start()
    {

    }

    /// <summary>
    /// 継承先でUpdateが書かれてる時は呼ばれない
    /// </summary>
    private void Update()
    {
        UpdatePosition();
    }
    #endregion

    public virtual Vector3 GenerateEnemyPos() 
    {
        Debug.Log($"[Enemy] BaseのGenerateEnemyPosが呼ばれてる。継承先でoverrideする方が望ましい");
        return new Vector3(0.0f, 15.0f, 0.0f);
    }

    /// <summary>
    /// 進む方向
    /// </summary>
    protected virtual Vector3 Direction
    {
        get
        {
            return Vector3.zero;
        }
    }

    protected virtual void UpdatePosition()
    {
        var diff = Time.deltaTime * Direction;
        var pos = transform.position + diff;
        transform.position = pos;
        //Debug.Log($"dir : {dir}");
        //Debug.Log($"pos : {pos}");
        //Debug.Log($"transform pos: {transform.position}");
    }
}
