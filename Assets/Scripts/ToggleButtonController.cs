using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleButtonController : MonoBehaviour {
	public bool isPrimary = true;

	public GameObject primaryIcon;
	public GameObject secondaryIcon;

	void Start() {
		UpdateState(isPrimary);
	}

	public void ToggleState() {
		UpdateState(!isPrimary);
	}

	public void UpdateState(bool isPrimary) {
		this.isPrimary = isPrimary;
		if(primaryIcon != null) {
			primaryIcon.SetActive(isPrimary);
		}
		if(secondaryIcon != null) {
			secondaryIcon.SetActive(!isPrimary);
		}
	}
}
