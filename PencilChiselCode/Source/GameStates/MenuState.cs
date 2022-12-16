using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PencilChiselCode.Source.GUI;

namespace PencilChiselCode.Source.GameStates;

public class MenuState : BonfireGameState
{
    public Box RootBox;

    public MenuState(Game game) : base(game) =>
        BgColor = new Color(9F / 255F, 10F / 255F, 20F / 255F);

    public override void LoadContent()
    {
        base.LoadContent();
        var textureMap = Game.TextureMap;
        RootBox = Game.GetRootBox();
        var logo = new UiTextureElement(textureMap["logo"]);
        var showSettings = false;
        var menuBox = new Box(Game, new Vector2(0F), new Vector2(0.75F))
        {
            IsVisible = () => !showSettings,
            IsPositionAbsolute = true,
            BoxAlignment = Alignments.MiddleCenter,
            SelfAlignment = Alignments.MiddleCenter
        };
        menuBox.AddChild(
            new Box(Game, new Vector2(0F, 120F), logo)
            {
                IsSizeAbsolute = true,
                IsPositionAbsolute = true,
                BoxAlignment = Alignments.TopCenter,
                SelfAlignment = Alignments.MiddleCenter,
                Scale = new(2.5F)
            }
        );
        var buttonBox = new Box(Game, new Vector2(0F), new Vector2(0.75F))
        {
            IsPositionAbsolute = true,
            BoxAlignment = Alignments.BottomCenter,
            SelfAlignment = Alignments.BottomCenter
        };
        var startButton = new TexturedButton(
            textureMap["start_button_normal"],
            textureMap["start_button_hover"],
            textureMap["start_button_pressed"],
            Game.SoundMap["button_press"],
            Game.SoundMap["button_release"],
            Game.Start
        );
        buttonBox.AddChild(
            new Box(Game, new Vector2(0), startButton)
            {
                IsSizeAbsolute = true,
                IsPositionAbsolute = true,
                BoxAlignment = Alignments.MiddleCenter,
                SelfAlignment = Alignments.MiddleCenter
            }
        );
        var settingsButton = new TexturedButton(
            textureMap["settings_button_normal"],
            textureMap["settings_button_hover"],
            textureMap["settings_button_pressed"],
            Game.SoundMap["button_press"],
            Game.SoundMap["button_release"],
            () => showSettings = true
        );
        buttonBox.AddChild(
            new Box(Game, new Vector2(0F, -85F), settingsButton)
            {
                IsSizeAbsolute = true,
                IsPositionAbsolute = true,
                BoxAlignment = Alignments.MiddleCenter,
                SelfAlignment = Alignments.MiddleCenter
            }
        );
        var exitButton = new TexturedButton(
            textureMap["exit_button_normal"],
            textureMap["exit_button_hover"],
            textureMap["exit_button_pressed"],
            Game.SoundMap["button_press"],
            Game.SoundMap["button_release"],
            Game.Exit
        );
        buttonBox.AddChild(
            new Box(Game, new Vector2(0F, -170F), exitButton)
            {
                IsSizeAbsolute = true,
                IsPositionAbsolute = true,
                BoxAlignment = Alignments.MiddleCenter,
                SelfAlignment = Alignments.MiddleCenter
            }
        );
        menuBox.AddChild(buttonBox);
        RootBox.AddChild(menuBox);
        var settingsBox = Menus.GetSettingsMenu(Game, () => showSettings = false);
        settingsBox.IsVisible = () => showSettings;
        RootBox.AddChild(settingsBox);
    }

    public override void Draw(GameTime gameTime)
    {
        Game.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        Game.GraphicsDevice.Clear(BgColor);
        RootBox.Draw(Game.SpriteBatch);
        Game.SpriteBatch.End();
    }

    public override void Update(GameTime gameTime)
    {
        if (Game.Controls.JustPressed(ControlKeys.START))
        {
            Game.Start();
            return;
        }

        RootBox.Update(gameTime);
    }
}
