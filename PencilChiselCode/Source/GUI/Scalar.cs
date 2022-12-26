using System;
using Microsoft.Xna.Framework;

namespace PencilChiselCode.Source.GUI;

public abstract record Scalar;

public record Pixels : Scalar
{
    public int Value { get; }

    public Pixels(int Value) => this.Value = Value;

    public static implicit operator Pixels(int value) => new(value);

    public static Pixels operator +(Pixels pixels, int value) => new(pixels.Value + value);

    public static Pixels operator -(Pixels pixels, int value) => new(pixels.Value - value);

    public static Pixels operator *(Pixels pixels, int value) => value * pixels;

    public static Pixels operator *(int value, Pixels pixels) => new(value * pixels.Value);

    public static Pixels operator /(Pixels pixels, int value) => new(pixels.Value / value);

    public static Pixels operator +(Pixels pixels1, Pixels pixels2) =>
        new(pixels1.Value + pixels2.Value);

    public static Pixels operator -(Pixels pixels1, Pixels pixels2) =>
        new(pixels1.Value - pixels2.Value);

    public static Ratio operator /(Pixels pixels1, Pixels pixels2) =>
        new((float)pixels1.Value / pixels2.Value);

    public static Pixels operator *(Pixels pixels, float value) => new((int)(pixels.Value * value));

    public static Pixels operator *(float value, Pixels pixels) => pixels * value;

    public static Pixels operator /(Pixels pixels, float value) => new((int)(pixels.Value / value));

    public static Pixels operator +(Pixels pixels, Percent percent) =>
        new((int)(pixels.Value * (1 + percent.Value)));

    public static Pixels operator -(Pixels pixels, Percent percent) =>
        new((int)(pixels.Value * (1 - percent.Value)));

    public static Pixels operator *(Pixels pixels, Percent percent) =>
        new((int)(pixels.Value * percent.Value));

    public static Pixels operator *(Percent percent, Pixels pixels) => pixels * percent;

    public static Pixels operator *(Pixels pixels, Ratio ratio) =>
        new((int)(pixels.Value * ratio.Value));

    public static Pixels operator *(Ratio ratio, Pixels pixels) => pixels * ratio;

    public static Pixels operator /(Pixels pixels, Percent percent) =>
        new((int)(pixels.Value / percent.Value));

    public static Pixels operator +(Pixels pixels, Scalar scalar) =>
        scalar switch
        {
            Pixels pixels2 => pixels + pixels2,
            Percent percent => pixels + (pixels * percent),
            _ => throw new NotImplementedException()
        };

    public static Pixels operator -(Pixels pixels, Scalar scalar) =>
        scalar switch
        {
            Pixels pixels2 => pixels - pixels2,
            Percent percent => pixels - (pixels * percent),
            _ => throw new NotImplementedException()
        };
}

public record Percent : Scalar
{
    public float Value { get; }

    public Percent(float Value) => this.Value = Value;

    public static implicit operator Percent(float value) => new(value);

    public static Percent operator +(Percent percent, Percent value) =>
        new(percent.Value + value.Value);

    public static Percent operator -(Percent percent, Percent value) =>
        new(percent.Value - value.Value);

    public static Percent operator /(Percent percent, float value) => new(percent.Value / value);
}

public record Ratio : Scalar
{
    public float Value { get; }

    public Ratio(float Value) => this.Value = Value;
}

public record FitElement : Scalar;

public class ScalarVector2
{
    public Scalar X { get; }
    public Scalar Y { get; }

    public ScalarVector2(Scalar X, Scalar Y)
    {
        this.X = X;
        this.Y = Y;
    }

    public ScalarVector2(int X, int Y) : this(new Pixels(X), new Pixels(Y)) { }

    public ScalarVector2((int X, int Y) value) : this(value.X, value.Y) { }

    public ScalarVector2(int value) : this(new Pixels(value), new Pixels(value)) { }

    public ScalarVector2(float value) : this(new Percent(value), new Percent(value)) { }

    public ScalarVector2(float X, float Y) : this(new Percent(X), new Percent(Y)) { }

    public ScalarVector2((float X, float Y) value) : this(value.X, value.Y) { }

    public ScalarVector2(int X, float Y) : this(new Pixels(X), new Percent(Y)) { }

    public ScalarVector2((int X, float Y) value) : this(value.X, value.Y) { }

    public ScalarVector2(float X, int Y) : this(new Percent(X), new Pixels(Y)) { }

    public ScalarVector2((float X, int Y) value) : this(value.X, value.Y) { }

    public ScalarVector2(int X, Scalar Y) : this(new Pixels(X), Y) { }

    public ScalarVector2((int X, Scalar Y) value) : this(value.X, value.Y) { }

    public ScalarVector2(Scalar X, int Y) : this(X, new Pixels(Y)) { }

    public ScalarVector2((Scalar X, int Y) value) : this(value.X, value.Y) { }

    public ScalarVector2(float X, Scalar Y) : this(new Percent(X), Y) { }

    public ScalarVector2((float X, Scalar Y) value) : this(value.X, value.Y) { }

    public ScalarVector2(Scalar X, float Y) : this(X, new Percent(Y)) { }

    public ScalarVector2((Scalar X, float Y) value) : this(value.X, value.Y) { }

    public ScalarVector2(Scalar value) : this(value, value) { }

    public ScalarVector2((Scalar, Scalar) tuple) : this(tuple.Item1, tuple.Item2) { }

    public ScalarVector2(Vector2 vector2)
        : this(new Pixels((int)vector2.X), new Pixels((int)vector2.Y)) { }

    public static implicit operator ScalarVector2((Scalar, Scalar) tuple) => new(tuple);

    // up for debate
    public static implicit operator ScalarVector2(Vector2 vector2) => new(vector2);

    public static implicit operator Vector2(ScalarVector2 scalarVector2) =>
        scalarVector2.ToAbsoluteVector2Safe();

    public static implicit operator ScalarVector2(int value) => new(value);

    public static implicit operator ScalarVector2(float value) => new(value);

    public static implicit operator ScalarVector2((int, int) tuple) => new(tuple);

    public static implicit operator ScalarVector2((float, float) tuple) => new(tuple);

    public static implicit operator ScalarVector2((int, float) tuple) => new(tuple);

    public static implicit operator ScalarVector2((float, int) tuple) => new(tuple);

    public static implicit operator ScalarVector2((int, Scalar) tuple) => new(tuple);

    public static implicit operator ScalarVector2((Scalar, int) tuple) => new(tuple);

    public static implicit operator ScalarVector2((float, Scalar) tuple) => new(tuple);

    public static implicit operator ScalarVector2((Scalar, float) tuple) => new(tuple);

    public static ScalarVector2 operator *(ScalarVector2 scalarVector2, float scalar) =>
        scalarVector2 switch
        {
            { X: Pixels x, Y: Pixels y } => (x * scalar, y * scalar),
            _ => throw new NotImplementedException()
        };

    public static ScalarVector2 operator *(float scalar, ScalarVector2 scalarVector2) =>
        scalarVector2 * scalar;

    public static ScalarVector2 operator *(ScalarVector2 scalarVector2, Percent percent) =>
        scalarVector2 switch
        {
            { X: Pixels x, Y: Pixels y } => (x * percent, y * percent),
            _ => throw new NotImplementedException()
        };

    public static ScalarVector2 operator *(Percent percent, ScalarVector2 scalarVector2) =>
        scalarVector2 * percent;

    public static ScalarVector2 operator +(ScalarVector2 scalarVector2, ScalarVector2 value) =>
        scalarVector2 switch
        {
            { X: Pixels x, Y: Pixels y } => (x + value.X, y + value.Y),
            _ => throw new NotImplementedException()
        };

    public static ScalarVector2 operator -(ScalarVector2 scalarVector2, ScalarVector2 value) =>
        scalarVector2 switch
        {
            { X: Pixels x, Y: Pixels y } => (x - value.X, y - value.Y),
            _ => throw new NotImplementedException()
        };

    public static ScalarVector2 operator /(ScalarVector2 scalarVector2, float scalar) =>
        scalarVector2 switch
        {
            { X: Pixels x, Y: Pixels y } => (x / scalar, y / scalar),
            { X: Percent x, Y: Percent y } => (x / scalar, y / scalar),
            { X: Pixels x, Y: Percent y } => (x / scalar, y / scalar),
            { X: Percent x, Y: Pixels y } => (x / scalar, y / scalar),
            _ => throw new NotImplementedException()
        };

    public Vector2 ToVector2()
    {
        var x = X switch
        {
            Pixels pixels => pixels.Value,
            Percent percent => percent.Value,
            Ratio ratio => ratio.Value,
            _ => throw new NotImplementedException()
        };
        var y = Y switch
        {
            Pixels pixels => pixels.Value,
            Percent percent => percent.Value,
            Ratio ratio => ratio.Value,
            _ => throw new NotImplementedException()
        };
        return new(x, y);
    }

    public Vector2 ToAbsoluteVector2Safe()
    {
        if (X is Pixels x && Y is Pixels y)
        {
            return new(x.Value, y.Value);
        }
        throw new InvalidOperationException("Cannot convert to absolute vector2");
    }
}
