using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour  {
    static public BattleModel battleModel;
    static Task battleStartTask;
    public static bool battleReady = false;

    static public void BattleStart( float startTimer ){
        battleStartTask = new Task( StartBattle( startTimer ) );
    }    

    static IEnumerator StartBattle(float waitTime )
    {
        yield return new WaitForSeconds(waitTime);
        battleReady = true;
        print("Ended");    
    }

	// Use this for initialization
	static void Start () {
		BattleStart( 5f );
	}
}
