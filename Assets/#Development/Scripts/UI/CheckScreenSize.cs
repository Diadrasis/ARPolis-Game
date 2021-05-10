using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CheckScreenSize : MonoBehaviour {

	public bool isMapButtonHelp=false;
	public bool needToMove=false;
	public RectTransform rectToMove;
	public Vector3 movePosTablet, movePosPhone, moveMapOpen;

	public bool isHorizContainer=false;
	public LayoutElement[] childLayouts;
	public float tabletSize=35f, phoneSize=70f;

	public Sprite sprBig, sprSmall;
	public bool delay;
	Image myImage;
	public HorizontalLayoutGroup hrlayout;

	void Start(){
		Init ();
	}

	void OnEnable(){
		Init ();
	}

	void Init(){

		if (isMapButtonHelp) {
			if (Diadrasis.Instance.menuUI.xartis.mapStatus == Xartis.MapStatus.Close) {
				if (Diadrasis.Instance.screenSize >1) {
					rectToMove.anchoredPosition = movePosTablet;
				} else {
					rectToMove.anchoredPosition = movePosPhone;
				}
			} else {
				rectToMove.anchoredPosition = moveMapOpen;
			}
		}

		if (needToMove && rectToMove!=null) {
			if (Diadrasis.Instance.screenSize >1) {
				rectToMove.anchoredPosition = movePosTablet;
			} else {
				rectToMove.anchoredPosition = movePosPhone;
			}
		}

//		if (isHorizContainer) {
			if (childLayouts.Length > 0) {
				foreach (LayoutElement elem in childLayouts) {
					if (Diadrasis.Instance.screenSize > 1) {
						elem.minWidth = tabletSize;
						elem.minHeight = tabletSize;
					} else {
						elem.minWidth = phoneSize;
						elem.minHeight = phoneSize;
					}
				}
			}
//		} 
			else {

			if (sprSmall && sprBig) {
				myImage = gameObject.GetComponent<Image> ();
				if (!delay && myImage) {
					//if tablet
					if (Diadrasis.Instance.screenSize > 1) {
						myImage.sprite = sprSmall;
					} else {
						myImage.sprite = sprBig;
					}
				} else if (delay && myImage) {
					StartCoroutine (vv ());
				}
			}
		}
	}

	IEnumerator vv(){
		yield return new WaitForSeconds (3f);

		if(delay && myImage){
			//if tablet
			if(Diadrasis.Instance.screenSize > 1){
				myImage.sprite = sprSmall;
			}else{
				myImage.sprite = sprBig;
			}
		}
		yield break;
	}
}
