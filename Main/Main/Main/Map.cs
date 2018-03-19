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
        public static float islandScale;
        private int edgeIslandBuffer = 50;
        private Island[] islands;


        public Map(int type, int seed)
        {
            rand = new Random(seed);
            this.type = type;
            adjFact = new int[2]; //for rendering the map centered on player
            preciseAdjFact = new float[2];
            maxAdjFact = new int[2];
            //generate random map
            Texture2D[] islandTexts;
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
                            //Console.WriteLine(game==null);
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
                    //load island texts
                    islandScale = .5f;
                    islandTexts = new Texture2D[5];
                    for (int i=0; i<islandTexts.Length; i++)
                    {
                        islandTexts[i] = game.Content.Load<Texture2D>("island" + (i + 1));
                    }
                    //put islands on map
                    List<Island> tempIslands = new List<Island>();
                    //Console.WriteLine(mapWidth + " is mapwidth");
                    //Console.WriteLine(mapHeight + " is mapHeight");
                    for (int p=0; p<50; p++)
                    {
                        int textInd = rand.Next(islandTexts.Length);
                        int tryX = rand.Next(edgeIslandBuffer, mapWidth - edgeIslandBuffer - (int)(islandTexts[textInd].Width * Math.Sqrt(islandScale)));
                        int tryY = rand.Next(edgeIslandBuffer, mapHeight - edgeIslandBuffer - (int)(islandTexts[textInd].Height * Math.Sqrt(islandScale)));
                        
                        //test for collisions with other islands
                        bool collides = false;
                        Rectangle potential = new Rectangle(tryX, tryY, (int)(islandTexts[textInd].Width * Math.Sqrt(islandScale)), (int)(islandTexts[textInd].Height*Math.Sqrt(islandScale)));
                        for (int b=0; b<tempIslands.Count(); b++)
                        {
                            if (tempIslands[b].isloc.Intersects(potential))
                            {
                                collides = true;
                                break;
                            }
                        }
                        if (!collides)
                        {
                            //Console.WriteLine("xWorkd " + tryX);
                            //Console.WriteLine("yWorkd " + tryY);
                            tempIslands.Add(new Main.Island(potential, islandTexts[textInd]));
                        }
                    }
                    islands = new Island[tempIslands.Count()];
                    for (int b=0; b<tempIslands.Count(); b++){
                        islands[b] = tempIslands[b];
                    }

                    break;
            }
        }

        

        private void drawWater ()
        {
            for (int r = 0; r < 2; r++)
            {
                for (int c = 0; c < 2; c++)
                {
                    //sb.Draw(waterT[r, c], new Rectangle(waterR[r, c].X-adjFact[0], waterR[r, c].Y - adjFact[1], waterR[r,c].Width, waterR[r,c].Height), Color.White);\
                    if (Game1.viewingPort.Intersects(new Rectangle(waterR[r,c].X, waterR[r,c].Y, waterR[r,c].Width, waterR[r,c].Height)))
                    {
                        sb.Draw(waterT[r, c], new Vector2(waterR[r, c].X - adjFact[0], waterR[r, c].Y - adjFact[1]), new Rectangle(0, 0, waterT[r, c].Width, waterT[r, c].Height), Color.White, 0f, new Vector2(0, 0), Game1.viewingScale, SpriteEffects.None, 0);
                    }
                    
                }
            }
        }
        private void drawIslands()
        {
            for (int i=0; i<islands.Length; i++)
            {
                islands[i].render();
            }
        }

        
        public void render ()
        {
            //render background
            drawWater();
            drawIslands();
        }

    }
}
