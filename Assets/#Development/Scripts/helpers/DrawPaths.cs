using UnityEngine;
using System.Collections;

public class DrawPaths : MonoBehaviour {

	#if UNITY_EDITOR
	void OnDrawGizmos(){
		if(Diadrasis.Instance.navMode==Diadrasis.NavMode.offSite)
		{
			//show path
			if(moveSettings.playerPath.Count>0){
				for (int k=0; k<moveSettings.playerPath.Count; k++) {
					Gizmos.color=Color.blue;
					Gizmos.DrawLine(new Vector3(moveSettings.playerPath[k].StartOfLine.x, 50f,moveSettings.playerPath[k].StartOfLine.y) , new Vector3(moveSettings.playerPath[k].EndOfLine.x, 50f,moveSettings.playerPath[k].EndOfLine.y));
				}
			}

//			//show active areas
//			if (moveSettings.activeAreas.Count > 0) {
//				for (int l=0; l<moveSettings.activeAreas.Count; l++) {
//					for (int m=0; m<moveSettings.activeAreas[l].Simeia.Count-1; m++) {
//						Gizmos.color=Color.blue;
//						Gizmos.DrawLine (new Vector3 (moveSettings.activeAreas[l].Simeia [m].x, 50f, moveSettings.activeAreas [l].Simeia [m].z), new Vector3 (moveSettings.activeAreas [l].Simeia [m + 1].x, 40, moveSettings.activeAreas [l].Simeia [m + 1].z));
//					}
//					//close the loop
//					Gizmos.color=Color.green;
//					Gizmos.DrawLine (new Vector3 (moveSettings.activeAreas[l].Simeia [moveSettings.activeAreas[l].Simeia.Count-1].x, 40, moveSettings.activeAreas[l].Simeia[moveSettings.activeAreas[l].Simeia.Count-1].z), new Vector3 (moveSettings.activeAreas [l].Simeia [0].x, 40, moveSettings.activeAreas[l].Simeia[0].z));
//				}
//			}
//
//			//show dead areas
//			if (moveSettings.deadSpots.Count > 0) {
//				for (int l=0; l<moveSettings.deadSpots.Count; l++) {
//					for (int m=0; m<moveSettings.deadSpots[l].points.Count-1; m++) {
//						Gizmos.color=Color.black;
//						Gizmos.DrawLine (new Vector3 (moveSettings.deadSpots[l].points [m].x, 40, moveSettings.deadSpots [l].points [m].z), new Vector3 (moveSettings.deadSpots [l].points [m + 1].x, 40, moveSettings.deadSpots [l].points [m + 1].z));
//					}
//					//close the loop
//					Gizmos.color=Color.black;
//					Gizmos.DrawLine (new Vector3 (moveSettings.deadSpots[l].points [moveSettings.deadSpots[l].points.Count-1].x, 40, moveSettings.deadSpots[l].points[moveSettings.deadSpots[l].points.Count-1].z), new Vector3 (moveSettings.deadSpots [l].points [0].x, 40, moveSettings.deadSpots[l].points[0].z));
//				}
//			}

			if(moveSettings.activeAreasPerimetroi.Count>0){
				for (int k=0; k<moveSettings.activeAreasPerimetroi.Count; k++) {
					Gizmos.color=Color.blue;
					Gizmos.DrawLine(new Vector3(moveSettings.activeAreasPerimetroi[k].StartOfLine.x, 50f,moveSettings.activeAreasPerimetroi[k].StartOfLine.y) , new Vector3(moveSettings.activeAreasPerimetroi[k].EndOfLine.x, 50f,moveSettings.activeAreasPerimetroi[k].EndOfLine.y));
				}
			}

			if(moveSettings.deadSpotsPerimetroi.Count>0){
				for (int k=0; k<moveSettings.deadSpotsPerimetroi.Count; k++) {
					Gizmos.color=Color.red;
					Gizmos.DrawLine(new Vector3(moveSettings.deadSpotsPerimetroi[k].StartOfLine.x, 50f,moveSettings.deadSpotsPerimetroi[k].StartOfLine.y) , new Vector3(moveSettings.deadSpotsPerimetroi[k].EndOfLine.x, 50f,moveSettings.deadSpotsPerimetroi[k].EndOfLine.y));
				}
			}

			if(moveSettings.activeAreasOnAirPerimetroi.Count>0){
				for (int k=0; k<moveSettings.activeAreasOnAirPerimetroi.Count; k++) {
					Gizmos.color=Color.yellow;
					Gizmos.DrawLine(new Vector3(moveSettings.activeAreasOnAirPerimetroi[k].StartOfLine.x, 50f,moveSettings.activeAreasOnAirPerimetroi[k].StartOfLine.y) , new Vector3(moveSettings.activeAreasOnAirPerimetroi[k].EndOfLine.x, 50f,moveSettings.activeAreasOnAirPerimetroi[k].EndOfLine.y));
				}
			}
		}
		else
		if(Diadrasis.Instance.navMode==Diadrasis.NavMode.onSite)
		{
			//show onsite path
			if(moveSettings.pathOnSite.Count>0){
				for (int k=0; k<moveSettings.pathOnSite.Count; k++) {
					Gizmos.color=Color.red;
					Gizmos.DrawLine(new Vector3(moveSettings.pathOnSite[k].StartOfLine.x, 50f,moveSettings.pathOnSite[k].StartOfLine.y) , new Vector3(moveSettings.pathOnSite[k].EndOfLine.x, 50f,moveSettings.pathOnSite[k].EndOfLine.y));
				}
			}
			//show onsite active areas
			if (moveSettings.activeAreasOnSite.Count > 0) {
				for (int l=0; l<moveSettings.activeAreasOnSite.Count; l++) {
					for (int m=0; m<moveSettings.activeAreasOnSite[l].Simeia.Count-1; m++) {
						Gizmos.color=Color.green;
						Gizmos.DrawLine (new Vector3 (moveSettings.activeAreasOnSite[l].Simeia [m].x, 40, moveSettings.activeAreasOnSite [l].Simeia [m].z), new Vector3 (moveSettings.activeAreasOnSite [l].Simeia [m + 1].x, 40, moveSettings.activeAreasOnSite [l].Simeia [m + 1].z));
					}
					//close the loop
					Gizmos.color=Color.green;
					Gizmos.DrawLine (new Vector3 (moveSettings.activeAreasOnSite[l].Simeia [moveSettings.activeAreasOnSite[l].Simeia.Count-1].x, 40, moveSettings.activeAreasOnSite[l].Simeia[moveSettings.activeAreasOnSite[l].Simeia.Count-1].z), new Vector3 (moveSettings.activeAreasOnSite [l].Simeia [0].x, 40, moveSettings.activeAreasOnSite[l].Simeia[0].z));
				}
			}
			//show onsite dead areas
			if (moveSettings.deadSpotsOnSite.Count > 0) {
				for (int l=0; l<moveSettings.deadSpotsOnSite.Count; l++) {
					for (int m=0; m<moveSettings.deadSpotsOnSite[l].points.Count-1; m++) {
						Gizmos.color=Color.yellow;
						Gizmos.DrawLine (new Vector3 (moveSettings.deadSpotsOnSite[l].points [m].x, 40, moveSettings.deadSpotsOnSite [l].points [m].z), new Vector3 (moveSettings.deadSpotsOnSite [l].points [m + 1].x, 40, moveSettings.deadSpotsOnSite [l].points [m + 1].z));
					}
					//close the loop
					Gizmos.color=Color.yellow;
					Gizmos.DrawLine (new Vector3 (moveSettings.deadSpotsOnSite[l].points [moveSettings.deadSpotsOnSite[l].points.Count-1].x, 40, moveSettings.deadSpotsOnSite[l].points[moveSettings.deadSpotsOnSite[l].points.Count-1].z), new Vector3 (moveSettings.deadSpotsOnSite [l].points [0].x, 40, moveSettings.deadSpotsOnSite[l].points[0].z));
				}
			}

			//show active area for current scene
			if (moveSettings.activeAreaForScene.Count > 0) {
				for (int l=0; l<moveSettings.activeAreaForScene.Count; l++) {
					for (int m=0; m<moveSettings.activeAreaForScene[l].Simeia.Count-1; m++) {
						Gizmos.color=Color.blue;
						Gizmos.DrawLine (new Vector3 (moveSettings.activeAreaForScene[l].Simeia [m].x, 40, moveSettings.activeAreaForScene [l].Simeia [m].z), new Vector3 (moveSettings.activeAreaForScene [l].Simeia [m + 1].x, 40, moveSettings.activeAreaForScene [l].Simeia [m + 1].z));
					}
					//close the loop
					Gizmos.color=Color.blue;
					Gizmos.DrawLine (new Vector3 (moveSettings.activeAreaForScene[l].Simeia [moveSettings.activeAreaForScene[l].Simeia.Count-1].x, 40, moveSettings.activeAreaForScene[l].Simeia[moveSettings.activeAreaForScene[l].Simeia.Count-1].z), new Vector3 (moveSettings.activeAreaForScene [l].Simeia [0].x, 40, moveSettings.activeAreaForScene[l].Simeia[0].z));
				}
			}

			if(moveSettings.activeAreasOnAirPerimetroi.Count>0){
				for (int k=0; k<moveSettings.activeAreasOnAirPerimetroi.Count; k++) {
					Gizmos.color=Color.yellow;
					Gizmos.DrawLine(new Vector3(moveSettings.activeAreasOnAirPerimetroi[k].StartOfLine.x, 50f,moveSettings.activeAreasOnAirPerimetroi[k].StartOfLine.y) , new Vector3(moveSettings.activeAreasOnAirPerimetroi[k].EndOfLine.x, 50f,moveSettings.activeAreasOnAirPerimetroi[k].EndOfLine.y));
				}
			}
		}
	}
	#endif
}
