
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;

namespace Main
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public static int[] dim = new int[2];
        Map map;
        Player play;
        public static float viewingScale = 1.0f;
        public static Rectangle viewingPort;

        int[] tPT; //target player traversal

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            dim[0] = 1200;
            dim[1] = 800;
            graphics.PreferredBackBufferWidth = dim[0];
            graphics.PreferredBackBufferHeight = dim[1];
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Drawable.setup(spriteBatch, this, dim[0], dim[1]);
            map = new Map(0, new Random().Next(5000));
            Drawable.setMap(map);
            
            tPT = new int[2];
            
            play = new Main.Player(0, 1);
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            MouseState ms = Mouse.GetState();
            if (ms.LeftButton == ButtonState.Pressed)
            {
                //process movement command by user (after checking buttons)
                if (!Player.charForm) //ship form
                {
                    if (ms.X >= 0 && ms.X <= dim[0] && ms.Y >= 0 && ms.Y <= dim[1])
                    {
                        play.setTarget(ms.X, ms.Y);
                        
                    }
                    
                }
            } else
            {
               if (!Player.charForm)
                {
                    play.shipNoClick();
                }
            }
            
            play.move_collisioncheck();
            //update viewing port in case player moved
            Game1.viewingPort = new Rectangle(map.adjFact[0], map.adjFact[1], (int)(Game1.dim[0] * 1.0 /Game1.viewingScale) + 300, (int)(Game1.dim[1] * 1.0 /Game1.viewingScale) + 300);
            //Console.WriteLine(play.shipSpeed);
            map.decideWhatToDraw();
            play.collisionCheckMapStuff();

            base.Update(gameTime);
            
            
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Honeydew);

            // TODO: Add your drawing code here
            spriteBatch.Begin();


            map.render();
            play.render();

            //for TESTING
            


            spriteBatch.End();

            base.Draw(gameTime);
            
        }
    }
}
