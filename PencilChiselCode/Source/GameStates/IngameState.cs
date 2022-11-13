using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;

namespace PencilChiselCode.Source.GameStates;

public class IngameState : GameScreen
{
    public static readonly Color BgColor = Color.Green;
    private Game1 _game => (Game1)Game;
    private Player _player;
    private Companion _companion;
    private bool _showDebug;
    public readonly HashSet<Keys> PreviousPressedKeys = new();
    private static float _cameraSpeed = 10.0F;
    private AttributeGroup _followerAttributes;
    private int _fps;
    private TimeSpan _fpsCounterGameTime;
    private TimeSpan _twigCounterGameTime;
    private static bool _pauseState;
    private Button _pauseButton;
    private Button _exitButton;
    private int _twigCount = 5;

    public IngameState(Game game) : base(game)
    {
    }

    public List<Pickupable> Pickupables { get; } = new();
    public List<CampFire> Campfires { get; } = new();

    public override void LoadContent()
    {
        base.LoadContent();
        for (int i = 0; i < _twigCount; i++)
        {
            Pickupables.Add(new Pickupable(PickupableTypes.Twig,
                _game.TextureMap["twigs"],
                _game.SoundMap["pickup_branches"],
                new Vector2(Utils.GetRandomInt((int)_game.Camera.Position.X,_game.GetWindowWidth()), 
                    Utils.GetRandomInt(10,_game.GetWindowHeight()-10)),
                Utils.RANDOM.NextAngle()));    
        }
        for (int i = 0; i < 10; i++)
        {
            Pickupables.Add(new Pickupable(PickupableTypes.Bush,
                _game.TextureMap["bush_berry"],
                _game.SoundMap["pickup_branches"],
                new Vector2(Utils.GetRandomInt((int)_game.Camera.Position.X,_game.GetWindowWidth()),
                    Utils.GetRandomInt(10,_game.GetWindowHeight()-10)),
                Utils.RANDOM.NextAngle()));
        }
        var resumeButton = _game.TextureMap["resume_button_normal"];
        var resumeButtonSize = new Size2(resumeButton.Width, resumeButton.Height);
        _pauseButton = new Button(resumeButton,
            _game.TextureMap["resume_button_hover"],
            _game.TextureMap["resume_button_pressed"],
            Utils.GetCenterStartCoords(resumeButtonSize, Game1.Instance.GetWindowDimensions()),
            () => { _pauseState = false; }
        );
        var exitButton = _game.TextureMap["exit_button_normal"];
        var exitButtonSize = new Size2(exitButton.Width, exitButton.Height);
        _exitButton = new Button(exitButton,
            _game.TextureMap["exit_button_hover"],
            _game.TextureMap["exit_button_pressed"],
            Utils.GetCenterStartCoords(exitButtonSize, Game1.Instance.GetWindowDimensions()) + Vector2.UnitY * 100,
            () => _game.Exit()
        );
        
        _companion = new Companion(_game, new Vector2(100, 100), 50F);
        _player = new Player(_game, new Vector2(150, 150));

        Campfires.Add(new CampFire(_game, new Vector2(500, 400))); // TEMP

        _followerAttributes = new AttributeGroup(new List<Attribute>
        {
            new(new(10, 10), new(100, 10), Color.Brown, 100, -0.5F),
            new(new(10, 30), new(100, 10), Color.LightBlue, 100, -1F),
            new(new(10, 50), new(100, 10), Color.Orange, 100, -5F),
            new(new(10, 70), new(100, 10), Color.Blue, 100, -2F)
        });
    }
    
    public void RandomBushSpawner(PickupableTypes type)
    {
        if (Utils.GetRandomInt(0, 101) < 10)
        {
            var pickupable = new Pickupable(type,
                _game.TextureMap["bush_berry"],
                _game.SoundMap["pickup_branches"],
                new Vector2(_game.Camera.Position.X + _game.GetWindowWidth() + 10,Utils.GetRandomInt(5,_game.GetWindowHeight())),
                Utils.RANDOM.NextAngle());
            Pickupables.Add(pickupable);
        }
    }
    public void RandomTwigSpawner(PickupableTypes type)
    {
        if (Utils.GetRandomInt(0, 101) < 10)
        {
            var pickupable = new Pickupable(type,
                _game.TextureMap["twigs"],
                _game.SoundMap["pickup_branches"],
                new Vector2(_game.Camera.Position.X + _game.GetWindowWidth() + 10,Utils.GetRandomInt(5,_game.GetWindowHeight())),
                Utils.RANDOM.NextAngle());
            Pickupables.Add(pickupable);
        }
    }

    public override void Update(GameTime gameTime)
    {
        if (gameTime.TotalGameTime.Subtract(_twigCounterGameTime).Milliseconds >= 500)
        {
            RandomTwigSpawner(PickupableTypes.Twig);
            RandomBushSpawner(PickupableTypes.Bush);
            _twigCounterGameTime = gameTime.TotalGameTime;
        }
        _game.TiledMapRenderer.Update(gameTime);
        var keyState = Keyboard.GetState();
        if (keyState.IsKeyDown(Keys.Escape) && !PreviousPressedKeys.Contains(Keys.Escape))
        {
            _pauseState = !_pauseState;
        }

        if (_pauseState)
        {
            _pauseButton.Update(gameTime);
            _exitButton.Update(gameTime);
        }
        else
        {
            _game.Camera.Move(Vector2.UnitX * _cameraSpeed * gameTime.GetElapsedSeconds());
            _companion.Update(this, gameTime, _player.Position);
            _player.Update(this, gameTime);
            _followerAttributes.Update(gameTime);
            Pickupables.ForEach(pickupable => pickupable.Update(gameTime));
            Campfires.ForEach(campfire =>
            {
                campfire.Update(gameTime);
            });
            if (Campfires.Any(campfire => campfire.IsInRange(_companion.Position)))
            {
                _followerAttributes.Attributes[2].ChangeValue(10F * gameTime.GetElapsedSeconds());
            }
        }

        if (keyState.IsKeyDown(Keys.F3) && !PreviousPressedKeys.Contains(Keys.F3))
        {
            _showDebug = !_showDebug;
        }

        if (keyState.IsKeyDown(Keys.Space) && !PreviousPressedKeys.Contains(Keys.Space))
        {
            Debug.WriteLine("STOP");
            _companion.StopResumeFollower();
        }

        PreviousPressedKeys.Clear();
        PreviousPressedKeys.UnionWith(keyState.GetPressedKeys());
    }

    public override void Draw(GameTime gameTime)
    {
        _game.Penumbra.BeginDraw();

        _game.Penumbra.Transform = Matrix.CreateTranslation(-_game.Camera.Position.X, -_game.Camera.Position.Y, 0);
        _game.GraphicsDevice.Clear(BgColor);
        var transformMatrix = _game.Camera.GetViewMatrix();

        _game.TiledMapRenderer.Draw(transformMatrix);
        _game.TiledMapRenderer.Draw(transformMatrix * Matrix.CreateTranslation(768, 0, 0));

        _game.SpriteBatch.Begin(transformMatrix: transformMatrix, samplerState: SamplerState.PointClamp);

        Pickupables.ForEach(pickupable => pickupable.Draw(_game.SpriteBatch));

        _companion.Draw(_game.SpriteBatch);
        _player.Draw(_game.SpriteBatch);

        Campfires.ForEach(campfire => campfire.Draw(_game.SpriteBatch)); // TEMP

        _game.SpriteBatch.End();
        _game.Penumbra.Draw(gameTime);

        DrawUI(gameTime);
    }

    private void DrawUI(GameTime gameTime)
    {
        var transformMatrix = _game.Camera.GetViewMatrix();
        _game.SpriteBatch.Begin(transformMatrix: transformMatrix, samplerState: SamplerState.PointClamp);
        _player.DrawPopupButton(_game.SpriteBatch);
        Campfires.ForEach(campfire => campfire.DrawUI(_game.SpriteBatch)); // TEMP
        _game.SpriteBatch.End();

        _game.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        _followerAttributes.Draw(_game.SpriteBatch);
        if (gameTime.TotalGameTime.Subtract(_fpsCounterGameTime).Milliseconds >= 500)
        {
            _fps = (int)(1 / gameTime.ElapsedGameTime.TotalSeconds);
            _fpsCounterGameTime = gameTime.TotalGameTime;
        }

        if (_pauseState)
        {
            _pauseButton.Draw(_game.SpriteBatch);
            _exitButton.Draw(_game.SpriteBatch);
        }

        if (_showDebug)
        {
            _game.SpriteBatch.DrawString(_game.FontMap["16"], $"FPS: {_fps}",
                new Vector2(16, 16), Color.Black);
        }

        _game.SpriteBatch.End();
    }
}