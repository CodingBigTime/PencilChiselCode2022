using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PencilChiselCode.Source.GameStates;

namespace PencilChiselCode.Source;

public class Inventory
{
    private readonly Dictionary<string, TextIcon> _textIcons;
    private readonly Dictionary<string, Icon> _icons;
    private readonly IngameState _state;
    private Bonfire Game => _state.Game;

    public Inventory(IngameState state)
    {
        _state = state;
        var font = Game.FontMap["32"];
        _textIcons = new Dictionary<string, TextIcon>
        {
            { "twigCount", new TextIcon("0", new Vector2(Game.Width - 175, 50), font) },
            { "berryCount", new TextIcon("0", new Vector2(Game.Width - 175, 100), font) },
            { "campfireTwigCost", new TextIcon("10 x", new Vector2(Game.Width - 275, 150), font) },
            { "campfireEquals", new TextIcon("=", new Vector2(Game.Width - 150, 150), font) },
            { "campfireRefuel2x", new TextIcon("2 x", new Vector2(Game.Width - 260, 200), font) },
            { "campfireRefuelPlus", new TextIcon("+", new Vector2(Game.Width - 150, 200), font) },
            { "followerFeedPlus", new TextIcon("+", new Vector2(Game.Width - 150, 250), font) },
            { "followStop", new TextIcon("follow/stop", new Vector2(Game.Width - 300, 300), font) }
        };
        _icons = new Dictionary<string, Icon>
        {
            { "twigIcon", new Icon(new Vector2(Game.Width - 120, 50), Game.TextureMap["twig"]) },
            {
                "twigEButton",
                new Icon(new Vector2(Game.Width - 60, 50), Game.TextureMap["e_key"], 2F)
            },
            {
                "berryIcon",
                new Icon(new Vector2(Game.Width - 120, 100), Game.TextureMap["berry"], 2F)
            },
            {
                "berryEButton",
                new Icon(new Vector2(Game.Width - 60, 100), Game.TextureMap["e_key"], 2F)
            },
            {
                "campfireCostTwigIcon",
                new Icon(new Vector2(Game.Width - 200, 150), Game.TextureMap["twig"])
            },
            {
                "campfireCostIcon",
                new Icon(new Vector2(Game.Width - 120, 150), Game.TextureMap["campfire"])
            },
            {
                "camfireXButton",
                new Icon(new Vector2(Game.Width - 60, 150), Game.TextureMap["x_key"], 2F)
            },
            {
                "campfireRefuelTwigIcon",
                new Icon(new Vector2(Game.Width - 200, 200), Game.TextureMap["twig"])
            },
            {
                "campfireRefuelIcon",
                new Icon(new Vector2(Game.Width - 120, 200), Game.TextureMap["campfire"])
            },
            {
                "camfireRefuelFButton",
                new Icon(new Vector2(Game.Width - 60, 200), Game.TextureMap["f_key"], 2F)
            },
            {
                "followerFeedBerry",
                new Icon(new Vector2(Game.Width - 200, 250), Game.TextureMap["berry"], 2F)
            },
            {
                "followerFeedIcon",
                new Icon(new Vector2(Game.Width - 120, 250), Game.TextureMap["follower"])
            },
            {
                "followerFeedFButton",
                new Icon(new Vector2(Game.Width - 60, 250), Game.TextureMap["q_key"], 2F)
            },
            {
                "followerFollowStand",
                new Icon(new Vector2(Game.Width - 120, 300), Game.TextureMap["follower"])
            },
            {
                "followerFollowStandSpace",
                new Icon(new Vector2(Game.Width - 75, 300), Game.TextureMap["space_key"], 2)
            }
        };
    }

    public void Update()
    {
        _textIcons["twigCount"].Text = _state.Player.Inventory[PickupableTypes.Twig].ToString();
        _textIcons["berryCount"].Text = _state.Player.Inventory[
            PickupableTypes.BerryBush
        ].ToString();
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _textIcons.Values.ToList().ForEach(textIcon => textIcon.Draw(spriteBatch));
        _icons.Values.ToList().ForEach(icon => icon.Draw(spriteBatch));
    }
}
