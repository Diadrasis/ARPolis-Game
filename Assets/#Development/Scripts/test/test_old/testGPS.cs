using UnityEngine;
using System.Collections;

public class testGPS : MonoBehaviour
{

	string msg;
	string msg2="null";
	string msg3="position";
	bool bLocationAvailable=false;
	Vector2 gpsRefLoc; //point 0


	float desiredAccuracy=5f;
	float updateDistance=1f;


	// Use this for initialization
	void Start ()
	{
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		gpsRefLoc=new Vector2(38.00667f, 23.79506f);

		//check if the system supports various sensors

		if (SystemInfo.supportsGyroscope){
			if (SystemInfo.supportsLocationService){
				if (Input.location.isEnabledByUser){
					Input.location.Start(desiredAccuracy, updateDistance);
					StartCoroutine(waitForInit());
				}else{
					msg+= "gps is not enabled by user... \n";
				}				
			}else{
				msg+="location service is not supported! \n";
			}
		}else{
			msg+="gyroscope is not supported! \n";
		}
	}

	IEnumerator waitForInit() {	
		//if ((Input.location.status == LocationServiceStatus.Initializing) && (count > 0)) {
		int count = 10;
		while (count>0){	
			if (Input.location.status == LocationServiceStatus.Initializing){ 
				msg += "...still waiting to initilize... \n" ;
			}else if (Input.location.status == LocationServiceStatus.Failed) {
				msg +="...failed...\n";
				yield break;
			}else if (Input.location.status == LocationServiceStatus.Running){
				msg +=" ...initilezed!... \n";
				bLocationAvailable=true;
				yield break;
			}else {
				msg +="...not available... \n";
				yield break;
			}
			yield return new WaitForSeconds(5);
			msg += "five sec passed!... \n";

			count--;
			if (count==0){
				msg +="...service is not initilazied on time or not available... \n";
			}
		}
	}

	// Update is called once per frame
	void Update ()	{
		if (bLocationAvailable){
			msg2=Input.location.lastData.latitude.ToString() + "-" +Input.location.lastData.longitude.ToString()  + " accuracy..." + Input.location.lastData.horizontalAccuracy.ToString(); 
			msg3=findPosition(new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude), gpsRefLoc).ToString();
			//msg3=findPosition(new Vector2(38.00667f, 23.79508f), gpsRefLoc).ToString();
		}
	}

	void OnGUI () 	{
			GUI.Label (new Rect (10, 10, 500, 700), msg);
			GUI.Label (new Rect (510, 10, 500, 20), msg2);
			GUI.Label (new Rect (510, 30, 500, 20), msg3);
	}

	Vector2 findPosition(Vector2 gpsCurLoc, Vector2 gpsRefLoc){
		Vector2 pos;
		float posX;
		float posY;

		posX=CalcPosition(gpsCurLoc.y, gpsRefLoc.x);
		posY= CalcPosition(gpsRefLoc.y,gpsCurLoc.x);

		pos=new Vector2(posX,posY);
		return pos;

	}

	float CalcPosition(float lat, float lon){
		int R=6371*1000; //km earth radius
		float aRad=Mathf.PI/180;
		
		float dlat=(lat-gpsRefLoc.y)*aRad;
		float dlon=(lon-gpsRefLoc.x)*aRad;		 
		float a = Mathf.Pow(Mathf.Sin(dlat/2), 2) + Mathf.Cos(lat *aRad) * Mathf.Cos(gpsRefLoc.y * aRad) * Mathf.Pow(Mathf.Sin(dlon / 2), 2);
		float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));	
		return R * c;
	}
		
	//calculates distance from point (lat - lon) to refPoint


	


	

}
