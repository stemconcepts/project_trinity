using System;
using UnityEngine;
using UnityEngine.UI;

namespace AssemblyCSharp
{
    public class Battle_Details_Manager : BasicManager
    {
        public GameObject status_symbol { get; set; }
        public Transform status_position { get; set; }
        public GameObject buffParent { get; set; }
        public GameObject debuffParent { get; set; }
        public Status_Manager statusManager {get; set;}
        public Battle_Details_Manager()
        {
            statusManager = GetComponent<Status_Manager>();
        }

        public void ShowLabel( singleStatus status ){
            GameObject live_object = status.buff ? 
                (GameObject)Instantiate( status_symbol, new Vector3 ( status_position.position.x, 0.5f + status_position.position.y, status_position.position.z ) , status_position.rotation ) : 
                (GameObject)Instantiate( status_symbol, new Vector3 ( status_position.position.x, status_position.position.y, status_position.position.z  ) , status_position.rotation );
            Image iconImageScript = live_object.GetComponent<Image>();
            iconImageScript.sprite = status.labelIcon;
            if( buffParent && status.buff ){
                live_object.transform.SetParent( buffParent.transform, true );
            } else if( debuffParent && !status.buff  ){
                live_object.transform.SetParent( debuffParent.transform, true );
            }
            var getprefab = live_object.GetComponent<statussinglelabel>();
            getprefab.buff = status.buff;
            getprefab.singleStatus = status;
            getprefab.statusname = status.name;
            getprefab.dispellable = status.debuffable;
        }

        public void RemoveLabel( string statuslabel, bool isbuff ){
            if ( statusManager.DoesStatusExist(statuslabel) ){
                var chosenStatus = statusManager.GetStatusIfExist( statuslabel );
                chosenStatus.DestroyMe();
            }
        }
    }
}