using UnityEngine;
using System.Collections;

public class HideJoyOnsite : MonoBehaviour {

	public GameObject[] hideOnsiteObjects;

	void LateUpdate () {

		if(Diadrasis.Instance.user==Diadrasis.User.inHelp)
		{

			if(Diadrasis.Instance.navMode==Diadrasis.NavMode.onSite)
			{
				foreach(GameObject g in hideOnsiteObjects){
					if(g.activeSelf){
						g.SetActive(false);
					}
				}
			}
			else
			{
				foreach(GameObject g in hideOnsiteObjects){
					if(!g.activeSelf){
						g.SetActive(true);
					}
				}
			}

		}

	}
}
