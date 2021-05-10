using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using System.Collections;
using System.Collections.Generic;

public class cilcleItems : MonoBehaviour {

	public List<UICircle> kykloi = new List<UICircle>();
	public Transform targetList;
	public Scrollbar skrolBar;

	public ScrollSnap snap;

	public Color myColorKyklos;

	int synoloKyklwn=0;
//	int barValue=0;

//	IEnumerator Start () {
//
//		yield return new WaitForSeconds(1f);
//
//		synoloKyklwn=snap.pages;
//		CreateKyklous();
//
//		Debug.Log("pages are "+snap.pages);
//	}

	public void Init()
	{
		synoloKyklwn=snap.pages;
		CreateKyklous();
		
		//Debug.Log("kykloi are "+kykloi.Count);
	}
	
	void Update () {

		if(kykloi.Count>0)
		{

			foreach(UICircle cr in kykloi)
			{
				if(cr)
				{
					cr.color=Color.white;

					//Debug.Log("page is "+snap.CurrentPage());
					//Debug.Log("my Index is "+kykloi.IndexOf(cr));

					if(kykloi.IndexOf(cr)==snap.CurrentPage())
					{
						//Debug.Log("In Page");
						if(cr.color!=myColorKyklos)
						{
							cr.color=myColorKyklos;
						}
					}
				}
			}
		}
	
	}

	void DestroyOldKyklous()
	{
		foreach(Transform tr in transform)
		{
			if(tr.name!=transform.name)
			{
				Destroy(tr.gameObject);
			}
		}
	}

	void MetraPosousKyklous()
	{
		synoloKyklwn=0;

		foreach(Transform tr in targetList)
		{
			if(tr.name!=targetList.name)
			{
				synoloKyklwn++;
			}
		}
	}

	public void CreateKyklous()
	{
		DestroyOldKyklous();
//		MetraPosousKyklous();

		if(synoloKyklwn>0)
		{
			kykloi.Clear();

			for(int i=0; i<synoloKyklwn; i++)
			{
				//instatiate new child item
				GameObject kyklos = Instantiate(Resources.Load("prefabs/FINAL/ui/circleItem")) as GameObject;
				kyklos.name = "kyklos_"+i.ToString();
				//add child to container
				kyklos.transform.SetParent(transform);
				kyklos.transform.localScale = new Vector3(1f,1f,1f);
				//add child to list transform
				UICircle cc = kyklos.transform.GetChild(0).GetComponent<UICircle>();
				if(cc==null){Debug.Log("No Script Circle !!!");}
				cc.myIndex=i;
				kykloi.Add(cc);
			}
		}

		synoloKyklwn=0;
	}
}
