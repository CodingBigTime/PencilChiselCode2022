using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PencilChiselCode.Source.GameStates;

namespace PencilChiselCode.Source
{
    public class Inventory
    {
        private Dictionary<string, TextIcon> _textIcons;

        private Dictionary<string, Icon> _icons;

        private Game1 _game;

        private Player _player;

        public Inventory(Game1 game, Player player)
        {
            _game = game;
            _player = player;
            var font = _game.FontMap["32"];
            _textIcons =
                new Dictionary<string, TextIcon> {
                    {
                        "twigCount",
                        new TextIcon("0",
                            new Vector2(game.Width - 150, 50),
                            font)
                    },
                    {
                        "berryCount",
                        new TextIcon("0",
                            new Vector2(game.Width - 150, 100),
                            font)
                    },
                    {
                        "campfireTwigCost",
                        new TextIcon("10 x",
                            new Vector2(game.Width - 275, 150),
                            font)
                    },
                    {
                        "campfireEquals",
                        new TextIcon("=",
                            new Vector2(game.Width - 150, 150),
                            font)
                    },
                    {
                        "campfireRefuel2x",
                        new TextIcon("2 x",
                            new Vector2(game.Width - 260, 200),
                            font)
                    },
                    {
                        "campfireRefuelPlus",
                        new TextIcon("+",
                            new Vector2(game.Width - 150, 200),
                            font)
                    },
                    {
                        "followerFeedPlus",
                        new TextIcon("+",
                            new Vector2(game.Width - 150, 250),
                            font)
                    }
                };
            _icons =
                new Dictionary<string, Icon> {
                    {
                        "twigIcon",
                        new Icon(new Vector2(game.Width - 120, 50),
                            _game.TextureMap["twigs"])
                    },
                    {
                        "twigEButton",
                        new Icon(new Vector2(game.Width - 60, 50),
                            _game.TextureMap["e_button"],
                            2F)
                    },
                    {
                        "berryIcon",
                        new Icon(new Vector2(game.Width - 120, 100),
                            _game.TextureMap["twigs"]) // TODO: Change to berries texture
                    },
                    {
                        "berryEButton",
                        new Icon(new Vector2(game.Width - 60, 100),
                            _game.TextureMap["e_button"],
                            2F)
                    },
                    {
                        "campfireCostTwigIcon",
                        new Icon(new Vector2(game.Width - 200, 150),
                            _game.TextureMap["twigs"])
                    },
                    {
                        "campfireCostIcon",
                        new Icon(new Vector2(game.Width - 120, 150),
                            _game.TextureMap["fire_01"], 2)  // TODO: change fire texture
                    },
                    {
                        "camfireXButton",
                        new Icon(new Vector2(game.Width - 60, 150),
                            _game.TextureMap["f_button"], // TODO: Change to x_button
                            2F)
                    },
                    {
                        "campfireRefuelTwigIcon",
                        new Icon(new Vector2(game.Width - 200, 200),
                            _game.TextureMap["twigs"])
                    },
                    {
                        "campfireRefuelIcon",
                        new Icon(new Vector2(game.Width - 120, 200),
                            _game.TextureMap["fire_01"], 2)  // TODO: change fire texture
                    },
                    {
                        "camfireRefuelFButton",
                        new Icon(new Vector2(game.Width - 60, 200),
                            _game.TextureMap["f_button"],
                            2F)
                    },
                    {
                        "followerFeedBerry",
                        new Icon(new Vector2(game.Width - 200, 250),
                            _game.TextureMap["twigs"])  // TODO: change texture to berry
                    },
                    {
                        "followerFeedIcon",
                        new Icon(new Vector2(game.Width - 120, 250),
                            _game.TextureMap["follower"])
                    },
                    {
                        "followerFeedFButton",
                        new Icon(new Vector2(game.Width - 60, 250),
                            _game.TextureMap["f_button"],
                            2F)
                    }
                };
        }

        public void Update()
        {
            _textIcons["twigCount"].Text = _player.Twigs.ToString();
            _textIcons["berryCount"].Text = _player.Berries.ToString();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _textIcons
                .Values
                .ToList()
                .ForEach(textIcon => textIcon.Draw(spriteBatch));
            _icons.Values.ToList().ForEach(icon => icon.Draw(spriteBatch));
        }
    }
}