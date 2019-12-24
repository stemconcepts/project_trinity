using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace AssemblyCSharp
{
    public class PhaseManager : MonoBehaviour
    {
        public Base_Character_Manager baseManager;
        public EnemyPhase currentEnemyPhase;
        private bool runInitialPhase;
        public enum EnemyPhase
        {
            phaseOne,
            phaseTwo,
            phaseThree
        }
        public List<PhaseModel> phases;

        private void ChangeBossPhase()
        {
            var healthPercentage = (baseManager.characterManager.characterModel.Health/baseManager.characterManager.characterModel.maxHealth) * 100;
            phases.ForEach(o =>
            {
                if((!runInitialPhase || currentEnemyPhase != o.enemyPhase) && healthPercentage > o.healthThreshhold.minHealthPercentage && healthPercentage <= o.healthThreshhold.maxHealthPercentage && !baseManager.skillManager.isSkillactive && !baseManager.animationManager.inAnimation )
                {
                    if (!string.IsNullOrEmpty(o.phaseChangeAnimation))
                    {
                        SetPhaseAnimations(o);
                    }
                    if (o.phaseBuffs.Count > 0)
                    {
                        for (int i = 0; i < o.phaseBuffs.Count; i++)
                        {
                            foreach (var s in o.phaseBuffs)
                            {
                                var sm = new StatusModel
                                {
                                    singleStatus = s.singleStatusModel,
                                    power = s.statusPower,
                                    duration = s.statusDuration,
                                    baseManager = baseManager
                                };
                                sm.singleStatus.dispellable = false;
                                baseManager.statusManager.RunStatusFunction(sm);
                            }
                        }
                    }
                    currentEnemyPhase = o.enemyPhase;
                    baseManager.skillManager.RefreshSkillList(GetPhaseSkills());
                    runInitialPhase = true;
                }
            });
        }

        void SetPhaseAnimations(PhaseModel phaseModel)
        {
            baseManager.animationManager.skeletonAnimation.skeleton.SetSkin(phaseModel.skinChange);
            baseManager.animationManager.inAnimation = true;
            baseManager.animationManager.skeletonAnimation.state.SetAnimation(0, phaseModel.phaseChangeAnimation, false);
            var animationDuration = baseManager.animationManager.skeletonAnimation.state.SetAnimation(0, phaseModel.phaseChangeAnimation, false).Animation.Duration;
            baseManager.animationManager.skeletonAnimation.state.AddAnimation(0, baseManager.animationManager.idleAnimation, true, 0);
            baseManager.animationManager.SetBusyAnimation(animationDuration);
        }

        public List<enemySkill> GetPhaseSkills()
        {
            var phase = phases.Where(o => o.enemyPhase == currentEnemyPhase).FirstOrDefault();
            if(phase != null)
            {
                return phase.phaseSkills;
            }
            return null;
        }

        private void Start()
        {
            baseManager = this.gameObject.GetComponent<Base_Character_Manager>();
        }
        private void Update()
        {
            ChangeBossPhase();
        }
    }
}