using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

namespace AssemblyCSharp
{
    public class CombatLogManager : MonoBehaviour
    {
        // Hex color codes for styling log descriptions
        private const string DAMAGE_COLOR = "#ff8a82";
        private const string HEALING_COLOR = "#a5ff82";
        private const string SOURCE_COLOR = "#9ecbff";
        private const string DEBUFF_COLOR = "#ff8a82";
        private const string BUFF_COLOR = "#a5ff82";

        public GameObject LogItemTemplate;
        public GameObject LogHolder;
        public GameObject TurnIndicatorTemplate;
        LogIndicatorController CurrentIndicator;

        // List to store instantiated LogItems
        private List<GameObject> LogItems = new List<GameObject>();

        /// <summary>
        /// Instantiates a LogItem GameObject into the specified indicator or creates a new turn indicator if necessary.
        /// Adds the LogItem to the LogItems list and ensures it is displayed at the top of the list.
        /// </summary>
        /// <param name="logContent">The content to display in the LogItem.</param>
        public void InitLogItem(string logContent)
        {
            void AddLogItemToIndicator(GameObject indicator)
            {
                var logItem = Instantiate(LogItemTemplate, indicator.transform);
                var textMesh = logItem.GetComponent<TextMeshProUGUI>();
                textMesh.text = logContent;
                logItem.transform.SetAsFirstSibling(); // Move the new log item to the top of the list
                LogItems.Add(logItem);
                Canvas.ForceUpdateCanvases();
            }

            var currentTurn = BattleManager.turn;
            if (CurrentIndicator != null && CurrentIndicator.Turn != currentTurn)
            {
                CurrentIndicator = null;
            }

            if (CurrentIndicator != null && LogItemTemplate != null)
            {
                AddLogItemToIndicator(CurrentIndicator.gameObject);

            } else if (LogItemTemplate != null && LogHolder != null && TurnIndicatorTemplate != null) {
                var indicator = Instantiate(TurnIndicatorTemplate, LogHolder.transform);
                CurrentIndicator = indicator.GetComponent<LogIndicatorController>();
                CurrentIndicator.SetTurn(currentTurn);
                indicator.transform.SetAsFirstSibling();
                AddLogItemToIndicator(indicator);
            }
        }

        /// <summary>
        /// Clears all instantiated LogItems and empties the LogItems list.
        /// </summary>
        public void ClearLogItems()
        {
            foreach (var logItem in LogItems)
            {
                if (logItem != null)
                {
                    Destroy(logItem);
                }
            }
            LogItems.Clear();
        }

        /// <summary>
        /// Generates a description of the damage result from the calculatedamage method.
        /// </summary>
        public void GetDamageDescription(BaseDamageModel damageModel, string extraInfo = "")
        {
            string source = damageModel.skillModel?.skillName ?? damageModel.enemySkillModel?.skillName ?? damageModel.skillSource;
            string element = damageModel.element != elementType.none ? damageModel.element.ToString() : "physical";
            string resisted = damageModel.resistedDmg > 0 ? damageModel.resistedDmg.ToString() : null;

            string description = $"{damageModel.baseManager.characterManager.characterModel.name} takes <color={DAMAGE_COLOR}>{damageModel.damageTaken} {element} damage</color>";

            if (!string.IsNullOrEmpty(source))
            {
                description += $" from <color={SOURCE_COLOR}>{source}</color>";
            }

            if (!string.IsNullOrEmpty(resisted))
            {
                description += $" resisted <color={SOURCE_COLOR}>{resisted}</color>";
            }

            if (!string.IsNullOrEmpty(extraInfo))
            {
                description += $" <color={SOURCE_COLOR}>{extraInfo}</color>";
            }

            InitLogItem(description);
        }

        /// <summary>
        /// Generates a description of the healing result.
        /// </summary>
        public void GetHealingDescription(BaseDamageModel damageModel)
        {
            string source = damageModel.skillModel?.skillName ?? damageModel.enemySkillModel?.skillName;

            string description = $"{damageModel.baseManager.characterManager.characterModel.name} heals for <color={HEALING_COLOR}>{damageModel.incomingHeal}</color>";

            if (!string.IsNullOrEmpty(source))
            {
                description += $" from <color={SOURCE_COLOR}>{source}</color>";
            }

            InitLogItem(description);
        }

        /// <summary>
        /// Generates a description when a user gains a new status.
        /// </summary>
        public void GetStatusGainDescription(StatusModel statusModel)
        {
            string statusName = statusModel.singleStatus.name;

            // Determine the color based on whether the status is a buff or debuff
            string statusColor = statusModel.singleStatus.buff ? BUFF_COLOR : DEBUFF_COLOR;
            int stacks = statusModel.stacks <= 0 ? 1 : statusModel.stacks;

            string description = $"{statusModel.baseManager.characterManager.characterModel.name} gains {stacks} stack/s of <color={statusColor}>{statusName}</color>";

            InitLogItem(description);
        }

        /// <summary>
        /// Generates a description when a status expires.
        /// </summary>
        public void GetStatusExpireDescription(StatusModel statusModel, int? stackAmount = null)
        {
            string statusName = statusModel.singleStatus.name;

            // Determine the color based on whether the status is a buff or debuff
            string statusColor = statusModel.singleStatus.buff ? BUFF_COLOR : DEBUFF_COLOR;
            int stacks = stackAmount != null ? (int)stackAmount : (statusModel.stacks <= 0 ? 1 : statusModel.stacks);

            string description = $"{statusModel.baseManager.characterManager.characterModel.name} loses {stacks} stack/s of <color={statusColor}>{statusName}</color>";

            InitLogItem(description);
        }

        /// <summary>
        /// Generates a description when a skill starts or finishes casting.
        /// </summary>
        public void GetSkillCastingDescription(GenericSkillModel skillModel, BaseCharacterManagerGroup caster)
        {
            string skillName = skillModel.skillName;
            string casterName = caster.characterManager.characterModel.name;
            string description;
            int turnsToComplete = Math.Abs(BattleManager.turnCount - skillModel.turnToComplete);

            if (BattleManager.turnCount < skillModel.turnToComplete)
            {
                description = $"{casterName} starts casting <color={SOURCE_COLOR}>{skillName}</color>. It will complete in {turnsToComplete} turn/s";
            }
            else
            {
                description = $"{casterName} casts <color={SOURCE_COLOR}>{skillName}</color>";
            }

            InitLogItem(description);
        }

        /// <summary>
        /// Generates a description when a skill misses a target.
        /// </summary>
        /// <param name="damageModel">The damage model containing details of the missed skill.</param>
        public void GetSkillMissedDescription(BaseDamageModel damageModel)
        {
            string skillName = damageModel.skillModel?.skillName ?? damageModel.enemySkillModel?.skillName;
            string sourceName = damageModel.dmgSource.characterModel.name;
            string targetName = damageModel.baseManager.characterManager.characterModel.name;

            string description = $"{sourceName}'s <color={SOURCE_COLOR}>{skillName ?? "auto attack"}</color> missed {targetName}";

            InitLogItem(description);
        }

        /// <summary>
        /// Generates a description when damage is absorbed.
        /// </summary>
        /// <param name="damageModel">The damage model containing details of the absorbed damage.</param>
        /// <param name="absorbedAmount">Additional information to include in the description.</param>
        public void GetAbsorbDescription(BaseDamageModel damageModel, string absorbedAmount = "")
        {
            string sourceName = damageModel.dmgSource.characterModel.name;
            string targetName = damageModel.baseManager.characterManager.characterModel.name;

            string description = $"{targetName} absorbs {damageModel.damageTaken} damage from {sourceName}";

            if (!string.IsNullOrEmpty(absorbedAmount))
            {
                description += $" <color={HEALING_COLOR}>{absorbedAmount}</color> absorbed";
            }

            InitLogItem(description);
        }
    }
}
