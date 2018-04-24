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
        private static HashSet<int> randomColorInd;

        public static void loadDecTexts ()
        {
            randomColorInd = new HashSet<int>();
            randomColorInd.Add(4);
            randomColorInd.Add(17);
            randomColorInd.Add(18);
            decTexts = new Texture2D[20];
            decScales = new float[20];
            decTexts[0] = game.Content.Load<Texture2D>("palmtree1");
            decScales[0] = .2f;
            decTexts[1] = game.Content.Load<Texture2D>("crab1");
            decScales[1] = .1f;
            decTexts[2] = game.Content.Load<Texture2D>("pebbles1");
            decScales[2] = .1f;
            decTexts[3] = game.Content.Load<Texture2D>("smallplant1");
            decScales[3] = .2f;
            decTexts[4] = game.Content.Load<Texture2D>("shell1");
            decScales[4] = .15f;
            decTexts[5] = game.Content.Load<Texture2D>("palmtree1");
            decScales[5] = .2f;
            decTexts[6] = game.Content.Load<Texture2D>("palmtree1"); //makes palm tree more likely
            decScales[6] = .2f;
            //misc items
            decTexts[7] = game.Content.Load<Texture2D>("miscitem1");
            decScales[7] = .15f;
            decTexts[8] = game.Content.Load<Texture2D>("miscitem2");
            decScales[8] = .15f;
            decTexts[9] = game.Content.Load<Texture2D>("miscitem3");
            decScales[9] = .15f;
            decTexts[10] = game.Content.Load<Texture2D>("miscitem4");
            decScales[10] = .15f;
            //more pebbles
            decTexts[11] = game.Content.Load<Texture2D>("pebbles2");
            decScales[11] = .1f;
            decTexts[12] = game.Content.Load<Texture2D>("pebbles3");
            decScales[12] = .1f;
            //more shells
            decTexts[13] = game.Content.Load<Texture2D>("shell2");
            decScales[13] = .15f;
            decTexts[14] = game.Content.Load<Texture2D>("shell3");
            decScales[14] = .15f;
            //more small plants
            decTexts[15] = game.Content.Load<Texture2D>("smallplant2");
            decScales[15] = .15f;
            decTexts[16] = game.Content.Load<Texture2D>("smallplant3");
            decScales[16] = .15f;
            decTexts[17] = game.Content.Load<Texture2D>("smallplant4");
            decScales[17] = .15f;
            //extras
            decTexts[18] = game.Content.Load<Texture2D>("starfish1");
            decScales[18] = .1f;
            decTexts[19] = game.Content.Load<Texture2D>("stump1");
            decScales[19] = .15f;
        }

        public Decoration (Rectangle rect, int ind)
        {
            decRect = rect;
            itemIndex = ind;
            decRot = (float)(Island.rand.NextDouble() * Math.PI * 2);
            if (randomColorInd.Contains(itemIndex))
            {
                myColor = new Color(Island.rand.Next(1, 255), Island.rand.Next(1, 255), Island.rand.Next(1, 255));
            } else
            {
                myColor = Color.White;
            }
            
        }

        public void render ()
        {
            sb.Draw(decTexts[itemIndex], new Vector2((int)((decRect.X - map.adjFact[0])*1.0f/Game1.viewingScale),(int)(( decRect.Y - map.adjFact[1])*1.0f/Game1.viewingScale)), new Rectangle(0, 0, decTexts[itemIndex].Width, decTexts[itemIndex].Height), myColor, decRot, new Vector2(decTexts[itemIndex].Width/2, decTexts[itemIndex].Height/2), 1.0f/Game1.viewingScale * decScales[itemIndex], SpriteEffects.None, 0);
        }
    }
}
