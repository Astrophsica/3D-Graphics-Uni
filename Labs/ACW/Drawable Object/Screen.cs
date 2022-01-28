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
    class Screen:DrawableObject
    {

        public int mColorBuffer_ID;

        public Screen(ShaderUtility pShader, int pColorBuffer_ID, int pScreenPositionLocation, int pScreenTexCoordsLocation)
        {
            mVertices = new float[]
            {
            -1.0f, 1.0f,       0.0f, 1.0f, // Top left
            -1.0f, -1.0f,      0.0f, 0.0f, // Bottom left
            1.0f, -1.0f,       1.0f, 0.0f, // Bottom right
            1.0f, 1.0f,        1.0f, 1.0f  // Top right  
            };

            mColorBuffer_ID = pColorBuffer_ID;
            mShader = pShader;
            mPrimitiveType = PrimitiveType.TriangleFan;
            mDrawCount = 4;

            base.BindArrayObject();
            base.BindArrayBufferObject();

            GL.EnableVertexAttribArray(pScreenPositionLocation);
            GL.VertexAttribPointer(pScreenPositionLocation, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);

            GL.EnableVertexAttribArray(pScreenTexCoordsLocation);
            GL.VertexAttribPointer(pScreenTexCoordsLocation, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
        }

        public override void Draw()
        {
            GL.UseProgram(mShader.ShaderProgramID);
            GL.BindVertexArray(mVAO_ID);
            GL.Disable(EnableCap.DepthTest);
            GL.BindTexture(TextureTarget.Texture2D, mColorBuffer_ID);

            GL.DrawArrays(PrimitiveType.TriangleFan, 0, mDrawCount);
        }

    }
}
