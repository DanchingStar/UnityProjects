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

    /// <summary>現在のカメラ位置名</summary>
    public string CurrentPositionName { get; private set; }

    /// <summary>
    /// カメラ位置情報クラス
    /// </summary>
    private class CameraPositionInfo
    {
        /// <summary>カメラの位置</summary>
        public Vector3 Position { get; set; }
        /// <summary>カメラの角度</summary>
        public Vector3 Rotate { get; set; }
        /// <summary>ボタンの移動先</summary>
        public MoveNames MoveNames { get; set; }
    }

    /// <summary>
    /// ボタンの移動先クラス
    /// </summary>
    private class MoveNames
    {
        /// <summary>左ボタンを押したときの位置名</summary>
        public string Left { get; set; }
        /// <summary>右ボタンを押したときの位置名</summary>
        public string Right { get; set; }
        /// <summary>下ボタンを押したときの位置名</summary>
        public string Back { get; set; }
    }

    [SerializeField] private DoorManager livingRoomDoor;
    [SerializeField] private DoorManager bedRoomDoor;

    [SerializeField] private string defaultPosition;

    private bool isStop = false;
    private bool stopFlg = false;

    /// <summary>
    /// 全カメラ位置情報
    /// </summary>
    private Dictionary<string, CameraPositionInfo> CameraPositionInfoes = new Dictionary<string, CameraPositionInfo>
    {
        {
            "テンプレート", //位置名
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
            "Hallway0", //位置名
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
            "Hallway4", //位置名
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
            "Hallway5", //位置名
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
            "Hallway6", //位置名
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
            "Hallway7", //位置名
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
            "LivingRoom0", //位置名
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
            "LivingRoom1", //位置名
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
            "LivingRoom2", //位置名
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
            "LivingRoom3", //位置名
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
            "LivingRoom4", //位置名
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
            "LivingRoom5", //位置名
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
            "LivingRoom6", //位置名
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
            "LivingRoom7", //位置名
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
            "BedRoom0", //位置名
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
            "BedRoom1", //位置名
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
            "BedRoom2", //位置名
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
            "BedRoom3", //位置名
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
            "BedRoom4", //位置名
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
            "BedRoom5", //位置名
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
            "BedRoom6", //位置名
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
            "BedRoom7", //位置名
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
            "ToiletRoom0", //位置名
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
            "ToiletRoom1", //位置名
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
            "ToiletRoom2", //位置名
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
            "BathRoom0", //位置名
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
            "BathRoom1", //位置名
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
            "BathRoom2", //位置名
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
            "FinalDoorKnob", //位置名
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
            "FinalDoorGimmick", //位置名
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
            "ToiletDoor", //位置名
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
            "HallwayPicture", //位置名
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
            "WoodBox", //位置名
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
            "WBHintCard", //位置名
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
            "GimmickToLivingDoor", //位置名
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
            "GimmickToToiletRoom", //位置名
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
            "LR0Left", //位置名
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
            "LR0LInside", //位置名
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
            "LR0LGimmick", //位置名
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
            "LR0Center", //位置名
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
            "LR0CTopGimmick", //位置名
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
            "LR0CTopInside", //位置名
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
            "LR0CBottomGimmick", //位置名
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
            "LR0CBottomInside", //位置名
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
            "LR0Right", //位置名
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
            "LR0RInside", //位置名
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
            "LR0RGimmick", //位置名
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
            "LR2Books", //位置名
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
            "LR2Hand", //位置名
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
            "LR2TreasureChest", //位置名
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
            "LR3Monitor", //位置名
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
            "LR3Table", //位置名
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
            "LR3TChess", //位置名
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
            "LR3TMugcups", //位置名
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
            "LR4Clock", //位置名
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
            "LR4UnderSofa", //位置名
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
            "LR5Desk", //位置名
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
            "LR5DMonitor", //位置名
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
            "LR5DPC", //位置名
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
            "LR5DDrawerGimmick", //位置名
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
            "LR5DDrawerInside", //位置名
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
            "LR5SideTable", //位置名
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
            "LR5STOthello", //位置名
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
            "LR5STDresser", //位置名
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
            "LR6Candle", //位置名
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
            "LR7WineBottles", //位置名
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
            "LR7Cabinet", //位置名
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
            "LR7CGimmick", //位置名
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
            "LR7CInside", //位置名
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
            "LR7Refrigerator", //位置名
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
            "LR7RGimmick", //位置名
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
            "LR7RLeft", //位置名
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
            "LR7RRight", //位置名
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
            "LR7RBottom", //位置名
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
            "BR0TableEnd", //位置名
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
            "BR0TInside", //位置名
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
            "BR0ShogiBan", //位置名
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
            "BR1Kotatsu", //位置名
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
            "BR3Locker", //位置名
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
            "BR3LockerGimmick", //位置名
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
            "BR3LSmallObjects", //位置名
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
            "BR3LNumberPanelA", //位置名
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
            "BR4Picture", //位置名
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
            "BR5Dresser", //位置名
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
            "BR5DRingCase", //位置名
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
            "BR6SideTable", //位置名
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
            "BR6STopLock", //位置名
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
            "BR6STopInside", //位置名
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
            "BR6SBottomInside", //位置名
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
            "BR7Bed", //位置名
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
            "BR7BUnderPillow", //位置名
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
            "BR7BUHintCard", //位置名
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
            "T1HighPlace", //位置名
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
            "T2ToiletFront", //位置名
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
            "T2TFToiletPaperHolder", //位置名
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
            "T2TFToiletHintPanel", //位置名
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
            "F0PhotoStand", //位置名
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
            "F1BathUnitFront", //位置名
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
            "F1BathUnitInside", //位置名
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
            "F1BUIDrain", //位置名
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
            "F2Showcase", //位置名
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
            "F2SGimmick", //位置名
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
            "F2SFront", //位置名
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
            "F2SInside", //位置名
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
    /// カメラ移動
    /// </summary>
    /// <param name="positionName">カメラ位置名</param>
    public void ChangeCameraPosition(string positionName)
    {
        if (positionName == null) return;

        CurrentPositionName = positionName;

        GetComponent<Camera>().transform.position = CameraPositionInfoes[CurrentPositionName].Position;
        GetComponent<Camera>().transform.rotation = Quaternion.Euler(CameraPositionInfoes[CurrentPositionName].Rotate);

        UpdateButtonActive();
    }

    /// <summary>
    /// カメラ移動
    /// </summary>
    /// <param name="positionName">カメラ位置名</param>
    /// <param name="isUpdateButtonActive">あとでボタンをアクティブさせるときにfalse</param>
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
