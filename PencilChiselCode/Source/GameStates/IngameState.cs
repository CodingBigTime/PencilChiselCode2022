using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;

namespace PencilChiselCode.Source;

public class IngameState : GameScreen
{

    private static readonly Color BgColor = Color.Green;
    private new Game1 Game => (Game1)base.Game;
    public IngameState(Game1 game) : base(game) { }
    
    
    public override void LoadContent()
    {
        base.LoadContent();
    }

    public override void Update(GameTime gameTime)
    {
        
    }

    public override void Draw(GameTime gameTime)
    {
        Game1.Instance._spriteBatch.Begin();
        Game1.Instance.GraphicsDevice.Clear(BgColor);
        Game1.Instance._spriteBatch.End();
    }
    
    
}