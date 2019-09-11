using System;
using UnityEngine;
using UnityEngine.UI;

namespace AssemblyCSharp
{
    [System.Serializable]
    public class CharacterModel
    {
        public CharacterModel()
        {
            attackedPos = new Vector2();
            origPosition = new Vector2();
            currentPosition = new Vector2();
            originalPDef = PDef;
            originalMDef = MDef;
            originalPAtk = PAtk;
            originalMAtk = MAtk;
            originalMDef = MDef;
            originalATKspd = ATKspd;
            originalactionPoints = actionPoints;
            maxHealth = Health;
            full_health = Health;
            isAlive = true;
            vigor = 1;
            originalthornsDmg = 0;
            originalvigor = 1;
            actionPoints = 6;
            maxactionPoints = 6;
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
        public float ATKspd;
        public float originalATKspd;
        public float critchance;
        public float vigor;
        public float thornsDmg;
        public float originalthornsDmg;
        public float originalvigor;
        public float actionPoints;
        public float originalactionPoints;
        public float maxactionPoints;
        public string characterType;
        public string objectName;
        public string role;
        public Character_Manager target;
        public bool damageImmune;
        public bool canAutoAttack;
        public bool isAttacking;
        public bool isBusy;
        public float incomingDmg;
        public float incomingMDmg;
        public float incomingHeal;
        public float current_health;
        public float full_health;

        //health slider group
        public GameObject targetHealthBar;
        public Slider sliderScript;
        public GameObject targetActionBar;
        public Slider apSliderScript;

        public GameObject actionPointsDisplay;
        public Text availableActionPoints;
        public bool isMoving;
        public Vector2 origPosition;
        public Vector2 currentPosition;
        public Quaternion currentRotation;
        public GameObject posMarker;
        public GameObject posMarkerMin;
        public Vector2 attackedPos;
        public GameObject currentPanel;
        public int rowNumber;
        public bool inVoidZone;
        public bool inVoidCounter;
        public bool inThreatZone;
        //private status statusScript;
        //animationControl targetAnimControl;
        public StatusModel deathStatus;
    }
}

