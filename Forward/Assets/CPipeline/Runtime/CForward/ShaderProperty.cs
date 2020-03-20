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


    }
}