using UnityEngine;
using System.Collections;

public class Shop : Singleton<Shop> {

	private Shop(){}

	public ShopManager _shopManager;

	public bool isProEnabled=false;
	public bool isCouponValid=false;
	public string myCoupon;
	public bool isFromSceneAction = false;
	public string customItemBuy;

	public bool checkIfCouponHasBeenUsed=false;

	public void Init(){
		if(!_shopManager){
			_shopManager = FindObjectOfType<ShopManager>();
//			_shopManager.transform.SetParent(this.transform);

			myCoupon = PlayerPrefs.GetString("myCoupon");
		}
	}
}
