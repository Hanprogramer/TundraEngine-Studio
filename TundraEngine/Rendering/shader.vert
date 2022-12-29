#version 330 core
layout (location = 0) in vec3 vPos;
layout (location = 1) in vec2 vUv;

uniform mat4 uProjection;

out vec2 fUv;

void main()
{
    gl_Position = uProjection * vec4(vPos, 1.0);
    //Setting the uv coordinates on the vertices will mean they get correctly divided out amongst the fragments.
    fUv = vUv;
}