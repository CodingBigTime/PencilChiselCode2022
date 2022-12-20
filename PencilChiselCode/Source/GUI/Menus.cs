using System;
using Microsoft.Xna.Framework;

namespace PencilChiselCode.Source.GUI;

public static class Menus
{
    public static RelativeBox GetSettingsMenu(Bonfire game, Action onDoneClick)
    {
        var page = 0;
        const float categoriesSize = 0.25F;
        var settingsMenu = new RelativeBox(game, 0, 0.66F)
        {
            BoxAlignment = Alignments.MiddleCenter,
            SelfAlignment = Alignments.MiddleCenter
        };
        var menuCategories = new RelativeBox(game, 0, (categoriesSize, 1F))
        {
            BoxAlignment = Alignments.MiddleLeft,
            SelfAlignment = Alignments.MiddleLeft
        };
        var videoElement = new TextButton(
            new UiTextElement(game.FontMap["32"], () => "Video", Color.White, Color.Black),
            new UiTextElement(game.FontMap["32"], () => "Video", Color.Red, Color.Black),
            new UiTextElement(game.FontMap["32"], () => "Video", Color.Green, Color.Black),
            game.SoundMap["button_press"],
            game.SoundMap["button_release"],
            () => page = 0
        );
        var audioElement = new TextButton(
            new UiTextElement(game.FontMap["32"], () => "Audio", Color.White, Color.Black),
            new UiTextElement(game.FontMap["32"], () => "Audio", Color.Red, Color.Black),
            new UiTextElement(game.FontMap["32"], () => "Audio", Color.Green, Color.Black),
            game.SoundMap["button_press"],
            game.SoundMap["button_release"],
            () => page = 1
        );
        var controlsElement = new TextButton(
            new UiTextElement(game.FontMap["32"], () => "Controls", Color.White, Color.Black),
            new UiTextElement(game.FontMap["32"], () => "Controls", Color.Red, Color.Black),
            new UiTextElement(game.FontMap["32"], () => "Controls", Color.Green, Color.Black),
            game.SoundMap["button_press"],
            game.SoundMap["button_release"],
            () => page = 2
        );
        var doneElement = new TextButton(
            new UiTextElement(game.FontMap["32"], () => "Done", Color.White, Color.Black),
            new UiTextElement(game.FontMap["32"], () => "Done", Color.Red, Color.Black),
            new UiTextElement(game.FontMap["32"], () => "Done", Color.Green, Color.Black),
            game.SoundMap["button_press"],
            game.SoundMap["button_release"],
            onDoneClick
        );
        menuCategories.AddChild(
            new RelativeBox(game, 16, videoElement.Size()) { DrawableElement = videoElement },
            new RelativeBox(game, (16, 64), audioElement.Size()) { DrawableElement = audioElement },
            new RelativeBox(game, (16, 112), controlsElement.Size())
            {
                DrawableElement = controlsElement
            },
            new RelativeBox(game, 16, doneElement.Size())
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
        var resolutionElement = new UiTextElement(
            game.FontMap["24"],
            () => "Resolution",
            Color.LimeGreen,
            Color.Green
        );
        var vsyncElement = new UiTextElement(
            game.FontMap["24"],
            () => "VSync",
            Color.LimeGreen,
            Color.Green
        );
        videoMenu.AddChild(
            new RelativeBox(game, 16, resolutionElement.Size())
            {
                DrawableElement = resolutionElement
            },
            new RelativeBox(game, (16, 48), vsyncElement.Size()) { DrawableElement = vsyncElement }
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
            new RelativeBox(game, 16, masterVolumeElement.Size())
            {
                DrawableElement = masterVolumeElement
            },
            new RelativeBox(game, (16, 48), musicElement.Size()) { DrawableElement = musicElement },
            new RelativeBox(game, (16, 80), sfxElement.Size()) { DrawableElement = sfxElement }
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
            new RelativeBox(game, 16, collectElement.Size()) { DrawableElement = collectElement },
            new RelativeBox(game, (16, 48), moveUpElement.Size())
            {
                DrawableElement = moveUpElement
            },
            new RelativeBox(game, (16, 80), moveDownElement.Size())
            {
                DrawableElement = moveDownElement
            },
            new RelativeBox(game, (16, 112), moveLeftElement.Size())
            {
                DrawableElement = moveLeftElement
            },
            new RelativeBox(game, (16, 144), moveRightElement.Size())
            {
                DrawableElement = moveRightElement
            }
        );
        settingsMenu.AddChild(videoMenu, soundMenu, controlsMenu);
        return settingsMenu;
    }
}
