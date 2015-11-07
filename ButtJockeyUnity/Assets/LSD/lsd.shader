Shader "Custom/LSD" {
	Properties {
		_X ("X", Float) = 0.1
		_Y ("Y", Float) = 0.1 
	}
    SubShader {
        Pass {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            
			uniform float _X;
			uniform float _Y;

            float4 vert(float4 v:POSITION) : SV_POSITION {
            	
            	float4 world = mul(_Object2World, v);
            	
            	world.y += world.z*world.z/1000*_X + 0.6*cos(world.z);
            	world.x += world.z*world.z/1000*_Y;
            	
            	return mul (UNITY_MATRIX_VP, world);
            }
            
     		fixed4 frag(float4 v:POSITION) : SV_Target {
     			
                return fixed4(v.z/3,0.0,0.0,1.0);
            }

            ENDCG
        }
    }
}
