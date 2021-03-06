﻿using System;
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
        public float shipSpeedX = 0;
        public float shipSpeedY = 0;
        public float shipSpeed = 0;
        private float maxShipSpeed = 8;
        private float shipDeccel = -.06f;
        private float shipAcc = .08f;
        private int[] targetDest = new int[2];
        private static int moveErrorMargin = 12;
        private int[] potDest = new int[2];
        private int[] fromShipCenterVariation = new int[2];
        private bool decellMode = false;
        private float shipHeadingRad = 0;
        private int shipInd = 0;
        private float shipTurnSpeed = 0;
        private float maxShipTurnSpeed = .05f;
        private float shipTurnErrorMargin = .1f;
        private float shipX = 0;
        private float shipY = 0;
        private float shipGoalHeading = 0;

        public Player (int shipType, int pNum)
        {

            charForm = false;
            this.shipType = shipType;
            shipScale = .20f;
            charScale = .1f;
            targetDest[0] = -1;
            targetDest[1] = -1;
            fromShipCenterVariation[0] = 7;
            fromShipCenterVariation[1] = 5;
            shipSpeed = 0;
            shipSpeedX = 0;
            shipSpeedY = 0;
            decellMode = false;
            shipHeadingRad = 0;
            shipTurnSpeed = .05f;
            
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
                    shipX = shipR.X;
                    shipY = shipR.Y;
                    map.adjFact[0] = 0;
                    map.adjFact[1] = 0;
                    map.preciseAdjFact[0] = 0;
                    map.preciseAdjFact[1] = 0;
                    shipHeadingRad = 0;
                    shipGoalHeading = -1;
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
            int[] cent = { shipR.X + shipR.Width / 2 - map.adjFact[0], shipR.Y + shipR.Height / 2 - map.adjFact[1] };
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
                //Console.WriteLine("hapnin");

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

        public void manageShipHeading()
        {
            if (shipSpeed != 0)
            {
                //Console.WriteLine("ship goal heading " + shipGoalHeading);
                
                if (shipGoalHeading != -1)
                {

                    

                    float cw = Math.Abs(shipGoalHeading - shipHeadingRad);
                    float ccw = (float)(Math.Abs(Math.PI * 2 - cw));
                    if (shipHeadingRad >= shipGoalHeading)
                    {
                        cw = shipHeadingRad - shipGoalHeading;
                        ccw = (float)(Math.PI * 2 - cw);
                    }
                    else
                    {
                        ccw = shipGoalHeading - shipHeadingRad;
                        cw = (float)(Math.PI * 2 - cw);
                    }

                    float min = ccw <= cw ? ccw : cw;
                    bool clockwise = ccw <= cw ? false : true;
                    //Console.WriteLine("begin rot ---");
                    // Console.WriteLine("clockwise is " + clockwise);
                    //Console.WriteLine("SH " + shipHeadingRad);
                    // Console.WriteLine("GH " + goalHeading);
                    //  Console.WriteLine("Rad to Overcome: " + min);
                    if (min > shipTurnErrorMargin)
                    {

                        if (clockwise)
                        {
                            shipHeadingRad -= shipTurnSpeed;
                        }
                        else
                        {
                            shipHeadingRad += shipTurnSpeed;
                        }
                    }
                    else
                    {
                        //  Console.WriteLine("*Heading spot on");
                        shipGoalHeading = -1;
                    }
                }
                else
                {
                    // Console.WriteLine("ship speed is zero");
                }
                //testing shipHeadingRad = (float)(Math.PI / 3);
            }
        }

        public float[] distroShipSpeed ()
        {
            normalizeShipHeading();
            float[] amts = new float[2];
            amts[0] = (float)(shipSpeed * Math.Cos(shipHeadingRad));
            amts[1] = (float)(shipSpeed * Math.Sin(shipHeadingRad));
            if (shipHeadingRad ==0)
            {
                amts[0] = shipSpeed;
                amts[1] = 0;
            } else if (shipHeadingRad<=Math.PI/2)
            {
                if (shipHeadingRad == Math.PI/2)
                {
                    amts[1] = shipSpeed;
                    amts[0] = 0;
                } else
                {
                    amts[0] = (float)(shipSpeed * Math.Cos(shipHeadingRad));
                    amts[1] = -(float)(shipSpeed * Math.Sin(shipHeadingRad));
                }
            } else if (shipHeadingRad<=Math.PI)
            {
                if (shipHeadingRad == Math.PI)
                {
                    amts[0] = -shipSpeed;
                    amts[1] = 0;
                } else
                {
                    float lilAngle = (float)(Math.PI - shipHeadingRad);
                    amts[0] = -(float)(shipSpeed * Math.Cos(lilAngle));
                    amts[1] = -(float)(shipSpeed * Math.Sin(lilAngle));
                }
            } else if (shipHeadingRad<=Math.PI*3/2)
            {
                if (shipHeadingRad == Math.PI*3/2)
                {
                    amts[1] = -shipSpeed;
                    amts[0] = 0;
                } else
                {
                    float lilAngle = (float)(shipHeadingRad - Math.PI);
                    amts[0] = -(float)(shipSpeed * Math.Cos(lilAngle));
                    amts[1] = (float)(shipSpeed * Math.Sin(lilAngle));
                }
            } else if (shipHeadingRad < Math.PI*2)
            {
                float lilAngle = (float)(Math.PI*2 - shipHeadingRad);
                amts[0] = (float)(shipSpeed * Math.Cos(lilAngle));
                amts[1] = (float)(shipSpeed * Math.Sin(lilAngle));
            }
            return amts;
        }

        public void move ()
        {
           // Console.WriteLine("TD= " + targetDest[0] + " " + targetDest[1]);
           // Console.WriteLine("Decell = " + decellMode);
           // Console.WriteLine("Speed = " + shipSpeed);

            if (targetDest[0] >=0 || targetDest[1] >=0)
            {
               // Console.WriteLine("MOVING");

                if (!charForm)
                {
                    
                    manageShipHeading();
                    manageShipSpeed();
                    float[] amts = distroShipSpeed();
                    float xAmt = amts[0];
                    float yAmt = amts[1];
                    int[] pCent = centerOnMap();
                    /* -------------------------------------
                    

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
                    */ //------------------------------------

                    //
                    float tempShipRX = shipX;
                    float tempShipRY = shipY;
                    tempShipRX += xAmt;
                    tempShipRY += yAmt;
                    
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
                    shipR.X = (int)tempShipRX;
                    shipR.Y = (int)tempShipRY;
                    shipX = tempShipRX;
                    shipY = tempShipRY;

                    //map.adjFact[0] += (int)xAmt;
                    //map.adjFact[1] += (int)yAmt;
                    ensureCameraWithinBoundaries(xAmt, yAmt);
                    if (pCent[0] < targetDest[0]+moveErrorMargin && pCent[0]> targetDest[0] - moveErrorMargin && pCent[1] < targetDest[1] + moveErrorMargin && pCent[1] > targetDest[1] - moveErrorMargin)
                    {
                        if (!decellMode)
                        {
                            decellMode = true;
                            //set new target
                            int displacement = (int)((-1 * shipSpeed * shipSpeed) / (2 * shipDeccel));
                            //Console.WriteLine("DISPLACEMENT - " + displacement);
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
                        //Console.WriteLine("---We're there!");
                        
                    }

                }

            } else
            {
                
                //Console.WriteLine("Not moving");
            }
        }

        public void normalizeShipHeading ()
        {
            
            shipHeadingRad = (float)(shipHeadingRad % (Math.PI * 2));
            if (shipHeadingRad <0)
            {
                shipHeadingRad = (float)(shipHeadingRad + Math.PI * 2);
            }
        }
     

        public void shipNoClick ()
        {
            decellMode = true;
        }



        public void setTarget(int mX, int mY)
        {
            potDest[0] = mX + map.adjFact[0];

            potDest[1] = mY + map.adjFact[1];
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

            normalizeShipHeading();
            
            //pCent[0] += shipR.Width / 2; 
            float goalHeading = 0;
            /*
            if (targetDest[0] > pCent[0])
            {
                goalHeading =(float)( Math.Atan((targetDest[1] - pCent[1]) * 1.0 / (targetDest[0]-pCent[0])));
            } else
            {
                goalHeading = (float)((Math.Atan((targetDest[1] - pCent[1]) * 1.0 / (targetDest[0] - pCent[0]))));
            }
            */
            goalHeading = (float)(Math.Atan(Math.Abs((targetDest[1] - pCent[1])) * 1.0 / Math.Abs(targetDest[0] - pCent[0])));

            if (goalHeading > Math.PI / 2 || goalHeading < 0)
            {
                Console.WriteLine("PROBLEM GOAL: " + goalHeading);
            }
            if (targetDest[0] > pCent[0])
            {
                if (targetDest[1] < pCent[1])
                {
                    //do nothing
                    // Console.WriteLine("Case 1");
                }
                else
                {
                    goalHeading = (float)(Math.PI * 2 - goalHeading);
                    // Console.WriteLine("Case 2");
                }
            }
            else
            {
                if (targetDest[1] < pCent[1])
                {
                    goalHeading = (float)(Math.PI - goalHeading);
                    //Console.WriteLine("Case 3");
                }
                else
                {
                    goalHeading = (float)(Math.PI + goalHeading);
                    //Console.WriteLine("Case 4");
                }
            }
            shipGoalHeading = goalHeading;

        }

        public void ensureCameraWithinBoundaries (float xAmt, float yAmt)
        {
            for (int i=0; i<2; i++)
            {
                if (map.preciseAdjFact[i] <= 0) // onLeft
                {
                    map.preciseAdjFact[i] = 0;

                    if (centerRelScreen()[i] >= Game1.dim[i] / 2)
                    {
                        map.preciseAdjFact[i] += centerRelScreen()[i] - Game1.dim[i] / 2;
                    }
                }
                else if (map.preciseAdjFact[i] >= map.maxAdjFact[i]) //onRight
                {
                    map.preciseAdjFact[i] = map.maxAdjFact[i];
                    //Console.WriteLine("Scrolling");
                    if (centerRelScreen()[i] <= Game1.dim[i] / 2)
                    {
                        
                        map.preciseAdjFact[i] += centerRelScreen()[i] - Game1.dim[i] / 2;
                    }
                } else
                {
                    switch (i)
                    {
                        case 0:
                            map.preciseAdjFact[i] += xAmt;
                            break;
                        case 1:
                            map.preciseAdjFact[i] += yAmt;
                            break;
                    }
                    
                }
            }
            map.adjFact[0] = (int)map.preciseAdjFact[0];
            map.adjFact[1] = (int)map.preciseAdjFact[1];
        }


        public void render ()
        {
            sb.Draw(shipT[shipInd],new Rectangle(shipR.X -map.adjFact[0], shipR.Y -map.adjFact[1], shipR.Width, shipR.Height), new Rectangle(0,0,shipT[shipInd].Width, shipT[shipInd].Height), Color.White, (float)(Math.PI*2 - shipHeadingRad), new Vector2(shipT[shipInd].Width/2, shipT[shipInd].Height/2), SpriteEffects.None, 0);
        }
    }
}
