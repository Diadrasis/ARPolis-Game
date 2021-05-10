using UnityEngine;

[ExecuteInEditMode]
public class PointTool_AreaPathManager : MonoBehaviour
{
	public float shieldArea = 2f;
	public int ID=-1;
	public float radius;
	public Vector3 direction;
	
	void OnDrawGizmos()
	{
//		direction = Vector3.Normalize(direction);
//		GameObject[] objs = FindObjectsOfType(typeof(GameObject))as GameObject[];
//		foreach (GameObject g in objs)
//		{
//			float r = radius;
//			CircleCollider2D c2d = g.GetComponent<CircleCollider2D>();
//			if(c2d != null)
//			{    r = c2d.radius;
//				UnityEditor.Handles.color = Color.green;
//				UnityEditor.Handles.DrawWireDisc(g.transform.position ,direction, r);
//			}
//		}
	}
}


