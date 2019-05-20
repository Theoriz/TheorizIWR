using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureBlender : MonoBehaviour
{

	public ComputeShader computeShader;
	public Material outputMaterial;
	public Vector2Int textureSize;

	public Texture2D maskTexture;

	public List<Texture2D> inputTextures;

	private RenderTexture outputTexture;
	private int blendKernel;

    // Start is called before the first frame update
    void Start()
    {
		
		// Create output rendertexture
		outputTexture = new RenderTexture(textureSize.x, textureSize.y, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.sRGB);
		outputTexture.enableRandomWrite = true;
		outputTexture.filterMode = FilterMode.Bilinear;
		outputTexture.wrapMode = TextureWrapMode.Mirror;
		outputTexture.Create();

		//Get kernel
		blendKernel = computeShader.FindKernel("Blender");

		//Bind textures
		computeShader.SetTexture(blendKernel, "MaskTexture", maskTexture);
		computeShader.SetTexture(blendKernel, "InputTex0", inputTextures[0]);
		computeShader.SetTexture(blendKernel, "Result", outputTexture);
	}

    // Update is called once per frame
    void Update()
    {

		computeShader.Dispatch(blendKernel, textureSize.x / 8, textureSize.y /8, 1);

		outputMaterial.SetTexture("_MainTex", outputTexture);
    }


}
