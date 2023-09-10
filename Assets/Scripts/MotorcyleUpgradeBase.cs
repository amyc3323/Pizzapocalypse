using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class MotorcyleUpgradeBase : MonoBehaviour,IPointerDownHandler
{
    [SerializeField] private float valueMultiplier;
    [SerializeField] private float priceMultiplier;
    [SerializeField] private int initialPrice;
    private int currentUpgradeIndex;
    private int currentPrice;//Calculated at runtime
    [SerializeField] private int upgradeID;
    [SerializeField] private WorkshopUpgradeBase motorcycleUpgrade;
    private void Awake()
    {
        currentUpgradeIndex = GetIndex();
        currentPrice = CalculatePrice();
    }
    public void OnPointerDown(PointerEventData pointerEventData)
    {
        if (GlobalSceneManager.instance.UseMoney(currentPrice))
        {
            Upgrade();
        }
    }
    public virtual void Upgrade()
    {
        currentUpgradeIndex++;
        SaveIndex();
        //Save upgrade index
        //Upgrade
        motorcycleUpgrade.Upgrade();
        currentPrice = CalculatePrice();
        //Play VFX and update UI
    }
    public void SaveIndex()
    {
        PlayerPrefs.SetInt("WorkshopUpgrade" + upgradeID, currentUpgradeIndex);
    }

    public int GetIndex()
    {
        int id = PlayerPrefs.GetInt("WorkshopUpgrade" + upgradeID);
        return id;

    }
    public int CalculatePrice()
    {
        return (int)(initialPrice * Mathf.Pow(priceMultiplier, currentUpgradeIndex));
    }
}
