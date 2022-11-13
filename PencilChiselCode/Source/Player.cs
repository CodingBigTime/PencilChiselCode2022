using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using PencilChiselCode.Source.GameStates;
using Penumbra;

namespace PencilChiselCode.Source;

public class Player
{
    public Vector2 Size;

    public Vector2 Position
    {
        get => position;
        set
        {
            position = value;
            PointLight.Position = new Vector2(value.X, value.Y);
            Spotlight.Position = new Vector2(value.X, value.Y);
        }
    }

    Vector2 position;
    private const float Sqrt12 = 0.70710678118654752440084436210485F;
    private const float PI = (float)Math.PI;
    private Game1 _game;
    private AnimatedSprite _animatedSprite;
    private Vector2 _speed;
    private static readonly int _pointLightScale = 300;
    private static readonly int _spotLightScale = 400;
    private static readonly float _scale = 2F;
    private static readonly float _maxSpeed = 80F;
    private static readonly float _acceleration = 1000F;
    private static readonly float _friction = 2.75F;
    private readonly Dictionary<string, PopupButton> _popupButtons = new();
    public uint Twigs { get; private set; }
    public int Berries { get; private set; }
    private ParticleGenerator _particleGenerator;

    public Player(Game1 game, Vector2 position)
    {
        _game = game;
        Position = position;
        _animatedSprite = new AnimatedSprite(_game.SpriteSheetMap["player"]);
        _animatedSprite.Play("right");
        Size = new(_animatedSprite.TextureRegion.Width * _scale, _animatedSprite.TextureRegion.Height * _scale);
        _game.Penumbra.Lights.Add(PointLight);
        _game.Penumbra.Lights.Add(Spotlight);
        _particleGenerator = new ParticleGenerator(
            (() => new Particle(
                3,
                Position + Vector2.UnitY * Utils.RANDOM.NextSingle(-10, 10) +
                Vector2.UnitX * Utils.RANDOM.NextSingle(-10, 10),
                Vector2.UnitY * Utils.RANDOM.NextSingle(-5, 5) - Vector2.UnitX * Utils.RANDOM.NextSingle(-5, 5),
                ((time) => (3 - time) / 3 * 1F),
                ((time) => Color.LightBlue)
            )),
            3F
        );
    }

    public Light PointLight { get; } = new PointLight
    {
        Scale = new Vector2(_pointLightScale),
        Color = Color.White,
        ShadowType = ShadowType.Occluded
    };

    public Light Spotlight { get; } = new Spotlight
    {
        Scale = new Vector2(_spotLightScale),
        Color = Color.White,
        ShadowType = ShadowType.Occluded,
        ConeDecay = 2.5F
    };

    public void Draw(SpriteBatch spriteBatch)
    {
        _particleGenerator.Draw(spriteBatch);
        var angle = (float)Math.Atan2(_speed.Y, _speed.X);
        _animatedSprite.Play(
            angle switch
            {
                >= -PI / 4 and <= PI / 4 => "right",
                > PI / 4 and < 3 * PI / 4 => "down",
                >= 3 * PI / 4 or <= -3 * PI / 4 => "left",
                > -3 * PI / 4 and < -PI / 4 => "up",
                _ => "right"
            }
        );
        _animatedSprite.Draw(
            spriteBatch: spriteBatch,
            position: Position,
            rotation: 0,
            scale: new(_scale)
        );
    }

    public void DrawPopupButton(SpriteBatch spriteBatch)
    {
        var i = 0;
        foreach (var (_, value) in _popupButtons)
        {
            value?.Draw(spriteBatch, Position + new Vector2(i * (value.Texture.Width * 1.5F), 0), Size);
            ++i;
        }
    }

    public void CreateFire(uint amount)
    {
        Twigs -= amount;
    }

    public void ReduceBerries(int amount)
    {
        Berries -= amount;
    }

    public bool CanCreateFire() => Twigs >= 10;

    public void Update(IngameState state, GameTime gameTime)
    {
        var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
        var keyState = Keyboard.GetState();
        var left = keyState.IsKeyDown(Keys.A) || keyState.IsKeyDown(Keys.Left);
        var up = keyState.IsKeyDown(Keys.W) || keyState.IsKeyDown(Keys.Up);
        var down = keyState.IsKeyDown(Keys.S) || keyState.IsKeyDown(Keys.Down);
        var right = keyState.IsKeyDown(Keys.D) || keyState.IsKeyDown(Keys.Right);
        var mx = Convert.ToSingle(right) - Convert.ToSingle(left);
        var my = Convert.ToSingle(down) - Convert.ToSingle(up);
        var angle = (float)Math.Atan2(_speed.Y, _speed.X);

        Spotlight.Rotation = angle;
        _animatedSprite.Update(gameTime);

        if (mx != 0 && my != 0)
        {
            // Normalize the diagonal movement
            mx *= Sqrt12;
            my *= Sqrt12;
        }

        var ax = _acceleration * mx * delta;
        var ay = _acceleration * my * delta;

        _speed += new Vector2(ax, ay);
        _speed += -(_speed * _friction * delta);
        var biasX = Math.Abs(_speed.X) / (float)Math.Sqrt(_speed.X * _speed.X + _speed.Y * _speed.Y);
        var biasY = Math.Abs(_speed.Y) / (float)Math.Sqrt(_speed.X * _speed.X + _speed.Y * _speed.Y);
        _speed.X = Math.Clamp(_speed.X, -_maxSpeed * biasX, _maxSpeed * biasX);
        _speed.Y = Math.Clamp(_speed.Y, -_maxSpeed * biasY, _maxSpeed * biasY);
        Position = new(Position.X + _speed.X * delta, Position.Y + _speed.Y * delta);

        if (Position.X >= _game.Camera.Center.X + _game.Width / 2F - Size.X / 2F)
        {
            _speed.X = 0;
            Position = new(Math.Min(Position.X, _game.Camera.Center.X + _game.Width / 2F - Size.X / 2F), Position.Y);
        }

        if (Position.Y <= Size.Y / 2F || Position.Y >= _game.Height - Size.Y / 2F)
        {
            _speed.Y = 0;
            Position = new(Position.X, Math.Clamp(Position.Y, Size.Y / 2F, _game.Height - Size.Y / 2F));
        }

        var nearestCampfire = state.Campfires
            .OrderBy(campfire => Vector2.DistanceSquared(campfire.Position, Position))
            .FirstOrDefault(campfire => Vector2.DistanceSquared(campfire.Position, Position) < 100 * 100);
        
        if (nearestCampfire != null && !_popupButtons.ContainsKey("F"))
        {
            _popupButtons["F"] = new PopupButton(_game, _game.TextureMap["f_button"]);
        }
        if (nearestCampfire == null)
        {
            _popupButtons.Remove("F");
        }

        if (!state.PreviousPressedKeys.Contains(Keys.F) && keyState.IsKeyDown(Keys.F) && nearestCampfire != null && Twigs > 0)
        {
            nearestCampfire.FeedFire(10F);
            --Twigs;
        }
        if (nearestCampfire == null || Twigs <= 0) _popupButtons.Remove("F");

        var nearestPickupable = state.Pickupables
            .OrderBy(pickupable => Vector2.DistanceSquared(pickupable.Position, Position))
            .FirstOrDefault(pickupable =>
                Vector2.DistanceSquared(pickupable.Position, Position) < 100 * 100 && pickupable.IsConsumable);

        if (nearestPickupable != null && !_popupButtons.ContainsKey("E"))
        {
            _popupButtons["E"] = new PopupButton(_game, _game.TextureMap["e_button"]);
        }

        foreach (var (_, value) in _popupButtons)
        {
            value.Update(state, gameTime);
        }

        if (!state.PreviousPressedKeys.Contains(Keys.E) && keyState.IsKeyDown(Keys.E) && nearestPickupable != null)
        {
            switch (nearestPickupable.Type)
            {
                case PickupableTypes.Twig:
                    ++Twigs;
                    nearestPickupable.PickupSound.Play();
                    state.Pickupables.Remove(nearestPickupable);
                    break;
                case PickupableTypes.Bush:
                    ++Berries;
                    nearestPickupable.PickupSound.Play();
                    nearestPickupable.Texture = _game.TextureMap["bush_empty"];
                    break;
            }

            _popupButtons.Remove("E");
            nearestPickupable.IsConsumable = false;
        }

        if (nearestPickupable == null && (nearestCampfire == null || Twigs <= 0))
        {
            _popupButtons.Clear();
        }

        _particleGenerator.Update(gameTime, _speed.Length() > 1);
    }
}