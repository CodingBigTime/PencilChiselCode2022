using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using PencilChiselCode.Source.GameStates;

namespace PencilChiselCode.Source;

public class MenuState : GameScreen
{
    private new Game1 Game => (Game1)base.Game;
    private static readonly Color BgColor = new(9F / 255F, 10F / 255F, 20F / 255F);
    private readonly List<Button> _buttons = new();
    private static Song _song;
    private Dictionary<string, Texture2D> _texture;

    public MenuState(Game1 game) : base(game)
    {
    }

    public override void LoadContent()
    {
        base.LoadContent();
        _texture = Game1.Instance.TextureMap;
        _buttons.Add(new Button(_texture["start_button_normal"],
            _texture["start_button_hover"],
            _texture["start_button_pressed"],
            () =>
            {
                Game1.Instance._screenManager.LoadScreen(new IngameState(Game),
                    new FadeTransition(Game1.Instance.GraphicsDevice, Color.Khaki));
            }
        ));
    }


    public override void Draw(GameTime gameTime)
    {
        Debug.WriteLine("3");
        Game1.Instance._spriteBatch.Begin();
        Game1.Instance.GraphicsDevice.Clear(BgColor);
        foreach (var button in _buttons)
        {
            button.Draw(Game1.Instance._spriteBatch);
        }

        Game1.Instance._spriteBatch.End();
    }

    public override void Update(GameTime gameTime)
    {
        foreach (var button in _buttons)
        {
            button.Update(gameTime);
        }
    }
}