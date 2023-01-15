using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Screens.Transitions;
using MonoGame.Extended.Tweening;
using PencilChiselCode.Source.GUI;
using PencilChiselCode.Source.Objects;

namespace PencilChiselCode.Source.GameStates;

public class IngameState : BonfireGameState
{
    public Player Player { get; private set; }
    public Companion Companion;
    private float _voidSpeed = 40F;
    private int _fps;
    private TimeSpan _fpsCounterGameTime;
    private bool _pauseState;
    private List<Chunk> _chunks;
    private ParticleGenerator _darknessParticles;
    private readonly List<string> _debugData = new() { "", "", "", "" };
    private bool _deathState;
    private Song _song;
    private float _score;
    public const int DarknessEndOffset = 64;
    public OrthographicCamera Camera { get; private set; }
    public Tweener DaytimeTweener { get; private set; }
    public float Daytime { get; set; }
    public float InverseDaytime => 1F - Daytime;
    public RootBox RootBox;

    public IngameState(Game game) : base(game) { }

    public void Cleanup()
    {
        Campfires.ClearAll();
        Player.Cleanup();
    }

    public IEnumerable<Pickupable> Pickupables
    {
        get
        {
            var pickupables = new EntityCollection<Pickupable>();
            foreach (var chunk in _chunks)
            {
                pickupables.AddRange(chunk.Pickupables);
            }
            return pickupables;
        }
    }
    public EntityCollection<GroundEntity> GroundEntities
    {
        get
        {
            var groundEntities = new EntityCollection<GroundEntity>();
            foreach (var chunk in _chunks)
            {
                groundEntities.AddRange(chunk.GroundEntities);
            }
            return groundEntities;
        }
    }

    public EntityCollection<CampFire> Campfires { get; } = new();

    public override void LoadContent()
    {
        Camera = new OrthographicCamera(Game.GraphicsDevice);
        _deathState = false;
        base.LoadContent();

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

        _chunks = new();
        for (var i = 0; i < 3; ++i)
        {
            CreateChunk();
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
            () => Daytime * 100F
        );
        var inventoryBox = Menus.GetInventory(Game, Player);
        inventoryBox.IsVisible = () => _pauseState;
        RootBox.AddChild(inventoryBox);
        _song = Game.SongMap["bonfire_song"];
        MediaPlayer.Play(_song);
        MediaPlayer.IsRepeating = true;

        DaytimeTweener = new Tweener();
        DaytimeTweener
            .TweenTo(
                target: this,
                expression: state => state.Daytime,
                toValue: 1F,
                duration: 100F,
                delay: 10F
            )
            .RepeatForever(repeatDelay: 10F)
            .AutoReverse()
            .Easing(EasingFunctions.SineInOut);
    }

    private void CreateChunk() => _chunks.Add(new Chunk(this, _chunks.Count));

    public override void Update(GameTime gameTime)
    {
        if (!_deathState && Game.Controls.JustPressed(ControlKeys.Pause))
        {
            _pauseState = !_pauseState;
        }

        RootBox.Update(gameTime);

        if (_deathState || _pauseState)
            return;

        var oldMapIndex = _chunks[0].MapIndex;

        DaytimeTweener.Update(gameTime.GetElapsedSeconds());
        Game.Penumbra.AmbientColor = new Color(InverseDaytime, InverseDaytime, InverseDaytime);

        Game.TiledMapRenderer.Update(gameTime);

        if (Companion.IsAnxious())
        {
            _deathState = true;
            return;
        }

        Camera.Move(
            Vector2.UnitX
                * _voidSpeed
                * (float)Math.Sqrt(1 - (Daytime - 1) * (Daytime - 1))
                * gameTime.GetElapsedSeconds()
        );
        Companion.Update(gameTime, Player.Position);
        Player.Update(gameTime);

        _chunks.ForEach(chunk => chunk.Update(gameTime));
        Campfires.Update(gameTime);

        _darknessParticles.Update(gameTime, true);
        _score += gameTime.ElapsedGameTime.Milliseconds;

        if (oldMapIndex != _chunks[0].MapIndex)
        {
            _chunks[0].Cleanup();
            _chunks.RemoveAt(0);
            CreateChunk();
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
        _debugData[2] = $"Map Index: {_chunks[0].MapIndex}";
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

        for (var i = 0; i < _chunks.Count; ++i)
        {
            _chunks[i].Draw(gameTime, transformMatrix, i);
        }

        Game.SpriteBatch.Begin(
            transformMatrix: transformMatrix,
            samplerState: SamplerState.PointClamp
        );

        _chunks.ForEach(chunk => chunk.DrawObjects(gameTime));
        Campfires.ForEach(campfire => campfire.Draw(Game.SpriteBatch));

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
            _debugData[3] = ((Daytime * 24 + 6) % 24).ToString("0.00");
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
