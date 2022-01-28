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
    class FlatPlane:DrawableObject
    {

        public FlatPlane(Matrix4 pMatrix, Material pMaterial, ShaderUtility pShader, int pTexturePositionLocation, int pTextureNormalLocation, int pTexCoordsLocation)
        {
            mVertices = new float[] {
                                -10, 0, -10, 0,1,0, 0, 0, // Bottom left
                                -10, 0, 10,  0,1,0, 0, 1, // Top left
                                 10, 0, 10,  0,1,0, 1, 1, // Top right
                                 10, 0, -10, 0,1,0, 1, 0  // Bottom right
            };

            mMatrix = pMatrix;
            mMaterial = pMaterial;
            mShader = pShader;
            mPrimitiveType = PrimitiveType.TriangleFan;
            mDrawCount = 4;

            base.BindArrayObject();
            base.BindArrayBufferObject();

            GL.EnableVertexAttribArray(pTexturePositionLocation);
            GL.VertexAttribPointer(pTexturePositionLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

            GL.EnableVertexAttribArray(pTextureNormalLocation);
            GL.VertexAttribPointer(pTextureNormalLocation, 3, VertexAttribPointerType.Float, true, 8 * sizeof(float), 3 * sizeof(float));

            GL.EnableVertexAttribArray(pTexCoordsLocation);
            GL.VertexAttribPointer(pTexCoordsLocation, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
        }

        // Clones model so that same VAO ID can be used
        public FlatPlane(FlatPlane pExistingFlatPlane)
        {
            mVAO_ID = pExistingFlatPlane.mVAO_ID;
            mVBO_IDs = pExistingFlatPlane.mVBO_IDs;
            mVertices = pExistingFlatPlane.mVertices;
            mIndices = pExistingFlatPlane.mIndices;
            mMatrix = pExistingFlatPlane.mMatrix;
            mMaterial = pExistingFlatPlane.mMaterial;
            mShader = pExistingFlatPlane.mShader;
            mPrimitiveType = pExistingFlatPlane.mPrimitiveType;
            mDrawCount = pExistingFlatPlane.mDrawCount;
            mElementArrayBufferEnabled = false;
        }

    }
}
