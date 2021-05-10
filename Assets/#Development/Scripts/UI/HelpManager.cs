using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class NavigationPanel: System.Object//Not a MonoBehaviour!
{
	public GameObject personUp;
	public GameObject personDown;
	public RectTransform infoLabelHelp;
	public GameObject joyLeftHelp;
	public GameObject joyRightHelp;
	public GameObject mapHelp;
	public GameObject returnHelp;
}

[Serializable]
public class MenuPanel: System.Object//Not a MonoBehaviour!
{
	public GameObject mapHelp;
	public GameObject settingsHelp;
	public GameObject greditsHelp;
	public GameObject shopHelp;
	public GameObject commentsHelp;
	public GameObject helpHelp;
}

public class HelpManager : MonoBehaviour {

	public NavigationPanel navigationPanel;
	public MenuPanel menuPanel;

	public Transform menuScenesHelp;
	public Transform menuPeriodHelp;

	List<GameObject> allObjects=new List<GameObject>();

	RectTransform followLabel;
	Vector3 posFollowScene=Vector3.zero;
	Vector3 posFollowPeriod=Vector3.zero;

	MenuUI menuUI;

	public bool dontCloseIfTap;

	public void Init () {

//		Diadrasis.Instance.menuUI.warningsUI.HideGpsWarnings();

		if(!menuUI)
		{
			menuUI=transform.parent.GetComponent<MenuUI>();
		}

		if(allObjects.Count==0)
		{
			allObjects.Add(navigationPanel.personUp);
			allObjects.Add(navigationPanel.personDown);
			allObjects.Add(navigationPanel.infoLabelHelp.gameObject);
			allObjects.Add(navigationPanel.joyLeftHelp);
			allObjects.Add(navigationPanel.joyRightHelp);
			allObjects.Add(navigationPanel.returnHelp);
			allObjects.Add(navigationPanel.mapHelp);

			allObjects.Add(menuPanel.mapHelp);
			allObjects.Add(menuPanel.settingsHelp);
			allObjects.Add(menuScenesHelp.gameObject);
			allObjects.Add(menuPeriodHelp.gameObject);

			allObjects.Add (menuPanel.commentsHelp);
			allObjects.Add (menuPanel.shopHelp);
			allObjects.Add (menuPanel.greditsHelp);
			allObjects.Add (menuPanel.helpHelp);

		}

		Gps.Instance.isHelpActive = true;

		CheckStatus();


	}

//	void OnDisable()
//	{
//		followLabel = null;
//		posFollowScene=Vector3.zero;
//		posFollowPeriod=Vector3.zero;
//	}

	private float xronos=-30f;

	void Update () {
		//auto close help main panel
		if(xronos>0)
		{
			//start counting
			xronos-=Time.deltaTime;

			//ask menuUI script if any label is enabled then make random selection and follow it
			if(followLabel)
			{
				if(!navigationPanel.infoLabelHelp.gameObject.activeSelf)
				{
					navigationPanel.infoLabelHelp.gameObject.SetActive(true);
				}
				navigationPanel.infoLabelHelp.localPosition = new Vector3(followLabel.localPosition.x , followLabel.localPosition.y - 50f,0f);
			}

			if(posFollowScene!=Vector3.zero && menuScenesHelp)
			{
				if(!menuScenesHelp.gameObject.activeSelf)
				{
					menuScenesHelp.gameObject.SetActive(true);
				}
//				menuScenesHelp.GetComponent<RectTransform>().position=Camera.main.WorldToScreenPoint(new Vector3(posFollowScene.x+1f, posFollowScene.y-1f, 0f));
			}

			if(posFollowPeriod!=Vector3.zero && menuPeriodHelp)
			{
				if(!menuPeriodHelp.gameObject.activeSelf)
				{
					menuPeriodHelp.gameObject.SetActive(true);
				}
				menuPeriodHelp.GetComponent<RectTransform>().localPosition = Vector3.zero;//Camera.main.WorldToScreenPoint(new Vector3(posFollowScene.x+1f, posFollowScene.y-1f, 0f));
			}


			//if time is 0 hide help		//Lean.LeanTouch.Fingers.Count>0
			if(xronos<=0 || (Input.GetMouseButtonDown(0) && Diadrasis.Instance.escapeUser==Diadrasis.EscapeUser.inHelp && !dontCloseIfTap) || Diadrasis.Instance.user==Diadrasis.User.inFullMap)
			{
				xronos=-30f;
				CloseHelps();
				menuUI.animControl.CloseRadialMenu();
//				gameObject.SetActive(false);

				#if UNITY_EDITOR
				Debug.LogWarning("HELP TAP!!!");
				#endif
			}

		}


	}

	public void CloseHelps()
	{
		dontCloseIfTap = false;
		//xronos=-30f;

		Diadrasis.Instance.escapeUser = Diadrasis.Instance.prevEscapeUser;

		//close all helps if any is still open
		if (allObjects.Count > 0) {
			foreach (GameObject gb in allObjects) {
				if (gb && gb.activeSelf) {
					gb.SetActive (false);
				}
			}
		}

		if (followLabel != null) {
			followLabel = null;
		}

		posFollowScene=Vector3.zero;
		posFollowPeriod=Vector3.zero;

		Gps.Instance.isHelpActive = false;

		menuUI.animControl.CloseRadialMenu ();
	}

	void CheckStatus()
	{
		//set show time duration
		xronos=15f;

		//close all helps if any is still open
		if (allObjects.Count > 0) {
			foreach (GameObject gb in allObjects) {
				if (gb && gb.activeSelf) {
					gb.SetActive (false);
				}
			}
		}

		if (followLabel != null) {
			followLabel = null;
		}

		posFollowScene=Vector3.zero;
		posFollowPeriod=Vector3.zero;

		Gps.Instance.isHelpActive = false;

		//check status to show proper help
		switch(Diadrasis.Instance.user)
		{
		case(Diadrasis.User.inMenu):

//				Diadrasis.Instance.menuUI.warningsUI.HideGpsWarnings();

				//show help
				menuPanel.settingsHelp.SetActive (true);

				menuPanel.greditsHelp.SetActive (true);

				menuPanel.helpHelp.SetActive (true);

				if (Diadrasis.Instance.appEntrances > 1) {
					menuPanel.commentsHelp.SetActive (true);
				}
				if (Diadrasis.Instance.isStart > 1 && Diadrasis.Instance.appEntrances == 1) {
					menuPanel.shopHelp.SetActive (true);
				} else if (Diadrasis.Instance.appEntrances > 1) {
					menuPanel.shopHelp.SetActive (true);
				}
				
				
				if(Diadrasis.Instance.menuStatus!=Diadrasis.MenuStatus.periodView)
				{
					posFollowScene=Diadrasis.Instance.xartisMenu.followPosSceneLabelHelp();
				}
				else
				if(Diadrasis.Instance.menuStatus==Diadrasis.MenuStatus.periodView)
				{
					posFollowPeriod=Diadrasis.Instance.xartisMenu.followPosSceneLabelHelp();
				}

				navigationPanel.returnHelp.SetActive(true);

				break;

			case(Diadrasis.User.isNavigating):
				//show help
				NavHelp();

				break;
			case(Diadrasis.User.onAir):
					//show help
				if (!Diadrasis.Instance.moveOnAir) {
					navigationPanel.personDown.SetActive (true);

					//set from menuUI the label to follow if any
					followLabel = menuUI.labelToFollowOnHelp ();

					menuPanel.mapHelp.SetActive (true);
					menuPanel.settingsHelp.SetActive (true);
				} else if (Diadrasis.Instance.moveOnAir) {
					AirMoveHelp ();
				}

				break;
		}
	
	}



	void NavHelp(){
		navigationPanel.personUp.SetActive(true);

		//set from menuUI the label to follow if any
		followLabel = menuUI.labelToFollowOnHelp();

		menuPanel.mapHelp.SetActive(true);
		menuPanel.settingsHelp.SetActive(true);
		menuPanel.helpHelp.SetActive (true);


		if(Diadrasis.Instance.navMode==Diadrasis.NavMode.offSite)
		{
			//joysticks help
			navigationPanel.joyLeftHelp.SetActive(true);
		}

		//if person is using 2 joysticks
		if(Diadrasis.Instance.sensorUsing==Diadrasis.SensorUsing.joysticks)
		{
			navigationPanel.joyRightHelp.SetActive(true);
		}

		navigationPanel.mapHelp.SetActive(true);
		navigationPanel.returnHelp.SetActive(true);
	}

	void AirMoveHelp(){
		navigationPanel.personDown.SetActive(true);

		//set from menuUI the label to follow if any
		followLabel = menuUI.labelToFollowOnHelp();

		menuPanel.mapHelp.SetActive(true);
		menuPanel.settingsHelp.SetActive(true);
		menuPanel.helpHelp.SetActive (true);


		if(Diadrasis.Instance.navMode==Diadrasis.NavMode.offSite)
		{
			//joysticks help
			navigationPanel.joyLeftHelp.SetActive(true);
		}

		//if person is using 2 joysticks
		if(Diadrasis.Instance.sensorUsing==Diadrasis.SensorUsing.joysticks)
		{
			navigationPanel.joyRightHelp.SetActive(true);
		}

		navigationPanel.mapHelp.SetActive(true);
		navigationPanel.returnHelp.SetActive(true);
	}
}
