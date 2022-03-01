using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;



namespace Submarine
{
    public static class StaticVariables
    {
        public static SoundEffect effect;
        public static Song song;
        // Selcted Menu
        // 1 = Play Game, 2 = Help, 3= About;
        public static int selectedMenu = 1;
        public static bool keyReleased = true;
        // Game States
        // 0 = Main Menu, 1 = In Game, 2 = Help, 3 = About, 4= Game Over
        public static int gameState = 0;
        public static float bestTime = 0f;

        
        
    }

    class Obstacle
    {
        public Vector2 position;
        public int speed;
        public int radius = 59;
        public bool offscreen = false;

        static Random rand = new Random();

        public Obstacle(int newSpeed)
        {
            speed = newSpeed;

            position = new Vector2(1280 + radius, rand.Next(0, 721));
        }

        public void obstacleUpdate(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            position.X -= speed * dt;
        }
    }
    class Sub
    {
        public int speed = 180;
        static public Vector2 defaultPosition = new Vector2(640, 360);
        public Vector2 position = defaultPosition;

        public void subUpdate(GameTime gameTime, Controller gameController)
        {
            KeyboardState kState = Keyboard.GetState();
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (gameController.inGame)
            {
                if (kState.IsKeyDown(Keys.Right) && position.X < 1280)
                {
                    position.X += speed * dt;
                }

                if (kState.IsKeyDown(Keys.Left) && position.X > 0)
                {
                    position.X -= speed * dt;
                }

                if (kState.IsKeyDown(Keys.Down) && position.Y < 720)
                {
                    position.Y += speed * dt;
                }

                if (kState.IsKeyDown(Keys.Up) && position.Y > 0)
                {
                    position.Y -= speed * dt;
                }
            }
        }
    }
    class Controller
    {
        
        public List<Obstacle> obstacles = new List<Obstacle>();
        public double timer = 2D;
        public double maxTime = 2D;
        public int nextSpeed = 240;
        public float totalTime = 0f;

        public bool inGame = false;

        public void conUpdate(GameTime gameTime)
        {
            if (inGame)
            {
                timer -= gameTime.ElapsedGameTime.TotalSeconds;
                totalTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if(totalTime >= StaticVariables.bestTime)
                {
                    StaticVariables.bestTime = totalTime;


                }
            }
            else
            {
                KeyboardState kState = Keyboard.GetState();
                if (kState.IsKeyDown(Keys.Enter) && StaticVariables.selectedMenu==1)
                {
                    inGame = true;
                    StaticVariables.gameState = 1;
                    totalTime = 0f;
                    timer = 2D;
                    maxTime = 2D;
                    nextSpeed = 240;
                    MediaPlayer.Play(StaticVariables.song);
                }
                if (kState.IsKeyDown(Keys.Enter) && StaticVariables.selectedMenu == 2)
                {
                    inGame = false;
                    StaticVariables.gameState = 2;
                    
                }

            }

            if (timer <= 0)
            {
                obstacles.Add(new Obstacle(nextSpeed));
                timer = maxTime;
                if (maxTime > 0.5)
                {
                    maxTime -= 0.1D;
                }
                if (nextSpeed < 720)
                {
                    nextSpeed += 4;
                }
            }
        }
    }
    public class Submarine : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        bool playOnce = true;


        Texture2D sub_Sprite;
        Texture2D fish_Sprite;
        Texture2D bomb_Sprite;
        Texture2D mine_Sprite;
        Texture2D shark_Sprite;
        Texture2D octopus_Sprite;
        Texture2D water_Sprite;

        SpriteFont gameFont;
        SpriteFont timerFont;
        SpriteFont helpFont;

        Sub player = new Sub();

        Controller gameController = new Controller();

        public Submarine()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            sub_Sprite = Content.Load<Texture2D>("sub");
            fish_Sprite = Content.Load<Texture2D>("fish");
            bomb_Sprite = Content.Load<Texture2D>("bomb");
            mine_Sprite = Content.Load<Texture2D>("mine");
            shark_Sprite = Content.Load<Texture2D>("shark");
            octopus_Sprite = Content.Load<Texture2D>("octopus");


            water_Sprite = Content.Load<Texture2D>("water");

            gameFont = Content.Load<SpriteFont>("mainFont");
            timerFont = Content.Load<SpriteFont>("timerFont");
            helpFont = Content.Load<SpriteFont>("helpFont");

            StaticVariables.effect = Content.Load<SoundEffect>("blast");
            StaticVariables.song = Content.Load<Song>("mario");

            // Reading high scores from file scores.txt
            /*string level0Data = null;
            using (var stream = TitleContainer.OpenStream("scores.txt"))
            {
                using (var reader = new System.IO.StreamReader(stream))
                {
                    // Call StreamReader methods like ReadLine, ReadBlock, or ReadToEnd to read in your data, e.g.:  
                    level0Data = reader.ReadToEnd();
                    StaticVariables.bestTime = float.Parse(level0Data);
                }
            }*/

        }
        

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {

            gameController.conUpdate(gameTime);

            KeyboardState kState = Keyboard.GetState();


            if (StaticVariables.gameState == 0)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();

                if (kState.IsKeyDown(Keys.Down) && StaticVariables.keyReleased == true)
                {
                    if (StaticVariables.selectedMenu == 1)
                    {
                        StaticVariables.selectedMenu = 2;
                        

                    }
                    else if (StaticVariables.selectedMenu == 2)
                    {
                        StaticVariables.selectedMenu = 3;
                        

                    }
                    StaticVariables.keyReleased = false;

                }
                if ((kState.IsKeyUp(Keys.Down)) && StaticVariables.keyReleased == false)
                {
                    StaticVariables.keyReleased = true;

                }
                if (kState.IsKeyDown(Keys.Up) && StaticVariables.keyReleased == true)
                {
                    if (StaticVariables.selectedMenu == 3)
                    {
                        StaticVariables.selectedMenu = 2;
                        StaticVariables.keyReleased = false;
                    }
                    else if (StaticVariables.selectedMenu == 2)
                    {
                        StaticVariables.selectedMenu = 1;
                        StaticVariables.keyReleased = false;
                    }

                }

                if (kState.IsKeyDown(Keys.Enter) && StaticVariables.selectedMenu == 1)
                {
                    StaticVariables.gameState = 1;
                }


                if (kState.IsKeyDown(Keys.Enter) && StaticVariables.selectedMenu == 2)
                {
                    StaticVariables.gameState = 2;
                }

                if (kState.IsKeyDown(Keys.Enter) && StaticVariables.selectedMenu == 3)
                {
                    StaticVariables.gameState = 3;
                    
                }


            } else if (StaticVariables.gameState == 1)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();

               
                player.subUpdate(gameTime, gameController);
                gameController.conUpdate(gameTime);

                for (int i = 0; i < gameController.obstacles.Count; i++)
                {
                    gameController.obstacles[i].obstacleUpdate(gameTime);

                    if (gameController.obstacles[i].position.X < (0 - gameController.obstacles[i].radius))
                    {
                        gameController.obstacles[i].offscreen = true;
                    }

                    int sum = gameController.obstacles[i].radius + 50;
                    if (Vector2.Distance(gameController.obstacles[i].position, player.position) < sum)
                    {
                        gameController.inGame = false;
                        StaticVariables.gameState = 4;
                        player.position = Sub.defaultPosition;
                        i = gameController.obstacles.Count;
                        gameController.obstacles.Clear();
                        StaticVariables.effect.Play();
                        MediaPlayer.Stop();
                        playOnce = true;

                        if (gameController.totalTime >= StaticVariables.bestTime)
                        {
                            StaticVariables.bestTime = gameController.totalTime;



                            // TODO Write high scores from file scores.txt

                            /*int time = (int) StaticVariables.bestTime;
                            string data = time.ToString();
                            using (var stream = TitleContainer.OpenWrite("scores.txt"))
                            {
                                using (var writer = new System.IO.StreamWriter(stream))
                                {
                                    writer.Write(data);
                                   
                                }
                            }*/


                        }

                    }
                    if (kState.IsKeyDown(Keys.Back) && StaticVariables.selectedMenu == 1)
                    {
                        gameController.inGame = false;
                        StaticVariables.gameState = 0;
                        player.position = Sub.defaultPosition;
                        i = gameController.obstacles.Count;
                        gameController.obstacles.Clear();

                        MediaPlayer.Stop();
                        playOnce = true;

                    }
                }
                //gameController.obstacles.RemoveAll(a => a.offscreen);

            } else if (StaticVariables.gameState == 2)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();
                // Help Screen Back Button Handle

            } else if (StaticVariables.gameState == 3)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();

            } else if (StaticVariables.gameState == 4)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();

            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();


            spriteBatch.Draw(water_Sprite, new Vector2(0, 0), Color.White);
            
            KeyboardState kState = Keyboard.GetState();

            if (StaticVariables.gameState == 0)
            {
                string menuMessage = "Start Game";
                string menuMessage2 = "Help";
                string menuMessage3 = "About";
                Vector2 sizeOfText = gameFont.MeasureString(menuMessage);
                Vector2 sizeOfText2 = gameFont.MeasureString(menuMessage2);
                Vector2 sizeOfText3 = gameFont.MeasureString(menuMessage3);

                spriteBatch.DrawString(gameFont, menuMessage, new Vector2(640 - sizeOfText.X / 2, 200), Color.White);
                spriteBatch.DrawString(gameFont, menuMessage2, new Vector2(640 - sizeOfText2.X / 2, 270), Color.White);
                spriteBatch.DrawString(gameFont, menuMessage3, new Vector2(640 - sizeOfText3.X / 2, 340), Color.White);

                if (StaticVariables.selectedMenu == 1)
                {
                    spriteBatch.DrawString(gameFont, menuMessage, new Vector2(640 - sizeOfText.X / 2, 200), Color.DarkRed);
                }
                else if (StaticVariables.selectedMenu == 2)
                {
                    spriteBatch.DrawString(gameFont, menuMessage2, new Vector2(640 - sizeOfText2.X / 2, 270), Color.DarkRed);
                    
                }
                else if (StaticVariables.selectedMenu == 3)
                {
                    spriteBatch.DrawString(gameFont, menuMessage3, new Vector2(640 - sizeOfText3.X / 2, 340), Color.DarkRed);
                }
                
                if (kState.IsKeyDown(Keys.Enter) && StaticVariables.selectedMenu == 2)
                {
                    StaticVariables.gameState = 2;
                }
                spriteBatch.DrawString(timerFont, "Best Time: " + Math.Floor(StaticVariables.bestTime).ToString(), new Vector2(3, 3), Color.White);

            }
            else if (StaticVariables.gameState == 1) // Game Playing
            {
                spriteBatch.Draw(sub_Sprite, new Vector2(player.position.X - 34, player.position.Y - 50), Color.White);
                List<Texture2D> objects = new List<Texture2D>();
                
                objects.Add(fish_Sprite);
                objects.Add(bomb_Sprite);
                objects.Add(mine_Sprite);
                objects.Add(shark_Sprite);
                objects.Add(octopus_Sprite);
                
                //Randomize the obstacles
                /*var random = new Random();
                int index = random.Next(5);*/

                if (gameController.inGame == true && playOnce == true)
                {
                    
                    playOnce = false;

                }

                for (int i = 0; i < gameController.obstacles.Count; i++)
                {
                    Vector2 tempPos = gameController.obstacles[i].position;
                    int tempRadius = gameController.obstacles[i].radius;
                    if (i%5==0)
                    {
                        spriteBatch.Draw(objects[4], new Vector2(tempPos.X - tempRadius, tempPos.Y - tempRadius), Color.White);
                    } else if (i % 4 == 0)
                    {
                        spriteBatch.Draw(objects[3], new Vector2(tempPos.X - tempRadius, tempPos.Y - tempRadius), Color.White);
                    }
                    else if (i % 3 == 0)
                    {
                        spriteBatch.Draw(objects[2], new Vector2(tempPos.X - tempRadius, tempPos.Y - tempRadius), Color.White);
                    }
                    else if (i % 2 == 0)
                    {
                        spriteBatch.Draw(objects[1], new Vector2(tempPos.X - tempRadius, tempPos.Y - tempRadius), Color.White);
                    } else
                    {
                        spriteBatch.Draw(objects[0], new Vector2(tempPos.X - tempRadius, tempPos.Y - tempRadius), Color.White);
                    }




                }

                spriteBatch.DrawString(timerFont, "Time: " + Math.Floor(gameController.totalTime).ToString(), new Vector2(3, 3), Color.White);
            } else if (StaticVariables.gameState == 2)
            {
                // Help Section

                GraphicsDevice.Clear(Color.CornflowerBlue);
                spriteBatch.Draw(water_Sprite, new Vector2(0, 0), Color.White);
                string help1 = "This is a keyboard only 2D submarine game";
                string help2 = "Use arrow keys left <- and right -> to move the submarine forward and back.";
                string help3 = "Use arrow keys up and down to keep the submarine away from obstacles";
                string help4 = "Once you hit any obstacle, the game will be over";
                string help5 = "The objective of the game is to keep going as long as you can without hitting anything.";
                string help6 = "At any time, Press Backspace for main menu, Esc to exit the game";
                Vector2 sizeOfText1 = helpFont.MeasureString(help1);
                Vector2 sizeOfText2 = helpFont.MeasureString(help2);
                Vector2 sizeOfText3 = helpFont.MeasureString(help3);
                Vector2 sizeOfText4 = helpFont.MeasureString(help4);
                Vector2 sizeOfText5 = helpFont.MeasureString(help5);
                Vector2 sizeOfText6 = helpFont.MeasureString(help6);

                spriteBatch.DrawString(helpFont, help1, new Vector2(640 - sizeOfText1.X / 2, 200), Color.White);
                spriteBatch.DrawString(helpFont, help2, new Vector2(640 - sizeOfText2.X / 2, 250), Color.White);
                spriteBatch.DrawString(helpFont, help3, new Vector2(640 - sizeOfText3.X / 2, 300), Color.White);
                spriteBatch.DrawString(helpFont, help4, new Vector2(640 - sizeOfText4.X / 2, 350), Color.White);
                spriteBatch.DrawString(helpFont, help5, new Vector2(640 - sizeOfText5.X / 2, 400), Color.White);
                spriteBatch.DrawString(helpFont, help6, new Vector2(640 - sizeOfText6.X / 2, 450), Color.Black);


                if (kState.IsKeyDown(Keys.Back) && StaticVariables.selectedMenu == 2)
                {
                    StaticVariables.gameState = 0;
                }
                

            } else if (StaticVariables.gameState == 3)
            {
                // About Section

                GraphicsDevice.Clear(Color.CornflowerBlue);
                spriteBatch.Draw(water_Sprite, new Vector2(0, 0), Color.White);
                string about1 = "Developer: Harjeet Singh";
                string about2 = "Press Backspace for main menu, Esc to exit the game";
                
                Vector2 sizeOfText1 = gameFont.MeasureString(about1);
                Vector2 sizeOfText2 = gameFont.MeasureString(about2);

                spriteBatch.DrawString(gameFont, about1, new Vector2(640 - sizeOfText1.X / 2, 200), Color.White);
                
                spriteBatch.DrawString(gameFont, about2, new Vector2(640 - sizeOfText2.X / 2, 450), Color.Black);


                if (kState.IsKeyDown(Keys.Back) && StaticVariables.selectedMenu == 3)
                {
                    StaticVariables.gameState = 0;
                }


            }
            else if (StaticVariables.gameState == 4)
            {

                
                // Game Over Screen

                GraphicsDevice.Clear(Color.CornflowerBlue);
                spriteBatch.Draw(water_Sprite, new Vector2(0, 0), Color.White);
                string gameOver1 = "Game Over";
                string gameOver2 = "Your score is "+ Math.Floor(gameController.totalTime).ToString();
                string gameOver3 = "Backspace :  Main Menu ";
                string gameOver4 = "Enter          :  Play Again ";

                string congrats = "Congratulations, You are the new high scorer!!!!!";
                Vector2 sizeOfText5 = gameFont.MeasureString(congrats);


                Vector2 sizeOfText1 = gameFont.MeasureString(gameOver1);
                Vector2 sizeOfText2 = gameFont.MeasureString(gameOver2);
                Vector2 sizeOfText3 = helpFont.MeasureString(gameOver3);
                Vector2 sizeOfText4 = helpFont.MeasureString(gameOver4);

                spriteBatch.DrawString(gameFont, gameOver1, new Vector2(640 - sizeOfText1.X / 2, 180), Color.Red);

               

                if (Math.Floor(gameController.totalTime) >= (Math.Floor(StaticVariables.bestTime)))
                {
                    spriteBatch.DrawString(gameFont, congrats, new Vector2(640 - sizeOfText5.X / 2, 280), Color.Green);
                }

                spriteBatch.DrawString(gameFont, gameOver2, new Vector2(640 - sizeOfText2.X / 2, 380), Color.White);

                spriteBatch.DrawString(helpFont, gameOver3, new Vector2(640 - sizeOfText3.X / 2, 550), Color.White);

                spriteBatch.DrawString(helpFont, gameOver4, new Vector2(640 - sizeOfText4.X / 2, 600), Color.White);


                

                if (kState.IsKeyDown(Keys.Back))
                {
                    StaticVariables.gameState = 0;
                }
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
