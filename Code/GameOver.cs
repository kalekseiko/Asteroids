using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System; // дл€ отладки

namespace AsteroidMono
{
    // класс дл€ отображени€ заставки игры
    public static class GameOver
    {
        public static Texture2D Background { get; set; } // картинка дл€ заставки
        static int timeCounter = 0;
        enum TimeCounterStatuses : int { Up, Down };
        static TimeCounterStatuses timeCounterStat = TimeCounterStatuses.Up;

        static Color colorText;
        public static SpriteFont HeaderFont { get; set; }
        public static SpriteFont TextFont { get; set; }

        static Vector2 headerTextPosition = new Vector2(50, 50); // координаты главной надписи
        static Vector2 textPosition = new Vector2(50, 100); // координаты дополнительной надписи

        static public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Background, Vector2.Zero, Color.White); // отрисовываем картинку
            spriteBatch.DrawString(HeaderFont, "¬ы проиграли", headerTextPosition, Color.DarkOrange); // отрисовываем надпись
            spriteBatch.DrawString(TextFont, "Ќо вы можете нажать пробел и продолжить играть...", textPosition, colorText); // отрисовываем надпись

        }

        static public void Update()
        {
            colorText = Color.FromNonPremultiplied(255, 255, 255, timeCounter % 255);
            if (timeCounter <= 100)
            {
                timeCounterStat = TimeCounterStatuses.Up;

            }
            else if (timeCounter >= 150)
            {
                timeCounterStat = TimeCounterStatuses.Down;
            }

            if (timeCounterStat == TimeCounterStatuses.Up)
            {
                timeCounter++;
            }
            else
            {
                timeCounter--;
            }
        }

    }
}
