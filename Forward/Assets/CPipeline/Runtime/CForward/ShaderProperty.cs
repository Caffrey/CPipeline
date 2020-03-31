using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CPipeline.Runtime
{

    public class ShaderProperty 
    {

        //----------------Light
        public static int VISIBLE_LIGHT_COUNT = Shader.PropertyToID("c_visible_light_count");
        public static int VISIBLE_LIGHT_COLOR = Shader.PropertyToID("c_visible_light_color");
        public static int VISIBLE_LIGHT_DIRECTION = Shader.PropertyToID("c_visible_light_direction");
        public static int VISIBLE_LIGHT_ATTENUATION = Shader.PropertyToID("c_visible_light_attenuation");
        public static int VISIBLE_LIGHT_SPOT_DIRECTION = Shader.PropertyToID("c_visible_light_spot_direction");


    }
}