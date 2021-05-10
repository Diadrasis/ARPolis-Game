using UnityEngine;
using System.Collections;

public class Internet : Singleton<Internet> {

	protected Internet(){}

	public void Init(){/*empty just to instatiate*/}

	public bool isOnline()
	{
		return false; //plugin.CheckInternet();
	}
}
