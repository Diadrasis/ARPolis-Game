using UnityEngine;
using System.Collections;
using Sdkbox;
using UnityEngine.UI;

public class PurchaseHandler : MonoBehaviour
{
	private Sdkbox.IAP _iap;

	public Text txtInfo;
	public GameObject btnAkropolis, btnOnSite;
	public string checkAkropolis, checkOnsite;

	// Use this for initialization
	void Start()
	{
		_iap = FindObjectOfType<Sdkbox.IAP>();
		if (_iap == null)
		{
			txtInfo.text = "Failed to find IAP instance";
		}

//		yield return new WaitForSeconds(5f);
//
//		Restore();
	}

	public void getProducts()
	{
		if (_iap != null)
		{
			txtInfo.text = "About to getProducts, will trigger onProductRequestSuccess event";
			_iap.getProducts ();
		}
	}

	public void Purchase(string item)
	{
		if (_iap != null)
		{
			txtInfo.text = "About to purchase " + item;
			_iap.purchase(item);
		}
	}

	public void Refresh()
	{
		if (_iap != null)
		{
			txtInfo.text = "About to refresh";
			_iap.refresh();
		}
	}

	public void Restore()
	{
		if (_iap != null)
		{
			txtInfo.text = "About to restore";
			_iap.restore();
		}
	}

	//
	// Event Handlers
	//

	public void onInitialized(bool status)
	{
		txtInfo.text = "PurchaseHandler.onInitialized " + status;
	}

	public void onSuccess(Product product)
	{
		txtInfo.text = "PurchaseHandler.onSuccess: " + product.name;
	}

	public void onFailure(Product product, string message)
	{
		txtInfo.text = "PurchaseHandler.onFailure " + message;
		Camera.main.backgroundColor = Color.red;
	}

	public void onCanceled(Product product)
	{
		txtInfo.text = "PurchaseHandler.onCanceled product: " + product.name;
		Camera.main.backgroundColor = Color.magenta;
	}

	public void onRestored(Product product)
	{
		txtInfo.text = "onRestored: " + product.name +" receipt = "+product.receipt+ " title = "+ product.title;
		if(product.name==checkOnsite){
			btnOnSite.SetActive(false);
			Camera.main.backgroundColor = Color.gray;
		}else
		if(product.name==checkAkropolis){
			btnAkropolis.SetActive(false);
		}
	}

	public void onProductRequestSuccess(Product[] products)
	{
		foreach (var p in products)
		{
			txtInfo.text += "Product: " + p.name + " price: " + p.price;
		}
		Camera.main.backgroundColor = Color.black;
	}

	public void onProductRequestFailure(string message)
	{
		txtInfo.text = "PurchaseHandler.onProductRequestFailure: " + message;
		Camera.main.backgroundColor = Color.magenta;
	}

	public void onRestoreComplete(string message)
	{
		txtInfo.text = "PurchaseHandler.onRestoreComplete: " + message;
		Camera.main.backgroundColor = Color.green;
	}
}
