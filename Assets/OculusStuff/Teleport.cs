using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Teleport : MonoBehaviour {

	RaycastHit hitT; //hit for teleport
	RaycastHit hitL; //hit for light
	bool _validTarget = false;
	bool _aborted = false;
	bool _showIndicator = true;
	bool _showPad = false;
	bool _showInvalidPad = false;
	Vector3 targetPosition = Vector3.zero;


	public Transform eye;
	public GameObject pad;
	public GameObject invalid_pad;
	public GameObject indicator;
	public LayerMask collisionLayers;
	public float maxTeleportDistance = 30f;
	public Light light;
	public float maxIntensity = 5f;
	public float maxLightRange = 150f;
	public float rangeSpeed = 10f;


	public Material mat;


	

	void Update () {
		
		/*************************************************/
		/***********      AUTO ROTATION       ************/
		/*************************************************/
		if (OVRInput.GetUp (OVRInput.Button.DpadLeft))
			transform.Rotate (new Vector3 (0, -20, 0));
		else if (OVRInput.GetUp (OVRInput.Button.DpadRight))
			transform.Rotate (new Vector3 (0, 20, 0));


		/*************************************************/
		/********      TELEPORT & INDICATOR       ********/
		/*************************************************/

		if (OVRInput.Get (OVRInput.Button.One)) {
			_validTarget = false;
			_showPad = false;
			_showInvalidPad = false;
			_showIndicator = true;
			_aborted = true;
		}
			
		if (_aborted && (OVRInput.GetUp (OVRInput.Button.PrimaryIndexTrigger) || OVRInput.GetUp (OVRInput.Button.DpadDown)) )
			_aborted = false;
		
		if (!_aborted && (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) || OVRInput.Get(OVRInput.Button.DpadDown))) {			
			if (Physics.Raycast (eye.position, eye.forward, out hitT, maxTeleportDistance)) {
				if (hitT.collider == null) {
					_showPad = false;
					_showInvalidPad = false;
					_showIndicator = true;
					_validTarget = false;
				} else {
					Vector3 p = hitT.point;
					Vector3 n = hitT.normal;
					if (hitT.collider.tag == "Walkable") {						
						pad.transform.position = p;
						pad.transform.rotation = Quaternion.FromToRotation (Vector3.up, n);
						targetPosition = p;
						_validTarget = true;
						_showPad = true;
						_showInvalidPad = false;
						_showIndicator = false;
					} else {
						invalid_pad.transform.position = p;
						invalid_pad.transform.rotation = Quaternion.FromToRotation (Vector3.up, n);
						_validTarget = false;
						_showPad = false;
						_showInvalidPad = true;
						_showIndicator = false;
					}
				}
			} else {
				_validTarget = false;
				_showPad = false;
				_showInvalidPad = false;
				_showIndicator = true;
			}
		}
			

		if ((OVRInput.GetUp (OVRInput.Button.PrimaryIndexTrigger) || OVRInput.GetUp (OVRInput.Button.DpadDown))) {
			if (_validTarget) {
				transform.position = targetPosition;
				_validTarget = false;
				_showPad = false;
				_showInvalidPad = false;
			} else {
				_showPad = false;
				_showInvalidPad = false;
			}
			_showIndicator = true;
		}

		indicator.SetActive (!OVRInput.Get(OVRInput.Button.Four) && _showIndicator);
		pad.SetActive (_showPad);
		invalid_pad.SetActive (_showInvalidPad);



		/*************************************************/
		/*************        LIGHT        ***************/
		/*************************************************/

		light.intensity = OVRInput.Get (OVRInput.Axis1D.SecondaryIndexTrigger, OVRInput.Controller.Gamepad) * maxIntensity;
		if (Physics.Raycast (eye.position, eye.forward, out hitL, maxLightRange)) {
			light.transform.LookAt (hitL.point);
			Vector3 ea = light.transform.eulerAngles;
			light.transform.eulerAngles = new Vector3 (ea.x, eye.transform.eulerAngles.y, eye.transform.eulerAngles.z);
			float dist = hitL.distance;
			light.range = Mathf.MoveTowards(light.range, dist + 10, rangeSpeed * Time.deltaTime * dist * 0.8f);
		}

	}

		

} 
