using UnityEngine;
using VRTK;
using System.Collections;

public class CornerGrabber : MonoBehaviour {
	private bool isGrabbedNow = false;

	void Start ()

	{
		if (GetComponent<VRTK_InteractableObject>() == null)
		{
			Debug.LogError("Team3_Interactable_Object_Extension is required to be attached to an Object that has the VRTK_InteractableObject script attached to it");
			return;
		}

		GetComponent<VRTK_InteractableObject>().InteractableObjectGrabbed += new InteractableObjectEventHandler(ObjectGrabbed);
		GetComponent<VRTK_InteractableObject> ().InteractableObjectUngrabbed += new InteractableObjectEventHandler (ObjectDropped);
	}

	public void ObjectGrabbed(object sender, InteractableObjectEventArgs e)

	{
		this.isGrabbedNow = true;
	}

	public void ObjectDropped(object sender, InteractableObjectEventArgs e)
	{
		this.isGrabbedNow = false;
	}

	public bool isThisGrabbed
	{
		get
		{
			return this.isGrabbedNow;
		}
	}

}