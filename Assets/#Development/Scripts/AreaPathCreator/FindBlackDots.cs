using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FindBlackDots : MonoBehaviour {

	public Texture2D tex;
	public List<Color> color = new List<Color> ();
	// Use this for initialization
	IEnumerator Start () {
		Color[] pix = tex.GetPixels();
		Debug.Log (pix.Length);

		Debug.Log ("wait");
		yield return new WaitForSeconds(1f);
//		Debug.Log ("count black dots");
//		int black = 0;
//
//		if(pix.Length>0)
//		{
//			for (int i = 0; i < pix.Length; i++) {
//				if (pix [i].a>0.1f){// != Color.white) {
//					black++;
//	//				Debug.Log (black+" "+pix[i].grayscale);
//					color.Add (pix [i]);
//				}
//			}
//		}else{
//			Debug.LogWarning("no colors");
//		}
////
//		Debug.Log ("black dots are " + black);
//
//		Debug.Log ("wait");
//		yield return new WaitForSeconds(1f);
//		Debug.Log ("get black vectors");
//
		for (int x = 0; x < tex.width; x++) {
			for (int y = 0; y < tex.height; y++) {
				Color c = tex.GetPixel (x, y);
				if (c.a > 0.1f) {
					Debug.Log ("black dot is at "+x+" , "+y);
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
