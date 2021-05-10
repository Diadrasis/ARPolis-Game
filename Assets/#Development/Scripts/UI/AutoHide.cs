using UnityEngine;
using System.Collections;

public class AutoHide : MonoBehaviour {



	IEnumerator Start () {
		if(PlayerPrefs.GetInt("off")==0){
			yield return new WaitForSeconds(0.7f);
		}

		gameObject.SetActive(false);
		PlayerPrefs.SetInt("off",1);
		PlayerPrefs.Save();
	}
	

}
