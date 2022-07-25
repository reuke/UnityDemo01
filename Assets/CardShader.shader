Shader "CardShader" 
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" { }
        _FrontTex("Front Texture", 2D) = "white" { }
        _BackTex("Back Texture", 2D) = "white" { }
        _Parallax("Parallax", Float) = 0.6
        _SpecularShine("SpecularShine", Float) = 16
        _LightComponent("LightComponent", Float) = 0
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _FrontTex;
            sampler2D _BackTex;

            float _Parallax;
            float _SpecularShine;
            float _LightComponent;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float4 posWorld : TEXCOORD1;
            };

            float4 _MainTex_ST;

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.normal = mul(unity_ObjectToWorld, v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            fixed4 frag(v2f i, float facing : VFACE) : SV_Target
            {
                const float fromX = 0.07;
                const float toX = 0.93;
                const float fromY = 0.439;
                const float toY = 0.892;

                float3 normal = normalize(i.normal);

                float scaleX = 1 / (toX - fromX);
                float scaleY = 1 / (toY - fromY);

                float3x3 uv_im_trans = float3x3(scaleX, 0.0, -fromX * scaleX, 0.0, scaleY, -fromY * scaleY, 0.0, 0.0, 1.0);

                float4 c;

                if (facing >= 0)
                {
                    c = tex2D(_FrontTex, i.uv);

                    float2 parallax = normalize(mul(UNITY_MATRIX_V, normal * -1)).xy;

                    float2 uv_img = mul(uv_im_trans, float3(i.uv.xy, 1.0));

                    float2 uv_img_parallax = (uv_img - 0.5) / (_Parallax + 1.0) + 0.5 - parallax * _Parallax / 2.5;

                    if (uv_img.x > 0 && uv_img.x < 1 &&
                        uv_img.y > 0 && uv_img.y < 1)
                    {
                        c += tex2D(_MainTex, uv_img_parallax) * (1 - c.a);
                        c.a = 1;
                    }

                }
                else
                {
                    i.uv.x *= -1;
                    c = tex2D(_BackTex, i.uv);
                }

                float3 viewDir = normalize(_WorldSpaceCameraPos - i.posWorld.xyz);
                float3 lightDir = _WorldSpaceLightPos0.xyz - i.posWorld.xyz * _WorldSpaceLightPos0.w;

                float3 diffuse = abs(dot(normal, lightDir));
                float specular = pow(abs(dot(reflect(lightDir, normal), viewDir)), _SpecularShine);

                float3 color = diffuse * c.rgb + float3(1.0, 1.0, 1.0) * specular;

                color = color * _LightComponent + c.rgb * (1 - _LightComponent);
                return float4(color, c.a);
            }
            ENDHLSL

        }
    }
}