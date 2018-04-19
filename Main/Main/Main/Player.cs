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
        private Color[] shipTextureData;
        //testing variables
        private bool testing = false;
        private Texture2D tmT;
        private float stillMargin = .5f;
        private bool hitIsland = false;
        private int hitIslandTimer = 0;
        private Vector2 shipIColSpot = new Vector2(-1,-1);

        public Player (int shipType, int pNum)
        {

            charForm = false;
            this.shipType = shipType;
            shipScale = .2f; //########################################################should be .2
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
            tmT = game.Content.Load<Texture2D>("testing_mask");

            switch (shipType)
            {
                case 0:
                    shipInd = 0;
                    shipT = new Texture2D[1];
                    shipT[0] = game.Content.Load<Texture2D>("shiptestcropped");
                    shipR = new Rectangle(0, 0, (int)(shipT[0].Width * shipScale), (int)(shipT[0].Height * shipScale));
                    shipTextureData = new Color[shipT[shipInd].Width * shipT[shipInd].Height];
                    
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
            shipTextureData = new Color[shipT[shipInd].Width * shipT[shipInd].Height];
            shipT[shipInd].GetData(shipTextureData);

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

                //Console.WriteLine("acc");
                
            } else
            {
                //Console.WriteLine("hapnin");
                if (shipSpeed>=0)
                {
                    shipSpeed += shipDeccel;
                } else
                {
                    shipSpeed -= shipDeccel;
                }
                
                if (shipSpeed < 0+stillMargin && shipSpeed>0-stillMargin)
                {
                    shipSpeed = 0;
                    decellMode = false;
                    targetDest[0] = -1;
                    targetDest[1] = -1;

                }
            }
        }

 

        public void collisionCheckMapStuff ()
        {
            int[] shipMapCent = centerOnMap();
            //Matrix shipMat =  Matrix.CreateTranslation(-shipMapCent[0], -shipMapCent[1], 0) * Matrix.CreateScale(shipScale) * Matrix.CreateRotationZ(shipHeadingRad) * Matrix.CreateTranslation(shipR.X, shipR.Y, 0);
            //Matrix shipMat = Matrix.CreateScale(shipScale) * Matrix.CreateTranslation(-shipT[shipInd].Width, -shipT[shipInd].Height, 0) * Matrix.CreateRotationZ(shipHeadingRad) * Matrix.CreateTranslation(shipR.X, shipR.Y, 0);
            //Matrix shipMat2 = Matrix.CreateTranslation(-shipMapCent[0], -shipMapCent[1], 0) *  Matrix.CreateRotationZ(shipHeadingRad) * Matrix.CreateTranslation(shipR.X, shipR.Y, 0);
            Matrix shipMat = Matrix.CreateScale(shipScale) * Matrix.CreateTranslation(-shipT[shipInd].Width / 2 * shipScale, -shipT[shipInd].Height / 2 * shipScale, 0) * Matrix.CreateRotationZ(shipHeadingRad) * Matrix.CreateTranslation(shipR.X, shipR.Y, 0);
            Matrix shipMat2 = Matrix.CreateTranslation(-shipT[shipInd].Width, -shipT[shipInd].Height, 0) * Matrix.CreateRotationZ(shipHeadingRad) * Matrix.CreateTranslation(shipR.X, shipR.Y, 0);
            Rectangle shipBoundingRect = CalculateBoundingRectangle(new Rectangle(0, 0, shipT[shipInd].Width, shipT[shipInd].Height), shipMat);
            bool collision = false;
            for (int i=0; i<Map.islandsToDraw.Count; i++)
            {
                int extension = 20;
                Rectangle isloc = Map.islandsToDraw[i].isloc;
                if (shipBoundingRect.Intersects(new Rectangle(isloc.X, isloc.Y, isloc.Width, isloc.Height)))
                {
                    //time to do pixel perfect yeet
                    
                    Island isleToCheck = Map.islandsToDraw[i];
                    Matrix islandMat = Matrix.CreateScale(Map.islandScale) * Matrix.CreateTranslation(isleToCheck.isloc.X, isleToCheck.isloc.Y, 0);
                    collision = IntersectPixels(shipMat, shipT[shipInd].Width, shipT[shipInd].Height, shipTextureData, islandMat, isleToCheck.islandT.Width, isleToCheck.islandT.Height, isleToCheck.islandTextureData);
                    if (collision)
                    {
                        Console.WriteLine("COLLISION!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        collision = true;
                        break;
                    }
                    
                    //collision = true;
                   // break;
                } else
                {
                    collision = false;
                    //Console.WriteLine("nope");
                }
            }
            Console.WriteLine("Drew " + Map.islandsToDraw.Count);
            testing = collision;


        }

        public static Rectangle CalculateBoundingRectangle(Rectangle rectangle,
                                                           Matrix transform)
        {
            // Get all four corners in local space
            Vector2 leftTop = new Vector2(rectangle.Left, rectangle.Top);
            Vector2 rightTop = new Vector2(rectangle.Right, rectangle.Top);
            Vector2 leftBottom = new Vector2(rectangle.Left, rectangle.Bottom);
            Vector2 rightBottom = new Vector2(rectangle.Right, rectangle.Bottom);

            // Transform all four corners into work space
            Vector2.Transform(ref leftTop, ref transform, out leftTop);
            Vector2.Transform(ref rightTop, ref transform, out rightTop);
            Vector2.Transform(ref leftBottom, ref transform, out leftBottom);
            Vector2.Transform(ref rightBottom, ref transform, out rightBottom);

            // Find the minimum and maximum extents of the rectangle in world space
            Vector2 min = Vector2.Min(Vector2.Min(leftTop, rightTop),
                                      Vector2.Min(leftBottom, rightBottom));
            Vector2 max = Vector2.Max(Vector2.Max(leftTop, rightTop),
                                      Vector2.Max(leftBottom, rightBottom));

            // Return that as a rectangle
            return new Rectangle((int)min.X, (int)min.Y,
                                 (int)(max.X - min.X), (int)(max.Y - min.Y));
        }

        public float manageShipHeading()
        {
            float radTurned = 0;
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
                            radTurned = -maxShipTurnSpeed;
                        }
                        else
                        {
                            shipHeadingRad += shipTurnSpeed;
                            radTurned = maxShipTurnSpeed;
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
            return radTurned;
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

        public void move_collisioncheck ()
        {
            // Console.WriteLine("TD= " + targetDest[0] + " " + targetDest[1]);
            // Console.WriteLine("Decell = " + decellMode);
            // Console.WriteLine("Speed = " + shipSpeed);
            if (hitIsland)
            {
                hitIslandTimer++;
            }
            float xMoved = 0;
            float yMoved = 0;
            float radTurned = 0;
            if (hitIsland)
            {
                targetDest[0] = -1;
                targetDest[1] = -1;
            }
            if (targetDest[0] >= 0 || targetDest[1] >= 0)
            {
                 //Console.WriteLine("MOVING TO DEST");

                if (!charForm)
                {

                    radTurned = manageShipHeading();
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
                    xMoved = tempShipRX - shipX;
                    yMoved = tempShipRY - shipY;
                    shipR.X = (int)tempShipRX;
                    shipR.Y = (int)tempShipRY;
                    shipX = tempShipRX;
                    shipY = tempShipRY;

                    //map.adjFact[0] += (int)xAmt;
                    //map.adjFact[1] += (int)yAmt;
                    ensureCameraWithinBoundaries(xAmt, yAmt);
                    if (pCent[0] < targetDest[0] + moveErrorMargin && pCent[0] > targetDest[0] - moveErrorMargin && pCent[1] < targetDest[1] + moveErrorMargin && pCent[1] > targetDest[1] - moveErrorMargin)
                    {
                        if (!decellMode)
                        {
                            decellMode = true;
                            //set new target
                            int displacement = (int)((-1 * shipSpeed * shipSpeed) / (2 * shipDeccel));
                            //Console.WriteLine("DISPLACEMENT - " + displacement);
                            float n = displacement / shipSpeed;
                            int moreXAmt = (int)(n * xAmt);
                            int moreYAmt = (int)(n * yAmt);
                            targetDest[0] += moreXAmt;
                            targetDest[1] += moreYAmt;
                        }
                        else
                        {
                            if (!hitIsland)
                            {
                                decellMode = false;
                                targetDest[0] = -1;
                                targetDest[1] = -1;
                            }
                            
                        }

                        //set new targetdest
                        //Console.WriteLine("---We're there!");

                    }

                }

            }
            else
            {

                //no target dest, still must move with existing speeds
                if (!charForm)
                {

                    manageShipSpeed();
                    float[] amts = distroShipSpeed();
                    float xAmt = amts[0];
                    float yAmt = amts[1];
                    int[] pCent = centerOnMap();



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
                    xMoved = tempShipRX - shipX;
                    yMoved = tempShipRY - shipY;
                    shipR.X = (int)tempShipRX;
                    shipR.Y = (int)tempShipRY;
                    shipX = tempShipRX;
                    shipY = tempShipRY;

                    //map.adjFact[0] += (int)xAmt;
                    //map.adjFact[1] += (int)yAmt;
                    ensureCameraWithinBoundaries(xAmt, yAmt);
                }
            }

            int[] shipMapCent = centerOnMap();
            //Matrix shipMat =  Matrix.CreateTranslation(-shipMapCent[0], -shipMapCent[1], 0) * Matrix.CreateScale(shipScale) * Matrix.CreateRotationZ(shipHeadingRad) * Matrix.CreateTranslation(shipR.X, shipR.Y, 0);
            //Matrix shipMat = Matrix.CreateScale(shipScale) * Matrix.CreateTranslation(-shipT[shipInd].Width, -shipT[shipInd].Height, 0) * Matrix.CreateRotationZ(shipHeadingRad) * Matrix.CreateTranslation(shipR.X, shipR.Y, 0);
            //Matrix shipMat2 = Matrix.CreateTranslation(-shipMapCent[0], -shipMapCent[1], 0) *  Matrix.CreateRotationZ(shipHeadingRad) * Matrix.CreateTranslation(shipR.X, shipR.Y, 0);
            Matrix shipMat = Matrix.CreateScale(shipScale) * Matrix.CreateTranslation(-shipT[shipInd].Width / 2 * shipScale, -shipT[shipInd].Height / 2 * shipScale, 0) * Matrix.CreateRotationZ(shipHeadingRad) * Matrix.CreateTranslation(shipR.X, shipR.Y, 0);
            Matrix shipMat2 = Matrix.CreateTranslation(-shipT[shipInd].Width, -shipT[shipInd].Height, 0) * Matrix.CreateRotationZ(shipHeadingRad) * Matrix.CreateTranslation(shipR.X, shipR.Y, 0);
            Rectangle shipBoundingRect = CalculateBoundingRectangle(new Rectangle(0, 0, shipT[shipInd].Width, shipT[shipInd].Height), shipMat);
            bool collision = false;
            for (int i = 0; i < Map.islandsToDraw.Count; i++)
            {
                int extension = 20;
                Rectangle isloc = Map.islandsToDraw[i].isloc;
                if (shipBoundingRect.Intersects(new Rectangle(isloc.X, isloc.Y, isloc.Width, isloc.Height)))
                {
                    //time to do pixel perfect yeet

                    Island isleToCheck = Map.islandsToDraw[i];
                    Matrix islandMat = Matrix.CreateScale(Map.islandScale) * Matrix.CreateTranslation(isleToCheck.isloc.X, isleToCheck.isloc.Y, 0);
                    collision = IntersectPixels(shipMat, shipT[shipInd].Width, shipT[shipInd].Height, shipTextureData, islandMat, isleToCheck.islandT.Width, isleToCheck.islandT.Height, isleToCheck.islandTextureData);
                    if (collision)
                    {
                        Console.WriteLine("COLLISION!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        collision = true;
                        break;
                    }

                    //collision = true;
                    // break;
                }
                else
                {
                    collision = false;
                    //Console.WriteLine("nope");
                }
            }
           // Console.WriteLine("Drew " + Map.islandsToDraw.Count);
            testing = collision;
            if (collision) //with an island
            {
                hitIsland = true;
                hitIslandTimer = 0;
                shipX -= changeMagn(xMoved, 3);
                shipY -= changeMagn(yMoved, 3);
                shipR.X = (int)shipX;
                shipR.Y = (int)shipY;
                shipHeadingRad -= radTurned;
                //shipGoalHeading = shipHeadingRad - radTurned;
                ensureCameraWithinBoundaries(-changeMagn(xMoved, 3), -changeMagn(yMoved, 3));
                shipSpeed = -shipSpeed/3-2;
                targetDest[0] = -1;
                targetDest[1] = -1;
                decellMode = true;
                Button bob = new Button(0, (int)(shipIColSpot.X), (int)(shipIColSpot.Y)); //create docking button
            } else
            {
                if (hitIslandTimer > 15)
                {
                    hitIsland = false;
                }
            }

            //Console.WriteLine(shipSpeed + " speeds");
        }

        public float changeMagn (float num, float inc)
        {

            float magn = Math.Abs(num);
            if (inc<0 && inc>magn)
            {
                throw new System.ArgumentException("Can't decrease magn to less than zero", "original");
            }
            magn += inc;
            if (num<0)
            {
                magn = -magn;
            }
            return magn;
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
            if (!hitIsland)
            {
                potDest[0] = mX + map.adjFact[0];

                potDest[1] = mY + map.adjFact[1];
                int[] pCent = centerOnMap();
                decellMode = false;
                if (shipSpeed < 1)
                {
                    shipSpeed += 3;
                }


                if (!charForm)
                {
                    if (potDest[0] > map.mapWidth - shipR.Width / 2)
                    {
                        potDest[0] = map.mapWidth - shipR.Width / 2;
                    }

                    if (potDest[1] > map.mapHeight - shipR.Height / 2)
                    {
                        potDest[1] = map.mapHeight - shipR.Height / 2;
                    }

                    if (potDest[0] < shipR.Width / 2)
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
                    Rectangle tempShipCenter = new Rectangle(centerRelScreen()[0] - fromShipCenterVariation[0], centerRelScreen()[1] - fromShipCenterVariation[1], fromShipCenterVariation[0] * 2, fromShipCenterVariation[1] * 2);
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

        }

        public void ensureCameraWithinBoundaries (float xAmt, float yAmt)
        {
            int shipCenterXAdjustment = 130;
            for (int i=0; i<2; i++)
            {
                if (map.preciseAdjFact[i] <= 0) // onLeft
                {
                    map.preciseAdjFact[i] = 0;

                    if (centerRelScreen()[i] >= Game1.dim[i] / 2 + (i==0? shipCenterXAdjustment : 0))
                    {
                        map.preciseAdjFact[i] += centerRelScreen()[i] - Game1.dim[i] / 2 - (i == 0 ? shipCenterXAdjustment : 0);
                    }
                }
                else if (map.preciseAdjFact[i] >= map.maxAdjFact[i]) //onRight
                {
                    map.preciseAdjFact[i] = map.maxAdjFact[i];
                    //Console.WriteLine("Scrolling");
                    if (centerRelScreen()[i] <= Game1.dim[i] / 2 + (i == 0 ? shipCenterXAdjustment : 0))
                    {
                        
                        map.preciseAdjFact[i] += centerRelScreen()[i] - Game1.dim[i] / 2 - (i == 0 ? shipCenterXAdjustment : 0);
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
            //sb.Draw(shipT[shipInd],new Rectangle(shipR.X -map.adjFact[0], shipR.Y -map.adjFact[1], shipR.Width, shipR.Height), new Rectangle(0,0,shipT[shipInd].Width, shipT[shipInd].Height), Color.White, (float)(Math.PI*2 - shipHeadingRad), new Vector2(shipT[shipInd].Width/2, shipT[shipInd].Height/2), SpriteEffects.None, 0);
            sb.Draw(shipT[shipInd], new Vector2((shipR.X - map.adjFact[0]), shipR.Y - map.adjFact[1]), new Rectangle(0,0,shipT[shipInd].Width, shipT[shipInd].Height), testing?Color.Red:Color.White, (float)(Math.PI*2 - shipHeadingRad), new Vector2(shipT[shipInd].Width/2, shipT[shipInd].Height/2), Game1.viewingScale*shipScale, SpriteEffects.None, 0);
            int[] shipMapCent = centerOnMap();
            // Matrix shipMat = Matrix.CreateTranslation(-shipMapCent[0], -shipMapCent[1], 0) * Matrix.CreateScale(shipScale) * Matrix.CreateRotationZ(shipHeadingRad) * Matrix.CreateTranslation(shipR.X, shipR.Y, 0);
            Matrix shipMat = Matrix.CreateScale(shipScale) * Matrix.CreateTranslation(-shipT[shipInd].Width/2*shipScale, -shipT[shipInd].Height/2*shipScale, 0) *  Matrix.CreateRotationZ(shipHeadingRad) * Matrix.CreateTranslation(shipR.X, shipR.Y, 0);
            Rectangle toMod = CalculateBoundingRectangle(new Rectangle(0, 0, shipT[shipInd].Width, shipT[shipInd].Height), shipMat);
            toMod.X -= map.adjFact[0];
            toMod.Y -= map.adjFact[1];
            //center on origin
           // sb.Draw(tmT, toMod, Color.White);       //the testing gray bounding rect!!!
            //Console.WriteLine(shipR.X + " " + shipR.Y + " for ship");
            // Console.WriteLine()

            /*
            for (int i = 0; i < Map.islandsToDraw.Count; i++)
            {
                int extension = 20;
                Rectangle isloc = Map.islandsToDraw[i].isloc;
                sb.Draw(tmT, new Rectangle(isloc.X - map.adjFact[0], isloc.Y - map.adjFact[1], isloc.Width, isloc.Height), Color.White);
            }
            */
                
        }

        /// <summary>
        /// Determines if there is overlap of the non-transparent pixels between two
        /// sprites.
        /// </summary>
        /// <param name="transformA">World transform of the first sprite.</param>
        /// <param name="widthA">Width of the first sprite's texture.</param>
        /// <param name="heightA">Height of the first sprite's texture.<;/param>
        /// <param name="dataA">Pixel color data of the first sprite.</param>
        /// <param name="transformB">World transform of the second sprite.</param>
        /// <param name="widthB">Width of the second sprite's texture.</param>
        /// <param name="heightB">Height of the second sprite's texture.</param>
        /// <param name="dataB">Pixel color data of the second sprite.</param>
        /// <returns>True if non-transparent pixels overlap; false otherwise</returns>
        bool IntersectPixels(
            Matrix transformA, int widthA, int heightA, Color[] dataA,
            Matrix transformB, int widthB, int heightB, Color[] dataB)
        {
            // Calculate a matrix which transforms from A's local space into
            // world space and then into B's local space
            Matrix transformAToB = transformA * Matrix.Invert(transformB);

            // For each row of pixels in A
            for (int yA = 0; yA < heightA; yA+=3)
            {
                // For each pixel in this row
                for (int xA = 0; xA < widthA; xA+=4)
                {
                    // Calculate this pixel's location in B
                    Vector2 positionInB =
                        Vector2.Transform(new Vector2(xA, yA), transformAToB);

                    // Round to the nearest pixel
                    int xB = (int)Math.Round(positionInB.X);
                    int yB = (int)Math.Round(positionInB.Y);

                    // If the pixel lies within the bounds of B
                    if (0 <= xB && xB < widthB &&
                        0 <= yB && yB < heightB)
                    {
                        // Get the colors of the overlapping pixels
                        Color colorA = dataA[xA + yA * widthA];
                        Color colorB = dataB[xB + yB * widthB];

                        // If both pixels are not completely transparent,
                        if (colorA.A != 0 && colorB.A != 0)
                        {
                            // then an intersection has been found
                            Vector2 collSpot = Vector2.Transform(new Vector2(xA, yA), transformA);
                            shipIColSpot = collSpot;
                            return true;
                        }
                    }
                }
            }

            // No intersection found
            shipIColSpot = new Vector2(-1, -1);
            return false;
        }
    }
}
