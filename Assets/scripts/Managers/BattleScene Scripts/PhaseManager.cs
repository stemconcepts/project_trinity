using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace AssemblyCSharp
{
    public class PhaseManager : MonoBehaviour
    {
        public EnemyCharacterManagerGroup baseManager;
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
                                    turnDuration = s.statusTurnDuration,
                                    baseManager = baseManager
                                };
                                sm.singleStatus.dispellable = false;
                                baseManager.statusManager.RunStatusFunction(sm);
                            }
                        }
                    }
                    currentEnemyPhase = o.enemyPhase;
                    ((EnemySkillManager)baseManager.skillManager).RefreshSkillList(GetPhaseDetail().phaseSkills);
                    runInitialPhase = true;
                }
            });
        }

        void SetPhaseAnimations(PhaseModel phaseModel)
        {
            baseManager.animationManager.skeletonAnimation.skeleton.SetSkin(phaseModel.skinChange);
            baseManager.animationManager.inAnimation = true;

            var animationDuration = baseManager.animationManager.PlaySetAnimation(phaseModel.phaseChangeAnimation, false);
            baseManager.animationManager.PlayAddAnimation(baseManager.animationManager.idleAnimation.ToString(), true);

            //baseManager.animationManager.skeletonAnimation.state.SetAnimation(0, phaseModel.phaseChangeAnimation, false);
            //var animationDuration = baseManager.animationManager.skeletonAnimation.state.SetAnimation(0, phaseModel.phaseChangeAnimation, false).Animation.Duration;
            //baseManager.animationManager.skeletonAnimation.state.AddAnimation(0, baseManager.animationManager.idleAnimation.ToString(), true, 0);
            baseManager.animationManager.SetBusyAnimation(animationDuration);
        }

        public PhaseDetail GetPhaseDetail()
        {
            var phase = phases.Where(o => o.enemyPhase == currentEnemyPhase).FirstOrDefault();
            if (phase != null)
            {
                if (baseManager)
                {
                    ((EnemySkillManager)baseManager.skillManager).summonList = phase.summonList;
                    var summonSkill = ((EnemySkillManager)baseManager.skillManager).enemySkillList.Where(o => o.summon).FirstOrDefault();
                    if (summonSkill != null)
                    {
                        //summonSkill.preRequisite.amount = phase.summonList.Count();
                        summonSkill.preRequisites.Where(o => o.preRequisiteType == PreRequisiteModel.preRequisiteTypeEnum.summonPanels).FirstOrDefault().amount = phase.summonList.Count();
                    }
                }
                return phase.phaseDetails.Where(o => o.powerLevel == (baseManager.characterManager.characterModel as EnemyCharacterModel).powerLevel).FirstOrDefault();
            }
            return null;
        }

        private void Awake()
        {
            baseManager = this.gameObject.GetComponent<EnemyCharacterManagerGroup>();
        }
        private void Update()
        {
            if (!BattleManager.disableActions && (BattleManager.turn == BattleManager.TurnEnum.EnemyTurn) && !baseManager.autoAttackManager.isAttacking 
                && !baseManager.skillManager.isSkillactive && baseManager.characterManager.characterModel.isAlive)
            {
                ChangeBossPhase();
            }
            
        }
    }
}