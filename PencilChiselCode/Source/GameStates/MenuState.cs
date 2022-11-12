using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;

namespace PencilChiselCode.Source.GameStates;

public class MenuState : GameScreen
{
    private Game1 _game => (Game1)Game;
    public static readonly Color BgColor = new(9F / 255F, 10F / 255F, 20F / 255F);
    private readonly List<Button> _buttons = new();
    private static Song _song;

    public MenuState(Game1 game) : base(game)
    {
    }

    public override void LoadContent()
    {
        base.LoadContent();
        var textureMap = Game1.Instance.TextureMap;
        var startButton = textureMap["start_button_normal"];
        var startButtonSize = new Vector2(startButton.Width, startButton.Height);
        _buttons.Add(new Button(startButton,
            textureMap["start_button_hover"],
            textureMap["start_button_pressed"],
            Utils.GetCenterStartCoords(startButtonSize, Game1.Instance.GetWindowDimensions()),
            () =>
            {
                Game1.Instance.ScreenManager.LoadScreen(new IngameState(_game),
                    new FadeTransition(Game1.Instance.GraphicsDevice, Color.Black));
            }
        ));
        var exitButton = textureMap["exit_button_normal"];
        var exitButtonSize = new Vector2(exitButton.Width, exitButton.Height);
        _buttons.Add(new Button(exitButton,
            textureMap["exit_button_hover"],
            textureMap["exit_button_pressed"],
            Utils.GetCenterStartCoords(exitButtonSize, Game1.Instance.GetWindowDimensions()) + Vector2.UnitY * 100,
            () => _game.Exit()
        ));
    }


    public override void Draw(GameTime gameTime)
    {
        Game1.Instance.SpriteBatch.Begin();
        Game1.Instance.GraphicsDevice.Clear(BgColor);
        foreach (var button in _buttons)
        {
            button.Draw(Game1.Instance.SpriteBatch);
        }

        Game1.Instance.SpriteBatch.End();
    }

    public override void Update(GameTime gameTime)
    {
        foreach (var button in _buttons)
        {
            button.Update(gameTime);
        }
    }
}