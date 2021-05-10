using UnityEngine;
using System.Collections;
using eChrono;

//for xml
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

#if UNITY_EDITOR
using System.Text.RegularExpressions;
#endif

public class CreatePoints : MonoBehaviour {
	
	public static string XmlPointsTagName; //read from selected menu item
	static cPoi myPoi;
	static XmlNode mySceneTagName;
	
	
	public static List<cLineSegment> segments=new List<cLineSegment>();
	public static List<cLineSegment> segmentsOnSite=new List<cLineSegment>();
	
	#if UNITY_EDITOR
	static bool printPoiNames; 
	static TextWriter sw;
	static string path ;
	static int poiID;
	#endif

	#region AWAKE
	
	void Awake () {	
		//Read general settings
		XmlNodeList generalSettings =Globals.Instance.settingsXml.SelectNodes ("/settings/general");
		
		if (generalSettings.Count > 0) 
		{
			
			foreach(XmlNode setting in generalSettings)
			{
				//speed parameters for differne modes
				if(PlayerPrefs.GetFloat("groundMoveSpeed")<=0){
					moveSettings.groundMoveSpeed = float.Parse (setting ["groundSpeed"].InnerText);
					//save
					PlayerPrefs.SetFloat("groundMoveSpeed",moveSettings.groundMoveSpeed);
				}else{
					moveSettings.groundMoveSpeed = PlayerPrefs.GetFloat("groundMoveSpeed");
				}
				
				//moveSettings.flyMoveSpeed = float.Parse (setting ["flySpeed"].InnerText);
				
				moveSettings.maxSnapOutOfAreaOnsite = float.Parse (setting ["maxSnapOutOfAreaOnsite"].InnerText);
				
				#if UNITY_EDITOR
				Debug.LogWarning("maxSnapOutOfAreaOnsite = "+moveSettings.maxSnapOutOfAreaOnsite);
				#endif
				

				if(Diadrasis.Instance.navMode==Diadrasis.NavMode.onSite){
					if(PlayerPrefs.GetFloat("maxSnapPathDistOnsite")<=0){
						moveSettings.maxSnapPathDistOnsite = float.Parse (setting ["maxSnapPathDist"].InnerText);
						//save
						PlayerPrefs.SetFloat("maxSnapPathDistOnsite",moveSettings.maxSnapPathDistOnsite);
					}else{
						moveSettings.maxSnapPathDistOnsite = PlayerPrefs.GetFloat("maxSnapPathDistOnsite");
					}
				}else //moveSettings.maxSnapPathDistOffsite
				if(Diadrasis.Instance.navMode==Diadrasis.NavMode.onSite){

					if(PlayerPrefs.GetFloat("maxSnapPathDistOffsite")<=0){
						moveSettings.maxSnapPathDistOffsite = float.Parse (setting ["maxSnapPathDistGround"].InnerText);
						//save
						PlayerPrefs.SetFloat("maxSnapPathDistOffsite",moveSettings.maxSnapPathDistOffsite);
					}else{
						moveSettings.maxSnapPathDistOffsite = PlayerPrefs.GetFloat("maxSnapPathDistOffsite");
					}
				}
				
				
				//				if(PlayerPrefs.GetFloat("flyHeight")<=0){
				//					moveSettings.flyHeight = float.Parse (setting ["flyHeight"].InnerText);
				//				}else{
				//					moveSettings.flyHeight = PlayerPrefs.GetFloat("flyHeight");
				//				}
				
				//moveSettings.rotSpeed = float.Parse (setting ["rotSpeed"].InnerText);
				
				//player height
				moveSettings.playerHeight = float.Parse (setting ["playerHeight"].InnerText);	
				
				//the distance to top moving
				//moveSettings.objDistance = float.Parse (setting ["objDistance"].InnerText);
				
				
				moveSettings.personHeight_IfOutOfArea = float.Parse (setting ["personHeight_IfOutOfArea"].InnerText);
				
				//camera settings
				if(PlayerPrefs.GetFloat("cameraFieldOfView")<=0){
					moveSettings.cameraFieldOfView = float.Parse (setting ["cameraFieldOfView"].InnerText);
					//save
					PlayerPrefs.SetFloat("cameraFieldOfView",moveSettings.cameraFieldOfView);
					
				}else{
					moveSettings.cameraFieldOfView = PlayerPrefs.GetFloat("cameraFieldOfView");
				}
				
				//Debug.LogWarning("cameraFieldOfView = "+moveSettings.cameraFieldOfView);
				
				
				if(PlayerPrefs.GetFloat("cameraHorAngleOffset")<=0){
					moveSettings.cameraHorAngleOffset = float.Parse (setting ["cameraHorAngleOffset"].InnerText);
					PlayerPrefs.SetFloat("cameraHorAngleOffset",moveSettings.cameraHorAngleOffset);
				}else{
					moveSettings.cameraHorAngleOffset = PlayerPrefs.GetFloat("cameraHorAngleOffset");
				}
				
				
				if(PlayerPrefs.GetFloat("cameraVertAngleOffset")<=0){
					moveSettings.cameraVertAngleOffset = float.Parse (setting ["cameraVertAngleOffset"].InnerText);
					PlayerPrefs.SetFloat("cameraVertAngleOffset",moveSettings.cameraVertAngleOffset);
				}else{
					moveSettings.cameraVertAngleOffset = PlayerPrefs.GetFloat("cameraVertAngleOffset");
				}
				
				if(PlayerPrefs.GetFloat("compassTurnSpeed")<=0){
					moveSettings.compassTurnSpeed = float.Parse (setting ["compassTurnSpeed"].InnerText);
					PlayerPrefs.SetFloat("compassTurnSpeed",moveSettings.compassTurnSpeed);
				}else{
					moveSettings.compassTurnSpeed = PlayerPrefs.GetFloat("compassTurnSpeed");
				}
				
				if(PlayerPrefs.GetFloat("camAccelRotX")<=0){
					moveSettings.camAccelRotX = float.Parse (setting ["camAccelRotX"].InnerText);
					PlayerPrefs.SetFloat("camAccelRotX",moveSettings.camAccelRotX);
				}else{
					moveSettings.camAccelRotX = PlayerPrefs.GetFloat("camAccelRotX");
				}
				
				if(PlayerPrefs.GetFloat("camAccelRotY")<=0){
					moveSettings.camAccelRotY = float.Parse (setting ["camAccelRotY"].InnerText);
					PlayerPrefs.SetFloat("camAccelRotY",moveSettings.camAccelRotY);
				}else{
					moveSettings.camAccelRotY = PlayerPrefs.GetFloat("camAccelRotY");
				}
				
				if(PlayerPrefs.GetFloat("accelSensitivity")<=0){
					moveSettings.accelSensitivity = float.Parse (setting ["accelSensitivity"].InnerText);
					PlayerPrefs.SetFloat("accelSensitivity",moveSettings.accelSensitivity);
				}else{
					moveSettings.accelSensitivity = PlayerPrefs.GetFloat("accelSensitivity");
				}
				
				
				if(PlayerPrefs.GetFloat("joyRightSensitivity")<=0){
					moveSettings.joyRightSensitivity = float.Parse (setting ["joyRightSensitivity"].InnerText);
					PlayerPrefs.SetFloat("joyRightSensitivity",moveSettings.joyRightSensitivity );
				}else{
					moveSettings.joyRightSensitivity = PlayerPrefs.GetFloat("joyRightSensitivity");
				}
				
				PlayerPrefs.Save();
				
//				#if UNITY_EDITOR
//				moveSettings.joyRightSensitivity = 13f;
//				#endif
			}
			
		}
		
	}

	#endregion
	
	#region SOUNDS
	
	/// <summary>
	/// Inits the sounds and create them into the scene.
	/// </summary>
	public static void InitSounds(){
		//scene sounds
		//		XmlNode sounds = Globals.Instance.dataXml.SelectSingleNode ("/chronomichani/" + XmlPointsTagName + "/sounds");
		XmlNodeList sounds = Globals.Instance.soundsXml.SelectNodes ("/sounds/" + XmlPointsTagName + "/sound");
		
		
		#if UNITY_EDITOR
		Debug.LogWarning(XmlPointsTagName+" have "+sounds.Count+" sounds");
		#endif
		
		if(sounds.Count > 0)
		{
			Diadrasis.Instance.allSounds.Clear();
			
			foreach(XmlNode sound in sounds)
			{
				if(!string.IsNullOrEmpty(sound ["file"].InnerText))
				{
					GameObject gb = new GameObject();
					
					Sound mySound = gb.AddComponent<Sound>();
					
					mySound.file = sound ["file"].InnerText;
					
					mySound.myClip = Resources.Load("audio/"+mySound.file)as AudioClip;
					
					if(mySound.myClip==null){
						//hide object
						gb.SetActive(false);
						
						#if UNITY_EDITOR
						Debug.LogWarning("SOUND "+mySound.file+" HAS WRONG Clip Name..hiding object");
						#endif
						
						//next sound please
						continue;
					}
					
					float pX = float.Parse(sound ["pos"] ["x"].InnerText);
					float pY= float.Parse(sound ["pos"] ["y"].InnerText);
					float pZ = float.Parse(sound ["pos"] ["z"].InnerText);
					
					mySound.pos = new Vector3(pX, pY, pZ);
					
					if(!string.IsNullOrEmpty(sound["volume"].InnerText))
					{
						float volume = float.Parse(sound["volume"].InnerText);
						
						if(volume>0f && volume<=1f){
							mySound.volume = volume;
							#if UNITY_EDITOR
							Debug.LogWarning("SOUND "+mySound.file+" HAS VOLUME "+mySound.volume);
							#endif
						}else{
							#if UNITY_EDITOR
							Debug.LogWarning("SOUND "+mySound.file+" HAS WRONG VOLUME NUMBER..falling back to 1f");
							#endif
							mySound.volume=0.1f;
						}
					}else{
						#if UNITY_EDITOR
						Debug.LogWarning("SOUND "+mySound.file+" VOLUME NUMBER is NULL OR TO SMALL..falling back to 1f");
						#endif
						mySound.volume = 1f;
					}
					
					if(!string.IsNullOrEmpty(sound["distanceToHear"]["min"].InnerText) && !string.IsNullOrEmpty(sound["distanceToHear"]["max"].InnerText))
					{
						float min = float.Parse(sound["distanceToHear"]["min"].InnerText);
						float max = float.Parse(sound["distanceToHear"]["max"].InnerText);
						
						if(min>=1f && max>1f){
							mySound.distanceToHear = new Vector2(min, max);
						}else{
							#if UNITY_EDITOR
							Debug.LogWarning("Distance "+mySound.file+" HAS WRONG distance NUMBERs..falling back to 1f - 500f");
							#endif
							mySound.distanceToHear = new Vector2(1f, 500f);
						}
					}else{
						#if UNITY_EDITOR
						Debug.LogWarning("Distance "+mySound.file+" HAS WRONG distance NUMBER..falling back to 1f - 500f");
						#endif
						mySound.distanceToHear = new Vector2(1f, 500f);
					}
					
					if(!string.IsNullOrEmpty(sound ["loop"].InnerText)){
						if(sound ["loop"] .InnerText == "true"){
							mySound.loop=true;
						}else{
							mySound.loop = false;
						}
					}else{
						mySound.loop = false;
					}
					
					if(!string.IsNullOrEmpty(sound["repeatTime"].InnerText))
					{
						mySound.repeatTime = float.Parse(sound["repeatTime"].InnerText);
					}else{
						mySound.repeatTime = 0;
					}
					
					if (!string.IsNullOrEmpty(sound["triggerDistance"].InnerText)){
						mySound.triggerDistance = float.Parse(sound["triggerDistance"].InnerText);
					}else{
						mySound.triggerDistance = 0f;
					}
					
					#if UNITY_EDITOR
					Debug.Log(mySound.file+" has triggerDistance = "+mySound.triggerDistance);
					#endif
					
					mySound.Init();
					
				}
			}
		}
		
		
	}
	
	#endregion

	#region PRINT POI NAMES FOR CURRENT PERIOD (STEP 1/4)

	#if UNITY_EDITOR
	private static string addName(string val){
		StringBuilder s = new StringBuilder();
		s.AppendLine(val);
		return s.ToString();
	}
	#endif

	#endregion
	
	public static void InitPoints() {	
		
		#if UNITY_EDITOR
		Debug.Log("InitPoints");
		#endif

		#region CLEAR ALL LISTS
		
		MenuUI.poiMnimeia.Clear();
		moveSettings.playerPath.Clear();
		moveSettings.pathOnSite.Clear();
		moveSettings.activeAreasOnSite.Clear();
		moveSettings.activeAreas.Clear();
		moveSettings.deadSpots.Clear();
		moveSettings.deadSpotsOnSite.Clear();
		
		//Read points of interest
		appData.myPoints = new Dictionary<string, cPoi>();

		GameManager.Instance.poiQuestions.Clear();

		#endregion

		if (XmlPointsTagName != null)
		{

			#region SCENE SETTINGS
			//get movement settings
			XmlNode settings = Globals.Instance.movementXml.SelectSingleNode ("/movement/" + XmlPointsTagName + "/settings");
			//get person first position into the scene
			moveSettings.startCamPosition=new Vector2(float.Parse(settings["transformation"]["position"]["x"].InnerText),float.Parse(settings["transformation"]["position"]["z"].InnerText));
			//get terrain size of current scene for map and movement calculations
			moveSettings.terrainSize = new Vector2(float.Parse(settings["terrainSize"]["x"].InnerText),float.Parse(settings["terrainSize"]["y"].InnerText));//new Vector2(610f,610f);

			moveSettings.personOnAirAltitude = float.Parse(settings["transformation"]["personOnAirAltitude"].InnerText);

			#endregion

			#region Points Of Interest

			#region FIND CURRENT PERIOD (SCENE) POIS FROM XML

			XmlNodeList poiKeys = Globals.Instance.scenesXml.SelectNodes ("/scenes/scene");

			//Debug.LogWarning("all periods are "+poiKeys.Count);

			if(poiKeys.Count>0){
				foreach(XmlNode keyPoi in poiKeys){
					if(keyPoi.Attributes["id"]!=null){

						//Debug.LogWarning("---> "+keyPoi.Attributes["id"].Value +" = " + XmlPointsTagName);

						if(keyPoi.Attributes["id"].Value == XmlPointsTagName){
							mySceneTagName = keyPoi;

							//Debug.LogWarning("XMLNODE FOUND!!");

							break;
						}
					}
				}
			}

			if(mySceneTagName==null){
				#if UNITY_EDITOR
				Debug.LogWarning("THIS PERIOD DOES NOT EXISTS!!!");
				#endif
				return;
			}
			XmlNodeList pointList = mySceneTagName.FirstChild.ChildNodes;

			#endregion

			#region PRINT POI NAMES FOR CURRENT PERIOD (STEP 2/4)

			#if UNITY_EDITOR
			printPoiNames=false;
			
			if(printPoiNames){
				poiID=0;
				path = "Poi Names of "+XmlPointsTagName+".txt";
				sw = new StreamWriter(path);
				sw.Flush();
			}
			#endif

			#endregion
			
			if(pointList.Count>0)
			{
				//				Debug.Log (pointList.Count);		
				foreach (XmlNode poi in pointList) {
					if (poi.Attributes ["status"] != null) { //check if exists
						if (poi.Attributes ["status"].Value == "1") {	 

							//create data to store new poi
							myPoi = new cPoi ();

							#region POI NAME

							XmlNode checkNode = poi ["name"];

							if(checkNode!=null){
								myPoi.name = poi["name"].InnerText.Trim();
							}
							#if UNITY_EDITOR
							else{Debug.LogWarning("XML ERROR!!! name");}
#endif

							#endregion

							#region POI Question

							checkNode = poi["quest"];

							if (checkNode != null)
							{
								GameManager.Instance.poiQuestions.Add(new PoiQuest { key = myPoi.name, questionEn = poi["quest"]["en"].InnerText, questionGR = poi["quest"]["gr"].InnerText });
							}
#if UNITY_EDITOR
							else { Debug.LogWarning("XML ERROR!!! name"); }
#endif

							#endregion

							#region BOOL SHOW INFO FOR CURRENT POI

							//getInfo
							checkNode = poi ["showInfo"];
							if(checkNode!=null){
								if(poi ["showInfo"].InnerText=="1"){
									myPoi.ShowtInfo = true;
								}else{
									myPoi.ShowtInfo = false;
									#if UNITY_EDITOR
									Debug.LogWarning(myPoi.name+" showInfo FALSE");
									#endif
								}
							}
							#if UNITY_EDITOR
							else{Debug.LogWarning(myPoi.name+" XML ERROR!!! showInfo");}
							#endif

							#endregion

							#region FIND POI TRANSFORMS AND SAVE THEM TO DISPLAY LABEL POSITION INTO SCENE
							
							//get mnimeia
							GameObject mnimeio = GameObject.Find(myPoi.name);
							if(mnimeio)
							{
								MenuUI.poiMnimeia.Add(mnimeio.transform);
								#if UNITY_EDITOR
								Debug.Log(mnimeio.name);
								#endif
							}

							#endregion

							#region TITLE

							checkNode = poi ["title"];
							if(checkNode!=null){
								myPoi.title = poi ["title"] [appSettings.language].InnerText;
							}
							#if UNITY_EDITOR
							else{Debug.LogWarning(myPoi.name+" XML ERROR!!! title");}
							#endif

							#endregion

							#region PRINT POI NAMES FOR CURRENT PERIOD (STEP 3/4)
							
							#if UNITY_EDITOR
							if(printPoiNames){
								poiID++;
								sw.Write(addName(poiID.ToString()+")"+myPoi.title+" = "+myPoi.name));
								sw.Flush();
							}
							#endif

							#endregion

							#region SHORT DESCRIPTIONS

							checkNode = poi ["shortDescriptions"];
							if(checkNode!=null){
								XmlNodeList nodeChilds = checkNode.ChildNodes;
								if(nodeChilds.Count>0){
									myPoi.shortDesc = checkNode.FirstChild [appSettings.language].InnerText;
								}else{
									myPoi.shortDesc = string.Empty;
									#if UNITY_EDITOR
									Debug.LogWarning(myPoi.name+" XML ERROR!!! NO shortDescriptions");
									#endif
								}
							}
							#if UNITY_EDITOR
							else{Debug.LogWarning(myPoi.name+" XML ERROR!!! NO shortDescriptions");}
							#endif

							#endregion

							#region DESCRIPTIONS
							
							checkNode = poi ["descriptions"];

							if(checkNode!=null){
								XmlNodeList nodeChilds = checkNode.ChildNodes;
								if(nodeChilds.Count>0){
									myPoi.desc = checkNode.FirstChild ["desc"][appSettings.language].InnerText;
								}else{
									myPoi.desc = string.Empty;
									#if UNITY_EDITOR
									Debug.LogWarning(myPoi.name+" XML ERROR!!! NO descriptions");
									#endif
								}
							}
							#if UNITY_EDITOR
							else{Debug.LogWarning(myPoi.name+" XML ERROR!!! NO descriptions");}
							#endif

							#endregion

							#region IMAGES

							checkNode = poi ["images"];

							if(checkNode!=null)
							{
								//create list to store images
								List<cImage> myImages = new List<cImage> ();

								XmlNodeList nodeChilds = checkNode.ChildNodes;

								if(nodeChilds.Count>0)
								{
									foreach (XmlNode image in nodeChilds)
									{
										//create data to store
										cImage newImage = new cImage ();

										//get file
										XmlNode imageFile = image ["file"];

										if(imageFile!=null){
											newImage.file = imageFile.InnerText;
										}else{
											#if UNITY_EDITOR
											Debug.LogWarning(myPoi.name+" XML ERROR!!! NO image file");
											#endif
											//skip 
											//dont save this photo
											continue;
										
										}

										//get caption
										XmlNode imageCaption = image ["caption"];
										
										if(imageCaption!=null){
											newImage.text = imageCaption [appSettings.language].InnerText;
										}else{
											newImage.text = string.Empty;
											#if UNITY_EDITOR
											Debug.LogWarning(myPoi.name+" "+newImage.file+" XML ERROR!!! NO image caption");
											#endif
										}

										//add image to list
										myImages.Add (newImage);
									}

									//save images for current poi
									if(myImages.Count>0){
										myPoi.images = myImages;
									}

								}
								#if UNITY_EDITOR
								else{Debug.LogWarning(myPoi.name+" XML ERROR!!! NO IMAGES");}
								#endif
							}
							#if UNITY_EDITOR
							else{Debug.LogWarning(myPoi.name+" XML ERROR!!! NO IMAGES");}
							#endif


							#endregion

							#region VIDEOS

							checkNode = poi ["videos"];

							if(checkNode!=null)
							{
								//create list to store
								List<cVideo> myVideos = new List<cVideo> ();

								XmlNodeList nodeChilds = checkNode.ChildNodes;

								if(nodeChilds.Count>0)
								{
									foreach (XmlNode video in nodeChilds)
									{
										//create data to store
										cVideo newVideo = new cVideo ();
										
										//get file
										XmlNode videoFile = video ["file"];
										
										if(videoFile!=null){
											newVideo.file = videoFile.InnerText;
										}else{
											#if UNITY_EDITOR
											Debug.LogWarning(myPoi.name+" XML ERROR!!! NO video file");
											#endif
											//skip 
											//dont save this video
											continue;
										}
										
										//get caption
										XmlNode videoLezanta = video ["lezanta"];
										
										if(videoLezanta!=null){
											newVideo.text = videoLezanta.InnerText;
										}else{
											newVideo.text = string.Empty;
											#if UNITY_EDITOR
											Debug.LogWarning(myPoi.name+" "+newVideo.file+" XML ERROR!!! NO video lezanta");
											#endif
										}
										
										//add image to list
										myVideos.Add (newVideo);
									}

									//save videos for current poi
									if(myVideos.Count>0){
										myPoi.videos = myVideos;
									}
								}
								#if UNITY_EDITOR
								else{Debug.LogWarning(myPoi.name+" XML ERROR!!! NO videos");}
								#endif
							}
							#if UNITY_EDITOR
							else{Debug.LogWarning(myPoi.name+" XML ERROR!!! NO videos");}
							#endif



							#endregion
									
							#region NARRATIONS
							
							checkNode = poi ["narrations"];

							if(checkNode!=null){
								//create draft list to store narrations
								List<cNarration> myNarrations = new List<cNarration> ();

								XmlNodeList nodeChilds = checkNode.ChildNodes;

								if(nodeChilds.Count>0)
								{
									foreach (XmlNode narration in nodeChilds)
									{
										//create data to store
										cNarration newNarration = new cNarration ();

										XmlNode narrFile = narration ["file"];

										if(narrFile!=null){
											newNarration.file = narrFile.InnerText;
										}else{
											#if UNITY_EDITOR
											Debug.LogWarning(myPoi.name+" XML ERROR!!! NO narration file");
											#endif
											//skip 
											//dont save this video
											continue;
										}

									}

									//save narrations for current poi
									if(myNarrations.Count>0){
										myPoi.narrations = myNarrations;
									}

								}
								#if UNITY_EDITOR
								else{Debug.LogWarning(myPoi.name+" XML ERROR!!! no narrations");}
								#endif
							}
							#if UNITY_EDITOR
							else{Debug.LogWarning(myPoi.name+" XML ERROR!!! no narrations");}
							#endif
							
							#endregion

							//add current poi to dictionary
							appData.myPoints.Add(myPoi.name,myPoi);
							
						}
					}
				}

				#region PRINT POI NAMES FOR CURRENT PERIOD (STEP 4/4)

				#if UNITY_EDITOR
				if(printPoiNames)
				{
					sw.Flush();
					Debug.Log ("mnimeia exported successfully.");
					sw.Close ();
				}
				#endif

				#endregion
				
			}
			#if UNITY_EDITOR
			else{Debug.LogWarning("NO Points for "+XmlPointsTagName);}
			#endif
			
			#endregion
			
			#region Areas OffSite
			
			//			moveSettings.activeAreas=new List<cArea>();
			
			XmlNodeList areaList = Globals.Instance.movementXml.SelectNodes ("/movement/" + XmlPointsTagName + "/ActiveAreas/activeArea");
			
			if(areaList.Count>0)
			{
				foreach (XmlNode area in areaList) {							
					if (area.Attributes ["status"] != null) {
						if (area.Attributes ["status"].Value == "on")
						{
							cArea myArea = new cArea();
							myArea.OnomaPerioxis=area["name"].InnerText;
							//myArea.center=new Vector2(float.Parse(area["center"]["x"].InnerText),float.Parse(area["center"]["z"].InnerText));
							//myArea.radius=float.Parse(area["radius"].InnerText);
							
							XmlNodeList psList = area["points"].ChildNodes;
							List<Vector3> ps = new List<Vector3> ();
							foreach (XmlNode p in psList) {
								if (p.Name == "point") {
									Vector3 v = new Vector3 (float.Parse (p.Attributes ["X"].InnerText), float.Parse (p.Attributes ["Y"].InnerText), float.Parse (p.Attributes ["Z"].InnerText));
									ps.Add (v);
								} 
							}
							myArea.Simeia=ps;
							
							//create perimetro of that area
							List<cLineSegment> segmentsA=new List<cLineSegment>();
							
							if(area.SelectSingleNode("perimetros")!=null)
							{
								
								XmlNodeList segmentListA = area["perimetros"].ChildNodes;
								
								if(segmentListA.Count>0)
								{
									foreach (XmlNode segmentNode in segmentListA){
										cLineSegment seg= new cLineSegment();
										float segsx=float.Parse(segmentNode["start"].Attributes["x"].Value);
										float segsy=float.Parse(segmentNode["start"].Attributes["y"].Value);
										seg.StartOfLine=new Vector2(segsx,segsy);
										float segfx=float.Parse(segmentNode["finish"].Attributes["x"].Value);
										float segfy=float.Parse (segmentNode["finish"].Attributes["y"].Value);
										seg.EndOfLine=new Vector2(segfx,segfy);
										//										if (segmentNode.Attributes ["limits"] != null) { //check if exists
										//											if (segmentNode.Attributes ["limits"].Value == "on") {
										//												seg.hasPathFreeMove=false;
										//											}else{
										//												seg.hasPathFreeMove=true;
										//											}
										//										}
										seg.hasPathFreeMove=true;
										//					Debug.Log(seg.limitsOn);
										segmentsA.Add(seg);
									}
									
									myArea.PerimetrosLines=segmentsA;
								}
							}
							
							moveSettings.activeAreas.Add(myArea);
							
						}
					}
				}
			}
			
			#endregion
			
			#region Areas OnSite
			
			//			moveSettings.activeAreasOnSite=new List<cArea>();
			
			
			
			areaList = Globals.Instance.movementXml.SelectNodes ("/movement/" + XmlPointsTagName + "/ActiveAreasOnSite/activeArea");
			
			if(areaList.Count>0)
			{
				foreach (XmlNode area in areaList) {							
					if (area.Attributes ["status"] != null) {
						if (area.Attributes ["status"].Value == "on")
						{
							cArea myArea = new cArea();
							
							XmlNodeList psList = area["points"].ChildNodes;
							List<Vector3> ps = new List<Vector3> ();
							foreach (XmlNode p in psList) {
								if (p.Name == "point") {
									Vector3 v = new Vector3 (float.Parse (p.Attributes ["X"].InnerText), float.Parse (p.Attributes ["Y"].InnerText), float.Parse (p.Attributes ["Z"].InnerText));
									ps.Add (v);
								} 
							}
							myArea.Simeia=ps;
							
							//create perimetro of that area
							List<cLineSegment> segmentsB = new List<cLineSegment>();
							
							if(area.SelectSingleNode("perimetros")!=null)
							{
								XmlNodeList segmentListB = area["perimetros"].ChildNodes;
								
								if(segmentListB.Count>0)
								{
									foreach (XmlNode segmentNode in segmentListB){
										cLineSegment seg= new cLineSegment();
										float segsx=float.Parse(segmentNode["start"].Attributes["x"].Value);
										float segsy=float.Parse(segmentNode["start"].Attributes["y"].Value);
										seg.StartOfLine=new Vector2(segsx,segsy);
										float segfx=float.Parse(segmentNode["finish"].Attributes["x"].Value);
										float segfy=float.Parse (segmentNode["finish"].Attributes["y"].Value);
										seg.EndOfLine=new Vector2(segfx,segfy);
										//										if (segmentNode.Attributes ["limits"] != null) { //check if exists
										//											if (segmentNode.Attributes ["limits"].Value == "on") {
										//												seg.hasPathFreeMove=false;
										//											}else{
										//												seg.hasPathFreeMove=true;
										//											}
										//										}
										seg.hasPathFreeMove=true;
										//					Debug.Log(seg.limitsOn);
										segmentsB.Add(seg);
									}
									
									myArea.PerimetrosLines=segmentsB;
								}
							}
							
							moveSettings.activeAreasOnSite.Add(myArea);
							
							
							
						}
					}
				}
			}
			
			#endregion
			
			#region Dead Areas OffSite
			
			//read the dead areas
			XmlNodeList deadList = Globals.Instance.movementXml.SelectNodes ("/movement/" + XmlPointsTagName + "/DeadSpots/deadArea");
			
			if(deadList.Count>0)
			{
				foreach (XmlNode dead in deadList) {							
					if (dead.Attributes ["status"] != null) {
						if (dead.Attributes ["status"].Value == "on")
						{
							cDeadSpot myDead = new cDeadSpot();
							
							XmlNodeList psList = dead["points"].ChildNodes;
							List<Vector3> ps = new List<Vector3> ();
							foreach (XmlNode p in psList) {
								if (p.Name == "point") {
									Vector3 v = new Vector3 (float.Parse (p.Attributes ["X"].InnerText), float.Parse (p.Attributes ["Y"].InnerText), float.Parse (p.Attributes ["Z"].InnerText));
									ps.Add (v);
								} 
							}
							myDead.points=ps;
							
							//create perimetro of that area
							List<cLineSegment> segmentsA=new List<cLineSegment>();
							
							if(dead.SelectSingleNode("perimetros")!=null)
							{
								
								XmlNodeList segmentListA = dead["perimetros"].ChildNodes;
								
								if(segmentListA.Count>0)
								{
									foreach (XmlNode segmentNode in segmentListA){
										cLineSegment seg= new cLineSegment();
										float segsx=float.Parse(segmentNode["start"].Attributes["x"].Value);
										float segsy=float.Parse(segmentNode["start"].Attributes["y"].Value);
										seg.StartOfLine=new Vector2(segsx,segsy);
										float segfx=float.Parse(segmentNode["finish"].Attributes["x"].Value);
										float segfy=float.Parse (segmentNode["finish"].Attributes["y"].Value);
										seg.EndOfLine=new Vector2(segfx,segfy);
										//										if (segmentNode.Attributes ["limits"] != null) { //check if exists
										//											if (segmentNode.Attributes ["limits"].Value == "on") {
										//												seg.hasPathFreeMove=false;
										//											}else{
										//												seg.hasPathFreeMove=true;
										//											}
										//										}
										seg.hasPathFreeMove=true;
										//					Debug.Log(seg.limitsOn);
										segmentsA.Add(seg);
									}
									
									myDead.DeadPerimetros=segmentsA;
								}
							}
							
							moveSettings.deadSpots.Add(myDead);
							
						}
					}
				}
			}
			
			#endregion
			
			#region Dead Areas OnSite
			
			deadList = Globals.Instance.movementXml.SelectNodes ("/movement/" + XmlPointsTagName + "/DeadSpotsOnSite/deadArea");
			
			if(deadList.Count>0)
			{
				foreach (XmlNode dead in deadList) {							
					if (dead.Attributes ["status"] != null) {
						if (dead.Attributes ["status"].Value == "on") {
							cDeadSpot myDead = new cDeadSpot();
							
							XmlNodeList psList = dead["points"].ChildNodes;
							List<Vector3> ps = new List<Vector3> ();
							foreach (XmlNode p in psList) {
								if (p.Name == "point") {
									Vector3 v = new Vector3 (float.Parse (p.Attributes ["X"].InnerText), float.Parse (p.Attributes ["Y"].InnerText), float.Parse (p.Attributes ["Z"].InnerText));
									ps.Add (v);
								} 
							}
							myDead.points=ps;
							
							//create perimetro of that area
							List<cLineSegment> segmentsA=new List<cLineSegment>();
							
							if(dead.SelectSingleNode("perimetros")!=null)
							{
								
								XmlNodeList segmentListA = dead["perimetros"].ChildNodes;
								
								if(segmentListA.Count>0)
								{
									foreach (XmlNode segmentNode in segmentListA){
										cLineSegment seg= new cLineSegment();
										float segsx=float.Parse(segmentNode["start"].Attributes["x"].Value);
										float segsy=float.Parse(segmentNode["start"].Attributes["y"].Value);
										seg.StartOfLine=new Vector2(segsx,segsy);
										float segfx=float.Parse(segmentNode["finish"].Attributes["x"].Value);
										float segfy=float.Parse (segmentNode["finish"].Attributes["y"].Value);
										seg.EndOfLine=new Vector2(segfx,segfy);
										//										if (segmentNode.Attributes ["limits"] != null) { //check if exists
										//											if (segmentNode.Attributes ["limits"].Value == "on") {
										//												seg.hasPathFreeMove=false;
										//											}else{
										//												seg.hasPathFreeMove=true;
										//											}
										//										}
										seg.hasPathFreeMove=true;
										//					Debug.Log(seg.limitsOn);
										segmentsA.Add(seg);
									}
									
									myDead.DeadPerimetros=segmentsA;
								}
							}
							
							moveSettings.deadSpotsOnSite.Add(myDead);
						}
					}
				}
			}
			
			#endregion
			
			#region Navigation Targets
			//read the targets of tour mode
			moveSettings.myTargets = new List<cTarget> ();						
			XmlNodeList targetList = Globals.Instance.movementXml.SelectNodes ("/movement/" + XmlPointsTagName + "/NavigationTargets/target");
			
			if(targetList.Count>0)
			{
				foreach (XmlNode target in targetList) {
					if (target.Attributes ["status"] != null) {
						if (target.Attributes ["status"].Value == "on") {
							cTarget myTarget = new cTarget ();
							if (Terrain.activeTerrain != null){
								float height= Terrain.activeTerrain.SampleHeight(
									new Vector3((float)Convert.ToDouble (target ["posX"].InnerText), 0, 
								            (float)Convert.ToDouble (target ["posZ"].InnerText)))+ moveSettings.playerHeight;
								
								Vector3 pos = new Vector3 ((float)Convert.ToDouble (target ["posX"].InnerText), height, 
								                           (float)Convert.ToDouble (target ["posZ"].InnerText));
								Vector3 rot = new Vector3 ((float)Convert.ToDouble (target ["rotX"].InnerText), (float)Convert.ToDouble (target ["rotY"].InnerText), (float)Convert.ToDouble (target ["rotZ"].InnerText));
								
								myTarget.name = target ["name"].InnerText;
								myTarget.position = pos;
								Quaternion quat=new Quaternion();
								quat.eulerAngles=rot;
								myTarget.rotation= quat;
								
							}
							
							moveSettings.myTargets.Add (myTarget);
						}
					}
				}
			}
			
			#endregion
			
			#region Paths OffSite
			
			XmlNodeList segmentList=Globals.Instance.movementXml.SelectNodes ("/movement/" + XmlPointsTagName + "/path/segment");
			
			if(segmentList.Count>0)
			{
				foreach (XmlNode segmentNode in segmentList){
					cLineSegment seg= new cLineSegment();
					float segsx=float.Parse(segmentNode["start"].Attributes["x"].Value);
					float segsy=float.Parse(segmentNode["start"].Attributes["y"].Value);
					seg.StartOfLine=new Vector2(segsx,segsy);
					float segfx=float.Parse(segmentNode["finish"].Attributes["x"].Value);
					float segfy=float.Parse (segmentNode["finish"].Attributes["y"].Value);
					seg.EndOfLine=new Vector2(segfx,segfy);
					//					if (segmentNode.Attributes ["limits"] != null) { //check if exists
					//						if (segmentNode.Attributes ["limits"].Value == "on") {
					//							seg.hasPathFreeMove=false;
					//						}else{
					//							seg.hasPathFreeMove=true;
					//						}
					//					}
					seg.hasPathFreeMove=true;
					//					Debug.Log(seg.limitsOn);
					segments.Add(seg);
				}
				
				moveSettings.playerPath=segments;
				
			}
			
			#endregion
			
			#region Paths OnSite
			
			segmentList=Globals.Instance.movementXml.SelectNodes ("/movement/" + XmlPointsTagName + "/pathOnSite/segment");
			
			foreach (XmlNode segmentNode in segmentList){
				cLineSegment seg= new cLineSegment();
				float segsx=float.Parse(segmentNode["start"].Attributes["x"].Value);
				float segsy=float.Parse(segmentNode["start"].Attributes["y"].Value);
				seg.StartOfLine=new Vector2(segsx,segsy);
				float segfx=float.Parse(segmentNode["finish"].Attributes["x"].Value);
				float segfy=float.Parse (segmentNode["finish"].Attributes["y"].Value);
				seg.EndOfLine=new Vector2(segfx,segfy);
				//				if (segmentNode.Attributes ["limits"] != null) { //check if exists
				//					if (segmentNode.Attributes ["limits"].Value == "on") {
				//						seg.hasPathFreeMove=false;
				//					}else{
				//						seg.hasPathFreeMove=true;
				//					}
				//				}
				
				seg.hasPathFreeMove=true;
				//					Debug.Log(seg.limitsOn);
				segmentsOnSite.Add(seg);
			}
			moveSettings.pathOnSite=segmentsOnSite;
			
			#endregion

		}
		#if UNITY_EDITOR
		else{Debug.LogWarning("XML ERROR!!! SCENE NAME IS NULL - EMPTY");}
		#endif
		
		#region AREA ON AIR
		//Auto create on Air move area
		
		moveSettings.onAirAreas.Clear();

		if(Diadrasis.Instance.navMode==Diadrasis.NavMode.offSite)
		{
		
			//get scene gps center from movement xml using poi name
			XmlNode myGpsCenter = Globals.Instance.movementXml.SelectSingleNode("movement/"+XmlPointsTagName+"/menu/GpsCenter");
			Vector2 mySceneCenter = Vector2.zero;
			if(myGpsCenter!=null)
			{
				float gpsCenterX = float.Parse(myGpsCenter["lat"].InnerText);
				float gpsCenterY = float.Parse(myGpsCenter["lon"].InnerText);
				
				mySceneCenter = new Vector2(gpsCenterX, gpsCenterY);
			}else{
				#if UNITY_EDITOR
				Debug.LogWarning("GpsCenter for "+XmlPointsTagName+" is Null");
				#endif
				
				mySceneCenter = Vector2.zero;
			}

			//get area on air from movement xml using poi name
			XmlNode xmlAreaOnAir = Globals.Instance.movementXml.SelectSingleNode("movement/"+XmlPointsTagName+"/areaOnAir");
			Vector2 myAreaRadius = Vector2.zero;
			if(xmlAreaOnAir!=null)
			{
				float radX = float.Parse(xmlAreaOnAir["x"].InnerText);
				float radY = float.Parse(xmlAreaOnAir["y"].InnerText);
				
				myAreaRadius = new Vector2(radX, radY);
			}else{
				#if UNITY_EDITOR
				Debug.LogWarning("AreaOnAir for "+XmlPointsTagName+" is Null");
				#endif
				
				myAreaRadius = new Vector2(100f, 100f);
			}
			
			cArea myAreaOnAir = new cArea();
			
			myAreaOnAir.CenterOfArea = gpsPosition.FindPosition(mySceneCenter);
			myAreaOnAir.CenterOfArea += moveSettings.posCenterOfMap;
			myAreaOnAir.Aktina = 100f;
			myAreaOnAir.Simeia = cirlceDots(63, myAreaOnAir.CenterOfArea, myAreaRadius);//new Vector2(myAreaOnAir.Aktina,myAreaOnAir.Aktina));
			myAreaOnAir.PerimetrosLines = cilrcleLines(myAreaOnAir.Simeia);
			
			moveSettings.onAirAreas.Add(myAreaOnAir);

		}
		
		#endregion
		
		#region AutoCheck DeadAreas & SAVE Perimetrous
		
		moveSettings.activeAreasPerimetroi.Clear();
		moveSettings.activeAreasOnSitesPerimetroi.Clear();
		moveSettings.deadSpotsPerimetroi.Clear();
		moveSettings.deadSpotsOnSitePerimetroi.Clear();
		moveSettings.activeAreasOnAirPerimetroi.Clear();
		
		if(moveSettings.onAirAreas.Count>0){
			for(int i=0; i<moveSettings.onAirAreas.Count; i++){
				if(moveSettings.onAirAreas[i].PerimetrosLines.Count>0){
					moveSettings.activeAreasOnAirPerimetroi.AddRange(moveSettings.onAirAreas[i].PerimetrosLines);
				}
			}
		}
		
		if(moveSettings.activeAreas.Count>0){
			for(int i=0; i<moveSettings.activeAreas.Count; i++){
				if(moveSettings.activeAreas[i].PerimetrosLines.Count>0){
					moveSettings.activeAreasPerimetroi.AddRange(moveSettings.activeAreas[i].PerimetrosLines);
				}
			}
		}
		if(moveSettings.activeAreasOnSite.Count>0){
			for(int i=0; i<moveSettings.activeAreasOnSite.Count; i++){
				if(moveSettings.activeAreasOnSite[i].PerimetrosLines.Count>0){
					moveSettings.activeAreasOnSitesPerimetroi.AddRange(moveSettings.activeAreasOnSite[i].PerimetrosLines);
				}
			}
		}
		

		
		if(moveSettings.deadSpots.Count>0){
			for(int i=0; i<moveSettings.deadSpots.Count; i++){
				if(moveSettings.deadSpots[i].DeadPerimetros.Count>0){
					moveSettings.deadSpotsPerimetroi.AddRange(moveSettings.deadSpots[i].DeadPerimetros);
				}
			}
		}
		if(moveSettings.deadSpotsOnSite.Count>0){
			for(int i=0; i<moveSettings.deadSpotsOnSite.Count; i++){
				if(moveSettings.deadSpotsOnSite[i].DeadPerimetros.Count>0){
					moveSettings.deadSpotsOnSitePerimetroi.AddRange(moveSettings.deadSpotsOnSite[i].DeadPerimetros);
				}
			}
		}
		
		/*
		GetDeadAreas(moveSettings.activeAreas, moveSettings.deadSpots, moveSettings.deadSpotsPerimetroi, moveSettings.activeAreasPerimetroi);
		ConnectAllPerimetrous(moveSettings.activeAreas, moveSettings.activeAreasPerimetroi);
		ConnectAllPerimetrous(moveSettings.deadSpots, moveSettings.deadSpotsPerimetroi);
		
		
		GetDeadAreas(moveSettings.activeAreasOnSite, moveSettings.deadSpotsOnSite, moveSettings.deadSpotsOnSitePerimetroi, moveSettings.activeAreasOnSitesPerimetroi);
		ConnectAllPerimetrous(moveSettings.activeAreasOnSite, moveSettings.activeAreasOnSitesPerimetroi);
		ConnectAllPerimetrous(moveSettings.deadSpotsOnSite, moveSettings.deadSpotsOnSitePerimetroi);
		*/
		#endregion

		#region EDITOR DEBUG TESTING
		
		#if UNITY_EDITOR
		Debug.Log("SCENE IS "+XmlPointsTagName);
		Debug.LogWarning("MNHMEIA IN SCENE = "+MenuUI.poiMnimeia.Count);
		Debug.LogWarning("MNHMEIA IN XML = "+appData.myPoints.Count);
		Debug.Log("Movement OFFsite");
		Debug.LogWarning("activeAreas are "+moveSettings.activeAreas.Count);
		Debug.LogWarning("dead areas offsite are "+moveSettings.deadSpots.Count);
		Debug.LogWarning("paths offsite are "+moveSettings.playerPath.Count);
		Debug.Log("Movement ONsite");
		Debug.LogWarning("onsite areas are "+moveSettings.activeAreasOnSite.Count);
		Debug.LogWarning("activeAreasOnSitesPerimetroi are "+moveSettings.activeAreasOnSitesPerimetroi.Count);
		Debug.LogWarning("dead areas onsite are "+moveSettings.deadSpotsOnSite.Count);
		Debug.LogWarning("deadSpotsOnSitePerimetroi are "+moveSettings.deadSpotsOnSitePerimetroi.Count);
		Debug.LogWarning("paths onsite are "+moveSettings.pathOnSite.Count);
		Debug.Log("Movement OnAIR");
		Debug.LogWarning("on air lines are "+moveSettings.activeAreasOnAirPerimetroi.Count);

		if(MenuUI.poiMnimeia.Count!=appData.myPoints.Count){
			Debug.LogWarning(XmlPointsTagName+" WARNING!!! XML - SCENE POIS ARE NOT EQUALS");
		}
		#endif
		
		#endregion
	}
	
	#region CREATE AREA FROM POINT
	static List<Vector3> cirlceDots(int numPoints, Vector2 centerPos, Vector2 radius){
		
		List<Vector3> dots = new List<Vector3>();
		
		for (int pointNum = 0; pointNum < numPoints; pointNum++)
		{
			// "i" now represents the progress around the circle from 0-1
			// we multiply by 1.0 to ensure we get a fraction as a result.
			float i = (pointNum * 1.0f) / numPoints;
			// get the angle for this step (in radians, not degrees)
			float angle = i * Mathf.PI * 2f;
			// the X and Y position for this angle are calculated using Sin and Cos
			float x = Mathf.Sin(angle) * radius.x;
			float y = Mathf.Cos(angle) * radius.y;
			Vector3 pos = new Vector3(x, 0f, y) + new Vector3(centerPos.x, 0f, centerPos.y);
			
			dots.Add(pos);
			
			#if UNITY_EDITOR
			//Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), pos, Quaternion.identity);
			#endif
		} 
		
		return dots;
		
	}
	
	static List<cLineSegment> cilrcleLines(List<Vector3> myDots){
		
		List<cLineSegment> lines = new List<cLineSegment>();
		
		for(int i=0; i<myDots.Count-1; i++){
			cLineSegment seg= new cLineSegment();
			
			float segsx = myDots[i].x;
			float segsy = myDots[i].z;
			
			seg.StartOfLine=new Vector2(segsx,segsy);
			
			float segfx = myDots[i+1].x;
			float segfy = myDots[i+1].z;
			
			seg.EndOfLine=new Vector2(segfx,segfy);
			
			seg.hasPathFreeMove=false;
			
			lines.Add(seg);
		}
		
		return lines;
	}
	
	#endregion
	
	#region Calculate Perimetrous
	
	static void ConnectAllPerimetrous(List<cArea> areaList, List<cLineSegment> perimetrosList){
		
		List<cLineSegment> crossLines = new List<cLineSegment>();
		List<cLineSegment> inAreaLines = new List<cLineSegment>();
		List<Vector3> crossPoints = new List<Vector3>();
		
		List<cLineSegment> synexomenes = new List<cLineSegment>();
		
		perimetrosList.Clear();
		
		if(areaList.Count>0){
			foreach(cArea ar in areaList){
				if(ar.PerimetrosLines!=null)
				{
					if(ar.PerimetrosLines.Count>0){
						perimetrosList.AddRange(ar.PerimetrosLines);
					}
				}
			}
		}else{
			#if UNITY_EDITOR
			Debug.LogWarning("No Areas");
			#endif
			return;
		}
		
		#if UNITY_EDITOR
		Debug.LogWarning("Remove from Lines Move Limits");
		#endif
		
		if(perimetrosList.Count>0){
			foreach(cLineSegment l in perimetrosList){
				l.hasPathFreeMove=true;
			}
		}
		
		#if UNITY_EDITOR
		Debug.LogWarning("lines are "+perimetrosList.Count);
		#endif
		
		if(perimetrosList.Count>0)
		{
			
			crossLines.Clear();
			inAreaLines.Clear();
			crossPoints.Clear();
			
			//			//find crossing lines and connect them at croosing point
			for(int i=0; i<perimetrosList.Count; i++)
			{
				//for every line
				for(int a=0; a<perimetrosList.Count; a++)
				{
					if(a!=i)
					{
						
						Vector3 crossPoint = Vector3.zero;
						
						//TODO check if cross point is not in list
						if(Stathis.AreaTools.isLineCrossing(out crossPoint, perimetrosList[i], perimetrosList[a], true, areaList)==1)
						{
							if(!crossPoints.Contains(crossPoint)){
								crossPoints.Add(crossPoint);
								
								
								if(!crossLines.Contains(perimetrosList[i]))
								{
									crossLines.Add(perimetrosList[i]);
								}
								
								if(!crossLines.Contains(perimetrosList[a]))
								{
									crossLines.Add(perimetrosList[a]);
								}
							}
						}
					}
				}
			}
			
			//Remove all lines that are entirly into an area
			if(perimetrosList.Count>0)
			{
				for(int x=0; x<perimetrosList.Count; x++)
				{
					//if(!crossLines.Contains(allPerimetroi[x]))
					//{
					foreach(cArea area in areaList)
					{
						if(!area.PerimetrosLines.Contains(perimetrosList[x]))
						{
							if(!inAreaLines.Contains(perimetrosList[x]))
							{
								if(Stathis.AreaTools.pointInsideArea(perimetrosList[x].StartOfLine, area) && Stathis.AreaTools.pointInsideArea(perimetrosList[x].EndOfLine, area))
								{
									inAreaLines.Add(perimetrosList[x]);
								}
							}
						}
					}
					//}
				}
			}
			
			#if UNITY_EDITOR
			Debug.Log("all crossing lines are "+crossLines.Count);
			Debug.Log("all in area lines are "+inAreaLines.Count);
			#endif
			
			
			if(inAreaLines.Count>0){
				foreach(cLineSegment l in inAreaLines){
					if(perimetrosList.Contains(l)){
						perimetrosList.Remove(l);
					}
				}
			}
			
			Debug.Log("final lines are "+perimetrosList.Count);
			
			synexomenes.Clear();
			
			for(int i=0; i<perimetrosList.Count; i++)
			{
				for(int a=0; a<perimetrosList.Count; a++)
				{
					if(a!=i){
						
						Vector3 crossPoint = Vector3.zero;
						//TODO check if cross point is not in list
						if(Stathis.AreaTools.isLineCrossing(out crossPoint, perimetrosList[i], perimetrosList[a], false, areaList)==0)
						{
							//Debug.Log("SYNEXOMENES");
							
							if(!synexomenes.Contains(perimetrosList[i])){
								synexomenes.Add(perimetrosList[i]);
							}
							
							if(!synexomenes.Contains(perimetrosList[a])){
								synexomenes.Add(perimetrosList[a]);
							}
							
						}else{
							//Debug.Log("error -> "+isLineCrossing(out crossPoint, allPerimetroi[i], allPerimetroi[a], false, moveSettings.activeAreas));
						}
					}
				}
			}
			
			Debug.Log("synexomenes are "+synexomenes.Count);
		}
	}
	
	static void ConnectAllPerimetrous(List<cDeadSpot> areaList, List<cLineSegment> perimetrosList){
		
		List<cLineSegment> crossLines = new List<cLineSegment>();
		List<cLineSegment> inAreaLines = new List<cLineSegment>();
		List<Vector3> crossPoints = new List<Vector3>();
		
		List<cLineSegment> synexomenes = new List<cLineSegment>();
		
		perimetrosList.Clear();
		
		if(areaList.Count>0){
			foreach(cDeadSpot ar in areaList){
				if(ar.DeadPerimetros.Count>0){
					perimetrosList.AddRange(ar.DeadPerimetros);
				}
			}
		}else{
			#if UNITY_EDITOR
			Debug.LogWarning("No Areas");
			#endif
			return;
		}
		
		#if UNITY_EDITOR
		Debug.LogWarning("Remove from Lines Move Limits");
		#endif
		
		if(perimetrosList.Count>0){
			foreach(cLineSegment l in perimetrosList){
				l.hasPathFreeMove=true;
			}
		}
		
		#if UNITY_EDITOR
		Debug.LogWarning("dead lines are "+perimetrosList.Count);
		#endif
		
		if(perimetrosList.Count>0){
			
			crossLines.Clear();
			inAreaLines.Clear();
			crossPoints.Clear();
			
			//			//find crossing lines and connect them at croosing point
			for(int i=0; i<perimetrosList.Count; i++)
			{
				//for every line
				for(int a=0; a<perimetrosList.Count; a++)
				{
					if(a!=i)
					{
						
						Vector3 crossPoint = Vector3.zero;
						
						//TODO check if cross point is not in list
						if(Stathis.AreaTools.isLineCrossing(out crossPoint, perimetrosList[i], perimetrosList[a], true, areaList)==1)
						{
							if(!crossPoints.Contains(crossPoint)){
								crossPoints.Add(crossPoint);
								
								
								if(!crossLines.Contains(perimetrosList[i]))
								{
									crossLines.Add(perimetrosList[i]);
								}
								
								if(!crossLines.Contains(perimetrosList[a]))
								{
									crossLines.Add(perimetrosList[a]);
								}
							}
						}
					}
				}
			}
			
			//Remove all lines that are entirly into an area
			if(perimetrosList.Count>0)
			{
				for(int x=0; x<perimetrosList.Count; x++)
				{
					//if(!crossLines.Contains(allPerimetroi[x]))
					//{
					foreach(cDeadSpot area in areaList)
					{
						if(!area.DeadPerimetros.Contains(perimetrosList[x]))
						{
							if(!inAreaLines.Contains(perimetrosList[x]))
							{
								if(Stathis.AreaTools.pointInsideDeadArea(perimetrosList[x].StartOfLine, area) && Stathis.AreaTools.pointInsideDeadArea(perimetrosList[x].EndOfLine, area))
								{
									inAreaLines.Add(perimetrosList[x]);
								}
							}
						}
					}
					//}
				}
			}
			
			#if UNITY_EDITOR
			Debug.Log("all crossing lines are "+crossLines.Count);
			Debug.Log("all in area lines are "+inAreaLines.Count);
			#endif
			
			
			if(inAreaLines.Count>0){
				foreach(cLineSegment l in inAreaLines){
					if(perimetrosList.Contains(l)){
						perimetrosList.Remove(l);
					}
				}
			}
			
			Debug.Log("final dead lines are "+perimetrosList.Count);
			
			synexomenes.Clear();
			
			for(int i=0; i<perimetrosList.Count; i++)
			{
				for(int a=0; a<perimetrosList.Count; a++)
				{
					if(a!=i){
						
						Vector3 crossPoint = Vector3.zero;
						//TODO check if cross point is not in list
						if(Stathis.AreaTools.isLineCrossing(out crossPoint, perimetrosList[i], perimetrosList[a], false, areaList)==0)
						{
							//Debug.Log("SYNEXOMENES");
							
							if(!synexomenes.Contains(perimetrosList[i])){
								synexomenes.Add(perimetrosList[i]);
							}
							
							if(!synexomenes.Contains(perimetrosList[a])){
								synexomenes.Add(perimetrosList[a]);
							}
							
						}else{
							//Debug.Log("error -> "+isLineCrossing(out crossPoint, allPerimetroi[i], allPerimetroi[a], false, moveSettings.activeAreas));
						}
					}
				}
				
				//				GameObject A = new GameObject();
				//				A.transform.position = new Vector3(perimetrosList[i].StartOfLine.x, 0f, perimetrosList[i].StartOfLine.y);
				//				A.name = "line_"+i+"_start";
				//				
				//				GameObject B = new GameObject();
				//				B.transform.position = new Vector3(perimetrosList[i].EndOfLine.x, 0f, perimetrosList[i].EndOfLine.y);
				//				B.name = "line_"+i+"_end";
			}
			
			Debug.Log("synexomenes are "+synexomenes.Count);
		}
	}
	
	#endregion
	
	#region GET DEAD AREAS
	/// <summary>
	/// Gets the dead areas.
	/// </summary>
	/// create dead areas if there are inside another area
	/// also creates the dead perimeter list
	static void GetDeadAreas(List<cArea> areaList, List<cDeadSpot> deadAreaList, List<cLineSegment> deadPerimetroi, List<cLineSegment> activePerimetroi){
		if(areaList.Count>0)
		{
			deadPerimetroi.Clear();
			deadAreaList.Clear();
			
			List<cArea> smallAreas = new List<cArea>();
			
			foreach(cArea area in areaList)
			{
				cArea biggerArea = new cArea();
				
				if(Stathis.AreaTools.areaInsideArea(out biggerArea, area, areaList))
				{
					if(!smallAreas.Contains(area))
					{
						smallAreas.Add(area);
						
						cDeadSpot dSpot = new cDeadSpot();
						
						dSpot.name = area.OnomaPerioxis;
						dSpot.points = area.Simeia;
						dSpot.center = area.CenterOfArea;
						dSpot.DeadPerimetros = biggerArea.DeadLines;
						
						//						Debug.Log(biggerArea.OnomaPerioxis+ " = " + biggerArea.DeadLines.Count);
						
						if(dSpot.DeadPerimetros.Count>0)
						{
							foreach(cLineSegment d in dSpot.DeadPerimetros)
							{
								d.hasPathFreeMove = true;
							}
						}
						
						deadAreaList.Add(dSpot);
						
						deadPerimetroi.AddRange(area.PerimetrosLines);
						
						//						#if UNITY_EDITOR
						//						Debug.LogWarning(dSpot.name+" is now dead area of "+biggerArea.OnomaPerioxis);
						//						#endif
					}
				}
			}
			
			//			#if UNITY_EDITOR
			//			Debug.LogWarning(smallAreas.Count+" VS "+areaList.Count);
			//			#endif
			
			//remove dead area from active areas list
			if(smallAreas.Count>0){
				for(int i=0; i<areaList.Count; i++)
				{
					//					Debug.LogWarning(i);
					foreach(cArea r in smallAreas)
					{
						if(r!=null)
							if(r.OnomaPerioxis == areaList[i].OnomaPerioxis)
						{
							areaList.Remove(areaList[i]);
						}
					}
				}
			}
			
			//remove dead perimetrous from active perimetrous
			if(deadPerimetroi.Count>0)
			{
				foreach(cLineSegment deadLine in deadPerimetroi)
				{
					if(activePerimetroi.Contains(deadLine))
					{
						activePerimetroi.Remove(deadLine);
					}
					
				}
			}
		}
	}
	
	#endregion
}
