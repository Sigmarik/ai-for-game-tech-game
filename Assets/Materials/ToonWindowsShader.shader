Shader "Unlit/ToonWindowsShader"
{
    Properties
    {
        _Color ("Glass Tint", Color) = (0.65, 0.8, 0.9, 0.35)
        _MainTex ("Texture", 2D) = "white" {}
        _Brightness ("Brightness", Range(0, 1)) = 0.6
        _Strength ("Strength", Range(0, 1)) = 0.25
        _ToonInfluence ("Toon Light Influence", Range(0, 1)) = 0.35

        [Header(Fresnel)]
        _FresnelPower ("Fresnel Power", Range(0.5, 8)) = 3
        _FresnelStrength ("Fresnel Strength", Range(0, 2)) = 0.8
        _ReflectionStrength ("Reflection Strength", Range(0, 2)) = 0.5

        [Header(Fracture)]
        _BreakAmount ("Break Amount", Range(0, 1)) = 1
        _CrackScale ("Crack Scale", Range(0.5, 30)) = 6
        _CrackWidth ("Crack Width", Range(0.001, 0.2)) = 0.035
        _CrackColor ("Crack Color", Color) = (0.8, 0.95, 1, 1)
        _CrackGlow ("Crack Glow", Range(0, 4)) = 1.5
        _CrackDetail ("Crack Detail", Range(0, 1)) = 0.5
        _CellTint ("Shard Tint Variation", Range(0, 1)) = 0.35
        _HoleAmount ("Missing Shards", Range(0, 1)) = 0.12
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                half3 worldNormal : TEXCOORD1;
                float3 objectPos : TEXCOORD2;
                float3 worldPos : TEXCOORD3;
                float3 objectNormal : TEXCOORD4;
                UNITY_FOG_COORDS(5)
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _Brightness;
            float _Strength;
            float _ToonInfluence;
            float _FresnelPower;
            float _FresnelStrength;
            float _ReflectionStrength;
            float _BreakAmount;
            float _CrackScale;
            float _CrackWidth;
            fixed4 _CrackColor;
            float _CrackGlow;
            float _CrackDetail;
            float _CellTint;
            float _HoleAmount;

            float Toon(float3 normal, float3 lightDir)
            {
                float NdotL = max(0.0, dot(normalize(normal), normalize(lightDir)));
                return floor(NdotL / 0.3);
            }

            float2 hash22(float2 p)
            {
                p = float2(dot(p, float2(127.1, 311.7)), dot(p, float2(269.5, 183.3)));
                return frac(sin(p) * 43758.5453);
            }

            float hash21(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
            }

            float2 voronoi(float2 p, out float2 cellId)
            {
                float2 n = floor(p);
                float2 f = frac(p);
                float f1 = 8.0;
                float f2 = 8.0;
                float2 id = float2(0.0, 0.0);

                [unroll]
                for (int j = -1; j <= 1; j++)
                {
                    [unroll]
                    for (int i = -1; i <= 1; i++)
                    {
                        float2 g = float2((float)i, (float)j);
                        float2 o = hash22(n + g);
                        float2 r = g + o - f;
                        float d = dot(r, r);

                        if (d < f1)
                        {
                            f2 = f1;
                            f1 = d;
                            id = n + g;
                        }
                        else if (d < f2)
                        {
                            f2 = d;
                        }
                    }
                }

                cellId = id;
                return float2(sqrt(f1), sqrt(f2));
            }

            float crackMask(float2 p, float width, out float2 cellId, out float edge)
            {
                float2 v = voronoi(p, cellId);
                edge = v.y - v.x;
                float aa = max(fwidth(edge), 0.0001);
                return 1.0 - smoothstep(width - aa, width + aa, edge);
            }

            float2 crackPlane(float3 objectPos, float3 objectNormal)
            {
                float3 an = abs(normalize(objectNormal));
                if (an.z >= an.x && an.z >= an.y)
                {
                    return objectPos.xy;
                }
                if (an.y >= an.x)
                {
                    return objectPos.xz;
                }
                return objectPos.zy;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.objectPos = v.vertex.xyz;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.objectNormal = v.normal;
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 n = normalize(i.worldNormal);
                float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);

                float ndv = dot(n, viewDir);
                if (ndv < 0.0)
                {
                    n = -n;
                    ndv = -ndv;
                }
                ndv = saturate(ndv);
                float fresnel = pow(1.0 - ndv, _FresnelPower);

                float toon = Toon(n, _WorldSpaceLightPos0.xyz) * _Strength + _Brightness;
                toon = lerp(1.0, toon, _ToonInfluence);

                fixed4 tex = tex2D(_MainTex, i.uv);
                float3 base = tex.rgb * _Color.rgb * toon;

                float sky = saturate(n.y * 0.5 + 0.5);
                float3 reflectionColor = lerp(float3(0.06, 0.09, 0.12), float3(0.8, 0.9, 1.0), sky);
                base += reflectionColor * fresnel * _ReflectionStrength;

                float alpha = saturate(tex.a * _Color.a + fresnel * _FresnelStrength * 0.5);
                float2 p = crackPlane(i.objectPos, i.objectNormal) * _CrackScale;

                float2 cellId;
                float edge;
                float crack = crackMask(p, _CrackWidth, cellId, edge);

                float2 cellId2;
                float edge2;
                float crack2 = crackMask(p * 2.7 + 19.0, _CrackWidth * 0.45, cellId2, edge2);
                crack = max(crack, crack2 * _CrackDetail);

                crack *= saturate(_BreakAmount);

                float cellRand = hash21(cellId);
                float shardTint = lerp(1.0, 0.72 + 0.55 * cellRand, _CellTint * _BreakAmount);
                base *= shardTint;

                float microRand = hash21(cellId2);
                base *= lerp(1.0, 0.9 + 0.2 * microRand, _CrackDetail * _BreakAmount * 0.4);

                float halo = (1.0 - smoothstep(_CrackWidth, _CrackWidth * 2.5, edge)) * (1.0 - crack);
                base *= lerp(1.0, 0.55, halo * _BreakAmount);

                float halo2 = (1.0 - smoothstep(_CrackWidth * 0.45, _CrackWidth * 1.2, edge2)) * (1.0 - crack2);
                base *= lerp(1.0, 0.72, halo2 * _CrackDetail * _BreakAmount);

                base = lerp(base, _CrackColor.rgb * _CrackGlow, crack);

                float3 halfDir = normalize(viewDir + normalize(_WorldSpaceLightPos0.xyz));
                float spec = pow(saturate(dot(n, halfDir)), 48.0);
                base += spec * crack * _CrackGlow * 0.5;

                alpha = lerp(alpha, 1.0, crack);

                float hole = 1.0 - step(_HoleAmount * _BreakAmount, cellRand);
                alpha *= (1.0 - hole);

                fixed4 col = fixed4(base, saturate(alpha));
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
