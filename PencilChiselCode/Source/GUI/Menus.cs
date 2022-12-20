using System;
using Microsoft.Xna.Framework;

namespace PencilChiselCode.Source.GUI;

public static class Menus
{
    public static Box GetSettingsMenu(Bonfire game, Action onDoneClick)
    {
        var page = 0;
        var vSync = game.IsVSyncEnabled;
        var windowMode = game.GetWindowMode();
        const float categoriesSize = 0.25F;
        var settingsMenu = new Box(game, new Vector2(0), new Vector2(0.66F))
        {
            IsPositionAbsolute = true,
            BoxAlignment = Alignments.MiddleCenter,
            SelfAlignment = Alignments.MiddleCenter
        };
        var menuCategories = new Box(game, new Vector2(0), new Vector2(categoriesSize, 1F))
        {
            BoxAlignment = Alignments.MiddleLeft,
            SelfAlignment = Alignments.MiddleLeft
        };
        menuCategories.AddChild(
            new Box(
                game,
                new Vector2(16),
                new Button(
                    new UiTextElement(game.FontMap["32"], () => "Video", Color.White, Color.Black),
                    new UiTextElement(game.FontMap["32"], () => "Video", Color.Red, Color.Black),
                    new UiTextElement(game.FontMap["32"], () => "Video", Color.Green, Color.Black),
                    game.SoundMap["button_press"],
                    game.SoundMap["button_release"],
                    () => page = 0
                )
            )
            {
                IsPositionAbsolute = true,
                IsSizeAbsolute = true
            },
            new Box(
                game,
                new Vector2(16, 64),
                new Button(
                    new UiTextElement(game.FontMap["32"], () => "Audio", Color.White, Color.Black),
                    new UiTextElement(game.FontMap["32"], () => "Audio", Color.Red, Color.Black),
                    new UiTextElement(game.FontMap["32"], () => "Audio", Color.Green, Color.Black),
                    game.SoundMap["button_press"],
                    game.SoundMap["button_release"],
                    () => page = 1
                )
            )
            {
                IsPositionAbsolute = true,
                IsSizeAbsolute = true
            },
            new Box(
                game,
                new Vector2(16, 112),
                new Button(
                    new UiTextElement(
                        game.FontMap["32"],
                        () => "Controls",
                        Color.White,
                        Color.Black
                    ),
                    new UiTextElement(game.FontMap["32"], () => "Controls", Color.Red, Color.Black),
                    new UiTextElement(
                        game.FontMap["32"],
                        () => "Controls",
                        Color.Green,
                        Color.Black
                    ),
                    game.SoundMap["button_press"],
                    game.SoundMap["button_release"],
                    () => page = 2
                )
            )
            {
                IsPositionAbsolute = true,
                IsSizeAbsolute = true
            },
            new Box(
                game,
                new Vector2(16),
                new Button(
                    new UiTextElement(game.FontMap["32"], () => "Done", Color.White, Color.Black),
                    new UiTextElement(game.FontMap["32"], () => "Done", Color.Red, Color.Black),
                    new UiTextElement(game.FontMap["32"], () => "Done", Color.Green, Color.Black),
                    game.SoundMap["button_press"],
                    game.SoundMap["button_release"],
                    () =>
                    {
                        game.SetVSync(vSync);
                        game.SetWindowMode(windowMode);
                        onDoneClick();
                    })
            )
            {
                IsPositionAbsolute = true,
                IsSizeAbsolute = true,
                BoxAlignment = Alignments.BottomLeft,
                SelfAlignment = Alignments.BottomLeft
            }
        );
        settingsMenu.AddChild(menuCategories);
        var videoMenu = new Box(
            game,
            new Vector2(categoriesSize, 0F),
            new Vector2(1 - categoriesSize, 1F)
        )
        {
            BoxAlignment = Alignments.MiddleLeft,
            SelfAlignment = Alignments.MiddleLeft,
            IsVisible = () => page == 0
        };
        var vSyncCheckbox = new Box(
            game,
            Vector2.Zero,
            new Checkbox(
                new UiTextureElement(game.TextureMap["checkbox_selected"]) { Color = Color.Salmon },
                new UiTextureElement(game.TextureMap["checkbox_empty"]) { Color = Color.Red },
                game.SoundMap["button_release"],
                game.SoundMap["button_press"],
                enabled => vSync = enabled,
                game.IsVSyncEnabled
            )
        )
        {
            IsPositionAbsolute = true,
            IsSizeAbsolute = true,
            BoxAlignment = Alignments.MiddleLeft,
            SelfAlignment = Alignments.MiddleLeft
        };
        var vSyncText = new Box(
            game,
            new Vector2(vSyncCheckbox.Size().X + 4, 0),
            new UiTextElement(
                game.FontMap["24"],
                () => $"VSync: {(vSync ? ": On" : ": Off")}",
                Color.LimeGreen,
                Color.Green
            )
        )
        {
            IsPositionAbsolute = true,
            IsSizeAbsolute = true,
            BoxAlignment = Alignments.MiddleLeft,
            SelfAlignment = Alignments.MiddleLeft
        };

        var vSyncBox = new Box(
            game,
            new Vector2(16, 48),
            new Vector2(vSyncText.Size().X + vSyncCheckbox.Size().X + 16, 48)
        )
        {
            IsPositionAbsolute = true,
            IsSizeAbsolute = true
        };
        vSyncBox.AddChild(vSyncCheckbox, vSyncText);

        var fullscreenButton = new Box(
            game,
            new Vector2(16, 80),
            new Button(
                new UiTextElement(
                    game.FontMap["24"],
                    () => $"Fullscreen: {windowMode}",
                    Color.White,
                    Color.Black
                ),
                new UiTextElement(
                    game.FontMap["24"],
                    () => $"Fullscreen: {windowMode}",
                    Color.GreenYellow,
                    Color.Black
                ),
                new UiTextElement(
                    game.FontMap["24"],
                    () => $"Fullscreen: {windowMode}",
                    Color.OliveDrab,
                    Color.Black
                ),
                game.SoundMap["button_release"],
                game.SoundMap["button_press"],
                () => windowMode.ShiftWindowMode()
            )
        )
        {
            IsPositionAbsolute = true,
            IsSizeAbsolute = true,
            BoxAlignment = Alignments.MiddleLeft,
            SelfAlignment = Alignments.MiddleLeft
        };
        videoMenu.AddChild(
            new Box(
                game,
                new Vector2(16),
                new UiTextElement(
                    game.FontMap["24"],
                    () => "Resolution",
                    Color.LimeGreen,
                    Color.Green
                )
            )
            {
                IsPositionAbsolute = true,
                IsSizeAbsolute = true
            },
            vSyncBox,
            fullscreenButton
        );
        var soundMenu = new Box(
            game,
            new Vector2(categoriesSize, 0F),
            new Vector2(1 - categoriesSize, 1F)
        )
        {
            BoxAlignment = Alignments.MiddleLeft,
            SelfAlignment = Alignments.MiddleLeft,
            IsVisible = () => page == 1
        };
        soundMenu.AddChild(
            new Box(
                game,
                new Vector2(16),
                new UiTextElement(
                    game.FontMap["24"],
                    () => "Master Volume",
                    Color.LimeGreen,
                    Color.Green
                )
            )
            {
                IsPositionAbsolute = true,
                IsSizeAbsolute = true
            },
            new Box(
                game,
                new Vector2(16, 48),
                new UiTextElement(game.FontMap["24"], () => "Music", Color.LimeGreen, Color.Green)
            )
            {
                IsPositionAbsolute = true,
                IsSizeAbsolute = true
            },
            new Box(
                game,
                new Vector2(16, 80),
                new UiTextElement(game.FontMap["24"], () => "SFX", Color.LimeGreen, Color.Green)
            )
            {
                IsPositionAbsolute = true,
                IsSizeAbsolute = true
            }
        );
        var controlsMenu = new Box(
            game,
            new Vector2(categoriesSize, 0F),
            new Vector2(1 - categoriesSize, 1F)
        )
        {
            BoxAlignment = Alignments.MiddleLeft,
            SelfAlignment = Alignments.MiddleLeft,
            IsVisible = () => page == 2
        };
        controlsMenu.AddChild(
            new Box(
                game,
                new Vector2(16),
                new UiTextElement(game.FontMap["24"], () => "Collect", Color.LimeGreen, Color.Green)
            )
            {
                IsPositionAbsolute = true,
                IsSizeAbsolute = true
            },
            new Box(
                game,
                new Vector2(16, 48),
                new UiTextElement(game.FontMap["24"], () => "Move Up", Color.LimeGreen, Color.Green)
            )
            {
                IsPositionAbsolute = true,
                IsSizeAbsolute = true
            },
            new Box(
                game,
                new Vector2(16, 80),
                new UiTextElement(
                    game.FontMap["24"],
                    () => "Move Down",
                    Color.LimeGreen,
                    Color.Green
                )
            )
            {
                IsPositionAbsolute = true,
                IsSizeAbsolute = true
            },
            new Box(
                game,
                new Vector2(16, 112),
                new UiTextElement(
                    game.FontMap["24"],
                    () => "Move Left",
                    Color.LimeGreen,
                    Color.Green
                )
            )
            {
                IsPositionAbsolute = true,
                IsSizeAbsolute = true
            },
            new Box(
                game,
                new Vector2(16, 144),
                new UiTextElement(
                    game.FontMap["24"],
                    () => "Move Right",
                    Color.LimeGreen,
                    Color.Green
                )
            )
            {
                IsPositionAbsolute = true,
                IsSizeAbsolute = true
            }
        );
        settingsMenu.AddChild(videoMenu, soundMenu, controlsMenu);
        return settingsMenu;
    }
}
