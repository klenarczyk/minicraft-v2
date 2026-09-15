#version 330 core

in vec2 vTexCoord;
in vec3 vNormal;

uniform sampler2D uTexture;
uniform vec3 uLightDirection;
                                      
out vec4 out_color;

void main()
{
    vec3 normal = normalize(vNormal);
    vec3 lightDir = normalize(uLightDirection);
    
    float ambient = 0.4;
    float diffuse = max(dot(normal, lightDir), 0.0);
    
    float brightness = ambient + (1.0 - ambient) * diffuse;
    vec4 texColor = texture(uTexture, vTexCoord);
    
    out_color = vec4(texColor.rgb * brightness, texColor.a);
}