using UnityEngine;
using System.Collections;

public class ChangeSprite : MonoBehaviour {

	public GameObject text_gr;
	public GameObject text_eng;

	
	void Update () {
		if(appSettings.language=="gr" && !text_gr.activeSelf){
			text_gr.SetActive(true);
			text_eng.SetActive(false);
		}else
		if(appSettings.language=="en" && !text_eng.activeSelf){
			text_gr.SetActive(false);
			text_eng.SetActive(true);
		}
	}
}
