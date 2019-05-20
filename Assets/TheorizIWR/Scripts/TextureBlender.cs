﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureBlender : MonoBehaviour
{

	public ComputeShader computeShader;
	public Material outputMaterial;
	public Vector2Int textureSize;

	public Texture2D inputTexture0;

	private RenderTexture outputTexture;
	private int blendKernel;

    // Start is called before the first frame update
    void Start()
    {
		
		//
		outputTexture = new RenderTexture(textureSize.x, textureSize.y, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.sRGB);
		outputTexture.enableRandomWrite = true;
		outputTexture.filterMode = FilterMode.Bilinear;
		outputTexture.wrapMode = TextureWrapMode.Mirror;
		outputTexture.Create();

		blendKernel = computeShader.FindKernel("CSMain");
		computeShader.SetTexture(blendKernel, "InputTex0", inputTexture0);
		computeShader.SetTexture(blendKernel, "Result", outputTexture);
	}

    // Update is called once per frame
    void Update()
    {

		computeShader.Dispatch(blendKernel, textureSize.x / 14, textureSize.y /14, 1);

		outputMaterial.SetTexture("_MainTex", outputTexture);
    }


}
