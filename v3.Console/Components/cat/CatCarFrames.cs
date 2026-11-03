namespace Garage.Terminal.Components;

internal static class CatCarFrames
{
    private static readonly string[] Body =
    [
        "                                                    ",
        "                                                    ",
        "                           BBBBBBBBBBB              ",
        "                          BSSSSSSSSSSSB             ",
        "                         BBSSWWWWWWWWSSS            ",
        "                        BBSSCWWWWCWWWWBSS           ",
        "                       BBRSCCCWWCCCWWWBBSS          ",
        "                      BRRSSCPCCCCPCWWWBDDD          ",
        "                      BRSSCCCCCCCCCCWWBDWWVV        ",
        "                     BBSSSCCCGCCGCCCWWBDWWWVV       ",
        "                     BSSSBCCCKCCKCCCWWBDWWWWVV      ",
        "                     BSSBKKKCCKKCCKKKWBDWGWWWVV     ",
        "                     BSSBBUCCCCCCCCUWWBDWGGWWWV     ",
        "                    DSSBBBCTCCCCCCTUUWBDWWWWWWWV    ",
        "                    DSBBDDTTTTTTTTTTUUBDWWWWWWWV    ",
        "                    DSBBDCCCCDBBBDCCCDBDDDDDDDDDSS  ",
        "                    DSBBDCCCCDBBBDCCCDBBRRRRBBSSSRSY",
        "                    DSBBDDDDDDBBBDDDDDBBBBBBBBBBSSSY",
        "                  DDDSBBBBHHHBBBBBBBBBBBHHHBBBBBBBSS",
    ];

    public static IReadOnlyList<string[]> Create()
    {
        List<string[]> frames = [];
        int[] bounce = [0, 1, 2, 1, 0];
        string[][] hubs =
        [
            [" H ", "HHH", " H "],
            ["H  ", " H ", "  H"],
            ["   ", "HHH", "   "],
            ["  H", " H ", "H  "],
            [" H ", " H ", " H "]
        ];

        for (int frame = 0; frame < 5; frame++)
        {
            char[][] pixels = Enumerable.Range(0, 26).Select(_ => new string(' ', 52).ToCharArray()).ToArray();
            Stamp(pixels, Body, 0, 2 - bounce[frame]);

            // Wheels stay on the ground; the body bounces above them.
            foreach (int left in new[] { 24, 38 })
            {
                Stamp(pixels, ["  HHH  ", " HKKKH ", "HKKKKKH", " KKKKK ", "  KKK  "], left, 20);
                Stamp(pixels, hubs[frame], left + 2, 21);
            }

            // Puffs move left and rise as the next puff appears near the exhaust.
            Stamp(pixels, [" KKK ", "KLLLK", " KLLK", "  KK "], 10 - frame, 16 - frame / 2);
            Stamp(pixels, [" K ", "KLK", " K "], 5 - frame, 12 - frame / 2);
            Stamp(pixels, ["KL", " K"], 15 - frame, 19 - frame / 2);
            frames.Add(pixels.Select(row => new string(row)).ToArray());
        }

        return frames;
    }

    private static void Stamp(char[][] canvas, string[] pattern, int left, int top)
    {
        for (int y = 0; y < pattern.Length; y++)
        {
            for (int x = 0; x < pattern[y].Length; x++)
            {
                int targetX = left + x;
                int targetY = top + y;
                if (pattern[y][x] != ' ' && targetY >= 0 && targetY < canvas.Length && targetX >= 0 && targetX < canvas[targetY].Length)
                    canvas[targetY][targetX] = pattern[y][x];
            }
        }
    }
}
