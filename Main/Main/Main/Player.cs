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
    class Player : Drawable
    {
        public static bool charForm;
        private int shipType;

        private Texture2D[] shipT;
        private Rectangle shipR;
        private float shipScale;
        private float charScale;
        public float shipSpeed = 0;
        private float maxShipSpeed = 8;
        private float shipDeccel = -.08f;
        private float shipAcc = .08f;
        private Map map;
        private int[] targetDest = new int[2];
        private static int moveErrorMargin = 12;
        private int[] potDest = new int[2];
        private int[] fromShipCenterVariation = new int[2];
        private bool decellMode = false;
        private float shipHeadingRad = 0;
        private int shipInd = 0;
        private float currSheading = 0;
        private float destSheading = 0;

        public Player (int shipType, int pNum, Map map)
        {
            this.map = map;
            charForm = false;
            this.shipType = shipType;
            shipScale = .15f;
            charScale = .1f;
            targetDest[0] = -1;
            targetDest[1] = -1;
            fromShipCenterVariation[0] = 7;
            fromShipCenterVariation[1] = 5;
            shipSpeed = 0;
            decellMode = false;
            shipHeadingRad = 0;
            switch(shipType)
            {
                case 0:
                    shipInd = 0;
                    shipT = new Texture2D[1];
                    shipT[0] = game.Content.Load<Texture2D>("ship1");
                    shipR = new Rectangle(0 , 0, (int)(shipT[0].Width * shipScale), (int)(shipT[0].Height*shipScale));
                    
                    break;
            }
            switch (pNum)
            {
                case 1:
                    shipR.X = Game1.dim[0] / 2 - shipR.Width / 2;
                    shipR.Y = Game1.dim[1] / 2 - shipR.Height / 2; ;
                    Map.adjFact[0] = 0;
                    Map.adjFact[1] = 0;
                    currSheading = 0;
                    destSheading = 0;
                    break;
            }
        }

        public int[] centerOnMap ()
        {
            int[] cent = {shipR.X+shipR.Width/2, shipR.Y + shipR.Height/2 };
            return cent;
        }

        public int[] centerRelScreen()
        {
            int[] cent = { shipR.X + shipR.Width / 2 - Map.adjFact[0], shipR.Y + shipR.Height / 2 - Map.adjFact[1] };
            return cent;
        }

        public void manageShipSpeed()
        {
            
            if (!decellMode)
            {
                shipSpeed += shipAcc;
                if (shipSpeed > maxShipSpeed)
                {
                    shipSpeed = maxShipSpeed;
                }
                
            } else
            {
                Console.WriteLine("hapnin");

                shipSpeed += shipDeccel;
                if (shipSpeed < 0)
                {
                    shipSpeed = 0;
                    decellMode = false;
                    targetDest[0] = -1;
                    targetDest[1] = -1;

                }
            }
        }

        public void move ()
        {
            Console.WriteLine("TD= " + targetDest[0] + " " + targetDest[1]);
            Console.WriteLine("Decell = " + decellMode);
            Console.WriteLine("Speed = " + shipSpeed);

            if (targetDest[0] >=0 || targetDest[1] >=0)
            {
                Console.WriteLine("MOVING");

                if (!charForm)
                {
                    manageShipSpeed();
                    int[] pCent = centerOnMap();

                    double hype = Math.Sqrt(Math.Pow(targetDest[0] - pCent[0], 2) + Math.Pow(targetDest[1] - pCent[1], 2));
                    double z = -1;
                    int xAmt = 0;
                    int yAmt = 0;
                    if (hype == 0)
                    {
                        if (targetDest[0] > shipR.X)
                        {
                            xAmt = (int)shipSpeed;
                        } else if (targetDest[0] < shipR.X)
                        {
                            xAmt = (int)-shipSpeed;
                        }

                        if (targetDest[1] > shipR.Y)
                        {
                            yAmt = (int)shipSpeed;
                        }
                        else if (targetDest[1] < shipR.Y)
                        {
                            yAmt = (int)-shipSpeed;
                        }
                    }
                    else
                    {
                        z = shipSpeed / hype; //constant to get directional components

                        xAmt = (int)(z * (targetDest[0] - pCent[0]));
                        yAmt = (int)(z * (targetDest[1] - pCent[1]));
                    }


                    //
                    int tempShipRX = shipR.X;
                    int tempShipRY = shipR.Y;
                    tempShipRX += (int)xAmt;
                    tempShipRY += (int)yAmt;
                    
                    //ensure move is within boundaries
                    if (tempShipRX < 0)
                    {
                        xAmt = -tempShipRX;
                        tempShipRX = 0;
                        tempShipRY = shipR.Y;
                        yAmt = 0;
                    }
                    if (tempShipRY < 0)
                    {
                        yAmt = -tempShipRY;
                        tempShipRY = 0;
                        tempShipRX = shipR.X;
                        xAmt = 0;
                    }
                    if (tempShipRX + shipR.Width >= map.mapWidth)
                    {
                        xAmt = map.mapWidth - shipR.Width - (tempShipRX);
                        tempShipRX = map.mapWidth - shipR.Width;
                        tempShipRY = shipR.Y;
                        yAmt = 0;
                    }
                    if (tempShipRY + shipR.Height >= map.mapHeight)
                    {
                        yAmt = map.mapHeight - shipR.Height - (tempShipRY);
                        tempShipRY = map.mapHeight - shipR.Height;
                        tempShipRX = shipR.X;
                        xAmt = 0;
                    }
                    //determine adjustment factor for rendering
                    //Console.WriteLine("real amounts of movement: " + xAmt + " " + yAmt);
                    shipR.X = tempShipRX;
                    shipR.Y = tempShipRY;

                    //map.adjFact[0] += (int)xAmt;
                    //map.adjFact[1] += (int)yAmt;
                    ensureCameraWithinBoundaries((int)xAmt, (int)yAmt);
                    if (pCent[0] < targetDest[0]+moveErrorMargin && pCent[0]> targetDest[0] - moveErrorMargin && pCent[1] < targetDest[1] + moveErrorMargin && pCent[1] > targetDest[1] - moveErrorMargin)
                    {
                        if (!decellMode)
                        {
                            decellMode = true;
                            //set new target
                            int displacement = (int)((-1 * shipSpeed * shipSpeed) / (2 * shipDeccel));
                            Console.WriteLine("DISPLACEMENT - " + displacement);
                            float n = displacement/shipSpeed;
                            int moreXAmt = (int)(n * xAmt);
                            int moreYAmt = (int)(n * yAmt);
                            targetDest[0] += moreXAmt;
                            targetDest[1] += moreYAmt;
                        } else
                        {
                            decellMode = false;
                            targetDest[0] = -1;
                            targetDest[1] = -1;
                        }
                        
                        //set new targetdest
                        Console.WriteLine("---We're there!");
                        
                    }

                }

            } else
            {
                
                Console.WriteLine("Not moving");
            }
        }

        public void shipNoClick ()
        {
            decellMode = true;
        }



        public void setTarget(int mX, int mY)
        {
            potDest[0] = mX + Map.adjFact[0];

            potDest[1] = mY + Map.adjFact[1];
            int[] pCent = centerOnMap();
            decellMode = false;
            if (shipSpeed<1)
            {
                shipSpeed = 3;
            }
            

            if (!charForm)
            {
                if (potDest[0] > map.mapWidth - shipR.Width/2)
                {
                    potDest[0] = map.mapWidth - shipR.Width / 2;
                }

                if (potDest[1] > map.mapHeight - shipR.Height / 2)
                {
                    potDest[1] = map.mapHeight - shipR.Height / 2;
                }

                if (potDest[0]<shipR.Width/2)
                {
                    potDest[0] = shipR.Width / 2;
                }

                if (potDest[1] < shipR.Height / 2)
                {
                    potDest[1] = shipR.Height / 2;
                }

                if (pCent[0] < potDest[0] + moveErrorMargin && pCent[0] > potDest[0] - moveErrorMargin && pCent[1] < potDest[1] + moveErrorMargin && pCent[1] > potDest[1] - moveErrorMargin)
                {
                    potDest[0] = -1;
                    potDest[1] = -1;

                }

                Rectangle moose = new Rectangle(mX, mY, 1, 1);
                Rectangle tempShipCenter = new Rectangle(centerRelScreen()[0]-fromShipCenterVariation[0], centerRelScreen()[1]-fromShipCenterVariation[1], fromShipCenterVariation[0]*2, fromShipCenterVariation[1]*2);
                if (moose.Intersects(tempShipCenter))
                {
                    potDest[0] = -1;
                    potDest[1] = -1;
                }
            }
            targetDest[0] = potDest[0];
            targetDest[1] = potDest[1];
            
        }

        public void ensureCameraWithinBoundaries (int xAmt, int yAmt)
        {
            for (int i=0; i<2; i++)
            {
                if (Map.adjFact[i] <= 0) // onLeft
                {
                    Map.adjFact[i] = 0;
                    if (centerRelScreen()[i] >= Game1.dim[i] / 2)
                    {
                        Map.adjFact[i] += centerRelScreen()[i] - Game1.dim[i] / 2;
                    }
                }
                else if (Map.adjFact[i] >= Map.maxAdjFact[i]) //onRight
                {
                    Map.adjFact[i] = Map.maxAdjFact[i];
                    Console.WriteLine("Scrolling");
                    if (centerRelScreen()[i] <= Game1.dim[i] / 2)
                    {
                        
                        Map.adjFact[i] += centerRelScreen()[i] - Game1.dim[i] / 2;
                    }
                } else
                {
                    switch (i)
                    {
                        case 0:
                            Map.adjFact[i] += xAmt;
                            break;
                        case 1:
                            Map.adjFact[i] += yAmt;
                            break;
                    }
                }
            }
        }


        public void render ()
        {
            //sb.Draw(shipT[shipInd],new Rectangle(shipR.X -map.adjFact[0], shipR.Y -map.adjFact[1], shipR.Width, shipR.Height), new Rectangle(0,0,shipT[shipInd].Width, shipT[shipInd].Height), Color.White, shipHeadingRad, new Vector2(shipR.Width/2, shipR.Height/2), SpriteEffects.None, 0);
            sb.Draw(shipT[shipInd], new Vector2(shipR.X - Map.adjFact[0], shipR.Y - Map.adjFact[1]), new Rectangle(0, 0, shipT[shipInd].Width, shipT[shipInd].Height), Color.White, shipHeadingRad, new Vector2(shipR.Width / 2, shipR.Height / 2), Game1.viewingScale*shipScale, SpriteEffects.None, 0);
        }
    }
}
