using System;
using Microsoft.Xna.Framework;

namespace PencilChiselCode.Source.GUI;

public static class Menus
{
    public static RelativeBox GetInventory(Bonfire game, Player player)
    {
        var eKey = new UiTextureElement(game.TextureMap["e_key"]);
        var twig = new UiTextureElement(game.TextureMap["twig"]);
        var twigCount = new UiTextElement(
            game.FontMap["32"],
            () => player.Inventory[PickupableTypes.Twig].ToString(),
            Color.White,
            Color.Black
        );
        var berry = new UiTextureElement(game.TextureMap["berry"]);
        var berryCount = new UiTextElement(
            game.FontMap["32"],
            () => player.Inventory[PickupableTypes.BerryBush].ToString(),
            Color.White,
            Color.Black
        );
        var xKey = new UiTextureElement(game.TextureMap["x_key"]);
        var campfire = new UiTextureElement(game.TextureMap["campfire"]);
        var equals = new UiTextElement(game.FontMap["32"], () => "=", Color.White, Color.Black);
        var campfireTwigCost = new UiTextElement(
            game.FontMap["32"],
            () => "10 x",
            Color.White,
            Color.Black
        );
        var fKey = new UiTextureElement(game.TextureMap["f_key"]);
        var plus = new UiTextElement(game.FontMap["32"], () => "+", Color.White, Color.Black);
        var campfireRefuelCost = new UiTextElement(
            game.FontMap["32"],
            () => "2 x",
            Color.White,
            Color.Black
        );
        var qKey = new UiTextureElement(game.TextureMap["q_key"]);
        var companion = new UiTextureElement(game.TextureMap["follower"]);
        var spaceKey = new UiTextureElement(game.TextureMap["space_key"]);
        var followStop = new UiTextElement(
            game.FontMap["32"],
            () => "follow/stop",
            Color.White,
            Color.Black
        );

        var inventoryBox = new RelativeBox(game, (0, 50), (300, 300))
        {
            BoxAlignment = Alignments.TopRight,
            SelfAlignment = Alignments.TopRight
        };
        var twigCountBox = new RelativeBox(game, 0, (300, 50))
        {
            BoxAlignment = Alignments.TopRight,
            SelfAlignment = Alignments.TopRight
        }.WithChild(
            new RelativeBox(game, 0, (new Ratio(2F), 1F))
            {
                BoxAlignment = Alignments.MiddleRight,
                SelfAlignment = Alignments.MiddleRight,
                DrawableElement = eKey,
                Padding = (0.65F, 0.3F)
            },
            new RelativeBox(game, 0, (new Ratio(1F), 1F))
            {
                BoxAlignment = Alignments.LeftOfPrevious,
                DrawableElement = twig,
                Padding = 0.3F
            },
            new RelativeBox(game, 0, (new Ratio(0.5F), 1F))
            {
                BoxAlignment = Alignments.LeftOfPrevious
            },
            new RelativeBox(game, 0, (new FitElement(), 1F))
            {
                BoxAlignment = Alignments.LeftOfPrevious,
                DrawableElement = twigCount,
                Padding = 0.3F
            }
        );
        var berryCountBox = new RelativeBox(game, 0, (300, 50))
        {
            BoxAlignment = Alignments.BelowPrevious,
        }.WithChild(
            new RelativeBox(game, 0, (new Ratio(2F), 1F))
            {
                BoxAlignment = Alignments.MiddleRight,
                SelfAlignment = Alignments.MiddleRight,
                DrawableElement = eKey,
                Padding = (0.65F, 0.3F)
            },
            new RelativeBox(game, 0, (new Ratio(1F), 1F))
            {
                BoxAlignment = Alignments.LeftOfPrevious,
                DrawableElement = berry,
                Padding = 0.3F
            },
            new RelativeBox(game, 0, (new Ratio(0.5F), 1F))
            {
                BoxAlignment = Alignments.LeftOfPrevious,
            },
            new RelativeBox(game, 0, (new FitElement(), 1F))
            {
                BoxAlignment = Alignments.LeftOfPrevious,
                DrawableElement = berryCount,
                Padding = 0.3F
            }
        );
        var campfireTwigCostBox = new RelativeBox(game, 0, (300, 50))
        {
            BoxAlignment = Alignments.BelowPrevious,
        }.WithChild(
            new RelativeBox(game, 0, (new Ratio(2F), 1F))
            {
                BoxAlignment = Alignments.MiddleRight,
                SelfAlignment = Alignments.MiddleRight,
                DrawableElement = xKey,
                Padding = (0.65F, 0.3F)
            },
            new RelativeBox(game, 0, (new Ratio(1F), 1F))
            {
                BoxAlignment = Alignments.LeftOfPrevious,
                DrawableElement = campfire,
                Padding = 0.3F
            },
            new RelativeBox(game, 0, (new FitElement(), 1F))
            {
                BoxAlignment = Alignments.LeftOfPrevious,
                DrawableElement = equals,
                Padding = 0.3F
            },
            new RelativeBox(game, 0, (new Ratio(1F), 1F))
            {
                BoxAlignment = Alignments.LeftOfPrevious,
                DrawableElement = twig,
                Padding = 0.3F
            },
            new RelativeBox(game, 0, (new FitElement(), 1F))
            {
                BoxAlignment = Alignments.LeftOfPrevious,
                DrawableElement = campfireTwigCost,
                Padding = 0.3F
            }
        );
        var campfireRefuelCostBox = new RelativeBox(game, 0, (300, 50))
        {
            BoxAlignment = Alignments.BelowPrevious,
        }.WithChild(
            new RelativeBox(game, 0, (new Ratio(2F), 1F))
            {
                BoxAlignment = Alignments.MiddleRight,
                SelfAlignment = Alignments.MiddleRight,
                DrawableElement = fKey,
                Padding = (0.65F, 0.3F)
            },
            new RelativeBox(game, 0, (new Ratio(1F), 1F))
            {
                BoxAlignment = Alignments.LeftOfPrevious,
                DrawableElement = campfire,
                Padding = 0.3F
            },
            new RelativeBox(game, 0, (new FitElement(), 1F))
            {
                BoxAlignment = Alignments.LeftOfPrevious,
                DrawableElement = plus,
                Padding = 0.3F
            },
            new RelativeBox(game, 0, (new Ratio(1F), 1F))
            {
                BoxAlignment = Alignments.LeftOfPrevious,
                DrawableElement = twig,
                Padding = 0.3F
            },
            new RelativeBox(game, 0, (new FitElement(), 1F))
            {
                BoxAlignment = Alignments.LeftOfPrevious,
                DrawableElement = campfireRefuelCost,
                Padding = 0.3F
            }
        );
        var companionFeedBox = new RelativeBox(game, 0, (300, 50))
        {
            BoxAlignment = Alignments.BelowPrevious,
        }.WithChild(
            new RelativeBox(game, 0, (new Ratio(2F), 1F))
            {
                BoxAlignment = Alignments.MiddleRight,
                SelfAlignment = Alignments.MiddleRight,
                DrawableElement = qKey,
                Padding = (0.65F, 0.3F)
            },
            new RelativeBox(game, 0, (new Ratio(1F), 1F))
            {
                BoxAlignment = Alignments.LeftOfPrevious,
                DrawableElement = companion,
                Padding = 0.3F
            },
            new RelativeBox(game, 0, (new FitElement(), 1F))
            {
                BoxAlignment = Alignments.LeftOfPrevious,
                DrawableElement = plus,
                Padding = 0.3F
            },
            new RelativeBox(game, 0, (new Ratio(1F), 1F))
            {
                BoxAlignment = Alignments.LeftOfPrevious,
                DrawableElement = berry,
                Padding = 0.3F
            }
        );
        var followStopBox = new RelativeBox(game, 0, (300, 50))
        {
            BoxAlignment = Alignments.BelowPrevious,
        }.WithChild(
            new RelativeBox(game, 0, (new FitElement(), 1F))
            {
                BoxAlignment = Alignments.MiddleRight,
                SelfAlignment = Alignments.MiddleRight,
                DrawableElement = spaceKey,
                Padding = 0.3F
            },
            new RelativeBox(game, 0, (new Ratio(1F), 1F))
            {
                BoxAlignment = Alignments.LeftOfPrevious,
                DrawableElement = companion,
                Padding = 0.3F
            },
            new RelativeBox(game, 0, (new FitElement(), 1F))
            {
                BoxAlignment = Alignments.LeftOfPrevious,
                DrawableElement = followStop,
                Padding = 0.3F
            }
        );

        inventoryBox.AddChild(
            twigCountBox,
            berryCountBox,
            campfireTwigCostBox,
            campfireRefuelCostBox,
            companionFeedBox,
            followStopBox
        );
        return inventoryBox;
    }

    public static RelativeBox GetSettingsMenu(Bonfire game, Action onDoneClick)
    {
        var page = 0;
        var vSync = game.IsVSyncEnabled;
        var windowMode = game.GetWindowMode();
        const float categoriesSize = 0.25F;
        var settingsMenu = new RelativeBox(game, 0, 0.66F)
        {
            BoxAlignment = Alignments.MiddleCenter,
            SelfAlignment = Alignments.MiddleCenter
        };
        var menuCategories = new RelativeBox(game, 0, (categoriesSize, 1F))
        {
            BoxAlignment = Alignments.MiddleLeft,
            SelfAlignment = Alignments.MiddleLeft,
            Gap = 32
        };
        var videoElement = new Button(
            new UiTextElement(game.FontMap["32"], () => "Video", Color.White, Color.Black),
            new UiTextElement(game.FontMap["32"], () => "Video", Color.Red, Color.Black),
            new UiTextElement(game.FontMap["32"], () => "Video", Color.Green, Color.Black),
            game.SoundMap["button_press"],
            game.SoundMap["button_release"],
            () => page = 0
        );
        var audioElement = new Button(
            new UiTextElement(game.FontMap["32"], () => "Audio", Color.White, Color.Black),
            new UiTextElement(game.FontMap["32"], () => "Audio", Color.Red, Color.Black),
            new UiTextElement(game.FontMap["32"], () => "Audio", Color.Green, Color.Black),
            game.SoundMap["button_press"],
            game.SoundMap["button_release"],
            () => page = 1
        );
        var controlsElement = new Button(
            new UiTextElement(game.FontMap["32"], () => "Controls", Color.White, Color.Black),
            new UiTextElement(game.FontMap["32"], () => "Controls", Color.Red, Color.Black),
            new UiTextElement(game.FontMap["32"], () => "Controls", Color.Green, Color.Black),
            game.SoundMap["button_press"],
            game.SoundMap["button_release"],
            () => page = 2
        );
        var doneElement = new Button(
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
            }
        );

        menuCategories.WithChild(
            new RelativeBox(game, 16, videoElement.Size()) { DrawableElement = videoElement },
            new RelativeBox(game, (0, 8), audioElement.Size())
            {
                BoxAlignment = Alignments.BelowPrevious,
                DrawableElement = audioElement
            },
            new RelativeBox(game, (0, 8), controlsElement.Size())
            {
                BoxAlignment = Alignments.BelowPrevious,
                DrawableElement = controlsElement
            },
            new RelativeBox(game, (16, -16), doneElement.Size())
            {
                BoxAlignment = Alignments.BottomLeft,
                SelfAlignment = Alignments.BottomLeft,
                DrawableElement = doneElement
            }
        );
        settingsMenu.AddChild(menuCategories);
        var videoMenu = new RelativeBox(game, (categoriesSize, 0F), (1F - categoriesSize, 1F))
        {
            BoxAlignment = Alignments.MiddleLeft,
            SelfAlignment = Alignments.MiddleLeft,
            IsVisible = () => page == 0
        };

        string FullscreenButtonText() => $"Fullscreen: {windowMode}";
        var fullscreenButton = new Button(
            new UiTextElement(game.FontMap["24"], FullscreenButtonText, Color.White, Color.Black),
            new UiTextElement(
                game.FontMap["24"],
                FullscreenButtonText,
                Color.GreenYellow,
                Color.Black
            ),
            new UiTextElement(
                game.FontMap["24"],
                FullscreenButtonText,
                Color.OliveDrab,
                Color.Black
            ),
            game.SoundMap["button_release"],
            game.SoundMap["button_press"],
            () => windowMode.ShiftWindowMode()
        );
        var resolutionElement = new UiTextElement(
            game.FontMap["24"],
            () => "Resolution",
            Color.LimeGreen,
            Color.Green
        );
        var vSyncCheckbox = new Checkbox(
            new Button(
                new UiTextureElement(game.TextureMap["checkbox_selected"])
                {
                    Color = Color.LimeGreen
                },
                new UiTextureElement(game.TextureMap["checkbox_selected"])
                {
                    Color = Color.GreenYellow
                },
                new UiTextureElement(game.TextureMap["checkbox_selected"])
                {
                    Color = Color.OliveDrab
                }
            ),
            new Button(
                new UiTextureElement(game.TextureMap["checkbox_empty"])
                {
                    Color = Color.PaleVioletRed
                },
                new UiTextureElement(game.TextureMap["checkbox_empty"])
                {
                    Color = Color.MediumVioletRed
                },
                new UiTextureElement(game.TextureMap["checkbox_empty"]) { Color = Color.DarkRed }
            ),
            game.SoundMap["button_release"],
            game.SoundMap["button_press"],
            enabled => vSync = enabled,
            () => vSync
        );
        var vSyncText = new Checkbox(
            new Button(
                new UiTextElement(
                    game.FontMap["24"],
                    () => $"VSync: {(vSync ? "On" : "Off")}",
                    Color.LimeGreen
                ),
                new UiTextElement(
                    game.FontMap["24"],
                    () => $"VSync: {(vSync ? "On" : "Off")}",
                    Color.GreenYellow
                ),
                new UiTextElement(
                    game.FontMap["24"],
                    () => $"VSync: {(vSync ? "On" : "Off")}",
                    Color.OliveDrab
                )
            ),
            new Button(
                new UiTextElement(
                    game.FontMap["24"],
                    () => $"VSync: {(vSync ? "On" : "Off")}",
                    Color.PaleVioletRed
                ),
                new UiTextElement(
                    game.FontMap["24"],
                    () => $"VSync: {(vSync ? "On" : "Off")}",
                    Color.MediumVioletRed
                ),
                new UiTextElement(
                    game.FontMap["24"],
                    () => $"VSync: {(vSync ? "On" : "Off")}",
                    Color.Violet
                )
            ),
            game.SoundMap["button_release"],
            game.SoundMap["button_press"],
            enabled => vSync = enabled,
            () => vSync
        );
        videoMenu.AddChild(
            new RelativeBox(game, 16, resolutionElement.Size())
            {
                DrawableElement = resolutionElement
            },
            new RelativeBox(
                game,
                (0, 8),
                (
                    (int)(vSyncText.Size().X + vSyncCheckbox.Size().X + 16),
                    (int)Math.Max(vSyncText.Size().Y, vSyncCheckbox.Size().Y)
                )
            )
            {
                BoxAlignment = Alignments.BelowPrevious
            }.WithChild(
                new RelativeBox(game, 0, vSyncCheckbox.Size() * 0.75F)
                {
                    DrawableElement = vSyncCheckbox,
                    SelfAlignment = Alignments.MiddleLeft,
                    BoxAlignment = Alignments.MiddleLeft
                },
                new RelativeBox(game, (8, 0), new FitElement())
                {
                    DrawableElement = vSyncText,
                    BoxAlignment = Alignments.RightOfPrevious
                }
            ),
            new RelativeBox(game, (0, 8), new FitElement())
            {
                DrawableElement = fullscreenButton,
                BoxAlignment = Alignments.BelowPrevious
            }
        );
        var soundMenu = new RelativeBox(game, (categoriesSize, 0F), (1 - categoriesSize, 1F))
        {
            BoxAlignment = Alignments.MiddleLeft,
            SelfAlignment = Alignments.MiddleLeft,
            IsVisible = () => page == 1
        };
        var masterVolumeElement = new UiTextElement(
            game.FontMap["24"],
            () => "Master Volume",
            Color.LimeGreen,
            Color.Green
        );
        var musicElement = new UiTextElement(
            game.FontMap["24"],
            () => "Music",
            Color.LimeGreen,
            Color.Green
        );
        var sfxElement = new UiTextElement(
            game.FontMap["24"],
            () => "SFX",
            Color.LimeGreen,
            Color.Green
        );
        soundMenu.AddChild(
            new RelativeBox(game, 16, new FitElement()) { DrawableElement = masterVolumeElement },
            new RelativeBox(game, (0, 8), new FitElement())
            {
                DrawableElement = musicElement,
                BoxAlignment = Alignments.BelowPrevious
            },
            new RelativeBox(game, (0, 8), new FitElement())
            {
                DrawableElement = sfxElement,
                BoxAlignment = Alignments.BelowPrevious
            }
        );
        var controlsMenu = new RelativeBox(game, (categoriesSize, 0F), (1 - categoriesSize, 1F))
        {
            BoxAlignment = Alignments.MiddleLeft,
            SelfAlignment = Alignments.MiddleLeft,
            IsVisible = () => page == 2
        };
        var collectElement = new UiTextElement(
            game.FontMap["24"],
            () => "Collect",
            Color.LimeGreen,
            Color.Green
        );
        var moveUpElement = new UiTextElement(
            game.FontMap["24"],
            () => "Move Up",
            Color.LimeGreen,
            Color.Green
        );
        var moveDownElement = new UiTextElement(
            game.FontMap["24"],
            () => "Move Down",
            Color.LimeGreen,
            Color.Green
        );
        var moveLeftElement = new UiTextElement(
            game.FontMap["24"],
            () => "Move Left",
            Color.LimeGreen,
            Color.Green
        );
        var moveRightElement = new UiTextElement(
            game.FontMap["24"],
            () => "Move Right",
            Color.LimeGreen,
            Color.Green
        );
        controlsMenu.AddChild(
            new RelativeBox(game, 16, new FitElement()) { DrawableElement = collectElement },
            new RelativeBox(game, (0, 8), new FitElement())
            {
                DrawableElement = moveUpElement,
                BoxAlignment = Alignments.BelowPrevious
            },
            new RelativeBox(game, (0, 8), new FitElement())
            {
                DrawableElement = moveDownElement,
                BoxAlignment = Alignments.BelowPrevious
            },
            new RelativeBox(game, (0, 8), new FitElement())
            {
                DrawableElement = moveLeftElement,
                BoxAlignment = Alignments.BelowPrevious
            },
            new RelativeBox(game, (0, 8), new FitElement())
            {
                DrawableElement = moveRightElement,
                BoxAlignment = Alignments.BelowPrevious
            }
        );
        settingsMenu.AddChild(videoMenu, soundMenu, controlsMenu);
        return settingsMenu;
    }
}
