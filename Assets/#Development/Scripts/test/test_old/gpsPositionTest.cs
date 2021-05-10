using UnityEngine;
using System.Collections;

public class gpsPositionTest : MonoBehaviour {
	public static Vector2 curGPS ;
	public static Vector2 gpsRefLoc; //pos 0,0
	private static Vector2 posPointA;
	private static Vector2 posPointB;
	private static Vector2 gpsPointA;
	private static Vector2 gpsPointB;
	private static float playerHeight;	// Use this for initialization
	private string message;
	GameObject cube;

	private static float corFactorX=1.0f;
	private static float corFactorY=1.0f;

	void Start () {
		gpsRefLoc = new Vector2 (38.480757f, 23.579611f);

		posPointA = new Vector2 (-292f, 195f);
		gpsPointA = new Vector2 (38.482597f, 23.576282f);
		posPointB = new Vector2 (231f, -99f);
		gpsPointB = new Vector2 (38.480757f, 23.579611f);
		cube = GameObject.Find ("testCube");

		playerHeight = 1f;
		curGPS = new Vector2 (38.480757f, 23.579611f);
		//curGPS = new Vector2 (35.344289f, 25.136560f);

		cube.transform.position = FindPosition (curGPS);
		message = cube.transform.position.ToString ();

	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnGUI(){
		GUI.Label (new Rect (10,10,150,50), new GUIContent(message));
	}

	public static Vector3 FindPosition(Vector2 gpsPos){
		float posX = 0;
		float posY = 0;
		float posZ = 0;
		
		posX = corFactorX * ((gpsPos.y - gpsRefLoc.y) * ((Mathf.Abs (posPointA.x) + Mathf.Abs (posPointB.x))) / (Mathf.Abs (gpsPointB.y - gpsPointA.y)));
		posZ = corFactorY * ((gpsPos.x - gpsRefLoc.x) * ((Mathf.Abs (posPointA.y) + Mathf.Abs (posPointB.y))) / (Mathf.Abs (gpsPointB.x - gpsPointA.x)));

		if (Terrain.activeTerrain != null){
			posY = Terrain.activeTerrain.SampleHeight (new Vector3 (posX, 0, posZ)) + playerHeight + Terrain.activeTerrain.transform.position.y;
		}else {
			RaycastHit hit;
			Ray downRay = new Ray(new Vector3(posX,100f,posZ), -Vector3.up);
			if (Physics.Raycast(downRay, out hit)){			
				posY = 100f-hit.distance + playerHeight;
			}

		}
		
		Vector3 pos=new Vector3(posX, posY, posZ);
		return pos;		
	}
}
