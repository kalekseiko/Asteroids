using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System; // для отладки

namespace AsteroidMono
{
    // класс для отображения заставки игры
    static class SplashScreen
    {
        public static Texture2D Background { get; set; } // картинка для заставки
        static int timeCounter = 0;
        enum TimeCounterStatuses: int { Up, Down};
        static TimeCounterStatuses timeCounterStat = TimeCounterStatuses.Up;
        static Vector2 headerTextPosition = new Vector2(350, 50); // координаты главной надписи
        static Vector2 textPosition = new Vector2(350, 200); // координаты надписи "Нажмите пробел для начала игры"

        static Color colorText;
        public static SpriteFont HeaderFont { get; set; }
        public static SpriteFont TextFont { get; set; }

        static public void Draw (SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Background, Vector2.Zero, Color.White); // отрисовываем картинку
            spriteBatch.DrawString(HeaderFont, "Asteroids!", headerTextPosition, Color.FromNonPremultiplied(124, 123, 62, 255)); // отрисовываем надпись
            spriteBatch.DrawString(TextFont, "Нажмите пробел или левую кнопку мышки для старта игры...", textPosition, colorText); // отрисовываем надпись

        }

        static public void Update()
        {
            colorText = Color.FromNonPremultiplied(255, 255, 255, timeCounter % 255);
            if ( timeCounter <= 100)
            {
                timeCounterStat = TimeCounterStatuses.Up;
                
            } else if (timeCounter >= 150)
            {
                timeCounterStat = TimeCounterStatuses.Down;
            }

            if (timeCounterStat == TimeCounterStatuses.Up)
            {
                timeCounter++;
            } else
            {
                timeCounter--;
            }
                
            //Console.WriteLine(timeCounter);
        }

    }
}
