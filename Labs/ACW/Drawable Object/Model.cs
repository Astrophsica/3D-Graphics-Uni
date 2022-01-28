using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Labs.Utility;

namespace Labs.ACW
{
    class Model:DrawableObject
    {
        public Model(ModelUtility pModelUtility, Matrix4 pMatrix, Material pMaterial, ShaderUtility pShader, int pPositionLocation, int pNormalLocation)
        {
            mVertices = pModelUtility.Vertices;

            mIndices = new uint[pModelUtility.Indices.Length];
            for(int i = 0; i < pModelUtility.Indices.Length; i++)
            {
                mIndices[i] = (uint)pModelUtility.Indices[i];
            }


            mMatrix = pMatrix;
            mMaterial = pMaterial;
            mShader = pShader;
            mPrimitiveType = PrimitiveType.Triangles;

            base.BindArrayObject();
            base.BindArrayBufferObject();
            base.BindElementArrayBufferObject();

            GL.EnableVertexAttribArray(pPositionLocation);
            GL.VertexAttribPointer(pPositionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(pNormalLocation);
            GL.VertexAttribPointer(pNormalLocation, 3, VertexAttribPointerType.Float, true, 6 * sizeof(float), 3 * sizeof(float));
        }

        // Clones model so that same VAO ID can be used
        public Model(Model pExistingModel)
        {
            mVAO_ID = pExistingModel.mVAO_ID;
            mVBO_IDs = pExistingModel.mVBO_IDs;
            mVertices = pExistingModel.mVertices;
            mIndices = pExistingModel.mIndices;
            mMatrix = pExistingModel.mMatrix;
            mMaterial = pExistingModel.mMaterial;
            mShader = pExistingModel.mShader;
            mPrimitiveType = pExistingModel.mPrimitiveType;
            mDrawCount = pExistingModel.mDrawCount;
            mElementArrayBufferEnabled = true;
        }
    }
}
