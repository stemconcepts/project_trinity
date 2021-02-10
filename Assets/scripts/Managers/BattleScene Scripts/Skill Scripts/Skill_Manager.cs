using System.Collections.Generic;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Linq;
using static AssemblyCSharp.PhaseManager;

namespace AssemblyCSharp
{
    public class Skill_Manager : MonoBehaviour
    {
        public GenericSkillModel activeSkill;
        public Base_Character_Manager baseManager;
        //private float multiplier;
        public List<Character_Manager> finalTargets = new List<Character_Manager>();
        public bool isSkillactive;
        public Character_Manager currenttarget;
        public bool isCasting;
        public List<GameObject> summonList = new List<GameObject>();

        public void OnEventSkillComplete(Spine.TrackEntry state, Spine.Event e)
        {
            if (e.Data.Name == "endEvent")
            {
                foreach (var target in finalTargets)
                {
                    var targetDamageManager = target.baseManager.damageManager;
                    if (targetDamageManager.skillDmgModels.ContainsKey(gameObject.name))
                    {
                        targetDamageManager.skillDmgModels.Remove(gameObject.name);
                    }
                }
                //currenttarget = null;
            }
        }

        public bool CompleteSkillOnCurrentTurn()
        {
            if (Battle_Manager.turnCount >= activeSkill.turnToComplete)
            {
                return true;
            } else
            {
                return false;
            }
        }
    }
}

