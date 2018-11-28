using System;
using UnityEngine;
using UnityEngine.UI;

namespace AssemblyCSharp
{
    public class CharacterModel
    {
        public CharacterModel()
        {
            sliderScript = targetHealthBar.GetComponent<Slider>();
            apSliderScript = targetActionBar.GetComponent<Slider>();
            originalPDef = PDef;
            originalMDef = MDef;
            originalPAtk = PAtk;
            originalMAtk = MAtk;
            originalMDef = MDef;
            originalATKspd = ATKspd;
            originalactionPoints = actionPoints;
            maxHealth = Health;
            full_health = Health;
        }

        public Sprite characterIcon {get; set;}
        public bool isAlive {get; set;} = true;
        public float Health {get; set;}
        public float maxHealth {get; set;}
        public float blockPoints {get; set;}
        public float absorbPoints {get; set;}
        public float PDef {get; set;}
        public float originalPDef {get; set;}
        public float MDef {get; set;}
        public float originalMDef {get; set;}
        public float PAtk {get; set;}
        public float originalPAtk {get; set;}
        public float MAtk {get; set;}
        public float originalMAtk {get; set;}
        public float ATKspd {get; set;}
        public float originalATKspd {get; set;}
        public float critchance {get; set;}
        public float vigor {get; set;} = 1;
        public float thornsDmg {get; set;}
        public float originalthornsDmg {get; set;} = 0;
        public float originalvigor {get; set;} = 1;
        public float actionPoints {get; set;} = 6;
        public float originalactionPoints {get; set;}
        public float maxactionPoints {get; set;} = 6;
        public string characterType {get; set;}
        public string objectName {get; set;}
        public string role {get; set;}
        public character_data target {get; set;}
        public bool damageImmune {get; set;}
        public bool canAutoAttack {get; set;}
        public bool isAttacking {get; set;}
        public float incomingDmg {get; set;}
        public float incomingMDmg {get; set;}
        public float incomingHeal {get; set;}
        public float current_health {get; set;}
        public float full_health {get; set;}

        //health slider group
        public GameObject targetHealthBar {get; set;}
        public Slider sliderScript {get; set;}
        public GameObject targetActionBar {get; set;}
        public Slider apSliderScript {get; set;}

        public GameObject actionPointsDisplay {get; set;}
        public Text availableActionPoints {get; set;}
        public bool isMoving {get; set;}
        public Vector2 origPosition {get; set;}
        public Vector2 currentPosition {get; set;}
        public Quaternion currentRotation {get; set;}
        public GameObject posMarker {get; set;}
        public GameObject posMarkerMin {get; set;}
        public Vector2 attackedPos {get; set;}
        public GameObject currentPanel {get; set;}
        public int rowNumber {get; set;}
        public bool inVoidZone {get; set;}
        public bool inVoidCounter {get; set;}
        public bool inThreatZone {get; set;}
        private status statusScript {get; set;}
        //animationControl targetAnimControl {get; set;}
        public singleStatus deathStatus {get; set;}
    }
}

