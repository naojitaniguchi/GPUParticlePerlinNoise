using UnityEngine;
using System.Collections;

public class GPUParticlePerlin : MonoBehaviour {

	public ComputeShader cs;
	private RenderTexture destTexture;
	private RenderTexture positionTexture0;
	private RenderTexture positionTexture1;
	private RenderTexture velocityTexture;
	private Texture2D noiseTexture;
	public float speed = 0.001f ;
	public float spread = 1.0f ;
	public float xOrg = 0.0f ;
	public float yOrg = 0.0f ;
	public float scale = 10.0F;
	public float upVelocity = 0.5f ;

	int posTexWidth = 2000 ;
	int posTexheight = 2000 ;

	bool zero2one = true ;

	// http://docs.unity3d.com/ScriptReference/Mathf.PerlinNoise.html

	void createBaseNoiseValue(float[] noiseArray){
		float y = 0.0F;
		float x ;
		
		while (y < posTexheight) {
			x = 0.0F;
			while (x < posTexWidth) {
				float xCoord = xOrg + ( x / posTexWidth ) * scale ;
				float yCoord = yOrg + ( y / posTexheight ) * scale ;
				float sample = Mathf.PerlinNoise(xCoord, yCoord);
				noiseArray[(int)(y * posTexWidth + x)] = sample * 0.5f + 0.5f  ;
				x++;
			}
			y++;
		}
	}

	void createNoiseValue(float freq, float[] noiseArray){
		float y = 0.0F;
		float x ;
		
		while (y < posTexheight) {
			x = 0.0F;
			while (x < posTexWidth) {
				float xCoord = xOrg + ( x / posTexWidth ) * scale * freq ;
				float yCoord = yOrg + ( y / posTexheight ) * scale * freq ;
				float sample = Mathf.PerlinNoise(xCoord, yCoord);
				noiseArray[(int)(y * posTexWidth + x)] += ( sample - 0.5f ) / freq  ;
				x++;
			}
			y++;
		}
	}

	void createNoiseTexture(){
		float[] noiseArray = new float[posTexWidth * posTexheight];

		createBaseNoiseValue(noiseArray);
		createNoiseValue( 2.0f, noiseArray);
		createNoiseValue( 4.0f, noiseArray);
		createNoiseValue( 8.0f, noiseArray);
		createNoiseValue( 16.0f, noiseArray);


		noiseTexture = new Texture2D( posTexWidth, posTexheight, TextureFormat.RGB24, false );

		Color[] noise = new Color[posTexWidth * posTexheight];

		float y = 0.0F;
		float x ;

		while (y < posTexheight) {
			x = 0.0F;
			while (x < posTexWidth) {
				float sample = noiseArray[(int)(y * posTexWidth + x)];
				noise[(int)(y * posTexWidth + x)] =  new Color(sample, sample, sample);
				x++;
			}
			y++;
		}

		noiseTexture.SetPixels(noise);
		noiseTexture.Apply();

	}

	void calcNoiseVelocity(){
		cs.SetFloat("posTexWidth", (float)posTexWidth );
		cs.SetFloat("posTexHeight", (float)posTexheight );
		cs.SetFloat("spread", spread);

		cs.SetTexture (0, "NoiseTexture", noiseTexture);
		cs.SetTexture (0, "Result", velocityTexture);
		cs.Dispatch (0, posTexWidth / 8, posTexheight / 8, 1);

	}

	void initPosition(){
		// CSInitPosition
		cs.SetFloat ("posTexWidth", (float)posTexWidth);
		cs.SetFloat ("posTexHeight", (float)posTexheight);
	
		cs.SetTexture (2, "Result", positionTexture0);
		cs.Dispatch (2, posTexWidth / 8, posTexheight / 8, 1);
		cs.SetTexture (2, "Result", positionTexture1);
		cs.Dispatch (2, posTexWidth / 8, posTexheight / 8, 1);
	}

	
	// Use this for initialization
	void Start () {
		destTexture = new RenderTexture( Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear );
		destTexture.enableRandomWrite = true;
		destTexture.antiAliasing = 8;
		destTexture.Create();
		
		positionTexture0 = new RenderTexture( posTexWidth, posTexheight, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear );
		positionTexture0.enableRandomWrite = true;
		positionTexture0.Create();
		
		positionTexture1 = new RenderTexture( posTexWidth, posTexheight, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear );
		positionTexture1.enableRandomWrite = true;
		positionTexture1.Create();
		
		velocityTexture = new RenderTexture( posTexWidth, posTexheight, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear );
		velocityTexture.enableRandomWrite = true;
		velocityTexture.Create();

		createNoiseTexture ();

		calcNoiseVelocity();

		initPosition();
	}
	
	// Update is called once per frame
	void Update () {
		cs.SetFloat("ScreenWidth", (float)Screen.width );
		cs.SetFloat("ScreenHeight", (float)Screen.height );
		cs.SetFloat("posTexWidth", (float)posTexWidth );
		cs.SetFloat("posTexHeight", (float)posTexheight );
		cs.SetFloat("deltaTime", Time.deltaTime);
		cs.SetFloat("speed", speed);
		cs.SetFloat("spread", spread);
		cs.SetFloat("upVelocity", upVelocity);


		calcNoiseVelocity();

		// CSClearWhite
		cs.SetTexture (1, "Result", destTexture);
		cs.Dispatch (1, destTexture.width / 8, destTexture.height / 8, 1);
		
		if (zero2one) {
			// CSCalcPosition
			cs.SetTexture (3, "Position", positionTexture0);
			cs.SetTexture (3, "Velocity", velocityTexture);
			cs.SetTexture (3, "Result", positionTexture1);
			cs.Dispatch (3, posTexWidth / 8, posTexheight / 8, 1);

			// CSDrawParticle
			cs.SetTexture (4, "Position", positionTexture1);
			cs.SetTexture (4, "Result", destTexture);
			cs.Dispatch (4, posTexWidth / 8, posTexheight / 8, 1);

			zero2one = false;

			renderer.material.mainTexture = positionTexture1;
		} else {
			// CSCalcPosition
			cs.SetTexture (3, "Position", positionTexture1);
			cs.SetTexture (3, "Velocity", velocityTexture);
			cs.SetTexture (3, "Result", positionTexture0);
			cs.Dispatch (3, posTexWidth / 8, posTexheight / 8, 1);

			// CSDrawParticle
			cs.SetTexture (4, "Position", positionTexture0);
			cs.SetTexture (4, "Result", destTexture);
			cs.Dispatch (4, posTexWidth / 8, posTexheight / 8, 1);

			zero2one = true;

			renderer.material.mainTexture = positionTexture0;
		}
		
		renderer.material.mainTexture = destTexture;


		//renderer.material.mainTexture = noiseTexture;
	}
}
