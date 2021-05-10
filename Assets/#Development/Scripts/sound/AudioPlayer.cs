using UnityEngine;
using System.Collections;

public class AudioPlayer: Singleton<AudioPlayer> {
	private AudioSource audioSrc1;
	private AudioSource audioSrc2;
	
	private AudioPlayer(){}
	
	public void Play(string clip){
		
		audioSrc1=this.gameObject.AddComponent<AudioSource>();
		AudioClip ac= Resources.Load(clip) as AudioClip;
		audioSrc1.clip=ac;
		audioSrc1.loop=true;
		audioSrc1.Play();
	}
	
	public void PlayAlso(string clip){
		audioSrc2=this.gameObject.AddComponent<AudioSource>();
		AudioClip ac= Resources.Load(clip) as AudioClip;
		audioSrc2.clip=ac;
		audioSrc2.loop=true;
		audioSrc2.Play();
	}
	
	public void FadeOutMusic()	{
		StartCoroutine(FadeMusic());
	}
	IEnumerator FadeMusic()	{
		while(audioSrc1.volume > 0.1F)	{
			audioSrc1.volume = Mathf.Lerp(audioSrc1.volume,0F,Time.deltaTime);
			yield return 0;
		}
		audioSrc1.volume = 0;
		audioSrc1.Stop();
		audioSrc1.volume=1f;
		//perfect opportunity to insert an on complete hook here before the coroutine exits.
		while(audioSrc2.volume > 0.1F)	{
			audioSrc2.volume = Mathf.Lerp(audioSrc2.volume,0F,Time.deltaTime);
			yield return 0;
		}
		audioSrc2.volume = 0;
		audioSrc2.Stop();
		audioSrc2.volume=1f;
		
	}
	
	
}
