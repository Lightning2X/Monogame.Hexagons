using System;
namespace Lightning2x.Hexagons {
    // Cube coordinate system point, only used for algorithms. 
    public class CubePos {
        public CubePos(float _x, float _y, float _z) {
            x = _x;
            y = _y;
            z = _z;
        }
        public float x;
        public float y;
        public float z;

        public static AxialPos ToAxial(CubePos pos) {
            // Just drop the y coordinate since thats all axial coordinates are
            return new AxialPos(pos.x, pos.z);
        }
        public static bool isNaN(CubePos a) {
            bool nan = float.IsNaN(a.x) || float.IsNaN(a.y) || float.IsNaN(a.z);
            if (nan)
                Console.WriteLine("WARNING: a Cube Position was NaN! This should not occur normally!");
            return nan;
        }

        public static CubePos operator +(CubePos a, CubePos b) => new CubePos(a.x + b.x, a.y + b.y, a.z + b.z);

        public static CubePos operator -(CubePos a, CubePos b) => new CubePos(a.x - b.x, a.y - b.y, a.z - b.z);

        public static CubePos operator *(CubePos a, int k) => new CubePos(a.x * k, a.y * k, a.z * k);

        public static bool operator ==(CubePos a, CubePos b) => a.x == b.x && a.y == b.y && a.z == b.z;

        public static bool operator !=(CubePos a, CubePos b) => !(a == b);

        public override bool Equals(object o) {
            if (o == null)
                return false;

            var cubePos = o as CubePos;
            
            return cubePos != null && cubePos == this;
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public override string ToString() {
            return x.ToString() + " " + y.ToString() + " " + z.ToString();
        }
    }
}