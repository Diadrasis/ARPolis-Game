using UnityEngine;
using System.Collections;

public class TestGUI : MonoBehaviour
{
		float virtualWidth = 1920f;
		float virtualHeight = 1080f;
		Matrix4x4 matrix;
		GUIStyle style;

		// Use this for initialization
		void Start ()
		{
				style = new GUIStyle ();
				style.fontSize = 15;
				matrix = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, new Vector3 (Screen.width / virtualWidth, Screen.height / virtualHeight, 1.0f));
		}

		void OnGUI ()
		{
				GUI.matrix = matrix;
				GUILayout.BeginArea (new Rect (0, 0, 860, 430));
				GUILayout.Label ("Blablablou dasadas gf dgfa gfadsg asdg afdsgfads gafd gsdf gsdfg" +
					"s gfsdfg sdfg sdfg sdf gsdf gdsf g dfsg sdfg sddsgf gfdsgsdfg " +
					"sdfg gdfsgfds gdfsg dfgdfsgg fgsdfg sdfgsdfg dfsg dfgdf dfg dfg dfsg sdfg dsfg dfsg fgdfs" +
					" sdfgsdfg dfsgfd dfgdf sgsdfg dfssgsdfgsdfg fdsfdg ddfsgs sfgddfsg dfgdfsgs dfgdfsgdfsg dfg sdfg " +
					"wretwertrtterwrwet erw erw rwetrt wert wretwert wert wer er erwtwertwreterwt retw rter wetwerwert reter" +
					"sgdgcvxbbcvbcbcvbbncbvncbvn cbvn cvn cvcvnb cbnvbvcnc  ncbv ncbvbcvnbcv nc vbn cvb ncv cvnb ncvbn cbv" +
					"kljgfl gsdkgjs dlkfgjdflskgj lskgfdjklsdgj kldgj dlkfg", GUILayout.Width(860), GUILayout.Height(430));
				GUILayout.EndArea ();
		}
}
