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
    class Light
    {
        public int mLightId;
        public string mType;
        private bool mEnabled;
        protected ShaderUtility mLightShader, mTextureShader;
        private Vector3 mAmbientLight, mDiffuseLight, mSpecularLight;

        public Light(int pLightId, string pType, bool pEnabled, ShaderUtility pLightShader, ShaderUtility pTextureShader, Matrix4 pView, Vector3 pAmbientLight, Vector3 pDiffuseLight, Vector3 pSpecularLight)
        {
            mLightId = pLightId;
            mLightShader = pLightShader;
            mTextureShader = pTextureShader;
            mType = pType;

            SetEnabled(pEnabled);
            SetAmbientLight(pAmbientLight);
            SetDiffuseLight(pDiffuseLight);
            SetSpecularLight(pSpecularLight);
        }

        public virtual void UpdateLightOnCameraChange(Matrix4 pView)
        {

        }

        public bool GetEnabled()
        {
            return mEnabled;
        }

        public void SetEnabled(bool pEnable)
        {
            mEnabled = pEnable;

            int enabledBoolInt = 0;
            if (mEnabled == true)
            {
                enabledBoolInt = 1;
            }


            // Light Shader
            GL.UseProgram(mLightShader.ShaderProgramID);
            int uLightAmbientLocation = GL.GetUniformLocation(mLightShader.ShaderProgramID, "u" + mType + "[" + mLightId + "].Enabled");
            GL.Uniform1(uLightAmbientLocation, enabledBoolInt);

            // Texture Shader
            GL.UseProgram(mTextureShader.ShaderProgramID);
            int uTextureLightAmbientLocation = GL.GetUniformLocation(mTextureShader.ShaderProgramID, "u" + mType + "[" + mLightId + "].Enabled");
            GL.Uniform1(uTextureLightAmbientLocation, enabledBoolInt);
        }

        public Vector3 GetAmbientLight()
        {
            return mAmbientLight;
        }

        public void SetAmbientLight(Vector3 pAmbientLight)
        {
            mAmbientLight = pAmbientLight;

            // Light Shader
            GL.UseProgram(mLightShader.ShaderProgramID);
            int uLightAmbientLocation = GL.GetUniformLocation(mLightShader.ShaderProgramID, "u" + mType + "[" + mLightId + "].AmbientLight");
            GL.Uniform3(uLightAmbientLocation, mAmbientLight);

            // Texture Shader
            GL.UseProgram(mTextureShader.ShaderProgramID);
            int uTextureLightAmbientLocation = GL.GetUniformLocation(mTextureShader.ShaderProgramID, "u" + mType + "[" + mLightId + "].AmbientLight");
            GL.Uniform3(uTextureLightAmbientLocation, mAmbientLight);
        }

        public Vector3 GetDiffuseLight()
        {
            return mDiffuseLight;
        }

        public void SetDiffuseLight(Vector3 pDiffuseLight)
        {
            mDiffuseLight = pDiffuseLight;

            // Light Shader
            GL.UseProgram(mLightShader.ShaderProgramID);
            int uLightDiffuseLocation = GL.GetUniformLocation(mLightShader.ShaderProgramID, "u" + mType + "[" + mLightId + "].DiffuseLight");
            GL.Uniform3(uLightDiffuseLocation, mDiffuseLight);

            // Texture Shader
            GL.UseProgram(mTextureShader.ShaderProgramID);
            int uTextureLightDiffuseLocation = GL.GetUniformLocation(mTextureShader.ShaderProgramID, "u" + mType + "[" + mLightId + "].DiffuseLight");
            GL.Uniform3(uTextureLightDiffuseLocation, mDiffuseLight);
        }

        public Vector3 GetSpecularLight()
        {
            return mSpecularLight;
        }

        public void SetSpecularLight(Vector3 pSpecularLight)
        {
            mSpecularLight = pSpecularLight;

            // Light Shader
            GL.UseProgram(mLightShader.ShaderProgramID);
            int uLightSpecularLocation = GL.GetUniformLocation(mLightShader.ShaderProgramID, "u" + mType + "[" + mLightId + "].SpecularLight");
            GL.Uniform3(uLightSpecularLocation, mSpecularLight);

            // Texture Shader
            GL.UseProgram(mTextureShader.ShaderProgramID);
            int uTextureLightSpecularLocation = GL.GetUniformLocation(mTextureShader.ShaderProgramID, "u" + mType + "[" + mLightId + "].SpecularLight");
            GL.Uniform3(uTextureLightSpecularLocation, mSpecularLight);
        }
    }
}
