#version 330 core

in vec2 vUV;

out vec4 out_color;

uniform mat4 uInvView;
uniform mat4 uInvProjection;

uniform vec3 uHorizonColor;
uniform vec3 uZenithColor;

void main()
{
    vec4 clipPosition = vec4(
            vUV * 2.0 - 1.0,
            1.0,
            1.0
    );

    vec4 viewPosition = uInvProjection * clipPosition;
    viewPosition /= viewPosition.w;

    vec3 direction = normalize(mat3(uInvView) * viewPosition.xyz);
    float height = direction.y;

    float t = smoothstep(0.0, 0.8, height);
    vec3 color = mix(uHorizonColor, uZenithColor, t);

    out_color = vec4(color, 1.0);
}