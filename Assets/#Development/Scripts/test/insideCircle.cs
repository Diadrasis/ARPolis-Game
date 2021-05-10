using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class insideCircle : MonoBehaviour {

	public Texture2D tex,tex2;
	public Image img;
	public RawImage rawImg;

	// Use this for initialization
	void Start () {
		tex = img.sprite.texture;

		tex2 = CircularCutoutTexture(img.sprite.texture);
		rawImg.texture = tex2;
	}
	
	public Vector3 circleCenter;
	
	public float radius;
	
	void LateUpdate (){
		// Do all other movement first
		// Constrain to a circle with the following
		Vector3 offset = transform.position - circleCenter;
		offset.Normalize();
		offset = offset * radius;
		transform.position = offset;
	}

	List<Color> pixels = new List<Color>();

	public Texture2D CircularCutoutTexture(Texture2D texture) {
		if (texture == null) return null;
		pixels = texture.GetPixels(0).ToList();

		Debug.LogWarning(pixels.Count);

		List<Color> ff = pixels.FindAll(x=> x.a==0);

		Debug.LogWarning("diafanes = "+ff.Count);
		
		// generate cutout
		Texture2D cutout = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);

		int size = (int)Mathf.Sqrt(pixels.Count);

		for (int y = 0; y < size; y++) {
			for (int x = 0; x < size; x++) {
				float dx = x - size/2;
				float dy = y - size/2;
				float d = Mathf.Sqrt(dx * dx + dy * dy) - size/2;
				int indx = x + y * size;
				if (d >= 0){
					Color xx = pixels[indx];
					xx.a =  Mathf.Max(1 - d, 0);
					pixels[indx] = xx;
				}
			}
		}
//		
		cutout.SetPixels(pixels.ToArray(), 0);
		cutout.Apply();
		return cutout;
	}

//	#region READ FILTER PIXELS
//	// Read pixel color at normalized hit point
//	Texture2D textureFilter = mapFilter.sprite.texture;
//	//		Color32[] resetColorArray = textureFilter.GetPixels();//.GetPixels32();
//	
//	filterArrayColors.Clear();
//	filterArrayVectors.Clear();
//	
//	//		colors2D = new Color[textureFilter.width, textureFilter.height];
//	
//	for (int x = 0; x < textureFilter.width; x++)
//	{
//		for (int y = 0; y < textureFilter.height; y++)
//		{
//			Color tick = textureFilter.GetPixel(x,y);
//			if(tick.a==0){
//				filterArrayColors.Add(tick);
//				filterArrayVectors.Add(new Vector2(x,y));
//			}
//		}
//	}
//	
//	#endregion
}
