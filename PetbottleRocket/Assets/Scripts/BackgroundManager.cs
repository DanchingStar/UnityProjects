using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _BackgroundImage;

    [SerializeField]
    private GameObject _Player;

    private GameObject[] _Images;

    private float _ImageHeight;
    private float _ImageWidth;
    private Vector3 _ImageMoveVertical;
    private Vector3 _ImageMoveHorizontal;
    private int _NowPlayerPosImageIndex;

    private void Start()
    {
        _ImageHeight = 1920 * 0.01f;
        _ImageWidth = 1080 * 0.01f;
        _ImageMoveVertical = new Vector3(0, _ImageHeight, 0);
        _ImageMoveHorizontal = new Vector3(_ImageWidth, 0, 0);
        _NowPlayerPosImageIndex = 0;

        // ���̂悤�Ȋ����ɉ摜�𐶐�(����Player�̈ʒu)
        // ���� 2 3
        // ���� 0 1
        // Player�̏����ʒu�́��̒��̉E���̕��ɂ��Ă���
        // 
        // ���Ԍo�߂��A�^��ɔ�Ԃƈȉ��̂悤�Ɉړ�����
        // ���� 0 1
        // ���� 2 3
        // 
        // �΂߂ɂ�����̂͏�ɏ�or�ׂɂ�����̂ƈꏏ�Ɉړ�����
        // �� ��� 1620 * 2304  (1080*1.5 * 1920*1.2)

        // Player�̏����ʒu��(0,0)
        var playerPos = _Player.transform.position;
        var leftDown = playerPos;
        var rightDown = leftDown + _ImageMoveHorizontal;
        var leftUp = leftDown + (_ImageMoveVertical * 1.18f);
        var rightUp = rightDown + (_ImageMoveVertical * 1.18f);

        _Images = new GameObject[4]
        {
            Instantiate(_BackgroundImage, leftDown, Quaternion.identity),
            Instantiate(_BackgroundImage, rightDown, Quaternion.identity),
            Instantiate(_BackgroundImage, leftUp, Quaternion.identity),
            Instantiate(_BackgroundImage, rightUp, Quaternion.identity)
        };
    }

    private void Update()
    {
        var direction = CheckMoveImage();
        MoveImage(direction);
    }

    private MoveDirection CheckMoveImage()
    {
        var playerPos = _Player.transform.position;

        // ����Player�̈ʒu���ǂ��ɂ��邩���擾
        for (int i = 0; i < 4; i++)
        {
            var img = _Images[i];
            var imgPos = img.transform.position;
            if (imgPos.x < playerPos.x && playerPos.x <= imgPos.x + _ImageWidth &&
                imgPos.y < playerPos.y && playerPos.y <= imgPos.y + _ImageHeight)
            {
                _NowPlayerPosImageIndex = i;
            }
        }

        var nowImagePos = _Images[_NowPlayerPosImageIndex].transform.position;
        var indexDiff = 2;
        // ��ɉ摜�����邩�m�F
        if (_NowPlayerPosImageIndex == 2 || _NowPlayerPosImageIndex == 3)
        {
            indexDiff = -2;
        }

        var pos = _Images[_NowPlayerPosImageIndex + indexDiff].transform.position;
        if (nowImagePos.y > pos.y)
        {
            // ���̉摜�����ɂ���
            if (playerPos.y > nowImagePos.y + (_ImageHeight * 0.1f))
            {
                // ������摜�̔��������Player�����݂���
                return MoveDirection.Up;
            }
        }

        // ���ɉ摜�����邩�m�F
        indexDiff = 1;
        if(_NowPlayerPosImageIndex == 1 || _NowPlayerPosImageIndex == 3)
        {
            indexDiff = -1;
        }

        pos = _Images[_NowPlayerPosImageIndex + indexDiff].transform.position;
        if (nowImagePos.x > pos.x)
        {
            // ���̉摜�����ɂ���
            if (playerPos.x > nowImagePos.x + (_ImageWidth * 0.2f))
            {
                // ������摜�̉E����Player�����݂���
                return MoveDirection.Right;
            }
        }
        else
        {
            // ���̉摜���E�ɂ���
            if (playerPos.x < nowImagePos.x - (_ImageWidth * 0.2f))
            {
                // ������摜�̍�����Player�����݂���
                return MoveDirection.Left;
            }
        }

        return MoveDirection.None;
    }

    private void MoveImage(MoveDirection direction)
    {
        switch (direction) 
        {
            case MoveDirection.None:
                {
                    // �������Ȃ�
                    return;
                }
            case MoveDirection.Up:
                {
                    // [0][1] or [2][3] ��������ɓ�����
                    if (_NowPlayerPosImageIndex == 2 || _NowPlayerPosImageIndex == 3)
                    {
                        _Images[0].transform.position = _Images[2].transform.position + (_ImageMoveVertical * 1.18f);
                        _Images[1].transform.position = _Images[3].transform.position + (_ImageMoveVertical * 1.18f);
                    }
                    else
                    {
                        _Images[2].transform.position = _Images[0].transform.position + (_ImageMoveVertical * 1.18f);
                        _Images[3].transform.position = _Images[1].transform.position + (_ImageMoveVertical * 1.18f);
                    }
                }
                break;
            case MoveDirection.Right:
                {
                    // [1][3] or [0][2] ���E�����ɓ�����
                    if (_NowPlayerPosImageIndex == 0 || _NowPlayerPosImageIndex == 2)
                    {
                        _Images[1].transform.position = _Images[0].transform.position + _ImageMoveHorizontal;
                        _Images[3].transform.position = _Images[2].transform.position + _ImageMoveHorizontal;
                    }
                    else
                    {
                        _Images[0].transform.position = _Images[1].transform.position + _ImageMoveHorizontal;
                        _Images[2].transform.position = _Images[3].transform.position + _ImageMoveHorizontal;
                    }
                }
                break;
            case MoveDirection.Left:
                {
                    // [1][3] or [0][2] ���������ɓ�����
                    if (_NowPlayerPosImageIndex == 0 || _NowPlayerPosImageIndex == 2)
                    {
                        _Images[1].transform.position = _Images[0].transform.position - _ImageMoveHorizontal;
                        _Images[3].transform.position = _Images[2].transform.position - _ImageMoveHorizontal;
                    }
                    else
                    {
                        _Images[0].transform.position = _Images[1].transform.position - _ImageMoveHorizontal;
                        _Images[2].transform.position = _Images[3].transform.position - _ImageMoveHorizontal;
                    }
                }
                break;
        }
    }

    enum MoveDirection
    {
        None = 0,
        Up,
        Right,
        Left
    }
}
