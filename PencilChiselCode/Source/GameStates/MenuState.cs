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
        var buttonBox = new RelativeBox(Game, 0, 0.75F)
        {
            BoxAlignment = Alignments.BottomCenter,
            SelfAlignment = Alignments.BottomCenter
        };
        var startButton = new Button(
            textureMap["start_button_normal"],
            textureMap["start_button_hover"],
            textureMap["start_button_pressed"],
            Game.SoundMap["button_press"],
            Game.SoundMap["button_release"],
            Game.Start
        );
        var settingsButton = new Button(
            textureMap["settings_button_normal"],
            textureMap["settings_button_hover"],
            textureMap["settings_button_pressed"],
            Game.SoundMap["button_press"],
            Game.SoundMap["button_release"],
            () => showSettings = true
        );
        var exitButton = new Button(
            textureMap["exit_button_normal"],
            textureMap["exit_button_hover"],
            textureMap["exit_button_pressed"],
            Game.SoundMap["button_press"],
            Game.SoundMap["button_release"],
            Game.Exit
        );
        buttonBox.AddChild(
            new RelativeBox(Game, 0, startButton.Size())
            {
                BoxAlignment = Alignments.MiddleCenter,
                SelfAlignment = Alignments.MiddleCenter,
                DrawableElement = startButton
            },
            new RelativeBox(Game, (0, 32), settingsButton.Size())
            {
                BoxAlignment = Alignments.BottomLeftOfPrevious,
                SelfAlignment = Alignments.TopLeft,
                DrawableElement = settingsButton
            },
            new RelativeBox(Game, (0, 32), exitButton.Size())
            {
                BoxAlignment = Alignments.BottomLeftOfPrevious,
                SelfAlignment = Alignments.TopLeft,
                DrawableElement = exitButton
            }
        );
        menuBox.AddChild(
            new RelativeBox(Game, (0, 120), logo.Size() * 2.5F)
            {
                BoxAlignment = Alignments.TopCenter,
                SelfAlignment = Alignments.MiddleCenter,
                DrawableElement = logo
            },
            buttonBox
        );
        var settingsBox = Menus.GetSettingsMenu(Game, () => showSettings = false);
        settingsBox.IsVisible = () => showSettings;
        RootBox.AddChild(menuBox, settingsBox);
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
