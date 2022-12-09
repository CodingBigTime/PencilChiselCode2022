using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using PencilChiselCode.Source.GameStates;
using Penumbra;

namespace PencilChiselCode.Source;

public class Player
{
    public Companion Companion => _state.Companion;

    public Vector2 Size =>
        new(
            _animatedSprite.TextureRegion.Width * _scale,
            _animatedSprite.TextureRegion.Height * _scale
        );

    public Vector2 Position
    {
        get => _position;
        set
        {
            _position = value;
            PointLight.Position = new Vector2(value.X, value.Y);
            Spotlight.Position = new Vector2(value.X, value.Y);
        }
    }

    private Vector2 _position;
    private const float Sqrt12 = 0.70710678118654752440084436210485F;
    private const float PI = (float)Math.PI;
    private AnimatedSprite _animatedSprite;
    private Vector2 _speed;
    private const int PointLightScale = 300;
    private const int SpotLightScale = 400;
    private readonly float _scale = 2F;
    private readonly float _maxSpeed = 80F;
    private readonly float _acceleration = 1000F;
    private readonly float _friction = 2.75F;
    private readonly Dictionary<string, PopupButton> _popupButtons = new();
    public readonly Dictionary<PickupableTypes, uint> Inventory = new();
    private readonly ParticleGenerator _particleGenerator;
    private readonly IngameState _state;
    private Bonfire Game => _state.Game;

    public Player(IngameState state, Vector2 position)
    {
        _state = state;
        Position = position;
        _animatedSprite = new AnimatedSprite(Game.SpriteSheetMap["player"]);
        _animatedSprite.Play("right");
        Game.Penumbra.Lights.Add(PointLight);
        Game.Penumbra.Lights.Add(Spotlight);
        _particleGenerator = new ParticleGenerator(
            () =>
                new Particle(
                    3,
                    Position
                        + Vector2.UnitY * Utils.RANDOM.NextSingle(-10, 10)
                        + Vector2.UnitX * Utils.RANDOM.NextSingle(-10, 10),
                    Vector2.UnitY * Utils.RANDOM.NextSingle(-5, 5)
                        - Vector2.UnitX * Utils.RANDOM.NextSingle(-5, 5),
                    time => (3 - time) / 3 * 1F,
                    _ => Color.LightBlue
                ),
            3F
        );
        Enum.GetValues(typeof(PickupableTypes))
            .Cast<PickupableTypes>()
            .ToList()
            .ForEach(type => Inventory.Add(type, 0));
    }

    public void Cleanup()
    {
        lock (Game.Penumbra)
        {
            Game.Penumbra.Lights.Remove(PointLight);
            Game.Penumbra.Lights.Remove(Spotlight);
        }
    }

    public Light PointLight { get; } =
        new PointLight
        {
            Scale = new Vector2(PointLightScale),
            Color = Color.White,
            ShadowType = ShadowType.Occluded
        };

    public Light Spotlight { get; } =
        new Spotlight
        {
            Scale = new Vector2(SpotLightScale),
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
        _popupButtons.Values
            .ToList()
            .ForEach(button =>
            {
                button?.Draw(
                    spriteBatch,
                    Position + new Vector2(i * (button.Texture.Width * 1.5F), 0),
                    Size
                );
                ++i;
            });
    }

    public void CreateFire()
    {
        if (!CanCreateFire())
            return;
        Game.SoundMap["light_fire"].Play();
        _state.AddCampfire(Position + new Vector2(20, -20));
        Inventory[PickupableTypes.Twig] -= CampFire.TwigCost;
    }

    public void ReduceBerries(uint amount)
    {
        if (Inventory[PickupableTypes.BerryBush] < amount)
            return;
        Inventory[PickupableTypes.BerryBush] -= amount;
    }

    public bool CanCreateFire() => Inventory[PickupableTypes.Twig] >= CampFire.TwigCost;

    public void Update(GameTime gameTime)
    {
        var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
        var (mx, my) = Game.Controls.GetMovement();

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
        var biasX =
            Math.Abs(_speed.X) / (float)Math.Sqrt(_speed.X * _speed.X + _speed.Y * _speed.Y);
        var biasY =
            Math.Abs(_speed.Y) / (float)Math.Sqrt(_speed.X * _speed.X + _speed.Y * _speed.Y);
        _speed.X = Math.Clamp(_speed.X, -_maxSpeed * biasX, _maxSpeed * biasX);
        _speed.Y = Math.Clamp(_speed.Y, -_maxSpeed * biasY, _maxSpeed * biasY);
        Position = new(Position.X + _speed.X * delta, Position.Y + _speed.Y * delta);

        if (
            _state.Game.Controls.JustPressed(ControlKeys.FEED)
            && Vector2.Distance(Companion.Position, Position) <= 100
            && _state.Player.Inventory[PickupableTypes.BerryBush] > 0
        )
        {
            _state.Player.ReduceBerries(1);
            Companion.ComfyMeter.Value += 10F;
        }

        if (Position.X >= Game.Camera.Center.X + Game.Width / 2F - Size.X / 2F)
        {
            _speed.X = 0;
            Position = new(
                Math.Min(Position.X, Game.Camera.Center.X + Game.Width / 2F - Size.X / 2F),
                Position.Y
            );
        }

        if (Position.Y <= Size.Y / 2F || Position.Y >= Game.Height - Size.Y / 2F)
        {
            _speed.Y = 0;
            Position = new(
                Position.X,
                Math.Clamp(Position.Y, Size.Y / 2F, Game.Height - Size.Y / 2F)
            );
        }

        var nearestCampfire = _state.Campfires
            .OrderBy(campfire => Vector2.DistanceSquared(campfire.Position, Position))
            .FirstOrDefault(
                campfire => Vector2.DistanceSquared(campfire.Position, Position) < 100 * 100
            );

        if (nearestCampfire != null && !_popupButtons.ContainsKey("F"))
        {
            _popupButtons["F"] = new PopupButton(_state, Game.TextureMap["f_key"]);
        }

        if (nearestCampfire == null)
        {
            _popupButtons.Remove("F");
        }

        if (
            _state.Game.Controls.JustPressed(ControlKeys.REFUEL)
            && nearestCampfire != null
            && Inventory[PickupableTypes.Twig] > 0
        )
        {
            nearestCampfire.FeedFire(25F);
            --Inventory[PickupableTypes.Twig];
        }

        if (nearestCampfire == null || Inventory[PickupableTypes.Twig] <= 0)
            _popupButtons.Remove("F");

        var nearestPickupable = _state.Pickupables
            .OrderBy(pickupable => Vector2.DistanceSquared(pickupable.Position, Position))
            .FirstOrDefault(
                pickupable =>
                    Vector2.DistanceSquared(pickupable.Position, Position) < 100 * 100
                    && pickupable.IsConsumable
            );

        if (nearestPickupable != null && !_popupButtons.ContainsKey("E"))
        {
            _popupButtons["E"] = new PopupButton(_state, Game.TextureMap["e_key"]);
        }

        _popupButtons.Values.ToList().ForEach(button => button?.Update(gameTime));

        if (Game.Controls.JustPressed(ControlKeys.COLLECT) && nearestPickupable != null)
        {
            ++Inventory[nearestPickupable.Type];
            nearestPickupable.OnPickup();
            _popupButtons.Remove("E");
            nearestPickupable.IsConsumable = false;
        }

        if (
            nearestPickupable == null
            && (nearestCampfire == null || Inventory[PickupableTypes.Twig] <= 0)
        )
        {
            _popupButtons.Clear();
        }

        _particleGenerator.Update(gameTime, _speed.Length() > 1);
    }
}
