using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using System;
using System.IO;

#if UNITY_EDITOR
    using UnityEditor;
#endif

public class RenderTextureToTexture2D : MonoBehaviour
{

    public static Texture2D Convert(RenderTexture rt)
    {
        TextureFormat format;

        switch (rt.format)
        {
            case RenderTextureFormat.ARGBFloat:
                format = TextureFormat.RGBAFloat;
                break;
            case RenderTextureFormat.ARGBHalf:
                format = TextureFormat.RGBAHalf;
                break;
            case RenderTextureFormat.ARGBInt:
                format = TextureFormat.RGBA32;
                break;
            case RenderTextureFormat.ARGB32:
                format = TextureFormat.ARGB32;
                break;
            default:
                format = TextureFormat.ARGB32;
                break;
        }

        return Convert(rt, format);
    }

    static Texture2D Convert(RenderTexture rt, TextureFormat format)
    {
        var tex2d = new Texture2D(rt.width, rt.height, format, false);
        var rect = Rect.MinMaxRect(0f, 0f, tex2d.width, tex2d.height);
        RenderTexture.active = rt;
        tex2d.ReadPixels(rect, 0, 0);
        RenderTexture.active = null;
        return tex2d;
    }
}


public class SkAnimationBaker : MonoBehaviour
{
    public ComputeShader texGenShader;
    public Shader playShader;
    public AnimationClip[] clips;

    public int Frames = 60;

    public struct VertexInfo
    {
        public Vector3 pos;
        public Vector3 normal;
        public VertexInfo(Vector3 p, Vector3 n)
        {
            this.pos = p;
            this.normal = n;
        }
    }


    private void Reset()
    {
        var animation = GetComponent<Animation>();
        var animator = GetComponent<Animator>();
        if(animation != null)
        {
            clips = new AnimationClip[animation.GetClipCount()];
            var i = 0; 
            foreach(AnimationState state in animation)
            {
                clips[i++] = state.clip;
            }
        }
        else if(animator != null)
        {
            clips = animator.runtimeAnimatorController.animationClips;
        }
    }

    [ContextMenu("Bake Tex")]
    void Bake()
    {
        float frameTime = 1 / (float)Frames;
        var skin = GetComponentInChildren<SkinnedMeshRenderer>();
        var vCount = skin.sharedMesh.vertexCount;
        var texWidth = Mathf.NextPowerOfTwo(vCount);
        var mesh = new Mesh();

        foreach (var clip in clips)
        {
            var frames = Mathf.NextPowerOfTwo((int)(clip.length / frameTime));
            var dt = clip.length / frames;
            var infoList = new List<VertexInfo>();

            var pRT = new RenderTexture(texWidth, frames, 0, RenderTextureFormat.ARGBHalf);
            pRT.name = string.Format("{0}.{1} posTx", name, clip.name);
            var nRT = new RenderTexture(texWidth, frames, 0, RenderTextureFormat.ARGBHalf);
            nRT.name = string.Format("{0}.{1} normalTex", name, clip.name);

            foreach (var rt in new[] { pRT, nRT })
            {
                rt.enableRandomWrite = true;
                rt.Create();
                RenderTexture.active = rt;
                GL.Clear(true, true, Color.clear);
            }

            //sampe 
            for (var i = 0; i < frames; i++)
            {
                clip.SampleAnimation(gameObject, dt * i);
                skin.BakeMesh(mesh);

                VertexInfo[] infos = new VertexInfo[vCount];
                 
                for(int j = 0; j < vCount;j++)
                {
                    infos[j].pos = mesh.vertices[j];
                    infos[j].normal = mesh.normals[j];
                }
                infoList.AddRange(infos);
               
            }
            var buffer = new ComputeBuffer(infoList.Count, System.Runtime.InteropServices.Marshal.SizeOf(typeof(VertexInfo)));
            buffer.SetData(infoList.ToArray());

            var kernel = texGenShader.FindKernel("CSMain");
            uint x, y, z;
            texGenShader.GetKernelThreadGroupSizes(kernel, out x, out y, out z);
            texGenShader.SetInt("VertCount", vCount);
            texGenShader.SetBuffer(kernel, "Info", buffer);
            texGenShader.SetTexture(kernel, "OutPosition", pRT);
            texGenShader.SetTexture(kernel, "OutNormal", nRT);
            texGenShader.Dispatch(kernel, vCount / (int)x + 1, frames / (int)y + 1,1);
            buffer.Release();

#if UNITY_EDITOR
            var folderName = "BakeAnimationTex";
            var folderPath = Path.Combine("Assets", folderName);
            if (!AssetDatabase.IsValidFolder(folderPath))
                AssetDatabase.CreateFolder("Assets", folderName);

            var subFolder = name;
            var subFolderPath = Path.Combine(folderPath, subFolder);
           if (!AssetDatabase.IsValidFolder(subFolderPath))
                AssetDatabase.CreateFolder(folderPath, subFolder);

            var posTex = RenderTextureToTexture2D.Convert(pRT);
            var normalTex = RenderTextureToTexture2D.Convert(nRT);
            Graphics.CopyTexture(pRT, posTex);
            Graphics.CopyTexture(nRT, normalTex);

            var mat = new Material(playShader);
            mat.SetTexture("_MainTex", skin.sharedMaterial.mainTexture);
            mat.SetTexture("_PosTex", posTex);
            mat.SetTexture("_NormalTex", normalTex);
            mat.SetFloat("_Length", clip.length);
            mat.SetFloat("_DT", dt);
            if (clip.wrapMode == WrapMode.Loop ||clip.isLooping)
            {
                mat.SetFloat("_Loop", 1f);
                mat.EnableKeyword("ANIM_LOOP");
            }

            var go = new GameObject(name + "." + clip.name);
            go.AddComponent<MeshRenderer>().sharedMaterial = mat;
            go.AddComponent<MeshFilter>().sharedMesh = skin.sharedMesh;

            AssetDatabase.CreateAsset(posTex, Path.Combine(subFolderPath, pRT.name + ".asset"));
            AssetDatabase.CreateAsset(normalTex, Path.Combine(subFolderPath, nRT.name + ".asset"));
            AssetDatabase.CreateAsset(mat, Path.Combine(subFolderPath, string.Format("{0}.{1}.mat",name,clip.name)));
            PrefabUtility.SaveAsPrefabAsset(go,Path.Combine(folderPath, go.name + ".prefab").Replace("\\", "/"));
       
#endif

        }


        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }


}
