using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

public class JoystickToInputAction : OnScreenControl
{
    private Vector2 inputValue;

    private Joystick joystick;

    private string myName;

    private bool errorFlg;

    [InputControl(layout = "Vector2")]
    [SerializeField] private string _controlPath = "<Gamepad>/leftStick";

    protected override string controlPathInternal
    {
        get => _controlPath;
        set => _controlPath = value;
    }

    //private void Reset()
    //{
    //    joystick = GetComponent<Joystick>();
    //}

    private void Start()
    {
        joystick = gameObject.GetComponent<Joystick>();

        if (joystick == null)
        {
            Debug.LogError("Nothing JoyStick");
            errorFlg = true;
        }
        else
        {
            //Debug.Log("OK JoyStick");
            errorFlg = false;
        }

        inputValue = Vector2.zero;

        myName = gameObject.name;
    }

    private void Update()
    {
        if (errorFlg) return;

        if (inputValue != joystick.Direction)
        {
            //Debug.Log($"JoystickToInputAction : {myName} -> X = {joystick.Direction.x}, Y = {joystick.Direction.y}");
        }

        inputValue = joystick.Direction;

        SendValueToControl(inputValue);

    }

}