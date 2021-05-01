using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
[ImageEffectAllowedInSceneView]
public class RaidalVolumeLight : MonoBehaviour
{
    [Range(0,1)]
    public float threshold = 0;

    [Range(0.0f, 5.0f)]
    public float lightRadius = 2.0f;
    //提取高亮结果Pow倍率，适当降低颜色过亮的情况
    [Range(1.0f, 4.0f)]
    public float lightPowFactor = 3.0f; 
    [Range(4,16)]
    public int Iteration = 4;


    public Light light;
    public Shader volumtricLightShader;
    private Material _mat;
    private Material mat
    {
        get
        {
            if (_mat == null && volumtricLightShader != null)
            {
                _mat = new Material(volumtricLightShader);
            }
            return _mat;
        }
    }

    private Camera _cam;
    private Camera cam
    {
        get {
            if (!_cam)
                _cam = GetComponent<Camera>();
            return _cam;
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if(mat == null)
        {
            Graphics.Blit(source, destination);
        }

        Vector3 LightViewportPos = new Vector3(0.5f, 0.5f,0f);
        if(light != null)
        {
            LightViewportPos = cam.WorldToViewportPoint(light.transform.position);
        }


        mat.SetVector("_BlurParams", new Vector4(threshold,  Iteration,0,0));
        mat.SetVector("_LightParams", new Vector4(LightViewportPos.x, LightViewportPos.y, LightViewportPos.z, 0));
        mat.SetFloat("_LightRadius", lightRadius);
        mat.SetFloat("_PowFactor", lightPowFactor);


    }

}
