using UnityEngine;
using System.Collections;
using UnityEditor;
//using System.IO;

public class CreateNewPath : MonoBehaviour {



	//[MenuItem ("Diadrasis/Path/Create father Path")]

	public static void MakePoints(int points,float height,Color xromaStart,Color xromaPoint,Vector3 myPos,bool hasLimits){

		GameObject fatherPath = new GameObject();
		fatherPath.transform.position=Vector3.zero;
		if(hasLimits){
			fatherPath.name="Path";
		}else{
			fatherPath.name="Path_NoLimit";
		}

		GameObject start = (GameObject)Instantiate(Resources.Load("editorPaths/start"));
		start.name="start";
		start.transform.parent=fatherPath.transform;
		Vector3 pos = start.transform.localPosition;
		pos = myPos; //Random.insideUnitSphere * 15 + myPos;
		pos.y=height;
		start.transform.localPosition=pos;

		start.renderer.sharedMaterial = new Material(Shader.Find("Diffuse"));
		start.renderer.sharedMaterial.color = xromaStart;

		start.hideFlags=HideFlags.DontSave;

		for(int i=0; i<points; i++){
			myPos.x+=2f;
			myPos.z+=2f;
			GameObject point = (GameObject)Instantiate(Resources.Load("editorPaths/point"));
			point.name="point";
			point.transform.parent=fatherPath.transform;
			Vector3 pos2 = point.transform.localPosition;
			pos2 = myPos; //Random.insideUnitSphere * 15 + myPos;
			pos2.y=height;
			point.transform.localPosition=pos2;
			point.renderer.sharedMaterial = new Material(Shader.Find("Diffuse"));
			point.renderer.sharedMaterial.color = xromaPoint;

			point.hideFlags=HideFlags.DontSave;
			
		}
	}
}
