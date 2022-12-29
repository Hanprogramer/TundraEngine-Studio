namespace TundraEngine.Rendering
{
    public static class DefaultShader
    {
        public static string Fragment = @"#version 330 core
in vec2 fUv;

//A uniform of the type sampler2D will have the storage value of our texture.
uniform sampler2D uTexture0;

out vec4 FragColor;

void main()
{
    //Here we sample the texture based on the Uv coordinates of the fragment
    FragColor = texture(uTexture0, fUv);
}";
        public static string Vertex = @"#version 330 core
layout (location = 0) in vec2 vPos;
layout (location = 1) in vec2 vUv;

uniform mat4 uProjection;

out vec2 fUv;

void main()
{
    gl_Position = uProjection * vec4(vPos, 0.0, 1.0);
    //Setting the uv coordinates on the vertices will mean they get correctly divided out amongst the fragments.
    fUv = vUv;
}";
    }
}
