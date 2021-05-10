using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpotPerson : MonoBehaviour {

	public Button btn;
	public RectTransform rt;
	public Vector2 personPos;
	public float rotY;
	public Transform target;

	public void SetPersonPos()
	{
		Diadrasis.Instance.ChangeStatus(Diadrasis.User.inAsanser);

		//MovePerson.cs for reseting rotation of kamera
		target.SendMessage("ResetKamera",SendMessageOptions.DontRequireReceiver);

		target.position = new Vector3(personPos.x,30f,personPos.y);
		target.localEulerAngles = new Vector3(0f,rotY,0f);
		Debug.Log("start ptosi!!!");

		RaycastHit hit;

		//get last y position of person to make a raycast
		Vector3 pos = target.position;
		
		//hit down from last y position of player
		Ray downRay = new Ray(pos, -Vector3.up);
		
		if (Physics.Raycast(downRay, out hit,Mathf.Infinity)){
			if(!hit.transform.name.Contains("Person")){
				//get hit distance and add person height
				float newPosY = (pos.y - hit.distance) + moveSettings.playerHeight;		Debug.Log("newPosY = "+newPosY);

				StartCoroutine(ptosi(newPosY));
			}
		}
	}

	IEnumerator ptosi(float Y)
	{
		SendMessageUpwards("MaxMap",SendMessageOptions.DontRequireReceiver);
		yield return new WaitForSeconds (1f);
		while(target.position.y>Y)
		{

			target.position = Vector3.Lerp(target.position,new Vector3(target.position.x,Y-0.1f,target.position.z),Time.deltaTime);
			yield return null;
		}

		Debug.Log("PERSON IS DOWN !!");
		Diadrasis.Instance.ChangeStatus(Diadrasis.User.isNavigating);
		yield break;
	}

	public void SetPos(Vector2 myPos){
		rt.localPosition=myPos;
	}
}
