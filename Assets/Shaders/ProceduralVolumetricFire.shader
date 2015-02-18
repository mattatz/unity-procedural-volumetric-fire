Shader "Mattatz/ProceduralVolumetricFire" {

    Properties {
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _FireTex ("Fire Texture", 2D) = "white" {}

        _Octives ("_Octives", int) = 4
        _Lacunarity ("_Lacunarity", float) = 2.0
        _Gain ("_Gain", float) = 0.5
        _Magnitude ("_Magnitude", float) = 1.3
    }

    SubShader {
        Tags { "RenderType"="Opaque" }

        LOD 200

        CGINCLUDE

        #include "UnityCG.cginc"

        #define MODULUS 61.0

        sampler2D _NoiseTex;
        sampler2D _FireTex;

        int _Octives;
        float _Lacunarity;
        float _Gain;
        float _Magnitude;

        float2 mBBS(float2 val, float mdls) {
            val = fmod(val, mdls);
            return fmod(val * val, mdls);
        }

        float mnoise(float3 pos) {
            float intArg = floor(pos.z);
            float fracArg = frac(pos.z);
            float2 hash = mBBS(intArg * 3.0 + float2(0, 3), MODULUS);
            float4 g = float4(
                tex2D(_NoiseTex, float2(pos.x, pos.y + hash.x) / MODULUS).xy, 
                tex2D(_NoiseTex, float2(pos.x, pos.y + hash.y) / MODULUS).xy * 2.0 - 1.0
            );
            return lerp(g.x + g.y * fracArg, g.z + g.w * (fracArg - 1.0), smoothstep(0.0, 1.0, fracArg));
        }

        float turbulence(float3 pos) {
            float sum = 0.0;
            float freq = 1.0;
            float amp = 1.0;
            for(int i = 0; i < _Octives; i++) {
                sum += abs(mnoise(pos * freq)) * amp;
                freq *= _Lacunarity;	
                amp *= _Gain;
            }
            return sum;
        }

        float4 sample_fire(float3 loc, float4 scale) {

            // convert to (radius, height) to sample fire texture.
            float2 st = float2(sqrt(dot(loc.xz, loc.xz)), loc.y);

            // convert loc to 'noise' space
            loc.y -= _Time.y * scale.w;
            loc *= scale.xyz;

            st.y += sqrt(st.y) * _Magnitude * turbulence(loc);

            if(st.y > 1.0) {
                return float4(0, 0, 0, 1);
            }

            float4 result = tex2D(_FireTex, st);

            if(st.y < 0.1) {
                result *= st.y / 0.1;
            }

            return result;
        }

        struct v2f {
            float4 pos : POSITION;
            float3 normal : NORMAL;
        };

        v2f vert (appdata_full v) {
            v2f o;
            o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
            o.normal = v.normal;

            return o;
        }

        ENDCG

        Pass {

            Cull Off
            Blend One One
            ZTest Always

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            half4 frag (v2f i) : COLOR {
                // use vertex' normal for tex location.
                float3 loc = i.normal;

                // Range [0.0, 1.0] to [- 1.0, 1.0]
                loc.xz = (loc.xz * 2) - 1.0;

                float3 col = sample_fire(loc, float4(1, 3, 1, 0.5));
                return float4(col * 0.25, 1);
            }

            ENDCG

        }

    } 
	
}
