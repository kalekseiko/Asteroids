using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace AsteroidMono
{
    class Asteroids
    {
        public static int screenWidth;
        public static int screenHeight;
        static public SpriteBatch SpriteBatch { get; set; }
        public static Random rnd = new Random();

        static Background[] backgrounds;
        static int currentBackground;

        static Star[] starsNear;
        static Star[] starsFar;

        static public StarShip StarShip1 { get; set; }
        
        
 
        static public int GetIntRnd(int min, int max)
        {
            return rnd.Next(min, max);
        }

        static public void Init (SpriteBatch SpriteBatch, int Width, int Height)
        {
            Asteroids.screenWidth = (Width > 100) ? Width : Game1.ScreenWidth;
            Asteroids.screenHeight = (Height > 100) ? Height : Game1.ScreenHeight;
            Asteroids.SpriteBatch = SpriteBatch;
            
            backgrounds = new Background[3];
            currentBackground = rnd.Next(0, backgrounds.Length);
            for (int i = 0; i < backgrounds.Length; i++)
            {
                backgrounds[i] = new Background(new Vector2(rnd.Next(0, screenWidth), 0), new Vector2(-1, 0), 0, i);
            }

            starsNear = new Star[50];
            int spriteY = 0;
            for (int i = 0; i < starsNear.Length; i++)
            {
                if (spriteY >= 15) spriteY = 0; else spriteY++;
                starsNear[i] = new Star(new Vector2(-rnd.Next(4, 10), 0), 0, spriteY);
            }
            starsFar = new Star[200];
            spriteY = 16;
            for (int i = 0; i < starsFar.Length; i++)
            {
                if (spriteY >= 24) spriteY = 0; else spriteY++;
                starsFar[i] = new Star(new Vector2(-rnd.Next(1, 4), 0), 0, spriteY);
            }

            StarShip1 = new StarShip(new Vector2(20, screenHeight / 2 - 36), 5);
        }

        static public void Draw()
        {
            backgrounds[currentBackground].Draw();

            foreach (Star star in starsNear)
                star.Draw();
            foreach (Star star in starsFar)
                star.Draw();

            StarShip1.Draw();
        }

        static public void Update()
        {
            float PosX = backgrounds[currentBackground].Update();
            //Console.WriteLine(PosX);
            if (PosX < 0 )
            {
                currentBackground = rnd.Next(0, backgrounds.Length);
            } else
            {
                backgrounds[currentBackground].Update();
            }


            foreach (Star star in starsNear)
                star.Update();
            foreach (Star star in starsFar)
                star.Update();
        }
    }

    class Star
    {
        Vector2 Pos; // позиция: x и y
        Vector2 Dir; // направление движения
        Color color = Color.FromNonPremultiplied(255, 255, 255, 255);

        Point currentFrame;
        Point spriteSize = new Point(1, 1);
        // размеры фрейма на спрайтовой карте
        int frameWidth = 16; 
        int frameHeight = 16; 
        

        public static Texture2D Texture2D { get; set; }

        public Star(Vector2 Pos, Vector2 Dir)
        {
            this.Pos = Pos;
            this.Dir = Dir;
        }

        public Star(Vector2 Dir, int spriteX, int spriteY)
        {
            this.Dir = Dir;
            RandomSet(true);
            currentFrame = new Point(spriteX, spriteY);
        }

        public void Update()
        {
            Pos += Dir;

            if (Pos.X < 0)
            {
                RandomSet();
            }
        }

        public void RandomSet(bool firstGen = false)
        {
            if (firstGen)
            {
                Pos = new Vector2(Asteroids.GetIntRnd(0, Asteroids.screenWidth), Asteroids.GetIntRnd(0, Asteroids.screenHeight));
            } else
            {
                Pos = new Vector2(Asteroids.GetIntRnd(Asteroids.screenWidth, Asteroids.screenWidth + 300), Asteroids.GetIntRnd(0, Asteroids.screenHeight));
            }
            color = Color.FromNonPremultiplied(255, 255, 255, 255);
        }

        public void Draw()
        {
            Asteroids.SpriteBatch.Draw(Texture2D, Pos, new Rectangle(currentFrame.X * frameWidth, currentFrame.Y * frameHeight, frameWidth, frameHeight), color, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
        }

    }

    class Background
    {
        Vector2 Pos; // позиция: x и y
        Vector2 Dir; // направление движения

        Point currentFrame;
        Point spriteSize = new Point(1, 1);
        // размеры фрейма на спрайтовой карте
        int frameWidth = Asteroids.screenWidth;
        int frameHeight = Asteroids.screenHeight;

        public static Texture2D Texture2D { get; set; }

        public Background(Vector2 Pos, Vector2 Dir, int spriteX, int spriteY)
        {
            this.Pos = Pos;
            this.Dir = Dir;
            currentFrame = new Point(spriteX, spriteY);
        }

        public float Update()
        {
            Pos += Dir;
            float posXRight = Pos.X + Asteroids.screenWidth;
            if (posXRight < 0)
            {
                Pos.X = Asteroids.screenWidth;
            }
            return posXRight;
        }

        public void Draw()
        {
            Asteroids.SpriteBatch.Draw(Texture2D, Pos, new Rectangle(currentFrame.X * frameWidth, currentFrame.Y * frameHeight, frameWidth, frameHeight), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
        }
    }


    class StarShip
    {
        Vector2 Pos; // позиция: x и y
        public int Speed { get; set; }

        public static Texture2D Texture2D { get; set; }

        public StarShip(Vector2 Pos, int speed)
        {
            this.Pos = Pos;
            this.Speed = speed;
        }

        public void Up()
        {
            if ((this.Pos.Y) > 0)
            {
                this.Pos.Y -= Speed;
            }
        }
        public void Down()
        {
            if (this.Pos.Y < (Asteroids.screenHeight-Texture2D.Height))
            {
                this.Pos.Y += Speed;
            }
        }
        public void Left()
        {
            if (this.Pos.X > 0)
            {
                this.Pos.X -= Speed;
            }
        }
        public void Right()
        {
            if (this.Pos.X < (Asteroids.screenWidth-Texture2D.Width))
            {
                this.Pos.X += Speed;
            }
        }

        public void Draw()
        {
            Asteroids.SpriteBatch.Draw(Texture2D, Pos, Color.White);
        }



    }

}
