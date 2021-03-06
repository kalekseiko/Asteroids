using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System; // ��� �������

namespace AsteroidMono
{
    // ����� ��� ����������� �������� ����
    public static class GameOver
    {
        public static Texture2D Background { get; set; } // �������� ��� ��������
        static int timeCounter = 0;
        enum TimeCounterStatuses : int { Up, Down };
        static TimeCounterStatuses timeCounterStat = TimeCounterStatuses.Up;

        static Color colorText;
        public static SpriteFont HeaderFont { get; set; }
        public static SpriteFont TextFont { get; set; }

        static Vector2 headerTextPosition = new Vector2(50, 50); // ���������� ������� �������
        static Vector2 textPosition = new Vector2(50, 100); // ���������� �������������� �������

        static public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Background, Vector2.Zero, Color.White); // ������������ ��������
            spriteBatch.DrawString(HeaderFont, "�� ���������", headerTextPosition, Color.DarkOrange); // ������������ �������
            spriteBatch.DrawString(TextFont, "�� �� ������ ������ ������ � ���������� ������...", textPosition, colorText); // ������������ �������

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
