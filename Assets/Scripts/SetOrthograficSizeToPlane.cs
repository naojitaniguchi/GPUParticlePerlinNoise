using UnityEngine;
using System.Collections;

public class SetOrthograficSizeToPlane : MonoBehaviour {

	void Awake () {
		Vector3 scaleValue;
		Vector3 posValue;

		scaleValue.x = Screen.width / 10;
		scaleValue.y = 1.0f;
		scaleValue.z = Screen.height / 10;

		transform.localScale = scaleValue ;

		posValue.x = 0.0f;
		posValue.y = 0.0f;
		posValue.z = 0.0f;

		transform.position = posValue;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
