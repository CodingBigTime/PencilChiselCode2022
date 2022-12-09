using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PencilChiselCode.Source;

public class Controls
{
    public readonly HashSet<Keys> PreviousPressedKeys = new();
    public readonly HashSet<Buttons> PreviousPressedButtons = new();
    public static KeyboardState KeyState => Keyboard.GetState();
    public static GamePadState GamepadState => GamePad.GetState(PlayerIndex.One);
    public readonly Dictionary<ControlKeys, Keys> KeyBindings = new();
    public readonly Dictionary<ControlKeys, Buttons> ControllerBindings = new();

    public void Update()
    {
        PreviousPressedKeys.Clear();
        PreviousPressedButtons.Clear();
        PreviousPressedKeys.UnionWith(KeyState.GetPressedKeys());
        var buttons = Enum.GetValues(typeof(Buttons)).Cast<Buttons>();
        buttons
            .Where(button => GamepadState.IsButtonDown(button))
            .ToList()
            .ForEach(button => PreviousPressedButtons.Add(button));
    }

    public Controls()
    {
        KeyBindings.Add(ControlKeys.MOVE_UP, Keys.W);
        KeyBindings.Add(ControlKeys.MOVE_DOWN, Keys.S);
        KeyBindings.Add(ControlKeys.MOVE_LEFT, Keys.A);
        KeyBindings.Add(ControlKeys.MOVE_RIGHT, Keys.D);
        KeyBindings.Add(ControlKeys.STOP_FOLLOWER, Keys.Space);
        KeyBindings.Add(ControlKeys.PAUSE, Keys.Escape);
        KeyBindings.Add(ControlKeys.DEBUG, Keys.F3);
        KeyBindings.Add(ControlKeys.FEED, Keys.Q);
        KeyBindings.Add(ControlKeys.COLLECT, Keys.E);
        KeyBindings.Add(ControlKeys.REFUEL, Keys.F);
        KeyBindings.Add(ControlKeys.START_FIRE, Keys.X);
        KeyBindings.Add(ControlKeys.START, Keys.Space);

        ControllerBindings.Add(ControlKeys.STOP_FOLLOWER, Buttons.LeftShoulder);
        ControllerBindings.Add(ControlKeys.PAUSE, Buttons.Start);
        ControllerBindings.Add(ControlKeys.FEED, Buttons.X);
        ControllerBindings.Add(ControlKeys.COLLECT, Buttons.A);
        ControllerBindings.Add(ControlKeys.REFUEL, Buttons.B);
        ControllerBindings.Add(ControlKeys.START_FIRE, Buttons.Y);
        ControllerBindings.Add(ControlKeys.START, Buttons.A);
    }

    public Vector2 GetMovement()
    {
        var left = IsPressed(ControlKeys.MOVE_LEFT);
        var up = IsPressed(ControlKeys.MOVE_UP);
        var down = IsPressed(ControlKeys.MOVE_DOWN);
        var right = IsPressed(ControlKeys.MOVE_RIGHT);
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
