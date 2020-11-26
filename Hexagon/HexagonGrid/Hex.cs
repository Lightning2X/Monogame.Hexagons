using Microsoft.Xna.Framework;

namespace Lightning2x.Hexagons {
    public class Hex {
        public Color color;
        public AxialPos pos;
        public Hex(Color _color, AxialPos _pos) {
            color = _color;
            pos = _pos;
        }
    }
}
