using UnityEngine;
using System.Collections;
using CnControls;
using System.Collections.Generic;
using eChrono;
using System;

public class MovePerson : MonoBehaviour {

	#region PUBLIC VARIABLES

	public Transform myTransform;
	public Transform myCamera;
	public float RotationSpeed = 40f;
		
	public Vector2 prevGPS,curGPS,nextPos;

	public Gyroscope myGyro;	
	private static Quaternion quatMult;	
	private static Quaternion quatMap;

	public enum Spot{stopMove, inPath, inArea, inDeadArea, freeMove, outOfArea, onAir};
	public Spot spot = Spot.stopMove;

	public CharacterMotorC cmotor;
	public bool canJump;

//	float currDist = 1f;
	public float anoxiFromMonopati = 0.3f;
	public Vector3 lastAllowedPosition;

	#endregion

	#region PRIVATE VARIABLES
	Vector3 newPosPerson;
	Vector2 prevPosPerson, validPos;
	float speedIntoArea = 1f;
	float maxDistance=0f;
	float minX,maxX,minZ,maxZ;
	cSnapPosition snapPathPoint, snapAreaPoint, snapPoint;
	bool hasPaths, hasPerimetro, hasDeadPerimterous;
	float distanceFromArea=1000f;
	float distanceFromPath=1000f;
	float closestDistanceFromArea=1000f;
	float closestDistanceFromPath=1000f;

	#endregion

	private void OnEnable () {
		myTransform=Diadrasis.Instance.person.transform;
	}
	
	#region GPS MOVEMENT
	
	public void GpsMove(){
		if(Gps.Instance.isWorking())
		{
			//get gps data
			curGPS = Gps.Instance.GetMyLocation();	
			
			//declare gps position
			Vector2 pos2D=gpsPosition.FindPosition(curGPS);			

			pos2D+= moveSettings.posCenterOfMap;

			//move person to gps position
			moveOnsite(pos2D);

			if(!Diadrasis.Instance.menuUI.xartis.personFullVelaki.gameObject.activeSelf && Diadrasis.Instance.user==Diadrasis.User.inFullMap){
				if(!Diadrasis.Instance.menuUI.xartis.personFullVelaki.gameObject.activeSelf){
					Diadrasis.Instance.menuUI.xartis.personFullVelaki.gameObject.SetActive(true);
				}
			}

		}else{
			//message bad gps signal or off ??
		}
		
	}


	void moveOnsite(Vector2 myPos){
		
		#region MOVE IN PATH
		
		//αν είναι επιλεγμένη η υποχρωτική κίνηση σε μονοπάτι
		//και δεν είναι στον αέρα
		if(moveSettings.snapToPath)// && Diadrasis.Instance.user != Diadrasis.User.inFullMap)
		{
			if(WarningEventsUI.isEndOfMove){
				WarningEventsUI.isEndOfMove=false;
			}
			
			#region using graphic areas and pahs with map filter texture
			
			//GraphicsAndXml(isGps, myPos);
			//return;

			#endregion
			
			#region OUT OF AREA CHECK

			#if UNITY_EDITOR
			Debug.LogWarning("OUT OF AREA CHECK");
			#endif

			//αν είναι εκτός ενεργών περιοχών
			if (!isInArea(myPos,true))
			{
				distanceFromArea=1000f;
				distanceFromPath=1000f;

				#if UNITY_EDITOR
				Debug.LogWarning("OUT OF AREA CHECK");
				#endif

				if((Diadrasis.Instance.user==Diadrasis.User.isNavigating) || (Diadrasis.Instance.user==Diadrasis.User.onAir && Diadrasis.Instance.moveOnAir))
				{
					//αν υπάρχει perimetros
					if(moveSettings.activeAreasOnSitesPerimetroi.Count>0){
						//και βρίσκουμε το κοντινότρο σημείο πάνω stin perimetro
						snapAreaPoint=gpsPosition.FindSnapPosition(myPos,moveSettings.activeAreasOnSitesPerimetroi);
						distanceFromArea = Vector2.Distance(myPos, snapAreaPoint.position);
					}
					//αν υπάρχει ενεργό μονοπάτι επιτόπιας πλοήγησης απο το xml
					if(moveSettings.pathOnSite.Count>0){
						//και βρίσκουμε το κοντινότρο σημείο πάνω στο μονοπάτι
						snapPathPoint=gpsPosition.FindSnapPosition(myPos,moveSettings.pathOnSite);
						distanceFromPath = Vector2.Distance(myPos, snapPathPoint.position);
					}

					#if UNITY_EDITOR
					Debug.LogWarning("Area = "+distanceFromArea+" - Path = "+distanceFromPath);
					#endif

				}
//				else
//				if(Diadrasis.Instance.user==Diadrasis.User.onAir && Diadrasis.Instance.moveOnAir)
//				{
//					if(moveSettings.onAirAreas.Count>0){
//						//και βρίσκουμε το κοντινότρο σημείο πάνω stin perimetro
//						snapAreaPoint=gpsPosition.FindSnapPosition(myPos,moveSettings.activeAreasOnAirPerimetroi);
//						distanceFromArea = Vector2.Distance(myPos, snapAreaPoint.position);
//					}
//				}

				//stick to area
				if(distanceFromArea<distanceFromPath){
					snapPoint = snapAreaPoint;
					maxDistance = moveSettings.maxSnapOutOfAreaOnsite;
				}else
				//stick to path
				if(distanceFromArea>distanceFromPath){
					snapPoint = snapPathPoint;
					maxDistance = moveSettings.maxSnapPathDistOnsite;
				}

//				#if UNITY_EDITOR
//				Debug.LogWarning("snapPoint = "+snapPoint.position);
//				#endif
				
				//αν έχουμε snap point και είναι εντός μέγιστης αποδεκτής απόστασης
				if(snapPoint!=null && snapPoint.sqrDistance < maxDistance * maxDistance)
				{
					//θέτουμε ότι είναι σε μονοπάτι
					spot=Spot.inArea;
					//προαιρετικά κλείνουμε τον μεγιστοποιήμενο χάρτη 
					//αν είναι ενεργός απο προηγούμενη κίνηση
					minimizeMap();
					//θέτουμε τη νέα μας θέση πάνω στο μονοπάτι
					nextPos=snapPoint.position;
				}
				else//αν δεν έχουμε μονοπάτι / perimetro ή είναι εκτός μέγιστης αποδεκτής απόστασης
				{
					//θέτουμε ότι είναι εκτός περιοχής
					spot=Spot.outOfArea;

					#if UNITY_EDITOR
					Debug.Log("GPS POS =" + myPos);
					#endif

					//θέτουμε τη νέα μας θέση απο το gps
					nextPos=myPos;
					//μεγιστοποίηση χάρτη
					maximizeMap();
				}

				#if UNITY_EDITOR
				Debug.LogWarning("OUT OF AREA CHECK");
				#endif
			}

			#endregion out of area
			

			else
			
			#region IN AREA CHECK

				#if UNITY_EDITOR
				Debug.LogWarning("IN AREA CHECK");
				#endif

			//αν είναι εντός ενεργής περιοχής
			if (isInArea(myPos,true))
			{
				#if UNITY_EDITOR
				Debug.LogWarning("IN AREA CHECK");
				#endif

				if((Diadrasis.Instance.user==Diadrasis.User.isNavigating) || (Diadrasis.Instance.user==Diadrasis.User.onAir && Diadrasis.Instance.moveOnAir))
				{
					#if UNITY_EDITOR
					Debug.LogWarning("IN AREA CHECK");
					#endif

					//if deadspots exists in xml
					if (moveSettings.deadSpotsOnSite.Count>0)
					{
						//αν είναι εκτός νεκρής περιοχής επιτόπιας απο xml
						//και άρα εντός ενεργής
						if(!gpsPosition.PlayerInsideDeadArea(myPos,moveSettings.deadSpotsOnSite))
						{
							//θέτουμε οτι είναι εντος ενεργής περιοχής
							spot=Spot.inArea;
							//προαιρετικά κλείνουμε τον μεγιστοποιήμενο χάρτη 
							//αν είναι ενεργός απο προηγούμενη κίνηση
							minimizeMap();
							//επόμεη θέση απο gps
							nextPos=myPos;
						}
						else
						{
							//μεγιστοποίηση χάρτη
							//maximizeMap();

							//αν υπάρχει perimetros
							if(moveSettings.deadSpotsOnSitePerimetroi.Count>0){
								//και βρίσκουμε το κοντινότρο σημείο πάνω stin perimetro
								snapPoint=gpsPosition.FindSnapPosition(myPos,moveSettings.deadSpotsOnSitePerimetroi);

								maxDistance = moveSettings.maxSnapOutOfAreaOnsite;

								//αν έχουμε snap point και είναι εντός μέγιστης αποδεκτής απόστασης
								if(snapPoint!=null && snapPoint.sqrDistance < maxDistance * maxDistance)
								{
									//θέτουμε ότι είναι σε μονοπάτι
									spot=Spot.inArea;
									//προαιρετικά κλείνουμε τον μεγιστοποιήμενο χάρτη 
									//αν είναι ενεργός απο προηγούμενη κίνηση
									minimizeMap();
									//θέτουμε τη νέα μας θέση πάνω στο μονοπάτι
									nextPos=snapPoint.position;
								}else{
									spot=Spot.inDeadArea;
									//επόμεvη θέση απο gps
									nextPos=myPos;
								}


							}else{
								spot=Spot.inDeadArea;
								//επόμεη θέση απο gps
								nextPos=myPos;
							}
						}
					}
					else//if no deadspots in xml move free in area
					{
						//θέτουμε οτι είναι εντος ενεργής περιοχής
						spot=Spot.inArea;
						//προαιρετικά κλείνουμε τον μεγιστοποιήμενο χάρτη 
						//αν είναι ενεργός απο προηγούμενη κίνηση
						minimizeMap();
						//επόμεη θέση απο gps
						nextPos=myPos;
						
					}

					#if UNITY_EDITOR
					Debug.LogWarning("IN AREA CHECK");
					#endif

				}
//				else//if on air move free in area
//				if(Diadrasis.Instance.user==Diadrasis.User.onAir && Diadrasis.Instance.moveOnAir)
//				{
//					//θέτουμε οτι είναι εντος ενεργής περιοχής
//					spot=Spot.inArea;
//					//προαιρετικά κλείνουμε τον μεγιστοποιήμενο χάρτη 
//					//αν είναι ενεργός απο προηγούμενη κίνηση
//					minimizeMap();
//					//επόμεη θέση απο gps
//					nextPos=myPos;
//					
//				}
				
			}

			#endregion in area check


			#region FINAL MOVEMENT

			newPosPerson=new Vector3(nextPos.x, gpsPosition.FindHeight(nextPos), nextPos.y);
			prevPosPerson=nextPos;

			//get current position of person
			Vector3 pos=myTransform.position;

			//new method for finding height at avery frame (bug falling in hole fix)
			myTransform.position=Vector3.Lerp(new Vector3(pos.x, gpsPosition.FindHeight(new Vector2(pos.x,pos.z)), pos.z), newPosPerson, Time.deltaTime);
			prevGPS=curGPS;	

			#endregion
		}

		
		#endregion

		else

		#region MOVE FREE
		//αλλιως εχουμε ελευθερη πλοηγηση
		if(!moveSettings.snapToPath)
		{	
			spot=Spot.freeMove;
			
			minimizeMap();
			
			//move person to gps position
			nextPos=myPos;
			//set pos + height
			if(Vector2.Distance(prevPosPerson,nextPos)>1f){
				newPosPerson=new Vector3(nextPos.x, gpsPosition.FindHeight(nextPos), nextPos.y);
				prevPosPerson=nextPos;
			}
			//get current position of person
			Vector3 pos=myTransform.position;
			//new method for finding height at avery frame (bug falling in hole fix)
			myTransform.position=Vector3.Lerp(new Vector3(pos.x, gpsPosition.FindHeight(new Vector2(pos.x,pos.z)), pos.z), newPosPerson, Time.deltaTime);
			
			prevGPS=curGPS;
		}
		
		#endregion
	}
	
	#endregion
	
	#region JOYSTICK MOVEMENT
	
	//move with left joystick
	public void JoyMove(){
		
		//get input from left joystick
		Vector2 inputVector = new Vector3(CnInputManager.GetAxis("Horizontal"), CnInputManager.GetAxis("Vertical"));

		#region IF CHARACTER MOTOR

		if(cmotor){
			// Get the input vector from keyboard or analog stick//
			Vector3 directionVector = new Vector3 (inputVector.x, 0f, inputVector.y); 	//new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			
			if (directionVector != Vector3.zero) {
				// Get the length of the directon vector and then normalize it
				// Dividing by the length is cheaper than normalizing when we already have the length anyway
				float directionLength = directionVector.magnitude;
				directionVector = directionVector / directionLength;
				
				// Make sure the length is no bigger than 1
				directionLength = Mathf.Min(1f, directionLength);
				
				// Make the input vector more sensitive towards the extremes and less sensitive in the middle
				// This makes it easier to control slow speeds when using analog sticks
				directionLength = directionLength * directionLength;
				
				// Multiply the normalized direction vector by the modified length
				directionVector = directionVector * directionLength;
			}
			
			// Apply the direction to the CharacterMotor
			cmotor.inputMoveDirection = myCamera.rotation * directionVector;
			
			cmotor.inputJump = canJump;
			
			#if UNITY_EDITOR
			Debug.Log("cmotor");
			#endif
			
			return;
		}

		#endregion
		
		//if is up or down (mpros/piso kinisi)
		if(Mathf.Abs(inputVector.y) > Mathf.Abs(inputVector.x)){

//			#if UNITY_EDITOR
//			Debug.LogWarning("KINISI MPROS - PISO");
//			#endif

			//get direction from camera view (αξονα z)
			Vector3 fwd=myCamera.transform.TransformDirection(Vector3.forward);
			//the next pos will be at where the camera is looking + joystick (mpros/piso) * speed + now person position
			Vector2 pos2d = new Vector2(fwd.x,fwd.z) * inputVector.y * Time.deltaTime * moveSettings.groundMoveSpeed + new Vector2(myTransform.position.x, myTransform.position.z);
			moveOffsite (pos2d);
		}
		//if is left or right (plagia kinisi)
		else
		if(Mathf.Abs(inputVector.y) < Mathf.Abs(inputVector.x)){

//			#if UNITY_EDITOR
//			Debug.LogWarning("KINISI LEFT - RIGHT");
//			#endif

			//get direction from camera view (αξονα x)
			Vector3 fwd=myCamera.transform.TransformDirection(Vector3.right);
			//the next pos will be at where the camera is looking + joystick (left/right) * speed + now person position
			Vector2 pos2d = new Vector2(fwd.x,fwd.z) * inputVector.x * Time.deltaTime * moveSettings.groundMoveSpeed + new Vector2(myTransform.position.x, myTransform.position.z);
			moveOffsite (pos2d);
		}
	}

	void moveOffsite(Vector2 myPos)
	{
		
		#region MOVE IN PATH
		
		//αν είναι επιλεγμένη η υποχρωτική κίνηση σε μονοπάτι
		//και δεν είναι στον αέρα
		if(moveSettings.snapToPath && Diadrasis.Instance.user != Diadrasis.User.inFullMap)
		{
			if(WarningEventsUI.isEndOfMove){
				WarningEventsUI.isEndOfMove=false;
			}
			
			#region using graphic areas and pahs with map filter texture
			//GraphicsAndXml(isGps, myPos);
			//return;
			#endregion
			
			#region OUT OF AREA CHECK
			//αν είναι εκτός ενεργών περιοχών
			if (!isInArea(myPos,false))
			{
				closestDistanceFromArea=1000f;
				closestDistanceFromPath=1000f;

				if(Diadrasis.Instance.user==Diadrasis.User.isNavigating)
				{
					//αν υπάρχει perimetros
					if(moveSettings.activeAreasPerimetroi.Count>0){
						//δήλωνουμε οτι υπάρχει μονοπατι
						hasPerimetro=true;
						
						maxDistance = moveSettings.maxSnapOutOfAreaOnsite;
						
						//και βρίσκουμε το κοντινότρο σημείο πάνω stin perimetro
						snapAreaPoint=gpsPosition.FindSnapPosition(myPos,moveSettings.activeAreasPerimetroi);
						closestDistanceFromArea = Vector2.Distance(myPos, snapAreaPoint.position);
					}else{
						hasPerimetro=false;
					}

					//αν υπάρχει ενεργό μονοπάτι πλοήγησης απο το xml
					if(moveSettings.playerPath.Count>0){
						//δήλωνουμε οτι υπάρχει μονοπατι
						hasPaths=true;
						
						maxDistance = moveSettings.maxSnapPathDistOffsite;
						
						//και βρίσκουμε το κοντινότρο σημείο πάνω στο μονοπάτι
						snapPathPoint=gpsPosition.FindSnapPosition(myPos,moveSettings.playerPath);
						closestDistanceFromPath = Vector2.Distance(myPos, snapPathPoint.position);
					}else{
						//δηλώνουμε οτι δεν υπάρχει μονοπάτι
						hasPaths=false;
					}
				}else
				if(Diadrasis.Instance.user==Diadrasis.User.onAir && Diadrasis.Instance.moveOnAir)
				{
					if(moveSettings.onAirAreas.Count>0){
						//δήλωνουμε οτι υπάρχει μονοπατι
						hasPerimetro=true;

						maxDistance = moveSettings.maxSnapOutOfAreaOnsite;
						
						//και βρίσκουμε το κοντινότρο σημείο πάνω stin perimetro
						snapAreaPoint=gpsPosition.FindSnapPosition(myPos,moveSettings.activeAreasOnAirPerimetroi);
						closestDistanceFromArea = Vector2.Distance(myPos, snapAreaPoint.position);
					}else{
						hasPerimetro=false;
					}
				}
				
				if(closestDistanceFromArea<closestDistanceFromPath){
					snapPoint = snapAreaPoint;
					hasPaths=false;
				}else
				if(closestDistanceFromArea>closestDistanceFromPath){
					snapPoint = snapPathPoint;
					hasPerimetro=false;
				}
				
				//αν έχουμε perimetro και είναι εντός μέγιστης αποδεκτής απόστασης
				if (hasPerimetro && snapPoint.sqrDistance < maxDistance * maxDistance)
				{
					//θέση πάνω στο μονοπάτι
					Vector2 snapPos= snapAreaPoint.position;
					
					//μετακίνηση προς σε αυτή τη θέση
					transform.position = new Vector3(snapPos.x, gpsPosition.FindHeight(myPos), snapPos.y);
					
				}else//αν δεν έχουμε μονοπάτι / perimetro ή είναι εκτός μέγιστης αποδεκτής απόστασης
				{
					//θέτουμε ότι είναι εκτός περιοχής
					spot=Spot.outOfArea;

					//stop move
					if(!WarningEventsUI.isEndOfMove){
						WarningEventsUI.isEndOfMove=true;
					}
				}
				
				
				//αν έχουμε μονοπάτι και είναι εντός μέγιστης αποδεκτής απόστασης
				if (hasPaths && snapPoint.sqrDistance < maxDistance * maxDistance)
				{
					if(WarningEventsUI.isEndOfMove){
						WarningEventsUI.isEndOfMove=false;
					}

					//θέτουμε ότι είναι σε μονοπάτι
					spot=Spot.inPath;

					//θέση πάνω στο μονοπάτι
					Vector2 snapPos= snapPoint.position;
					
					//μετακίνηση προς σε αυτή τη θέση
					myTransform.position = new Vector3(snapPos.x, gpsPosition.FindHeight(myPos), snapPos.y);
					
				}
				else//αν δεν έχουμε μονοπάτι / perimetro ή είναι εκτός μέγιστης αποδεκτής απόστασης
				{
					//θέτουμε ότι είναι εκτός περιοχής
					spot=Spot.outOfArea;

					//stop move
					if(!WarningEventsUI.isEndOfMove){
						WarningEventsUI.isEndOfMove=true;
					}
				}
			}
			
			#endregion

			else

			#region IN AREA CHECK
			//αν είναι εντός ενεργής περιοχής
			if (isInArea(myPos, false))
			{
				if(Diadrasis.Instance.user==Diadrasis.User.isNavigating)
				{
				//if deadspots exists in xml
					if (moveSettings.deadSpots.Count>0)
					{
						//αν είναι εκτός νεκρής περιοχής απομακρυσμένης απο xml
						//και άρα εντός ενεργής
						if(!gpsPosition.PlayerInsideDeadArea(myPos,moveSettings.deadSpots))
						{
							//θέτουμε οτι είναι εντος ενεργής περιοχής
							spot=Spot.inArea;
							//επόμενη θέση απο joystick
							Vector3 nPos=new Vector3(myPos.x,gpsPosition.FindHeight(myPos), myPos.y);	
							myTransform.position= nPos;
						}
						else
						{
							//θέτουμε οτι είναι εντος απαγορευμένης περιοχής
							spot=Spot.inDeadArea;
							
							//dont move
							
							//αν υπάρχει perimetros
							if(moveSettings.deadSpotsPerimetroi.Count>0){
								//δήλωνουμε οτι υπάρχει μονοπατι
								hasDeadPerimterous=true;

								//TODO
								//perhaps set distance to 5meters so person stays always at perimeter of current dead area
								maxDistance = 0.2f;
								
								//και βρίσκουμε το κοντινότρο σημείο πάνω stin perimetro
								snapPathPoint=gpsPosition.FindSnapPosition(myPos,moveSettings.deadSpotsPerimetroi);
							}else{
								//δηλώνουμε οτι δεν υπάρχει μονοπάτι
								hasDeadPerimterous=false;
							}
							
							//αν έχουμε dead perimetro και είναι εντός μέγιστης αποδεκτής απόστασης
							if (hasDeadPerimterous && snapPathPoint.sqrDistance < maxDistance * maxDistance)
							{
								//θέση πάνω στο μονοπάτι
								Vector2 snapPos= snapPathPoint.position;
								
								//μετακίνηση προς σε αυτή τη θέση
								transform.position = new Vector3(snapPos.x, gpsPosition.FindHeight(myPos), snapPos.y);
								
								//Debug.Log("limits are "+gpsPosition.FindSnapPosition(myPos,moveSettings.playerPath).limitsOn);
								
								//αν απο το xml είναι ενεργό το περιθώριο κίνησης εκτός μονοπατιού
								//με όριο κάποια απόσταση apo το μονοπατι
	//							if(!gpsPosition.FindSnapPosition(myPos,moveSettings.deadSpotsPerimetroi).limitsOn)
	//							{
	//								//								Debug.Log("dead limits are ON");
	//								
	//								//get pos from joy
	//								Vector3 nPos=new Vector3(myPos.x, gpsPosition.FindHeight(myPos), myPos.y);
	//								//TODO
	//								//move person with lerp
	//								transform.position = nPos;
	//								//get limits from path
	//								minX = snapPos.x - anoxiFromMonopati;   //Debug.Log(minX);
	//								maxX = snapPos.x + anoxiFromMonopati;	//Debug.Log(maxX);
	//								minZ = snapPos.y - anoxiFromMonopati;	//Debug.Log(minZ);
	//								maxZ = snapPos.y + anoxiFromMonopati;	//Debug.Log(maxZ);
	//								//keep movement in limits
	//								transform.position=new Vector3(Mathf.Clamp(transform.position.x , minX , maxX), gpsPosition.FindHeight(myPos), Mathf.Clamp(transform.position.z , minZ , maxZ));
	//							}else{
	//								//								Debug.Log("dead limits are OFF");
	//							}
							}
						}
					}
					else//if no deadspots in xml move free in area
					{
						//θέτουμε οτι είναι εντος ενεργής περιοχής
						spot=Spot.inArea;
						//επόμενη θέση απο joystick
						Vector3 nPos=new Vector3(myPos.x,gpsPosition.FindHeight(myPos), myPos.y);	
						myTransform.position= nPos;
					}
				}else//if on air move free into the area
				if(Diadrasis.Instance.user==Diadrasis.User.onAir && Diadrasis.Instance.moveOnAir)
				{
					//θέτουμε οτι είναι εντος ενεργής περιοχής
					spot=Spot.inArea;
					//επόμενη θέση απο joystick
					Vector3 nPos=new Vector3(myPos.x,gpsPosition.FindHeight(myPos), myPos.y);	
					myTransform.position= nPos;
				}
			}
			
			#endregion

		}
		
		#endregion

		else

		#region MOVE FREE
		//αλλιως εχουμε ελευθερη πλοηγηση

		if(!moveSettings.snapToPath)
		{	
			spot=Spot.freeMove;
			
			minimizeMap();
			
			//move person to gps position
			nextPos=myPos;
			//set pos + height
			if(Vector2.Distance(prevPosPerson,nextPos)>1f){
				newPosPerson=new Vector3(nextPos.x, gpsPosition.FindHeight(nextPos), nextPos.y);
				prevPosPerson=nextPos;
			}
			//get current position of person
			Vector3 pos=myTransform.position;
			//new method for finding height at avery frame (bug falling in hole fix)
			myTransform.position=Vector3.Lerp(new Vector3(pos.x, gpsPosition.FindHeight(new Vector2(pos.x,pos.z)), pos.z), newPosPerson, Time.deltaTime);
			
			prevGPS=curGPS;
		}
		
		#endregion
		
	}
	
	#endregion

	#region GET NEAREST POINT ON ANSASER DOWN
	cSnapPosition pathPoint, areaPoint, deadPoint;
	
	Vector2 nearestSpotPosition(Vector2 personPos){
		
		//float closestDistFromArea=1000f;
		//float closestDistFromPath=1000f;
		//float closestDistFromDeadArea=1000f;
		float[] apostaseis = new float[]{1000f, 1000f, 1000f};

		#region OFFSITE
		//if offsite
		if(Diadrasis.Instance.navMode==Diadrasis.NavMode.offSite)
		{
			//if in area ground
			if (isInAreaGround(personPos,false) && (!gpsPosition.PlayerInsideDeadArea(personPos,moveSettings.deadSpots)))
			{
				return personPos;

				/*
				#region NOT IN USE
				//αν υπάρχει perimetros
				if(moveSettings.activeAreasPerimetroi.Count>0){
					//δήλωνουμε οτι υπάρχει μονοπατι
					hasPerimetro=true;
					
					//και βρίσκουμε το κοντινότρο σημείο πάνω stin perimetro
					snapAreaPoint=gpsPosition.FindSnapPosition(personPos,moveSettings.activeAreasPerimetroi);
					closestDistanceFromArea = Vector2.Distance(personPos, snapAreaPoint.position);
				}else{
					hasPerimetro=false;
				}
				
				//αν υπάρχει ενεργό μονοπάτι πλοήγησης απο το xml
				if(moveSettings.playerPath.Count>0){
					//δήλωνουμε οτι υπάρχει μονοπατι
					hasPaths=true;
					
					//και βρίσκουμε το κοντινότρο σημείο πάνω στο μονοπάτι
					snapPathPoint=gpsPosition.FindSnapPosition(personPos,moveSettings.playerPath);
					closestDistanceFromPath = Vector2.Distance(personPos, snapPathPoint.position);
				}else{
					//δηλώνουμε οτι δεν υπάρχει μονοπάτι
					hasPaths=false;
				}
				
				if(closestDistanceFromArea<closestDistanceFromPath){
					snapPoint = snapAreaPoint;
					//θέση πάνω στο μονοπάτι
					Vector2 snapPos= snapAreaPoint.position;
					//μετακίνηση προς σε αυτή τη θέση
					return snapPos;
				}else
				if(closestDistanceFromArea>closestDistanceFromPath){
					snapPoint = snapPathPoint;
					//θέση πάνω στο μονοπάτι
					Vector2 snapPos= snapPoint.position;
					//μετακίνηση προς σε αυτή τη θέση
					return snapPos;
				}
				
				return personPos;

				#endregion
				*/
			}
			else//check for nearest
			{
					//αν υπάρχει perimetros
					if(moveSettings.deadSpotsPerimetroi.Count>0){
						deadPoint = new cSnapPosition();
						//και βρίσκουμε το κοντινότρο σημείο πάνω stin perimetro
						deadPoint=gpsPosition.FindSnapPosition(personPos,moveSettings.deadSpotsPerimetroi);

						//save distance
						apostaseis[0] = Vector2.Distance(personPos, deadPoint.position);

					}

					//αν υπάρχει perimetros
					if(moveSettings.activeAreasPerimetroi.Count>0){
						areaPoint = new cSnapPosition();
						//και βρίσκουμε το κοντινότρο σημείο πάνω stin perimetro
						areaPoint=gpsPosition.FindSnapPosition(personPos,moveSettings.activeAreasPerimetroi);
						//save distance
						apostaseis[1] = Vector2.Distance(personPos, areaPoint.position);
					}

					if(moveSettings.playerPath.Count>0){
						pathPoint = new cSnapPosition();
						//και βρίσκουμε το κοντινότρο σημείο πάνω στο μονοπάτι
						pathPoint=gpsPosition.FindSnapPosition(personPos,moveSettings.playerPath);
						//save distance
						apostaseis[2] = Vector2.Distance(personPos, pathPoint.position);
					}

					//check if area is nearest
					if(Mathf.Min(apostaseis)==apostaseis[0]){
						return deadPoint.position;
					}else
					if(Mathf.Min(apostaseis)==apostaseis[1]){
						return areaPoint.position;
					}else
					if(Mathf.Min(apostaseis)==apostaseis[2]){
						return pathPoint.position;
					}


				return personPos;
			}

			//return personPos;
		}



		#endregion

		#region ONSITE

		else
		if(Diadrasis.Instance.navMode==Diadrasis.NavMode.onSite)
		{
			//if in area ground
			if (isInAreaGround(personPos,true) && (!gpsPosition.PlayerInsideDeadArea(personPos,moveSettings.deadSpotsOnSite)))
			{
				return personPos;

			}
			else//check for nearest
			{
				//αν υπάρχει perimetros
				if(moveSettings.deadSpotsOnSitePerimetroi.Count>0){
					deadPoint = new cSnapPosition();
					//και βρίσκουμε το κοντινότρο σημείο πάνω stin perimetro
					deadPoint=gpsPosition.FindSnapPosition(personPos,moveSettings.deadSpotsOnSitePerimetroi);
					
					//save distance
					apostaseis[0] = Vector2.Distance(personPos, deadPoint.position);
					
				}
				
				//αν υπάρχει perimetros
				if(moveSettings.activeAreasOnSitesPerimetroi.Count>0){
					areaPoint = new cSnapPosition();
					//και βρίσκουμε το κοντινότρο σημείο πάνω stin perimetro
					areaPoint=gpsPosition.FindSnapPosition(personPos,moveSettings.activeAreasOnSitesPerimetroi);
					//save distance
					apostaseis[1] = Vector2.Distance(personPos, areaPoint.position);
				}
				
				if(moveSettings.pathOnSite.Count>0){
					pathPoint = new cSnapPosition();
					//και βρίσκουμε το κοντινότρο σημείο πάνω στο μονοπάτι
					pathPoint=gpsPosition.FindSnapPosition(personPos,moveSettings.pathOnSite);
					//save distance
					apostaseis[2] = Vector2.Distance(personPos, pathPoint.position);
				}
				
				//check if area is nearest
				if(Mathf.Min(apostaseis)==apostaseis[0]){
					return deadPoint.position;
				}else
				if(Mathf.Min(apostaseis)==apostaseis[1]){
					return areaPoint.position;
				}else
				if(Mathf.Min(apostaseis)==apostaseis[2]){
					return pathPoint.position;
				}
				
				
				return personPos;
			}
			
			
		}


		#endregion

		return personPos;
		
	}

	#endregion
	
	#region JOYSTICK CAMERA ROTATION
	
	public void JoyRot(){
		//Edit/Project Settings/Input -> add 2 more axis Horizontal1 and Vertical1
		Vector3 inputVector2 = new Vector3(CnInputManager.GetAxis("Horizontal1"), CnInputManager.GetAxis("Vertical1"));
		
		myCamera.Rotate(Vector3.up, inputVector2.x * Time.deltaTime * moveSettings.joyRightSensitivity);
		myCamera.Rotate(Vector3.right, -inputVector2.y * Time.deltaTime * moveSettings.joyRightSensitivity);
		
		Vector3 rotAngles = myCamera.localEulerAngles;
		//		Debug.Log(rotAngles.x);
		rotAngles.z=0f;
		if(rotAngles.x<315f && rotAngles.x>180f){rotAngles.x=315f;}else
		if(rotAngles.x<180f && rotAngles.x>25f){rotAngles.x=25f;}
		myCamera.localEulerAngles = rotAngles;
	}
	
	#endregion
	
	#region KAMERA ROTATION RESET
	
	//entoli apo spot button in scene map
	//reset kamera rotation so person look at the view we want
	public void ResetKamera()
	{
		//Debug.Log("Reseting Kamera");
		
		Vector3 rot = myCamera.localEulerAngles;
		rot = Vector3.zero;
		myCamera.localEulerAngles = rot;
		if(Diadrasis.Instance.sensorUsing==Diadrasis.SensorUsing.gyroscopio)
		{
			ResetGyro();
		}
	}
	
	#endregion

	#region MIN-MAX MAP

	void minimizeMap(){
		if(Diadrasis.Instance.user==Diadrasis.User.inFullMap){
			//set user current status
			Diadrasis.Instance.ChangeStatus(Diadrasis.User.isNavigating);
			Diadrasis.Instance.FullMapClose();
		}
	}


	void maximizeMap(){
		if(Diadrasis.Instance.user!=Diadrasis.User.inFullMap){
			Diadrasis.Instance.FullScreenMap();
		}
	}

	#endregion

	#region IN AREA CHECK

	bool isInArea(Vector2 pos,bool isOnSite)
	{
		if(isOnSite){

			if(Diadrasis.Instance.user==Diadrasis.User.isNavigating || (Diadrasis.Instance.user==Diadrasis.User.onAir && Diadrasis.Instance.moveOnAir))
			{
				if(moveSettings.activeAreasOnSite.Count>0f && gpsPosition.PlayerInsideArea(pos,moveSettings.activeAreasOnSite)){
					#if UNITY_EDITOR
					Debug.LogWarning("in area");
					#endif
					return true;
				}
				#if UNITY_EDITOR
				Debug.LogWarning("out of area");
				#endif
				return false;
			}
		}else
		if(!isOnSite){
			if(Diadrasis.Instance.user==Diadrasis.User.onAir && Diadrasis.Instance.moveOnAir)
			{

				if (moveSettings.onAirAreas.Count>0f && gpsPosition.PlayerInsideArea(pos,moveSettings.onAirAreas)){
//					#if UNITY_EDITOR
//					Debug.LogWarning("in area on air!!!");
//					#endif
					return true;
				}

//				#if UNITY_EDITOR
//				Debug.LogWarning("false");
//				#endif
				return false;
			}else
			if(Diadrasis.Instance.user==Diadrasis.User.isNavigating)
			{
				if (moveSettings.activeAreas.Count>0f && gpsPosition.PlayerInsideArea(pos,moveSettings.activeAreas)){
					return true;
				}

				#if UNITY_EDITOR
				Debug.LogWarning("out of area");
				#endif

				return false;
			}
		}

		#if UNITY_EDITOR
		Debug.LogWarning("out of area");
		#endif
		return false;
	}

	bool isInAreaGround(Vector2 pos,bool isOnSite)
	{
		if(isOnSite){
			if(moveSettings.activeAreasOnSite.Count>0f && gpsPosition.PlayerInsideArea(pos,moveSettings.activeAreasOnSite)){
				return true;
			}
		}else{
			if (moveSettings.activeAreas.Count>0f && gpsPosition.PlayerInsideArea(pos,moveSettings.activeAreas)){
				return true;
			}
		}
		return false;
	}

	#endregion

	#region GYROSKOPIO

	public void Gyroskopio(){

		if(myGyro==null){
			return;
		}
		//calculate rotation for all
		quatMap = new Quaternion(myGyro.attitude.x,myGyro.attitude.y,myGyro.attitude.z, myGyro.attitude.w);
		//Horizontal offset
		//myCamera.transform.localRotation = quatMap * quatMult * Quaternion.Euler(UserInterface.hSliderVertAngleOffset, 0f, 0f);
		float threshold = 0.02f;
		if (myGyro.rotationRate.x < -threshold || myGyro.rotationRate.x > threshold ||
		    myGyro.rotationRate.y < -threshold || myGyro.rotationRate.y > threshold || 
		    myGyro.rotationRate.z < -threshold || myGyro.rotationRate.z > threshold )
		{
			myCamera.localRotation = Quaternion.Lerp(myCamera.localRotation,quatMap * quatMult * Quaternion.Euler(moveSettings.cameraVertAngleOffset, 0f, 0f),5*Time.deltaTime);
			//myCamera.transform.localRotation = quatMap * quatMult * Quaternion.Euler(UserInterface.hSliderVertAngleOffset, 0f, 0f);
			
		}
		
		//Vertical offset
		myTransform.eulerAngles = new Vector3(90f, + moveSettings.cameraHorAngleOffset-180f, 0f);
	}
	
	//entering the scene
	public void ResetGyro(){
		
		myTransform.eulerAngles = Vector3.zero;
		myCamera.localEulerAngles = Vector3.zero;
//		myTransform.eulerAngles = Vector3.zero;
		myTransform.rotation = Quaternion.identity;
		myCamera.rotation=Quaternion.identity;
		
//		if (appSettings.mode==appSettings.Mode.gpsCheck){
//			myTransform.eulerAngles = new Vector3(90f, -180f, 0f);
//			myCamera.localEulerAngles=new Vector3(0f,0f,0f);
//		}else{
			quatMult = new Quaternion(0f, 0f, 1f, 0f);	
			quatMap = new Quaternion(0f, 0f, 0f, 0f);
			myCamera.localRotation = quatMap * quatMult;
			//Vertical offset
			myTransform.eulerAngles = new Vector3(90f, -180f, 0f);
			
//		}
		
	}

	#endregion

	#region ASANSER

	public void MovePersonUpDown()
	{
		//Debug.Log("MovePersonUpDown");
		StopCoroutine("asanser");
		StartCoroutine("asanser");
	}


	public void Ypsos(bool down)
	{
		if(down)
		{
			spot=Spot.stopMove;
		}
		else
		{
			spot=Spot.onAir;
		}
	}


//	bool isUp=false;
	//float newPosY=0f;
	//float onAirPosY=0f;
	
	public IEnumerator asanser()
	{
		Vector3 personPos = myTransform.position;	//Debug.Log("personPos = "+personPos);

		if(Diadrasis.Instance.user==Diadrasis.User.isNavigating)
		{

			#if UNITY_EDITOR
			Debug.Log("asanser up");
			#endif

			if (cmotor) {
				cmotor.enabled = false;
			}

			//if tablet
			if(Diadrasis.Instance.screenSize<2){
				Diadrasis.Instance.menuUI.btnsMenu.imgPersonMoveUpDown.overrideSprite = Diadrasis.Instance.menuUI.btnsMenu.btnUpDownSprites[0];
			}else{
				Diadrasis.Instance.menuUI.btnsMenu.imgPersonMoveUpDown.overrideSprite = Diadrasis.Instance.menuUI.btnsMenu.btnUpDownSprites[2];
			}

			Diadrasis.Instance.ChangeStatus(Diadrasis.User.inAsanser);

			spot=Spot.onAir;

			Diadrasis.Instance.menuUI.xartis.SetMapPreviusStatus();

			#if UNITY_EDITOR
			Debug.Log("close map !!!");
			#endif

			Diadrasis.Instance.animControl.CloseMap();
			
			if(!Diadrasis.Instance.moveOnAir)
			{
				Diadrasis.Instance.menuUI.HideAllJoys();

				yield return new WaitForSeconds (0.1f);
				
				while(myTransform.position.y<moveSettings.personOnAirAltitude)
				{
					myTransform.position = Vector3.Lerp(myTransform.position,new Vector3(personPos.x, moveSettings.personOnAirAltitude+1f,personPos.z),Time.deltaTime);
					yield return null;
				}
			}else
			if(Diadrasis.Instance.moveOnAir){

				if (Diadrasis.Instance.navMode == Diadrasis.NavMode.onSite) {
					yield return new WaitForSeconds (0.1f);

					while (myTransform.position.y < moveSettings.personOnAirAltitude) {
						myTransform.position = Vector3.Lerp (myTransform.position, new Vector3 (personPos.x, moveSettings.personOnAirAltitude + 1f, personPos.z), Time.deltaTime);
						yield return null;
					}
				} else {

					bool isInAirArea = false;

					if (moveSettings.onAirAreas.Count > 0f && gpsPosition.PlayerInsideArea (new Vector2 (personPos.x, personPos.z), moveSettings.onAirAreas)) {
						isInAirArea = true;
					}

					if (!isInAirArea) {
						if (moveSettings.onAirAreas.Count > 0) {
							//και βρίσκουμε το κοντινότρο σημείο πάνω stin perimetro
							cSnapPosition snapPoint = gpsPosition.FindSnapPosition (new Vector2 (personPos.x, personPos.z), moveSettings.activeAreasOnAirPerimetroi);


							Vector2 p = snapPoint.position;
							Vector2 userPos = new Vector2 (personPos.x, personPos.z);
						
						
							while (!FastApproximately (p.x, userPos.x, 0.1f) || !FastApproximately (p.y, userPos.y, 0.1f)) {
								myTransform.position = Vector3.Lerp (myTransform.position, new Vector3 (p.x, moveSettings.personOnAirAltitude, p.y), Time.deltaTime * 2f); //Vector3.Lerp(myTransform.position,new Vector3(personPos.x,newPosY-1f,personPos.z),Time.deltaTime);
								userPos = new Vector2 (myTransform.position.x, myTransform.position.z);
								yield return null;
							}
						}
					} else {
						yield return new WaitForSeconds (0.1f);
					
						while (myTransform.position.y < moveSettings.personOnAirAltitude) {
							myTransform.position = Vector3.Lerp (myTransform.position, new Vector3 (personPos.x, moveSettings.personOnAirAltitude + 1f, personPos.z), Time.deltaTime);
							yield return null;
						}
					}

				}
			}

			#if UNITY_EDITOR
			Debug.Log("PERSON IS UP !!");
			#endif

			Diadrasis.Instance.ChangeStatus(Diadrasis.User.onAir);

			yield break;
			
		}
		else
		if(Diadrasis.Instance.user==Diadrasis.User.onAir)
		{
			#if UNITY_EDITOR
			Debug.Log("asanser down");
			#endif

			Diadrasis.Instance.ChangeStatus(Diadrasis.User.inAsanser);

			//if tablet
			if(Diadrasis.Instance.screenSize<2){
				Diadrasis.Instance.menuUI.btnsMenu.imgPersonMoveUpDown.overrideSprite = Diadrasis.Instance.menuUI.btnsMenu.btnUpDownSprites[1];
			}else{
				Diadrasis.Instance.menuUI.btnsMenu.imgPersonMoveUpDown.overrideSprite = Diadrasis.Instance.menuUI.btnsMenu.btnUpDownSprites[3];
			}

			if (Diadrasis.Instance.navMode == Diadrasis.NavMode.offSite)
			{

				if (Diadrasis.Instance.moveOnAir)
				{

					#if UNITY_EDITOR
					Debug.Log ("close map !!!");
					#endif
					Diadrasis.Instance.animControl.CloseMap ();

					//TODO
					//check if a building is down

					yield return new WaitForSeconds (0.1f);

					Vector2 p = nearestSpotPosition (new Vector2 (myTransform.position.x, myTransform.position.z));
					Vector2 userPos = new Vector2 (myTransform.position.x, myTransform.position.z);

					float newHeight = gpsPosition.FindHeight (new Vector2 (personPos.x, personPos.z));
					//an to neo simeio einai pio psila katebase ton taytoxrona me tin kinisi
					if (newHeight > personPos.y) {
						personPos.y = newHeight;
					}
					//allios prota kinisi kai meta katebasma

					while (!FastApproximately (p.x, userPos.x, 0.1f) || !FastApproximately (p.y, userPos.y, 0.1f)) {
						myTransform.position = Vector3.Lerp (myTransform.position, new Vector3 (p.x, personPos.y, p.y), Time.deltaTime * 2f); //Vector3.Lerp(myTransform.position,new Vector3(personPos.x,newPosY-1f,personPos.z),Time.deltaTime);
						userPos = new Vector2 (myTransform.position.x, myTransform.position.z);
						yield return null;
					}

					//set new position
					personPos = myTransform.position;
				}

			}

			//set new pos y for person
			personPos.y = gpsPosition.FindHeight(new Vector2(personPos.x, personPos.z));//newPosY
			
			yield return new WaitForSeconds (0.1f);
			
			while(myTransform.position.y>personPos.y)
			{
				myTransform.position = Vector3.Lerp(myTransform.position,new Vector3(personPos.x,personPos.y-1f,personPos.z),Time.deltaTime); //Vector3.Lerp(myTransform.position,new Vector3(personPos.x,newPosY-1f,personPos.z),Time.deltaTime);
				yield return null;
			}

			#if UNITY_EDITOR
			Debug.Log("PERSON IS DOWN !!");
			#endif
		

			if (cmotor) {
				cmotor.enabled = true;
			}

			if(Diadrasis.Instance.sensorUsing!=Diadrasis.SensorUsing.joysticks)
			{
				if(Diadrasis.Instance.navMode==Diadrasis.NavMode.offSite){
					//add one joystick for move
					Diadrasis.Instance.menuUI.joy.singleJoyLeft.SetActive(true);
				}else{
					Diadrasis.Instance.menuUI.joy.singleJoyLeft.SetActive(false);
				}
			}
			else
				if(Diadrasis.Instance.sensorUsing==Diadrasis.SensorUsing.joysticks)
			{
				if(Diadrasis.Instance.navMode==Diadrasis.NavMode.offSite){
					//add 2 joysticks (left) move - (right) camera rotation
					Diadrasis.Instance.menuUI.joy.dualJoys.SetActive(true);
				}else{
					//add 1 joystick for camera rot and moves with gps
					Diadrasis.Instance.menuUI.joy.singleJoyRight.SetActive(true);
				}
			}

			spot=Spot.stopMove;
			
			Diadrasis.Instance.ChangeStatus(Diadrasis.User.isNavigating);		

			yield break;
		}
	}


	static bool FastApproximately(float a, float b, float threshold)
	{
		return ((a - b) < 0 ? ((a - b) * -1) : (a - b)) <= threshold;
	}

	#endregion

	#region DEPRECATED

	//	float distFromScene =0f;
	
	/*

	bool isUserNearToScene()
	{
//		if(personPos!=Vector2.zero && Diadrasis.Instance.sceneGpsPosition!=Vector2.zero)
//		{
			distFromScene=Vector3.Distance(myTransform.position,Vector3.zero);
			//Debug.Log("person pos = "+myTransform.position);
			//Debug.Log("distFromScene="+distFromScene);
			//TODO
			//set max distance foreach scene from xml
			if(distFromScene<300f)
			{
				//					Debug.Log("close to an area = "+(Vector2.Distance(posUser,p)* 3.38f).ToString());
//				Debug.Log("PERSON IS NEAR "+ distFromScene);
				return true;
			}
//		}
//		Debug.Log("FULL MAP - PERSON IS FAR AWAY");
		return false;
	}

*/

	public void BtnMove()
	{
		//get direction from camera view (αξονα z)
		Vector3 fwd=myCamera.transform.TransformDirection(Vector3.forward);
		//the next pos will be at where the camera is looking + joystick (mpros/piso) * speed + now person position
		Vector2 pos2d = new Vector2(fwd.x,fwd.z) * 1f * Time.deltaTime * moveSettings.groundMoveSpeed + new Vector2(myTransform.position.x, myTransform.position.z);
		//			//check if next pos is in path or area
		moveOffsite (pos2d);
	}

	/*
		public static cArea currentArea;
	
		public void LateUpdate(){
	
			if(Diadrasis.Instance.menuUI.xartis.isPersonInGraphicsArea){
				lastAllowedPosition = myTransform.position;
	
				#if UNITY_EDITOR
	//			Debug.LogWarning(lastAllowedPosition);
				#endif
			}
	
			
		}
	*/

	#region FIND HEIGHT
	float FindHeight(Vector2 pos){
		
		//declare 2 values
		float rayHeight=0f;
		float terrainHeight=0f;
		
		//get terrain height
		if (Terrain.activeTerrain != null) {
			//		if (moveSettings.bUseTerrainForHeight)
			terrainHeight = Terrain.activeTerrain.SampleHeight (new Vector3 (pos.x, 0, pos.y)) + moveSettings.playerHeight + Terrain.activeTerrain.transform.position.y;
		}
		
		RaycastHit hit;
		
		//get last y position of person to make a raycast
		float prevPosY = myTransform.position.y;
		
		//hit down from last y position of player
		Ray downRay = new Ray(new Vector3(pos.x,prevPosY,pos.y), -Vector3.up);
		
		if (Physics.Raycast(downRay, out hit,Mathf.Infinity)){
			//			if(!hit.transform.name.Contains("Person")){
			//get hit distance and add person height
			rayHeight = (prevPosY - hit.distance) + moveSettings.playerHeight;
			//			}
		}
		
		//if terrain height ! ray height 
		if(terrainHeight!=rayHeight){
			//find difference
			float dif = Mathf.Abs(terrainHeight-rayHeight);
			if(dif>2f && terrainHeight>rayHeight){
				return terrainHeight;
			}
		}
		
		return rayHeight;
	}
	#endregion
	
	#region GRAPHICS AND PATHS
	/*
	void GraphicsAndXml(bool isGps, Vector2 myPos){
		if(Diadrasis.Instance.useMapFilterForMovement)
		{
			
			if(Diadrasis.Instance.menuUI.xartis.isPersonInGraphicsArea)
			{
				//προαιρετικά κλείνουμε τον μεγιστοποιήμενο χάρτη 
				//αν είναι ενεργός απο προηγούμενη κίνηση
				minimizeMap();
				
				//θέτουμε οτι είναι εντος ενεργής περιοχής
				spot=Spot.inArea;
				
				//αν είμαστε επιτόπου κίνηση
				//σταδιακή μεσω gps
				if(isGps)
				{
					nextPos = myPos;
				}else{
					//μετακίνηση προς σε αυτή τη θέση
					myTransform.position = new Vector3(myPos.x, gpsPosition.FindHeight(myPos), myPos.y);
				}
				
			}else{
				
				#region GRAPHICS ONLY
				

//
//					//θέτουμε ότι είναι εκτός περιοχής
//					spot=Spot.outOfArea;
//					//αν είναι κίνηση μέσω gps
//					if(isGps)
//					{
//						//θέτουμε τη νέα μας θέση απο το gps
//						nextPos=myPos;
//
//						//μεγιστοποίηση χάρτη only onsite
//						maximizeMap();
//					}
//					else//αν έχουμε απομακρυσμένη κίνηση
//					{
//						//stop move
//						if(!WarningEventsUI.isEndOfMove){
//							WarningEventsUI.isEndOfMove=true;
//						}
//
//						if(Diadrasis.Instance.menuUI.xartis.personAllowPosition!=Vector3.zero){
//							//get current position of person
//							Vector3 pos=myTransform.position;
//
//
//							if(Vector3.Distance(pos,Diadrasis.Instance.menuUI.xartis.personAllowPosition)>1f){
//								speedIntoArea=4f;
//							}else{
//								speedIntoArea=1f;
//							}
//
//							//TODO
//							//find first x or z value that is accepted by graphics filter position
//
//
//							myTransform.position = Vector3.MoveTowards(new Vector3(pos.x, gpsPosition.FindHeight(new Vector2(pos.x,pos.z)), pos.z), Diadrasis.Instance.menuUI.xartis.personAllowPosition, Time.deltaTime * speedIntoArea);
//						}
//					}


				
				#endregion
				
				
				#region GRAPHICS & PATHS
				//αν είναι κίνηση μέσω gps
				if(isGps)
				{
					//αν υπάρχει ενεργό μονοπάτι απομακρυσμένης πλοήγησης απο το xml
					if(moveSettings.pathOnSite.Count>0){
						//δήλωνουμε οτι υπάρχει μονοπατι
						hasPaths=true;
						
						maxDistance = moveSettings.maxSnapPathDistOnsite;
						
						//και βρίσκουμε το κοντινότρο σημείο πάνω στο μονοπάτι
						snapPathPoint=gpsPosition.FindSnapPosition(myPos,moveSettings.pathOnSite);
					}else{
						//δηλώνουμε οτι δεν υπάρχει μονοπάτι
						hasPaths=false;
					}
				}
				else//αν έχουμε απομακρυσμένη κίνηση
				{
					//αν υπάρχει ενεργό μονοπάτι επιτόπιας πλοήγησης απο το xml
					if(moveSettings.playerPath.Count>0){
						//δήλωνουμε οτι υπάρχει μονοπατι
						hasPaths=true;
						
						maxDistance = moveSettings.maxSnapPathDistOffsite;
						
						//και βρίσκουμε το κοντινότρο σημείο πάνω στο μονοπάτι
						snapPathPoint=gpsPosition.FindSnapPosition(myPos,moveSettings.playerPath);
					}else{
						//δηλώνουμε οτι δεν υπάρχει μονοπάτι
						hasPaths=false;
					}
				}
				
				//αν έχουμε μονοπάτι και είναι εντός μέγιστης αποδεκτής απόστασης
				if (hasPaths && snapPathPoint.sqrDistance < maxDistance * maxDistance)
				{
					//θέτουμε ότι είναι σε μονοπάτι
					spot=Spot.inPath;
					//αν είναι κίνηση μέσω gps
					if(isGps)
					{
						//προαιρετικά κλείνουμε τον μεγιστοποιήμενο χάρτη 
						//αν είναι ενεργός απο προηγούμενη κίνηση
						minimizeMap();
						//θέτουμε τη νέα μας θέση πάνω στο μονοπάτι
						nextPos=snapPathPoint.position;
					}
					else//αν έχουμε επιτόπια κίνηση
					{
						//θέση πάνω στο μονοπάτι
						Vector2 snapPos= snapPathPoint.position;
						
						//μετακίνηση προς σε αυτή τη θέση
						myTransform.position = new Vector3(snapPos.x, gpsPosition.FindHeight(myPos), snapPos.y);
						
						//Debug.Log("limits are "+gpsPosition.FindSnapPosition(myPos,moveSettings.playerPath).limitsOn);
						
						//αν απο το xml είναι ενεργό το περιθώριο κίνησης εκτός μονοπατιού
						//με όριο κάποια απόσταση apo το μονοπατι
//						if(!gpsPosition.FindSnapPosition(myPos,moveSettings.playerPath).limitsOn)
//						{
//							//get pos from joy
//							Vector3 nPos=new Vector3(myPos.x, gpsPosition.FindHeight(myPos), myPos.y);
//							//TODO
//							//move person with lerp
//							myTransform.position = nPos;
//							//get limits from path
//							minX = snapPos.x - anoxiFromMonopati;   //Debug.Log(minX);
//							maxX = snapPos.x + anoxiFromMonopati;	//Debug.Log(maxX);
//							minZ = snapPos.y - anoxiFromMonopati;	//Debug.Log(minZ);
//							maxZ = snapPos.y + anoxiFromMonopati;	//Debug.Log(maxZ);
//							//keep movement in limits
//							myTransform.position=new Vector3(Mathf.Clamp(myTransform.position.x , minX , maxX), gpsPosition.FindHeight(myPos), Mathf.Clamp(myTransform.position.z , minZ , maxZ));
//						}
					}
				}
				else//αν δεν έχουμε μονοπάτι ή είναι εκτός μέγιστης αποδεκτής απόστασης
				{
					//θέτουμε ότι είναι εκτός περιοχής
					spot=Spot.outOfArea;
					//αν είναι κίνηση μέσω gps
					if(isGps)
					{
						//θέτουμε τη νέα μας θέση απο το gps
						nextPos=myPos;
						
						//μεγιστοποίηση χάρτη only onsite
						maximizeMap();
					}
					else//αν έχουμε απομακρυσμένη κίνηση
					{
						//stop move
						if(!WarningEventsUI.isEndOfMove){
							WarningEventsUI.isEndOfMove=true;
						}
						
						if(Diadrasis.Instance.menuUI.xartis.personAllowPosition!=Vector3.zero){
							//get current position of person
							Vector3 pos=myTransform.position;
							
							
							if(Vector3.Distance(pos,Diadrasis.Instance.menuUI.xartis.personAllowPosition)>1f){
								speedIntoArea=4f;
							}else{
								speedIntoArea=1f;
							}
							
							//TODO
							//find first x or z value that is accepted by graphics filter position
							
							
							myTransform.position = Vector3.MoveTowards(new Vector3(pos.x, gpsPosition.FindHeight(new Vector2(pos.x,pos.z)), pos.z), Diadrasis.Instance.menuUI.xartis.personAllowPosition, Time.deltaTime * speedIntoArea);
						}
					}
				}
				
				#endregion
			}
			
			//lerp gps movement
			if(isGps)
			{
				//set pos + height
				if(Vector2.Distance(prevPosPerson,nextPos)>1f){
					newPosPerson=new Vector3(nextPos.x, gpsPosition.FindHeight(nextPos), nextPos.y);
					prevPosPerson=nextPos;
				}
				//get current position of person
				Vector3 pos=myTransform.position;
				//new method for finding height at avery frame (bug falling in hole fix)
				myTransform.position=Vector3.Lerp(new Vector3(pos.x, gpsPosition.FindHeight(new Vector2(pos.x,pos.z)), pos.z), newPosPerson, Time.deltaTime);
				prevGPS=curGPS;	
			}else{
				
			}
			
			return;
		}
	}
	*/
	#endregion
	
	#endregion

}

