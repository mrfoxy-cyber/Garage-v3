namespace Garage.ConsoleApp.Display;

internal static class ConsoleScreen
{
    public static void Clear()
    {
        if (!Console.IsOutputRedirected)
            Console.Clear();
    }
}
