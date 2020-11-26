using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lightning2x.Hexagons
{
    public static class Global
    {
        public static SpriteBatch S;
        public static Game Game;
        public static GraphicsDeviceManager Graphics;
        public static HexMath HexMath = new HexMath(HexOrientation.Pointy);
        public static void Setup(Game game, GraphicsDeviceManager _graphics, SpriteBatch _spriteBatch)
        {
            Game = game;
            Graphics = _graphics;
            S = _spriteBatch;
            HexMath = new HexMath(HexOrientation.Pointy);
        }

    }
}
