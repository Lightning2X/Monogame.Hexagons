using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;
namespace Lightning2x.Hexagons {
    // Flat hexagonal grid
    public class HexagonGrid {
        // Note: fill method is not ideal and currently fills equally which results in a rhombus.
        // Might be wise to make a rectangle fill for later for easier debugging.
        public HexagonGrid() {
            AxialPos furthestPoint = Global.HexMath.PixelToAxial(new Point(Global.Graphics.PreferredBackBufferWidth, Global.Graphics.PreferredBackBufferHeight));
            int rows = (int) Utilities.Max(furthestPoint.q, furthestPoint.r);
            int cols = rows;
            // X and Y are swapped because of axial coords
            hexGrid = new Hex[rows, cols];
            for (int q = 0; q < cols; q++)
                for (int r = 0; r < rows; r++) {
                    Hex h = new Hex(defaultColor, new AxialPos(q, r));
                    hexGrid[q, r] = h;
                }
        }

        // Value class that the Hexagongrid consists of
        Color defaultColor = Color.Black;
        Hex[, ] hexGrid;

        // TODO: This is a test update to see if PixelToHex works.
        public void Update() {
            ResetColors();
            AxialPos mousePos = Global.HexMath.PixelToAxial(Mouse.GetState().Position);
            ColorOnRing(mousePos);
            ColorOnRange(mousePos);
            ColorOnLine(mousePos);
            ColorOnMouse(mousePos);
        }

        // TODO: Make a more intuitive fill method. Rigt now it is very misleading with how axial coordinates are actually represented on a 2D array.

        // Debug func
        public void Draw() {
            List<Polygon> polygonList = new List<Polygon>();
            for (int q = 0; q < hexGrid.GetLength(0); q++) {
                for (int r = 0; r < hexGrid.GetLength(1); r++) {
                    Hex current = hexGrid[r, q];
                    // Vector is a temporary offset since otherwise the first hex is out of bounds (0,0) is the center of the first hex
                    Global.S.DrawPolygon(Vector2.Zero, Global.HexMath.GeneratePolygon(current.pos), current.color, 5f);
                }
            }
        }

        public void ColorOnMouse(AxialPos mousePos) {
            Hex h = AxialToHex(mousePos);
            // Draws on current mouse position
            if (h != null)
                h.color = Color.Purple;
        }
        public void ColorOnRange(AxialPos mousePos) {
            List<AxialPos> rangeCoords = Global.HexMath.AxialCoordinateRange(mousePos, 2);
            ColorAllInList(rangeCoords, Color.LightPink);
        }

        public void ColorOnLine(AxialPos mousePos) {
            // Random begin position on the 0.25 x coordinate and 0.25 y coordinate (just some stupid hardcoded stuff)
            AxialPos beginPos = new AxialPos(MathF.Round(hexGrid.GetLength(0) / 2f, 0), MathF.Round(hexGrid.GetLength(1) / 2f, 0));
            List<AxialPos> linePositions = Global.HexMath.AxialLineDraw(beginPos, mousePos);
            // Draws a test line
            ColorAllInList(linePositions, Color.HotPink);
        }

        public void ColorOnRing(AxialPos mousePos) {
            List<AxialPos> ringPositions = Global.HexMath.AxialRing(mousePos, 6);
            ColorAllInList(ringPositions, Color.Blue);
        }

        public void ColorAllInList(List<AxialPos> list, Color color) {
            foreach (AxialPos index in list) {
                if (OutOfBounds(index))
                    continue;
                else
                    hexGrid[(int) index.q, (int) index.r].color = color;
            }
        }
        public Hex AxialToHex(AxialPos index) {
            // If the point is outside the array return null
            if (OutOfBounds(index))
                return null;
            return hexGrid[(int) index.q, (int) index.r];
        }

        public void ResetColors() {
            foreach (Hex h in hexGrid)
                h.color = defaultColor;
        }

        private bool OutOfBounds(AxialPos index) {
            if (index.q >= hexGrid.GetLength(0) || index.r >= hexGrid.GetLength(1) || index.q < 0 || index.r < 0 || AxialPos.isNaN(index))
                return true;
            else
                return false;
        }
    }

}
