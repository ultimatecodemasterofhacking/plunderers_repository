using System;
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
        public Color[] islandTextureData;
        public Texture2D tmT = game.Content.Load<Texture2D>("testing_mask");

        public Island(Rectangle IL, Texture2D t)
        {
            isloc = IL;
            islandT = t;
            islandTextureData = new Color[islandT.Width * islandT.Height];
            islandT.GetData(islandTextureData);
        }

        public void render()
        {
            if (Game1.viewingPort.Intersects(isloc))
            {
                sb.Draw(islandT, new Vector2(isloc.X - map.adjFact[0], isloc.Y - map.adjFact[1]), new Rectangle(0, 0, islandT.Width, islandT.Height), Color.White, 0, new Vector2(0,0), Game1.viewingScale * Map.islandScale, SpriteEffects.None, 0);
               // sb.Draw(tmT, new Rectangle(isloc.X-map.adjFact[0], isloc.Y - map.adjFact[1], (int)(isloc.Width*Game1.viewingScale), (int)(isloc.Height * Game1.viewingScale)), Color.White);
            }
            
        }

        public bool decide()
        {
            if (Game1.viewingPort.Intersects(isloc))
            {
                return true;
            }
            return false;
        }

    }
}
