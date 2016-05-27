Shader "mattatz/ProceduralVolumetricFire" {

    Properties {
        _FireTex ("Fire Texture", 2D) = "white" {}
		_NoiseTex ("Noise Texture", 3D) = "" {}
		_Scale ("Fire Scale", Vector) = (1, 3, 1, 0.5)
		_Lacunarity ("_Lacunarity", float) = 2.0
		_Gain ("_Gain", float) = 0.5
		_Magnitude ("_Magnitude", float) = 1.3
		_Atten ("Attenuation", Range(0.05, 0.7)) = 0.25

		_WavePower ("Wave Power", float) = 0.8
		_WaveSpeed ("Wave Speed", float) = 0.25
		_WaveIntensity ("Wave Intensity", float) = 1.0
		_WaveScale ("Wave Scale", Vector) = (0.25, 1, 0.5)
	}

	SubShader {
		Tags { "RenderType" = "Opaque" }

		LOD 200

		CGINCLUDE

		#include "UnityCG.cginc"

		#include "./SimplexNoise3D.cginc"

		sampler3D _NoiseTex;
		float sample_noise(float3 seed) {
			float n = (tex3D(_NoiseTex, seed).r - 0.5) * 2.0;
			return n;
		}

		// USE PROCEDURAL NOISE
		// #define FIRE_NOISE snoise

		// USE SAMPLING NOISE
		#define FIRE_NOISE sample_noise

        // #include "./ClassicNoise3D.cginc"
        // #define FIRE_NOISE cnoise

        #define FIRE_OCTIVES 4

		fixed _WavePower, _WaveSpeed, _WaveIntensity;
		fixed3 _WaveScale;

        sampler2D _FireTex;
        fixed4 _Scale;
        float _Lacunarity;
        float _Gain;
        float _Magnitude;
        fixed _Atten;

        float turbulence(float3 pos) {
            float sum = 0.0;
            float freq = 1.0;
            float amp = 1.0;

            for(int i = 0; i < FIRE_OCTIVES; i++) {
                sum += abs(FIRE_NOISE(pos * freq)) * amp;
                freq *= _Lacunarity;	
                amp *= _Gain;
            }
            return sum;
        }

        float4 sample_fire (float3 loc, float4 scale) {
            // convert to (radius, height) to sample fire texture.
            float2 st = float2(sqrt(dot(loc.xz, loc.xz)), loc.y);

            // convert loc to noise space
            loc.y -= _Time.y * scale.w;
            loc *= scale.xyz;

            st.y += sqrt(st.y) * _Magnitude * turbulence(loc);

            if(st.y > 1.0) {
                return float4(0, 0, 0, 1);
            }

            return tex2D(_FireTex, st);
        }

        struct v2f {
            float4 pos : POSITION;
            float3 normal : TEXCOORD0;
        };

        v2f vert (appdata_full v) {
            v2f o;

			float y = v.vertex.y; // 0.0 ~ 1.0
			float h = pow(y, _WavePower);
			float n = snoise(v.vertex.xyz * _WaveScale + float3(0, 0, _Time.y * _WaveSpeed));
			v.vertex.x += n * 0.5 * h * _WaveIntensity;
            o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
            o.normal = v.normal;
            return o;
        }

        float4 frag (v2f i) : COLOR {
            // use vertex' normal for tex location.
            float3 loc = i.normal;

            // Range [0.0, 1.0] to [- 1.0, 1.0]
            loc.xz = (loc.xz * 2) - 1.0;

            float4 col = sample_fire(loc, _Scale);
            return float4(col.rgb * _Atten, 1.0);
        }

        ENDCG

        Pass {
            Cull Off
            Blend One One
            ZTest Always

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }

    } 

}

