using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace AsteroidMono
{
    // сама игра будет обрабатываться в этом классе
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

        static public List<Fire> firesList { get; set; }
        static public int fireTimerCounter = 0; // костыль ограничивающий количество выстрелов при нажатии срельбы игроком

        // инициализация
        static public void Init (SpriteBatch SpriteBatch, int Width, int Height)
        {
            Asteroids.screenWidth = (Width > 100) ? Width : Game1.ScreenWidth;
            Asteroids.screenHeight = (Height > 100) ? Height : Game1.ScreenHeight;
            Asteroids.SpriteBatch = SpriteBatch;
            
            backgrounds = new Background[3];
            currentBackground = rnd.Next(0, backgrounds.Length);
            for (int i = 0; i < backgrounds.Length; i++)
            {
                backgrounds[i] = new Background(new Vector2(screenWidth, 0), new Vector2(-1, 0), 0, i);
            }

            starsNear = new Star[50];
            int spriteY = 0;
            for (int i = 0; i < starsNear.Length; i++)
            {
                if (spriteY >= 15) spriteY = 0; else spriteY++;
                starsNear[i] = new Star(new Vector2(-1, -1), new Vector2(-rnd.Next(4, 10), 0), 0, spriteY);
            }
            starsFar = new Star[200];
            spriteY = 16;
            for (int i = 0; i < starsFar.Length; i++)
            {
                if (spriteY >= 24) spriteY = 0; else spriteY++;
                starsFar[i] = new Star(new Vector2(-1, -1), new Vector2(-rnd.Next(1, 4), 0), 0, spriteY);
            }

            StarShip1 = new StarShip(new Vector2(20, screenHeight / 2 - 36), new Vector2(0,0), 0, 0, 5);
            
            firesList = new List<Fire>();
        }

        // обновление состояния игровых объектов
        static public void Update()
        {
            if (backgrounds[currentBackground].ChangeSprite )
            {
                currentBackground = rnd.Next(0, backgrounds.Length);
            }
            backgrounds[currentBackground].Update();
            
            foreach (Star star in starsNear)
                star.Update();
            foreach (Star star in starsFar)
                star.Update();

            foreach (Fire shoot in firesList) 
            {
                if (shoot.CanHide == false)
                    shoot.Update();
            }

            StarShip1.Update();
        }
        // отрисовка
        static public void Draw()
        {
            backgrounds[currentBackground].Draw();

            foreach (Star star in starsNear)
                star.Draw();
            foreach (Star star in starsFar)
                star.Draw();

            foreach (Fire shoot in firesList) 
            {
                if (shoot.CanHide == false)
                    shoot.Draw();
            }

            StarShip1.Draw();
        }

        /*
         *  **********\/ ОСТАЛЬНЫЕ МЕТОДЫ \/**********
        */
        
        // генератор ПСЧ
        static public int GetIntRnd(int min, int max)
        {
            return rnd.Next(min, max);
        }
        // выстрел корабля
        static public void Shoot() 
        {
            fireTimerCounter++;
                if (fireTimerCounter == 1 )
                    firesList.Add(new Fire(StarShip1.GetPosForFire, new Vector2(0, 0), 0, 0, 30));
            Console.WriteLine(fireTimerCounter);
        }
    }

    /*
     *  **********\/ ИГРОВЫЕ ОБЪЕКТЫ \/**********
    */    

    // Абстрактный класс на основе которого будем создавать остальные классы
    abstract class BasedObject
    {
        public Vector2 Pos; // текущая позиция (x и y)
        protected Vector2 Dir; // направление движения

        protected Color color; // как ни странно это цвет спрайта
        
        // Спрайты
        protected Point currentFrame; 
        protected Point spriteSize;
        // размеры фрейма на спрайтовой карте
        protected int frameWidth = 16; 
        protected int frameHeight = 16; 

        // конструктор
        protected BasedObject(Vector2 pos, Vector2 dir, int spriteX, int spriteY) 
        {
            Pos = pos;
            Dir = dir;
            color = Color.FromNonPremultiplied(255, 255, 255, 255); // по умолчанию белый
            spriteSize = new Point(1, 1);
            currentFrame = new Point(spriteX, spriteY);
            frameWidth = 16;
            frameHeight = 16;
        }
 
        // логика (например перемещение) объекта
        public abstract void Update();
        //public abstract float Update();
        // отрисовка объекта
        public abstract void Draw();
    }

    class Star: BasedObject
    {

        public static Texture2D Texture2D { get; set; }

        public Star(Vector2 pos, Vector2 dir, int spriteX, int spriteY)
            :base (pos, dir, spriteX, spriteY)
        {
            if ((pos.X < 0) || (pos.Y < 0))
                Pos = RandomSet(true);
            else 
                Pos = pos;
        }

        public override void Update()
        {
            Pos += Dir;
            if (Pos.X < 0)
            {
                Pos = RandomSet();
            }
        }

        public override void Draw()
        {
            Asteroids.SpriteBatch.Draw(Texture2D, Pos, new Rectangle(currentFrame.X * frameWidth, currentFrame.Y * frameHeight, frameWidth, frameHeight), color, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
        }

        Vector2 RandomSet(bool firstGen = false)
        {
            if (firstGen)
            {
                return new Vector2(Asteroids.GetIntRnd(0, Asteroids.screenWidth), Asteroids.GetIntRnd(0, Asteroids.screenHeight));
            } else
            {
                return new Vector2(Asteroids.GetIntRnd(Asteroids.screenWidth, Asteroids.screenWidth + 300), Asteroids.GetIntRnd(0, Asteroids.screenHeight));
            }
        }

    }

    class Background: BasedObject
    {
        public bool ChangeSprite {get; set; }

        public static Texture2D Texture2D { get; set; }

        public Background(Vector2 pos, Vector2 dir, int spriteX, int spriteY)
            :base (pos, dir, spriteX, spriteY)
        {
            frameWidth = 400;
            frameHeight = 250;
            ChangeSprite = false;
            Pos.Y -= 10; // Корректировка из-за масштабирования
        }

         public override void Update()
        {
            Pos += Dir;
            if ((Pos.X + Asteroids.screenWidth) < 0)
            {
                Pos.X = Asteroids.screenWidth;
                ChangeSprite = true;
            } else 
                ChangeSprite = false;
        }

        public override void Draw()
        {
            Asteroids.SpriteBatch.Draw(Texture2D, Pos, new Rectangle(currentFrame.X * frameWidth, currentFrame.Y * frameHeight, frameWidth, frameHeight), color, 0, Vector2.Zero, 4.4f, SpriteEffects.None, 0);
        }
    }


    class StarShip: BasedObject
    {
        public int Speed { get; set; }

        public static Texture2D Texture2D { get; set; }

        public StarShip(Vector2 pos, Vector2 dir, int spriteX, int spriteY, int speed)
            :base (pos, dir, spriteX, spriteY)
        {
            Speed = speed;
            frameWidth = 64;
            frameHeight = 73;
        }

        public Vector2 GetPosForFire => new Vector2(Pos.X, Pos.Y);

        public void Up()
        {
            if ((this.Pos.Y) > 0)
            {
                this.Dir.Y -= Speed;
            }
        }
        public void Down()
        {
            if (this.Pos.Y < (Asteroids.screenHeight-Texture2D.Height))
            {
                this.Dir.Y += Speed;
            }
        }
        public void Left()
        {
            if (this.Pos.X > 0)
            {
                this.Dir.X -= Speed;
            }
        }
        public void Right()
        {
            if (this.Pos.X < (Asteroids.screenWidth-Texture2D.Width))
            {
                this.Dir.X += Speed;
            }
        }


        public override void Update()
        {
            Pos += Dir;
            Dir = new Vector2(0, 0);
        }
        public override void Draw()
        {
            Asteroids.SpriteBatch.Draw(Texture2D, Pos, new Rectangle(currentFrame.X * frameWidth, currentFrame.Y * frameHeight, frameWidth, frameHeight), color, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
        }
    }

    class Fire: BasedObject 
    {
        // Hide = true спрятать и не просчитывать этот высрел 
        public bool CanHide {set; get;}
        int speed; // скоростиь полета снаряда

        public static Texture2D Texture2D { get; set; }
        
        
        public Fire(Vector2 pos, Vector2 dir, int spriteX, int spriteY, int speed)
            :base (pos, dir, spriteX, spriteY)
        {
            this.speed = speed;
            Dir.X = speed;
            CanHide = false;
            frameWidth = 76;
            frameHeight = 76;
        }

        public override void Update()
        {
            Pos += Dir;
            if (Pos.X > Asteroids.screenWidth) 
            {
                CanHide = true;
            }
        }
        public override void Draw()
        {
            Asteroids.SpriteBatch.Draw(Texture2D, Pos, new Rectangle(currentFrame.X * frameWidth, currentFrame.Y * frameHeight, frameWidth, frameHeight), color, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
        }
    }

}
