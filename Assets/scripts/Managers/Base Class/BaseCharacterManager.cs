using UnityEngine;
using System.Linq;

namespace AssemblyCSharp
{
    public class BaseCharacterManager : MonoBehaviour 
    { 
        public bool initialSetupDone;
        public BaseCharacterManagerGroup baseManager;
        [Header("Character Model")]
        public BaseCharacterModel characterModel;
        [Header("Character Template")]
        public CharacterTemplate template;
        public bool useStats;
        public bool useResistance;
        [Header("Character Resistances")]
        public Resistances resistances;
        [Header("Health Object:")]
        public GameObject healthBar;

        public void UpdateBarSize(BaseCharacterModel characterModel)
        {
            characterModel.current_health = characterModel.Health;
            characterModel.sliderScript.maxValue = characterModel.fullHealth;
            characterModel.sliderScript.value = characterModel.current_health;
            characterModel.healthBarText.text = characterModel.current_health.ToString();
        }

        public void MaintainHealthValue(BaseCharacterModel characterModel)
        {
            if (characterModel.current_health > characterModel.maxHealth)
            {
                characterModel.Health = characterModel.maxHealth;
            }
            else if (characterModel.current_health <= 0 && characterModel.isAlive)
            {
                characterModel.isAlive = false;
                BattleManager.characterSelectManager.UpdateCharacters(this.gameObject.name);
                characterModel.Health = 0;
                var duration =  baseManager.animationManager.PlaySetAnimation("death");
                BattleManager.gameEffectManager.FadeOutSpine(baseManager.animationManager.skeletonAnimation);
                foreach (var task in BattleManager.taskManager.taskList)
                {
                    if (task.Key == baseManager.name)
                    {
                        BattleManager.taskManager.taskList.Remove(task.Key);
                    }
                }
                if (characterModel.characterType == CharacterModel.CharacterTypeEnum.enemy)
                {
                    Destroy(this.gameObject, duration);
                    var dataUI = GameObject.Find(this.gameObject.name + "_data");
                    Destroy(dataUI, duration);
                    BattleManager.AddToEXP((baseManager.characterManager.characterModel as EnemyCharacterModel).experience);

                    (baseManager.characterManager.characterModel as EnemyCharacterModel).loot.ForEach(l =>
                    {
                        if (GameManager.GetChanceByPercentage(l.dropChancePercentage))
                        {
                            BattleManager.AddToLoot(l);
                        }
                    });

                }
            }
        }

        public void ResetAbsorbPoints(BaseCharacterModel characterModel)
        {
            if (characterModel.absorbPoints <= 0 && baseManager.statusManager.DoesStatusExist("damageAbsorb"))
            {
                characterModel.absorbPoints = 0;
                baseManager.statusManager.ForceStatusOff(baseManager.statusManager.GetStatus("damageAbsorb"));
            }
            if (characterModel.blockPoints <= 0 && baseManager.statusManager.DoesStatusExist("block"))
            {
                characterModel.blockPoints = 0;
                baseManager.statusManager.ForceStatusOff(baseManager.statusManager.GetStatus("block"));
            }
        }

        public bool DoesAttributeExist(string attributeName, BaseCharacterModel characterModel)
        {
            if (!string.IsNullOrEmpty(attributeName))
            {
                return characterModel.GetType().GetField(attributeName) != null ? true : false;
            }
            else
            {
                GameManager.logger.Log(attributeName, "No attribute given");
                return false;
            }
        }

        public float GetAttributeValue(string attributeName, BaseCharacterModel characterModel)
        {
            if (DoesAttributeExist(attributeName, characterModel))
            {
                var attributeValue = (float)characterModel.GetType().GetField(attributeName).GetValue(characterModel);
                return attributeValue;
            }
            return 0;
        }

        public void SetAttribute(string attributeName, float value, BaseCharacterModel characterModel)
        {
            if (DoesAttributeExist(attributeName, characterModel))
            {
                characterModel.GetType().GetField(attributeName).SetValue(characterModel, value);
            }
        }

        public float GetResistanceValue(string resistance)
        {
            if (DoesResistanceExist(resistance))
            {
                var resistanceValue = (float)this.resistances.GetType().GetField(resistance).GetValue(this.resistances);
                return resistanceValue;
            }
            return 0;
        }

        public bool DoesResistanceExist(string resistance)
        {
            if (!string.IsNullOrEmpty(resistance))
            {
                return this.resistances.GetType().GetField(resistance) != null ? true : false;
            }
            else
            {
                GameManager.logger.Log(resistance, "No attribute given");
                return false;
            }
        }

        public bool GetChanceToBeHit(float accuracy, float evasion)
        {
            var accuracyRemaining = accuracy - evasion;
            var chanceToHit = UnityEngine.Random.Range(0.0f, 1.0f);
            return accuracyRemaining >= chanceToHit;
        }

        public void SetResistance(string resistance, float value, BaseCharacterModel characterModel)
        {
            if (DoesResistanceExist(resistance))
            {
                characterModel.GetType().GetField(resistance).SetValue(this.resistances, value);
            }
        }

        public T GetTarget<T>(bool isAutoAttack = false)
        {
            if (isAutoAttack && this.tag == "Enemy" && BattleManager.IsTankInThreatZone())
            {
                return BattleManager.characterSelectManager.guardianObject.GetComponent<T>();
            }
            var enemyCharacters = BattleManager.GetEnemyCharacterManagers();
            var friendlyCharacters = BattleManager.GetFriendlyCharacterManagers();
            var targetCount = this.tag == "Player" ? enemyCharacters.Count : friendlyCharacters.Count;
            var i = Random.Range(0, targetCount);
            return typeof(T) == typeof(EnemyCharacterManagerGroup) ? enemyCharacters[i].GetComponent<T>() : friendlyCharacters[i].GetComponent<T>();
        }
    }
}