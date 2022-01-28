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
    class Material
    {
        enum MaterialType
        {
            NormalMaterial,
            TextureWShininess,
        }

        private MaterialType mMaterialType;
        private Vector3 mAmbient;
        private Vector3 mDiffuse;
        private Vector3 mSpecular;
        private float mShininess;
        private int mTexture_ID;
        private ShaderUtility mShader;

        public Material(ShaderUtility pShader, Vector3 pAmbient, Vector3 pDiffuse, Vector3 pSpecular, float pShininess)
        {
            mShader = pShader;
            mAmbient = pAmbient;
            mDiffuse = pDiffuse;
            mSpecular = pSpecular;
            mShininess = pShininess;

            mMaterialType = MaterialType.NormalMaterial;
        }

        public Material(ShaderUtility pShader, int pTexture_ID, float pShininess)
        {
            mShader = pShader;
            mTexture_ID = pTexture_ID;
            mShininess = pShininess;

            mMaterialType = MaterialType.TextureWShininess;
        }

        public void UseMaterial()
        {
            if (mMaterialType == MaterialType.NormalMaterial)
            {
                // Light Shader
                GL.UseProgram(mShader.ShaderProgramID);
                int uMaterialAmbientReflectivity = GL.GetUniformLocation(mShader.ShaderProgramID, "uMaterial.AmbientReflectivity");
                GL.Uniform3(uMaterialAmbientReflectivity, mAmbient);

                int uMaterialDiffuseReflectivity = GL.GetUniformLocation(mShader.ShaderProgramID, "uMaterial.DiffuseReflectivity");
                GL.Uniform3(uMaterialDiffuseReflectivity, mDiffuse);

                int uMaterialSpecularReflectivity = GL.GetUniformLocation(mShader.ShaderProgramID, "uMaterial.SpecularReflectivity");
                GL.Uniform3(uMaterialSpecularReflectivity, mSpecular);

                int uShininess = GL.GetUniformLocation(mShader.ShaderProgramID, "uMaterial.Shininess");
                GL.Uniform1(uShininess, mShininess);
            }
            else if (mMaterialType == MaterialType.TextureWShininess)
            {
                // Texture Shader
                GL.UseProgram(mShader.ShaderProgramID);
                int uTextureSamplerLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uTextureSampler");
                GL.Uniform1(uTextureSamplerLocation, mTexture_ID);

                int uTextureShininess = GL.GetUniformLocation(mShader.ShaderProgramID, "Shininess");
                GL.Uniform1(uTextureShininess, mShininess);
            }

        }
    }
}
