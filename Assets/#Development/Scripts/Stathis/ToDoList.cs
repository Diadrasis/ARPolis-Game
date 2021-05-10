using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ToDoList : Singleton<ToDoList> {
	
	protected ToDoList(){}

	public string[] lista;

	[Tooltip ("SAVE CURRENT LIST")]
	public bool saveLista;

	[Tooltip ("HIDE LISTA - YOU CAN USE 'L' IN KEYBOARD")]
	public bool hideLista;

	Text txt;
	GameObject listText;

	public void SaveList(){
		PlayerPrefsX.SetStringArray("lista",lista);
		ReplaceText();
		saveLista=false;
	}

	public void Init()
	{
		#if UNITY_EDITOR

		lista = PlayerPrefsX.GetStringArray("lista");

		Transform kanvas = Diadrasis.Instance.canvasMainRT.transform;

		listText = Instantiate(Resources.Load("prefabs/FINAL/ui/Poi/titlosPoi")) as GameObject;
		listText.transform.SetParent(kanvas);
		listText.transform.SetAsLastSibling();

		listText.transform.localPosition = Vector3.zero;

		RectTransform rt = listText.GetComponent<RectTransform>();

		Stathis.Tools_UI.Move(rt, Stathis.Tools_UI.Mode.up);

		txt = listText.GetComponentInChildren<Text>();

		ReplaceText();

//		rt.sizeDelta = Diadrasis.Instance.canvasMainRT.sizeDelta;

		StartCoroutine(_update());

		#endif
	}

	void ReplaceText(){
		if(lista.Length>0){
			txt.text=string.Empty;
			for(int i=0; i<lista.Length; i++){
				txt.text += lista[i] + "\n";
			}
		}
	}
	
	IEnumerator _update(){
		while(true){
			if(saveLista){
				SaveList();
				yield return new WaitForSeconds(1f);
			}

			if(Input.GetKeyDown(KeyCode.L)){
				hideLista = !hideLista;
			}

			if(hideLista){
				listText.SetActive(false);
			}else
			if(!hideLista){
				listText.SetActive(true);
			}

			yield return null;
		}
	}
}
