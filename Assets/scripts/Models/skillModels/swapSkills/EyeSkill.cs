using AssemblyCSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.scripts.Models.skillModels.swapSkills
{
    [System.Serializable]
    public class EyeSkill : GenericSkillModel
    {
        public Sprite skillIcon;
        public bool equipped;
        public bool learned;
        public bool assigned;
        public int curroptionRequired = 0;
    }
}
