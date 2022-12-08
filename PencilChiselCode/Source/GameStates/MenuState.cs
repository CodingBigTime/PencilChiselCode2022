using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PencilChiselCode.Source.GameStates;

public class MenuState : BonfireGameState
{
    private readonly List<Button> _buttons = new();
    private readonly Texture2D _logo;
    private Vector2 _logoPosition;
    private const float LogoScale = 2.5F;

    public MenuState(Game game) : base(game)
    {
        _logo = Game.TextureMap["logo"];
        _logoPosition = new Vector2((Game.GetWindowWidth() - _logo.Width * LogoScale) / 2F, 100);
        BgColor = new Color(9F / 255F, 10F / 255F, 20F / 255F);
    }

    public override void LoadContent()
    {
        base.LoadContent();
        var textureMap = Game.TextureMap;
        var startButton = textureMap["start_button_normal"];
        var startButtonSize = new Vector2(startButton.Width, startButton.Height);
        _buttons.Add(new Button(this,
            startButton,
            textureMap["start_button_hover"],
            textureMap["start_button_pressed"],
            Utils.GetCenterStartCoords(startButtonSize, Game.GetWindowDimensions()),
            Game.Start
        ));
        var exitButton = textureMap["exit_button_normal"];
        var exitButtonSize = new Vector2(exitButton.Width, exitButton.Height);
        _buttons.Add(new Button(this,
            exitButton,
            textureMap["exit_button_hover"],
            textureMap["exit_button_pressed"],
            Utils.GetCenterStartCoords(exitButtonSize, Game.GetWindowDimensions()) + Vector2.UnitY * 100,
            Game.Exit
        ));
    }


    public override void Draw(GameTime gameTime)
    {
        Game.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        Game.GraphicsDevice.Clear(BgColor);
        Game.SpriteBatch.Draw(_logo, _logoPosition, null, Color.White, 0F, Vector2.Zero, LogoScale, SpriteEffects.None, 0F);
        _buttons.ForEach(button => button.Draw(Game.SpriteBatch));
        Game.SpriteBatch.End();
    }

    public override void Update(GameTime gameTime)
    {
        if (Game.Controls.JustPressed(ControlKeys.START))
        {
            Game.Start();
            return;
        }

        _buttons.ForEach(button => button.Update(gameTime));
    }
}