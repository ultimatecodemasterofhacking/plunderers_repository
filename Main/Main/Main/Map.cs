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
    class Map : Drawable
    {
        private Random rand;
        private int type;
        private Texture2D[,] waterT;
        private Rectangle[,] waterR;
        public int waeY = 1; //1 is up, 2 is down, -1 is not
        public int waeX = 1; //1 is left, 2 is right, -1 is not
        public int mapWidth = 0;
        public int mapHeight = 0;
        public int[] adjFact;
        public int[] maxAdjFact;
        public float[] preciseAdjFact;


        public Map(int type, int seed)
        {
            rand = new Random(seed);
            this.type = type;
            adjFact = new int[2]; //for rendering the map centered on player
            preciseAdjFact = new float[2];
            maxAdjFact = new int[2];
            //generate random map
            switch (type)
            {
                case 0:
                    waterT = new Texture2D[2, 2];
                    waterR = new Rectangle[2, 2];
                    int[] backDims = new int[2];
                    for (int r=0; r<2; r++)
                    {
                        for (int c=0; c<2; c++)
                        {
                            //fix
                            waterT[c,r] = game.Content.Load<Texture2D>("ocean-" + r + "-" + c);
                            if (r == 0 && c == 0)
                            {
                                backDims[0] = waterT[r, c].Width;
                                backDims[1] = waterT[r, c].Height;
                            }
                        }
                    }
                    for (int r = 0; r < 2; r++)
                    {
                        for (int c = 0; c < 2; c++)
                        {
                            waterR[r, c] = new Rectangle(r*backDims[0], c*backDims[1], backDims[0], backDims[1]);
                            
                        }
                    }
                    mapWidth = backDims[0] * 2;
                    mapHeight = backDims[1] * 2;
                    maxAdjFact[0] = mapWidth - Game1.dim[0];
                    maxAdjFact[1] = mapHeight - Game1.dim[1];
                    break;
            }
        }

        

        private void drawWater ()
        {
            for (int r = 0; r < 2; r++)
            {
                for (int c = 0; c < 2; c++)
                {
                    sb.Draw(waterT[r, c], new Rectangle(waterR[r, c].X-adjFact[0], waterR[r, c].Y - adjFact[1], waterR[r,c].Width, waterR[r,c].Height), Color.White);

                }
            }
        }

        
        public void render ()
        {
            //render background
            drawWater();
        }

    }
}
