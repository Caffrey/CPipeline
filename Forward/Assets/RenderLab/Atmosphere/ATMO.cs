using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATMO : MonoBehaviour
{
    public Light Sun;
    public Material _ppMaterial;


    public int ScaterringSampleCount = 8;
    public int OpticalSampleCount = 8;

    private const float AtmosphereHeight = 80000.0f;
    private const float PlanetRadius = 6371000.0f; 
    private readonly Vector4 DensityScale = new Vector4(7994.0f, 1200.0f, 0, 0);
    
    [ColorUsage(true,true)]
    public Color RS = new Vector4(5.8f, 13.5f, 33.1f, 0.0f);
    [ColorUsage(true, true)]
    public Color MS = new Vector4(2.0f, 2.0f, 2.0f, 0.0f);
    private readonly Vector4 RayleighSct = new Vector4(5.8f, 13.5f, 33.1f, 0.0f) * 0.000001f;
    private readonly Vector4 MieSct = new Vector4(2.0f, 2.0f, 2.0f, 0.0f) * 0.00001f;

    public Color IncomingLight = new Color(4, 4, 4, 4);
    [Range(0, 10.0f)]
    public float RayleighScatterCoef = 1;
    [Range(0, 10.0f)]
    public float RayleighExtinctionCoef = 1;
    [Range(0, 10.0f)]
    public float MieScatterCoef = 1;
    [Range(0, 10.0f)]
    public float MieExtinctionCoef = 1;
    [Range(0.0f, 0.999f)]
    public float MieG = 0.76f;
    [HideInInspector]
    public float DistanceScale = 1;
    public float SunIntensity = 1;
    private void Awake()
    {
        _ppMaterial = GetComponent<MeshRenderer>().sharedMaterial;
    }

    private void OnEnable()
    {
        _ppMaterial = GetComponent<MeshRenderer>().sharedMaterial;
    }

    private void OnValidate()
    {
        UpdateMaterial();
    }


    public void UpdateMaterial()
    {
        _ppMaterial.SetInt("_ScaterringSampleCount", ScaterringSampleCount);
        _ppMaterial.SetInt("_OpticalSampleCount", OpticalSampleCount);

        _ppMaterial.SetFloat("_AtmosphereHeight", AtmosphereHeight);
        _ppMaterial.SetFloat("_PlanetRadius", PlanetRadius);
        _ppMaterial.SetVector("_DensityScaleHeight", DensityScale);

        Vector4 scatteringR = RS * 0.000001f;
        Vector4 scatteringM = MS * 0.00001f;
        _ppMaterial.SetVector("_ScatteringR", RayleighSct * RayleighScatterCoef);
        _ppMaterial.SetVector("_ScatteringM", MieSct * MieScatterCoef);
        _ppMaterial.SetVector("_ExtinctionR", RayleighSct * RayleighExtinctionCoef);
        _ppMaterial.SetVector("_ExtinctionM", MieSct * MieExtinctionCoef);

        _ppMaterial.SetColor("_IncomingLight", IncomingLight);
        _ppMaterial.SetFloat("_MieG", MieG);
        _ppMaterial.SetFloat("_DistanceScale", DistanceScale);

        _ppMaterial.SetFloat("_SunIntensity", SunIntensity);
    }
}
