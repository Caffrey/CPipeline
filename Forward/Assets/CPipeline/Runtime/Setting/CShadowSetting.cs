using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace CPipeline.Runtime
{

    public enum TextureSize
    {
        _256 = 256, _512 = 512, _1024 = 1024,
        _2048 = 2048, _4096 = 4096
    }

    [System.Serializable]
    public struct Directional
    {
        public TextureSize altasSize;
    };
    

    [System.Serializable]
    public class CShadowSetting
    {
        [Min(0f)]
        public float maxDistance = 100f;

        public Directional directional = new Directional { altasSize = TextureSize._1024 };
    }

}