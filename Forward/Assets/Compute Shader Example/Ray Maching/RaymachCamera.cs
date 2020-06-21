using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;    


[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
[ImageEffectAllowedInSceneView]
public class RaymachCamera : MonoBehaviour
{
    public Shader _shader;
    public Material _raymachMatrial
    {
        get
        {
            if(!_raymachMat && _shader)
            {
                _raymachMat = new Material(_shader);
                _raymachMat.hideFlags = HideFlags.HideAndDontSave;
            }
            return _raymachMat;
        }
    }
    private Material _raymachMat;

    private Camera _cam;
    public Camera _camera
    {
        get
        {
            if(!_camera)
            {
                _cam = GetComponent<Camera>();
            }
            return _cam;
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!_raymachMatrial)
        {
            Graphics.Blit(source, destination);
            return;
        }
        Graphics.Blit(source, destination);
        
        //_raymachMatrial.SetMatrix("_CamFrustum", CamFrustum(_camera));
        
        _raymachMatrial.SetMatrix("_CamToWorld", _camera.cameraToWorldMatrix);
        /*_raymachMatrial.SetVector("_CamworldSapce", _camera.transform.position);
            
        RenderTexture.active = destination;
        GL.PushMatrix();
        GL.LoadOrtho();
        _raymachMatrial.SetPass(0);
        GL.Begin(GL.QUADS);

        //BL
        GL.MultiTexCoord2(0, 0.0f, 0.0f);
        GL.Vertex3(0.0f, 0.0f, 3.0f);
        //BR
        GL.MultiTexCoord2(0, 1.0f, 0.0f);
        GL.Vertex3(1.0f, 0.0f, 2.0f);
        //TR
        GL.MultiTexCoord2(0, 1.0f, 1.0f);
        GL.Vertex3(1.0f, 1.0f, 1.0f);
        //TL
        GL.MultiTexCoord2(0, 0.0f, 1.0f);
        GL.Vertex3(0.0f, 1.0f, 0.0f);

        GL.End();
        GL.PopMatrix();*/
        
    }

    private Matrix4x4 CamFrustum(Camera cam)
    {
        Matrix4x4 frustum = Matrix4x4.identity;
        float fov = Mathf.Tan(cam.fieldOfView * 0.5f *Mathf.Deg2Rad);

        Vector3 goUp = Vector3.up * fov;
        Vector3 goRight = Vector3.right * fov * cam.aspect;

        Vector3 TL = (-Vector3.forward - goRight + goUp);
        Vector3 TR = (-Vector3.forward + goRight + goUp);
        Vector3 BL = (-Vector3.forward - goRight - goUp);
        Vector3 BR = (-Vector3.forward + goRight - goUp);


        frustum.SetRow(0, TL);
        frustum.SetRow(0, TR);
        frustum.SetRow(0, BL);
        frustum.SetRow(0, BR);

        return frustum;
    }


}
