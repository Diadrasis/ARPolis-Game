using UnityEngine;
using System.Collections;

// Require these components when using this script
using System.Collections.Generic;


[RequireComponent(typeof (Animator))]
[RequireComponent(typeof (CapsuleCollider))]
[RequireComponent(typeof (Rigidbody))]
public class garissonLife : MonoBehaviour
{
	[System.NonSerialized]					
	public float lookWeight;					// the amount to transition when using head look
	
	[System.NonSerialized]
	public Transform enemy;						// a transform to Lerp the camera to during head look
	
	public float animSpeed = 0.5f;				// a public setting for overall animator animation speed
	public float lookSmoother = 3f;				// a smoothing setting for camera motion							
	public List<Vector3> checkpoints = new List<Vector3>();

	private Animator anim;							// a reference to the animator on the character
	private AnimatorStateInfo currentBaseState;			// a reference to the current state of the animator, used for base layer
	private AnimatorStateInfo layer2CurrentState;	// a reference to the current state of the animator, used for layer 2
//	private CapsuleCollider col;					// a reference to the capsule collider of the character
	
	
	static int IdleState = Animator.StringToHash("Base Layer.Idle");
	static int WalkForwardState = Animator.StringToHash("Base Layer.WalkForward");
	static int TurnState = Animator.StringToHash("Base Layer.Turn");
	
	int current = 0;
	Vector3 target;
	void Start ()
	{
		// initialising reference variables
		anim = GetComponent<Animator>();					  
//		col = GetComponent<CapsuleCollider>();				
		if (checkpoints.Count > 1 ) {
			target = checkpoints[current];
			current++;
		}
	}
	
	float v = 0.5f, h = 0f;
	bool doTurn = false;
	void Update ()
	{
		if (checkpoints.Count > 1) {
			anim.SetFloat("Speed", v);							// set our animator's float parameter 'Speed' equal to the vertical input axis				
			anim.SetFloat("Direction", h); 						// set our animator's float parameter 'Direction' equal to the horizontal input axis		

			anim.speed = animSpeed;								// set the speed of our animator to the public variable 'animSpeed'
			anim.SetLookAtWeight(lookWeight);					// set the Look At Weight - amount to use look at IK vs using the head's animation
			currentBaseState = anim.GetCurrentAnimatorStateInfo(0);	// set our currentState variable to the current state of the Base Layer (0) of animation


			Vector3 curentDir = anim.rootRotation * Vector3.forward;
			Vector3 wantedDir = (target - anim.rootPosition).normalized;

			if(Vector3.Distance(target, anim.rootPosition) > 1f)
			{
				Transform t = GameObject.Find("First Person Controller").transform;
				if (Vector3.Distance(t.position, transform.position) > 3f) {	// Stop moving if the distance from viewer is less than 3 meters
					v = 0.5f;
					anim.SetFloat("Speed", v);
					
					if(Vector3.Dot(curentDir,wantedDir) > 0)
					{
						anim.SetFloat("Direction",Vector3.Cross(curentDir, wantedDir).y);
					}
					else
					{
						anim.SetFloat("Direction", Vector3.Cross(curentDir, wantedDir).y > 0 ? 1 : -1);
					}
				}else {
					wantedDir = (t.position - anim.rootPosition).normalized;
					v = 0f;
					anim.SetFloat("Speed", v);

					if(Vector3.Dot(curentDir,wantedDir) > 0)
					{
						anim.SetFloat("Direction",Vector3.Cross(curentDir, wantedDir).y);
					}
					else
					{
						anim.SetFloat("Direction", Vector3.Cross(curentDir, wantedDir).y > 0 ? 1 : -1);
					}
				}
			}
			else
			{
				target = checkpoints[current];
				current = (current+1) % checkpoints.Count;
			}

			if (currentBaseState.nameHash == TurnState) {

				//if (!anim.IsInTransition(0))
				//	anim.MatchTarget(Vector3.one, targetRotation, AvatarTarget.Root, new MatchTargetWeightMask(Vector3.zero, 1), currentBaseState.normalizedTime, 0.9f);
			}
		}
	}
	
	void OnGUI() {
		//GUI.Label (new Rect(0,0,200,40), checkpoints[current].ToString());
	}
}
