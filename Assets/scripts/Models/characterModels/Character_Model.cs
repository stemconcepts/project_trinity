using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace AssemblyCSharp
{
    [System.Serializable]
    public class Character_Model
    {
        public void SetUp()
        {
            attackedPos = new Vector2();
            maxHealth = Health;
            full_health = Health;
            isAlive = true;
            //vigor = 1;
            originalthornsDmg = 0;
            //originalvigor = 1;
            //actionPoints = 6;
            //maxactionPoints = 6;
            originalPDef = PDef;
            originalMDef = MDef;
            originalPAtk = PAtk;
            originalMAtk = MAtk;
            originalMDef = MDef;
            originalHaste = Haste;
            //originalactionPoints = actionPoints;
        }

        public Sprite characterIcon;
        public bool isAlive;
        public float Health;
        public float maxHealth;
        public float blockPoints;
        public float absorbPoints;
        public float PDef;
        public float originalPDef;
        public float MDef;
        public float originalMDef;
        public float PAtk;
        public float originalPAtk;
        public float MAtk;
        public float originalMAtk;
        public float Haste;
        public float originalHaste;
        public float critchance;
        public float vigor;
        public float thornsDmg;
        public float originalthornsDmg;
        public float originalvigor;
        //public float actionPoints;
        //public float originalactionPoints;
        //public float maxactionPoints;
        public CharacterTypeEnum characterType;
        public enum CharacterTypeEnum{
            Player,
            enemy
        };
        public RoleEnum role;
        public enum RoleEnum{
            none,
            tank,
            healer,
            dps,
            boss,
            minion
        };
        //public Base_Character_Manager target;
        public bool damageImmune;
        public bool canAutoAttack;
        public bool isBusy;
        public float incomingDmg;
        public float incomingMDmg;
        public float incomingHeal;
        public float current_health;
        public float full_health;
        public Text healthBarText;
        public Slider sliderScript;
        public GameObject targetActionBar;
        //public Slider apSliderScript;
        public bool isMoving;
        public Quaternion currentRotation;
        public Vector2 attackedPos;
        public int rowNumber;
        public bool inVoidZone;
        public bool inVoidCounter;
        public bool inThreatZone;
        public StatusModel deathStatus;
    }
}

