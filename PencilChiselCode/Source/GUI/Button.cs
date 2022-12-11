using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PencilChiselCode.Source.GameStates;

namespace PencilChiselCode.Source.GUI;

public class Button : IUiElement
{
    public Texture2D Texture { get; set; }
    public Vector2 Size() => new(Texture.Width, Texture.Height);
    private readonly Texture2D _normalTexture;
    private readonly Texture2D _hoveredTexture;
    private readonly Texture2D _pressedTexture;
    private readonly Action _action;
    private readonly BonfireGameState _state;
    private Bonfire Game => _state.Game;

    public Button(
        BonfireGameState state,
        Texture2D normal,
        Texture2D hovered,
        Texture2D pressed,
        Action action
    )
    {
        _state = state;
        _normalTexture = normal;
        _hoveredTexture = hovered;
        _pressedTexture = pressed;
        Texture = normal;
        _action = action;
    }

    public void Draw(SpriteBatch spriteBatch, Box parent) =>
        spriteBatch.Draw(
            Texture,
            parent.Position,
            null,
            Color.White,
            0F,
            Vector2.Zero,
            parent.Scale,
            SpriteEffects.None,
            0F
        );

    public void Update(GameTime gameTime, Box parent)
    {
        var mouseState = Mouse.GetState();
        var inside = Utils.IsPointInRectangle(
            mouseState.Position.ToVector2(),
            new Rectangle(parent.Position.ToPoint(), parent.Size().ToPoint())
        );
        var released = mouseState.LeftButton == ButtonState.Released;

        var pressSound = Game.SoundMap["button_press"];
        var releaseSound = Game.SoundMap["button_release"];

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
