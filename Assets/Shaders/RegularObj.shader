Shader "Custom/RegularObj"
{
    SubShader
    {
        Pass
        {
            Stencil
            {
                Ref 1
                Comp Equal
            }
        }
    }
}
