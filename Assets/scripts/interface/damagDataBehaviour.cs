using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class damagDataBehaviour : MonoBehaviour {
	private float currentPositionX;
	private float currentPositionY;
	public string skillLabel;
	public bool isDmg;
	public bool isAbsorb;
	public bool isImmune;
	public int healData;
	public int absorbData;
	public int damageData;
	public int fixedData;
	public GameObject textObject;
	private Text displayText;

	void Move(){
		currentPositionX = this.transform.position.x;
		currentPositionY = this.transform.position.y;
		var dmgPositionY = currentPositionY + 0.03f;
		var healPositionY = currentPositionY - 0.03f;
		if( isDmg ){
			this.transform.position = new Vector2( currentPositionX, dmgPositionY ) ;
		} else {
			this.transform.position = new Vector2( currentPositionX, healPositionY ) ;
		}
	}

	void CreateData() {
		displayText = textObject.GetComponent<Text>();
		//fixedData = damageData;
		if( isDmg ){
			//displayText.text = skillLabel + ": -" + damageData.ToString();
			displayText.text = damageData.ToString();
		} else 
		if ( isAbsorb ) {
			//displayText.text = skillLabel + ":" + absorbData.ToString();
			displayText.text = absorbData.ToString();
			displayText.color = Color.blue;
		} else 
		if ( isImmune ) {
				//displayText.text = skillLabel + ": Immune";
				displayText.text = "Immune";
				displayText.color = Color.white;
		} else {
			//displayText.text = "+" + healData.ToString();
			displayText.text = healData.ToString();
			displayText.color = Color.green;
		}
	}

	// Use this for initialization
	void Start () {
		CreateData();
				CanvasGroup cg = displayText.GetComponent<CanvasGroup>();
					if( cg ) {
						cg.alpha = 1;
						canvasFader cf = cg.gameObject.GetComponent<canvasFader>();
		
						if ( cf ){
							cf.Restart();
						} else {
							cg.gameObject.AddComponent<canvasFader>();
						}
					}
		//this.transform.localScale = new Vector3(0.07f,0.07f,0.07f);
		StartCoroutine( DestroyMe( 2f ) );
	}
	
	// Update is called once per frame
	void Update () {
		Move();
	}

	IEnumerator DestroyMe( float waitTime ){
		yield return new WaitForSeconds(waitTime);
		Destroy(gameObject);
	}
}
