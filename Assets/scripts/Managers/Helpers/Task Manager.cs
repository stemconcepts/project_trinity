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

        public bool CallTask( float waitTime, System.Action action = null ){
            var myTask = new Task( CountDown( waitTime ) );
            if( action ){
                action();
            } else {
                return true;
            }
        } 

        public bool CallStatusTask( float waitTime, singleStatus singleStatus, System.Action action = null ){
            var myTask = new Task( CountDown( waitTime ) );
            if( action ){
                action();
            } else {
                return true;
            }
        } 

        IEnumerator CountDown(float waitTime )
        {
            yield return new WaitForSeconds(waitTime); 
        }
    }
}

