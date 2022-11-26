using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Screens.Transitions;
using MonoGame.Extended.Tiled;

namespace PencilChiselCode.Source.GameStates;

public class IngameState : BonfireGameState
{
    private Player _player;
    private Companion _companion;
    private bool _showDebug;
    public readonly HashSet<Keys> PreviousPressedKeys = new();
    private float _cameraSpeed = 10F;
    private Attribute _followerAttribute;
    private Inventory _inventory;
    private int _fps;
    private TimeSpan _fpsCounterGameTime;
    private TimeSpan _pickupableCounterGameTime;
    private bool _pauseState;
    private Button _pauseButton;
    private Button _exitButton;
    private Button _menuButton;
    private Button _restartButton;
    private int _twigCount = 14;
    private int _bushCount = 14;
    private int _treeCount = 36;
    private List<TiledMap> _maps;
    private ParticleGenerator _darknessParticles;
    private readonly List<string> _debugData = new() { "", "", "" };
    private float _minimialFollowerPlayerDistance = 100F;
    private bool _deathState;
    private int _glowFlowerCount = 10;
    private Song _song;
    private float _score;
    private int _spawnOffset = 128;

    private int MapIndex =>
        (int)Math.Abs(Math.Floor(Game.Camera.GetViewMatrix().Translation.X / _maps[0].HeightInPixels));


    public IngameState(Game game) : base(game)
    {
    }

    public List<Pickupable> Pickupables { get; } = new();
    public List<GroundEntity> GroundEntities { get; } = new();
    public List<CampFire> Campfires { get; } = new();

    public override void LoadContent()
    {
        _deathState = false;
        base.LoadContent();
        for (var i = 0; i < _twigCount; i++)
        {
            Pickupables.Add(new Pickupable(this, PickupableTypes.Twig,
                Game.TextureMap["twigs"],
                Game.SoundMap["pickup_branches"],
                new Vector2(Utils.GetRandomInt((int)Game.Camera.Position.X, Game.GetWindowWidth() + _spawnOffset),
                    Utils.GetRandomInt(10, Game.GetWindowHeight() - 10)),
                Vector2.One, Utils.RANDOM.NextAngle()));
        }

        for (var i = 0; i < _bushCount; i++)
        {
            Pickupables.Add(new Pickupable(this, PickupableTypes.Bush,
                Game.TextureMap["bush_berry"],
                Game.SoundMap["pickup_branches"],
                new Vector2(Utils.GetRandomInt((int)Game.Camera.Position.X, Game.GetWindowWidth() + _spawnOffset),
                    Utils.GetRandomInt(10, Game.GetWindowHeight() - 10)),
                Vector2.One * 2));
        }

        for (var i = 0; i < _treeCount; i++)
        {
            var treeType = Utils.GetRandomInt(1, Bonfire.TreeVariations + 1);
            GroundEntities.Add(new GroundEntity(this, Game.TextureMap[$"tree_{treeType}"],
                new Vector2(Utils.GetRandomInt((int)Game.Camera.Position.X, Game.GetWindowWidth() + _spawnOffset),
                    Utils.GetRandomInt(10, Game.GetWindowHeight() - 10)),
                Vector2.One * 2F));
        }

        for (var i = 0; i < _glowFlowerCount; i++)
        {
            GroundEntities.Add(new GroundEntity(this, Game.TextureMap["flower_lamp_1"],
                new Vector2(Utils.GetRandomInt((int)Game.Camera.Position.X, Game.GetWindowWidth() + _spawnOffset),
                    Utils.GetRandomInt(10, Game.GetWindowHeight() - 10)),
                Vector2.One * 1.5F, new Color(0F, 0.3F, 0.75F)));
        }

        var resumeButton = Game.TextureMap["resume_button_normal"];
        var resumeButtonSize = new Size2(resumeButton.Width, resumeButton.Height);
        _pauseButton = new Button(this,
            resumeButton,
            Game.TextureMap["resume_button_hover"],
            Game.TextureMap["resume_button_pressed"],
            Utils.GetCenterStartCoords(resumeButtonSize, Game.GetWindowDimensions()),
            () => { _pauseState = false; }
        );
        var exitButton = Game.TextureMap["exit_button_normal"];
        var exitButtonSize = new Size2(exitButton.Width, exitButton.Height);
        _exitButton = new Button(this,
            exitButton,
            Game.TextureMap["exit_button_hover"],
            Game.TextureMap["exit_button_pressed"],
            Utils.GetCenterStartCoords(exitButtonSize, Game.GetWindowDimensions()) + Vector2.UnitY * 100,
            () => Game.Exit()
        );

        var menuButton = Game.TextureMap["menu_button_normal"];
        var menuButtonSize = new Size2(menuButton.Width, menuButton.Height);
        _menuButton = new Button(this,
            menuButton,
            Game.TextureMap["menu_button_hover"],
            Game.TextureMap["menu_button_pressed"],
            Utils.GetCenterStartCoords(menuButtonSize, Game.GetWindowDimensions()) + Vector2.UnitY * 100,
            () => Game.ScreenManager.LoadScreen(new MenuState(Game),
                new FadeTransition(Game.GraphicsDevice, Color.Black)));

        var restartButton = Game.TextureMap["restart_button_normal"];
        var restartButtonSize = new Size2(restartButton.Width, restartButton.Height);
        _restartButton = new Button(this,
            restartButton,
            Game.TextureMap["restart_button_hover"],
            Game.TextureMap["restart_button_pressed"],
            Utils.GetCenterStartCoords(restartButtonSize, Game.GetWindowDimensions()),
            () =>
            {
                Game.ScreenManager.LoadScreen(new IngameState(Game),
                    new FadeTransition(Game.GraphicsDevice, Color.Black));
                Game.ResetPenumbra();
            }
        );
        for (var i = 0; i < 10; ++i)
        {
            var pickupable = new Pickupable(this, PickupableTypes.Twig, Game.TextureMap["twigs"],
                Game.SoundMap["pickup_branches"],
                new Vector2(100 + Utils.RANDOM.Next(1, 20) * 50, Utils.RANDOM.Next(1, 15) * 50),
                Vector2.One, 0.5F);
            Pickupables.Add(pickupable);
        }


        _companion = new Companion(this, new Vector2(100, 100), 50F);
        _player = new Player(this, new Vector2(150, 150));

        Campfires.Add(new CampFire(this, new Vector2(500, 400))); // TEMP

        _maps = new List<TiledMap>();
        for (var i = 0; i < 3; ++i)
        {
            AddRandomMap();
        }

        _darknessParticles = new ParticleGenerator(
            () => new Particle(
                2F,
                new(0, Utils.RANDOM.Next(0, Game.Height)),
                new(Utils.RANDOM.Next(0, 20), Utils.RANDOM.Next(-10, 10)),
                time => 2 + time,
                _ => Color.Black
            ),
            100F
        );
        var attributeTexture = Game.TextureMap["attribute_bar"];
        var comfyAttributeTexture = Game.TextureMap["comfy_bar"];
        _followerAttribute =
            new Attribute(
                new Vector2(Game.GetWindowWidth() / 2F, Game.GetWindowHeight() - attributeTexture.Height * 3F), 3F,
                attributeTexture, comfyAttributeTexture, attributeTexture.Bounds.Center.ToVector2(), 100, -2F);

        _inventory = new Inventory(this, _player);
        _song = Game.SongMap["bonfire_song"];
        MediaPlayer.Play(_song);
        MediaPlayer.IsRepeating = true;
    }

    public void RandomBushSpawner()
    {
        if (Utils.GetRandomInt(0, 101) >= _twigCount) return;
        Pickupables.Add(new Pickupable(this, PickupableTypes.Bush,
            Game.TextureMap["bush_berry"],
            Game.SoundMap["pickup_branches"],
            new Vector2(Game.Camera.Position.X + Game.GetWindowWidth() + _spawnOffset,
                Utils.GetRandomInt(5, Game.GetWindowHeight())),
            Vector2.One * 2)
        );
    }

    public void RandomEntitySpawner()
    {
        if (Utils.GetRandomInt(0, 101) >= 24) return;
        var treeType = Utils.GetRandomInt(1, Bonfire.TreeVariations + 1);
        GroundEntities.Add(new GroundEntity(this, Game.TextureMap[$"tree_{treeType}"],
            new Vector2(Game.Camera.Position.X + Game.GetWindowWidth() + _spawnOffset,
                Utils.GetRandomInt(5, Game.GetWindowHeight())),
            Vector2.One * 2F));
        GroundEntities.Add(new GroundEntity(this, Game.TextureMap["flower_lamp_1"],
            new Vector2(Game.Camera.Position.X + Game.GetWindowWidth() + _spawnOffset,
                Utils.GetRandomInt(5, Game.GetWindowHeight())),
            Vector2.One * 1.5F, new Color(0F, 0.3F, 0.75F)));
    }

    public void RandomTwigSpawner()
    {
        if (Utils.GetRandomInt(0, 101) >= _twigCount) return;
        Pickupables.Add(new Pickupable(this, PickupableTypes.Twig,
            Game.TextureMap["twigs"],
            Game.SoundMap["pickup_branches"],
            new Vector2(Game.Camera.Position.X + Game.GetWindowWidth() + 128,
                Utils.GetRandomInt(5, Game.GetWindowHeight())),
            Vector2.One, Utils.RANDOM.NextAngle())
        );
    }

    private void AddRandomMap() => _maps.Add(Game.TiledMaps[Utils.RANDOM.Next(0, Game.TiledMaps.Count)]);

    public override void Update(GameTime gameTime)
    {
        if (gameTime.TotalGameTime.Subtract(_pickupableCounterGameTime).Milliseconds >= 500)
        {
            RandomTwigSpawner();
            RandomBushSpawner();
            RandomEntitySpawner();
            _pickupableCounterGameTime = gameTime.TotalGameTime;
        }

        var oldMapIndex = MapIndex;
        Game.TiledMapRenderer.Update(gameTime);
        var keyState = Keyboard.GetState();
        if (_followerAttribute.Value <= 0)
        {
            _deathState = true;
        }

        if (keyState.IsKeyDown(Keys.Escape) && !PreviousPressedKeys.Contains(Keys.Escape))
        {
            _pauseState = !_pauseState;
        }

        if (_deathState)
        {
            _restartButton.Update(gameTime);
            _exitButton.Update(gameTime);
        }
        else if (_pauseState)
        {
            _pauseButton.Update(gameTime);
            _menuButton.Update(gameTime);
        }
        else
        {
            Game.Camera.Move(Vector2.UnitX * _cameraSpeed * gameTime.GetElapsedSeconds());
            _companion.Update(gameTime, _player.Position);
            _player.Update(gameTime);
            _followerAttribute.Update(gameTime);
            Pickupables.ForEach(pickupable => pickupable.Update(gameTime));
            Pickupables.RemoveAll(pickupable => pickupable.ShouldRemove);
            GroundEntities.ForEach(groundEntity => groundEntity.Update(gameTime, _player.Position));
            GroundEntities.RemoveAll(groundEntity => groundEntity.ShouldRemove);
            Campfires.ForEach(campfire => { campfire.Update(gameTime); });
            Campfires.RemoveAll(campfire => campfire.ShouldRemove);

            if (Campfires.Any(campfire => campfire.IsInRange(_companion.Position)))
            {
                _followerAttribute.ChangeValue(8F * gameTime.GetElapsedSeconds());
            }

            var followerPlayerDistance = Vector2.Distance(_player.Position, _companion.Position);
            if (keyState.IsKeyDown(Keys.Q) && !PreviousPressedKeys.Contains(Keys.Q) && followerPlayerDistance <= 100 &&
                _player.Berries >= 1)
            {
                _player.ReduceBerries(1);
                _followerAttribute.ChangeValue(10F);
            }

            if (!Campfires.Any(campfire => campfire.IsInRange(_companion.Position)) &&
                followerPlayerDistance > _minimialFollowerPlayerDistance)
            {
                _followerAttribute.ChangeValue(-8F * gameTime.GetElapsedSeconds());
            }

            _inventory.Update();
            _darknessParticles.Update(gameTime, true);
            _score += gameTime.ElapsedGameTime.Milliseconds;
        }

        if (keyState.IsKeyDown(Keys.F3) && !PreviousPressedKeys.Contains(Keys.F3))
        {
            _showDebug = !_showDebug;
        }

        if (keyState.IsKeyDown(Keys.Space) && !PreviousPressedKeys.Contains(Keys.Space))
        {
            _companion.StopResumeFollower();
        }

        if (keyState.IsKeyDown(Keys.X) && !PreviousPressedKeys.Contains(Keys.X) && _player.CanCreateFire())
        {
            _player.CreateFire(10);
            Game.SoundMap["light_fire"].Play();
            Campfires.Add(new CampFire(this, new Vector2(_player.Position.X + 20, _player.Position.Y - 20)));
        }

        if (oldMapIndex != MapIndex)
        {
            _maps.RemoveAt(0);
            AddRandomMap();
        }

        PreviousPressedKeys.Clear();
        PreviousPressedKeys.UnionWith(keyState.GetPressedKeys());
        _debugData[1] = $"Translation: {Game.Camera.GetViewMatrix().Translation}";
        _debugData[2] = $"Map Index: {MapIndex}";
    }

    public override void Draw(GameTime gameTime)
    {
        Game.Penumbra.BeginDraw();
        Game.Penumbra.Transform = Matrix.CreateTranslation(-Game.Camera.Position.X, -Game.Camera.Position.Y, 0);
        Game.GraphicsDevice.Clear(BgColor);
        var transformMatrix = Game.Camera.GetViewMatrix();

        for (var i = 0; i < _maps.Count; ++i)
        {
            Game.TiledMapRenderer.LoadMap(_maps[i]);
            Game.TiledMapRenderer.Draw(
                transformMatrix * Matrix.CreateTranslation(_maps[i].WidthInPixels * (i + MapIndex - 1), 0, 0));
        }

        Game.SpriteBatch.Begin(transformMatrix: transformMatrix, samplerState: SamplerState.PointClamp);

        Pickupables.ForEach(pickupable => pickupable.Draw(Game.SpriteBatch));
        Campfires.ForEach(campfire => campfire.Draw(Game.SpriteBatch));
        GroundEntities.ForEach(groundEntity => groundEntity.Draw(Game.SpriteBatch));

        _companion.Draw(Game.SpriteBatch);
        _player.Draw(Game.SpriteBatch);

        Game.SpriteBatch.End();

        Game.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        _darknessParticles.Draw(Game.SpriteBatch);

        Game.SpriteBatch.End();

        Game.Penumbra.Draw(gameTime);

        DrawUI(gameTime);
    }

    private void DrawUI(GameTime gameTime)
    {
        var transformMatrix = Game.Camera.GetViewMatrix();
        Game.SpriteBatch.Begin(transformMatrix: transformMatrix, samplerState: SamplerState.PointClamp);
        _player.DrawPopupButton(Game.SpriteBatch);
        Campfires.ForEach(campfire => campfire.DrawUI(Game.SpriteBatch));
        Game.SpriteBatch.End();

        Game.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        _followerAttribute.Draw(Game.SpriteBatch);
        Game.SpriteBatch.DrawString(Game.FontMap["32"], "Comfy meter",
            new Vector2(Game.GetWindowWidth() / 2 - 350, Game.GetWindowHeight() - 100), Color.Orange);
        _inventory.Draw(Game.SpriteBatch);

        if (gameTime.TotalGameTime.Subtract(_fpsCounterGameTime).Milliseconds >= 500)
        {
            _fps = (int)(1 / gameTime.ElapsedGameTime.TotalSeconds);
            _fpsCounterGameTime = gameTime.TotalGameTime;
            _debugData[0] = $"FPS: {_fps}";
        }

        else if (_pauseState)
        {
            _pauseButton.Draw(Game.SpriteBatch);
            _menuButton.Draw(Game.SpriteBatch);
        }
        else if (_deathState)
        {
            var finalScore = (int)Math.Ceiling(_score / 10);
            Game.SpriteBatch.DrawOutlinedString(Game.FontMap["32"], "Your companion got too anxious!",
                new Vector2(Game.GetWindowWidth() / 2F, Game.GetWindowHeight() / 3F - 60F), Color.Red, Color.Black,
                Utils.HorizontalFontAlignment.Center, Utils.VerticalFontAlignment.Center);
            Game.SpriteBatch.DrawOutlinedString(Game.FontMap["24"], $"Final score {finalScore}",
                new Vector2(Game.GetWindowWidth() / 2F, Game.GetWindowHeight() / 3F), Color.Red, Color.Black,
                Utils.HorizontalFontAlignment.Center, Utils.VerticalFontAlignment.Center);
            _restartButton.Draw(Game.SpriteBatch);
            _exitButton.Draw(Game.SpriteBatch);
        }

        if (_showDebug)
        {
            for (var i = 0; i < _debugData.Count; ++i)
            {
                Game.SpriteBatch.DrawString(Game.FontMap["16"], _debugData[i],
                    new Vector2(16, 16 * (i + 1)), Color.White);
            }
        }

        Game.SpriteBatch.End();
    }
}