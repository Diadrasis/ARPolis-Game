using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using eChrono;

public class moveSettings : MonoBehaviour {
	//live parameters - get from globals
	public static Vector2 gpsRefLoc; //ref center of diagonial points A,B
	public static Vector2 gpsPointA; //ref point A
	public static Vector2 gpsPointB; //ref point B
	public static Vector2 posPointA;
	public static Vector2 posPointB;

	public static Vector2 posCenterOfMap;

	public static Vector2 gpsFakeRefLoc;
	public static Vector2 fakeRefLoc;

	public static float gpsUpdateTime = 0.0f;

	public static float gpsOffsetX=0f;
	public static float gpsOffsetY=0f;

	public static float locStartingAccuracy; //gps accuracy to start location services 
	public static float locAccuracy=7f; //gps accuracy limit
	public static float locUpdateDistance; //when to update gps
	public static float distFromCenter; //distance from scene ref point	
	
	//accuracy boolean parameters
	public static bool deviceHasGyro=false; //gyroscope is supported;
	public static bool deviceLocationEnabled=false; //false if one of the followings is false
	public static bool deviceHasLocationService=false; //gps location service is supported;
	public static bool deviceHasLocationServiceEnabled=false;//gpa location service is enabled;
	public static bool deviceIsInArea=false; //the device is in the active area - check radius from center;
	public static bool deviceHasLocationAccuracy=false; //is the accuracy within the accepted limits;
	
	//move parameters
	public static Vector2 startCamPosition; //the value comes from gCreatePoints
	public static Quaternion startCamRotation;  ////the value comes from gCreatePoints
	public static float groundMoveSpeed=5f;//the value comes from gCreatePoints
	//public static float flyMoveSpeed;//the value comes from gCreatePoints
	public static float maxSnapOutOfAreaOnsite = 10f;
	public static float maxSnapPathDistOnsite=30f;
	public static float maxSnapPathDistOffsite=20f;
	//public static float flyHeight=56f;//the value comes from gCreatePoints
	//public static float rotSpeed;//the value comes from gCreatePoints
	//public static float objDistance; //the distance from the object that the player stops
	public static float playerHeight=2f; //the height above the ground
	public static float personOnAirAltitude = 32f;

	public static float personHeight_IfOutOfArea;

	//use terrain or ray to calcualte height
	public static bool bUseTerrainForHeight=false;
	public static Vector2 terrainSize;


	//we have to transfer here the active areas also!
	public static List<cArea> onAirAreas = new List<cArea>();
	public static List<cArea> activeAreas = new List<cArea>();
	public static List<cArea> activeAreasOnSite = new List<cArea>();
	public static List<cArea> activeAreaForScene = new List<cArea>();
	public static List<cDeadSpot> deadSpots=new List<cDeadSpot>();
	public static List<cDeadSpot> deadSpotsOnSite=new List<cDeadSpot>();
	public static List<cLineSegment> playerPath=new List<cLineSegment>();
	public static List<cLineSegment> pathOnSite=new List<cLineSegment>();
	public static List<cLineSegment> activeAreasPerimetroi=new List<cLineSegment>();
	public static List<cLineSegment> activeAreasOnSitesPerimetroi=new List<cLineSegment>();
	public static List<cLineSegment> activeAreasOnAirPerimetroi=new List<cLineSegment>();
	public static List<cLineSegment> deadSpotsPerimetroi=new List<cLineSegment>();
	public static List<cLineSegment> deadSpotsOnSitePerimetroi=new List<cLineSegment>();
	public static List<cTarget> myTargets=new List<cTarget>();

	public static bool bPlayerInsideArea=false;
	public static bool bPlayerNearPath=false;

	//recognizing points
	public static float regDistance = 1000f; //the distance to recogzine objects
	public static float regRadious=2f; //the radius of the sphere cast
	
	//camera parameters
	public static float cameraFieldOfView ;
	public static float cameraHorAngleOffset=0f;
	public static float cameraVertAngleOffset=-20f;

	public static float compassTurnSpeed = 1f;

	public static float camAccelRotX=25f;
	public static float camAccelRotY=0f;
	public static float accelSensitivity = 0.1f;
	public static float joyRightSensitivity = 1f;
	
	//public enum LocMode{calc, prop};
	//calc: trigonometry calculation 
	//prop: proportional calculation
	//public static LocMode locMode=LocMode.prop;
	//public static LocMode locMode=LocMode.prop;	original

	public static bool snapToPath =true;

	/*
	public static bool canWalk = false;
	public static bool canLRotate = false;
	public static bool canZoom = false;
	public static bool canDrag = false;
	*/
}
