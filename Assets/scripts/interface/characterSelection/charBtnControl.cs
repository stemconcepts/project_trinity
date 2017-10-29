using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class charBtnControl : MonoBehaviour {
	public GameObject charObj;
	private Task holdTimeTask;
	private bool dragging = false;

	//selects character
	public void RunCharSelect(){
		charObj.GetComponent<button_clicks>().SelectChar();
	}
	//allows character selection
	void OnMouseUp(){
		charObj.GetComponent<button_clicks>().SelectChar();
		charObj.GetComponent<movement_script>().OnMouseUp();
	}
	//allows character drag
	public void OnMouseDown(){
		charObj.GetComponent<movement_script>().OnMouseDown();
	}

	void Start(){
		this.transform.localScale = new Vector3( 1f, 1f, 1f);
	}
}
