using System.Collections.Generic;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Linq;
using static AssemblyCSharp.PhaseManager;
using static AssemblyCSharp.GenericSkillModel;

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
            }
        }
    }
}

