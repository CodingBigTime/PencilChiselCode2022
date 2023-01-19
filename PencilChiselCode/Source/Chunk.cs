using System;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using PencilChiselCode.Source.GameStates;
using PencilChiselCode.Source.Objects;

namespace PencilChiselCode.Source;

public class Chunk
{
    private TiledMap _map;
    private IngameState _state;
    private Bonfire Game => _state.Game;

    private const int TwigCount = 14;
    private const int BushCount = 14;
    private const int TreeCount = 36;
    private const int GlowFlowerCount = 10;
    public EntityCollection<Pickupable> Pickupables { get; } = new();
    public EntityCollection<GroundEntity> GroundEntities { get; } = new();

    public int MapIndex =>
        (int)Math.Floor(Math.Abs(_state.Camera.GetViewMatrix().Translation.X / _map.WidthInPixels));

    public Chunk(IngameState state, int offsetIndex)
    {
        _state = state;
        _map = Game.TiledMaps[Utils.Random.Next(0, Game.TiledMaps.Count)];

        for (var i = 0; i < TwigCount; i++)
        {
            SpawnRandomTwig(
                Utils.GetRandomInt(
                    (int)_state.Camera.Position.X,
                    (int)_state.Camera.Position.X + _map.WidthInPixels
                )
                    + offsetIndex * _map.WidthInPixels,
                Utils.GetRandomInt(10, Game.GetWindowHeight() - 10)
            );
        }

        for (var i = 0; i < BushCount; i++)
        {
            SpawnRandomBush(
                Utils.GetRandomInt(
                    (int)_state.Camera.Position.X,
                    (int)_state.Camera.Position.X + _map.WidthInPixels
                )
                    + offsetIndex * _map.WidthInPixels,
                Utils.GetRandomInt(10, Game.GetWindowHeight() - 10)
            );
        }

        for (var i = 0; i < TreeCount; i++)
        {
            SpawnRandomTree(
                Utils.GetRandomInt(
                    (int)_state.Camera.Position.X,
                    (int)_state.Camera.Position.X + _map.WidthInPixels
                )
                    + offsetIndex * _map.WidthInPixels,
                Utils.GetRandomInt(10, Game.GetWindowHeight() - 10)
            );
        }

        for (var i = 0; i < GlowFlowerCount; i++)
        {
            SpawnRandomPlant(
                Utils.GetRandomInt(
                    (int)_state.Camera.Position.X,
                    (int)_state.Camera.Position.X + _map.WidthInPixels
                )
                    + offsetIndex * _map.WidthInPixels,
                Utils.GetRandomInt(10, Game.GetWindowHeight() - 10)
            );
        }
    }

    public void Draw(GameTime gameTime, Matrix transformMatrix, int index)
    {
        Game.TiledMapRenderer.LoadMap(_map);
        Game.TiledMapRenderer.Draw(
            transformMatrix
                * Matrix.CreateTranslation(_map.WidthInPixels * (index + MapIndex), 0, 0)
        );
    }

    public void DrawObjects(GameTime gameTime)
    {
        Pickupables.ForEach(pickupable => pickupable.Draw(Game.SpriteBatch));
        GroundEntities.ForEach(groundEntity => groundEntity.Draw(Game.SpriteBatch));
    }

    public void Update(GameTime gameTime)
    {
        Pickupables.Update(gameTime);
        GroundEntities.Update(gameTime);
    }

    public static void TryGenerate(Func<bool> generator, double chance = 1, int attempts = 10)
    {
        if (Utils.Random.NextDouble() > chance)
            return;
        for (var i = 0; i < attempts; ++i)
        {
            if (generator())
            {
                break;
            }
        }
    }

    public void SpawnRandomBush(
        float x,
        float y,
        double chance = 1F,
        int attempts = 30
    ) =>
        TryGenerate(
            () =>
            {
                var position = new Vector2(x, y);
                var texture = Game.TextureMap["bush_berry"];
                var scale = Vector2.One * 2F;
                var size = texture.Bounds.Size.ToVector2() * scale;
                if (
                    GroundEntities.Any(entity => entity.Intersects(position, size))
                    || Pickupables.Any(entity => entity.Intersects(position, size))
                )
                    return false;
                Pickupables.Add(new BerryBush(_state, position, scale));
                return true;
            },
            chance,
            attempts
        );

    public void SpawnRandomTree(
        float x,
        float y,
        int treeType = 0,
        double chance = 1F,
        int attempts = 30
    ) =>
        TryGenerate(
            () =>
            {
                if (treeType == 0)
                    treeType = Utils.GetRandomInt(1, Bonfire.TreeVariations + 1);
                var position = new Vector2(x, y);
                var texture = Game.TextureMap[$"tree_{treeType}"];
                var scale = Vector2.One * 2F;
                var size = texture.Bounds.Size.ToVector2() * scale;
                if (
                    GroundEntities.Any(entity => entity.Intersects(position, size))
                    || Pickupables.Any(entity => entity.Intersects(position, size))
                )
                    return false;
                GroundEntities.Add(new Tree(_state, texture, position, scale));
                return true;
            },
            chance,
            attempts
        );

    public void SpawnRandomPlant(
        float x,
        float y,
        double chance = 1F,
        int attempts = 30
    ) =>
        TryGenerate(
            () =>
            {
                var position = new Vector2(x, y);
                var texture = Game.TextureMap["flower_lamp_1"];
                var scale = Vector2.One * 1.5F;
                var size = texture.Bounds.Size.ToVector2() * scale;
                if (
                    GroundEntities.Any(entity => entity.Intersects(position, size))
                    || Pickupables.Any(entity => entity.Intersects(position, size))
                )
                    return false;
                GroundEntities.Add(
                    new Tree(_state, texture, position, scale, new Color(0F, 0.3F, 0.75F))
                );
                return true;
            },
            chance,
            attempts
        );

    public void SpawnRandomTwig(
        float x,
        float y,
        double chance = 1F,
        int attempts = 30
    ) =>
        TryGenerate(
            () =>
            {
                var position = new Vector2(x, y);
                var texture = Game.TextureMap["twigs"];
                var scale = Vector2.One;
                var size = texture.Bounds.Size.ToVector2() * scale;
                if (
                    GroundEntities.Any(entity => entity.Intersects(position, size))
                    || Pickupables.Any(entity => entity.Intersects(position, size))
                )
                    return false;
                Pickupables.Add(new Twig(_state, position, scale, Utils.Random.NextAngle()));
                return true;
            },
            chance,
            attempts
        );

    public void Cleanup()
    {
        Pickupables.ClearAll();
        GroundEntities.ClearAll();
    }
}
