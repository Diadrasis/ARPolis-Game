using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AutoCloseMessage : MonoBehaviour {

	public int xronos = 0;
	public bool onTapHide;
	public GameObject closeBtn;
	public GameObject omixli;
	public GameObject objToSendMessage;
	public Button btnClose;

	void OnEnable () {
		if(xronos==0 && appSettings.xronosEmfanisisGpsInfo==0){
			xronos = 3;
		}

		if (omixli = null) {
			omixli = Diadrasis.Instance.menuUI.warningsUI.introScript.omixli;
		}


		if (omixli) {
			if (btnClose == null) {
				btnClose = omixli.transform.GetChild(0).GetComponent<Button> ();
			}

			if (btnClose) {
				if (closeBtn == null) {
					closeBtn = btnClose.gameObject;
				}

				closeBtn.SetActive (false);

				btnClose.onClick.AddListener (() => CloseMessage ());
			}
		}

		StartCoroutine(hideMe());

	}

	void Update(){
		if(onTapHide){
			if(Input.GetMouseButtonDown(0)){

				#if UNITY_EDITOR
				Debug.LogWarning(gameObject.name+" close from tap");
				#endif

				CloseMessage ();

			}
		}
	}

	public void CloseMessage(){
		StopAllCoroutines ();

		if (objToSendMessage) {
			objToSendMessage.SendMessage ("popupClose", SendMessageOptions.DontRequireReceiver);
		}

//		if(omixli){
//			omixli.SetActive (false);
//		}

		if (closeBtn) {
			closeBtn.SetActive (false);
		}

		objToSendMessage = null;
		onTapHide = false;

		gameObject.SetActive(false);
	}

	IEnumerator hideMe(){

		int time = 0;

		if(xronos>0){
			time = xronos;
		}else{
			time = appSettings.xronosEmfanisisGpsInfo;
		}

		while(time>=1 && gameObject.activeSelf){

			time--;

			yield return new WaitForSeconds(1f);

		}

		#if UNITY_EDITOR
		Debug.Log("close "+ gameObject.name +" warning");
		#endif

		CloseMessage ();

		yield break;
	}
	



}
