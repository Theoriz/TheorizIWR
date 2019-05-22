using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureBlender : MonoBehaviour
{

	public ComputeShader computeShader;
	public Material outputMaterial;
	public Vector2Int textureSize;
    public int colorMode = 1;
    [Range(0,2)] public int textureSet = 0;
    public bool invertTime = false;

    public RenderTexture maskTexture;

	public List<Texture2D> inputTextures0;
    public List<Texture2D> inputTextures1;
    public List<Texture2D> inputTextures2;

    private RenderTexture outputTexture;
	private int blendKernel;

	private ComputeBuffer computeBuffer;

    private bool initialized = false;

    // Start is called before the first frame update
    void Start()
    {
        initialized = false;
        maskTexture = null;
	}

    // Update is called once per frame
    void Update()
    {
        if (!initialized)
        {
            return;
            //if (!maskTexture)
            //{
            //    Initialize();
            //}
            //else
            //{
            //    return;
            //}
        }

        computeShader.SetInt("ColorMode", colorMode);

        computeShader.Dispatch(blendKernel, textureSize.x / 8, textureSize.y /8, 1);

		outputMaterial.SetTexture("_EmissionMap", outputTexture);
    }

    private void OnValidate()
    {
        if(Application.isPlaying && initialized)
            Initialize();
    }

    private void OnDisable()
    {
        computeBuffer.Release();
    }

    public void Initialize()
    {
        if(computeBuffer != null)
        {
            computeBuffer.Release();
            computeBuffer = null;
        }

        List<Texture2D> inputTextures = new List<Texture2D>();

        switch (textureSet)
        {
            case 0:
                inputTextures = inputTextures0;
                break;
            case 1:
                inputTextures = inputTextures1;
                break;
            case 2:
                inputTextures = inputTextures2;
                break;
        }

        if (inputTextures.Count <= 0)
            return;

        // Create output rendertexture
        outputTexture = new RenderTexture(textureSize.x, textureSize.y, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
        outputTexture.enableRandomWrite = true;
        outputTexture.filterMode = FilterMode.Bilinear;
        outputTexture.wrapMode = TextureWrapMode.Mirror;
        outputTexture.Create();

        //Create texture array buffer
        computeBuffer = new ComputeBuffer(textureSize.x * textureSize.y * inputTextures.Count, 4);

        //Fill texture array buffer
        for (int i = 0; i < inputTextures.Count; i++)
        {
            computeBuffer.SetData(Texture2DToIntArray(inputTextures[i]), 0, textureSize.x * textureSize.y * i, textureSize.x * textureSize.y);
        }

        //Get kernel
        blendKernel = computeShader.FindKernel("Blender");

        //Bind buffers and textures
        computeShader.SetInt("TextureWidth", textureSize.x);
        computeShader.SetInt("TextureHeight", textureSize.y);
        computeShader.SetInt("TextureCount", inputTextures.Count);
        computeShader.SetBool("InvertTime", invertTime);

        computeShader.SetBuffer(blendKernel, "TextureArray", computeBuffer);
        computeShader.SetTexture(blendKernel, "MaskTexture", maskTexture);
        computeShader.SetTexture(blendKernel, "Result", outputTexture);

        //Debug.Log("initialized " + gameObject.name);

        initialized = true;
    }

    int[] Texture2DToIntArray(Texture2D tex)
    {
        int[] intArray = new int[textureSize.y * textureSize.x];
        byte[] raw = tex.GetRawTextureData();
        for (int y = 0; y < textureSize.y; y++)
        {
            for (int x = 0; x < textureSize.x; x++)
            {
                int index = (y * textureSize.x + x) * 3;
                intArray[y * textureSize.x + x] = raw[index] + (raw[index + 1] << 8) + (raw[index + 2] << 16);
            }
        }
        return intArray;
    }

    public int GetTextureCount()
    {
        switch (textureSet)
        {
            case 0:
                return inputTextures0.Count;
            case 1:
                return inputTextures1.Count;
            case 2:
                return inputTextures2.Count;
            default:
                return 0;
        }
    }
}
