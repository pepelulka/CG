using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace cg_lab1
{
    internal class Utils
    {
        public static Vector2 Rotate(Vector2 v, float angle)
        {
            return new Vector2(
                v.X * (float)Math.Cos(angle) - v.Y * (float)Math.Sin(angle),
                v.X * (float)Math.Sin(angle) + v.Y * (float)Math.Cos(angle)
            );
        }

        public static void GeneratePolygon(int n, float radius, out float[] outVertices, out uint[] outIndices)
        {
            var vertices = new float[2 * n];
            var indices = new uint[3 * (n - 2)];
            Vector2 vector = new Vector2(0, radius);
            float rotationAngle = 2 * (float)Math.PI / n;
            // Vertices
            for (int i = 0; i < n; i++)
            {
                vertices[2 * i] = vector.X;
                vertices[2 * i + 1] = vector.Y;
                vector = Rotate(vector, rotationAngle);
            }
            // Indices
            for (uint i = 0; i < n - 2; i++)
            {
                indices[3 * i] = 0;
                indices[3 * i + 1] = i + 1;
                indices[3 * i + 2] = i + 2;
            }
            // Pushing result
            outVertices = vertices;
            outIndices = indices;
        }

    }
}
