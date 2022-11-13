using System;

namespace PencilChiselCode;

public static class Program
{
    [STAThread]
    private static void Main()
    {
        using var game = new Bonfire();
        game.Run();
    }
}