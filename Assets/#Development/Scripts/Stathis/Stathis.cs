using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

//tools for less code and time
//@2016 Stathis Georgiou
using eChrono;

namespace Stathis
{
	public class Methods : MonoBehaviour
	{
		///calculate physical inches with pythagoras theorem
		public static float DeviceDiagonalSizeInInches ()
		{
			float screenWidth = Screen.width / Screen.dpi;
			float screenHeight = Screen.height / Screen.dpi;
			float diagonalInches = Mathf.Sqrt (Mathf.Pow (screenWidth, 2) + Mathf.Pow (screenHeight, 2));
			
			#if UNITY_EDITOR
			Debug.Log ("Getting device inches: " + diagonalInches);
			#endif
			
			return diagonalInches;
		}


	}


	public class Tools_Load : MonoBehaviour 
	{

		/// <summary>
		/// Loads the sprite from resources folder
		/// folder = select the folder conatining sprite
		/// file = name of image sprite
		/// removeFileExtencion = if true file name must be with extencion (etc. .png or .jpg)
		/// </summary>
		public static Sprite LoadSpriteFromResources (string folder, string file, bool removeFileExtencion)
		{
			if(removeFileExtencion){
				if (file.Contains (".")) {
					file = file.Substring (0, file.Length - 4); //truncate the file extension
				} else {
					Debug.LogWarning ("file name does not have an extencion");
				}
			}

			Sprite spr = Resources.Load <Sprite> (folder + "/" + file);

			if (!spr) {
				Debug.LogWarning ("Sprite Not Found in "+folder + "/" + file);
				return null;
			}

			return  spr;
		}

		public static Sprite LoadSpriteFromResources (string folder, string file)
		{
			Sprite spr = Resources.Load <Sprite> (folder + "/" + file);

			if (!spr) {
				Debug.LogWarning ("Sprite Not Found in "+folder + "/" + file);
				return null;
			}

			return  spr;
		}

		public static Sprite LoadSpriteFromResources (string file)
		{
			Sprite spr = Resources.Load <Sprite> (file);

			if (!spr) {
				Debug.LogWarning ("Sprite Not Found in Resources/" + file);
				return null;
			}

			return  spr;
		}

		public static Texture2D LoadTexture(string folder, string t){
			return (Texture2D) Resources.Load (folder + "/" + t) as Texture2D;
		}

//		public static Texture LoadTexture(string file){
//			return (Texture) (file) as Texture;
//		}


		///universal loading
		/// usage -> Tools_Load.LoadElement<Texture2D>(string a, string b);
//		public static T LoadElement<T>(string folder, string file)
//		{
//
//			T element = new <T>();
//
//			element =Resources.Load <T> (folder + "/" + file);
//			
//			if (!element) {
//				Debug.LogWarning ("Element Not Found in "+folder + "/" + file);
//				return null;
//			}
//			
//			return  element;
//
//		}

//		public static <T> Load <T>(string t){
//			return (<T>) Resources.Load (t) as <T>;
//		}
	}

	#region UI TOOLS

	public class Tools_UI : MonoBehaviour
	{
		#region check ray on ui
		public static PointerEventData eventDataCurrentPosition;
		
		/// <summary>
		/// Cast a ray to test if Input.mousePosition is over any UI object in EventSystem.current. This is a replacement
		/// for IsPointerOverGameObject() which does not work on Android in 4.6.0f3
		/// </summary>
		public static bool IsPointerOverUIObject() {
			// Referencing this code for GraphicRaycaster https://gist.github.com/stramit/ead7ca1f432f3c0f181f
			// the ray cast appears to require only eventData.position.
			eventDataCurrentPosition = new PointerEventData(EventSystem.current);
			eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
//			Debug.Log("clicks at = "+eventDataCurrentPosition.position);
			
			List<RaycastResult> results = new List<RaycastResult>();
			EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
			//Debug.Log("rays = "+results.Count);

//			#if UNITY_EDITOR
//			if(results.Count>0){
//				Debug.LogWarning("UI = "+results[0].gameObject.name);
//			}else{
//				Debug.LogWarning("UI no results!!!");
//			}
//			#endif

			return results.Count > 0;
		}

		public static bool IsPointerOverUIObject(Vector2 pos) {
			// Referencing this code for GraphicRaycaster https://gist.github.com/stramit/ead7ca1f432f3c0f181f
			// the ray cast appears to require only eventData.position.
			eventDataCurrentPosition = new PointerEventData(EventSystem.current);
			eventDataCurrentPosition.position = pos;
			//Debug.Log("clicks = "+eventDataCurrentPosition.clickCount);
			
			List<RaycastResult> results = new List<RaycastResult>();
			EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
			//Debug.Log("rays = "+results.Count);
			return results.Count > 0;
		}

		public static bool IsPointerOverUIObject(string onoma) {
			// Referencing this code for GraphicRaycaster https://gist.github.com/stramit/ead7ca1f432f3c0f181f
			// the ray cast appears to require only eventData.position.
			eventDataCurrentPosition = new PointerEventData(EventSystem.current);
			eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			//Debug.Log("clicks = "+eventDataCurrentPosition.clickCount);

			if (!eventDataCurrentPosition.selectedObject) {
				return false;
			}
			
			if(eventDataCurrentPosition.selectedObject.name == onoma){
				#if UNITY_EDITOR
				Debug.Log(onoma);
				#endif
				return true;
			}

			return false;

		}

		public static bool IsPointerOverUIObjectTag(string tag) {
			// Referencing this code for GraphicRaycaster https://gist.github.com/stramit/ead7ca1f432f3c0f181f
			// the ray cast appears to require only eventData.position.
			eventDataCurrentPosition = new PointerEventData(EventSystem.current);
			eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			//Debug.Log("clicks = "+eventDataCurrentPosition.clickCount);
			
			if (!eventDataCurrentPosition.selectedObject) {
				return false;
			}
			
			if(eventDataCurrentPosition.selectedObject.tag == tag){
				#if UNITY_EDITOR
				Debug.Log("tag = "+tag);
				#endif
				return true;
			}
			#if UNITY_EDITOR
			Debug.Log("tag ERROR = "+tag);
			#endif
			return false;
			
		}
		#endregion

		#region image - rawimage tools

		/// <summary>
		/// Rescales the image to fit in preffered dimensions
		/// </summary>
		public static void RescaleImage(Image img, Vector2 prefferedSize){
			if (img.sprite == null) {
				Debug.LogWarning ("Image "+img.gameObject.name+" has Null Sprite - Please assign sprite texture to Image component first! ");
				return;
			}
			RectTransform rekt = img.GetComponent<RectTransform> ();
			// diastaseis eikonas
			Vector2 spriteSize = new Vector2 (img.sprite.texture.width, img.sprite.texture.height);
			// metavliti
			float dy = prefferedSize.y / spriteSize.y;
			//rescale
			float Y = spriteSize.y * dy;
			float X = spriteSize.x * dy;

			if (X > prefferedSize.x) {
				// metavliti
				float dx = prefferedSize.x / X;
				//rescale
				Y = Y * dx;
				X = X * dx;
			}

			rekt.sizeDelta = new Vector2(X,Y);
		}

		/// <summary>
		/// Rescales the image to fit in parent dimensions
		/// </summary>
		public static void RescaleImage(Image img){
			if (img.sprite == null) {
				Debug.LogWarning ("Image "+img.gameObject.name+" has Null Sprite - Please assign sprite texture to Image component first! ");
				return;
			}
			RectTransform rektParent = img.transform.parent.GetComponent<RectTransform> ();

			Vector2 prefferedSize = rektParent.sizeDelta;

			RectTransform rekt = img.GetComponent<RectTransform> ();
			// diastaseis eikonas
			Vector2 spriteSize = new Vector2 (img.sprite.texture.width, img.sprite.texture.height);
			// metavliti
			float dy = prefferedSize.y / spriteSize.y;
			//rescale
			float Y = spriteSize.y * dy;
			float X = spriteSize.x * dy;

			if (X > prefferedSize.x) {
				// metavliti
				float dx = prefferedSize.x / X;
				//rescale
				Y = Y * dx;
				X = X * dx;
			}

			rekt.sizeDelta = new Vector2(X,Y);
		}

		/// <summary>
		/// Rescales the image to fit in parent dimensions
		/// If height > width and fitVertical = true , rotates the iamge to fit vertical
		/// </summary>
		public static void RescaleImage(Image img, bool fitVertical){
			if (img.sprite == null) {
				Debug.LogWarning ("Image "+img.gameObject.name+" has Null Sprite - Please assign sprite texture to Image component first! ");
				return;
			}
			RectTransform rektParent = img.transform.parent.GetComponent<RectTransform> ();

			Vector2 prefferedSize = rektParent.sizeDelta;

			if (fitVertical) {
				if (prefferedSize.y >= prefferedSize.x) {
					prefferedSize = new Vector2 (prefferedSize.y, prefferedSize.x);
					img.rectTransform.eulerAngles = new Vector3 (0f,0f,90f);
				}
			}

			RectTransform rekt = img.GetComponent<RectTransform> ();
			// diastaseis eikonas
			Vector2 spriteSize = new Vector2 (img.sprite.texture.width, img.sprite.texture.height);
			// metavliti
			float dy = prefferedSize.y / spriteSize.y;
			//rescale
			float Y = spriteSize.y * dy;
			float X = spriteSize.x * dy;

			if (X > prefferedSize.x) {
				// metavliti
				float dx = prefferedSize.x / X;
				//rescale
				Y = Y * dx;
				X = X * dx;
			}

			rekt.sizeDelta = new Vector2(X,Y);
		}
	

		/// <summary>
		/// Rescales the image to fit in parent dimensions
		/// If height > width and fitVertical = true , rotates the image to fit vertical
		/// rotLeft enables 90 degrees left rotation if true else right
		/// left rotation = left side of image is moving at the bottom of the screen
		/// right rotation = left side of image is moving at the top of the screen
		/// </summary>
		public static void RescaleImage(Image img, bool fitVertical, bool rotLeft){
			if (img.sprite == null) {
				Debug.LogWarning ("Image "+img.gameObject.name+" has Null Sprite - Please assign sprite texture to Image component first! ");
				return;
			}
			RectTransform rektParent = img.transform.parent.GetComponent<RectTransform> ();

			Vector2 prefferedSize = rektParent.sizeDelta;

			if (fitVertical) {
				if (prefferedSize.y >= prefferedSize.x) {
					prefferedSize = new Vector2 (prefferedSize.y, prefferedSize.x);
					if (rotLeft) {
						img.rectTransform.eulerAngles = new Vector3 (0f, 0f, 90f);
					} else {
						img.rectTransform.eulerAngles = new Vector3 (0f, 0f, -90f);
					}
				}
			}

			RectTransform rekt = img.GetComponent<RectTransform> ();
			// diastaseis eikonas
			Vector2 spriteSize = new Vector2 (img.sprite.texture.width, img.sprite.texture.height);
			// metavliti
			float dy = prefferedSize.y / spriteSize.y;
			//rescale
			float Y = spriteSize.y * dy;
			float X = spriteSize.x * dy;

			if (X > prefferedSize.x) {
				// metavliti
				float dx = prefferedSize.x / X;
				//rescale
				Y = Y * dx;
				X = X * dx;
			}

			rekt.sizeDelta = new Vector2(X,Y);
		}

		/// <summary>
		/// Rescales the rawImage to fit in preffered dimensions
		/// </summary>
		public static void RescaleRawImage(RawImage rawImg, Vector2 prefferedSize){
			if (rawImg.texture == null) {
				Debug.LogWarning ("Image "+rawImg.gameObject.name+" has Null Sprite - Please assign sprite texture to Image component first! ");
				return;
			}
			RectTransform rekt = rawImg.GetComponent<RectTransform> ();
			// diastaseis eikonas
			Vector2 spriteSize = new Vector2 (rawImg.texture.width, rawImg.texture.height);
			// metavliti
			float dy = prefferedSize.y / spriteSize.y;
			//rescale
			float Y = spriteSize.y * dy;
			float X = spriteSize.x * dy;

			if (X > prefferedSize.x) {
				// metavliti
				float dx = prefferedSize.x / X;
				//rescale
				Y = Y * dx;
				X = X * dx;
			}

			rekt.sizeDelta = new Vector2(X,Y);
		}

		/// <summary>
		/// Rescales the rawImage to fit in preffered dimensions
		/// </summary>
		public static void RescaleRawImage(RawImage rawImg, Vector2 prefferedSize, bool keepWidth){
			if (rawImg.texture == null) {
				Debug.LogWarning ("Image "+rawImg.gameObject.name+" has Null Sprite - Please assign sprite texture to Image component first! ");
				return;
			}
			RectTransform rekt = rawImg.GetComponent<RectTransform> ();
			// diastaseis eikonas
			Vector2 spriteSize = new Vector2 (rawImg.texture.width, rawImg.texture.height);
			// metavliti
			float dy = prefferedSize.y / spriteSize.y;
			//rescale
			float Y = spriteSize.y * dy;
			float X = spriteSize.x * dy;

			if(!keepWidth){
				if (X > prefferedSize.x) {
					// metavliti
					float dx = prefferedSize.x / X;
					//rescale
					Y = Y * dx;
					X = X * dx;
				}
			}
			
			rekt.sizeDelta = new Vector2(X,Y);
		}

		//RescaleRawImage_keepWidth

		/// <summary>
		/// Rescales the rawImage to fit in preffered dimensions 
		/// Y size only if keepWdth is true
		/// X and Y if keepWidth is false
		/// </summary>
		public static void RescaleRawImage_keepWidth(RawImage rawImg, Vector2 prefferedSize, bool keepWidth){
			if (rawImg.texture == null) {
				Debug.LogWarning ("Image "+rawImg.gameObject.name+" has Null Sprite - Please assign sprite texture to Image component first! ");
				return;
			}
			RectTransform rekt = rawImg.GetComponent<RectTransform> ();
			// diastaseis eikonas
			Vector2 spriteSize = new Vector2 (rawImg.texture.width, rawImg.texture.height);
			// metavliti
			float dy = prefferedSize.y / spriteSize.y;
			//rescale
			float Y = spriteSize.y * dy;
			float X = spriteSize.x * dy;
			
			if(!keepWidth){
				if (X > prefferedSize.x) {
					// metavliti
					float dx = prefferedSize.x / X;
					//rescale
					Y = Y * dx;
					X = X * dx;
				}
			}
			
			rekt.sizeDelta = new Vector2(X,Y);
		}

		/// <summary>
		/// Rescales the rawImage to fit in parent's dimensions
		/// </summary>
		public static void RescaleRawImage(RawImage rawImg){
			if (rawImg.texture == null) {
				Debug.LogWarning ("Image "+rawImg.gameObject.name+" has Null Sprite - Please assign sprite texture to Image component first! ");
				return;
			}
			RectTransform rektParent = rawImg.transform.parent.GetComponent<RectTransform> ();

			Vector2 prefferedSize = rektParent.sizeDelta;

			RectTransform rekt = rawImg.GetComponent<RectTransform> ();
			// diastaseis eikonas
			Vector2 spriteSize = new Vector2 (rawImg.texture.width, rawImg.texture.height);
			// metavliti
			float dy = prefferedSize.y / spriteSize.y;
			//rescale
			float Y = spriteSize.y * dy;
			float X = spriteSize.x * dy;

			if (X > prefferedSize.x) {
				// metavliti
				float dx = prefferedSize.x / X;
				//rescale
				Y = Y * dx;
				X = X * dx;
			}

			rekt.sizeDelta = new Vector2(X,Y);
		}

		/// <summary>
		/// Rescales the rawImage to fit in parent dimensions
		/// If height > width and fitVertical = true , rotates the iamge to fit vertical
		/// </summary>
		public static void RescaleRawImage(RawImage rawImg, bool fitVertical){
			if (rawImg.texture == null) {
				Debug.LogWarning ("Image "+rawImg.gameObject.name+" has Null Sprite - Please assign sprite texture to Image component first! ");
				return;
			}
			RectTransform rektParent = rawImg.transform.parent.GetComponent<RectTransform> ();

			Vector2 prefferedSize = rektParent.sizeDelta;

			if (fitVertical) {
				if (prefferedSize.y >= prefferedSize.x) {
					prefferedSize = new Vector2 (prefferedSize.y, prefferedSize.x);
					rawImg.rectTransform.eulerAngles = new Vector3 (0f,0f,90f);
				}
			}

			RectTransform rekt = rawImg.GetComponent<RectTransform> ();
			// diastaseis eikonas
			Vector2 spriteSize = new Vector2 (rawImg.texture.width, rawImg.texture.height);
			// metavliti
			float dy = prefferedSize.y / spriteSize.y;
			//rescale
			float Y = spriteSize.y * dy;
			float X = spriteSize.x * dy;

			if (X > prefferedSize.x) {
				// metavliti
				float dx = prefferedSize.x / X;
				//rescale
				Y = Y * dx;
				X = X * dx;
			}

			rekt.sizeDelta = new Vector2(X,Y);
		}


		/// <summary>
		/// Rescales the rawImage to fit in parent dimensions
		/// If height > width and fitVertical = true , rotates the image to fit vertical
		/// rotLeft enables 90 degrees left rotation if true else right
		/// left rotation = left side of image is moving at the bottom of the screen
		/// right rotation = left side of image is moving at the top of the screen
		/// </summary>
		public static void RescaleRawImage(RawImage rawImg, bool fitVertical, bool rotLeft){
			if (rawImg.texture == null) {
				Debug.LogWarning ("Image "+rawImg.gameObject.name+" has Null Sprite - Please assign sprite texture to Image component first! ");
				return;
			}
			RectTransform rektParent = rawImg.transform.parent.GetComponent<RectTransform> ();

			Vector2 prefferedSize = rektParent.sizeDelta;

			if (fitVertical) {
				if (prefferedSize.y >= prefferedSize.x) {
					prefferedSize = new Vector2 (prefferedSize.y, prefferedSize.x);
					if (rotLeft) {
						rawImg.rectTransform.eulerAngles = new Vector3 (0f, 0f, 90f);
					} else {
						rawImg.rectTransform.eulerAngles = new Vector3 (0f, 0f, -90f);
					}
				}
			}

			RectTransform rekt = rawImg.GetComponent<RectTransform> ();
			// diastaseis eikonas
			Vector2 spriteSize = new Vector2 (rawImg.texture.width, rawImg.texture.height);
			// metavliti
			float dy = prefferedSize.y / spriteSize.y;
			//rescale
			float Y = spriteSize.y * dy;
			float X = spriteSize.x * dy;

			if (X > prefferedSize.x) {
				// metavliti
				float dx = prefferedSize.x / X;
				//rescale
				Y = Y * dx;
				X = X * dx;
			}

			rekt.sizeDelta = new Vector2(X,Y);
		}

		/// <summary>
		/// Resizes the size of the texture into container rect transform.
		/// Valid for Image and RawImage only!!!
		/// </summary>
		/// <param name="container">Container.</param>
		/// <param name="fitVertical">If set to <c>true</c> fit vertical.</param>
		/// <param name="rotLeft">If set to <c>true</c> rot left.</param>
		public static void ResizeTextureToContainerSize(RectTransform container, bool fitVertical, bool rotLeft){

			bool isSprite = false;

			RawImage rawImg = container.GetComponent<RawImage>();
			Image img = null;

			//if not a raw image try for image
			if(!rawImg){
				img = container.GetComponent<Image>();
				isSprite =true;
			}

			if (rawImg == null && img == null) {
				Debug.LogWarning ("container "+container.gameObject.name+" is not valid Image or RawImage !");
				return;
			}
			
			Vector2 prefferedSize = container.sizeDelta;
			
			if (fitVertical) {
				if (prefferedSize.y >= prefferedSize.x) {
					prefferedSize = new Vector2 (prefferedSize.y, prefferedSize.x);
					if (rotLeft) {
						container.eulerAngles = new Vector3 (0f, 0f, 90f);
					} else {
						container.eulerAngles = new Vector3 (0f, 0f, -90f);
					}
				}
			}
			
//			RectTransform rekt = rawImg.GetComponent<RectTransform> ();

			// diastaseis eikonas
			Vector2 spriteSize = Vector2.zero;

			if (!isSprite) {
				spriteSize = new Vector2 (rawImg.texture.width, rawImg.texture.height);
			} else {
				spriteSize = new Vector2 (img.sprite.texture.width, img.sprite.texture.height);
			}

			// metavliti
			float dy = prefferedSize.y / spriteSize.y;
			//rescale
			float Y = spriteSize.y * dy;
			float X = spriteSize.x * dy;
			
			if (X > prefferedSize.x) {
				// metavliti
				float dx = prefferedSize.x / X;
				//rescale
				Y = Y * dx;
				X = X * dx;
			}
			
			container.sizeDelta = new Vector2(X,Y);
		}

		public static void ResizeTextureToContainerSize(RectTransform container, bool expandWidth){

			bool isSprite = false;

			RawImage rawImg = container.GetComponent<RawImage>();
			Image img = null;

			//if not a raw image try for image
			if(!rawImg){
				img = container.GetComponent<Image>();
				isSprite =true;
			}

			if (rawImg == null && img == null) {
				Debug.LogWarning ("container "+container.gameObject.name+" is not valid Image or RawImage !");
				return;
			}

			Vector2 prefferedSize = container.sizeDelta;

			// diastaseis eikonas
			Vector2 spriteSize = Vector2.zero;

			if (!isSprite) {
				spriteSize = new Vector2 (rawImg.texture.width, rawImg.texture.height);
			} else {
				spriteSize = new Vector2 (img.sprite.texture.width, img.sprite.texture.height);
			}

			// metavliti
			float dy = prefferedSize.y / spriteSize.y;
			//rescale
			float Y = spriteSize.y * dy;
			float X = spriteSize.x * dy;

			if (!expandWidth) {
				if (X > prefferedSize.x) {
					// metavliti
					float dx = prefferedSize.x / X;
					//rescale
					Y = Y * dx;
					X = X * dx;
				}
			}

			container.sizeDelta = new Vector2(X,Y);
		}

		public static void MoveDown(RawImage rm){
			rm.rectTransform.pivot = new Vector2 (0.5f, 0f);
			rm.rectTransform.anchorMin = new Vector2 (0.5f, 0f); 
			rm.rectTransform.anchorMax = new Vector2 (0.5f, 0f); 
		}

		public enum Mode{center, down, up,left, right};

		static float x,y;

		public static void Move(RectTransform rt, Mode mode){
			if (mode == Mode.center) {
				x=y=0.5f;
			} else if (mode == Mode.down) {
				x=0.5f;
				y=0f;
			} else if (mode == Mode.up) {
				x=0.5f;
				y=1f;
			} else if (mode == Mode.left) {
				x=0f;
				y=0.5f;
			} else if (mode == Mode.right) {
				x=1f;
				y=0.5f;
			}

			Vector2 val = new Vector2 (x, y);

			rt.pivot = val;
			rt.anchorMin = val; 
			rt.anchorMax = val;
		}

		public static bool hasChangedSizeDelta(Vector2 currentSize, Vector2 prevFrameSize){
			if(prevFrameSize!=currentSize){
				return true;
			}
			return false;
		}

		/// <summary>
		/// Resizes the rect_ relative to parent.
		/// krataei tin analogia sizedelta se sxesi me tin allagi 
		/// tou parent sizedelta oste to child na paramenei
		/// se megethos idio analogika os pros ton parent rectTrasform
		/// </summary>
		public static void ResizeMoveRect_RelativeToParent(Vector2 actualParentSize, RectTransform parentRect, Vector2 actualChildSize,  RectTransform childRect, Vector2 actualChildPosition, bool needScaling){

			if(needScaling)
			{
				Vector2 posostoSizing = parentRect.sizeDelta;

				float dX = posostoSizing.x / actualParentSize.x;
				float dY = posostoSizing.y / actualParentSize.y;

				childRect.sizeDelta = new Vector2(actualChildSize.x * dX , actualChildSize.y * dY);
			}

			Vector2 childNewPosition = childRect.localPosition;

			childNewPosition.x = actualChildPosition.x / actualParentSize.x * parentRect.sizeDelta.x;
			childNewPosition.y = actualChildPosition.y / actualParentSize.y * parentRect.sizeDelta.y;

			childRect.localPosition = childNewPosition;

//			childRect.localPosition = calculateOnParentResize (actualChildSize, parentRect, childRect.localPosition);

		}

		public static Vector2 calculateOnParentResize(Vector2 actualParentSize, RectTransform parentRect, Vector2 childPosition, Vector2 actualChildPosition){

			Vector2 childNewPosition = childPosition;
			
			childNewPosition.x = actualChildPosition.x / actualParentSize.x * parentRect.sizeDelta.x;
			childNewPosition.y = actualChildPosition.y / actualParentSize.y * parentRect.sizeDelta.y;
			
			return childNewPosition;

		}

		public static void ScaleSizeDelta(RectTransform targetRekt, float scale, Vector2 minMax){
			if (targetRekt != null && scale != 1.0f)
			{
				Vector2 finalScale = targetRekt.sizeDelta;

				finalScale.x *= scale;
				finalScale.y *= scale;
				
				finalScale.x = Mathf.Clamp(finalScale.x, minMax.x, minMax.y);
				finalScale.y = Mathf.Clamp(finalScale.y, minMax.x, minMax.y);
				
				targetRekt.sizeDelta = finalScale;
			}
		}
		#endregion

		#region text tools
		/// <summary>
		/// Resets the position of scroll keimeno to start
		/// </summary>
		public static void ResetKeimeno(Text keimeno){
			//init text at start
			RectTransform rektKeimeno = keimeno.GetComponent<RectTransform>();
			RectTransform rektContainerKeimeno = keimeno.transform.parent.GetComponent<RectTransform>();

			rektKeimeno.localPosition = Vector3.zero;

			#if UNITY_EDITOR
			if (rektKeimeno.sizeDelta.y <= rektContainerKeimeno.sizeDelta.y) {  
				Debug.Log ("keimeo is smaller than parent");
			} else {															
				Debug.Log ("keimeo is bigger than parent -> "+rektKeimeno.sizeDelta.y +" > "+rektContainerKeimeno.sizeDelta.y);
			}
			#endif
		}
		#endregion


//		public static Rect GetScreenRect(RectTransform rectTransform, Canvas canvas) {
//			
//			Vector3[] corners = new Vector3[4];
//			Vector3[] screenCorners = new Vector3[2];
//			
//			rectTransform.GetWorldCorners(corners);
//			
//			if (canvas.renderMode == RenderMode.ScreenSpaceCamera || canvas.renderMode == RenderMode.WorldSpace)
//			{
//				screenCorners[0] = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, corners[1]);
//				screenCorners[1] = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, corners[3]);
//			}
//			else
//			{
//				screenCorners[0] = RectTransformUtility.WorldToScreenPoint(null, corners[1]);
//				screenCorners[1] = RectTransformUtility.WorldToScreenPoint(null, corners[3]);
//			}
//			
//			screenCorners[0].y = Screen.height - screenCorners[0].y;
//			screenCorners[1].y = Screen.height - screenCorners[1].y;
//			
//			return new Rect(screenCorners[0], screenCorners[1] - screenCorners[0]);
//		}
	}
	
	#endregion
	
	#region AREAS Tools
	
	public class AreaTools {
		
		public static  bool pointInsideArea(Vector2 point, cArea area) {		
			bool  oddNodes = false;
			cArea ar=area;			
			int   i, j = ar.Simeia.Count - 1 ;
			float x = point.x;
			float z = point.y;		
			
			for (i = 0; i < ar.Simeia.Count; i++) {
				if (ar.Simeia[i].z < z && ar.Simeia[j].z >= z ||  ar.Simeia[j].z < z && ar.Simeia[i].z >= z) {
					if (ar.Simeia[i].x + (z-ar.Simeia[i].z)/(ar.Simeia[j].z-ar.Simeia[i].z)*(ar.Simeia[j].x-ar.Simeia[i].x) < x) {
						oddNodes=!oddNodes; 
					}
				}
				j=i; 
			}	
			if (oddNodes){
				return true;
			}
			// if none returned true then it is  false!
			return oddNodes;
			
		}

		public static  bool pointInsideDeadArea(Vector2 point, cDeadSpot deadArea) {		
			bool  oddNodes = false;
			cDeadSpot ar=deadArea;			
			int   i, j = ar.points.Count - 1 ;
			float x = point.x;
			float z = point.y;		
			
			for (i = 0; i < ar.points.Count; i++) {
				if (ar.points[i].z < z && ar.points[j].z >= z ||  ar.points[j].z < z && ar.points[i].z >= z) {
					if (ar.points[i].x + (z-ar.points[i].z)/(ar.points[j].z-ar.points[i].z)*(ar.points[j].x-ar.points[i].x) < x) {
						oddNodes=!oddNodes; 
					}
				}
				j=i; 
			}	
			if (oddNodes){
				return true;
			}
			// if none returned true then it is  false!
			return oddNodes;
			
		}

		/// <summary>
		/// -1 αν ειναι παραλληλες
		/// 0 αν ειναι συνεχομενες
		/// 1 αν τεμνονται εντος οριων
		/// 2 αν η μια γραμμη ακουμπα την μια ακρη της στο ενδιαμεσο της αλλης
		/// 3 αν τεμνονται νοητα (εκτος οριων) και η μια γραμμη ακουμπα νοητα την μια ακρη της στο ενδιαμεσο της αλλης
		///   και το σημειο επαφης ειναι εντος οριων σε μια απο τις 2 γραμμες
		/// 4 αν τεμνονται νοητα και το σημειο επαφης ειναι εκτος οριων και απο τις 2 γραμμες
		public static int isLineCrossing(out Vector3 resultPoint, cLineSegment lineA, cLineSegment lineB, bool cutLines, List<cArea> perioxes){
			//Line A
			Vector3 Astart = new Vector3(lineA.StartOfLine.x, 0f, lineA.StartOfLine.y);
			Vector3 Aend = new Vector3(lineA.EndOfLine.x, 0f, lineA.EndOfLine.y);
			//Line B
			Vector3 Bstart = new Vector3(lineB.StartOfLine.x, 0f, lineB.StartOfLine.y);
			Vector3 Bend = new Vector3(lineB.EndOfLine.x, 0f, lineB.EndOfLine.y);
			
			//dianysma A
			Vector3 dirA = Aend-Astart;
			//dianysna B
			Vector3 dirB = Bend-Bstart;
			
			//find if 2 lines are crossing
			if(LineLineIntersection(out resultPoint,Astart,dirA,Bstart,dirB)){	//Debug.LogWarning(resultPoint);
				
				//dianysma crossin point to start of line A
				Vector3 dir1 = Astart-resultPoint;
				//dianysma crossin point to end of line A
				Vector3 dir2 = Aend-resultPoint;
				
				//get dot product 
				//για να δουμε αν το σημειο ειναι εντος των οριων της γραμμης
				//η νοητα (με προεκταση) εκτος των οριων της γραμμης
				float dotA = Vector3.Dot(dir1.normalized,dir2.normalized);
				
				//dianysma crossin point to start of line B
				Vector3 dir3 = Bstart-resultPoint;
				//dianysma crossin point to end of line B
				Vector3 dir4= Bend-resultPoint;
				
				//get dot product 
				//για να δουμε αν το σημειο ειναι εντος των οριων της γραμμης
				//η νοητα (με προεκταση) εκτος των οριων της γραμμης
				float dotB = Vector3.Dot(dir3.normalized,dir4.normalized);
				
				//η γραμμες ειναι διαδοχικα συνεχομενες
				if(dotA==0 && dotB==0){
					return 0;
				}else
					//οι γραμμες τεμνονται και το σημειο επαφης
					//βρισκεται εντος των οριων και των 2 γραμμων
				if(dotA==-1 && dotB ==-1){		
					
					if(cutLines)
					{
						Vector2 r = new Vector2(resultPoint.x, resultPoint.z);
						
						//					Debug.LogWarning("cutting lines at "+r);
						
						if(perioxes.Count>0)
						{
							foreach(cArea area in perioxes)
							{
								if(!area.PerimetrosLines.Contains(lineA))
								{
									if(pointInsideArea(lineA.StartOfLine, area) && !pointInsideArea(lineA.EndOfLine, area)){
										lineA.StartOfLine = r;
									}else
									if(pointInsideArea(lineA.EndOfLine, area) && !pointInsideArea(lineA.StartOfLine, area)){
										lineA.EndOfLine = r;
									}
								}
								
								if(!area.PerimetrosLines.Contains(lineB))
								{
									if(pointInsideArea(lineB.StartOfLine, area) && !pointInsideArea(lineB.EndOfLine, area)){
										lineB.StartOfLine = r;
									}else
									if(pointInsideArea(lineB.EndOfLine, area) && !pointInsideArea(lineB.StartOfLine, area)){
										lineB.EndOfLine = r;
									}
								}
							}
						}
					}
					
					//temnontai
					return 1;
				}
				else
					//τεμνονται και το σημειο επαφης ειναι μεσα στα ορια της μιας γραμμης
					//και πανω σε ενα απο τα ορια της αλλης
					//δηλαδη η μια γραμμη ακουμπα την μια ακρη της στο ενδιαμεσο της αλλης
				if((dotA==-1 && dotB==0) || (dotA==0 && dotB==-1)){
					return 2;
				}else
					//τεμνονται νοητα και το σημειο επαφης ειναι μεσα στα ορια της μιας γραμμης
					//και πανω σε ενα απο τα ορια της αλλης
					//δηλαδη η μια γραμμη ακουμπα νοητα την μια ακρη της στο ενδιαμεσο της αλλης
				if((dotA==1 && dotB==0) || (dotA==0 && dotB==1)){
					return 3;
				}else
					//τεμνονται νοητα και το σημειο επαφης ειναι εκτος οριων και απο τις 2 γραμμες
				if((dotA==1 && dotB==1) || (dotA==1 && dotB==1)){
					return 4;
				}
				
				//Debug.Log("CROSSING to "+result);
				
				//			targetResult.position = resultPoint;
			}
			
			
			return -1;
			
		}

		/// <summary>
		/// -1 αν ειναι παραλληλες
		/// 0 αν ειναι συνεχομενες
		/// 1 αν τεμνονται εντος οριων
		/// 2 αν η μια γραμμη ακουμπα την μια ακρη της στο ενδιαμεσο της αλλης
		/// 3 αν τεμνονται νοητα (εκτος οριων) και η μια γραμμη ακουμπα νοητα την μια ακρη της στο ενδιαμεσο της αλλης
		///   και το σημειο επαφης ειναι εντος οριων σε μια απο τις 2 γραμμες
		/// 4 αν τεμνονται νοητα και το σημειο επαφης ειναι εκτος οριων και απο τις 2 γραμμες
		public static int isLineCrossing(out Vector3 resultPoint, cLineSegment lineA, cLineSegment lineB, bool cutLines, List<cDeadSpot> deadPerioxes){
			//Line A
			Vector3 Astart = new Vector3(lineA.StartOfLine.x, 0f, lineA.StartOfLine.y);
			Vector3 Aend = new Vector3(lineA.EndOfLine.x, 0f, lineA.EndOfLine.y);
			//Line B
			Vector3 Bstart = new Vector3(lineB.StartOfLine.x, 0f, lineB.StartOfLine.y);
			Vector3 Bend = new Vector3(lineB.EndOfLine.x, 0f, lineB.EndOfLine.y);
			
			//dianysma A
			Vector3 dirA = Aend-Astart;
			//dianysna B
			Vector3 dirB = Bend-Bstart;
			
			//find if 2 lines are crossing
			if(LineLineIntersection(out resultPoint,Astart,dirA,Bstart,dirB)){	//Debug.LogWarning(resultPoint);
				
				//dianysma crossin point to start of line A
				Vector3 dir1 = Astart-resultPoint;
				//dianysma crossin point to end of line A
				Vector3 dir2 = Aend-resultPoint;
				
				//get dot product 
				//για να δουμε αν το σημειο ειναι εντος των οριων της γραμμης
				//η νοητα (με προεκταση) εκτος των οριων της γραμμης
				float dotA = Vector3.Dot(dir1.normalized,dir2.normalized);
				
				//dianysma crossin point to start of line B
				Vector3 dir3 = Bstart-resultPoint;
				//dianysma crossin point to end of line B
				Vector3 dir4= Bend-resultPoint;
				
				//get dot product 
				//για να δουμε αν το σημειο ειναι εντος των οριων της γραμμης
				//η νοητα (με προεκταση) εκτος των οριων της γραμμης
				float dotB = Vector3.Dot(dir3.normalized,dir4.normalized);
				
				//η γραμμες ειναι διαδοχικα συνεχομενες
				if(dotA==0 && dotB==0){
					return 0;
				}else
					//οι γραμμες τεμνονται και το σημειο επαφης
					//βρισκεται εντος των οριων και των 2 γραμμων
				if(dotA==-1 && dotB ==-1){		
					
					if(cutLines)
					{
						Vector2 r = new Vector2(resultPoint.x, resultPoint.z);
						
						//					Debug.LogWarning("cutting lines at "+r);
						
						if(deadPerioxes.Count>0)
						{
							foreach(cDeadSpot area in deadPerioxes)
							{
								if(!area.DeadPerimetros.Contains(lineA))
								{
									if(pointInsideDeadArea(lineA.StartOfLine, area) && !pointInsideDeadArea(lineA.EndOfLine, area)){
										lineA.StartOfLine = r;
									}else
									if(pointInsideDeadArea(lineA.EndOfLine, area) && !pointInsideDeadArea(lineA.StartOfLine, area)){
										lineA.EndOfLine = r;
									}
								}
								
								if(!area.DeadPerimetros.Contains(lineB))
								{
									if(pointInsideDeadArea(lineB.StartOfLine, area) && !pointInsideDeadArea(lineB.EndOfLine, area)){
										lineB.StartOfLine = r;
									}else
									if(pointInsideDeadArea(lineB.EndOfLine, area) && !pointInsideDeadArea(lineB.StartOfLine, area)){
										lineB.EndOfLine = r;
									}
								}
							}
						}
					}
					
					//temnontai
					return 1;
				}
				else
					//τεμνονται και το σημειο επαφης ειναι μεσα στα ορια της μιας γραμμης
					//και πανω σε ενα απο τα ορια της αλλης
					//δηλαδη η μια γραμμη ακουμπα την μια ακρη της στο ενδιαμεσο της αλλης
				if((dotA==-1 && dotB==0) || (dotA==0 && dotB==-1)){
					return 2;
				}else
					//τεμνονται νοητα και το σημειο επαφης ειναι μεσα στα ορια της μιας γραμμης
					//και πανω σε ενα απο τα ορια της αλλης
					//δηλαδη η μια γραμμη ακουμπα νοητα την μια ακρη της στο ενδιαμεσο της αλλης
				if((dotA==1 && dotB==0) || (dotA==0 && dotB==1)){
					return 3;
				}else
					//τεμνονται νοητα και το σημειο επαφης ειναι εκτος οριων και απο τις 2 γραμμες
				if((dotA==1 && dotB==1) || (dotA==1 && dotB==1)){
					return 4;
				}
				
				//Debug.Log("CROSSING to "+result);
				
				//			targetResult.position = resultPoint;
			}
			
			
			return -1;
			
		}


		///Calculate the intersection point of two lines. Returns true if lines intersect, otherwise false.
		///Note that in 3d, two lines do not intersect most of the time. So if the two lines are not in the 
		///same plane, use ClosestPointsOnTwoLines() instead.
		public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2){
			
			Vector3 lineVec3 = linePoint2 - linePoint1;
			Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
			Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);
			
			float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);
			
			///is coplanar, and not parrallel
			if(Mathf.Abs(planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f)
			{
				float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
				intersection = linePoint1 + (lineVec1 * s);
				return true;
			}
			else
			{
				intersection = Vector3.zero;
				return false;
			}
		}

		public static bool LineLineIntersection(cLineSegment lineA, cLineSegment lineB){// Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2){

			//Line A
			Vector3 Astart = new Vector3(lineA.StartOfLine.x, 0f, lineA.StartOfLine.y);
			Vector3 Aend = new Vector3(lineA.EndOfLine.x, 0f, lineA.EndOfLine.y);
			//Line B
			Vector3 Bstart = new Vector3(lineB.StartOfLine.x, 0f, lineB.StartOfLine.y);
			Vector3 Bend = new Vector3(lineB.EndOfLine.x, 0f, lineB.EndOfLine.y);
			
			//dianysma A
			Vector3 dirA = Aend-Astart;
			//dianysna B
			Vector3 dirB = Bend-Bstart;


			
			Vector3 lineVec3 = Bstart - Astart;
			Vector3 crossVec1and2 = Vector3.Cross(dirA, dirB);
			Vector3 crossVec3and2 = Vector3.Cross(lineVec3, dirB);
			
			float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);
			
			///is coplanar, and not parrallel
			if(Mathf.Abs(planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f)
			{
				//float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
//				intersection = linePoint1 + (lineVec1 * s);
				return true;
			}
			else
			{
//				intersection = Vector3.zero;
				return false;
			}
		}


		///find if an area's all points are inside another area
		///and if true make small area dead area of the bigger area
		/// and remove small area from areas list
		public static bool areaInsideArea(out cArea areaBig, cArea area, List<cArea> areas){
			if(areas.Count>0)
			{
				foreach(cArea perioxi in areas)
				{
					//dont check the same area
					if(area != perioxi)
					{
						//check all points of the area if are inside perioxi
						if(area.Simeia.Count>0)
						{
							int count = 0;

							foreach(Vector3 point in area.Simeia)
							{
								Vector2 p = new Vector2(point.x, point.z);
								
								if(pointInsideArea(p,perioxi))
								{
									//count points that are inside perioxi
									count++;
								}
							}
							
							//if all points are inside make area dead area of perioxi
							if(count == area.Simeia.Count){
//								perioxi.DeadLines = new List<cLineSegment>();
								perioxi.DeadLines = area.PerimetrosLines;
								//TODO ????? error ?
								//							areas.Remove(area);
								areaBig = perioxi;
								return true;
							}
						}
					}
				}
			}
			areaBig=null;
			return false;
		}



		public static List<Vector3> ExportAreaSimeia(List<GameObject> points){
			List<Vector3> simeia=new List<Vector3>();
			List<GameObject> pointsOrdered=new List<GameObject>();
					
			//clear list from previous path
			pointsOrdered.Clear();
			
			//tranfer to ordered list according to distance
			pointsOrdered.Add (points[0]);
			GameObject curPoint=points[0];
			curPoint.transform.position=new Vector3(Mathf.Round(curPoint.transform.position.x *100f)/100f, curPoint.transform.position.y, Mathf.Round(curPoint.transform.position.z *100f)/100f);
			
			int num=points.Count;

			for (int a=0; a<num; a++){
				points.Remove(curPoint);

				pointsOrdered.Add (curPoint);
				
				if (a<num-1){
					GameObject nextPoint=FindNearestPoint(curPoint,points);
					curPoint=nextPoint;
					nextPoint.transform.position=new Vector3(Mathf.Round(nextPoint.transform.position.x *100f)/100f, nextPoint.transform.position.y, Mathf.Round(nextPoint.transform.position.z * 100f)/100f);
				}
				
			}
					
			if(pointsOrdered.Count>0){
				for(int i=0; i<pointsOrdered.Count; i++){
					simeia.Add(pointsOrdered[i].transform.position);
				}
			}

			return simeia;
		}



		static GameObject FindNearestPoint(GameObject point, List<GameObject> pointsList){
			int r = (int)Mathf.Floor(Random.Range(0f, (float)(pointsList.Count-1)));
			GameObject closestPoint=pointsList[r];
			float distance=Vector2.Distance(new Vector2(point.transform.position.x, point.transform.position.z), new Vector2(closestPoint.transform.position.x,closestPoint.transform.position.z));
			foreach (GameObject pt in pointsList) {
				float dist=Vector2.Distance(new Vector2(point.transform.position.x, point.transform.position.z), new Vector2(pt.transform.position.x,pt.transform.position.z));
				if ((dist!=0)&&(dist<distance)){
					distance=dist;
					closestPoint=pt;
				}
			}
			return closestPoint;
		}


	}

	#endregion

//	public class Create : MonoBehaviour
//	{
//		public static Canvas NewCanvas(Vector2 size, Vector2 pos){
//			GameObject kanvas = new GameObject();
//			kanvas.name = "Canvas2D";
//			RectTransform canvasRT = kanvas.AddComponent<RectTransform>();
//			Canvas canvasCV = kanvas.AddComponent<Canvas>();
//			canvasCV.renderMode = RenderMode.ScreenSpaceCamera;
//			return canvasCV;
////			Vector3 kanvasPos = Camera.main.transform.position;
////			pos  += Camera.main.transform.forward * 10.0f; 
////			canvasCV.worldCamera = Camera.main;
//		}
//
//		public static void NewButton(Canvas kanvas, Vector2 size, Vector2 pos){
//			Button btn = new Button();
////			GameObject btn = new GameObject ();
//			RectTransform btnRT = btn.GetComponent<RectTransform> ();
////			btn.AddComponent<Image> ();
////			btn.AddComponent<Button> ();
//			btnRT.position = pos;
//			btnRT.sizeDelta = size;
////			btn.transform.parent = kanvas.transform;
//
//		}
//	}


}
