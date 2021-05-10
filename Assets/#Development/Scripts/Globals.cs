using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;
using System;

public class Globals : Singleton<Globals> {
	
	//public static Vector2 gpsRefLoc;	
	public XmlDocument menuXML;
	public XmlDocument scenesXml;
	public XmlDocument settingsXml;
	public XmlDocument movementXml;
	public XmlDocument soundsXml;
	
	protected Globals() { }

	string[] savedmenuXML, savedsettingsXml, savedmovementXml, savedsoundsXml;
	
	public void Init() {

		//LoadXml("menu", menuXML, savedmenuXML);

		savedmenuXML = PlayerPrefsX.GetStringArray("fromServer_"+"menu");
		
		if(savedmenuXML.Length<=0)
		{
			//load terms and help
			menuXML=new XmlDocument();
			TextAsset textAsset = (TextAsset) Resources.Load("XML/"+"menu");	
			string  localExcludedComments = Regex.Replace(textAsset.text, "(<!--(.*?)-->)", string.Empty);
			menuXML.LoadXml(localExcludedComments);
		}else{
			//check local saved xml version
			menuXML=new XmlDocument();
			string textAsset = string.Empty;
			
			for(int x=0; x<savedmenuXML.Length; x++){
				textAsset+=savedmenuXML[x];
			}
			
			string  localExcludedComments = Regex.Replace(textAsset, "(<!--(.*?)-->)", string.Empty);
			menuXML.LoadXml(localExcludedComments);
		}

		//LoadXml("settings", settingsXml, savedsettingsXml);

		savedsettingsXml = PlayerPrefsX.GetStringArray("fromServer_"+"settings");
		
		if(savedsettingsXml.Length<=0)
		{
			//load terms and help
			settingsXml=new XmlDocument();
			TextAsset textAsset = (TextAsset) Resources.Load("XML/"+"settings");	
			string  localExcludedComments = Regex.Replace(textAsset.text, "(<!--(.*?)-->)", string.Empty);
			settingsXml.LoadXml(localExcludedComments);
		}else{
			//check local saved xml version
			settingsXml=new XmlDocument();
			string textAsset = string.Empty;
			
			for(int x=0; x<savedsettingsXml.Length; x++){
				textAsset+=savedsettingsXml[x];
			}
			
			string  localExcludedComments = Regex.Replace(textAsset, "(<!--(.*?)-->)", string.Empty);
			settingsXml.LoadXml(localExcludedComments);
		}

		//LoadXml("movement", movementXml, savedmovementXml);

		savedmovementXml = PlayerPrefsX.GetStringArray("fromServer_"+"movement");
		
		if(savedmovementXml.Length<=0)
		{
			//load terms and help
			movementXml=new XmlDocument();
			TextAsset textAsset = (TextAsset) Resources.Load("XML/"+"movement");	
			string  localExcludedComments = Regex.Replace(textAsset.text, "(<!--(.*?)-->)", string.Empty);
			movementXml.LoadXml(localExcludedComments);
		}else{
			//check local saved xml version
			movementXml=new XmlDocument();
			string textAsset = string.Empty;
			
			for(int x=0; x<savedmovementXml.Length; x++){
				textAsset+=savedmovementXml[x];
			}
			
			string  localExcludedComments = Regex.Replace(textAsset, "(<!--(.*?)-->)", string.Empty);
			movementXml.LoadXml(localExcludedComments);
		}

		//LoadXml("sounds", soundsXml, savedsoundsXml);

		savedsoundsXml = PlayerPrefsX.GetStringArray("fromServer_"+"sounds");
		
		if(savedmovementXml.Length<=0)
		{
			//load terms and help
			soundsXml=new XmlDocument();
			TextAsset textAsset = (TextAsset) Resources.Load("XML/"+"sounds");	
			string  localExcludedComments = Regex.Replace(textAsset.text, "(<!--(.*?)-->)", string.Empty);
			soundsXml.LoadXml(localExcludedComments);
		}else{
			//check local saved xml version
			soundsXml=new XmlDocument();
			string textAsset = string.Empty;
			
			for(int x=0; x<savedsoundsXml.Length; x++){
				textAsset+=savedsoundsXml[x];
			}
			
			string  localExcludedComments = Regex.Replace(textAsset, "(<!--(.*?)-->)", string.Empty);
			soundsXml.LoadXml(localExcludedComments);
		}

		scenesXml = new XmlDocument();
		
		TextAsset textAsset7 = (TextAsset) Resources.Load("XML/scenes");
		string excludedComments7 = Regex.Replace(textAsset7.text, "(<!--(.*?)-->)", string.Empty);
		scenesXml.LoadXml(excludedComments7);

		#region DEPRECATED
		/*
		//load data xml files
		menuXML = new XmlDocument();
		
		TextAsset textAsset = (TextAsset) Resources.Load("XML/menu");
		string excludedComments = Regex.Replace(textAsset.text, "(<!--(.*?)-->)", string.Empty);
		menuXML.LoadXml(excludedComments);

		scenesXml = new XmlDocument();
		
		TextAsset textAsset2 = (TextAsset) Resources.Load("XML/scenes");
		string excludedComments2 = Regex.Replace(textAsset2.text, "(<!--(.*?)-->)", string.Empty);
		scenesXml.LoadXml(excludedComments2);

		settingsXml = new XmlDocument();
		
		TextAsset textAsset3 = (TextAsset) Resources.Load("XML/settings");
		string excludedComments3 = Regex.Replace(textAsset3.text, "(<!--(.*?)-->)", string.Empty);
		settingsXml.LoadXml(excludedComments3);

		movementXml = new XmlDocument();
		
		TextAsset textAsset4 = (TextAsset) Resources.Load("XML/movement");
		string excludedComments4 = Regex.Replace(textAsset4.text, "(<!--(.*?)-->)", string.Empty);
		movementXml.LoadXml(excludedComments4);

		soundsXml = new XmlDocument();
		
		TextAsset textAsset5 = (TextAsset) Resources.Load("XML/sounds");
		string excludedComments5 = Regex.Replace(textAsset5.text, "(<!--(.*?)-->)", string.Empty);
		soundsXml.LoadXml(excludedComments5);
		*/
		#endregion
	}

	void LoadXml(string xmlName, XmlDocument xmlDoc, string[] savedArray){
		savedArray = PlayerPrefsX.GetStringArray("fromServer_"+xmlName);
		
		if(savedArray.Length<=0)
		{
			//load terms and help
			xmlDoc=new XmlDocument();
			TextAsset textAsset = (TextAsset) Resources.Load("XML/"+xmlName);	
			string  localExcludedComments = Regex.Replace(textAsset.text, "(<!--(.*?)-->)", string.Empty);
			xmlDoc.LoadXml(localExcludedComments);
		}else{
			//check local saved xml version
			xmlDoc=new XmlDocument();
			string textAsset = string.Empty;
			
			for(int x=0; x<savedArray.Length; x++){
				textAsset+=savedArray[x];
			}
			
			string  localExcludedComments = Regex.Replace(textAsset, "(<!--(.*?)-->)", string.Empty);
			xmlDoc.LoadXml(localExcludedComments);
		}
	}

	
//	private bool isNewVersion(string newXmlPath)
//	{
//		string curVersion = "";
//		StreamReader sr1 = new StreamReader(Application.persistentDataPath + "/data.xml");
//		StreamReader sr2 = new StreamReader(Application.persistentDataPath + "/" + newXmlPath);
//		string excludedComments1 = Regex.Replace(sr1.ReadToEnd(), "(<!--(.*?)-->)", string.Empty);
//		string excludedComments2 = Regex.Replace(sr2.ReadToEnd(), "(<!--(.*?)-->)", string.Empty);
//		sr1.Close();
//		sr2.Close();
//		
//		XmlDocument dataXmlTmp = new XmlDocument();
//		dataXmlTmp.LoadXml(excludedComments1);
//		
//		XmlNode version = dataXmlTmp.SelectSingleNode("/chronomichani/version");
//		if (version.Attributes ["val"] != null) {
//			curVersion = version.Attributes ["val"].Value;
//		}
//		
//		dataXmlTmp.LoadXml(excludedComments2);		
//		version = dataXmlTmp.SelectSingleNode("/chronomichani/version");
//		
//		if (version.Attributes ["val"] != null) {
//			if (curVersion.Equals(version.Attributes ["val"].Value))
//			{
//				File.Delete(Application.persistentDataPath + "/" + newXmlPath);
//				Debug.Log ("The file was not downloaded. Same version on the server.");
//				//lang = version.Attributes ["val"].Value;
//				return false;
//			}
//			else
//			{
//				File.Replace(Application.persistentDataPath + "/" + newXmlPath, Application.persistentDataPath + "/data.xml",
//				             Application.persistentDataPath + "/data"+ DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "") + ".xml");
//				Debug.Log ("File replaced with the new one.");
//				//lang = version.Attributes ["val"].Value;
//				return true;
//			}
//		}
//		
//		return false;
//	}
	

	
}
