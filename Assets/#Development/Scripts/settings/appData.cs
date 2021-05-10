using UnityEngine;
using System.Collections;
using System.Xml;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;
using eChrono;

public class appData {
	public static XmlDocument termXml, couponXml;

	public static Dictionary<string, cSceneArea> mySceneAreas;
	
	public static Dictionary<string, cPoi> myPoints= new Dictionary<string, cPoi>();

	
	public static List<cTerm> textTerms=new List<cTerm>();
	public static List<cCoupon> couponCodes=new List<cCoupon>();
	
	public static List<Transform> myTourPoints=new List<Transform>();

	static string[] savedTermsXml, savedCouponsXml;// = PlayerPrefsX.GetStringArray("fromServer"+xml);
	
	public static void Init(){

		savedTermsXml = PlayerPrefsX.GetStringArray("fromServer_terms");

		if(savedTermsXml.Length<=0)
		{
			//load terms and help
			termXml=new XmlDocument();
			TextAsset terms = (TextAsset) Resources.Load("XML/terms");						
			termXml.LoadXml(Regex.Replace(terms.text, "(<!--(.*?)-->)", string.Empty));
		}else{
			//check local saved xml version
			termXml=new XmlDocument();
			string textAsset = string.Empty;
			
			for(int x=0; x<savedTermsXml.Length; x++){
				textAsset+=savedTermsXml[x];
			}
			
			string  localExcludedComments = Regex.Replace(textAsset, "(<!--(.*?)-->)", string.Empty);
			termXml.LoadXml(localExcludedComments);
		}

		ReadTerms();
	}

	public static void InitCoupons(){

		//load terms and help
		couponXml=new XmlDocument();
		TextAsset coupons = (TextAsset) Resources.Load("XML/coupons");						
		couponXml.LoadXml(Regex.Replace(coupons.text, "(<!--(.*?)-->)", string.Empty));

		savedCouponsXml = PlayerPrefsX.GetStringArray("fromServer_coupons");

		if(savedCouponsXml.Length>0)
		{
			//check local saved xml version
			couponXml=new XmlDocument();
			string textAsset = string.Empty;
			
			for(int x=0; x<savedCouponsXml.Length; x++){
				textAsset+=savedCouponsXml[x];
			}
			
			string  localExcludedComments = Regex.Replace(textAsset, "(<!--(.*?)-->)", string.Empty);
			couponXml.LoadXml(localExcludedComments);
		}

		ReadCoupons();
	}

	//read all terms
	public static void ReadCoupons(){

		XmlNode checkBoolean = couponXml.SelectSingleNode("coupons/checkIfCouponHasBeenUsed");

		if (checkBoolean != null) {
			if (!string.IsNullOrEmpty (checkBoolean.InnerText)) {
				if (checkBoolean.InnerText == "1") {
					Shop.Instance.checkIfCouponHasBeenUsed = true;
				} else {
					Shop.Instance.checkIfCouponHasBeenUsed = false;
				}

				#if UNITY_EDITOR
				Debug.LogWarning ("checkIfCouponHasBeenUsed = "+Shop.Instance.checkIfCouponHasBeenUsed);
				#endif
			}
		}

		// Clear previous loaded coupons
		couponCodes.Clear();
		cCoupon myCoupon;
		XmlNodeList couponList = couponXml.GetElementsByTagName("coupon");
		
		foreach (XmlNode term in couponList) {				
			myCoupon = new cCoupon();

			XmlNode myCode = term ["code"];
			if (myCode!=null) {
				myCoupon.Code = term ["code"].InnerText;

				XmlNode myLength = term ["length"];
				if (myLength!=null) {
					myCoupon.Mikos = int.Parse (term ["length"].InnerText);

					XmlNode customItemBuy = term ["customItemBuy"];
					if (customItemBuy != null) {
						if (customItemBuy.Attributes ["status"] != null) { //check if exists
							if (customItemBuy.Attributes ["status"].Value == "1") {
								if (!string.IsNullOrEmpty (customItemBuy.InnerText)) {
									myCoupon.CustomItem = customItemBuy.InnerText;
								} else {
									myCoupon.CustomItem = string.Empty;
								}
							}else {
								myCoupon.CustomItem = string.Empty;
							}
						}
					}

					#if UNITY_EDITOR
					Debug.LogWarning (myCoupon.Code);
					Debug.LogWarning (myCoupon.Mikos);
					Debug.LogWarning (customItemBuy.InnerText);
					Debug.LogWarning (myCoupon.CustomItem);
					#endif

					couponCodes.Add (myCoupon);

				}

			}
		}
	}

	public static bool isCouponCorrect(string val){
		for (int i=0; i<couponCodes.Count;i++){
			//get numbers length
			int xLength = val.Length - couponCodes [i].Mikos;
			//get first 3 letters
			string letters = val.Substring (0, xLength);
			//if letters exist
			if (couponCodes[i].Code==letters){

				//TODO
				//check if code has been used
				//apo tin lista --> Shop.Instance._shopManager.couponUsed
				if (Shop.Instance.checkIfCouponHasBeenUsed) {
					if (Shop.Instance._shopManager.couponUsed.Count > 0) {
						if (Shop.Instance._shopManager.couponUsed.Contains (val)) {
							//Debug.LogWarning ("COUPON EXIST!!");
							Shop.Instance.customItemBuy = string.Empty;
							return false;
						}
					}
				}

				Shop.Instance.customItemBuy = couponCodes [i].CustomItem;
				return true;
			} 
		}	
		Shop.Instance.customItemBuy = string.Empty;
		return false;
	}
	
	//read all terms
	public static void ReadTerms(){
		// Clear previous loaded Terms
		textTerms.Clear();
		cTerm myTerm;
		XmlNodeList termList = termXml.GetElementsByTagName("term");

//		Debug.Log(appSettings.language);

		foreach (XmlNode term in termList) {				
			myTerm = new cTerm();
			myTerm.Name = term ["name"].InnerText;
			myTerm.Text = term ["value"] [appSettings.language].InnerText;
//					myTerm.icon = term ["image"][appSettings.language].InnerText;
			textTerms.Add(myTerm);

//			#if UNITY_EDITOR
//			Debug.LogWarning(myTerm.Name);
//			#endif
		}

		#if UNITY_EDITOR
		Debug.LogWarning("terms are "+textTerms.Count.ToString());
		#endif
	}
	
	public static string FindTerm_text(string name){
		string term="unknown";
		for (int i=0; i<textTerms.Count;i++){
			if (textTerms[i].Name==name){
				term=textTerms[i].Text;
				return term;
			} 
		}	
		return term;
	}

	
}
