using Labs.Utility;
using System;
using System.Collections.Generic;
using OpenTK;

using OpenTK.Graphics.OpenGL;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labs.ACW
{
    class DrawableObject
    {

        public int mVAO_ID;
        protected int[] mVBO_IDs = new int[2];
        protected uint[] mIndices;
        protected float[] mVertices;
        public int mDrawCount;
        public Matrix4 mMatrix;
        public Material mMaterial;
        protected ShaderUtility mShader;
        protected PrimitiveType mPrimitiveType;
        protected bool mElementArrayBufferEnabled = false;

        public void BindArrayObject()
        {
            GL.UseProgram(mShader.ShaderProgramID);
            mVAO_ID = GL.GenVertexArray();

            GL.BindVertexArray(mVAO_ID);
        }

        public void BindArrayBufferObject()
        {
            GL.UseProgram(mShader.ShaderProgramID);
            mVBO_IDs[0] = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, mVBO_IDs[0]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(mVertices.Length * sizeof(float)), mVertices, BufferUsageHint.StaticDraw);

            int bufferSize;
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out bufferSize);
            if (mVertices.Length * sizeof(float) != bufferSize)
            {
                throw new ApplicationException("Vertex data not loaded onto graphics card correctly");
            }
        }

        public void BindElementArrayBufferObject()
        {
            GL.UseProgram(mShader.ShaderProgramID);
            mVBO_IDs[1] = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mVBO_IDs[1]);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(mIndices.Length * sizeof(int)), mIndices, BufferUsageHint.StaticDraw);

            int bufferSize;
            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out bufferSize);
            if (mIndices.Length * sizeof(int) != bufferSize)
            {
                throw new ApplicationException("Index data not loaded onto graphics card correctly");
            }

            mDrawCount = mIndices.Length;
            mElementArrayBufferEnabled = true;
        }

        public virtual void Draw()
        {
            if (mElementArrayBufferEnabled == false)
            {
                GL.UseProgram(mShader.ShaderProgramID);
                mMaterial.UseMaterial();
                GL.BindVertexArray(mVAO_ID);
                GL.DrawArrays(mPrimitiveType, 0, mDrawCount);
            }
            else
            {
                GL.UseProgram(mShader.ShaderProgramID);
                mMaterial.UseMaterial();
                GL.BindVertexArray(mVAO_ID);
                GL.DrawElements(mPrimitiveType, mDrawCount, DrawElementsType.UnsignedInt, 0);
            }
        }

        public void Unload()
        {
            GL.DeleteVertexArray(mVAO_ID);
            GL.DeleteBuffers(mVBO_IDs.Length, mVBO_IDs);
        }

    }
}
