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
    class Decoration :Drawable
    {
        public Rectangle decRect;
        public static Texture2D[] decTexts;
        public static float[] decScales; //map relative
        private int itemIndex = 0;
        private float decRot = 0;
        private Color myColor;

        public static void loadDecTexts ()
        {
            decTexts = new Texture2D[2];
            decScales = new float[2];
            decTexts[0] = game.Content.Load<Texture2D>("palmtree");
            decScales[0] = .3f;
            decTexts[1] = game.Content.Load<Texture2D>("crab");
            decScales[1] = .2f;
        }

        public Decoration (Rectangle rect, int ind)
        {
            decRect = rect;
            itemIndex = ind;
            decRot = (float)(Island.rand.NextDouble() * Math.PI * 2);
            if (itemIndex == 1)
            {
                myColor = new Color(Island.rand.Next(1, 255), Island.rand.Next(1, 255), Island.rand.Next(1, 255));
            } else
            {
                myColor = Color.White;
            }
            
        }

        public void render ()
        {
            sb.Draw(decTexts[itemIndex], new Vector2(decRect.X - map.adjFact[0], decRect.Y - map.adjFact[1]), new Rectangle(0, 0, decTexts[itemIndex].Width, decTexts[itemIndex].Height), myColor, decRot, new Vector2(decTexts[itemIndex].Width/2, decTexts[itemIndex].Height/2), Game1.viewingScale * decScales[itemIndex], SpriteEffects.None, 0);
        }
    }
}
