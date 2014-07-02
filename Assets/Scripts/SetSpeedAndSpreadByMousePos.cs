using UnityEngine;
using System.Collections;

public class SetSpeedAndSpreadByMousePos : MonoBehaviour {

	GPUParticlePerlin particlePerlin ;

	// Use this for initialization
	void Start () {
		particlePerlin = GetComponent<GPUParticlePerlin>();
	}
	
	// Update is called once per frame
	void Update () {
		// Vector3でマウス位置座標を取得する
		Vector3 position = Input.mousePosition;

		// Debug.Log ("mouse position" + position);

		float speed = 0.001f + 0.002f * ( position.x / Screen.width -0.5f );
		float spread = 1.0f + 5.0f *  (position.y / Screen.height);

		particlePerlin.speed = speed;
		particlePerlin.spread = spread;
	}
}
