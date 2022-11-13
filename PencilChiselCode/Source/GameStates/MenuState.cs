using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;

namespace PencilChiselCode.Source.GameStates;

public class MenuState : GameScreen
{
    private Bonfire _game => (Bonfire)Game;
    public static readonly Color BgColor = new(9F / 255F, 10F / 255F, 20F / 255F);
    private readonly List<Button> _buttons = new();
    private static Song _song;

    public MenuState(Bonfire game) : base(game)
    {
    }

    public override void LoadContent()
    {
        base.LoadContent();
        var textureMap = Bonfire.Instance.TextureMap;
        var startButton = textureMap["start_button_normal"];
        var startButtonSize = new Vector2(startButton.Width, startButton.Height);
        _buttons.Add(new Button(startButton,
            textureMap["start_button_hover"],
            textureMap["start_button_pressed"],
            Utils.GetCenterStartCoords(startButtonSize, Bonfire.Instance.GetWindowDimensions()),
            () =>
            {
                Bonfire.Instance.ScreenManager.LoadScreen(new IngameState(_game),
                    new FadeTransition(Bonfire.Instance.GraphicsDevice, Color.Black));
            }
        ));
        var exitButton = textureMap["exit_button_normal"];
        var exitButtonSize = new Vector2(exitButton.Width, exitButton.Height);
        _buttons.Add(new Button(exitButton,
            textureMap["exit_button_hover"],
            textureMap["exit_button_pressed"],
            Utils.GetCenterStartCoords(exitButtonSize, Bonfire.Instance.GetWindowDimensions()) + Vector2.UnitY * 100,
            () => _game.Exit()
        ));
    }


    public override void Draw(GameTime gameTime)
    {
        Bonfire.Instance.SpriteBatch.Begin();
        Bonfire.Instance.GraphicsDevice.Clear(BgColor);
        foreach (var button in _buttons)
        {
            button.Draw(Bonfire.Instance.SpriteBatch);
        }

        Bonfire.Instance.SpriteBatch.End();
    }

    public override void Update(GameTime gameTime)
    {
        foreach (var button in _buttons)
        {
            button.Update(gameTime);
        }
    }
}