using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace PencilChiselCode.Source.GameStates;

public class MenuState : BonfireGameState
{
    private readonly List<Button> _buttons = new();

    public MenuState(Game game) : base(game)
    {
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
        Game.SpriteBatch.Begin();
        Game.GraphicsDevice.Clear(BgColor);
        foreach (var button in _buttons)
        {
            button.Draw(Game.SpriteBatch);
        }

        Game.SpriteBatch.End();
    }

    public override void Update(GameTime gameTime)
    {
        if (Game.Controls.JustPressed(ControlKeys.START))
        {
            Game.Start();
            return;
        }

        foreach (var button in _buttons)
        {
            button.Update(gameTime);
        }
    }
}