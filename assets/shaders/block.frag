#version 330 core

in vec2 vTexCoord;
in vec3 vNormal;
in float vAO;

in float vDistance;

uniform sampler2D uTexture;
uniform vec3 uLightDirection;

uniform vec3 uFogColor;
uniform float uFogStart;
uniform float uFogEnd;
                                      
out vec4 out_color;

void main()
{
    vec3 normal = normalize(vNormal);
    vec3 lightDir = normalize(uLightDirection);
    
    float ambient = 0.45;
    float diffuse = dot(normal, lightDir) * 0.5 + 0.5;

    float faceLight;

    if (normal.y > 0.5) // Top
    {
        faceLight = 1.0;
    }
    else if (normal.y < -0.5) // Bottom
    {
        faceLight = 0.6;
    }
    else // Sides
    {
        faceLight = 0.8;
    }
    
    float lighting = ambient + (1.0 - ambient) * diffuse;
    lighting *= faceLight * vAO;
    
    vec4 texColor = texture(uTexture, vTexCoord);

    vec3 color = texColor.rgb * lighting;
    
    float fogFactor = smoothstep(uFogStart, uFogEnd, vDistance);
    
    color = mix(color, uFogColor, fogFactor);
    
    out_color = vec4(color, texColor.a);
}