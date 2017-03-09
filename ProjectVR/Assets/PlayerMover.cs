using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour {

	public SteamVR_TrackedObject leftHand;
	public SteamVR_TrackedObject rightHand;

	public CornerGrabber leftCornerGrab;
	public CornerGrabber rightCornerGrab;

	private Rigidbody rb;
	private Vector3 rightHandOriginalPosition;
	private Vector3 leftHandOriginalPosition;
	private float balance;

	public float XSensitivity = 15.0f;
	public float liftModifier = 5.0f;

	public bool smooth;
	public float smoothTime = 5.0f;

	private bool rightGripDown = false;
	private bool leftGripDown = false;

	private Quaternion characterTargetRot;
	[HideInInspector] public float CurrentTargetSpeed = 8f;
	public float ForwardSpeed = 8.0f;   // Speed when walking forward
	public float BackwardSpeed = 4.0f;  // Speed when walking backwards
	public float StrafeSpeed = 4.0f;    // Speed when walking sideways
	public float runModifier = 1.5f; 

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		CornerGrabber leftCornerGrab = gameObject.GetComponent<CornerGrabber> ();
		CornerGrabber rightCornerGrab = gameObject.GetComponent<CornerGrabber> ();
		Init (transform);
	}

	public void Init(Transform character)
	{
		characterTargetRot = character.localRotation;
	}

	public void LookRotation(Transform character)
	{
		float yRot = balance * XSensitivity;
		characterTargetRot *= Quaternion.Euler (0f, yRot, 0f);
		if(smooth)
		{
			character.localRotation = Quaternion.Slerp (character.localRotation, characterTargetRot,
				smoothTime * Time.deltaTime);
		}
		else
		{
			character.localRotation = characterTargetRot;
		}
	}

	void FixedUpdate () {
		// Steam Controller setup
		if (SteamVR_Controller.Input ((int)leftHand.index) == null)
			Debug.LogWarning ("Left Controller not connected");
		if (SteamVR_Controller.Input ((int)rightHand.index) == null)
			Debug.LogWarning ("Right Controller not connected");
		var lDevice = SteamVR_Controller.Input ((int)leftHand.index);
		var rDevice = SteamVR_Controller.Input ((int)rightHand.index);

		// Get position of controllers when grip is pushed.
		if (rDevice.GetPress (SteamVR_Controller.ButtonMask.Grip)) {
			if (!rightGripDown)
				rightHandOriginalPosition = rightHand.transform.localPosition;
			rightGripDown = true;
		} else {
			rightGripDown = false;
		}
		if (lDevice.GetPress (SteamVR_Controller.ButtonMask.Grip)) {
			if (!leftGripDown)
				leftHandOriginalPosition = leftHand.transform.localPosition;
			leftGripDown = true;
		} else {
			leftGripDown = false;
		}
		float handsAvg = (leftHandOriginalPosition.y + rightHandOriginalPosition.y) / 2;
		float handsCurrentAvg = (leftHand.transform.localPosition.y + rightHand.transform.localPosition.y) / 2;
		float liftForce = (handsCurrentAvg-0.1f - handsAvg) * liftModifier;
		balance = leftHand.transform.localPosition.y - rightHand.transform.localPosition.y;
		Vector2 input = GetInput();
		Debug.Log ("input X: " + input.x + " input Y: " + input.y);
		if (leftCornerGrab.isThisGrabbed && rightCornerGrab.isThisGrabbed) {
			LookRotation (transform);
			Vector3 desiredMove = gameObject.transform.forward*input.y + gameObject.transform.right*input.x + gameObject.transform.up*liftForce;

			desiredMove.x = desiredMove.x*CurrentTargetSpeed;
			desiredMove.z = desiredMove.z*CurrentTargetSpeed;
			desiredMove.y = desiredMove.y*CurrentTargetSpeed;

			if (rb.velocity.sqrMagnitude <
				(CurrentTargetSpeed*CurrentTargetSpeed))
			{
				rb.AddForce(desiredMove, ForceMode.Impulse);
			}


//			rb.velocity = new Vector3 (0f, liftForce,0f);
		}
	}

	private Vector2 GetInput()
	{
		Vector2 input = new Vector2
		{
			x = Input.GetAxis("Horizontal"),
			y = Input.GetAxis("Vertical")
		};
		/*if (Input.GetKeyDown (KeyCode.LeftShift)) {
			input.x = input.x * runModifier;
			input.y = input.y * runModifier;
		}*/
		UpdateDesiredTargetSpeed(input);
		return input;
	}

	public void UpdateDesiredTargetSpeed(Vector2 input)
	{
		if (input == Vector2.zero)
			return;
		if (input.x > 0 || input.x < 0) {
			//strafe
			CurrentTargetSpeed = StrafeSpeed;
		}
		if (input.y < 0) {
			//backwards
			CurrentTargetSpeed = BackwardSpeed;
		}
		if (input.y > 0) {
			//forwards
			//handled last as if strafing and moving forward at the same time forwards speed should take precedence
			CurrentTargetSpeed = ForwardSpeed;
		}
	}
}
