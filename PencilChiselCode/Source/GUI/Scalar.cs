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

    public static Percent operator /(Pixels pixels1, Pixels pixels2) =>
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
}

public record Percent : Scalar
{
    public float Value { get; }

    public Percent(float Value) => this.Value = Value;

    public static implicit operator Percent(float value) => new(value);
}

public record Ratio : Scalar
{
    public float Value { get; }

    public Ratio(float Value) => this.Value = Value;
}

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

    public ScalarVector2(int X, Ratio Y) : this(new Pixels(X), Y) { }

    public ScalarVector2((int X, Ratio Y) value) : this(value.X, value.Y) { }

    public ScalarVector2(Ratio X, int Y) : this(X, new Pixels(Y)) { }

    public ScalarVector2((Ratio X, int Y) value) : this(value.X, value.Y) { }

    public ScalarVector2(float X, Ratio Y) : this(new Percent(X), Y) { }

    public ScalarVector2((float X, Ratio Y) value) : this(value.X, value.Y) { }

    public ScalarVector2(Ratio X, float Y) : this(X, new Percent(Y)) { }

    public ScalarVector2((Ratio X, float Y) value) : this(value.X, value.Y) { }

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

    public static implicit operator ScalarVector2((int, Ratio) tuple) => new(tuple);

    public static implicit operator ScalarVector2((Ratio, int) tuple) => new(tuple);

    public static implicit operator ScalarVector2((float, Ratio) tuple) => new(tuple);

    public static implicit operator ScalarVector2((Ratio, float) tuple) => new(tuple);

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

    public ScalarVector2 RelativeTo(ScalarVector2 parent)
    {
        if (!(parent.X is Pixels && parent.Y is Pixels))
        {
            throw new InvalidOperationException("Parent must be in pixels");
        }
        var parentX = (Pixels)parent.X;
        var parentY = (Pixels)parent.Y;
        return new(
            this switch
            {
                { X: Pixels x, Y: Pixels y } => (x, y),
                { X: Percent x, Y: Percent y } => (parentX * x, parentY * y),
                { X: Percent x, Y: Pixels y } => (parentX * x, y),
                { X: Pixels x, Y: Percent y } => (x, parentY * y),
                { X: Ratio x, Y: Pixels y } => (y * x, y),
                { X: Pixels x, Y: Ratio y } => (x, x * y),
                { X: Ratio x, Y: Percent y } => (y * parentY * x, y * parentY),
                { X: Percent x, Y: Ratio y } => (x * parentX, x * parentX * y),
                { X: Ratio _, Y: Ratio _ }
                    => throw new InvalidOperationException("Cannot have 2 ratios"),
                _ => throw new NotImplementedException()
            }
        );
    }

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
