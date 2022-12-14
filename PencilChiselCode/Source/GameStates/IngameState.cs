using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Screens.Transitions;
using MonoGame.Extended.Tiled;
using PencilChiselCode.Source.GUI;
using PencilChiselCode.Source.Objects;

namespace PencilChiselCode.Source.GameStates;

public class IngameState : BonfireGameState
{
    public Player Player { get; private set; }
    public Companion Companion;
    private float _cameraSpeed = 20F;
    private int _fps;
    private TimeSpan _fpsCounterGameTime;
    private TimeSpan _pickupableCounterGameTime;
    private bool _pauseState;
    private const double TwigSpawnChance = 0.14;
    private const double BushSpawnChance = 0.14;
    private const double TreeSpawnChance = 0.24;
    private const int TwigCount = 14;
    private const int BushCount = 14;
    private const int TreeCount = 36;
    private const int GlowFlowerCount = 10;
    private List<TiledMap> _maps;
    private ParticleGenerator _darknessParticles;
    private readonly List<string> _debugData = new() { "", "", "" };
    public const float MinimumFollowerPlayerDistance = 100F;
    private bool _deathState;
    private Song _song;
    private float _score;
    private const int SpawnOffset = 128;
    public const int DarknessEndOffset = 64;
    public OrthographicCamera Camera { get; private set; }
    public RootBox RootBox;

    private int MapIndex =>
        (int)Math.Abs(Math.Floor(Camera.GetViewMatrix().Translation.X / _maps[0].HeightInPixels));

    public IngameState(Game game) : base(game) { }

    public void Cleanup()
    {
        Pickupables.Clear();
        GroundEntities.Clear();
        Campfires.Clear();
        Player.Cleanup();
    }

    public EntityCollection<Pickupable> Pickupables { get; } = new();
    public EntityCollection<GroundEntity> GroundEntities { get; } = new();
    public EntityCollection<CampFire> Campfires { get; } = new();

    public override void LoadContent()
    {
        Camera = new OrthographicCamera(Game.GraphicsDevice);
        _deathState = false;
        base.LoadContent();
        for (var i = 0; i < TwigCount; i++)
        {
            SpawnRandomTwig(
                Utils.GetRandomInt((int)Camera.Position.X, Game.GetWindowWidth() + SpawnOffset),
                Utils.GetRandomInt(10, Game.GetWindowHeight() - 10),
                chance: 1,
                attempts: 30
            );
        }

        for (var i = 0; i < BushCount; i++)
        {
            SpawnRandomBush(
                Utils.GetRandomInt((int)Camera.Position.X, Game.GetWindowWidth() + SpawnOffset),
                Utils.GetRandomInt(10, Game.GetWindowHeight() - 10),
                chance: 1,
                attempts: 30
            );
        }

        for (var i = 0; i < TreeCount; i++)
        {
            SpawnRandomTree(
                Utils.GetRandomInt((int)Camera.Position.X, Game.GetWindowWidth() + SpawnOffset),
                Utils.GetRandomInt(10, Game.GetWindowHeight() - 10),
                chance: 1,
                attempts: 64
            );
        }

        for (var i = 0; i < GlowFlowerCount; i++)
        {
            SpawnRandomPlant(
                Utils.GetRandomInt((int)Camera.Position.X, Game.GetWindowWidth() + SpawnOffset),
                Utils.GetRandomInt(10, Game.GetWindowHeight() - 10),
                chance: 1,
                attempts: 30
            );
        }

        var resumeButton = new Button(
            Game.TextureMap["resume_button_normal"],
            Game.TextureMap["resume_button_hover"],
            Game.TextureMap["resume_button_pressed"],
            Game.SoundMap["button_press"],
            Game.SoundMap["button_release"],
            () => _pauseState = false
        );
        var menuButton = new Button(
            Game.TextureMap["menu_button_normal"],
            Game.TextureMap["menu_button_hover"],
            Game.TextureMap["menu_button_pressed"],
            Game.SoundMap["button_press"],
            Game.SoundMap["button_release"],
            () =>
                ScreenManager.LoadScreen(
                    new MenuState(Game),
                    new FadeTransition(GraphicsDevice, Color.Black, 0.5F)
                )
        );
        var restartButton = new Button(
            Game.TextureMap["restart_button_normal"],
            Game.TextureMap["restart_button_hover"],
            Game.TextureMap["restart_button_pressed"],
            Game.SoundMap["button_press"],
            Game.SoundMap["button_release"],
            () =>
            {
                Cleanup();
                Game.ScreenManager.LoadScreen(
                    new IngameState(Game),
                    new FadeTransition(Game.GraphicsDevice, Color.Black, 0.5F)
                );
                Game.ResetPenumbra();
            }
        );

        RootBox = Game.GetRootBox();
        var buttonBox = new RelativeBox(Game, 0, 0.5F)
        {
            BoxAlignment = Alignments.MiddleCenter,
            SelfAlignment = Alignments.MiddleCenter
        }.WithChild(
            new RelativeBox(Game, (0, 0.6F), new FitElement())
            {
                BoxAlignment = Alignments.MiddleCenter,
                SelfAlignment = Alignments.MiddleCenter,
                IsVisible = () => _pauseState || _deathState,
                DrawableElement = menuButton
            },
            new RelativeBox(Game, (0, -32), new FitElement())
            {
                BoxAlignment = Alignments.TopCenterOfPrevious,
                SelfAlignment = Alignments.BottomCenter,
                IsVisible = () => _pauseState,
                DrawableElement = resumeButton
            },
            new RelativeBox(Game, (0, -32), new FitElement())
            {
                BoxAlignment = Alignments.TopCenterOfPrevious,
                SelfAlignment = Alignments.BottomCenter,
                PreviousBoxIndex = ^2,
                IsVisible = () => _deathState,
                DrawableElement = restartButton
            }
        );
        RootBox.AddChild(buttonBox);

        var gameOverText = new UiTextElement(
            Game.FontMap["32"],
            () => "Your companion got too anxious!",
            Color.Red,
            Color.Black
        );
        var textInfoBox = new RelativeBox(Game, 0, (0.5F, 0.25F))
        {
            BoxAlignment = Alignments.MiddleCenter,
            SelfAlignment = Alignments.MiddleCenter
        };
        var finalScoreText = new UiTextElement(
            Game.FontMap["24"],
            () =>
                $"{(_deathState ? "Final score" : "Current score")}: {(int)Math.Ceiling(_score / 10)}",
            Color.Red,
            Color.Black
        );
        textInfoBox.AddChild(
            new RelativeBox(Game, 0, new FitElement())
            {
                BoxAlignment = Alignments.TopCenter,
                SelfAlignment = Alignments.TopCenter,
                IsVisible = () => _deathState,
                DrawableElement = gameOverText
            },
            new RelativeBox(Game, (0, 50), new FitElement())
            {
                BoxAlignment = Alignments.TopCenter,
                SelfAlignment = Alignments.TopCenter,
                IsVisible = () => _deathState || _pauseState,
                DrawableElement = finalScoreText
            }
        );
        RootBox.AddChild(textInfoBox);

        Companion = new Companion(this, new Vector2(128, Game.GetWindowHeight() / 2F), 100F);
        Player = new Player(this, new Vector2(96, Game.GetWindowHeight() / 2F));

        Campfires.Add(new CampFire(this, new Vector2(500, 400))); // TEMP

        _maps = new List<TiledMap>();
        for (var i = 0; i < 3; ++i)
        {
            AddRandomMap();
        }

        _darknessParticles = new ParticleGenerator(
            () =>
                new Particle(
                    4F,
                    new Vector2(0, Utils.Random.Next(0, Game.Height)),
                    new Vector2(Utils.Random.Next(0, 20), Utils.Random.Next(-10, 10)),
                    time => 2 + time,
                    _ => Color.Black
                ),
            100F
        );
        var inventoryBox = Menus.GetInventory(Game, Player);
        inventoryBox.IsVisible = () => _pauseState;
        RootBox.AddChild(inventoryBox);
        _song = Game.SongMap["bonfire_song"];
        MediaPlayer.Play(_song);
        MediaPlayer.IsRepeating = true;
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
        double chance = BushSpawnChance,
        int attempts = 10
    ) =>
        TryGenerate(
            () =>
            {
                var position = new Vector2(x, y);
                var size = Vector2.One * 2F;
                if (
                    GroundEntities.Any((entity) => entity.Intersects(position, size))
                    || Pickupables.Any((entity) => entity.Intersects(position, size))
                )
                    return false;
                var pickupable = new BerryBush(this, position, size);
                Pickupables.Add(pickupable);
                return true;
            },
            chance,
            attempts
        );

    public void SpawnRandomTree(
        float x,
        float y,
        int treeType = 0,
        double chance = TreeSpawnChance,
        int attempts = 10
    ) =>
        TryGenerate(
            () =>
            {
                if (treeType == 0)
                    treeType = Utils.GetRandomInt(1, Bonfire.TreeVariations + 1);
                var position = new Vector2(x, y);
                var size = Vector2.One * 2F;
                if (
                    GroundEntities.Any((entity) => entity.Intersects(position, size))
                    || Pickupables.Any((entity) => entity.Intersects(position, size))
                )
                    return false;
                var tree = new Tree(this, Game.TextureMap[$"tree_{treeType}"], position, size);
                GroundEntities.Add(tree);
                return true;
            },
            chance,
            attempts
        );

    public void SpawnRandomPlant(
        float x,
        float y,
        double chance = TreeSpawnChance,
        int attempts = 10
    ) =>
        TryGenerate(
            () =>
            {
                var position = new Vector2(x, y);
                var size = Vector2.One * 1.5F;
                if (
                    GroundEntities.Any((entity) => entity.Intersects(position, size))
                    || Pickupables.Any((entity) => entity.Intersects(position, size))
                )
                    return false;
                var plant = new Tree(
                    this,
                    Game.TextureMap["flower_lamp_1"],
                    position,
                    size,
                    new Color(0F, 0.3F, 0.75F)
                );
                GroundEntities.Add(plant);
                return true;
            },
            chance,
            attempts
        );

    public void SpawnRandomTwig(
        float x,
        float y,
        double chance = TwigSpawnChance,
        int attempts = 10
    ) =>
        TryGenerate(
            () =>
            {
                var position = new Vector2(x, y);
                var size = Vector2.One;
                if (
                    GroundEntities.Any((entity) => entity.Intersects(position, size))
                    || Pickupables.Any((entity) => entity.Intersects(position, size))
                )
                    return false;
                var pickupable = new Twig(this, position, size, Utils.Random.NextAngle());
                Pickupables.Add(pickupable);
                return true;
            },
            chance,
            attempts
        );

    private void AddRandomMap() =>
        _maps.Add(Game.TiledMaps[Utils.Random.Next(0, Game.TiledMaps.Count)]);

    public override void Update(GameTime gameTime)
    {
        if (!_deathState && Game.Controls.JustPressed(ControlKeys.Pause))
        {
            _pauseState = !_pauseState;
        }

        RootBox.Update(gameTime);

        if (_deathState || _pauseState)
            return;
        if (gameTime.TotalGameTime.Subtract(_pickupableCounterGameTime).TotalSeconds >= 0.25)
        {
            SpawnRandomTwig(
                Camera.Position.X + Game.GetWindowWidth() + SpawnOffset,
                Utils.GetRandomInt(5, Game.GetWindowHeight())
            );
            SpawnRandomBush(
                Camera.Position.X + Game.GetWindowWidth() + SpawnOffset,
                Utils.GetRandomInt(5, Game.GetWindowHeight())
            );
            SpawnRandomTree(
                Camera.Position.X + Game.GetWindowWidth() + SpawnOffset,
                Utils.GetRandomInt(5, Game.GetWindowHeight())
            );
            SpawnRandomPlant(
                Camera.Position.X + Game.GetWindowWidth() + SpawnOffset,
                Utils.GetRandomInt(5, Game.GetWindowHeight())
            );
            _pickupableCounterGameTime = gameTime.TotalGameTime;
        }

        var oldMapIndex = MapIndex;
        Game.TiledMapRenderer.Update(gameTime);

        if (Companion.IsAnxious())
        {
            _deathState = true;
            return;
        }

        Camera.Move(Vector2.UnitX * _cameraSpeed * gameTime.GetElapsedSeconds());
        Companion.Update(gameTime, Player.Position);
        Player.Update(gameTime);

        Pickupables.Update(gameTime);

        GroundEntities.Update(gameTime);

        Campfires.Update(gameTime);

        _darknessParticles.Update(gameTime, true);
        _score += gameTime.ElapsedGameTime.Milliseconds;

        if (oldMapIndex != MapIndex)
        {
            _maps.RemoveAt(0);
            AddRandomMap();
        }

        if (Game.Controls.JustPressed(ControlKeys.StopFollower))
        {
            Companion.ToggleSitting();
        }

        if (Game.Controls.JustPressed(ControlKeys.StartFire))
        {
            Player.CreateFire();
        }

        _debugData[1] = $"Translation: {Camera.GetViewMatrix().Translation}";
        _debugData[2] = $"Map Index: {MapIndex}";
    }

    public override void Draw(GameTime gameTime)
    {
        Game.Penumbra.BeginDraw();
        Game.Penumbra.Transform = Matrix.CreateTranslation(
            -Camera.Position.X,
            -Camera.Position.Y,
            0
        );
        Game.GraphicsDevice.Clear(BgColor);
        var transformMatrix = Camera.GetViewMatrix();

        for (var i = 0; i < _maps.Count; ++i)
        {
            Game.TiledMapRenderer.LoadMap(_maps[i]);
            Game.TiledMapRenderer.Draw(
                transformMatrix
                    * Matrix.CreateTranslation(_maps[i].WidthInPixels * (i + MapIndex - 1), 0, 0)
            );
        }

        Game.SpriteBatch.Begin(
            transformMatrix: transformMatrix,
            samplerState: SamplerState.PointClamp
        );

        Pickupables.ForEach(pickupable => pickupable.Draw(Game.SpriteBatch));
        Campfires.ForEach(campfire => campfire.Draw(Game.SpriteBatch));
        GroundEntities.ForEach(groundEntity => groundEntity.Draw(Game.SpriteBatch));

        Companion.Draw(Game.SpriteBatch);
        Player.Draw(Game.SpriteBatch);

        Game.SpriteBatch.End();

        Game.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        _darknessParticles.Draw(Game.SpriteBatch);

        Game.SpriteBatch.End();

        lock (Game.Penumbra)
        {
            Game.Penumbra.Draw(gameTime);
        }

        DrawUI(gameTime);
    }

    private void DrawUI(GameTime gameTime)
    {
        var transformMatrix = Camera.GetViewMatrix();
        Game.SpriteBatch.Begin(
            transformMatrix: transformMatrix,
            samplerState: SamplerState.PointClamp
        );
        Player.DrawPopupButton(Game.SpriteBatch);
        Campfires.ForEach(campfire => campfire.DrawUI(Game.SpriteBatch));
        Game.SpriteBatch.End();

        Game.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        Companion.ComfyMeter.Draw(Game.SpriteBatch);
        Game.SpriteBatch.DrawString(
            Game.FontMap["32"],
            "Comfy meter",
            new Vector2(Game.GetWindowWidth() / 2 - 350, Game.GetWindowHeight() - 100),
            Color.Orange
        );

        if (gameTime.TotalGameTime.Subtract(_fpsCounterGameTime).TotalSeconds >= 0.5)
        {
            _fps = (int)(1 / gameTime.GetElapsedSeconds());
            _fpsCounterGameTime = gameTime.TotalGameTime;
            _debugData[0] = $"FPS: {_fps}";
        }

        RootBox.Draw(Game.SpriteBatch);

        if (Game.DebugMode > 0)
        {
            for (var i = 0; i < _debugData.Count; ++i)
            {
                Game.SpriteBatch.DrawString(
                    Game.FontMap["16"],
                    _debugData[i],
                    new Vector2(16, 16 * (i + 1)),
                    Color.White
                );
            }
        }

        Game.SpriteBatch.End();
    }

    public void AddCampfire(Vector2 position) => Campfires.Add(new CampFire(this, position));
}
