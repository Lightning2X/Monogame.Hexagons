using System;
namespace Lightning2x.Hexagons {
    // Axial coordinate system point
    // Stores 2 values by using that x + y + z has to be zero for diagonal sliced cube maps.
    public class AxialPos {
        public AxialPos(float _q, float _r) {
            q = _q;
            r = _r;
        }
        // Column of the hex (x)
        public float q;
        // Row of the hex (z)
        public float r;

        public static CubePos ToCube(AxialPos pos) {
            // x + y + z has to be zero according to the aforementioned constraint
            float y = -pos.q - pos.r;
            return new CubePos(pos.q, y, pos.r);
        }
        public static bool isNaN(AxialPos a) {
            bool nan = float.IsNaN(a.q) || float.IsNaN(a.r);
            if (nan)
                Console.WriteLine("WARNING: an Axial Position was NaN! This should not occur normally!");
            return nan;
        }

        public static AxialPos operator +(AxialPos a, AxialPos b) => CubePos.ToAxial(AxialPos.ToCube(a) + AxialPos.ToCube(b));
        public static AxialPos operator -(AxialPos a, AxialPos b) => CubePos.ToAxial(AxialPos.ToCube(a) + AxialPos.ToCube(b));
        public static AxialPos operator *(AxialPos a, int k) => CubePos.ToAxial(AxialPos.ToCube(a) * k);
        public static bool operator ==(AxialPos a, AxialPos b) => AxialPos.ToCube(a) == AxialPos.ToCube(b);
        public static bool operator !=(AxialPos a, AxialPos b) => !(a == b);

        public override bool Equals(object o) {
            if (o == null)
                return false;

            var axialPos = o as AxialPos;
            return axialPos != null && axialPos == this;
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public override string ToString() {
            return q.ToString() + " " + r.ToString(); 
        }
    }
}