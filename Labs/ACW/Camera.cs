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

    class Camera
    {

        public int mCameraId;
        private Matrix4 mView;
        private Vector4 mEyePosition;
        public bool mFixed;
        private ShaderUtility mLightShader, mTextureShader;


        public Camera(int pCameraId, Matrix4 pView, bool pFixed, ShaderUtility pLightShader, ShaderUtility pTextureShader)
        {
            mCameraId = pCameraId;
            mFixed = pFixed;
            mView = pView;
            mEyePosition = new Vector4(mView.ExtractTranslation(), 1);
            mLightShader = pLightShader;
            mTextureShader = pTextureShader;

        }

        public void UpdateCameraMatrix(Matrix4 pView)
        {
            if (mFixed == false)
            {
                mView = pView;
                mEyePosition = new Vector4(mView.ExtractTranslation(), 1);

                // Light Shader
                GL.UseProgram(mLightShader.ShaderProgramID);
                int uEyePosition = GL.GetUniformLocation(mLightShader.ShaderProgramID, "uEyePosition");
                int uView = GL.GetUniformLocation(mLightShader.ShaderProgramID, "uView");

                GL.UniformMatrix4(uView, true, ref mView);
                GL.Uniform4(uEyePosition, ref mEyePosition);
                

                // Texture Shader
                GL.UseProgram(mTextureShader.ShaderProgramID);
                int uTextureEyePosition = GL.GetUniformLocation(mTextureShader.ShaderProgramID, "uEyePosition");
                int uTextureView = GL.GetUniformLocation(mTextureShader.ShaderProgramID, "uView");

                GL.UniformMatrix4(uTextureView, true, ref mView);
                GL.Uniform4(uTextureEyePosition, ref mEyePosition);
            }
        }

        public void UseCamera()
        {
            mEyePosition = new Vector4(mView.ExtractTranslation(), 1);

            // Light Shader
            GL.UseProgram(mLightShader.ShaderProgramID);
            int uEyePosition = GL.GetUniformLocation(mLightShader.ShaderProgramID, "uEyePosition");
            int uView = GL.GetUniformLocation(mLightShader.ShaderProgramID, "uView");

            GL.UniformMatrix4(uView, true, ref mView);
            GL.Uniform4(uEyePosition, ref mEyePosition);


            // Texture Shader
            GL.UseProgram(mTextureShader.ShaderProgramID);
            int uTextureEyePosition = GL.GetUniformLocation(mTextureShader.ShaderProgramID, "uEyePosition");
            int uTextureView = GL.GetUniformLocation(mTextureShader.ShaderProgramID, "uView");

            GL.UniformMatrix4(uTextureView, true, ref mView);
            GL.Uniform4(uTextureEyePosition, ref mEyePosition);
        }

        public Matrix4 GetCameraMatrix()
        {
            return mView;
        }


    }
}
