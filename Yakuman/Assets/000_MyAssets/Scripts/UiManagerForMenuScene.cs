using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManagerForMenuScene : MonoBehaviour
{

    public void PushGameStartToTimeAttack()
    {

    }

    public void PushGameStartToPuzzle(int _stageNumber)
    {

    }

    public void PushGameStartToFree(bool _support)
    {
        GameModeManager.Instance.SetGameModeFree(_support);
        MoveGameScene();
    }

    public void PushGameStartToDebug(int _number)
    {
        int _yama = -1;
        int _rinshan = -1;

        switch (_number)
        {
            case 11:
                {
                    _yama = 0;
                }
                break;
            case 12:
                {
                    _yama = 1;
                }
                break;
            case 20:
                {
                    _rinshan = 0;
                }
                break;
        }

        GameModeManager.Instance.SetGameModeDebug(_number, _yama, _rinshan);
        MoveGameScene();
    }


    private void MoveGameScene()
    {
        FadeManager.Instance.LoadScene("Game", 0.5f);
    }

}
