using UnityEngine;
using System.Collections;

public class SetOrthograficCameraSize : MonoBehaviour {

	GameObject oCamera ;
	void Awake () {
		oCamera = transform.gameObject;
		oCamera.camera.orthographicSize = Screen.height / 2;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
