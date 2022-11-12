using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PencilChiselCode.Source;

public class Button
{
    public Vector2 Position;
    public Vector2 Size => new(Texture.Width, Texture.Height);
    public Texture2D Texture { get; set; }
    private readonly Texture2D _normalTexture;
    private readonly Texture2D _hoveredTexture;
    private readonly Texture2D _pressedTexture;
    private readonly Action _action;

    public Button(Texture2D normal, Texture2D hovered, Texture2D pressed,Action action)
    {
        _normalTexture = normal;
        _hoveredTexture = hovered;
        _pressedTexture = pressed;
        Texture = normal;
        Position = Utils.GetCenterStartCoords(Size, Game1.Instance.GetWindowDimensions());
        _action = action;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Texture, Position, Color.White);
    }

    public void Update(GameTime gameTime)
    {
        var mouseState = Mouse.GetState();
        var inside = Utils.IsPointInRectangle(mouseState.Position.ToVector2(),
            new Rectangle(Position.ToPoint(), Size.ToPoint()));
        var released = mouseState.LeftButton == ButtonState.Released;

        var pressSound = Game1.Instance.SoundMap["button_press"];
        var releaseSound = Game1.Instance.SoundMap["button_release"];

        if (inside)
        {
            if (released)
            {
                if (Texture == _pressedTexture)
                {
                    releaseSound.Play();
                    Click();
                }

                Texture = _hoveredTexture;
            }
            else if (Texture == _hoveredTexture)
            {
                pressSound.Play();
                Texture = _pressedTexture;
            }
        }
        else
        {
            if (Texture == _pressedTexture)
            {
                releaseSound.Play();
            }

            Texture = _normalTexture;
        }
    }

    private void Click()
    {
        _action.Invoke();
    }
}