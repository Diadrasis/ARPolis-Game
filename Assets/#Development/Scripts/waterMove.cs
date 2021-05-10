using UnityEngine;
using System.Collections;

public class waterMove : MonoBehaviour {

	MeshRenderer meshRend;
	Material mat;
	float offset,offset2;

	void Start(){
		meshRend = GetComponent<MeshRenderer>();
		mat = meshRend.sharedMaterial;
	}

	void Update () {
		offset = Time.time * 0.1f;
		offset2 = Time.time * 0.05f;
		mat.mainTextureOffset = new Vector2(offset, offset2);
	}
}
