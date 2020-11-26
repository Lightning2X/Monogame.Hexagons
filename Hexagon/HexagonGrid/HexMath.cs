using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;
namespace Lightning2x.Hexagons {
    // Class that represents common hex math methods
    public class HexMath {

        public HexMath(HexOrientation _orientation) {
            orientation = _orientation;
            RecalculateConversionMatrices();
        }

        // Size of hexes in px. Default is 30 pixels
        private float _size = 30f;
        // The amount a vertices a hex has is always 6
        private readonly int hexVerticeAmount = 6;

        // Directions you can move in from one hex used for neighbours.
        private readonly CubePos[] cubeDirections = {
            new CubePos(1, -1, 0),
            new CubePos(1, 0, -1),
            new CubePos(0, 1, -1),
            new CubePos(-1, 1, 0),
            new CubePos(-1, 0, 1),
            new CubePos(0, -1, 1)
        };

        // Hex type variables
        // Matrix that converts axial coordinates to screen space coordinates
        private Matrix2x2 hexToPixelMatrix;
        // Inverse matrix of hexToPixel
        private Matrix2x2 pixelToHexMatrix;
        // Angle offset of a hex flat top = 0, pointy = 30 degrees.
        private int angleOffset;

        private HexOrientation orientation;

        // Offset in pixels
        private Vector2 offset = Vector2.Zero;

        // Converts an axial position to a pixel position
        public Vector2 AxialToPixel(AxialPos pos) {
            Vector2 pixelPos = Utilities.MultiplyVectorWithMatrix(hexToPixelMatrix, AxialToVector2(pos));
            pixelPos += offset;
            return pixelPos;
        }

        // Returns closest axial position to a pixel
        public AxialPos PixelToAxial(Point px) {
            Vector2 hexPos = Utilities.MultiplyVectorWithMatrix(pixelToHexMatrix, new Vector2(px.X, px.Y) - offset);
            AxialPos index = AxialRoundToNearestHex(Vector2ToAxial(hexPos));
            return index;
        }

        // Returns the AxialPos based on a given direction
        public AxialPos AxialNeighbour(AxialPos hex, int direction) => CubeToAxial(CubeNeighbour(AxialToCube(hex), direction));

        // Generates the 6 vertices of a hexagon given the position for this hexagon
        public Polygon GeneratePolygon(AxialPos hexPos) {
            List<Vector2> vertices = new List<Vector2>();
            for (int i = 0; i < hexVerticeAmount; i++) {
                vertices.Add(GetHexCorner(AxialToPixel(hexPos), i));
            }
            return new Polygon(vertices);
        }

        public List<AxialPos> AxialLineDraw(AxialPos a, AxialPos b) {
            List<CubePos> linePositions = CubeLineDraw(AxialToCube(a), AxialToCube(b));
            return CubeCollectionToAxial(linePositions);
        }

        public List<AxialPos> AxialCoordinateRange(AxialPos center, int range) {
            List<CubePos> rangeCoords = CubeCoordinateRange(AxialToCube(center), range);
            return CubeCollectionToAxial(rangeCoords);
        }

        public List<AxialPos> AxialRing(AxialPos center, int radius) {
            List<CubePos> ringCoords = CubeRing(AxialToCube(center), radius);
            return CubeCollectionToAxial(ringCoords);
        }

        public void ChangeHexOrientation(HexOrientation newOrientation) {
            orientation = newOrientation;
            RecalculateConversionMatrices();
        }

        // Global truths
        public float HexWidth {
            get {
                if (orientation == HexOrientation.Flat) {
                    return _size * 2f;
                } else {
                    return _size * MathF.Sqrt(3f);
                }
            }
            set {
                if (orientation == HexOrientation.Flat) {
                    PixelSize = value / 2f;
                } else {
                    PixelSize = value / MathF.Sqrt(3f);
                }
            }
        }
        public float HexHeight {
            get {
                if (orientation == HexOrientation.Flat) {
                    return _size * MathF.Sqrt(3f);
                } else {
                    return _size * 2f;
                }
            }
            set {
                if (orientation == HexOrientation.Flat) {
                    PixelSize = value / MathF.Sqrt(3f);
                } else {
                    PixelSize = value / 2f;
                }
            }
        }

        public float HexVerticeAmount => hexVerticeAmount;

        public float PixelSize {
            get => _size;
            set {
                _size = value;
                RecalculateConversionMatrices();
            }
        }

        public HexOrientation CurrentOrientation => orientation;

        // Offset in pixels (whole) Setter ensures that no fractional values appear in the offset
        public Vector2 Offset { get => offset; set => offset = Utilities.Floor(value); }

        // Gets the corner of a hex given the iteration of such a corner (so 3 represents the third corner)
        private Vector2 GetHexCorner(Vector2 center, int corner) {
            float angleDegrees = 60f * corner - angleOffset;
            float angleRadians = (MathF.PI / 180f) * angleDegrees;
            // Might have to round this later?
            return new Vector2(center.X + _size * MathF.Cos(angleRadians),
                center.Y + _size * MathF.Sin(angleRadians));
        }

        private void RecalculateConversionMatrices() {
            if (orientation == HexOrientation.Flat) {
                hexToPixelMatrix = new Matrix2x2(3f / 2f, 0f, MathF.Sqrt(3f) / 2f, MathF.Sqrt(3f));
                pixelToHexMatrix = new Matrix2x2(2f / 3f, 0f, -1f / 3f, 1f / MathF.Sqrt(3f));
                angleOffset = 0;
            } else {
                hexToPixelMatrix = new Matrix2x2(MathF.Sqrt(3f), MathF.Sqrt(3f) / 2f, 0, 3f / 2f);
                pixelToHexMatrix = new Matrix2x2(1f / MathF.Sqrt(3f), -1f / 3f, 0, 2f / 3f);
                angleOffset = 30;
            }
            hexToPixelMatrix = Utilities.MultiplyScalarWithMatrix(hexToPixelMatrix, _size);
            pixelToHexMatrix = Utilities.MultiplyScalarWithMatrix(pixelToHexMatrix, 1f / _size);
        }

        // Axial methods

        // Returns a direction given an int.
        private AxialPos AxialDirection(int direction) => CubeToAxial(CubeDirection(direction));

        // Linear interpolation for axial positions
        public AxialPos AxialLerp(AxialPos a, AxialPos b, float t) => CubeToAxial(CubeLerp(AxialToCube(a), AxialToCube(b), t));

        private AxialPos AxialRoundToNearestHex(AxialPos axialPos) => CubeToAxial(CubeRoundToNearestHex(AxialToCube(axialPos)));
        public float AxialDistance(AxialPos a, AxialPos b) => CubeDistance(AxialToCube(a), AxialToCube(b));

        private List<AxialPos> CubeCollectionToAxial(List<CubePos> ls) {
            List<AxialPos> results = new List<AxialPos>();
            foreach (CubePos cubePos in ls)
                results.Add(CubeToAxial(cubePos));
            return results;
        }

        private List<CubePos> AxialCollectionToCube(List<AxialPos> ls) {
            List<CubePos> results = new List<CubePos>();
            foreach (AxialPos axialPos in ls)
                results.Add(AxialToCube(axialPos));
            return results;
        }

        private Vector2 AxialToVector2(AxialPos pos) => new Vector2(pos.q, pos.r);
        private AxialPos Vector2ToAxial(Vector2 pos) => new AxialPos(pos.X, pos.Y);
        private AxialPos CubeToAxial(CubePos cubePos) => CubePos.ToAxial(cubePos);
        private CubePos AxialToCube(AxialPos axialPos) => AxialPos.ToCube(axialPos);

        // Cube coordinate methods

        // Gets all positions within range steps of a given position
        private List<CubePos> CubeCoordinateRange(CubePos center, int range) {
            // Nothing is either in range of the square itself or negative ranges so we just return an empty list
            if (range <= 0)
                return new List<CubePos>();
            List<CubePos> results = new List<CubePos>();
            for (int x = -range; x <= range; x++) {
                for (int y = -range; y <= range; y++) {
                    for (int z = -range; z <= range; z++) {
                        if (x + y + z == 0)
                            results.Add(center + new CubePos(x, y, z));
                    }
                }
            }
            return results;
        }

        // Returns the hexes that are contained in a ring in radius Axial positions away from the given position
        private List<CubePos> CubeRing(CubePos center, int radius) {
            // There is no ring that "is" the center itself so 0 and lower are invalid values and we return an empty list.
            if (radius <= 0)
                return new List<CubePos>();
            List<CubePos> results = new List<CubePos>();
            CubePos currentPos = center + CubeDirection(4) * radius;
            for (int i = 0; i < 6; i++) {
                for (int j = 0; j < radius; j++) {
                    results.Add(currentPos);
                    currentPos = CubeNeighbour(currentPos, i);
                }
            }
            return results;
        }
        private CubePos CubeNeighbour(CubePos hex, int direction) {
            CubePos cubeDir = CubeDirection(direction);
            return hex + cubeDir;
        }

        // Gives a list of positions that make up a line given a beginning position (a) and an ending position (b)
        private List<CubePos> CubeLineDraw(CubePos a, CubePos b) {
            float hexDistance = CubeDistance(a, b);
            // If distance is 0 nothing can be drawn (NaN would be returned)
            if (hexDistance <= 0)
                return new List<CubePos>();
            List<CubePos> results = new List<CubePos>();
            for (int i = 0; i <= hexDistance; i++) {
                results.Add(CubeRoundToNearestHex(CubeLerp(a, b, (1f / hexDistance) * i)));
            }
            return results;
        }

        // Cube coordinate linear interpolation
        private CubePos CubeLerp(CubePos a, CubePos b, float t) {
            return new CubePos(MathHelper.Lerp(a.x, b.x, t),
                MathHelper.Lerp(a.y, b.y, t),
                MathHelper.Lerp(a.z, b.z, t));
        }

        private CubePos CubeDirection(int direction) => cubeDirections[direction];

        // Round fractional coords to the nearest hex
        private CubePos CubeRoundToNearestHex(CubePos fracPos) {
            float rX = MathF.Round(fracPos.x);
            float rY = MathF.Round(fracPos.y);
            float rZ = MathF.Round(fracPos.z);

            float xDifference = MathF.Abs(rX - fracPos.x);
            float yDifference = MathF.Abs(rY - fracPos.y);
            float zDifference = MathF.Abs(rZ - fracPos.z);

            if (xDifference > yDifference && xDifference > zDifference)
                rX = -rY - rZ;
            else if (yDifference > zDifference)
                rY = -rX - rZ;
            else
                rZ = -rX - rY;

            return new CubePos(rX, rY, rZ);
        }

        private float CubeDistance(CubePos a, CubePos b) => Utilities.Max(MathF.Abs(a.x - b.x), MathF.Abs(a.y - b.y), MathF.Abs(a.z - b.z));

    }
}
