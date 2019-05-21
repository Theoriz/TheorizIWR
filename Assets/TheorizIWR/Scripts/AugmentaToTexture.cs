using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AugmentaToTexture : MonoBehaviour
{

	public ComputeShader computeShader;

	public AugmentaAreaAnchor augmentaAreaAnchor;

	public float radius = 1;
	public int maxPointsCount = 50;

	private RenderTexture maskTexture;
	private int augmentaToTextureKernel;

	private Vector4[] augmentaPoints;

	private bool initialized = false;

    // Start is called before the first frame update
    void Start()
    {
		initialized = false;
	}

    // Update is called once per frame
    void Update()
    {
		if (!initialized) {
			if (augmentaAreaAnchor.linkedAugmentaArea.AugmentaScene.Width >= 0 && augmentaAreaAnchor.linkedAugmentaArea.AugmentaScene.Height >= 0) {
				Initialize();
			} else {
				return;
			}
		}


		//Update the augmentaPoints array
		for(int i=0; i<augmentaAreaAnchor.InstantiatedObjects.Count; i++) {

			augmentaPoints[i].x = augmentaArea.AugmentaPeople[augmentaAreaAnchor.InstantiatedObjects[i].Key].Position.x;
			augmentaPoints[i].y = augmentaArea.AugmentaPeople[i].Position.y;
			augmentaPoints[i].z = augmentaArea.AugmentaPeople[i].boundingRect.width;
			augmentaPoints[i].w = augmentaArea.AugmentaPeople[i].boundingRect.height;
			Debug.Log("Augmenta person " + i + ": " + augmentaPoints[i]);
		}

		//Bind the augmenta points array
		computeShader.SetVectorArray("AugmentaPoints", augmentaPoints);
		computeShader.SetInt("PointsCount", augmentaArea.arrayPersonCount());

    }

	void Initialize() {

		// Create mask rendertexture
		maskTexture = new RenderTexture((int)augmentaArea.AugmentaScene.Width, (int)augmentaArea.AugmentaScene.Height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.sRGB);
		maskTexture.enableRandomWrite = true;
		maskTexture.filterMode = FilterMode.Bilinear;
		maskTexture.wrapMode = TextureWrapMode.Mirror;
		maskTexture.Create();

		//Get kernel index
		augmentaToTextureKernel = computeShader.FindKernel("AugmentaToTexture");

		//Create augmentaPoints vector
		augmentaPoints = new Vector4[maxPointsCount];

		initialized = true;
	}
}
