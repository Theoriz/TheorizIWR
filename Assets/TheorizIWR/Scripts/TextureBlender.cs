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

	private ComputeBuffer computeBuffer;

    // Start is called before the first frame update
    void Start()
    {
		
		// Create output rendertexture
		outputTexture = new RenderTexture(textureSize.x, textureSize.y, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
		outputTexture.enableRandomWrite = true;
		outputTexture.filterMode = FilterMode.Bilinear;
		outputTexture.wrapMode = TextureWrapMode.Mirror;
		outputTexture.Create();

		//Create texture array buffer
		computeBuffer = new ComputeBuffer(textureSize.x * textureSize.y * inputTextures.Count, 4);

		//Fill texture array buffer
		for(int i=0; i<inputTextures.Count; i++) {
			computeBuffer.SetData(inputTextures[i].GetRawTextureData(), 0, textureSize.x * textureSize.y * i, textureSize.x * textureSize.y);
		}

		//Get kernel
		blendKernel = computeShader.FindKernel("Blender");

		//Bind buffers and textures
		computeShader.SetInt("TextureWidth", textureSize.x);
		computeShader.SetInt("TextureHeight", textureSize.y);

		computeShader.SetBuffer(blendKernel, "TextureArray", computeBuffer);
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
