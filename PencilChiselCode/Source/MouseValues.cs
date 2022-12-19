using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PencilChiselCode.Source;

public class MouseValues
{
    public Vector2 PreviousPosition { get; set; }
    public Vector2 CurrentPosition { get; set; }
    public MouseState PreviousState { get; set; }
    public MouseState CurrentState { get; set; }

    public bool JustEntered(Rectangle region) =>
        region.Contains(CurrentPosition) && !region.Contains(PreviousPosition);

    public bool JustExited(Rectangle region) =>
        !region.Contains(CurrentPosition) && region.Contains(PreviousPosition);

    public bool JustPressed(MouseButton button) =>
        button switch
        {
            MouseButton.Left
                => CurrentState.LeftButton == ButtonState.Pressed
                    && PreviousState.LeftButton == ButtonState.Released,
            MouseButton.Middle
                => CurrentState.MiddleButton == ButtonState.Pressed
                    && PreviousState.MiddleButton == ButtonState.Released,
            MouseButton.Right
                => CurrentState.RightButton == ButtonState.Pressed
                    && PreviousState.RightButton == ButtonState.Released,
            MouseButton.XButton1
                => CurrentState.XButton1 == ButtonState.Pressed
                    && PreviousState.XButton1 == ButtonState.Released,
            MouseButton.XButton2
                => CurrentState.XButton2 == ButtonState.Pressed
                    && PreviousState.XButton2 == ButtonState.Released,
            _ => false
        };

    public bool JustReleased(MouseButton button) =>
        button switch
        {
            MouseButton.Left
                => CurrentState.LeftButton == ButtonState.Released
                    && PreviousState.LeftButton == ButtonState.Pressed,
            MouseButton.Middle
                => CurrentState.MiddleButton == ButtonState.Released
                    && PreviousState.MiddleButton == ButtonState.Pressed,
            MouseButton.Right
                => CurrentState.RightButton == ButtonState.Released
                    && PreviousState.RightButton == ButtonState.Pressed,
            MouseButton.XButton1
                => CurrentState.XButton1 == ButtonState.Released
                    && PreviousState.XButton1 == ButtonState.Pressed,
            MouseButton.XButton2
                => CurrentState.XButton2 == ButtonState.Released
                    && PreviousState.XButton2 == ButtonState.Pressed,
            _ => false
        };

    public void Update()
    {
        PreviousPosition = CurrentPosition;
        PreviousState = CurrentState;
        CurrentState = Mouse.GetState();
        CurrentPosition = new Vector2(CurrentState.X, CurrentState.Y);
    }
}

public enum MouseButton
{
    Left,
    Middle,
    Right,
    XButton1,
    XButton2
}
