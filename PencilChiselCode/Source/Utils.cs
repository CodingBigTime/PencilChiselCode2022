using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;

namespace PencilChiselCode.Source;

public static class Utils
{
    public static readonly Random Random = new();

    public static Vector2 GetCenterStartCoords(Vector2 drawableSize, Vector2 containerSize) =>
        new((containerSize.X - drawableSize.X) / 2, (containerSize.Y - drawableSize.Y) / 2);

    public static bool IsPointInRectangle(Vector2 point, Rectangle rectangle) =>
        rectangle.Contains(point);

    public static bool IsPointInRectangle(
        Vector2 point,
        Vector2 rectanglePosition,
        Vector2 rectangleSize
    ) => new Rectangle(rectanglePosition.ToPoint(), rectangleSize.ToPoint()).Contains(point);

    public static bool Intersects(Rectangle rectangle1, Rectangle rectangle2) =>
        rectangle1.Intersects(rectangle2);

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

    public static CircleF Expand(this CircleF circle, int radius) =>
        new(circle.Center, circle.Radius + radius);

    public static void DrawOutlinedString(
        this SpriteBatch spriteBatch,
        BitmapFont font,
        string text,
        Vector2 position,
        Color frontColor,
        Color backColor
    )
    {
        spriteBatch.DrawString(font, text, position + new Vector2(1, 1), backColor);
        spriteBatch.DrawString(font, text, position + new Vector2(-1, 1), backColor);
        spriteBatch.DrawString(font, text, position + new Vector2(1, -1), backColor);
        spriteBatch.DrawString(font, text, position + new Vector2(-1, -1), backColor);

        spriteBatch.DrawString(font, text, position, frontColor);
    }
    public static void ShiftWindowMode(ref this WindowMode windowMode) {
        windowMode = windowMode switch
        {
            WindowMode.Fullscreen => WindowMode.Windowed,
            WindowMode.Windowed => WindowMode.BorderlessFullscreen,
            WindowMode.BorderlessFullscreen => WindowMode.Fullscreen,
        };
    }

    public static void Deconstruct(this Size2 size, out float width, out float height)
    {
        width = size.Width;
        height = size.Height;
    }
}
