using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace PencilChiselCode.Source;

public class Controls
{
    public readonly HashSet<Keys> PreviousPressedKeys = new();
    public readonly HashSet<Buttons> PreviousPressedButtons = new();
    public static KeyboardState KeyState => Keyboard.GetState();
    public static GamePadState GamepadState => GamePad.GetState(PlayerIndex.One);
    public readonly Dictionary<ControlKeys, Keys> KeyBindings = new();
    public readonly Dictionary<ControlKeys, Buttons> ControllerBindings = new();
    public readonly Dictionary<Keys, double> KeyHoldDuration = new();
    public readonly Dictionary<Buttons, double> ButtonHoldDuration = new();
    private bool _wasControllerUsed;

    public bool WasControllerUsed
    {
        get => _wasControllerUsed;
        private set
        {
            _wasControllerUsed = value;
            OnControllerTypeChanged?.Invoke(this, _wasControllerUsed);
        }
    }

    public event EventHandler<bool> OnControllerTypeChanged;

    public Controls()
    {
        KeyBindings.Add(ControlKeys.MoveUp, Keys.W);
        KeyBindings.Add(ControlKeys.MoveDown, Keys.S);
        KeyBindings.Add(ControlKeys.MoveLeft, Keys.A);
        KeyBindings.Add(ControlKeys.MoveRight, Keys.D);
        KeyBindings.Add(ControlKeys.StopFollower, Keys.Space);
        KeyBindings.Add(ControlKeys.Pause, Keys.Escape);
        KeyBindings.Add(ControlKeys.Debug, Keys.F3);
        KeyBindings.Add(ControlKeys.Feed, Keys.Q);
        KeyBindings.Add(ControlKeys.Collect, Keys.E);
        KeyBindings.Add(ControlKeys.Refuel, Keys.F);
        KeyBindings.Add(ControlKeys.StartFire, Keys.X);
        KeyBindings.Add(ControlKeys.Start, Keys.Space);
        KeyBindings.Add(ControlKeys.SpeedupDebug, Keys.LeftShift);

        ControllerBindings.Add(ControlKeys.StopFollower, Buttons.LeftShoulder);
        ControllerBindings.Add(ControlKeys.Pause, Buttons.Start);
        ControllerBindings.Add(ControlKeys.Feed, Buttons.X);
        ControllerBindings.Add(ControlKeys.Collect, Buttons.A);
        ControllerBindings.Add(ControlKeys.Refuel, Buttons.B);
        ControllerBindings.Add(ControlKeys.StartFire, Buttons.Y);
        ControllerBindings.Add(ControlKeys.Start, Buttons.A);
    }

    public void Update(GameTime gameTime)
    {
        PreviousPressedKeys.Clear();
        PreviousPressedButtons.Clear();
        PreviousPressedKeys.UnionWith(KeyState.GetPressedKeys());
        var buttons = Enum.GetValues(typeof(Buttons)).Cast<Buttons>();
        buttons
            .Where(button => GamepadState.IsButtonDown(button))
            .ToList()
            .ForEach(button => PreviousPressedButtons.Add(button));
        foreach (var key in KeyHoldDuration.Keys)
        {
            if (PreviousPressedKeys.Contains(key))
            {
                KeyHoldDuration[key] += gameTime.GetElapsedSeconds();
            }
            else
            {
                KeyHoldDuration.Remove(key);
            }
        }

        foreach (var key in PreviousPressedKeys.Where(key => !KeyHoldDuration.ContainsKey(key)))
        {
            KeyHoldDuration[key] = 0;
        }

        foreach (var button in ButtonHoldDuration.Keys)
        {
            if (PreviousPressedButtons.Contains(button))
            {
                ButtonHoldDuration[button] += gameTime.GetElapsedSeconds();
            }
            else
            {
                ButtonHoldDuration.Remove(button);
            }
        }

        foreach (
            var button in PreviousPressedButtons.Where(
                button => !ButtonHoldDuration.ContainsKey(button)
            )
        )
        {
            ButtonHoldDuration[button] = 0;
        }

        if (PreviousPressedButtons.Any())
        {
            WasControllerUsed = true;
        }
        else if (PreviousPressedKeys.Any())
        {
            WasControllerUsed = false;
        }
    }

    public double GetHoldDuration(ControlKeys key)
    {
        if (!IsPressed(key))
            return 0;

        if (KeyBindings.ContainsKey(key) && KeyHoldDuration.ContainsKey(KeyBindings[key]))
        {
            return KeyHoldDuration[KeyBindings[key]];
        }

        if (
            ControllerBindings.ContainsKey(key)
            && ButtonHoldDuration.ContainsKey(ControllerBindings[key])
        )
        {
            return ButtonHoldDuration[ControllerBindings[key]];
        }

        return 0D;
    }

    public Vector2 GetMovement()
    {
        var left = IsPressed(ControlKeys.MoveLeft);
        var up = IsPressed(ControlKeys.MoveUp);
        var down = IsPressed(ControlKeys.MoveDown);
        var right = IsPressed(ControlKeys.MoveRight);
        var mx =
            GamepadState.ThumbSticks.Left.X != 0
                ? GamepadState.ThumbSticks.Left.X
                : Convert.ToSingle(right) - Convert.ToSingle(left);
        var my =
            GamepadState.ThumbSticks.Left.Y != 0
                ? -GamepadState.ThumbSticks.Left.Y
                : Convert.ToSingle(down) - Convert.ToSingle(up);
        return new Vector2(mx, my);
    }

    public bool IsPressed(ControlKeys key)
    {
        return KeyBindings.ContainsKey(key) && KeyState.IsKeyDown(KeyBindings[key])
            || ControllerBindings.ContainsKey(key)
                && GamepadState.IsButtonDown(ControllerBindings[key]);
    }

    public bool JustPressed(ControlKeys key)
    {
        return (
                KeyBindings.ContainsKey(key)
                && !PreviousPressedKeys.Contains(KeyBindings[key])
                && KeyState.IsKeyDown(KeyBindings[key])
            )
            || (
                ControllerBindings.ContainsKey(key)
                && !PreviousPressedButtons.Contains(ControllerBindings[key])
                && GamepadState.IsButtonDown(ControllerBindings[key])
            );
    }

    public bool JustReleased(ControlKeys key)
    {
        return (
                KeyBindings.ContainsKey(key)
                && PreviousPressedKeys.Contains(KeyBindings[key])
                && !KeyState.IsKeyDown(KeyBindings[key])
            )
            || (
                ControllerBindings.ContainsKey(key)
                && PreviousPressedButtons.Contains(ControllerBindings[key])
                && !GamepadState.IsButtonDown(ControllerBindings[key])
            );
    }
}
