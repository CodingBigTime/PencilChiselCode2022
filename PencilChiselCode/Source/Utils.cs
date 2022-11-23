using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;

namespace PencilChiselCode.Source;

public static class Utils
{
    public static readonly Random RANDOM = new();

    public static Vector2 GetCenterStartCoords(Vector2 drawableSize, Vector2 containerSize) =>
        new((containerSize.X - drawableSize.X) / 2, (containerSize.Y - drawableSize.Y) / 2);

    public static bool IsPointInRectangle(Vector2 point, Rectangle rectangle) =>
        point.X >= rectangle.X && point.X <= rectangle.X + rectangle.Width &&
        point.Y >= rectangle.Y && point.Y <= rectangle.Y + rectangle.Height;

    public static bool IsPointInRectangle(Vector2 point, Vector2 rectanglePosition, Vector2 rectangleSize) =>
        IsPointInRectangle(point,
            new Rectangle((int)rectanglePosition.X, (int)rectanglePosition.Y, (int)rectangleSize.X,
                (int)rectangleSize.Y));

    public static int GetRandomInt(int min, int max) => new Random().Next(min, max);
    public static Vector2 ToVector2(this Point point) => new(point.X, point.Y);
    public static Point ToPoint(this Vector2 vector) => new((int)vector.X, (int)vector.Y);

    public static float GetAverageSize(this Vector2 vector) => (vector.X + vector.Y) / 2;

    public static Vector2 Clamp(Vector2 vector, float min, float max) =>
        new(Math.Clamp(vector.X, min, max), Math.Clamp(vector.Y, min, max));

    public static Rectangle CreateRectangle(Vector2 position, Vector2 size) =>
        new(position.ToPoint(), size.ToPoint());

    public static CircleF CreateCircle(Vector2 position, float radius) =>
        new(position.ToPoint(), radius);

    public static Rectangle Expand(this Rectangle rectangle, int radius)
    {
        var position = rectangle.Location.ToVector2() - new Vector2(radius);
        var size = rectangle.Size.ToVector2() + new Vector2(radius * 2);
        return CreateRectangle(position, size);
    }

    public static CircleF Expand(this CircleF circle, int radius) => new(circle.Center, circle.Radius + radius);

    public static void DrawOutlinedString(this SpriteBatch spriteBatch, BitmapFont font, string text, Vector2 position,
        Color frontColor,
        Color backColor, HorizontalFontAlignment horizontalFontAlignment = HorizontalFontAlignment.Left,
        VerticalFontAlignment verticalFontAlignment = VerticalFontAlignment.Top)
    {
        var (x, y) = font.MeasureString(text);

        x = horizontalFontAlignment switch
        {
            HorizontalFontAlignment.Center => -x / 2F,
            HorizontalFontAlignment.Right => -x,
            _ => 0
        };

        y = verticalFontAlignment switch
        {
            VerticalFontAlignment.Center => y / 2F,
            VerticalFontAlignment.Bottom => y,
            _ => 0
        };

        position += new Vector2(x, y);

        spriteBatch.DrawString(font, text, position + new Vector2(1, 1), backColor);
        spriteBatch.DrawString(font, text, position + new Vector2(-1, 1), backColor);
        spriteBatch.DrawString(font, text, position + new Vector2(1, -1), backColor);
        spriteBatch.DrawString(font, text, position + new Vector2(-1, -1), backColor);

        spriteBatch.DrawString(font, text, position, frontColor);
    }

    public enum HorizontalFontAlignment
    {
        Left,
        Center,
        Right
    }

    public enum VerticalFontAlignment
    {
        Top,
        Center,
        Bottom
    }

    public static void Deconstruct(this Size2 size, out float width, out float height)
    {
        width = size.Width;
        height = size.Height;
    }
}