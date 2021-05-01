using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


[ExecuteInEditMode]
public class ComputeRadianus : MonoBehaviour
{
    public ComputeShader ComputeShader;
    public RenderTexture ResultTex;
    public Texture2D TargetTex;
    public Cubemap paramap;

    void Start()
    {
        Convolune();
    }

    public void Convolune()
    {
        
        ResultTex = new RenderTexture(1024, 512, 0, RenderTextureFormat.DefaultHDR) ;
        ResultTex.enableRandomWrite = true;

        int Kernel = ComputeShader.FindKernel("CSMain");
        ComputeShader.SetTexture(Kernel, "Result", ResultTex);
        ComputeShader.SetTexture(Kernel, "radianus", paramap);

       
        ComputeShader.Dispatch(Kernel, ResultTex.width / 8, ResultTex.height / 8, 1);
        TargetTex = new Texture2D(1024, 512, TextureFormat.RGBAFloat,false,true);
        RenderTexture.active = ResultTex;

        TargetTex.ReadPixels(new Rect(0, 0, 1024, 512),0,0);
        RenderTexture.active = null;
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
