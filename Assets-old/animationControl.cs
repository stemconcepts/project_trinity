using UnityEngine;
using System.Collections;
using Spine.Unity;

public class animationControl : MonoBehaviour {
	public bool inAnimation;
    [SpineAnimation]
    SkeletonAnimation skeletonAnimation;   
    calculateDmg calculateDmg;

	// Use this for initialization
	void Start () {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        calculateDmg = gameObject.transform.parent.GetComponent<calculateDmg>();
	    skeletonAnimation.state.Event += calculateDmg.OnEventHit;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
