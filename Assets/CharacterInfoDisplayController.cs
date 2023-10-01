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
    public void SetHealthInfo(classType classType, int currentHealth, int? maxHealth = null)
    {
        switch (classType)
        {
            case classType.guardian:
                var currentGuardianMaxHealth = maxHealth ?? int.Parse(guardianText.text.Split('/')[1]);
                guardianText.text = $"{currentHealth}/{currentGuardianMaxHealth}";
                break;
            case classType.stalker:
                var currentStalkerMaxHealth = maxHealth ?? int.Parse(stalkerText.text.Split('/')[1]);
                stalkerText.text = $"{currentHealth}/{currentStalkerMaxHealth}";
                break;
            case classType.walker:
                var currentWalkerMaxHealth = maxHealth ?? int.Parse(walkerText.text.Split('/')[1]);
                walkerText.text = $"{currentHealth}/{currentWalkerMaxHealth}";
                break;
        }
    }

    public void LoadHealthInfo()
    {
        PlayerData playerdata = SavedDataManager.SavedDataManagerInstance.LoadPlayerData();
        SetHealthInfo(classType.guardian, playerdata.tankHealth, playerdata.tankMaxHealth);
        SetHealthInfo(classType.stalker, playerdata.dpsHealth, playerdata.dpsMaxHealth);
        SetHealthInfo(classType.walker, playerdata.healerHealth, playerdata.healerMaxHealth);
    }
}
