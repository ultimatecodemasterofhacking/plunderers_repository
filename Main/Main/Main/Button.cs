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
        private static List<Button> livingButts;
        private float buttScale;
        private int status;
        private static bool mouseDown = false;
        private static int msX;
        private static int msY;

        //x and y are map relative
        public Button (int ty, int x, int y)
        {
            buttType = ty;

            myTexts = new Texture2D[3];
            for (int i=0; i<myTexts.Length; i++)
            {
                myTexts[i] = allTexts[buttType,i];
            }
            switch(buttType)
            {
                case 0: //for docking at an island
                    buttScale = .7f;
                    break;
            }

            buttRect = new Rectangle(x, y, (int)(myTexts[0].Width * buttScale), (int)(myTexts[0].Height * buttScale));

            status = 0; //0 for regular, 1 for highlighted, 2 for pressed

            livingButts.Add(this);
        }

        public void update (bool freshPress, bool freshRelease)
        {
            if (inDatRect(msX, msY, buttRect)) //could affect this particular button
            {
                switch (status)
                {
                    case 0: //normal
                        if (!mouseDown)
                        {
                            status = 1;
                        } else
                        {
                            if (freshPress)
                            {
                                status = 2;
                            }
                        }
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
                            //time to trigger this buttons event!
                            Console.WriteLine("Button Released! Action to happen");
                        }
                        break;
                }
            } else
            {
                status = 0;
            }
            //now deal with timers and lifespan for terminal buttons
            
        }

        public static bool inDatRect (int x, int y, Rectangle rect)
        {
            return rect.Intersects(new Rectangle(x, y, 1, 1));
        }

        public static void mouseInteract (MouseState ms)
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
            for (int i=0; i<livingButts.Count; i++)
            {
                livingButts[i].update(freshPress, freshRelease);
            }
        }

        public void render()
        {
            sb.Draw(myTexts[status], new Vector2(buttRect.X - map.adjFact[0], buttRect.Y - map.adjFact[1]), new Rectangle(0, 0, myTexts[status].Width, myTexts[status].Height), Color.White, 0, new Vector2(myTexts[status].Width / 2, myTexts[status].Height / 2), Game1.viewingScale * buttScale, SpriteEffects.None, 0);
        }



        public static void loadButtTexts ()
        {
            livingButts = new List<Button>();
            allTexts = new Texture2D[1,3];
            //0,1,2 cols for regular, highlighted, and pressed respectively
            allTexts[0,0] = game.Content.Load<Texture2D>("DockButtonUnpressed");
            allTexts[0, 1] = game.Content.Load<Texture2D>("DockButtonHighlighted");
            allTexts[0, 2] = game.Content.Load<Texture2D>("DockButtonPressed");
        }



    }
}
