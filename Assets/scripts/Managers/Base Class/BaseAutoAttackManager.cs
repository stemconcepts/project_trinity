using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
    public class BaseAutoAttackManager : MonoBehaviour
    {
        public BaseCharacterManagerGroup baseManager;
        public float attackMovementSpeed = 50f;
        public int attackCoolDown = 1;
        public bool isAttacking = false;
        public bool hasAttacked = false;
        public int turnToReset = 0;
        public int turnToComplete = 0;
        public BaseCharacterManagerGroup autoAttackTarget;
        //public DamageModel dmgModel;

        public void OnEventAAComplete(Spine.TrackEntry state, Spine.Event e)
        {
            if (e.Data.Name == "endEvent")
            {
                var target = autoAttackTarget;
                if (target != null)
                {
                    var targetDamageManager = target.damageManager;
                    if (targetDamageManager.autoAttackDmgModels.ContainsKey(gameObject.name))
                    {
                        targetDamageManager.autoAttackDmgModels.Remove(gameObject.name);
                    }
                }
            }
        }

        public void SaveTurnToReset()
        {
            turnToReset = BattleManager.turnCount + attackCoolDown;
        }
    }
}