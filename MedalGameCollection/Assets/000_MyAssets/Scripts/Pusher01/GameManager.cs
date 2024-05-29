using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Pusher01
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private MedalSpawn medalSpawn;
        [SerializeField] private MedalPayFall medalPayFall;
        [SerializeField] private UIAnimationPanel uIAnimationPanel;
        [SerializeField] private HoryuuLamp[] horyuuLamp;
        [SerializeField] private Lille[] Lilles;
        [SerializeField] private ParticleSystem backgroundParticle;

        [SerializeField] private GameObject settingLightPanel;
        [SerializeField] private GameObject pointLightsObject;
        [SerializeField] private GameObject directionalLightObject;
        [SerializeField] private Text nowQualityText;

        /// <summary> �ۗ��̍ő�l(���I���ۗ̕����܂�) </summary>
        private const int MAX_HORYUU = 10;
        /// <summary> ��x�̔��˂Ŏg�����_���̖��� </summary>
        private const int ONCE_BET_MEDALS = 1;
        /// <summary> �X�[�p�[���I���̃��_�������o������ </summary>
        private const int PAY_MEDALS_SUPER = 200;
        /// <summary> �m�ϓ��I���̃��_�������o������ </summary>
        private const int PAY_MEDALS_KAKUHEN = 100;
        /// <summary> �ʏ퓖�I���̃��_�������o������ </summary>
        private const int PAY_MEDALS_TUUJOU = 100;

        /// <summary> ���_�������˂ł��Ȃ����̃t���O </summary>
        private bool disableMedalStartFlg = false;
        /// <summary> �ۗ��ɓ������Ƃ��ɒ��I���铖����Ȃǂ̃f�[�^ </summary>
        private List<SlotData> slotData;
        /// <summary> �X���b�g����]�ł��邩�̃t���O </summary>
        private bool waitSlot = false;
        /// <summary> ���[�`��Ԃ̃t���O </summary>
        private bool reachFlg = false;
        /// <summary> ���݂̃��[�h�A�ۗ��͂�������Ƃɒ��I���� </summary>
        private Atari naibuMode;
        /// <summary> ���݂̃��[�h�A�w�i��BGM�͂�������ƂɌ��߂� </summary>
        private Atari mitameMode;
        /// <summary> ���݂̌��̐ݒ� true:���i�� , false:��i�� </summary>
        private bool lightQualityFlg;

        private UiSeManager uiSeManager;

        private const string LIGHT_QUALITY_KEY = "SmartBall01_LightQuality";
        private const int LIGHT_QUALITY_HIGH = 1;
        private const int LIGHT_QUALITY_LOW = 0;

        private const int RANDOM_RANGE = 10000;


        /// <summary>
        /// ���I������e
        /// </summary>
        public struct SlotData
        {
            public LilleNumber lilleLeft;
            public LilleNumber lilleCenter;
            public LilleNumber lilleRight;
            public HoryuuColor horyuuColor;
            public Atari atari;
        }

        /// <summary>
        /// ���[���̐���
        /// </summary>
        public enum LilleNumber
        {
            Zero,
            One,
            Two,
            Three,
            Four,
            Five,
            Six,
            Seven,
            Eight,
            Nine,
        }

        /// <summary>
        /// �ۗ��̐F
        /// </summary>
        public enum HoryuuColor
        {
            Normal,
            Blue,
            Green,
            Purple,
            Red,
            None,
        }

        /// <summary>
        /// �����肩�ǂ����Ɠ�����̎��
        /// </summary>
        public enum Atari
        {
            Hazure,
            Reach,
            Tuujou,
            Kakuhen,
            Super,
        }


        public static GameManager Instance;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            Physics.gravity = new Vector3(0, -20f, 0);

            uiSeManager = GameObject.Find("UiSeManager").GetComponent<UiSeManager>();
            InitSettingLight();

            slotData = new List<SlotData>();
            SetNaibuMode(Atari.Tuujou);
            SetMitameMode(Atari.Tuujou);
            UpdateHoryuuColors();
        }

        private void Update()
        {
            RotateSlot();
        }

        /// <summary>
        /// ���_�����X�^�[�g�ł��Ȃ���Ԃ̃t���O�̃Z�b�^�[
        /// </summary>
        /// <param name="b">���Ȃ�{�[�����łĂȂ�</param>
        public void SetDisableMedalStartFlg(bool b)
        {
            disableMedalStartFlg = b;
        }

        /// <summary>
        /// ���_�����X�^�[�g�ł��Ȃ���Ԃ̃t���O�̃Q�b�^�[
        /// </summary>
        /// <returns>���Ȃ�{�[�����łĂȂ�</returns>
        public bool GetDisableMedalStartFlg()
        {
            return disableMedalStartFlg;
        }

        /// <summary>
        /// ���_�����˃{�^�����������Ƃ�
        /// </summary>
        public void PushMedalSpawn()
        {
            if (PlayerInformationManager.Instance.ConsumptionMedal(ONCE_BET_MEDALS))
            {
                medalSpawn.SpawnMedal();
                PlayerInformationManager.Instance.IncrementField("Pusher01Data", "betMedalTotal");
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.MedalIn);
            }
            else
            {

            }
        }

        /// <summary>
        /// �ۗ�����1���₷
        /// </summary>
        /// <param name="cheatFlg">�`�[�g���[�h : true -> �m��哖����</param>
        public void IncrementHoryuu(bool cheatFlg)
        {
            SlotData addData = new SlotData();
            int count = slotData.Count;
            if (count < MAX_HORYUU)
            {
                addData = LotterySlotData(cheatFlg);

                slotData.Add(addData);
                if (!(addData.atari == Atari.Hazure || addData.atari == Atari.Reach))
                {
                    if (naibuMode != addData.atari)
                    {
                        // ���݂̊m���̓������[�h��ύX����
                        SetNaibuMode(addData.atari);
                    }
                }
                horyuuLamp[count].ChangeHoryuuColor(slotData[count].horyuuColor);
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.HoryuuIn);
            }
        }

        /// <summary>
        /// �������ۗ��Ȃǂ𒊑I����
        /// </summary>
        /// /// <param name="cheatFlg">�`�[�g���[�h : true -> �m��哖����</param>
        /// <returns>������Ȃǂ̃f�[�^</returns>
        private SlotData LotterySlotData(bool cheatFlg)
        {
            SlotData nowData = new SlotData();

            int randomNum = Random.Range(0, RANDOM_RANGE);
            int subRandomNum = Random.Range(0, RANDOM_RANGE);

            // �哖����̒��I
            nowData.atari = DecideAtari(naibuMode, randomNum, cheatFlg);

            // ���[���̐��������肷��
            nowData = DecideLilleNumbers(nowData, subRandomNum);

            // �ۗ��̐F�����߂�
            nowData.horyuuColor = DecideHoryuuColor(nowData, subRandomNum);

            return nowData;
        }

        /// <summary>
        /// �哖����𒊑I����
        /// </summary>
        /// <param name="nowMode"></param>
        /// <param name="randomNum"></param>
        /// <param name="cheatFlg"></param>
        /// <returns></returns>
        private Atari DecideAtari(Atari nowMode, int randomNum, bool cheatFlg)
        {
            //�Ԃ�l
            Atari decide = Atari.Hazure;

            //�f�o�b�O�p�`�[�g�t���O�����̂Ƃ�
            if (cheatFlg)
            {
                if (naibuMode == Atari.Tuujou)
                {
                    randomNum /= 100;
                }
                else
                {
                    randomNum /= 10;
                }
            }

            // ���݂̃��[�h�ɍ��킹�ē�����𒊑I����
            if (naibuMode == Atari.Tuujou)
            {
                if (randomNum < 100) //�哖����̂Ƃ��̐U�蕪��
                {
                    if (randomNum < 5)
                    {
                        decide = Atari.Super;
                    }
                    else if (randomNum < 40)
                    {
                        decide = Atari.Kakuhen;
                    }
                    else
                    {
                        decide = Atari.Tuujou;
                    }
                }
                else //�n�Y���̂Ƃ��̐U�蕪��
                {
                    if (randomNum < 1000)
                    {
                        decide = Atari.Reach;
                    }
                    else
                    {
                        decide = Atari.Hazure;
                    }
                }
            }
            else if (naibuMode == Atari.Kakuhen)
            {
                if (randomNum < 1000) //�哖����̂Ƃ��̐U�蕪��
                {
                    if (randomNum < 100)
                    {
                        decide = Atari.Super;
                    }
                    else if (randomNum < 750)
                    {
                        decide = Atari.Kakuhen;
                    }
                    else
                    {
                        decide = Atari.Tuujou;
                    }
                }
                else //�n�Y���̂Ƃ��̐U�蕪��
                {
                    if (randomNum < 2000)
                    {
                        decide = Atari.Reach;
                    }
                    else
                    {
                        decide = Atari.Hazure;
                    }
                }
            }
            else if (naibuMode == Atari.Super)
            {
                if (randomNum < 1000) //�哖����̂Ƃ��̐U�蕪��
                {
                    if (randomNum < 500)
                    {
                        decide = Atari.Super;
                    }
                    else
                    {
                        decide = Atari.Kakuhen;
                    }
                }
                else //�n�Y���̂Ƃ��̐U�蕪��
                {
                    if (randomNum < 2000)
                    {
                        decide = Atari.Reach;
                    }
                    else
                    {
                        decide = Atari.Hazure;
                    }
                }
            }

            return decide;
        }

        /// <summary>
        /// 3�̃��[���̐��������߂�
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private SlotData DecideLilleNumbers(SlotData data,int subRandomNum)
        {
            if (data.atari == Atari.Hazure)
            {
                int leftNum = subRandomNum % 10;
                int centerNum = Random.Range(0, 10);
                int rightNum = leftNum + Random.Range(1, 10);
                if (rightNum >= 10)
                {
                    rightNum -= 10;
                }

                data.lilleLeft = (LilleNumber)leftNum;
                data.lilleCenter = (LilleNumber)centerNum;
                data.lilleRight = (LilleNumber)rightNum;
            }
            else if (data.atari == Atari.Reach)
            {
                int leftNum = subRandomNum % 10;
                if (leftNum == 7)
                {
                    leftNum += Random.Range(1, 10);
                    if (leftNum >= 10)
                    {
                        leftNum -= 10;
                    }
                }
                int rightNum = leftNum;

                int centerNum = leftNum + 1;
                if (Random.Range(0, 2) == 0)
                {
                    centerNum = leftNum + 9;
                }

                if (centerNum >= 10)
                {
                    centerNum -= 10;
                }

                data.lilleLeft = (LilleNumber)leftNum;
                data.lilleCenter = (LilleNumber)centerNum;
                data.lilleRight = (LilleNumber)rightNum;
            }
            else if (data.atari == Atari.Tuujou)
            {
                int leftNum = subRandomNum % 10;
                if (leftNum % 2 == 1)
                {
                    leftNum--;
                }
                int centerNum = leftNum;
                int rightNum = leftNum;

                data.lilleLeft = (LilleNumber)leftNum;
                data.lilleCenter = (LilleNumber)centerNum;
                data.lilleRight = (LilleNumber)rightNum;
            }
            else if (data.atari == Atari.Kakuhen)
            {
                int num = subRandomNum % 4;
                int leftNum;
                switch (num)
                {
                    case 0: leftNum = 1; break;
                    case 1: leftNum = 3; break;
                    case 2: leftNum = 5; break;
                    case 3: leftNum = 9; break;
                    default: leftNum = -1; break;
                }
                int centerNum = leftNum;
                int rightNum = leftNum;

                data.lilleLeft = (LilleNumber)leftNum;
                data.lilleCenter = (LilleNumber)centerNum;
                data.lilleRight = (LilleNumber)rightNum;
            }
            else if (data.atari == Atari.Super)
            {
                int leftNum = 7;
                int centerNum = leftNum;
                int rightNum = leftNum;

                data.lilleLeft = (LilleNumber)leftNum;
                data.lilleCenter = (LilleNumber)centerNum;
                data.lilleRight = (LilleNumber)rightNum;

            }

            return data;
        }

        /// <summary>
        /// �ۗ��̐F�����肷��
        /// </summary>
        /// <param name="nowData"></param>
        /// <param name="subRandomNum"></param>
        /// <param name="leftNum"></param>
        /// <returns></returns>
        private HoryuuColor DecideHoryuuColor(SlotData nowData,int subRandomNum)
        {
            HoryuuColor color = HoryuuColor.Normal;
            bool errorFlg = false;

            if (naibuMode == Atari.Tuujou)
            {
                if (nowData.atari == Atari.Hazure)
                {
                    if (subRandomNum < (RANDOM_RANGE / 50))
                    {
                        color = HoryuuColor.Blue;
                    }
                    else
                    {
                        color = HoryuuColor.Normal;
                    }
                }
                else if (nowData.atari == Atari.Reach)
                {
                    if (subRandomNum < (RANDOM_RANGE / 60))
                    {
                        color = HoryuuColor.Purple;
                    }
                    else if (subRandomNum < (RANDOM_RANGE / 30))
                    {
                        color = HoryuuColor.Green;
                    }
                    else if (subRandomNum < (RANDOM_RANGE / 10))
                    {
                        color = HoryuuColor.Blue;
                    }
                    else
                    {
                        color = HoryuuColor.Normal;
                    }
                }
                else if(nowData.atari == Atari.Tuujou)
                {
                    if (subRandomNum < (RANDOM_RANGE / 50))
                    {
                        color = HoryuuColor.Red;
                    }
                    else if (subRandomNum < (RANDOM_RANGE / 30))
                    {
                        color = HoryuuColor.Purple;
                    }
                    else if (subRandomNum < (RANDOM_RANGE / 10))
                    {
                        color = HoryuuColor.Green;
                    }
                    else if (subRandomNum < (RANDOM_RANGE / 5))
                    {
                        color = HoryuuColor.Blue;
                    }
                    else
                    {
                        color = HoryuuColor.Normal;
                    }
                }
                else if(nowData.atari == Atari.Kakuhen)
                {
                    if (subRandomNum < (RANDOM_RANGE / 30))
                    {
                        color = HoryuuColor.Red;
                    }
                    else if (subRandomNum < (RANDOM_RANGE / 15))
                    {
                        color = HoryuuColor.Purple;
                    }
                    else if (subRandomNum < (RANDOM_RANGE / 5))
                    {
                        color = HoryuuColor.Green;
                    }
                    else if (subRandomNum < (RANDOM_RANGE / 3))
                    {
                        color = HoryuuColor.Blue;
                    }
                    else
                    {
                        color = HoryuuColor.Normal;
                    }
                }
                else if (nowData.atari == Atari.Super)
                {
                    if (subRandomNum < (RANDOM_RANGE / 20))
                    {
                        color = HoryuuColor.Red;
                    }
                    else if (subRandomNum < (RANDOM_RANGE / 10))
                    {
                        color = HoryuuColor.Purple;
                    }
                    else if (subRandomNum < (RANDOM_RANGE / 4))
                    {
                        color = HoryuuColor.Green;
                    }
                    else if (subRandomNum < (RANDOM_RANGE / 2))
                    {
                        color = HoryuuColor.Blue;
                    }
                    else
                    {
                        color = HoryuuColor.Normal;
                    }
                }
                else
                {
                    errorFlg = true;
                }
            }
            else if (naibuMode == Atari.Kakuhen)
            {
                if (nowData.atari == Atari.Hazure)
                {
                    color = HoryuuColor.Normal;
                }
                else if (nowData.atari == Atari.Reach)
                {
                    if (0 == (int)nowData.lilleLeft % 2)
                    {
                        color = HoryuuColor.Normal;
                    }
                    else
                    {
                        if (subRandomNum < (RANDOM_RANGE / 50))
                        {
                            color = HoryuuColor.Purple;
                        }
                        else if (subRandomNum < (RANDOM_RANGE / 20))
                        {
                            color = HoryuuColor.Green;
                        }
                        else if (subRandomNum < (RANDOM_RANGE / 5))
                        {
                            color = HoryuuColor.Blue;
                        }
                        else
                        {
                            color = HoryuuColor.Normal;
                        }
                    }
                }
                else if (nowData.atari == Atari.Tuujou)
                {
                    color = HoryuuColor.Normal;
                }
                else if (nowData.atari == Atari.Kakuhen)
                {
                    if (subRandomNum < (RANDOM_RANGE / 20))
                    {
                        color = HoryuuColor.Red;
                    }
                    else if (subRandomNum < (RANDOM_RANGE / 10))
                    {
                        color = HoryuuColor.Purple;
                    }
                    else if (subRandomNum < (RANDOM_RANGE / 5))
                    {
                        color = HoryuuColor.Green;
                    }
                    else if (subRandomNum < (RANDOM_RANGE / 3))
                    {
                        color = HoryuuColor.Blue;
                    }
                    else
                    {
                        color = HoryuuColor.Normal;
                    }
                }
                else if (nowData.atari == Atari.Super)
                {
                    if (subRandomNum < (RANDOM_RANGE / 10))
                    {
                        color = HoryuuColor.Red;
                    }
                    else if (subRandomNum < (RANDOM_RANGE / 8))
                    {
                        color = HoryuuColor.Purple;
                    }
                    else if (subRandomNum < (RANDOM_RANGE / 4))
                    {
                        color = HoryuuColor.Green;
                    }
                    else if (subRandomNum < (RANDOM_RANGE / 2))
                    {
                        color = HoryuuColor.Blue;
                    }
                    else
                    {
                        color = HoryuuColor.Normal;
                    }
                }
                else
                {
                    errorFlg = true;
                }
            }
            else if(naibuMode == Atari.Super)
            {
                if (nowData.atari == Atari.Super)
                {
                    if (subRandomNum < (RANDOM_RANGE / 10))
                    {
                        color = HoryuuColor.Red;
                    }
                    else
                    {
                        color = HoryuuColor.Normal;
                    }
                }
                else
                {
                    color = HoryuuColor.Normal;
                }

            }
            else
            {
                errorFlg = true;
            }

            if (errorFlg)
            {
                Debug.LogError($"GameManager.DecideHoryuuColor : naibuMode is {naibuMode} , nowData is {nowData}");
            }

            return color;
        }

        /// <summary>
        /// �X���b�g����
        /// </summary>
        private void RotateSlot()
        {
            if (slotData.Count <= 0) return;
            if (waitSlot == true) return;

            StartCoroutine(RotateSlotCoroutine());
        }

        /// <summary>
        /// �X���b�g�̃R���[�`��
        /// </summary>
        /// <returns></returns>
        private IEnumerator RotateSlotCoroutine()
        {
            waitSlot = true;
            reachFlg = (slotData[0].lilleLeft == slotData[0].lilleRight) ? true : false;

            UpdateHoryuuColors();

            // ���[���̉�]���J�n����
            Lilles[0].StartSpin();
            Lilles[1].StartSpin();
            Lilles[2].StartSpin();
            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.SlotRotate);

            if (slotData.Count > 6)
            {
                yield return new WaitForSeconds(0.5f);
            }
            else if (slotData.Count > 3)
            {
                yield return new WaitForSeconds(2);
            }
            else
            {
                yield return new WaitForSeconds(4);
            }

            // �����[�����~�߂�
            Lilles[0].StopNumber((int)slotData[0].lilleLeft, false, (int)slotData[0].lilleLeft);

            while (!Lilles[0].GetFinishFlg())
            {
                yield return null;
            }

            // �E���[�����~�߂�
            Lilles[2].StopNumber((int)slotData[0].lilleRight, false, (int)slotData[0].lilleLeft);

            while (!Lilles[2].GetFinishFlg())
            {
                yield return null;
            }

            bool myReachFlg = reachFlg;

            //���[�`���o
            if (reachFlg)
            {
                //���[�`���o�p�̃X�N���v�g�։��o���˗�����
                uIAnimationPanel.ReachAnimation();

                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Reach);

                while (reachFlg)
                {
                    yield return null;
                }
            }

            // �����[�����~�߂�
            Lilles[1].StopNumber((int)slotData[0].lilleCenter, myReachFlg, (int)slotData[0].lilleLeft);

            while (!Lilles[1].GetFinishFlg())
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);

            // �n�Y�ꂽ�Ƃ�
            if (slotData[0].atari == Atari.Hazure || slotData[0].atari == Atari.Reach)
            {
                horyuuLamp[0].ChangeHoryuuColor(HoryuuColor.None);
            }
            // ���������Ƃ�
            else
            {
                yield return new WaitForSeconds(1.0f);

                if (mitameMode != slotData[0].atari)
                {
                    // ���݂̊m���̌����ڂ̃��[�h��ύX����
                    SetMitameMode(slotData[0].atari);
                }

                // ������̎�ނɂ���ĕ����o�����������߂ē`����
                if(slotData[0].atari == Atari.Super)
                {
                    uIAnimationPanel.AcquisitionMedalAnimation(PAY_MEDALS_SUPER);
                    medalPayFall.AddFallMedals(PAY_MEDALS_SUPER);
                    SoundManager.Instance.PlaySE(SoundManager.SoundSeType.GetTuujou);
                }
                else if (slotData[0].atari == Atari.Kakuhen)
                {
                    uIAnimationPanel.AcquisitionMedalAnimation(PAY_MEDALS_KAKUHEN);
                    medalPayFall.AddFallMedals(PAY_MEDALS_KAKUHEN);
                    SoundManager.Instance.PlaySE(SoundManager.SoundSeType.GetKakuhen);
                }
                else if (slotData[0].atari == Atari.Tuujou)
                {
                    uIAnimationPanel.AcquisitionMedalAnimation(PAY_MEDALS_TUUJOU);
                    medalPayFall.AddFallMedals(PAY_MEDALS_TUUJOU);
                    SoundManager.Instance.PlaySE(SoundManager.SoundSeType.GetSuper);
                }

                PlayerInformationManager.Instance.IncrementField("Pusher01Data", "bonusTimes");

                yield return new WaitForSeconds(0.5f);
                horyuuLamp[0].ChangeHoryuuColor(HoryuuColor.None);
            }

            yield return new WaitForSeconds(1.0f);

            slotData.RemoveAt(0);
            waitSlot = false;

        }

        /// <summary>
        /// �ۗ��̐F�����ׂčX�V����
        /// </summary>
        private void UpdateHoryuuColors()
        {
            for (int i = 0; i < horyuuLamp.Length; i++) 
            {
                if (i < slotData.Count)
                {
                    horyuuLamp[i].ChangeHoryuuColor(slotData[i].horyuuColor);
                }
                else
                {
                    horyuuLamp[i].ChangeHoryuuColor(HoryuuColor.None);
                }
            }
        }

        /// <summary>
        /// reachFlg�̃Z�b�^�[
        /// </summary>
        /// <param name="flg">�ς������t���O</param>
        public void SetReachFlg(bool flg)
        {
            reachFlg = flg;
        }

        /// <summary>
        /// �����ڃ��[�h��ύX����
        /// </summary>
        /// <param name="mode"></param>
        private void SetMitameMode(Atari mode)
        {
            mitameMode = mode;
            Debug.Log($"�����ڃ��[�h�ύX : {mitameMode}");

            // BGM�̕ύX
            if (mode == Atari.Tuujou)
            {
                SoundManager.Instance.ChangeAndPlayBgm(SoundManager.SoundBgmType.Tuujou);
            }
            else if (mode == Atari.Kakuhen)
            {
                SoundManager.Instance.ChangeAndPlayBgm(SoundManager.SoundBgmType.Kakuhen);
            }
            else if (mode == Atari.Super)
            {
                SoundManager.Instance.ChangeAndPlayBgm(SoundManager.SoundBgmType.Super);
            }

            //�ύX����w�i�F������
            Color color = Color.white;
            if (mode == Atari.Tuujou)
            {
                color = Color.blue;
            }
            else if (mode == Atari.Kakuhen)
            {
                color = Color.red;
            }
            else if (mode == Atari.Super)
            {
                color = Color.yellow;
            }

            //�O���f�[�V�����̍쐬
            Gradient grad = new Gradient();
            grad.SetKeys(
                new GradientColorKey[] { new GradientColorKey(color, 0.0f) },
                new GradientAlphaKey[]
                {
                    new GradientAlphaKey(0.0f, 0.0f),
                    new GradientAlphaKey(1.0f, 0.2f),
                    new GradientAlphaKey(1.0f, 0.8f),
                    new GradientAlphaKey(0.0f, 1.0f)
                });

            //ColorOverLifetime���㏑��
            var colt = backgroundParticle.colorOverLifetime;
            colt.color = new ParticleSystem.MinMaxGradient(grad);
        }

        /// <summary>
        /// �������[�h��ύX����
        /// </summary>
        /// <param name="mode"></param>
        private void SetNaibuMode(Atari mode)
        {
            naibuMode = mode;
            Debug.Log($"�������[�h�ύX : {naibuMode}");
        }

        /// <summary>
        /// ���̐ݒ�{�^�����������Ƃ�
        /// </summary>
        public void PushSettingLightButton()
        {
            uiSeManager.PlaySE(UiSeManager.SoundSeType.SE_Yes);

            if (lightQualityFlg)
            {
                nowQualityText.text = "���݂̐ݒ�\n[���i��]";
            }
            else
            {
                nowQualityText.text = "���݂̐ݒ�\n[��i��]";
            }

            settingLightPanel.SetActive(true);
        }

        /// <summary>
        /// ���̕i����I�ԃ{�^�����������Ƃ�
        /// </summary>
        /// <param name="flg">true:���i�� , false:��i��</param>
        public void PushLightQualityButton(bool flg)
        {
            if (flg)
            {
                lightQualityFlg = true;
                pointLightsObject.SetActive(true);
                directionalLightObject.SetActive(false);
                PlayerPrefs.SetInt(LIGHT_QUALITY_KEY, LIGHT_QUALITY_HIGH);
            }
            else
            {
                lightQualityFlg = false;
                pointLightsObject.SetActive(false);
                directionalLightObject.SetActive(true);
                PlayerPrefs.SetInt(LIGHT_QUALITY_KEY, LIGHT_QUALITY_LOW);
            }

            uiSeManager.PlaySE(UiSeManager.SoundSeType.SE_OK);
            settingLightPanel.SetActive(false);
        }

        /// <summary>
        /// ���̐ݒ�̏�����
        /// </summary>
        private void InitSettingLight()
        {
            int flgInt = PlayerPrefs.GetInt(LIGHT_QUALITY_KEY, LIGHT_QUALITY_LOW);

            if (flgInt == LIGHT_QUALITY_HIGH)
            {
                lightQualityFlg = true;
                pointLightsObject.SetActive(true);
                directionalLightObject.SetActive(false);
            }
            else
            {
                lightQualityFlg = false;
                pointLightsObject.SetActive(false);
                directionalLightObject.SetActive(true);
            }

            settingLightPanel.SetActive(false);
        }

    }
}

