﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Main
{
    class Island:Drawable
    {
        public Rectangle isloc;
        public Texture2D islandT;

        public Island(Rectangle IL, Texture2D t)
        {
            isloc = IL;
            islandT = t;
        }

        public void render()
        {
            sb.Draw(islandT, new Vector2(isloc.X - map.adjFact[0], isloc.Y - map.adjFact[1]), new Rectangle(0, 0, islandT.Width, islandT.Height), Color.White, 0, new Vector2(isloc.Width / 2, isloc.Height / 2), Game1.viewingScale * Map.islandScale, SpriteEffects.None, 0);
        }

    }
}
