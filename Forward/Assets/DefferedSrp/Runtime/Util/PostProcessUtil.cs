using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessUtil 
{
    //Provide PostMesh

    private static Mesh M_RectangleMesh;
    public static Mesh RectangleMesh
    {
        get
        {
            if(M_RectangleMesh == null)
            {
                M_RectangleMesh = new Mesh();
                M_RectangleMesh.vertices = new Vector3[] {
                new Vector3(-1,-1,0f),
                new Vector3(-1,1,0f),
                new Vector3(1,1,0f),
                new Vector3(1,-1,0f)
            };
                M_RectangleMesh.uv = new Vector2[] {
                new Vector2(0,1),
                new Vector2(0,0),
                new Vector2(1,0),
                new Vector2(1,1)
            };

                M_RectangleMesh.SetIndices(new int[] { 0, 1, 2, 0, 3, 2 }, MeshTopology.Triangles, 0);
                return M_RectangleMesh;

            }
            return M_RectangleMesh;
        }
    }


}
