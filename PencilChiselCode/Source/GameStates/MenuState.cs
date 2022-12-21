using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PencilChiselCode.Source.GUI;

namespace PencilChiselCode.Source.GameStates;

public class MenuState : BonfireGameState
{
    public RootBox RootBox;

    public MenuState(Game game) : base(game) =>
        BgColor = new Color(9F / 255F, 10F / 255F, 20F / 255F);

    public override void LoadContent()
    {
        base.LoadContent();
        var textureMap = Game.TextureMap;
        RootBox = Game.GetRootBox();
        var logo = new UiTextureElement(textureMap["logo"]);
        var showSettings = false;
        var menuBox = new RelativeBox(Game, 0, 0.75F)
        {
            BoxAlignment = Alignments.MiddleCenter,
            SelfAlignment = Alignments.MiddleCenter,
            IsVisible = () => !showSettings
        };
        menuBox.AddChild(
            new RelativeBox(Game, (0, 120), logo.Size() * 2.5F)
            {
                BoxAlignment = Alignments.TopCenter,
                SelfAlignment = Alignments.MiddleCenter,
                DrawableElement = logo
            }
        );
        var buttonBox = new RelativeBox(Game, 0, 0.75F)
        {
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
            new RelativeBox(Game, 0, startButton.Size())
            {
                BoxAlignment = Alignments.MiddleCenter,
                SelfAlignment = Alignments.MiddleCenter,
                DrawableElement = startButton
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
            new RelativeBox(Game, (0, 85), settingsButton.Size())
            {
                BoxAlignment = Alignments.MiddleCenter,
                SelfAlignment = Alignments.MiddleCenter,
                DrawableElement = settingsButton
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
            new RelativeBox(Game, (0, 170), exitButton.Size())
            {
                BoxAlignment = Alignments.MiddleCenter,
                SelfAlignment = Alignments.MiddleCenter,
                DrawableElement = exitButton
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
        if (Game.Controls.JustPressed(ControlKeys.Start))
        {
            Game.Start();
            return;
        }

        RootBox.Update(gameTime);
    }
}
