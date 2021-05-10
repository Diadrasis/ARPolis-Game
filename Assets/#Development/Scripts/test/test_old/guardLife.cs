using UnityEngine;
using System.Collections;

// Require these components when using this script
[RequireComponent(typeof (Animator))]
[RequireComponent(typeof (CapsuleCollider))]
[RequireComponent(typeof (Rigidbody))]
public class guardLife : MonoBehaviour
{
	[System.NonSerialized]					
	public float lookWeight;					// the amount to transition when using head look
	
	[System.NonSerialized]
	public Transform enemy;						// a transform to Lerp the camera to during head look
	
	public float animSpeed = 0.5f;				// a public setting for overall animator animation speed
	public float lookSmoother = 3f;				// a smoothing setting for camera motion							
	
	private Animator anim;							// a reference to the animator on the character
	private AnimatorStateInfo currentBaseState;			// a reference to the current state of the animator, used for base layer
	private AnimatorStateInfo layer2CurrentState;	// a reference to the current state of the animator, used for layer 2
	private CapsuleCollider col;					// a reference to the capsule collider of the character
	
	
	static int IdleState = Animator.StringToHash("Base Layer.Idle");
	static int TurnState = Animator.StringToHash("Base Layer.Turn");
	static int WalkForwardState = Animator.StringToHash("Base Layer.WalkForward");
	
	
	void Start ()
	{
		// initialising reference variables
		anim = GetComponent<Animator>();					  
		col = GetComponent<CapsuleCollider>();				
		//enemy = GameObject.Find("Enemy").transform;	
	}

	float v = 0.5f, h = 180f, idleTimer = 0f;
	int dir = 1;
	bool doTurn = false;
	Quaternion targetRotation;

	string s = "";
	void Update ()
	{
		anim.SetFloat("Speed", v);							// set our animator's float parameter 'Speed' equal to the vertical input axis				
		anim.SetFloat("Direction", h); 						// set our animator's float parameter 'Direction' equal to the horizontal input axis		
		anim.SetBool("Turn", doTurn);
		anim.speed = animSpeed;								// set the speed of our animator to the public variable 'animSpeed'
		anim.SetLookAtWeight(lookWeight);					// set the Look At Weight - amount to use look at IK vs using the head's animation
		currentBaseState = anim.GetCurrentAnimatorStateInfo(0);	// set our currentState variable to the current state of the Base Layer (0) of animation

		// Raycast down from the center of the character.. 
		Ray ray = new Ray(transform.position + Vector3.up, -Vector3.up);
		RaycastHit hitInfo = new RaycastHit();
		
		if (Physics.Raycast(ray, out hitInfo))
		{
			if (currentBaseState.nameHash == WalkForwardState) {
				if (hitInfo.collider.name == "rotateCollider") {								
					v = 0f;
					anim.SetFloat("Speed", v);
					doTurn = true;
				}
				//h = 180f;
			}else if (currentBaseState.nameHash == IdleState) {
				if (doTurn) {
					if (!anim.IsInTransition(0)) {
						targetRotation = transform.rotation * Quaternion.AngleAxis(h, Vector3.up);
						doTurn = false;
						h *= -1;
					}
				}else {
					if (idleTimer > 5f) {
						v = 0.5f;
						anim.SetFloat("Speed", v);
						idleTimer = 0f;
					}else {
						idleTimer += Time.deltaTime;
					}
				}
			}else if (currentBaseState.nameHash == TurnState) {
				if (!anim.IsInTransition(0))
					anim.MatchTarget(Vector3.one, targetRotation, AvatarTarget.Root, new MatchTargetWeightMask(Vector3.zero, 1), currentBaseState.normalizedTime, 0.9f);
			}
		}

		/*if (currentBaseState.nameHash == WalkForwardState) {
			if (walkTimer > 7f)
			{
				v = 0f;
				anim.SetFloat("Speed", v);
				walkTimer = 0f;
				doTurn = true;
			}else {
				walkTimer += Time.deltaTime;
			}
		} else if (currentBaseState.nameHash == IdleState) {
			if (doTurn) {
				targetRotation = transform.rotation * Quaternion.AngleAxis(180f, Vector3.up);
				doTurn = false;
			}else {
				if (standTimer > 3f) {
					v = 0.5f;
					anim.SetFloat("Speed", v);
					standTimer = 0f;
				}else {
					standTimer += Time.deltaTime;
				}
			}
		}else if (currentBaseState.nameHash == TurnState) {
			anim.MatchTarget(Vector3.one, targetRotation, AvatarTarget.Root, new MatchTargetWeightMask(Vector3.zero, 1), currentBaseState.normalizedTime, 0.9f);			
		}*/
	}

	void OnGUI() {
		GUI.Label (new Rect(0,0,200,15), s);
	}
}
