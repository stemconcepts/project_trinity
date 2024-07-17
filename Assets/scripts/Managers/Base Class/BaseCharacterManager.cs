using UnityEngine;
using System.Collections.Generic;
using Assets.scripts.Models.statusModels;

namespace AssemblyCSharp
{
    public class BaseCharacterManager : MonoBehaviour 
    { 
        public bool initialSetupDone;
        public BaseCharacterManagerGroup baseManager;
        public List<AudioClip> idleSound;
        [Header("Character Model")]
        public BaseCharacterModel characterModel;
        [Header("Character Template")]
        public CharacterTemplate template;
        public bool useStats;
        public bool useResistance;
        [ConditionalHide("useResistance")]
        [Header("Character Resistances")]
        public Resistances resistances;
        [Header("Health Object:")]
        public GameObject healthBar;

        public void UpdateBarSize(BaseCharacterModel characterModel)
        {
            if (characterModel != null)
            {
                characterModel.current_health = characterModel.Health;
                characterModel.sliderScript.maxValue = characterModel.fullHealth;
                characterModel.sliderScript.value = characterModel.current_health;
                if (characterModel.healthBarText)
                {
                    characterModel.healthBarText.text = characterModel.current_health.ToString();
                }
            }
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
                //var trackEntry = baseManager.animationManager.PlaySetAnimation("toDeath");

                foreach (var task in BattleManager.taskManager.taskList)
                {
                    if (task.Key == baseManager.name)
                    {
                        BattleManager.taskManager.taskList.Remove(task.Key);
                    }
                }
                if (this is EnemyCharacterManager)
                {
                    var enemyCharacterManager = (EnemyCharacterManager)this;
                    if (this.gameObject)
                    {
                        Destroy(this.gameObject, 3f);
                        var dataUI = GameObject.Find(this.gameObject.name + "_data");
                        Destroy(dataUI, 3f);
                    }

                    //Remove enemy selector if it exists
                    if (enemyCharacterManager.enemyCharacterSelector)
                    {
                        Destroy(enemyCharacterManager.enemyCharacterSelector);
                    }

                    BattleManager.AddToEXP((baseManager.characterManager.characterModel as EnemyCharacterModel).experience);

                    (baseManager.characterManager.characterModel as EnemyCharacterModel).loot.ForEach(l =>
                    {
                        if (GameManager.GetChanceByPercentage(l.dropChancePercentage))
                        {
                            BattleManager.AddToLoot(l);
                        }
                    });

                    if (baseManager.animationManager.skeletonAnimationMulti != null)
                    {
                        baseManager.animationManager.skeletonAnimationMulti.GetSkeletonAnimations().ForEach(skeleton =>
                        {
                            BattleManager.gameEffectManager.FadeOutSpine(skeleton);
                        });
                    }
                    else
                    {
                        BattleManager.gameEffectManager.FadeOutSpine(baseManager.animationManager.skeletonAnimation);
                    }
                } else if (this is CharacterManager)
                {
                    var KOsingleStatus = baseManager.statusManager.singleStatusList.Find(s => s.statusName == StatusNameEnum.KnockedOut);
                    var KOstatusModel = new StatusModel
                    {
                        singleStatus = KOsingleStatus,
                        turnDuration = ((CharacterManager)this).RecoverTime,
                        baseManager = baseManager
                    };
                    baseManager.statusManager.StatusOn(KOstatusModel);

                    var characterManager = (CharacterManager)this;
                    BattleManager.GenericEventManager.AddDelegateToEvent(GenericEventEnum.PlayerTurn, characterManager.Ressurect);
                }
            }
        }

        public void ResetAbsorbPoints(BaseCharacterModel characterModel)
        {
            if (characterModel.absorbPoints <= 0 && baseManager.statusManager.DoesStatusExist(StatusNameEnum.DamageAbsorb))
            {
                characterModel.absorbPoints = 0;
                baseManager.statusManager.ForceStatusOff(baseManager.statusManager.GetStatus(StatusNameEnum.DamageAbsorb));
            }
            if (characterModel.blockPoints <= 0 && baseManager.statusManager.DoesStatusExist(StatusNameEnum.Block))
            {
                characterModel.blockPoints = 0;
                baseManager.statusManager.ForceStatusOff(baseManager.statusManager.GetStatus(StatusNameEnum.Block));
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

        public bool GetChanceToBeHit(float accuracy)
        {
            var accuracyRemaining = accuracy - this.characterModel.evasion; //evasion;
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

        void PlayIdleSound()
        {
            if (BattleManager.gameManager.GetChance(5))
            {
                if(idleSound.Count > 0)
                {
                    BattleManager.soundManager.playSound(idleSound);
                }
            }
        }
    }
}