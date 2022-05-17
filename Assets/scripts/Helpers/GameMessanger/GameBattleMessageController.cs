using UnityEngine;
using System.Collections;
using TMPro;
using System;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    public class GameBattleMessageController : MonoBehaviour
    {
        public TextMeshProUGUI EXPText;
        public TextMeshProUGUI resourceText;
        public GameObject itemTemplate;

        public Transform itemsHolder;
        public List<ItemBase> items;

        public GameObject tankMainWeapon;
        public GameObject tankSecondWeapon;
        public GameObject tankBauble;

        public GameObject dpsMainWeapon;
        public GameObject dpsSecondWeapon;
        public GameObject dpsBauble;

        public GameObject healerMainWeapon;
        public GameObject healerSecondWeapon;
        public GameObject healerBauble;

        public Action closeAction;

        public void CloseMessage()
        {
            closeAction?.Invoke();
            Destroy(this.gameObject);
        }

        public void LoadGainedLoot()
        {
            items.ForEach(l =>
            {
                var i = Instantiate(itemTemplate, itemsHolder);
                var itemInfoController = i.GetComponent<ItemInfoController>();
                itemInfoController.itemBase = l;
            });
        }

        // Use this for initialization
        void Start()
        {
            //  EXPText.text = BattleManager.GetEXPValue().ToString();
            // resourceText.text = 0.ToString(); //BattleManager.GetEXPValue();
            EXPText.text = $"EXP gained : {BattleManager.GetEXPValue().ToString()}";
            items = BattleManager.GetLoot();
            LoadGainedLoot();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}