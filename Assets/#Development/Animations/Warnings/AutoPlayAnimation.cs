using UnityEngine;
using System.Collections;

public class AutoPlayAnimation : MonoBehaviour {

	public Animator anim;

	// Use this for initialization
	void OnEnable(){
		anim.Play("gpsError");
	}
}
