using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TestRichText : MonoBehaviour {

	Text txt;

	public string keimeno;

	// Use this for initialization
	void Start () {
		txt = GetComponent<Text>();

		txt.text = keimeno;

		foreach(char d in txt.text){
			Debug.Log(d);
			System.Diagnostics.Debugger.Log(0,null,d+ System.Environment.NewLine);
		}
	}
	

}
