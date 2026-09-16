#version 330 core

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoord;
layout (location = 3) in float aAO;

out vec2 vTexCoord;
out vec3 vNormal;
out float vAO;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

void main()
{
    gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
    
    vTexCoord = aTexCoord;
    vNormal = mat3(uModel) * aNormal;
    
    const float aoLevel[4] = float[](0.55, 0.72, 0.88, 1.0);
    int aoIdx = clamp(int(aAO), 0, 3);
    vAO = aoLevel[aoIdx];
}