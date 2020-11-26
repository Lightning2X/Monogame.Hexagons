using System;
using System.Linq;
using Microsoft.Xna.Framework;
namespace Lightning2x.Hexagons {
    public class Utilities {
        public static T Max<T>(params T[] numbers) where T : IComparable {
            return numbers.Max();
        }

        public static Vector2 MultiplyVectorWithMatrix(Matrix2x2 mat, Vector2 vec) {
            float newX = vec.X * mat.a + vec.Y * mat.b;
            float newY = vec.X * mat.c + vec.Y * mat.d;
            return new Vector2(newX, newY);
        }

        public static Matrix2x2 MultiplyScalarWithMatrix(Matrix2x2 mat, float scalar) {
            return new Matrix2x2(mat.a * scalar, mat.b * scalar, mat.c * scalar, mat.d * scalar);
        }

        public static T[, ] FillWithNull<T>(T[, ] array) {
            for (int x = 0; x < array.GetLength(0); x++) {
                for (int y = 0; y < array.GetLength(1); y++) {
                    array[x, y] = default(T);
                }
            }
            return array;
        }

        public static Vector2 Floor(Vector2 vec) {
            return new Vector2(MathF.Floor(vec.X), MathF.Floor(vec.Y));
        }
    }

    // Simple 2x2 float matrix
    public struct Matrix2x2 {
        // Structure of the matrix is like this:
        // a b
        // c d
        public float a, b, c, d;
        public Matrix2x2(float _a, float _b, float _c, float _d) {
            a = _a;
            b = _b;
            c = _c;
            d = _d;
        }
        // Inits a matrix with row1 = a b and row2 = c d
        public Matrix2x2(Vector2 row1, Vector2 row2) {
            a = row1.X;
            b = row1.Y;
            c = row2.X;
            d = row2.Y;
        }
    }

    public enum HexOrientation {
        Pointy,
        Flat
    }
}
