using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Tweening;
using PencilChiselCode.Source.GameStates;

namespace PencilChiselCode.Source;

public class PopupButton
{
    private readonly Tweener _scaleTweener;
    private readonly Tweener _rotationTweener;
    public float scale = 0.75F;
    public float rotation = -0.33F;
    public Texture2D Texture =>
        Game.Controls.WasControllerUsed ? ControllerTexture : KeyboardTexture;
    public Texture2D ControllerTexture;
    public Texture2D KeyboardTexture;
    private Vector2 Size => new Vector2(Texture.Width, Texture.Height);
    private readonly BonfireGameState _state;
    private Bonfire Game => _state.Game;

    public PopupButton(BonfireGameState state, Texture2D texture) : this(state, texture, texture)
    { }

    public PopupButton(
        BonfireGameState state,
        Texture2D keyboardTexture,
        Texture2D controllerTexture
    )
    {
        _state = state;
        KeyboardTexture = keyboardTexture;
        ControllerTexture = controllerTexture;
        _scaleTweener = new Tweener();
        _scaleTweener
            .TweenTo(
                target: this,
                expression: button => button.scale,
                toValue: 1.25F,
                duration: 1.5F,
                delay: 0F
            )
            .RepeatForever(repeatDelay: 0.2f)
            .AutoReverse()
            .Easing(EasingFunctions.SineInOut);
        _rotationTweener = new Tweener();
        _rotationTweener
            .TweenTo(
                target: this,
                expression: button => button.rotation,
                toValue: 0.33F,
                duration: 3F,
                delay: 0F
            )
            .RepeatForever(repeatDelay: 0.2f)
            .AutoReverse()
            .Easing(EasingFunctions.SineInOut);
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position, Vector2 size)
    {
        spriteBatch.Draw(
            texture: Texture,
            position: position + new Vector2(size.X, -size.Y * 1.75F),
            sourceRectangle: null,
            color: Color.White,
            rotation: rotation,
            origin: Size * scale * 0.5F,
            scale: 2F * scale,
            effects: SpriteEffects.None,
            layerDepth: 0
        );
    }

    public void Update(GameTime gameTime)
    {
        _scaleTweener.Update(gameTime.GetElapsedSeconds());
        _rotationTweener.Update(gameTime.GetElapsedSeconds());
    }
}
