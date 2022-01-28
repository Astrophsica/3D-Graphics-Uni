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
    class SpotLight : Light
    {
        private Vector4 mLightPosition;
        private float mCutOff;
        private Vector4 mDirection;

        public SpotLight(int pLightId, bool pEnabled, ShaderUtility pLightShader, ShaderUtility pTextureShader, Vector4 pLightPosition, Matrix4 pView, Vector3 pAmbientLight, Vector3 pDiffuseLight, Vector3 pSpecularLight, float pCutOff, Vector4 pDirection)
        : base(pLightId, "SpotLight", pEnabled, pLightShader, pTextureShader, pView, pAmbientLight, pDiffuseLight, pSpecularLight)
        {
            SetPosition(pLightPosition, pView);
            SetCutOff(pCutOff);
            SetDirection(pDirection, pView);
        }

        public Vector4 GetPosition()
        {
            return mLightPosition;
        }

        public void SetPosition(Vector4 pLightPosition, Matrix4 pView)
        {
            mLightPosition = pLightPosition;

            UpdatePositionOnCameraChange(pView);
        }

        public void UpdatePositionOnCameraChange(Matrix4 pView)
        {
            Vector4 lightPosition = Vector4.Transform(mLightPosition, pView);
            // Light Shader
            GL.UseProgram(mLightShader.ShaderProgramID);
            int uLightPositionLocation = GL.GetUniformLocation(mLightShader.ShaderProgramID, "u" + mType + "[" + mLightId + "].Position");
            GL.Uniform4(uLightPositionLocation, lightPosition);

            // Texture Shader
            GL.UseProgram(mTextureShader.ShaderProgramID);
            int uTextureLightPositionLocation = GL.GetUniformLocation(mTextureShader.ShaderProgramID, "u" + mType + "[" + mLightId + "].Position");
            GL.Uniform4(uTextureLightPositionLocation, lightPosition);
        }

        public void SetCutOff(float pCutOff)
        {
            mCutOff = pCutOff;

            // Light Shader
            GL.UseProgram(mLightShader.ShaderProgramID);
            int uSpotLightCutOffLocation = GL.GetUniformLocation(mLightShader.ShaderProgramID, "u" + mType + "[" + mLightId + "].CutOff");
            GL.Uniform1(uSpotLightCutOffLocation, mCutOff);

            // Texture Shader
            GL.UseProgram(mTextureShader.ShaderProgramID);
            int uTextureSpotLightCutOffLocation = GL.GetUniformLocation(mTextureShader.ShaderProgramID, "u" + mType + "[" + mLightId + "].CutOff");
            GL.Uniform1(uTextureSpotLightCutOffLocation, mCutOff);
        }
        
        public void SetDirection(Vector4 pDirection, Matrix4 pView)
        {
            mDirection = pDirection;

            UpdateDirectionOnCameraChange(pView);
        }

        public void UpdateDirectionOnCameraChange(Matrix4 pView)
        {
            Vector4 lightDirection = Vector4.Transform(mDirection, pView);

            // Light Shader
            GL.UseProgram(mLightShader.ShaderProgramID);
            int uSpotLightDirectionLocation = GL.GetUniformLocation(mLightShader.ShaderProgramID, "u" + mType + "[" + mLightId + "].Direction");
            GL.Uniform4(uSpotLightDirectionLocation, lightDirection);

            // Texture Shader
            GL.UseProgram(mTextureShader.ShaderProgramID);
            int uTextureSpotLightDirectionLocation = GL.GetUniformLocation(mTextureShader.ShaderProgramID, "u" + mType + "[" + mLightId + "].Direction");
            GL.Uniform4(uTextureSpotLightDirectionLocation, lightDirection);
        }

        public override void UpdateLightOnCameraChange(Matrix4 pView)
        {
            UpdatePositionOnCameraChange(pView);
            UpdateDirectionOnCameraChange(pView);
        }

    }

}
