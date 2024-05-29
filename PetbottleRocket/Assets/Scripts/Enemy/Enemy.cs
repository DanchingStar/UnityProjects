using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// “G‹¤’ÊBehavior
/// </summary>
public class Enemy : MonoBehaviour
{
    //[SerializeField]
    protected float _DamageAngle = 0.0f;

    /// <summary>
    /// “–‚½‚Á‚½‚ÉŒX‚­Šp“x
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
    /// Player‚ÌŠp“x‚ÉˆË‘¶‚·‚é‚©‚Ç‚¤‚©
    /// Player‚Æ‚Ô‚Â‚©‚Á‚½‚ÉEnemy‚ÌDamageAngle‚Ì‚İ‚ÅŠ®Œ‹‚µ‚È‚¢
    /// True = Player‘¤‚Å‚Ç‚Á‚¿‚ÉŒü‚­‚©l—¶‚·‚é
    /// False = Player‘¤‚ÍDamageAngle‚ğ‘«‚·‚¾‚¯
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
    /// Œp³æ‚ÅUpdate‚ª‘‚©‚ê‚Ä‚é‚ÍŒÄ‚Î‚ê‚È‚¢
    /// </summary>
    private void Update()
    {
        UpdatePosition();
    }
    #endregion

    public virtual Vector3 GenerateEnemyPos() 
    {
        Debug.Log($"[Enemy] Base‚ÌGenerateEnemyPos‚ªŒÄ‚Î‚ê‚Ä‚éBŒp³æ‚Åoverride‚·‚é•û‚ª–]‚Ü‚µ‚¢");
        return new Vector3(0.0f, 15.0f, 0.0f);
    }

    /// <summary>
    /// i‚Ş•ûŒü
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
