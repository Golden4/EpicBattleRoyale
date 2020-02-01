using System.Linq.Expressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthArmorUI : MonoBehaviour
{
    [System.Serializable]
    public class BarUI
    {
        public bool needBlinkWhenLowAmount;
        public Color blinkColor;
        Color origBgColor;

        HealthSystem healthSystem;
        public Image barImage;
        public Image barDamageImage;
        public Image barHealImage;
        public Image barBg;
        float targetAmount;
        float targetDamagedAmount;
        float targetHealedAmount;
        int maxAmount;

        int curAmount;

        bool isAnimatingDamage;

        bool isAnimatingHeal;
        float waitingTimer = .5f;
        float curWaitingTimer;

        float animatingSpeed = 1f;

        public void Init(int maxAmount)
        {
            this.maxAmount = maxAmount;
            origBgColor = barBg.color;
        }

        public void SetAmount(int amount)
        {
            curAmount = amount;
            barImage.fillAmount = GetPersent(curAmount);
            barDamageImage.fillAmount = GetPersent(0);
            barHealImage.fillAmount = GetPersent(0);
            targetAmount = amount;
            targetHealedAmount = amount;
            targetDamagedAmount = amount;
        }

        public void SetAnimatedAmount(int amount)
        {
            int oldAmount = curAmount;

            curAmount = amount;

            if (oldAmount - curAmount > 0)
            {
                OnDamage(amount);
            }

            if (oldAmount - curAmount < 0)
            {
                OnHeal(amount);
            }
        }

        void OnDamage(int curAmount)
        {
            targetAmount = curAmount;
            targetHealedAmount = curAmount;
            isAnimatingHeal = false;
            isAnimatingDamage = true;
            barHealImage.fillAmount = GetPersent(0);
        }

        void OnHeal(int curAmount)
        {
            targetHealedAmount = curAmount;
            targetDamagedAmount = curAmount;
            isAnimatingHeal = true;
            isAnimatingDamage = false;
            barDamageImage.fillAmount = GetPersent(0);
        }

        float GetPersent(int amount)
        {
            return (float)amount / maxAmount;
        }

        public void OnUpdate()
        {
            if (isAnimatingDamage)
            {
                if (targetDamagedAmount > curAmount)
                {
                    float speed = (-curAmount + targetDamagedAmount);
                    speed = Mathf.Clamp(speed, 10f, 30f);
                    barImage.fillAmount = GetPersent((int)targetAmount);
                    targetDamagedAmount -= Time.deltaTime * animatingSpeed * speed;
                    barDamageImage.fillAmount = GetPersent((int)targetDamagedAmount);
                }
                else if (targetDamagedAmount < curAmount)
                {
                    targetDamagedAmount = curAmount;
                    barImage.fillAmount = GetPersent((int)targetAmount);
                    barDamageImage.fillAmount = GetPersent((int)targetDamagedAmount);
                }
                else
                {
                    isAnimatingDamage = false;
                    barImage.fillAmount = GetPersent((int)targetAmount);
                    barDamageImage.fillAmount = GetPersent((int)targetDamagedAmount);
                }
            }

            if (isAnimatingHeal)
            {
                if (targetHealedAmount > targetAmount)
                {
                    float speed = (-targetAmount + targetHealedAmount);
                    speed = Mathf.Clamp(speed, 10, 20);
                    barHealImage.fillAmount = GetPersent((int)targetHealedAmount);

                    targetAmount += Time.deltaTime * animatingSpeed * speed;

                    barImage.fillAmount = GetPersent((int)targetAmount);
                }
                else if (targetHealedAmount < targetAmount)
                {
                    targetHealedAmount = curAmount;
                    barImage.fillAmount = GetPersent((int)targetAmount);
                    barHealImage.fillAmount = GetPersent((int)targetHealedAmount);
                }
                else
                {
                    isAnimatingHeal = false;
                    barImage.fillAmount = GetPersent((int)targetAmount);
                    barHealImage.fillAmount = GetPersent((int)targetHealedAmount);
                }
            }

            if (needBlinkWhenLowAmount && curAmount < 20)
            {
                barBg.color = Color.Lerp(origBgColor, blinkColor, Mathf.PingPong(Time.time * 1.5f, 1f));
            }
            else
            {
                if (barBg.color != origBgColor)
                {
                    barBg.color = origBgColor;
                }
            }
        }
    }

    public BarUI[] bars;


    HealthSystem healthSystem;

    public void Setup(HealthSystem hs)
    {
        healthSystem = hs;

        bars[0].Init(100);
        bars[1].Init(100);

        bars[0].SetAmount(healthSystem.GetHealth());
        bars[1].SetAmount(healthSystem.GetArmor());

        SetHealth();
        healthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
    }

    void HealthSystem_OnHealthChanged(object sender, System.EventArgs e)
    {
        SetHealth();
    }

    void SetHealth()
    {
        bars[0].SetAnimatedAmount(healthSystem.GetHealth());
        bars[1].SetAnimatedAmount(healthSystem.GetArmor());
    }

#if UNITY_EDITOR
    void Update()
    {
        if (Debug.isDebugBuild)
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                healthSystem.Damage(Random.Range(5, 20));
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                healthSystem.HealHealth(Random.Range(5, 20));
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                healthSystem.HealArmor(Random.Range(5, 20));
            }
        }

        for (int i = 0; i < bars.Length; i++)
        {
            bars[i].OnUpdate();
        }
    }

#endif
}
