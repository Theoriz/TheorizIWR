using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AugmentaToTexture : MonoBehaviour
{

	public ComputeShader computeShader;

	public AugmentaAreaAnchor augmentaAreaAnchor;

    public float evolutionRadius = 0.1f;
    public float innerRadius = 0.01f;
    public float devolutionSpeed = 0.2f;

    public int maxPointsCount = 50;

	public Vector3 rayCastDirection = Vector3.down;

	public TextureBlender textureBlender;

	public RenderTexture maskTexture;

	private int augmentaToTextureKernel;

	private Vector4[] augmentaPoints;

	private bool initialized = false;
	private RaycastHit hit;

	private Vector3Int threadsConfiguration;

	private ComputeBuffer augmentaPointsBuffer;

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
		UpdateAugmentaPoints();

		//Bind the augmenta points array
		computeShader.SetBuffer(augmentaToTextureKernel, "AugmentaPoints", augmentaPointsBuffer);
		computeShader.SetInt("PointsCount", augmentaAreaAnchor.InstantiatedObjects.Count);
        computeShader.SetFloat("DeltaTime", Time.deltaTime);

        //Run the kernel
        computeShader.Dispatch(augmentaToTextureKernel, threadsConfiguration.x, threadsConfiguration.y, threadsConfiguration.z);

    }

    private void OnValidate()
    {
        if (Application.isPlaying && initialized)
        {
            computeShader.SetBool("InvertTime", textureBlender.invertTime);
            computeShader.SetFloat("EvolutionRadius", evolutionRadius);
            computeShader.SetFloat("InnerRadius", innerRadius);
            computeShader.SetFloat("DevolutionSpeed", devolutionSpeed);
        }
    }

    private void OnDisable() {

		//Release buffer
		augmentaPointsBuffer.Release();
	}

	void Initialize() {

        // Create mask rendertexture
        //maskTexture = new RenderTexture((int)augmentaAreaAnchor.linkedAugmentaArea.AugmentaScene.Width, (int)augmentaAreaAnchor.linkedAugmentaArea.AugmentaScene.Height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.sRGB);
        maskTexture = new RenderTexture((int)textureBlender.textureSize.x, (int)textureBlender.textureSize.y, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.sRGB);
        maskTexture.enableRandomWrite = true;
		maskTexture.filterMode = FilterMode.Bilinear;
		maskTexture.wrapMode = TextureWrapMode.Mirror;
		maskTexture.Create();

		//Set the mask texture of textureblender
		textureBlender.maskTexture = maskTexture;
        textureBlender.Initialize();

		//Create compute buffer
		augmentaPointsBuffer = new ComputeBuffer(maxPointsCount, 4 * sizeof(float));

		//Get kernel index
		augmentaToTextureKernel = computeShader.FindKernel("AugmentaToTexture");

		//Bind texture size
		computeShader.SetInt("TextureWidth", maskTexture.width);
		computeShader.SetInt("TextureHeight", maskTexture.height);
        computeShader.SetInt("TextureCount", textureBlender.GetTextureCount());
        computeShader.SetBool("InvertTime", textureBlender.invertTime);
        computeShader.SetFloat("EvolutionRadius", evolutionRadius);
        computeShader.SetFloat("InnerRadius", innerRadius);
        computeShader.SetFloat("DevolutionSpeed", devolutionSpeed);

        //Bind output texture
        computeShader.SetTexture(augmentaToTextureKernel, "Result", maskTexture);

		//Bind buffer
		computeShader.SetBuffer(augmentaToTextureKernel, "AugmentaPoints", augmentaPointsBuffer);

		//Create augmentaPoints vector
		augmentaPoints = new Vector4[maxPointsCount];

		threadsConfiguration.x = Mathf.CeilToInt(maskTexture.width / 8.0f);
		threadsConfiguration.y = Mathf.CeilToInt(maskTexture.height / 8.0f);
		threadsConfiguration.z = 1;

		initialized = true;
	}

	void UpdateAugmentaPoints() {

        int i = 0;
        //for (int i = 0; i < augmentaAreaAnchor.InstantiatedObjects.Count; i++) {
        foreach (var element in augmentaAreaAnchor.InstantiatedObjects) { 

            Physics.Raycast(element.Value.transform.position, rayCastDirection, out hit, 5);

			augmentaPoints[i].x = hit.textureCoord.x;
			augmentaPoints[i].y = hit.textureCoord.y;
			augmentaPoints[i].z = 0;
			augmentaPoints[i].w = 0;
			//Debug.Log("Augmenta person " + i + ": " + augmentaPoints[i]);

            i++;
		}

		augmentaPointsBuffer.SetData(augmentaPoints);
	}
}
