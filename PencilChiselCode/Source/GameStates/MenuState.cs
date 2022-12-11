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
        var menuBox = new Box(Game, new Vector2(0F), new Vector2(0.75F))
        {
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
        var startButton = new Button(
            this,
            textureMap["start_button_normal"],
            textureMap["start_button_hover"],
            textureMap["start_button_pressed"],
            Game.Start
        );
        var buttonBox = new Box(Game, new Vector2(0F), new Vector2(0.75F))
        {
            IsPositionAbsolute = true,
            BoxAlignment = Alignments.BottomCenter,
            SelfAlignment = Alignments.BottomCenter
        };
        buttonBox.AddChild(
            new Box(Game, new Vector2(0), startButton)
            {
                IsSizeAbsolute = true,
                IsPositionAbsolute = true,
                BoxAlignment = Alignments.MiddleCenter,
                SelfAlignment = Alignments.MiddleCenter
            }
        );
        var exitButton = new Button(
            this,
            textureMap["exit_button_normal"],
            textureMap["exit_button_hover"],
            textureMap["exit_button_pressed"],
            Game.Exit
        );
        buttonBox.AddChild(
            new Box(Game, new Vector2(0F, -85F), exitButton)
            {
                IsSizeAbsolute = true,
                IsPositionAbsolute = true,
                BoxAlignment = Alignments.MiddleCenter,
                SelfAlignment = Alignments.MiddleCenter
            }
        );
        menuBox.AddChild(buttonBox);
        RootBox.AddChild(menuBox);
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
