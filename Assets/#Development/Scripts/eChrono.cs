using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace eChrono{

	public class cLineSegment {
		private Vector2 startOfLine;
		private Vector2 endOfLine;
		private bool moveOutOfPathWithLimits;
		
		public Vector2 StartOfLine{ get; set; }		

		public Vector2 EndOfLine{ get; set; }

		public bool hasPathFreeMove{ get; set; }

		public cLineSegment(){}
			
		public cLineSegment(Vector2 s, Vector2 e){
			StartOfLine=s;
			EndOfLine=e;
		}
	}

	public class cSnapPosition{
		public Vector2 position;
		public float sqrDistance;
		private bool _limitsOn;

		public bool limitsOn{
			get{return _limitsOn;}
			set{_limitsOn=value;}
		}
		
		public cSnapPosition(){}
	}

	public class cCoupon{
		private string code;
		private int mikos;
		private string customItem;

		public int Mikos{get; set;}
		public string Code{get;set;}
		public string CustomItem{ get; set; }
	}

	public class cTerm {	
		private string name;
		private string text;
		
		public string Name{ get; set; }	
		public string Text{ get; set; }	
	}
	
	public class cHelpTerm {	
		private string _name;
		private string _text;
		private string _image;
		
		public string name   {
			get{return _name;}
			set{_name = value;}
		}		
		public string text  {
			get{return _text;}
			set{_text = value;}
		}	
		
		public string image{
			get{return _image;}
			set{_image = value;}
		}
	}

	//define video
	public class cVideo {
		private string _file;
		private string _text;
		
		public string file	{
			get{return _file;}
			set{_file = value;}
		}		
		public string text 	{
			get{return _text;}
			set{_text = value;}
		}		
		public cVideo(){}			
	}
	
	public class cImage {
		private string _file;
		private string _text;
		
		public string file	{
			get{return _file;}
			set{_file = value;}
		}		
		public string text 	{
			get{return _text;}
			set{_text = value;}
		}		
		public cImage(){			
		}			
	}

	//narration
	public class cNarration{
		private string _file;


		public string file	{
			get{return _file;}
			set{_file = value;}
		}		

		public cNarration(){			
		}			

	}
	
	public class cTarget {
		private string _name;
		private Vector3 _position;
		private Quaternion _rotation;
		
		public string name {
			get { return _name;}
			set { _name = value; }
		}
		
		public Vector3 position {
			get { return _position;}
			set { _position = value;}
		}
		
		public Quaternion rotation {
			get { return _rotation;}
			set { _rotation = value;}
		}
	}

	public class cDeadSpot {
		private string _name;
		private Vector2 _center;
		private float _radius; //alternative
		private List<Vector3> _points;

		private List<cLineSegment> deadPerimetros;
		//dead perimetros lines
		public List<cLineSegment> DeadPerimetros{get; set;}
		
		public string name {
			get { return _name;}
			set{_name = value;}
		}
		
		public Vector2 center{
			get { return _center;}
			set{_center = value;}
		}
		
		public float radius{
			get { return _radius;}
			set{_radius = value;}
		}
		
		public List<Vector3> points {
			get {return _points;}
			set {_points = value;}
		}	
		
		public cDeadSpot () {}
	}
	
	public class cArea {
		private string nomaPerioxis;
		private Vector2 centerOfArea;
		private float aktina; //alternative
		private List<Vector3> simeia;
		private List<cLineSegment> perimetrosLines;
		private List<cLineSegment> deadLines;

		//perimetros of this area
		public List<cLineSegment> PerimetrosLines{get; set;}
		//dead areas inside this area
		public List<cLineSegment> DeadLines{get; set;}
		
		public string OnomaPerioxis{ get; set; }
		
		public Vector2 CenterOfArea{ get; set; }
		
		public float Aktina{ get; set; }
		
		public List<Vector3> Simeia { get; set; }
		
		public cArea () {}
	}
	
	public class cIntroNarration {
		private string file;
		private float pauseTime;
		
		public string File{ get; set; }
		
		public float PauseTime{ get; set; }
		
		public cIntroNarration(){}
	}

	public class cIntro {
		private List<cIntroNarration> narrations;
		private string loadingText;
		private string introText;
		private string introTitle;

		public string IntroTitle{get;set;}
		public string IntroText{get;set;}

		public List<cIntroNarration> Narrations{ get; set; }
		
		public string LoadingText{ get; set; }

		public cIntro(){}
	}

	public class cPeriod{
		private string poiName;
		private string period_Image;
		private string sceneName;
		private string periodTitle;
		private string loadinImage;
		private string loadingText;
		private string mapImage;
		private string mapFilterPaths;
		private Vector2 mapPivot;
		private Vector2 mapFullPos;
		private Vector2 mapFullZoom;
		private Vector2 gpsCenter;

		private cIntro intro;

		public cIntro Intro{ get; set; }

		public Vector2 MapFullPivot{ get; set; }

		public Vector2 MapFullPosition{ get; set; }

		public Vector2 MapFullZoom{ get; set; }

		public string PoiName{ get; set; }

		public string MapImage { get; set; }

		//public string MapFilterPaths { get; set; }

		public string Period_Image{ get; set; }

		public string SceneName{ get; set; }

		public string LabelTitle{ get; set; }

		public string LoadinImage{ get; set; }

		public string LoadingText{ get; set; }

		public Vector2 GpsCenter{ get; set; }
	}

	public class cSceneArea {
		private string name;
		private string labelTitle;
		private string imageOnMap;
		private Vector2 mapLabelPos;
		private Vector4 mapBtnSizePos;
		private List<cPeriod> periods;

		///just a refference name
		public string Name {get; set;}
		///the name of scene on label
		public string LabelTitle{ get; set;}
		///the scene image on mmenu map
		public string ImageOnMap{ get; set;}
		///the pos of label on menu map
		public Vector2 MapLabelPos { get; set;}
		///the size and the pos of invisible button for scene
		public Vector4 MapBtnSizePos{ get; set; }
		///the periods of current scene
		public List<cPeriod> Periods{ get; set; }
	}
	
	//define point 
	//declare the model class
	public class cPoi {	
		private string _name;
		private string _title;
		private string _shortDesc;
		private string _desc;
		private GameObject _go; //do we need?
		private bool _isHumanoid;
		private List<cImage> _images;
		private List<cVideo> _videos;
		private List<cNarration> _narrations;
		private bool showInfo;

		public bool ShowtInfo{get; set;}
		
		public string name   {
			get{return _name;}
			set{_name = value;}
		}
		
		public string title  {
			get{return _title;}
			set{_title = value;}
		}
		
		public string shortDesc {
			get{return _shortDesc;}
			set{_shortDesc = value;}
		}	
		
		public string desc  {
			get{return _desc;}
			set{_desc = value;}
		}	
		
		public List<cImage> images{
			get{return _images;}
			set{_images =value;}
		}
		
		public List<cVideo> videos{
			get{return _videos;}
			set{_videos =value;}
		}

		public List<cNarration> narrations{
			get{return _narrations;}
			set{_narrations =value;}
		}
		
		public GameObject gameObject{
			get{return _go;}
			set{_go = value;}	
		}
		
		public cPoi(){			
			
		}
	}
}
