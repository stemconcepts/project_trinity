using System.Collections.Generic;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Linq;
using static AssemblyCSharp.PhaseManager;
using static AssemblyCSharp.GenericSkillModel;
using Spine;
using Unity.Burst.CompilerServices;
using System;

namespace AssemblyCSharp
{
    public class BaseSkillManager : MonoBehaviour
    {
        public bool hasCasted;
        public BaseCharacterManagerGroup baseManager;
        public List<BaseCharacterManager> finalTargets = new List<BaseCharacterManager>();
        public bool isSkillactive;
        public BaseCharacterManager currenttarget;
        public bool isCasting;
        public List<GameObject> summonList = new List<GameObject>();
        public GameObject defaultSwingEffect;
        public bool triggerEndEvent;

        public void AddStatuses(BaseCharacterManager target, GenericSkillModel skillModel, bool didHit)
        {
            if (skillModel.statusGroupFriendly.Count() > 0)
            {
                ProcessStatus(target, skillModel, skillModel.statusGroupFriendly);
            }
            if (skillModel.statusGroup.Count() > 0 && didHit)
            {
                ProcessStatus(target, skillModel, skillModel.statusGroup);
            }
        }

        private void ProcessStatus(BaseCharacterManager target, GenericSkillModel skill, List<StatusItem> statusItems)
        {
            var hitAnimation = statusItems.Where(statusItem => statusItem.status.hitAnim != animationOptionsEnum.none).Select(o => o.status.hitAnim).FirstOrDefault();
            var hitIdleAnimation = statusItems.Where(statusItem => statusItem.status.holdAnim != animationOptionsEnum.none).Select(o => o.status.holdAnim).FirstOrDefault();
            skill.AttachStatus(statusItems, target.baseManager, skill);
            target.baseManager.animationManager.SetHitIdleAnimation(hitAnimation, hitIdleAnimation);
            MainGameManager.instance.soundManager.PlayNegativeEffectSound();
        }

        public void MovecharacterOnComplete(TrackEntry spineEntry, GenericSkillModel activeSkill = null)
        {
            if (activeSkill != null && triggerEndEvent)
            {
                if (activeSkill.Reposition != GenericSkillModel.moveType.None && !isCasting)
                {
                    var charList = new List<BaseCharacterManager>() { baseManager.characterManager };
                    activeSkill.RepositionCharacters(charList);
                }
                triggerEndEvent = false;
            }
        }

        public void CleanUpDamageModelsEvent(TrackEntry spineEntry, GenericSkillModel activeSkill = null)
        {
            if (activeSkill != null)
            {
                foreach (var target in finalTargets)
                {
                    var targetDamageManager = target.baseManager.damageManager;
                    if (targetDamageManager.skillDmgModels.ContainsKey(gameObject.name))
                    {
                        targetDamageManager.skillDmgModels.Remove(gameObject.name);
                    }
                }
            }
        }

        public void ForcedMove(List<BaseCharacterManager> targets, GenericSkillModel skillModel)
        {
            targets.ForEach(characterManager =>
            {
                var movementScript = characterManager.baseManager.movementManager;
                if (movementScript.isInBackRow)
                {
                    characterManager.baseManager.damageManager.DoDamage(3, characterManager, skillModel);
                    characterManager.baseManager.statusManager.MakeStunned(1);
                }
                else
                {
                    movementScript.ForceMove(skillModel);
                }
            });
        }
    }
}

