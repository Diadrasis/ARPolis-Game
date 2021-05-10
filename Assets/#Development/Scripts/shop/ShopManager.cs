using UnityEngine;
using System.Collections;
using Sdkbox;
using System.Collections.Generic;
using System.Linq;

public class ShopManager : MonoBehaviour{

	private Sdkbox.IAP _iap;
	
	public string athensTimeWalkPro="Pro";//Athens Time-Walk Pro
	public string athensTimeWalkPro_Affiliate="Pro Affiliate";
	public string priceOfProEdition=string.Empty;
	public string priceOfProAffiliateEdition=string.Empty;
	bool hasInit = false;

	public List<string> couponUsed = new List<string>();

	public string currentCoupon;

	public void RemoveCurrentCoupon(){
		couponUsed.Remove(currentCoupon);
		PlayerPrefsX.SetStringArray("couponUsed",couponUsed.ToArray());
		PlayerPrefs.Save();
	}

	public void SaveCoupon(){
		couponUsed.Add(currentCoupon);

		PlayerPrefsX.SetStringArray("couponUsed",couponUsed.ToArray());
		PlayerPrefs.Save ();

		if (!string.IsNullOrEmpty (PlayerPrefs.GetString ("myCoupon"))) {
			StartCoroutine (sendCoupon ());
		}
	}

	IEnumerator sendCoupon(){
		string couponNumb = PlayerPrefs.GetString("myCoupon");

		if (!string.IsNullOrEmpty (couponNumb)) {
		
			string url = "http://www.e-chronomichani.gr/timewalk/coupons/recCoupon.php?txtcontent="; //"http://www.e-chronomichani.gr/TimeWalk/sxolia/recSxolia.php?txtcontent="; //serverPHP+phpSxolia+"txtcontent="; //"http://www.stagegames.eu/GAMES/gnothi/recSxolia.php?txtcontent=";
		
			#if UNITY_EDITOR
			url = "http://www.e-chronomichani.gr/timewalk/coupons/testCoupon.php?txtcontent=";
			#endif

			string finalCoupon = "\nApp Version : " + Diadrasis.Instance.appVersion + "\nApply date " + System.DateTime.Now.ToString () + "\n*Model = " + SystemInfo.deviceModel + "\n*OS = " + SystemInfo.operatingSystem + "\n*Processor Count = " + SystemInfo.processorCount + "\nCOUPON:\n{" + couponNumb + "}";
		
			finalCoupon = WWW.EscapeURL (finalCoupon);
		
			WWW www = new WWW (url + finalCoupon);
		
			yield return www;
		
			yield return new WaitForSeconds (0.25f);

			currentCoupon = string.Empty;

		}
		
		yield break;
	}

	public bool isCouponUsed(string val){
		for(int i=0; i<couponUsed.Count; i++){
			if(val==couponUsed[i]){
				return true;
			}
		}
		return false;
	}
	
	// Use this for initialization
	void Start()
	{
		if(!hasInit)
		{
			_iap = FindObjectOfType<Sdkbox.IAP>();

			couponUsed = PlayerPrefsX.GetStringArray("couponUsed").ToList();

			if (_iap == null)
			{
				//#if UNITY_EDITOR 
				Debug.Log("Failed to find IAP instance");
				//#endif
				return;
			}

			_iap.transform.SetParent(transform);

//			_iap.callbacks.onInitialized.AddListener(onInitialized(_iap.));
			//_iap.callbacks.onFailure.AddListener(onf());


			hasInit=true;
		}
	}

	
	public void getProducts()
	{
		if (_iap != null)
		{
			#if UNITY_EDITOR 
			Debug.Log("About to getProducts, will trigger onProductRequestSuccess event");
			#endif
			_iap.getProducts ();
		}
	}
	
	public void Purchase(string item)
	{
		if (_iap != null)
		{
			#if UNITY_EDITOR 
			Debug.Log("About to purchase " + item);
			StartCoroutine(sendCoupon());
			#endif
			_iap.purchase(item);
		}
	}
	
	public void Refresh()
	{
		if (_iap != null)
		{
			#if UNITY_EDITOR
			Debug.Log("About to refresh");
			#endif
			_iap.refresh();
		}
	}
	
	public void Restore()
	{
		if (_iap != null)
		{
			#if UNITY_EDITOR
			Debug.Log("About to restore");
			#endif
			_iap.restore();
		}
	}
	
	//
	// Event Handlers
	//
	
	public void onInitialized(bool status)
	{
		#if UNITY_EDITOR
		Debug.Log("PurchaseHandler.onInitialized " + status);
		#endif
	}
	
	public void onSuccess(Product product)
	{
		#if UNITY_EDITOR
		Debug.Log("PurchaseHandler.onSuccess: " + product.name);
		#endif

		if(!string.IsNullOrEmpty(PlayerPrefs.GetString("myCoupon"))){
			SaveCoupon();
		}

		Shop.Instance.customItemBuy = string.Empty;
	}
	
	public void onFailure(Product product, string message)
	{
		#if UNITY_EDITOR
		Debug.Log("PurchaseHandler.onFailure " + message);
		#endif
		

		Shop.Instance.isCouponValid=false;
		currentCoupon=string.Empty;
	}
	
	public void onCanceled(Product product)
	{
		#if UNITY_EDITOR
		Debug.Log("PurchaseHandler.onCanceled product: " + product.name);
		#endif

		Shop.Instance.isCouponValid=false;
		currentCoupon=string.Empty;
	}
	
	public void onRestored(Product product)
	{
		#if UNITY_EDITOR
		Debug.Log("onRestored: " + product.name +" receipt = "+product.receipt+ " title = "+ product.title);
		#endif

		//user has bought product
		if(product.name==athensTimeWalkPro || product.name==athensTimeWalkPro_Affiliate){
			Diadrasis.Instance.menuUI.warningsUI.HideShopButton();
			PlayerPrefs.SetInt("useOnSiteMode",111);
			PlayerPrefs.Save();
			Diadrasis.Instance.useOnSiteMode=true;
			Diadrasis.Instance.showPoiInfo=true;
			Diadrasis.Instance.menuUI.warningsUI.BuyCancel(true);
			return;
		}

	}
	
	public void onProductRequestSuccess(Product[] products)
	{
		foreach (var p in products)
		{
			//me ekptosi
			if(p.name==athensTimeWalkPro_Affiliate){
				priceOfProAffiliateEdition = p.price;
			}
			else//xoris ekptosi
			if(p.name==athensTimeWalkPro){
				priceOfProEdition = p.price;
			}
		}
	}
	
	public void onProductRequestFailure(string message)
	{
		#if UNITY_EDITOR
		Debug.Log("PurchaseHandler.onProductRequestFailure: " + message);
		#endif
	}
	
	public void onRestoreComplete(string message)
	{
		#if UNITY_EDITOR
		Debug.Log("PurchaseHandler.onRestoreComplete: " + message);
		#endif

	}
}
