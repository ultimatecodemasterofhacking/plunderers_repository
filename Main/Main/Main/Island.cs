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
        public Rectangle isloc; //is scaled relative to map
        public Texture2D islandT;
        public Color[] islandTextureData;
        public Texture2D tmT = game.Content.Load<Texture2D>("testing_mask");
        public Decoration[] decs;
        private int decEdgeBuffer = 60;
        public static Random rand;
        private int interItemBuffer = 20;

        public Island(Rectangle IL, Texture2D t)
        {
            if (rand==null)
            {
                rand = new Random();
            }
            isloc = IL;
            islandT = t;
            islandTextureData = new Color[islandT.Width * islandT.Height];
            islandT.GetData(islandTextureData);

            //place chest first

            //now generate decs (add in a check so they don't collide with the chest
            Texture2D[] decTexts = Decoration.decTexts;
            float[] decScales = Decoration.decScales;
            List<Decoration> tempDecs = new List<Decoration>();
            List<Rectangle> tempCollCheckDecs = new List<Rectangle>();


            for (int p = 0; p < 15; p++)
            {
                int itemInd = rand.Next(decTexts.Length);
                Console.WriteLine("this itemInd is " + itemInd);
                Console.WriteLine(decTexts.Length + " " +decScales.Length);
                Console.WriteLine((isloc.Y + decEdgeBuffer) + " is less than " + (isloc.Height - decEdgeBuffer - (int)(decTexts[itemInd].Height * decScales[itemInd])));
                int tryX = rand.Next(isloc.X + decEdgeBuffer + (int)(decTexts[itemInd].Width * decScales[itemInd]/2), isloc.X + isloc.Width - decEdgeBuffer - (int)(decTexts[itemInd].Width * decScales[itemInd]/2));
                int tryY = rand.Next(isloc.Y + decEdgeBuffer + (int)(decTexts[itemInd].Height * decScales[itemInd] / 2), isloc.Y + isloc.Height - decEdgeBuffer - (int)(decTexts[itemInd].Height * decScales[itemInd]/2));

                //test for collisions with other islands
                bool collides = false;
                Rectangle potential = new Rectangle(tryX, tryY, (int)(decTexts[itemInd].Width * decScales[itemInd]), (int)(decTexts[itemInd].Height * decScales[itemInd]));
                for (int b = 0; b < tempDecs.Count(); b++)
                {
                    if (tempCollCheckDecs[b].Intersects(potential))
                    {
                        collides = true;
                        break;
                    }
                }
                
                if (!collides)
                {
                    //Console.WriteLine("xWorkd " + tryX);
                    //Console.WriteLine("yWorkd " + tryY);
                    tempDecs.Add(new Decoration(potential, itemInd));
                    tempCollCheckDecs.Add(new Rectangle(potential.X - interItemBuffer, potential.Y - interItemBuffer, potential.Width + interItemBuffer * 2, potential.Height + interItemBuffer * 2));

                }
            }
            decs = new Decoration[tempDecs.Count()];
            for (int b = 0; b < tempDecs.Count(); b++)
            {
                decs[b] = tempDecs[b];
            }
        }

        public void render()
        {
            if (Game1.viewingPort.Intersects(isloc))
            {
                sb.Draw(islandT, new Vector2(isloc.X - map.adjFact[0], isloc.Y - map.adjFact[1]), new Rectangle(0, 0, islandT.Width, islandT.Height), Color.White, 0, new Vector2(0,0), Game1.viewingScale * Map.islandScale, SpriteEffects.None, 0);
                // sb.Draw(tmT, new Rectangle(isloc.X-map.adjFact[0], isloc.Y - map.adjFact[1], (int)(isloc.Width*Game1.viewingScale), (int)(isloc.Height * Game1.viewingScale)), Color.White);
                for (int i=0; i<decs.Count(); i++)
                {
                    decs[i].render();
                }
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
