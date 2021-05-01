using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

[CustomEditor(typeof(ComputeRadianus))]
public class ComputeRadianusEditor : Editor
{



    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("akjlsdha"))
        {
            (target as ComputeRadianus).Convolune();

            byte[] bytes =  (target as ComputeRadianus).TargetTex.EncodeToPNG();
            string contents = "E:/test.png";
            FileStream file = File.Open(contents, FileMode.Create);
            BinaryWriter write = new BinaryWriter(file);
            write.Write(bytes);
            file.Close();

        }
    }


}
