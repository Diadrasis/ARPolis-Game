using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Test_CollisionTerrain : MonoBehaviour {

	public MeshFilter myMesh;
	public Terrain[] myTerrains;

	// Use this for initialization
	IEnumerator Start () {
		myTerrains = Terrain.activeTerrains;
		if(myTerrains.Length==0){
			Debug.Log("oops!!");
		}

		yield return new WaitForSeconds(1f);

		MeshCollisionCheck();
	}

	public bool useLowHeight;

	void MeshCollisionCheck () {
		Mesh mesh = myMesh.GetComponent<MeshFilter>().mesh;
		Vector3[] vertices = mesh.vertices;
		
		List<Vector3> simeia = new List<Vector3>();
		List<GameObject> simeiaTr = new List<GameObject>();
		
		if(vertices.Length>0){
			foreach(Vector3 p in vertices){
//				if(p.y==0f){
					if(!simeia.Contains(p)){
						simeia.Add(p);
					}
//				}
			}
		}

		float h = 1000f;

		if(useLowHeight){

			h = 1000f;
			
			if(simeia.Count>0){

				foreach(Vector3 p in simeia){
					if(p.y<h){
						h=p.y;
					}
				}

				Debug.Log("lowest height is "+h);
			}
		}else{
			h = -1000f;
			
			if(simeia.Count>0){
				
				foreach(Vector3 p in simeia){
					if(p.y>h){
						h=p.y;
					}
				}
				
				Debug.Log("lowest height is "+h);
			}

		}




		for(int i=0; i<simeia.Count; i++)
		{
//			float ss = myTerrains[0].terrainData.GetInterpolatedHeight(simeia[i].x, simeia[i].y);
			Debug.Log(simeia[i]);
			float ss = FindHeight(simeia[i]);

			Vector3 myPos = simeia[i];

			if(myPos.y == h)//if it has lowest height
			{
				GameObject gb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				gb.transform.localScale = Vector3.one/10f;
				gb.transform.parent = myMesh.transform;
				Vector3 newPos = simeia[i];
				newPos.y = ss;
				gb.transform.localPosition = newPos;
			}
		}
		
		Debug.Log("vertsices are "+vertices.Length);
		Debug.Log("final are "+simeia.Count);
	}


	float FindHeight(Vector3 pos){
		
		//declare 2 values
		float rayHeight=0f;
		float terrainHeight=0f;
		
		RaycastHit hit;
		
		//hit down from last y position of player
		Ray downRay = new Ray(pos , -Vector3.up);
		
		if (Physics.Raycast(downRay, out hit,Mathf.Infinity)){
			//get hit distance and add person height
			rayHeight = (pos.y - hit.distance);
		}

		#if UNITY_EDITOR
		Debug.Log("rayHeight = "+rayHeight);
		#endif
		
		//get terrain height
		if (myTerrains[0] != null) {
			terrainHeight = myTerrains[0].SampleHeight (new Vector3(pos.x, 0f, pos.z))  ;//+ Terrain.activeTerrain.transform.position.y;
			
			//find difference
//			float dif = Mathf.Abs(terrainHeight-rayHeight);

			#if UNITY_EDITOR
			Debug.Log("terrainHeight = "+terrainHeight);
			#endif

//			return terrainHeight;
		}
		
		return rayHeight;
	}	

}
