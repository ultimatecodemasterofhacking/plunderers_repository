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
    class Button : Drawable
    {
        public Rectangle buttRect;
        private int buttType;
        private static Texture2D[,] allTexts;
        private Texture2D[] myTexts;
        public static List<Button> livingButts;
        public static List<Button> dyingButts;
        private float buttScale;
        private int status;
        private static bool mouseDown = false;
        private static int msX;
        private static int msY;
        private bool timed = false;
        private int timer = 0;
        private int timerSwitch = 0;
        private int alpha = 255;
        private static int deadButtSwitch = 25;
        private static int maxDockButtNum = 3;
        private static int dockButtNum = 0;

        //x and y are map relative
        public Button (int ty, int x, int y)
        {
            
            buttType = ty;
            if (buttType == 0 && dockButtNum < maxDockButtNum)
            {
                myTexts = new Texture2D[3];
                for (int i = 0; i < myTexts.Length; i++)
                {
                    myTexts[i] = allTexts[buttType, i];
                }
                switch (buttType)
                {
                    case 0: //for docking at an island
                        buttScale = .8f;
                        timed = true;
                        timer = 0;
                        timerSwitch = 160;
                        dockButtNum++;
                        break;
                }
                alpha = 255;

                buttRect = new Rectangle(x, y, (int)(myTexts[0].Width * buttScale), (int)(myTexts[0].Height * buttScale));

                status = 0; //0 for regular, 1 for highlighted, 2 for pressed

                livingButts.Add(this);
            }
            
        }

        public int update (bool freshPress, bool freshRelease) //0 is nothing, 1 is action, -1 is intersection
        {
            bool buttonClicked = false;
            bool intersection = false;
           // Console.WriteLine("doing the butt method");
            if (inDatRect(msX, msY, buttRect)) //could affect this particular button
            {
                // Console.WriteLine("mouse in a butt");
                intersection = true;
                /*
                 switch (status)
                 {
                     case 0: //normal
                         status = 1;
                         break;
                     case 1: //highlighted
                         if (freshPress)
                         {
                             status = 2;
                         }
                         break;
                     case 2: //pressed
                         if (freshRelease)
                         {
                             //time to trigger this button's event!
                             buttonClicked = true;
                             Console.WriteLine("Button Released! Action to happen");
                         }
                         break;
                 }
                 */
                /*
                if (!mouseDown)
                {
                    status = 1;
                } else
                {
                    if (freshPress)
                    {
                        status = 2;
                    }
                    if (status ==1 || status ==0)
                    {
                        if (freshRelease)
                        {
                            //time to trigger this button's event!
                            buttonClicked = true;
                            Console.WriteLine("Button Released! Action to happen");
                        }
                    }
                }
                */
                
                if (status==0)
                {
                    status = 1;
                }
                if (status == 1 && freshPress)
                {
                    status = 2;
                } else if (status==2 && mouseDown)
                {
                    status = 2;
                }
                if (status == 2 && freshRelease)
                {
                    //time to trigger this button's event!
                    buttonClicked = true;
                    Console.WriteLine("Button Released! Action to happen");
                }
                
                
            } else
            {
               // Console.WriteLine("not in butt");
                status = 0;
            }
            //now deal with timers and lifespan for terminal buttons
            if (timed)
            {
                timer++;
                if(timer>timerSwitch)
                {
                    //kill the button
                    livingButts.Remove(this);
                    dyingButts.Add(this);
                    timer = deadButtSwitch;
                }
            }
            if (buttonClicked)
            {
                //kill the button
                livingButts.Remove(this);
                dyingButts.Add(this);
                timer = deadButtSwitch;
            }
            if (buttonClicked)
            {
                return 1;
            } else
            {
                if (intersection && mouseDown)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }


            
            
        }

        public static void renderAll ()
        {
            for (int i=0; i<livingButts.Count; i++)
            {
                livingButts[i].render();
            }

            for (int i = 0; i < dyingButts.Count; i++)
            {
                dyingButts[i].render();
            }
        }

        public static void fadeDyingButts ()
        {
            for (int i=dyingButts.Count-1; i>=0; i--)
            {
                dyingButts[i].timer--;
                dyingButts[i].alpha = (int)((dyingButts[i].timer*1.0/deadButtSwitch) * 255);
               // Console.WriteLine("a dying alpha "  + dyingButts[i].alpha);
                
                if (dyingButts[i].timer<0)
                {
                    if (dyingButts[i].buttType==0)
                    {
                        dockButtNum--;
                    }
                    dyingButts.Remove(dyingButts[i]); //final death
                    
                }
            }
        }
        

        public static bool inDatRect (int x, int y, Rectangle rect)
        {
            return new Rectangle(rect.X-rect.Width/2 - map.adjFact[0], rect.Y-rect.Height - map.adjFact[1], rect.Width, rect.Height).Intersects(new Rectangle(x , y , 2, 2));
        }

        public static int mouseInteract (MouseState ms) //0 is nothing, 1 is action, -1 is mouse intersection
        {
            bool freshPress = false;
            bool freshRelease = false;

            if (ms.LeftButton == ButtonState.Pressed)
            {
                if (mouseDown == false)
                {
                    freshPress = true;
                }
                mouseDown = true;
            } else
            {
                if (mouseDown==true)
                {
                    freshRelease = true;
                }
                mouseDown = false;
                
            }
            msX = ms.X;
            msY = ms.Y;
            bool intersection = false;
            bool actionOccurred = false;
            for (int i=livingButts.Count-1; i>=0; i--)
            {
                int tempOccur = livingButts[i].update(freshPress, freshRelease);
                if (tempOccur == 1)
                {
                    actionOccurred = true;
                    break;
                }
                if (tempOccur == -1)
                {
                    intersection = true;
                }
            }
            if (actionOccurred == true)
            {
                return 1;
            } else
            {
                if (intersection)
                {
                    return -1;
                } else
                {
                    return 0;
                }
            }
        }

        public void render()
        {
            sb.Draw(myTexts[status], new Vector2((int)((buttRect.X - map.adjFact[0])*1.0f/Game1.viewingScale), (int)((buttRect.Y - map.adjFact[1])*1.0f/Game1.viewingScale)), new Rectangle(0, 0, myTexts[status].Width, myTexts[status].Height), new Color(255,255,255,alpha), 0, new Vector2(myTexts[status].Width / 2, myTexts[status].Height / 2), 1.0f/Game1.viewingScale * buttScale, SpriteEffects.None, 0);
            
        }
        


        public static void loadButtTexts ()
        {
            livingButts = new List<Button>();
            dyingButts = new List<Button>();
            allTexts = new Texture2D[1,3];
            //0,1,2 cols for regular, highlighted, and pressed respectively
            allTexts[0,0] = game.Content.Load<Texture2D>("DockButtonUnpressed");
            allTexts[0, 1] = game.Content.Load<Texture2D>("DockButtonHighlighted");
            allTexts[0, 2] = game.Content.Load<Texture2D>("DockButtonPressed");
        }



    }
}
