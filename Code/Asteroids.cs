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
        public static int elapsedGameTime; // количество милисекунд на 1 игровой цикл
        static public SpriteBatch SpriteBatch { get; set; }
        public static Random rnd = new Random();

        static Background[] backgrounds;
        static int currentBackground;

        static Star[] starsNear;
        static Star[] starsFar;

        static public StarShip StarShip1;

        static public Engine SustainerEngine;
        static public Engine leftTopEngine;
        static public Engine rightTopEngine;
        static public Engine leftBottomEngine;
        static public Engine rightBottomEngine;

        static public List<BigFire> firesList { get; set; }
        static public int fireTimerCounter = 0; // костыль ограничивающий количество выстрелов при нажатии срельбы игроком (для одиночного выстрела)

        static public Asteroid[] asteroidsArray;

        static public Blast blast;


        static public int ElapsedGameTime 
        {
            get {return  elapsedGameTime; }
        }

        // инициализация
        static public void Init (SpriteBatch SpriteBatch, int Width, int Height)
        {
            Asteroids.screenWidth = (Width > 100) ? Width : Game1.ScreenWidth;
            Asteroids.screenHeight = (Height > 100) ? Height : Game1.ScreenHeight;
            Asteroids.SpriteBatch = SpriteBatch;

            elapsedGameTime = 0;
            
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

            SustainerEngine = new Engine(new Vector2(-96,23), new Vector2(0,0), 0, 0, 0f, 1.5f, true);
            leftTopEngine = new Engine(new Vector2(10, -26), new Vector2(0,0), 0, 0, 0.78f, 0.5f);
            rightTopEngine = new Engine(new Vector2(89, -16), new Vector2(0,0), 0, 0, 2.3f, 0.5f);
            leftBottomEngine = new Engine(new Vector2(0, 108), new Vector2(0,0), 0, 0, 5.4f, 0.5f);
            rightBottomEngine = new Engine(new Vector2(81, 116), new Vector2(0,0), 0, 0, 3.9f, 0.5f);

            StarShip1 = new StarShip(new Vector2(20, screenHeight / 2 - 36), new Vector2(0,0), 0, 0, 5, SustainerEngine, leftTopEngine, rightTopEngine, leftBottomEngine, rightBottomEngine);
            
            firesList = new List<BigFire>();

            asteroidsArray = new Asteroid[10];
            for (int i = 0; i < asteroidsArray.Length; i++)
            {
                if (spriteY >= 6) spriteY = 0; else spriteY++;
                asteroidsArray[i] = new Asteroid(new Vector2(GetIntRnd(screenWidth, screenWidth + 300), GetIntRnd(64, screenHeight - 64)), new Vector2(0,0), GetIntRnd(0, 6), 0, GetIntRnd(3, 6));
            }
            blast = new Blast(new Vector2(0, 0), new Vector2(0, 0), 0, 0);
        }

        // перезапуск игры (например после проигрыша)
         static public void Reset ()
         {
            for (int i = 0; i < backgrounds.Length; i++)
            {
                backgrounds[i].Reset(new Vector2(screenWidth, 0), new Vector2(-1, 0));
            }
            for (int i = 0; i < starsNear.Length; i++)
            {
                starsNear[i].Reset();
            }
            for (int i = 0; i < starsFar.Length; i++)
            {
                starsFar[i].Reset();
            }
            StarShip1.Reset();
            firesList.Clear();
            for (int i = 0; i < asteroidsArray.Length; i++)
            {
                asteroidsArray[i].Reset(new Vector2(GetIntRnd(screenWidth, screenWidth + 300), GetIntRnd(64, screenHeight - 64)), new Vector2(0,0));
            }
            blast.Reset();
         }

        // обновление состояния игровых объектов
        static public void Update()
        {
            elapsedGameTimeUpdate();

            if (backgrounds[currentBackground].ChangeSprite )
            {
                currentBackground = rnd.Next(0, backgrounds.Length);
            }
            backgrounds[currentBackground].Update();
            
            foreach (Star star in starsNear)
                star.Update();
            foreach (Star star in starsFar)
                star.Update();
            for (int i=0; i<asteroidsArray.Length; i++) 
            {
                asteroidsArray[i].Update();
                // Проверка столкновений
                if (asteroidsArray[i].Collision(StarShip1)) {
                    asteroidsArray[i].StarShipCollide(StarShip1.Collide());
                }
                foreach (BigFire shoot in firesList) 
                {
                    if (shoot.CanHide == false) 
                    {
                        if (asteroidsArray[i].Collision(shoot)) 
                        {
                            blast.Show(asteroidsArray[i].Destroy());
                            shoot.Destroy();
                        }
                    }
                }
                for (int j=0; j<asteroidsArray.Length; j++) {
                    if (j == i )
                    {
                        continue;
                    } else if (asteroidsArray[i].Collision(asteroidsArray[j])) {
                        Vector2 TempVector2 = asteroidsArray[i].GetDir();
                        asteroidsArray[i].AsterroidCollide(asteroidsArray[j].GetDir());
                        asteroidsArray[j].AsterroidCollide(TempVector2);
                    }
                }
            }
            
            foreach (BigFire shoot in firesList) 
            {
                if (shoot.CanHide == false)
                    shoot.Update();

            }
            StarShip1.Update();

            SustainerEngine.SetPos(StarShip1.GetPos);
            leftTopEngine.SetPos(StarShip1.GetPos);
            rightTopEngine.SetPos(StarShip1.GetPos);
            leftBottomEngine.SetPos(StarShip1.GetPos);
            rightBottomEngine.SetPos(StarShip1.GetPos);

            SustainerEngine.Update();
            leftTopEngine.Update();
            rightTopEngine.Update();
            leftBottomEngine.Update();
            rightBottomEngine.Update();
            
            blast.Update();
        }

        // отрисовка
        static public void Draw()
        {
            backgrounds[currentBackground].Draw();

            foreach (Star star in starsNear)
                star.Draw();
            foreach (Star star in starsFar)
                star.Draw();
            foreach (Asteroid asteroid in asteroidsArray)
                asteroid.Draw();
            foreach (BigFire shoot in firesList) 
            {
                if (shoot.CanHide == false)
                    shoot.Draw();
            }

            StarShip1.Draw();
            SustainerEngine.Draw();
            leftTopEngine.Draw();
            rightTopEngine.Draw();
            leftBottomEngine.Draw();
            rightBottomEngine.Draw();

            blast.Draw();
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
                    firesList.Add(new BigFire(StarShip1.GetPos, new Vector2(0, 0), 0, 0, 30));
        }
        // обновляем параметр миллисекуд на цикл
        static public void elapsedGameTimeUpdate() 
        {
            if (Game1.getCurrentGameTime != null )
                elapsedGameTime = Game1.getCurrentGameTime.ElapsedGameTime.Milliseconds;
        }
    }

    // Класс для хранения размеров объектов
    class Size 
    {
        public float W{set; get;} // Width
        public float H{set; get;} // Height
    }

    // Интерфейс в котором мы реализуем столкновения объектов
    interface ICollision
    {
        bool Collision (ICollision obj);
        Rectangle Rect {get;}
    }

    /*
     *  **********\/ ИГРОВЫЕ ОБЪЕКТЫ \/**********
    */    

    // Абстрактный класс на основе которого будем создавать остальные классы
    abstract class BasedObject : ICollision
    {
        protected Vector2 Pos; // текущая позиция (x и y)
        protected Vector2 Dir; // направление движения

        protected Color color; // как ни странно это цвет спрайта
        
        // Спрайты
        protected Point currentFrame; 
        protected Point spriteSize;
        // размеры фрейма на спрайтовой карте
        protected int frameWidth = 16; 
        protected int frameHeight = 16;
        protected Size size = new Size();
        // поправочный коэффициент на который будет уменьшаться размер объекта
        protected int sizeCoef = 16; 


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
            size.W = frameWidth - sizeCoef;
            size.H = frameHeight - sizeCoef;
        }
 
        // логика (например перемещение) объекта
        public abstract void Update();
        //public abstract float Update();
        // отрисовка объекта
        public abstract void Draw();

        // Столкновения
        public bool Collision (ICollision o) => o.Rect.Intersects(this.Rect);
        public Rectangle Rect => new Rectangle((int)Pos.X + 4, (int)Pos.Y + 4, (int)size.W, (int)size.H);
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

        public void Reset()
        {
            Pos = RandomSet(true);
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

        public void Reset(Vector2 pos, Vector2 dir)
        {
            Pos = pos;
            Dir = dir;
            ChangeSprite = false;
            Pos.Y -= 10;
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
        
        Vector2 StartPos;
        Vector2 StartDir;

        public static Texture2D Texture2D { get; set; }

        Engine sustainerEngine;
        Engine leftTop;
        Engine rightTop;
        Engine leftBottom;
        Engine rightBottom;

        // игровые параметры корабля
        public int Strength {get; set;} // жизни корабля
        /*public int Fuel {get; }         // топливо / запас воды для движения
        public int MainEnergy {get;}    // энергия реактора
        public int Сharge {get;}        // заряд накопителей корабля (для оружия щитов и т.д.)*/


        public StarShip(Vector2 pos, Vector2 dir, int spriteX, int spriteY, int speed, 
                        Engine sustainerEngine, Engine leftTop, Engine rightTop, Engine leftBottom, Engine rightBottom)
            :base (pos, dir, spriteX, spriteY)
        {
            StartPos = pos;
            StartDir = dir;
            Speed = speed;
            frameWidth = 126;
            frameHeight = 90;
            currentFrame.X = spriteX;
            currentFrame.Y = spriteY;
            size.W = frameWidth - sizeCoef;
            size.H = frameHeight - sizeCoef;
            Strength = 100;
            this.sustainerEngine = sustainerEngine;
            this.leftTop = leftTop;
            this.rightTop = rightTop;
            this.leftBottom = leftBottom;
            this.rightBottom = rightBottom;
        }

        public void Reset ()
        {
            Pos = StartPos;
            Dir = StartDir;
            Strength = 100;
        }

        public Vector2 GetPos => new Vector2(Pos.X, Pos.Y);

        public void Up()
        {
                this.Dir.Y -= Speed;
                leftBottom.isOn = true;
                rightBottom.isOn = true;
        }
        public void Down()
        {
                this.Dir.Y += Speed;
                leftTop.isOn = true;
                rightTop.isOn = true;

        }
        public void Left()
        {
                this.Dir.X -= Speed;
                sustainerEngine.isOn = false;
                rightBottom.isOn = true;
                rightTop.isOn = true;
        }
        public void Right()
        {
                this.Dir.X += Speed;
                leftBottom.isOn = true;
                leftTop.isOn = true;
        }

        public Vector2 Collide() 
        {
            Strength -= 10;
            Dir = -Dir*2;
            return Dir;
        }

        bool PerimetrCheck() 
        {
            bool flag = true;
            if (Pos.X < 0) {Pos.X = 0; Dir.X = 0; flag = false;}
            if (Pos.X > Asteroids.screenWidth-Texture2D.Width) {Pos.X = Asteroids.screenWidth-Texture2D.Width; Dir.X = 0; flag = false;}
            if (Pos.Y < 0) {Pos.Y = 0; Dir.Y = 0; flag = false;}
            if (Pos.Y > Asteroids.screenHeight-Texture2D.Height) {Pos.Y = Asteroids.screenHeight-Texture2D.Height; Dir.Y = 0; flag = false;}
            return flag;
        }
         // ускорение торможение
        void SpeedBalance() 
        {
            if ((Dir.X > 0) & (Dir.X < 1)) Dir.X = 0; 
            else if (Dir.X >= 1) Dir.X--;
            if ((Dir.X < 0) & (Dir.X > -1)) Dir.X = 0; 
            else if (Dir.X <= -1) Dir.X++;
            if ((Dir.Y > 0) & (Dir.Y < 1)) Dir.Y = 0; 
            else if (Dir.Y >= 1) Dir.Y--;
            if ((Dir.Y < 0) & (Dir.Y > -1)) Dir.Y = 0; 
            else if (Dir.Y <= -1) Dir.Y++;
        }

        public override void Update()
        {
            //if (PerimetrCheck()) SpeedBalance();
            PerimetrCheck();
            Pos += Dir;
            Dir = new Vector2(0, 0);
        }

        public override void Draw()
        {
            Asteroids.SpriteBatch.Draw(Texture2D, Pos, new Rectangle(currentFrame.X * frameWidth, currentFrame.Y * frameHeight, frameWidth, frameHeight), color, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            Asteroids.SpriteBatch.DrawString(SplashScreen.TextFont, Strength.ToString(), new Vector2(10, Asteroids.screenHeight-50), color); // отрисовываем HP
        }
    }

    // анимация пламени двигателя
    class Engine: BasedObject 
    {
        // Pos относительно космического корабля
        Vector2 localPos;
        
        public static Texture2D Texture2D { get; set; }
        float rotate;
        float scale;

        public bool isOn {get; set;}
        bool IsIgnition;
        bool permaWork; // по умолчанию включенный/выключенный двигатель

        // Для ограничения скорости анимации
        int animSpeed = 50; // скорость анимации (чем больше число тем медленнее анимация)
        int currentTime;
        int period;
       
        public Engine(Vector2 pos, Vector2 dir, int spriteX, int spriteY, float rotate=0, float scale=1f, bool permaWork=false)
            :base (pos, dir, spriteX, spriteY)
        {
            localPos = pos;
            frameWidth = 64;
            frameHeight = 32;
            currentFrame.X = spriteX;
            currentFrame.Y = spriteY;
            isOn = true;
            IsIgnition = true;
            currentTime = 0;
            period = animSpeed; 
            this.rotate = rotate; 
            this.scale = scale; 
            this.permaWork = permaWork;
        }

        public void SetPos(Vector2 pos)
        {
            Pos.X = pos.X+localPos.X;
            Pos.Y = pos.Y+localPos.Y;
        }

        public override void Update()
        {
            currentTime += Asteroids.elapsedGameTime;

            if (currentTime > period) 
            {
                currentTime -= period;
                if (IsIgnition) 
                { 
                    period = animSpeed * 3;
                    currentFrame.X = 1;
                } else 
                {
                    period = animSpeed;
                    currentFrame.X = 0;
                }

                if (currentFrame.Y < 4)
                {
                    currentFrame.Y++;
                }
                else
                {
                    currentFrame.Y = 0;
                    IsIgnition = false;
                }

                if (!isOn) IsIgnition = true;

                isOn = permaWork ? true : false;
            }
        }

        public override void Draw()
        {
            if (isOn)
                Asteroids.SpriteBatch.Draw(Texture2D, Pos, new Rectangle(currentFrame.X * frameWidth, currentFrame.Y * frameHeight, frameWidth, frameHeight), color, rotate, Vector2.Zero, scale, SpriteEffects.None, 0);
        }
    }

    class BigFire: BasedObject 
    {
        // Hide = true спрятать и не просчитывать этот высрел 
        public bool CanHide {set; get;}
        int speed; // скоростиь полета снаряда

        public static Texture2D Texture2D { get; set; }
        
        public BigFire(Vector2 pos, Vector2 dir, int spriteX, int spriteY, int speed)
            :base (pos, dir, spriteX, spriteY)
        {
            Pos.X = pos.X;
            Pos.Y = pos.Y + 17;
            this.speed = speed;
            Dir.X = speed;
            CanHide = false;
            frameWidth = 224;
            frameHeight = 48;
            currentFrame.X = spriteX;
            currentFrame.Y = 0;
            size.W = frameWidth - sizeCoef;
            size.H = frameHeight - sizeCoef;
        }

        public void Destroy()
        {
           Pos.X = Asteroids.screenWidth + 10;
           CanHide = true;
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
            currentFrame.Y = (currentFrame.Y > 4) ? 0:currentFrame.Y+1;
            Asteroids.SpriteBatch.Draw(Texture2D, Pos, new Rectangle(currentFrame.X * frameWidth, currentFrame.Y * frameHeight, frameWidth, frameHeight), color, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
        }
    }

    class Asteroid: BasedObject 
    {
        int speed; // скоростиь полета астероида

        public static Texture2D Texture2D { get; set; }

        public Asteroid(Vector2 pos, Vector2 dir, int spriteX, int spriteY, int speed)
            :base (pos, dir, spriteX, spriteY)
        {
            this.speed = speed;
            Dir.X = speed;
            frameWidth = 64;
            frameHeight = 64;
            size.W = frameWidth - sizeCoef;
            size.H = frameHeight - sizeCoef;
            currentFrame.X = spriteX;
            currentFrame.Y = spriteY;
        }

        public void Reset(Vector2 pos, Vector2 dir)
        {
            Dir = pos;
            Pos = dir;
        }

        public Vector2 GetDir () 
        {
            return Dir;
        }

        public void StarShipCollide(Vector2 StarShipDir) 
        {
            //Pos.X += 10;
            //Dir.X = -Dir.X;
            //Dir.Y = Asteroids.GetIntRnd(1, 5);
            if (StarShipDir.X == 0) Dir.X = -Dir.X;
            else Dir.X = StarShipDir.X/2;
            if (StarShipDir.Y == 0) Dir.Y = Asteroids.GetIntRnd(-3, 3);
            else Dir.Y = StarShipDir.Y/2;
        }


        public void AsterroidCollide(Vector2 Asteroid1Dir) 
        {
            Dir = Asteroid1Dir*2;
        }

        public Vector2 Destroy()
        {
            Vector2 PredPos = Pos;
            Pos = RandomSet();
            return PredPos;
        }

        bool PerimetrCheck() 
        {
            bool flag = true;
            if (Pos.X + frameWidth < 0) flag = false;
            if (Pos.X > Asteroids.screenWidth + 300) flag = false;
            if (Pos.Y + frameHeight < 0) flag = false;
            if (Pos.Y > Asteroids.screenHeight) flag = false;
            return flag;
        }

        Vector2 RandomSet()
        {
            return new Vector2(Asteroids.GetIntRnd(Asteroids.screenWidth, Asteroids.screenWidth + 300), Asteroids.GetIntRnd(frameWidth, Asteroids.screenHeight-frameWidth));
        }

        public override void Update()
        {
            Pos -= Dir;
            if (!PerimetrCheck()) 
            {
                Dir.X = speed;
                Dir.Y = 0;
                Pos = RandomSet();
            }
        }


        public override void Draw()
        {
            Asteroids.SpriteBatch.Draw(Texture2D, Pos, new Rectangle(currentFrame.X * frameWidth, currentFrame.Y * frameHeight, frameWidth, frameHeight), color, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
        }

        
    }

    class ComplexAsteroid : BasedObject
    {
        int speed; // скоростиь полета астероида

        public static Texture2D Texture2D { get; set; }

        public ComplexAsteroid(Vector2 pos, Vector2 dir, int spriteX, int spriteY, int speed)
            : base(pos, dir, spriteX, spriteY)
        {
            this.speed = speed;
            Dir.X = speed;
            frameWidth = 64;
            frameHeight = 64;
            size.W = frameWidth - sizeCoef;
            size.H = frameHeight - sizeCoef;
            currentFrame.X = spriteX;
            currentFrame.Y = spriteY;
        }

        public override void Update()
        {
        }


        public override void Draw()
        {
            Asteroids.SpriteBatch.Draw(Texture2D, Pos, new Rectangle(currentFrame.X * frameWidth, currentFrame.Y * frameHeight, frameWidth, frameHeight), color, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
        }

    }

    class Blast: BasedObject
    {

        bool isShow;

         public static Texture2D Texture2D { get; set; }

        public Blast(Vector2 pos, Vector2 dir, int spriteX, int spriteY)
            :base (pos, dir, spriteX, spriteY)
        {
            frameWidth = 64;
            frameHeight = 64;
            currentFrame.X = spriteX;
            currentFrame.Y = spriteY;
            isShow = false;
        }

        public void Reset() 
        {
            isShow = false;
        }

        public override void Update() 
        {
            if (isShow) {
                currentFrame.X++;
                if (currentFrame.X > 7) {
                    currentFrame.X = 0;
                    isShow = false;
                }
            }
        }

        public void Show( Vector2 pos) {
            Pos = pos;
            isShow = true;
        }

        public override void Draw()
        {

            if (isShow) Asteroids.SpriteBatch.Draw(Texture2D, Pos, new Rectangle(currentFrame.X * frameWidth, currentFrame.Y * frameHeight, frameWidth, frameHeight), color, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
        }
    }

}
