using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Main
{
    abstract class Drawable
    {
        protected static SpriteBatch sb;
        protected static Game game;
        protected static int width;
        protected static int height;
        protected static Map map;

        public static void setup (SpriteBatch ssbb, Game g, int w, int h)
        {
            sb = ssbb;
            game = g;
            width = w;
            height = h;

        }

        public static void setMap (Map m)
        {
            map = m;
        }

        
    }
}
