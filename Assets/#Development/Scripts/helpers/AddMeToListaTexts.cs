using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AddMeToListaTexts : MonoBehaviour {

	private Text myText;

	void OnEnable()
	{
		if(myText==null){myText=GetComponent<Text>();}

		if(Diadrasis.Instance.olaTaKeimena_Settings.Count>0 && myText)
		{
			if(!Diadrasis.Instance.olaTaKeimena_Settings.Contains(myText))
			{
				Diadrasis.Instance.olaTaKeimena_Settings.Add(myText);
			}
		}

		ReplaceText();
	}

	void ReplaceText(){
		//find text from terms xml with the name of transform if exists
		if(myText)
		{
			if(appData.FindTerm_text(myText.transform.name)!="unknown"){
				myText.text = appData.FindTerm_text(myText.transform.name);
			}
		}
	}

	void OnDisable()
	{
//		if(myText==null){myText=GetComponent<Text>();}
//		
//		if(Diadrasis.Instance.olaTaLabelKeimena.Count>0 && myText)
//		{
//			if(Diadrasis.Instance.olaTaLabelKeimena.Contains(myText))
//			{
//				Diadrasis.Instance.olaTaLabelKeimena.Remove(myText);
//			}
//		}
	}
}
