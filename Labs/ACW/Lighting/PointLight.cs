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
    class PointLight : Light
    {
        private Vector4 mLightPosition;        

        public PointLight(int pLightId, bool pEnabled, ShaderUtility pLightShader, ShaderUtility pTextureShader, Vector4 pLightPosition, Matrix4 pView, Vector3 pAmbientLight, Vector3 pDiffuseLight, Vector3 pSpecularLight)
            :base(pLightId, "PointLight", pEnabled, pLightShader, pTextureShader, pView, pAmbientLight, pDiffuseLight, pSpecularLight)
        {
            SetPosition(pLightPosition, pView);
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

        public override void UpdateLightOnCameraChange(Matrix4 pView)
        {
            UpdatePositionOnCameraChange(pView);
        }
    }
}
