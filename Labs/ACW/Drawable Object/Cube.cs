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
    class Cube:DrawableObject
    {


        public Cube(float pSize, Matrix4 pMatrix, Material pMaterial, ShaderUtility pShader, int pPositionLocation, int pNormalLocation)
        {   

            float halfSize = pSize / 2;

            // NOTE: Triangles for each side needs to be unique, as the normal changes depending on the side

            mVertices = new float[] {
                // Front
                -halfSize, halfSize, halfSize,   0f, 0f, 1f, // 0 Top left 
                -halfSize, -halfSize, halfSize,  0f, 0f, 1f, // 1 Bottom left
                halfSize, -halfSize, halfSize,   0f, 0f, 1f, // 2 Bottom right
                halfSize, halfSize, halfSize,    0f, 0f, 1f, // 3 Top right

                // Right
                halfSize, halfSize, halfSize,    1f, 0f, 0f, // 4 Top left
                halfSize, -halfSize, halfSize,   1f, 0f, 0f, // 5 Bottom left
                halfSize, -halfSize, -halfSize,  1f, 0f, 0f, // 6 Bottom right
                halfSize, halfSize, -halfSize,   1f, 0f, 0f, // 7 Top right

                // Back
                halfSize, halfSize, -halfSize,   0f, 0f, -1f, // 8 Top left
                halfSize, -halfSize, -halfSize,  0f, 0f, -1f, // 9 Bottom left
                -halfSize, -halfSize, -halfSize, 0f, 0f, -1f, // 10 Bottom right
                -halfSize, halfSize, -halfSize,  0f, 0f, -1f, // 11 Top right

                // Left
                -halfSize, halfSize, -halfSize,  -1f, 0f, 0f, // 12 Top left
                -halfSize, -halfSize, -halfSize, -1f, 0f, 0f, // 13 Bottom left
                -halfSize, -halfSize, halfSize,  -1f, 0f, 0f, // 14 Bottom right
                -halfSize, halfSize, halfSize,   -1f, 0f, 0f, // 15 Top right

                // Top
                -halfSize, halfSize, -halfSize,  0f, 1f, 0f,  // 16 Top left
                -halfSize, halfSize, halfSize,   0f, 1f, 0f,  // 17 Bottom Left
                halfSize, halfSize, halfSize,    0f, 1f, 0f,  // 18 Bottom right
                halfSize, halfSize, -halfSize,   0f, 1f, 0f,   // 19 Top right

                // Bottom
                -halfSize, -halfSize, halfSize,  0f, -1f, 0f,  // 20 Top left
                halfSize, -halfSize, -halfSize,  0f, -1f, 0f,  // 21 Bottom Left
                -halfSize, -halfSize, -halfSize, 0f, -1f, 0f,  // 22 Bottom right
                halfSize, -halfSize, halfSize,   0f, -1f, 0f    // 23 Top right
            };

            mIndices = new uint[]
            {
                // Front
                0, 1, 2,     2, 3, 0,

                // Right
                4, 5, 6,     6, 7, 4,

                // Back
                8, 9, 10,    10, 11, 8,

                // Left
                12, 13, 14,  14, 15, 12,

                // Top
                16, 17, 18,  18, 19, 16,

                // Bottom
                22, 21, 20,  22, 23, 20
            };

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

    }
}
