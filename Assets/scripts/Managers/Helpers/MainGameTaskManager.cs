using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AssemblyCSharp
{
    public class MainGameTaskManager : MonoBehaviour
    {
        public Dictionary<string, Task> taskList = new Dictionary<string, Task>();
        // Use this for initialization
        public void CallTask(float waitTime, System.Action action = null, string taskName = null)
        {
            var myTask = new Task(CountDown(waitTime, action));
            if (!string.IsNullOrEmpty(taskName) && !taskList.ContainsKey(taskName))
            {
                taskList.Add(taskName, myTask);
            }
        }
        IEnumerator CountDown(float waitTime, System.Action action = null)
        {
            yield return new WaitForSeconds(waitTime);
            if (action != null)
            {
                action();
            }
        }
    }
}