using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UVRectAnimation : MonoBehaviour {

	public RawImage mainImage;

	void Start () {
	
	}
	
	void Update () {
		Rect rt  = mainImage.uvRect;
		rt.x = Mathf.PingPong(Time.time * 0.05f,0.68f);//(0f, 1f, Time.time);
		mainImage.uvRect = rt;
	}
}
