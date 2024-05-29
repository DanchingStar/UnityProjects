using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance { get; private set; }

    public GameObject ButtonBack;
    public GameObject ButtonLeft;
    public GameObject ButtonRight;

    /// <summary>���݂̃J�����ʒu��</summary>
    public string CurrentPositionName { get; private set; }

    /// <summary>
    /// �J�����ʒu���N���X
    /// </summary>
    private class CameraPositionInfo
    {
        /// <summary>�J�����̈ʒu</summary>
        public Vector3 Position { get; set; }
        /// <summary>�J�����̊p�x</summary>
        public Vector3 Rotate { get; set; }
        /// <summary>�{�^���̈ړ���</summary>
        public MoveNames MoveNames { get; set; }
    }

    /// <summary>
    /// �{�^���̈ړ���N���X
    /// </summary>
    private class MoveNames
    {
        /// <summary>���{�^�����������Ƃ��̈ʒu��</summary>
        public string Left { get; set; }
        /// <summary>�E�{�^�����������Ƃ��̈ʒu��</summary>
        public string Right { get; set; }
        /// <summary>���{�^�����������Ƃ��̈ʒu��</summary>
        public string Back { get; set; }
    }

    [SerializeField] private DoorManager livingRoomDoor;
    [SerializeField] private DoorManager bedRoomDoor;

    [SerializeField] private string defaultPosition;

    private bool isStop = false;
    private bool stopFlg = false;

    /// <summary>
    /// �S�J�����ʒu���
    /// </summary>
    private Dictionary<string, CameraPositionInfo> CameraPositionInfoes = new Dictionary<string, CameraPositionInfo>
    {
        {
            "�e���v���[�g", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(0,2,0),
                Rotate = new Vector3(10,0,0),
                MoveNames = new MoveNames
                {
                    Left = "",
                    Right = "",
                    Back = "",
                }
            }
        },

#region MainRoad

        {
            "Hallway0", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(0,2,9),
                Rotate = new Vector3(10,0,0),
                MoveNames = new MoveNames
                {
                    Left = "Hallway7",
                    //Right = "",
                    Back = "LivingRoom1",
                }
            }
        },
        {
            "Hallway4", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(0,2,9),
                Rotate = new Vector3(10,180,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    Right = "Hallway5",
                    //Back = "",
                }
            }
        },
        {
            "Hallway5", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(0,2,9),
                Rotate = new Vector3(10,225,0),
                MoveNames = new MoveNames
                {
                    Left = "Hallway4",
                    Right = "Hallway6",
                    //Back = "",
                }
            }
        },
        {
            "Hallway6", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(0,2,9),
                Rotate = new Vector3(10,270,0),
                MoveNames = new MoveNames
                {
                    Left = "Hallway5",
                    Right = "Hallway7",
                    //Back = "",
                }
            }
        },
        {
            "Hallway7", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(0,2,9),
                Rotate = new Vector3(10,315,0),
                MoveNames = new MoveNames
                {
                    Left = "Hallway6",
                    Right = "Hallway0",
                    //Back = "",
                }
            }
        },
        {
            "LivingRoom0", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-4,2,-2.5f),
                Rotate = new Vector3(10,0,0),
                MoveNames = new MoveNames
                {
                    Left = "LivingRoom7",
                    Right = "LivingRoom1",
                    //Back = "",
                }
            }
        },
        {
            "LivingRoom1", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-4,2,-2.5f),
                Rotate = new Vector3(10,45,0),
                MoveNames = new MoveNames
                {
                    Left = "LivingRoom0",
                    Right = "LivingRoom2",
                    //Back = "",
                }
            }
        },
        {
            "LivingRoom2", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-4,2,-2.5f),
                Rotate = new Vector3(10,90,0),
                MoveNames = new MoveNames
                {
                    Left = "LivingRoom1",
                    Right = "LivingRoom3",
                    Back = "BedRoom2",
                }
            }
        },
        {
            "LivingRoom3", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-4,2,-2.5f),
                Rotate = new Vector3(10,135,0),
                MoveNames = new MoveNames
                {
                    Left = "LivingRoom2",
                    Right = "LivingRoom4",
                    //Back = "",
                }
            }
        },
        {
            "LivingRoom4", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-4,2,-2.5f),
                Rotate = new Vector3(10,180,0),
                MoveNames = new MoveNames
                {
                    Left = "LivingRoom3",
                    Right = "LivingRoom5",
                    //Back = "",
                }
            }
        },
        {
            "LivingRoom5", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-4,2,-2.5f),
                Rotate = new Vector3(10,225,0),
                MoveNames = new MoveNames
                {
                    Left = "LivingRoom4",
                    Right = "LivingRoom6",
                    //Back = "",
                }
            }
        },
        {
            "LivingRoom6", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-4,2,-2.5f),
                Rotate = new Vector3(10,270,0),
                MoveNames = new MoveNames
                {
                    Left = "LivingRoom5",
                    Right = "LivingRoom7",
                    //Back = "",
                }
            }
        },
        {
            "LivingRoom7", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-4,2,-2.5f),
                Rotate = new Vector3(10,315,0),
                MoveNames = new MoveNames
                {
                    Left = "LivingRoom6",
                    Right = "LivingRoom0",
                    //Back = "",
                }
            }
        },
        {
            "BedRoom0", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-13.5f,2,-2.5f),
                Rotate = new Vector3(10,0,0),
                MoveNames = new MoveNames
                {
                    Left = "BedRoom7",
                    Right = "BedRoom1",
                    //Back = "",
                }
            }
        },
        {
            "BedRoom1", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-13.5f,2,-2.5f),
                Rotate = new Vector3(10,45,0),
                MoveNames = new MoveNames
                {
                    Left = "BedRoom0",
                    Right = "BedRoom2",
                    //Back = "",
                }
            }
        },
        {
            "BedRoom2", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-13.5f,2,-2.5f),
                Rotate = new Vector3(10,90,0),
                MoveNames = new MoveNames
                {
                    Left = "BedRoom1",
                    Right = "BedRoom3",
                    //Back = "",
                }
            }
        },
        {
            "BedRoom3", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-13.5f,2,-2.5f),
                Rotate = new Vector3(10,135,0),
                MoveNames = new MoveNames
                {
                    Left = "BedRoom2",
                    Right = "BedRoom4",
                    //Back = "",
                }
            }
        },
        {
            "BedRoom4", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-13.5f,2,-2.5f),
                Rotate = new Vector3(10,180,0),
                MoveNames = new MoveNames
                {
                    Left = "BedRoom3",
                    Right = "BedRoom5",
                    //Back = "",
                }
            }
        },
        {
            "BedRoom5", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-13.5f,2,-2.5f),
                Rotate = new Vector3(10,225,0),
                MoveNames = new MoveNames
                {
                    Left = "BedRoom4",
                    Right = "BedRoom6",
                    //Back = "",
                }
            }
        },
        {
            "BedRoom6", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-13.5f,2,-2.5f),
                Rotate = new Vector3(10,270,0),
                MoveNames = new MoveNames
                {
                    Left = "BedRoom5",
                    Right = "BedRoom7",
                    Back = "LivingRoom6",
                }
            }
        },
        {
            "BedRoom7", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-13.5f,2,-2.5f),
                Rotate = new Vector3(10,315,0),
                MoveNames = new MoveNames
                {
                    Left = "BedRoom6",
                    Right = "BedRoom0",
                    //Back = "",
                }
            }
        },
        {
            "ToiletRoom0", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-5.5f,2,5),
                Rotate = new Vector3(10,270,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    Right = "ToiletRoom1",
                    Back = "Hallway5",
                }
            }
        },
        {
            "ToiletRoom1", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-5.5f,2,5),
                Rotate = new Vector3(10,315,0),
                MoveNames = new MoveNames
                {
                    Left = "ToiletRoom0",
                    Right = "ToiletRoom2",
                    Back = "Hallway5",
                }
            }
        },
        {
            "ToiletRoom2", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-5.5f,2,5),
                Rotate = new Vector3(10,0,0),
                MoveNames = new MoveNames
                {
                    Left = "ToiletRoom1",
                    //Right = "",
                    Back = "Hallway5",
                }
            }
        },
                {
            "BathRoom0", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-13,2,5),
                Rotate = new Vector3(10,270,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    Right = "BathRoom1",
                    Back = "ToiletRoom0",
                }
            }
        },
        {
            "BathRoom1", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-13,2,5),
                Rotate = new Vector3(10,315,0),
                MoveNames = new MoveNames
                {
                    Left = "BathRoom0",
                    Right = "BathRoom2",
                    Back = "ToiletRoom0",
                }
            }
        },
        {
            "BathRoom2", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-13,2,5),
                Rotate = new Vector3(10,0,0),
                MoveNames = new MoveNames
                {
                    Left = "BathRoom1",
                    //Right = "",
                    Back = "ToiletRoom0",
                }
            }
        },
        #endregion

#region Hallway

         {
            "FinalDoorKnob", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(0.1f,2,12),
                Rotate = new Vector3(25,15,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "Hallway0",
                }
            }
         },
         {
            "FinalDoorGimmick", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(0.61f,1.28f,13.3f),
                Rotate = new Vector3(40,0,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "FinalDoorKnob",
                }
            }
        },
        {
            "ToiletDoor", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(0,2,5),
                Rotate = new Vector3(10,270,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "Hallway5",
                }
            }
        },
        {
            "HallwayPicture", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-2f,2.3f,9),
                Rotate = new Vector3(0,270,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "Hallway6",
                }
            }
        },
        {
            "WoodBox", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-2.15f,1.3f,11.5f),
                Rotate = new Vector3(30,330,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "Hallway7",
                }
            }
        },
        {
            "WBHintCard", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-2.83f,0.45f,12.7f),
                Rotate = new Vector3(60,340,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "WoodBox",
                }
            }
        },
        {
            "GimmickToLivingDoor", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-1.25f,2,5),
                Rotate = new Vector3(0,180,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "Hallway4",
                }
            }
        },
        {
            "GimmickToToiletRoom", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-2.25f,1.68f,5.01f),
                Rotate = new Vector3(5,270,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "ToiletDoor",
                }
            }
        },


        #endregion

#region LivingRoom

        {
            "LR0Left", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-4.85f,1.45f,1.4f),
                Rotate = new Vector3(30,0,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "LivingRoom0",
                }
            }
        },
        {
            "LR0LInside", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-4.85f,0.5f,2.6f),
                Rotate = new Vector3(30,0,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "LR0Left",
                }
            }
        },
        {
            "LR0LGimmick", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-4.85f,0.5f,1.7f),
                Rotate = new Vector3(5,0,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "LR0Left",
                }
            }
        },
        {
            "LR0Center", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-4,1.45f,1.4f),
                Rotate = new Vector3(30,0,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "LivingRoom0",
                }
            }
        },
        {
            "LR0CTopGimmick", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-4,0.65f,2.35f),
                Rotate = new Vector3(20,0,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "LR0Center",
                }
            }
        },
        {
            "LR0CTopInside", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-4,1.04f,2.38f),
                Rotate = new Vector3(60,0,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "LR0Center",
                }
            }
        },
        {
            "LR0CBottomGimmick", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-4,0.4f,2.35f),
                Rotate = new Vector3(20,0,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "LR0Center",
                }
            }
        },
        {
            "LR0CBottomInside", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-4,0.8f,2.39f),
                Rotate = new Vector3(60,0,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "LR0Center",
                }
            }
        },
        {
            "LR0Right", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-3.15f,1.45f,1.4f),
                Rotate = new Vector3(30,0,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "LivingRoom0",
                }
            }
        },
        {
            "LR0RInside", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-3.15f,0.5f,2.6f),
                Rotate = new Vector3(30,0,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "LR0Right",
                }
            }
        },
        {
            "LR0RGimmick", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-3.14f,0.5f,1.7f),
                Rotate = new Vector3(5,0,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "LR0Right",
                }
            }
        },
        {
            "LR2Books", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-0.65f,2.5f,-1.87f),
                Rotate = new Vector3(10,90,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "LivingRoom2",
                }
            }
        },
        {
            "LR2Hand", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-0.35f,1.9f,-2.78f),
                Rotate = new Vector3(10,90,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "LivingRoom2",
                }
            }
        },
        {
            "LR2TreasureChest", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-0.75f,1.9f,-2.76f),
                Rotate = new Vector3(65,90,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "LivingRoom2",
                }
            }
        },
        {
            "LR3Monitor", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-3,2.3f,-6.78f),
                Rotate = new Vector3(2,90,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "LivingRoom3",
                }
            }
        },
        {
            "LR3Table", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-2.75f,2.9f,-5.45f),
                Rotate = new Vector3(50,135,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "LivingRoom3",
                }
            }
        },
        {
            "LR3TChess", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-1.99f,2.16f,-6.14f),
                Rotate = new Vector3(85,135,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "LR3Table",
                }
            }
        },
        {
            "LR3TMugcups", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-1.705f,2.05f,-7.08f),
                Rotate = new Vector3(85,180,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "LR3Table",
                }
            }
        },
        {
            "LR4Clock", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-3.75f,2.25f,-7.5f),
                Rotate = new Vector3(340,180,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "LivingRoom4",
                }
            }
        },
        {
            "LR4UnderSofa", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-3.88f,0.55f,-3.55f),
                Rotate = new Vector3(10,180,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "LivingRoom4",
                }
            }
        },
        {
            "LR5Desk", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-4.5f,2.7f,-6.9f),
                Rotate = new Vector3(30,270,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "LivingRoom5",
                }
            }
        },
        {
            "LR5DMonitor", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-6.7f,1.9f,-7.28f),
                Rotate = new Vector3(10,270,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "LR5Desk",
                }
            }
        },
        {
            "LR5DPC", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-7.4f,1.64f,-6.62f),
                Rotate = new Vector3(10,270,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "LR5Desk",
                }
            }
        },
        {
            "LR5DDrawerGimmick", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-6.85f,0.9f,-6.305f),
                Rotate = new Vector3(5,270,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "LR5Desk",
                }
            }
        },
        {
            "LR5DDrawerInside", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-6.95f,1.93f,-6.305f),
                Rotate = new Vector3(70,270,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "LR5Desk",
                }
            }
        },
        {
            "LR5SideTable", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-6.9f,1.34f,-4.73f),
                Rotate = new Vector3(30,270,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "LivingRoom5",
                }
            }
        },
        {
            "LR5STOthello", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-8.04f,1.82f,-4.73f),
                Rotate = new Vector3(85,270,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "LR5SideTable",
                }
            }
        },
        {
            "LR5STDresser", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-7.31f,1.25f,-4.73f),
                Rotate = new Vector3(50,270,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "LR5SideTable",
                }
            }
        },
        {
            "LR6Candle", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-8.0f,2.22f,-1.7f),
                Rotate = new Vector3(30,300,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "LivingRoom6",
                }
            }
        },
        {
            "LR7WineBottles", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-6.3f,1.9f,1f),
                Rotate = new Vector3(-15,270,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "LivingRoom7",
                }
            }
        },
        {
            "LR7Cabinet", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-6.55f,2.1f,0.87f),
                Rotate = new Vector3(30,270,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "LivingRoom7",
                }
            }
        },
        {
            "LR7CGimmick", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-8.1f,2.01f,0.87f),
                Rotate = new Vector3(80,270,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "LR7Cabinet",
                }
            }
        },
        {
            "LR7CInside", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-7.6f,1.6f,0.87f),
                Rotate = new Vector3(60,270,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "LR7Cabinet",
                }
            }
        },
        {
            "LR7Refrigerator", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-4.9f,2.45f,2.8f),
                Rotate = new Vector3(20,270,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "LivingRoom7",
                }
            }
        },
        {
            "LR7RGimmick", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-7.2f,1.9f,2.526f),
                Rotate = new Vector3(5,270,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "LR7Refrigerator",
                }
            }
        },
        {
            "LR7RLeft", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-6.5f,2.3f,3.1f),
                Rotate = new Vector3(20,230,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "LR7Refrigerator",
                }
            }
        },
        {
            "LR7RRight", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-6.5f,2.3f,2.5f),
                Rotate = new Vector3(20,310,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "LR7Refrigerator",
                }
            }
        },
        {
            "LR7RBottom", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-7.1f,2.3f,2.8f),
                Rotate = new Vector3(80,270,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "LR7Refrigerator",
                }
            }
        },


        #endregion

#region BedRoom

        {
            "BR0TableEnd", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-14.4f,1.76f,1.4f),
                Rotate = new Vector3(30,0,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "BedRoom0",
                }
            }
        },
        {
            "BR0TInside", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-14.4f,1.34f,2.47f),
                Rotate = new Vector3(60,0,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "BR0TableEnd",
                }
            }
        },
        {
            "BR0ShogiBan", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-12.6f,1.55f,3.0f),
                Rotate = new Vector3(80,0,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "BedRoom0",
                }
            }
        },
        {
            "BR1Kotatsu", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-10.75f,3.9f,0.25f),
                Rotate = new Vector3(85,45,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "BedRoom1",
                }
            }
        },
        {
            "BR3Locker", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-10.2f,3.2f,-3.28f),
                Rotate = new Vector3(18,180,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "BedRoom3",
                }
            }
        },
        {
            "BR3LockerGimmick", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-10.695f,1.95f,-6.19f),
                Rotate = new Vector3(5,180,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "BR3Locker",
                }
            }
        },
        {
            "BR3LSmallObjects", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-10.2f,2.8f,-6.34f),
                Rotate = new Vector3(45,180,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "BR3Locker",
                }
            }
        },
        {
            "BR3LNumberPanelA", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-10.2f,2.35f,-6.9f),
                Rotate = new Vector3(5,180,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "BR3Locker",
                }
            }
        },
        {
            "BR4Picture", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-13.5f,2.6f,-6.6f),
                Rotate = new Vector3(-5,180,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "BedRoom4",
                }
            }
        },
        {
            "BR5Dresser", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-14.5f,1.9f,-7.32f),
                Rotate = new Vector3(15,270,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "BedRoom5",
                }
            }
        },
        {
            "BR5DRingCase", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-17.2f,1.8f,-7.32f),
                Rotate = new Vector3(30,270,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "BR5Dresser",
                }
            }
        },
        {
            "BR6SideTable", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-15.7f,1.9f,-2.5f),
                Rotate = new Vector3(30,270,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "BedRoom6",
                }
            }
        },
        {
            "BR6STopLock", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-17.3f,0.8f,-2.5f),
                Rotate = new Vector3(10,270,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "BR6SideTable",
                }
            }
        },
        {
            "BR6STopInside", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-17.3f,1.48f,-2.5f),
                Rotate = new Vector3(70,270,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "BR6SideTable",
                }
            }
        },
        {
            "BR6SBottomInside", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-17.45f,0.55f,-2.5f),
                Rotate = new Vector3(70,270,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "BR6SideTable",
                }
            }
        },
        {
            "BR7Bed", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-15.9f,2.2f,0.7f),
                Rotate = new Vector3(30,315,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "BedRoom7",
                }
            }
        },
        {
            "BR7BUnderPillow", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-17.63f,2.52f,2.17f),
                Rotate = new Vector3(70,0,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "BR7Bed",
                }
            }
        },
        {
            "BR7BUHintCard", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-17.34f,1.2f,2.48f),
                Rotate = new Vector3(80,0,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "BR7BUnderPillow",
                }
            }
        },


#endregion

#region ToiletRoom

        {
            "T1HighPlace", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-7.7f,1.5f,10.4f),
                Rotate = new Vector3(350,310,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "ToiletRoom1",
                }
            }
        },
        {
            "T2ToiletFront", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-4.3f,1.64f,9.9f),
                Rotate = new Vector3(10,0,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "ToiletRoom2",
                }
            }
        },
        {
            "T2TFToiletPaperHolder", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-5,1.64f,10.9f),
                Rotate = new Vector3(10,30,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "T2ToiletFront",
                }
            }
        },
        {
            "T2TFToiletHintPanel", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-4.6f,1.25f,12.7f),
                Rotate = new Vector3(5,0,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "T2ToiletFront",
                }
            }
        },


        #endregion

#region BathRoom

        {
            "F0PhotoStand", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-17.15f,1.55f,5f),
                Rotate = new Vector3(40,270,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "BathRoom0",
                }
            }
        },
        {
            "F1BathUnitFront", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-16.1f,2.34f,11.24f),
                Rotate = new Vector3(10,270,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "BathRoom1",
                }
            }
        },
        {
            "F1BathUnitInside", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-17f,2.34f,11.24f),
                Rotate = new Vector3(45,315,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "BathRoom1",
                }
            }
        },
        {
            "F1BUIDrain", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-17.51f,0.65f,12.56f),
                Rotate = new Vector3(45,315,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "F1BathUnitInside",
                }
            }
        },
        {
            "F2Showcase", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-12.1f,2.05f,9.6f),
                Rotate = new Vector3(15,0,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "BathRoom2",
                }
            }
        },
        {
            "F2SGimmick", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-12.1f,1.15f,12f),
                Rotate = new Vector3(5,0,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "F2Showcase",
                }
            }
        },
        {
            "F2SFront", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-12.1f,1.86f,11.6f),
                Rotate = new Vector3(10,0,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "F2Showcase",
                }
            }
        },
        {
            "F2SInside", //�ʒu��
            new CameraPositionInfo
            {
                Position = new Vector3(-12.1f,1.72f,12.86f),
                Rotate = new Vector3(50,0,0),
                MoveNames = new MoveNames
                {
                    //Left = "",
                    //Right = "",
                    Back = "F2SFront",
                }
            }
        },



#endregion 

    };

    void Start()
    {
        instance = this;

        ChangeCameraPosition(defaultPosition);

        ButtonBack.GetComponent<Button>().onClick.AddListener(() =>
        {
            ChangeCameraPosition(CameraPositionInfoes[CurrentPositionName].MoveNames.Back);
        });
        ButtonLeft.GetComponent<Button>().onClick.AddListener(() =>
        {
            ChangeCameraPosition(CameraPositionInfoes[CurrentPositionName].MoveNames.Left);
        });
        ButtonRight.GetComponent<Button>().onClick.AddListener(() =>
        {
            ChangeCameraPosition(CameraPositionInfoes[CurrentPositionName].MoveNames.Right);
        });

    }

    private void Update()
    {
        isStop = ControlStopper.instance.GetIsControlStop();
        if (isStop == true)
        {
            ButtonLeft.SetActive(false);
            ButtonRight.SetActive(false);
            ButtonBack.SetActive(false);
            stopFlg = true;
        }
        else if (isStop == false && stopFlg == true) 
        {
            UpdateButtonActive();
            stopFlg = false;
        }
    }



    /// <summary>
    /// �J�����ړ�
    /// </summary>
    /// <param name="positionName">�J�����ʒu��</param>
    public void ChangeCameraPosition(string positionName)
    {
        if (positionName == null) return;

        CurrentPositionName = positionName;

        GetComponent<Camera>().transform.position = CameraPositionInfoes[CurrentPositionName].Position;
        GetComponent<Camera>().transform.rotation = Quaternion.Euler(CameraPositionInfoes[CurrentPositionName].Rotate);

        UpdateButtonActive();
    }

    /// <summary>
    /// �J�����ړ�
    /// </summary>
    /// <param name="positionName">�J�����ʒu��</param>
    /// <param name="isUpdateButtonActive">���ƂŃ{�^�����A�N�e�B�u������Ƃ���false</param>
    public void ChangeCameraPosition(string positionName,bool isUpdateButtonActive)
    {
        if (positionName == null) return;

        CurrentPositionName = positionName;

        GetComponent<Camera>().transform.position = CameraPositionInfoes[CurrentPositionName].Position;
        GetComponent<Camera>().transform.rotation = Quaternion.Euler(CameraPositionInfoes[CurrentPositionName].Rotate);

        if (isUpdateButtonActive)
        {
            UpdateButtonActive();
        }
    }

    private void UpdateButtonActive()
    {
        if (CameraPositionInfoes[CurrentPositionName].MoveNames.Left == null)
            ButtonLeft.SetActive(false);
        else
            ButtonLeft.SetActive(true);

        if (CameraPositionInfoes[CurrentPositionName].MoveNames.Right == null)
            ButtonRight.SetActive(false);
        else
            ButtonRight.SetActive(true);


        if (CurrentPositionName == "Hallway0")
        {
            if (livingRoomDoor.getDoorOpen() == false)
                ButtonBack.SetActive(false);
            else
                ButtonBack.SetActive(true);
        }
        else if (CurrentPositionName == "LivingRoom2")
        {
            if (bedRoomDoor.getDoorOpen() == false)
                ButtonBack.SetActive(false);
            else
                ButtonBack.SetActive(true);
        }
        else
        {
            if (CameraPositionInfoes[CurrentPositionName].MoveNames.Back == null)
                ButtonBack.SetActive(false);
            else
                ButtonBack.SetActive(true);
        }
    }
}
