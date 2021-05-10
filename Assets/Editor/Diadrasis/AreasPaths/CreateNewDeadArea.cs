using UnityEngine;
using System.Collections;

public class CreateNewDeadArea : MonoBehaviour {
	public static void MakeDeadAreaPoints(int points,float height,float radius,Color xromaStart,Color xromaPoint,Vector3 myPos){
		
		GameObject fatherPath = new GameObject();
		fatherPath.transform.position=Vector3.zero;
		fatherPath.name="Dead";
		
		GameObject start = (GameObject)Instantiate(Resources.Load("editorPaths/start"));
		start.name="start";
		start.transform.parent=fatherPath.transform;
		Vector3 pos = start.transform.localPosition;
		pos = myPos; //Random.insideUnitSphere * 15 + myPos;
		pos.y=height;
		start.transform.localPosition=pos;
		
		start.renderer.sharedMaterial = new Material(Shader.Find("Diffuse"));
		start.renderer.sharedMaterial.color = xromaStart;
		
		for(int i=0; i<points; i++){
			
			//			float d  = i*5f/points;
			//
			//			 angle = d * Mathf.PI * 2f;
			//
			//			float x = Mathf.Sin(angle) * 5f;
			//			float y = Mathf.Cos(angle) * 5f;
			
			float angle = i * Mathf.PI * 2 / points;
			//			var pos = Vector3 (Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
			
			Vector3 newPos = new Vector3 (Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius + myPos;
			
			
			//			myPos.x+=2f;
			//			myPos.z+=2f;
			GameObject point = (GameObject)Instantiate(Resources.Load("editorPaths/point"));
			point.name="point";
			point.transform.parent=fatherPath.transform;
			Vector3 pos2 = point.transform.localPosition;
			pos2 = newPos; //Random.insideUnitSphere * 15 + myPos;
			pos2.y=height;
			point.transform.localPosition=pos2;
			point.renderer.sharedMaterial = new Material(Shader.Find("Diffuse"));
			point.renderer.sharedMaterial.color = xromaPoint;
		}
		
		
		
		
	}
}
