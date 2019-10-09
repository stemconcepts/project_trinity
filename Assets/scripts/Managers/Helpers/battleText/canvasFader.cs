using UnityEngine;
using System.Collections;

public class canvasFader : MonoBehaviour {

	public CanvasGroup cg;

	public void Restart(){
		StopCoroutine( canvasFade( 3f ) );
		StartCoroutine ( canvasFade( 3f ) );
	}

	// Use this for initialization
	void Start () {

		cg = GetComponent<CanvasGroup>();

		if ( cg ) {
			StartCoroutine ( canvasFade( 3f ) );
		}

	}

	IEnumerator canvasFade( float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		cg.alpha = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
