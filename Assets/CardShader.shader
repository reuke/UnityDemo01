Shader "CardShader" 
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" { }
        _FrontTex("Front Texture", 2D) = "white" { }
        _BackTex("Back Texture", 2D) = "white" { }
        _parallax("Parallax", Float) = 0.5
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

            float _parallax;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL  ;
            };

            float4 _MainTex_ST;

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.normal = mul(unity_ObjectToWorld, v.normal);
                return o;
            }

            fixed4 frag(v2f i, float facing : VFACE) : SV_Target
            {
                const float fromX = 0.07;
                const float toX = 0.93;
                const float fromY = 0.439;
                const float toY = 0.892;

                float scaleX = 1 / (toX - fromX);
                float scaleY = 1 / (toY - fromY);

                float3x3 uv_im_trans = float3x3(scaleX, 0.0, -fromX * scaleX, 0.0, scaleY, -fromY * scaleY, 0.0, 0.0, 1.0);

                float4 c;

                if (facing >= 0)
                {
                    c = tex2D(_FrontTex, i.uv);

                    float2 parallax = normalize(mul(UNITY_MATRIX_V, i.normal * -1)).xy;

                    float2 uv_img = mul(uv_im_trans, float3(i.uv.xy, 1.0));

                    float2 uv_img_parallax = (uv_img - 0.5) / (_parallax + 1.0) + 0.5 - parallax * _parallax / 2.5;

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

                return c;
            }
            ENDHLSL

        }
    }
}