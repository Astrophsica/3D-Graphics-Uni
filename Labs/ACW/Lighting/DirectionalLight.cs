using Labs.Utility;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labs.ACW
{
    class DirectionalLight:Light
    {
        private Vector4 mDirection;

        public DirectionalLight(int pLightId, bool pEnabled, ShaderUtility pLightShader, ShaderUtility pTextureShader, Matrix4 pView, Vector3 pAmbientLight, Vector3 pDiffuseLight, Vector3 pSpecularLight, Vector4 pDirection)
        : base(pLightId, "DirectionalLight", pEnabled, pLightShader, pTextureShader, pView, pAmbientLight, pDiffuseLight, pSpecularLight)
        {
            SetDirection(pDirection, pView);
        }
        
        public void SetDirection(Vector4 pDirection, Matrix4 pView)
        {
            mDirection = pDirection;

            UpdateDirectionOnCameraChange(pView);
        }

        public void UpdateDirectionOnCameraChange(Matrix4 pView)
        {
            Vector4 normalisedLightDirection = mDirection;
            normalisedLightDirection = Vector4.Transform(mDirection, pView);

            // Light Shader
            GL.UseProgram(mLightShader.ShaderProgramID);
            int uSpotLightDirectionLocation = GL.GetUniformLocation(mLightShader.ShaderProgramID, "u" + mType + "[" + mLightId + "].Direction");
            GL.Uniform4(uSpotLightDirectionLocation, normalisedLightDirection);

            // Texture Shader
            GL.UseProgram(mTextureShader.ShaderProgramID);
            int uTextureSpotLightDirectionLocation = GL.GetUniformLocation(mTextureShader.ShaderProgramID, "u" + mType + "[" + mLightId + "].Direction");
            GL.Uniform4(uTextureSpotLightDirectionLocation, normalisedLightDirection);
        }

        public override void UpdateLightOnCameraChange(Matrix4 pView)
        {
            UpdateDirectionOnCameraChange(pView);
        }

    }

}
