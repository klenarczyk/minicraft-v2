#version 330 core

in vec2 vTexCoord;
in vec3 vNormal;
in float vAO;

uniform sampler2D uTexture;
uniform vec3 uLightDirection;
                                      
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

    out_color = vec4(texColor.rgb * lighting, texColor.a);
    
    // Debug Output:
//    out_color = vec4(vec3(1, 1, 1) * lighting, 1.0);
}