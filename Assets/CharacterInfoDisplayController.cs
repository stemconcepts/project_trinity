using AssemblyCSharp;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterInfoDisplayController : MonoBehaviour
{
    [Header("Info Display")]
    public TextMeshProUGUI guardianText;
    public TextMeshProUGUI stalkerText;
    public TextMeshProUGUI walkerText;

    /// <summary>
    /// Set health for inventory screen
    /// </summary>
    /// <param name="classType"></param>
    /// <param name="currentHealth"></param>
    /// <param name="maxHealth"></param>
    public void SetHealthInfo(classType classType, int currentHealth, int maxHealth)
    {
        switch (classType)
        {
            case classType.guardian:
                guardianText.text = $"{currentHealth}/{maxHealth}";
                break;
            case classType.stalker:
                stalkerText.text = $"{currentHealth}/{maxHealth}";
                break;
            case classType.walker:
                walkerText.text = $"{currentHealth}/{maxHealth}";
                break;
        }
    }

    public void UpdateHealthInfo()
    {
        PlayerData playerdata = SavedDataManager.SavedDataManagerInstance.LoadPlayerData();
        SetHealthInfo(classType.guardian, playerdata.tankHealth, playerdata.tankMaxHealth);
        SetHealthInfo(classType.stalker, playerdata.dpsHealth, playerdata.dpsMaxHealth);
        SetHealthInfo(classType.walker, playerdata.healerHealth, playerdata.healerMaxHealth);
    }
}
