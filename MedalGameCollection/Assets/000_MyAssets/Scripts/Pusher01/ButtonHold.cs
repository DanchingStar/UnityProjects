using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SmartBall01
{
    public class ButtonHold : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private FireTrigger trigger;

        public float holdTime;
        public float power;

        private const float ADD_TIME = 1f;
        private const float MAX_TIME = 4f;
        private const float BASE_POWER = 1000f;

        private bool isButtonOn = false;

        private void Start()
        {
            ResetValue();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            StartCoroutine("IncreaseHoldTime");
            SoundManager.Instance.PlaySEOnlyOne(SoundManager.SoundSeType.SpringNow);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            StopCoroutine("IncreaseHoldTime");
            AddPowerToBall(power);
            ResetValue();
        }

        private void ResetValue()
        {
            holdTime = 0f;
            power = 0f;
            isButtonOn = false;
        }

        IEnumerator IncreaseHoldTime()
        {
            isButtonOn = true;

            while (true)
            {
                holdTime += Time.unscaledDeltaTime;

                if (holdTime < MAX_TIME)
                {
                    power = (holdTime + ADD_TIME) * BASE_POWER;
                }
                else
                {
                    power = (MAX_TIME + ADD_TIME) * BASE_POWER;
                }

                yield return null;
            }
        }

        /// <summary>
        /// 球に力を与える
        /// </summary>
        /// <param name="power">力の大きさ</param>
        void AddPowerToBall(float power)
        {
            // Debug.Log("Button held for " + power + " power.");

            if (power > BASE_POWER * (MAX_TIME + ADD_TIME - 1))
            {
                SoundManager.Instance.PlaySEOnlyOne(SoundManager.SoundSeType.SpringL);

            }
            else if (power > BASE_POWER * (ADD_TIME + 1))
            {
                SoundManager.Instance.PlaySEOnlyOne(SoundManager.SoundSeType.SpringM);
            }
            else
            {
                SoundManager.Instance.PlaySEOnlyOne(SoundManager.SoundSeType.SpringS);
            }

            trigger.FireBall(power);
        }

        /// <summary>
        /// powerのゲッター
        /// </summary>
        /// <returns>power</returns>
        public float GetPower()
        {
            return power;
        }

        /// <summary>
        /// basePowerのゲッター
        /// </summary>
        /// <returns>basePower</returns>
        public float GetBasePower()
        {
            return BASE_POWER;
        }

        /// <summary>
        /// isButtonOnのゲッター
        /// </summary>
        /// <returns>isButtonOn</returns>
        public bool GetIsButtonOn()
        {
            return isButtonOn;
        }
    }
}
