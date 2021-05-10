using UnityEngine;
using System.Collections;

public class Sound : MonoBehaviour {

	public string file;
	
	public Vector3 pos;
	
	public float volume;
	
	public Vector2 distanceToHear;
	
	public bool loop;
	
	public float repeatTime;
	
	private AudioSource ad;

	public AudioClip myClip;

	public float triggerDistance;

	float distToPerson=0;
	bool finishedInit;

	public void Init(){
		Transform tr = this.transform;
		
		if(tr && !string.IsNullOrEmpty(file)){
			tr.name = file;
			tr.position = pos;
			
			ad = tr.gameObject.AddComponent<AudioSource>();
			
			ad.playOnAwake=false;
			
			ad.clip = myClip;
			
			ad.loop = loop;
			
			ad.minDistance = distanceToHear.x;
			ad.maxDistance = distanceToHear.y;

			ad.volume = volume;
			
			ad.Play();
			
			Diadrasis.Instance.allSounds.Add(ad);

			if(!loop){
				StartCoroutine(repeatPlay());
			}

			finishedInit=true;
			
		}else{
			#if UNITY_EDITOR
			Debug.LogWarning("Transform "+file+" is NULL");
			#endif
		}
	}

	void LateUpdate(){
		if(triggerDistance>0 && finishedInit && Diadrasis.Instance.person){
			distToPerson = Vector2.Distance(new Vector2(pos.x,pos.z), new Vector2(Diadrasis.Instance.person.transform.position.x, Diadrasis.Instance.person.transform.position.z));

//			#if UNITY_EDITOR
//			Debug.LogWarning(file+" has dist from person "+distToPerson+" m");
//			#endif

			if(distToPerson<triggerDistance){
				PlayClip();
			}else{
				StopClip();
			}
		}
	}
	
	private IEnumerator repeatPlay(){
		while(myClip!=null)
		{
			if(repeatTime>0){
				if(!ad.isPlaying){

//					#if UNITY_EDITOR
//					Debug.LogWarning("waiting for "+file+" to finished playing");
//					#endif

					yield return new WaitForSeconds(repeatTime);
					PlayClip();
				}
			}

			yield return new WaitForSeconds(0);
		}
	}

	public void PlayClip(){
		if(!ad.isPlaying || isPaused){
			isPaused=false;

			ad.Play();

			ad.volume = volume;
		}
	}

	public void StopClip(){
		if(ad.isPlaying){
			isPaused=false;
			ad.volume = 0f;
			ad.Stop();
		}
	}

	bool isPaused;

	public void PauseClip(){
		isPaused=true;
		ad.volume = 0f;
		ad.Pause();
	}

}
