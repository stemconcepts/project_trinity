using System;
using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
    public class Task_Manager : BasicManager
    {
        Task task;
        public Task_Manager()
        {
   
        }

        public bool CallTask( float waitTime, singleStatus singleStatus = null, System.Action action = null)
        {
            var myTask = new Task( CountDown( waitTime, singleStatus ) );
            if( action != null ){
                action();
            }
            return true;
        } 

        IEnumerator CountDown(float waitTime, singleStatus singleStatus )
        {
            yield return new WaitForSeconds(waitTime);
            battleDetailsManager.RemoveLabel(singleStatus.statusName, singleStatus.buff);
        }

        IEnumerator ChangePoints( singleStatus singleStatus, float power, string stat, bool regenOn ){
            var currentStat = characterScript.GetAttributeValue( stat );
            var maxStat = characterScript.GetAttributeValue( "max" + stat );
            while( currentStat <= maxStat && currentStat > 0 ){
                if( regenOn ){
                    characterScript.incomingHeal = power;
                    calculateDMGScript.calculateHdamage();
                } else { 
                    characterScript.incomingDmg = power;
                    calculateDMGScript.calculatedamage( singleStatus.statusName );
                }
                yield return new WaitForSeconds(5f);
            } 
        }
    }
}