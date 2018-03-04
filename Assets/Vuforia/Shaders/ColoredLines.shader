<<<<<<< HEAD
Shader "Custom/ColoredLines" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
    }
    
    SubShader {
        Pass { 
            Lighting Off
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha
            Color [_Color]
        }
    } 
}
=======
Shader "Custom/ColoredLines" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
    }
    
    SubShader {
        Pass { 
            Lighting Off
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha
            Color [_Color]
        }
    } 
}
>>>>>>> 12b0a4668dd80710aa3ab2feca134c6c308dbb32
