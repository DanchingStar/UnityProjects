using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceMain : MonoBehaviour
{
    [SerializeField] private DiceEye[] diceEyes;

    private int diceNumber;

    private Rigidbody rb;
    private MeshRenderer myRenderer;

    private bool stopFlg;
    private float stopTimer;

    private bool finishStartFlg = false;

    private DiceController.DiceEyeKinds myFinalEye = DiceController.DiceEyeKinds.None;

    private const float DICE_POWER = 20f;

    private void Start()
    {
        if (finishStartFlg) return;

        finishStartFlg = true;

        rb = GetComponent<Rigidbody>();
        myRenderer = GetComponent<MeshRenderer>();

        stopFlg = true;
        stopTimer = 0f;

        RigidbodyEnable(false);
    }

    private void Update()
    {
        if (stopFlg) return;

        if (rb.velocity.magnitude < 0.05f)
        {
            stopTimer += Time.deltaTime;

            if(stopTimer > 0.3f)
            {
                RollingFinish();
            }
        }
        else
        {
            stopTimer = 0f;
        }

    }

    public void SetMyStatus(int num)
    {
        if (!finishStartFlg) Start();

        diceNumber = num;

        gameObject.name = $"Dice{(char)('A' + diceNumber)}";
        myRenderer.material= DiceController.Instance.GetDiceMaterial(diceNumber);
        transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));

        //switch (diceNumber)
        //{
        //    case 0: transform.position = new Vector3(3, 15, 0); break;
        //    case 1: transform.position = new Vector3(0, 15, -3); break;
        //    case 2: transform.position = new Vector3(-3, 15, 2); break;
        //    default: break;
        //}

    }

    public void ChangeEyes(DiceController.DiceEyeKinds baseEye , DiceController.DiceEyeKinds changeEye)
    {
        int _eyeNumber = (int)baseEye - 1;
        if (_eyeNumber < diceEyes.Length && _eyeNumber >= 0)
        {
            diceEyes[_eyeNumber].ChangeMyKind(changeEye);
        }
        else
        {
            Debug.LogWarning($"DiceMain.ChangeEyes : Return Null , baseEye = {baseEye}");
        }
    }

    public void Fire()
    {
        RigidbodyEnable(true);

        stopFlg = false;
        stopTimer = 0f;

        Vector3 vec = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 0f), Random.Range(-1f, 1f));
        rb.AddForce(vec * DICE_POWER, ForceMode.VelocityChange);
        rb.AddTorque(vec * DICE_POWER, ForceMode.VelocityChange);

        //Debug.Log($"Fire , {gameObject.name} vec = {vec}");
    }

    private void RollingFinish()
    {
        stopFlg = true;

        RigidbodyEnable(false);

        var topIndex = 0;
        var topValue = diceEyes[0].transform.position.y;
        for (var i = 1; i < diceEyes.Length; ++i)
        {
            if (diceEyes[i].transform.position.y < topValue)
                continue;
            topValue = diceEyes[i].transform.position.y;
            topIndex = i;
        }

        myFinalEye = diceEyes[topIndex].GetMyFinalKind();
        //Debug.Log($"RollingFinish , {gameObject.name} = {myFinalEye}");
    }

    private void RigidbodyEnable(bool flg)
    {
        if (flg)
        {
            rb.isKinematic = false;
        }
        else
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
        }
    }

    public void DeleteMyObject()
    {
        Destroy(gameObject);
    }

    public bool GetStopFlg()
    {
        return stopFlg;
    }

    public DiceController.DiceEyeKinds GetMyFinalEye()
    {
        return myFinalEye;
    }

    public int GetMyDiceNumber()
    {
        return diceNumber;
    }

    /// <summary>
    /// シミュレーション用の目を返すゲッター
    /// </summary>
    /// <param name="inputKind">本来の目</param>
    /// <returns>最終的な目</returns>
    public DiceController.DiceEyeKinds GetEyeForSimulation(DiceController.DiceEyeKinds inputKind)
    {
        int _eyeNumber = (int)inputKind - 1;
        if (_eyeNumber < diceEyes.Length && _eyeNumber >= 0)
        {
            return diceEyes[_eyeNumber].GetMyFinalKind();
        }
        else
        {
            Debug.LogWarning($"DiceMain.GetEyeForSimulation : Return None , InputKind = {inputKind}");
            return DiceController.DiceEyeKinds.None;
        }
    }

}
