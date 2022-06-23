using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.EventSystems;

namespace AssemblyCSharp
{
    public enum CharacterStats
    {
        None,
        PDef,
        MDef,
        PAtk,
        MAtk,
        Haste,
        vigor,
        thornsDmg,
        Health,
        blockPoints,
        absorbPoints,
        accuracy,
        evasion,
        critChance
    };

    public class BaseCharacterModel : MonoBehaviour
    {
        public bool isAlive;
        public float Health;
        [HideInInspector]
        public float maxHealth;
        public float fullHealth;
        public float blockPoints;
        public float absorbPoints;
        [HideInInspector]
        public float originalAccuracy;
        [Range(0, 1)]
        public float accuracy = 1;
        [HideInInspector]
        public float originalEvasion;
        [Range(0, 1)]
        public float evasion = 0.2f;
        [Range(0, 100)]
        public float critChance;
        [HideInInspector]
        public float originalCritChance;
        public float PDef;
        [HideInInspector]
        public float originalPDef;
        public float MDef;
        [HideInInspector]
        public float originalMDef;
        public float PAtk;
        public float piercing = 1;
        [HideInInspector]
        public float originalPAtk;
        public float MAtk;
        [HideInInspector]
        public float originalMAtk;
        public float Haste = 1;
        [HideInInspector]
        public float originalHaste;
        public float critchance;
        public float vigor;
        public float thornsDmg;
        [HideInInspector]
        public float originalthornsDmg;
        [HideInInspector]
        public float originalvigor;
        public CharacterTypeEnum characterType;
        public enum CharacterTypeEnum
        {
            Player,
            enemy
        };
        public RoleEnum role;
        public enum RoleEnum
        {
            none,
            tank,
            healer,
            dps,
            boss,
            minion
        };
        public bool damageImmune;
        public bool canAutoAttack;
        public bool isBusy;
        [HideInInspector]
        public float incomingDmg;
        [HideInInspector]
        public float incomingMDmg;
        [HideInInspector]
        public float incomingHeal;
        [HideInInspector]
        public float current_health;
        public Text healthBarText;
        public Slider sliderScript;
        public GameObject targetActionBar;
        public bool isMoving;
        [HideInInspector]
        public Quaternion currentRotation;
        [HideInInspector]
        public Vector2 attackedPos;
        public int rowNumber;
        public bool inVoidZone;
    }
}