using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class globalEffectsController : MonoBehaviour {

    public static void callEffectTarget( GameObject target, GameObject fxObject, string position = "center"){
        GameObject fxposition = target.transform.Find("FXpositions").transform.Find("FXcenter").gameObject;
        Debug.Log( fxposition.transform.position );
        if( fxposition ) {
            if( position == "bottom" ){
                fxposition = target.transform.Find("FXpositions").transform.Find("FXfloor").gameObject;
            } else if ( position == "front" ){
                fxposition = target.transform.Find("FXpositions").transform.Find("FXfront").gameObject;
            }
            if( target.tag != "Player" ){
                var fx = Instantiate( fxObject, new Vector3 ( fxposition.transform.position.x , fxposition.transform.position.y, fxposition.transform.position.z ), new Quaternion ( 0, 180, 0, 0 ), fxposition.transform );
                //fx.transform.SetParent(position.transform);
            } else {
                var fx = Instantiate( fxObject, new Vector3 ( fxposition.transform.position.x , fxposition.transform.position.y, fxposition.transform.position.z ), fxposition.transform.rotation, fxposition.transform );
                //fx.transform.SetParent(position.transform);
            }   
        } else {
            var fx = Instantiate( fxObject, new Vector3 ( fxposition.transform.position.x , fxposition.transform.position.y, fxposition.transform.position.z ), fxposition.transform.rotation );
        }
    }
    
    /* public IEnumerator DelayedStart( GameObject newCreature ){
        yield return new WaitForEndOfFrame();
    }*/

}
