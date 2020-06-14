using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleComputeShaderArray : MonoBehaviour {
    public ComputeShader computeShader;
    int krenelIndex_KrenelFunction_A;
    int krenelIndex_KrenelFunction_B;
    ComputeBuffer intComputeBuffer;

    void Start () {
        Run2D ();
    }

    void Run1D () {
        this.krenelIndex_KrenelFunction_A = this.computeShader.FindKernel ("KernelFunction_A");
        this.krenelIndex_KrenelFunction_B = this.computeShader.FindKernel ("KernelFunction_B");

        this.intComputeBuffer = new ComputeBuffer (4, sizeof (int));
        this.computeShader.SetBuffer (this.krenelIndex_KrenelFunction_A, "intBuffer", this.intComputeBuffer);
        this.computeShader.SetInt ("intValue", 11);

        this.computeShader.Dispatch (this.krenelIndex_KrenelFunction_A, 1, 1, 1);
        int[] result = new int[4];
        this.intComputeBuffer.GetData (result);
        for (int i = 0; i < 4; i++) {
            Debug.Log (result[i]);
        }

        this.intComputeBuffer.Release ();
    }

    RenderTexture rtA;
    int krenelIndex_KrenelFunction_C;

    public GameObject plane;
    void Run2D () {

        this.rtA = new RenderTexture (512, 512, 0, RenderTextureFormat.ARGB32);
        this.rtA.enableRandomWrite = true;
        this.rtA.Create ();

        plane.GetComponent<Renderer>().material.mainTexture = this.rtA;
        this.krenelIndex_KrenelFunction_C = this.computeShader.FindKernel ("KernelFunction_C");

        uint threadSizeX, threadSizeY, threadSizeZ;
        this.computeShader.GetKernelThreadGroupSizes (this.krenelIndex_KrenelFunction_C, out threadSizeX, out threadSizeY, out threadSizeZ);
        this.computeShader.SetTexture(this.krenelIndex_KrenelFunction_C,"textureBuffer",this.rtA);
        this.computeShader.Dispatch (this.krenelIndex_KrenelFunction_C, (int) (this.rtA.width / threadSizeX), (int) (this.rtA.height / threadSizeY), (int) threadSizeZ);
        

        //this.rtA.Release ();
    }

}