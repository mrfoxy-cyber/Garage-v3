namespace Garage.ConsoleApp.Prompts;

internal static class ConsoleInput
{
    public static int ReadInt(
        string prompt,
        int minimum,
        int maximum)
    {
        while (true)
        {
            Console.Write(prompt);

            if (int.TryParse(Console.ReadLine(), out int value) &&
                value >= minimum &&
                value <= maximum)
            {
                return value;
            }

            Console.WriteLine(
                $"Enter a number between {minimum} and {maximum}.");
        }
    }

    public static string ReadRequiredString(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine()?.Trim();

            if (!string.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            Console.WriteLine("A value is required.");
        }
    }

    public static string? ReadOptionalString(string prompt)
    {
        Console.Write(prompt);

        string? input = Console.ReadLine()?.Trim();

        return string.IsNullOrWhiteSpace(input)
            ? null
            : input;
    }

    public static int? ReadOptionalInt(
        string prompt,
        int minimum,
        int maximum)
    {
        while (true)
        {
            Console.Write(prompt);

            string? input = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            if (int.TryParse(input, out int value) &&
                value >= minimum &&
                value <= maximum)
            {
                return value;
            }

            Console.WriteLine(
                $"Enter a number between {minimum} and {maximum}, " +
                "or leave it empty for Any.");
        }
    }

    public static double ReadDouble(
        string prompt,
        double minimum,
        double maximum)
    {
        while (true)
        {
            Console.Write(prompt);

            if (double.TryParse(Console.ReadLine(), out double value) &&
                value >= minimum &&
                value <= maximum)
            {
                return value;
            }

            Console.WriteLine(
                $"Enter a number between {minimum} and {maximum}.");
        }
    }

    public static double? ReadOptionalDouble(
        string prompt,
        double minimum,
        double maximum)
    {
        while (true)
        {
            Console.Write(prompt);

            string? input = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            if (double.TryParse(input, out double value) &&
                value >= minimum &&
                value <= maximum)
            {
                return value;
            }

            Console.WriteLine(
                $"Enter a number between {minimum} and {maximum}, " +
                "or leave it empty for Any.");
        }
    }

    public static bool ReadBoolean(string prompt)
    {
        while (true)
        {
            Console.Write($"{prompt} (Y/N): ");
            string? input = Console.ReadLine()?.Trim();

            if (input?.Equals(
                    "Y",
                    StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }

            if (input?.Equals(
                    "N",
                    StringComparison.OrdinalIgnoreCase) == true)
            {
                return false;
            }

            Console.WriteLine("Enter Y or N.");
        }
    }

    public static Enum ReadEnum(
        Type enumType,
        string prompt)
    {
        List<Enum> values = [];

        foreach (Enum value in Enum.GetValues(enumType))
        {
            values.Add(value);
        }

        Console.WriteLine(prompt);

        for (int index = 0; index < values.Count; index++)
        {
            Console.WriteLine($"{index + 1}. {values[index]}");
        }

        int choice = ReadInt(
            "Choose: ",
            minimum: 1,
            maximum: values.Count);

        return values[choice - 1];
    }

    public static Enum? ReadOptionalEnum(
        Type enumType,
        string prompt)
    {
        List<Enum> values = [];

        foreach (Enum value in Enum.GetValues(enumType))
        {
            values.Add(value);
        }

        Console.WriteLine(prompt);
        Console.WriteLine("0. Any");

        for (int index = 0; index < values.Count; index++)
        {
            Console.WriteLine($"{index + 1}. {values[index]}");
        }

        int choice = ReadInt(
            "Choose: ",
            minimum: 0,
            maximum: values.Count);

        return choice == 0
            ? null
            : (Enum)values[choice -1]!;
    }

    public static void Pause()
    {
        Console.WriteLine();
        Console.WriteLine("Press Enter to continue.");
        Console.ReadLine();
    }

    public static Enum? ReadEnumOrCancel(
        Type enumType,
        string prompt,
        string cancelOptionText = "Cancel")
    {
        Array values = Enum.GetValues(enumType);

        Console.WriteLine(prompt);
        Console.WriteLine($"0. {cancelOptionText}");

        for (int index = 0; index < values.Length; index++)
        {
            Console.WriteLine($"{index + 1}. {values.GetValue(index)}");
        }

        int choice = ReadInt(
            "Choose: ",
            minimum: 0,
            maximum: values.Length);

        return choice == 0
            ? null
            : (Enum)values.GetValue(choice - 1)!;
    }
}
