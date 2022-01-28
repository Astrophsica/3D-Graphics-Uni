using OpenTK;
using System;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Labs.Utility;

namespace Labs.ACW
{
    class Cone:DrawableObject
    {

        public Cone(float pRadius, int pNumSegments, float topY, Matrix4 pMatrix, Material pMaterial, ShaderUtility pShader, int pPositionLocation, int pNormalLocation)
        {
            // Gets vector positions for circle
            Vector3[] circleVectors = CreateCircleVectors(pRadius, pNumSegments, 0);

            // Initialise variables
            int verticesArraySize = (pNumSegments * 6) + 6;
            mVertices = new float[verticesArraySize];

            // Sets top of cone point and normal
            mVertices[0] = 0;
            mVertices[1] = topY;
            mVertices[2] = 0;

            mVertices[3] = 0;
            mVertices[4] = topY;
            mVertices[5] = 0;

            // Adds circle vertices from "circleVectors"
            for (int i = 6; i < verticesArraySize; i = i + 6)
            {
                int circleVectorIndex = (i / 6) - 1;
                mVertices[i] = circleVectors[circleVectorIndex].X;
                mVertices[i + 1] = circleVectors[circleVectorIndex].Y;
                mVertices[i + 2] = circleVectors[circleVectorIndex].Z;

                mVertices[i + 3] = circleVectors[circleVectorIndex].X;
                mVertices[i + 4] = circleVectors[circleVectorIndex].Y;
                mVertices[i + 5] = circleVectors[circleVectorIndex].Z;
            }

            // Initialise indices array
            mIndices = new uint[circleVectors.Length + 2];

            // Set first index to top of cone
            mIndices[0] = 0;

            // Adds circle indices in opposite direction (So that it is shown the correct way)
            uint countDown = (uint)circleVectors.Length;
            for (int i = 1; i <= circleVectors.Length; i++)
            {
                mIndices[i] = countDown;
                countDown = countDown - 1;
            }

            // Last index to complete cone
            mIndices[mIndices.Length - 1] = (uint)circleVectors.Length;


            mMatrix = pMatrix;
            mMaterial = pMaterial;
            mShader = pShader;
            mPrimitiveType = PrimitiveType.TriangleFan;

            base.BindArrayObject();
            base.BindArrayBufferObject();
            base.BindElementArrayBufferObject();

            GL.EnableVertexAttribArray(pPositionLocation);
            GL.VertexAttribPointer(pPositionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(pNormalLocation);
            GL.VertexAttribPointer(pNormalLocation, 3, VertexAttribPointerType.Float, true, 6 * sizeof(float), 3 * sizeof(float));
        }


        /// <summary>
        /// Returns cirvle vectors (Originally from 2D Graphics ACW)
        /// </summary>
        private Vector3[] CreateCircleVectors(float pRadius, int pNumSegments, float y)
        {
            Vector3[] vectors = new Vector3[pNumSegments];
            double anglePerSegment = Math.PI * 2 / pNumSegments;

            for (var i = 0; i < pNumSegments; i = i + 1)
            {
                double angle = anglePerSegment * i;
                float x = (float)(pRadius * Math.Cos(angle));
                float z = (float)(pRadius * Math.Sin(angle));

                vectors[i] = new Vector3(x, y, z);
            }

            return vectors;
        }
    }
}
