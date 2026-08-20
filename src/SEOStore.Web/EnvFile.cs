namespace SEOStore.Web;

public static class EnvFile
{
    private static readonly Dictionary<string, string?> Values = new(StringComparer.OrdinalIgnoreCase);

    public static IReadOnlyDictionary<string, string?> CurrentValues => Values;

    public static void Load()
    {
        Values.Clear();

        var startPaths = new[]
        {
            Directory.GetCurrentDirectory(),
            AppContext.BaseDirectory
        };

        foreach (var start in startPaths.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var directory = new DirectoryInfo(start);
            for (var i = 0; i < 8 && directory is not null; i++)
            {
                var envPath = Path.Combine(directory.FullName, ".env");
                if (File.Exists(envPath))
                {
                    Apply(envPath);
                    return;
                }

                directory = directory.Parent;
            }
        }
    }

    private static void Apply(string envPath)
    {
        foreach (var rawLine in File.ReadAllLines(envPath))
        {
            var line = rawLine.Trim();
            if (line.Length == 0 || line.StartsWith('#'))
                continue;

            if (line.StartsWith("export ", StringComparison.OrdinalIgnoreCase))
                line = line["export ".Length..].Trim();

            var separator = line.IndexOf('=');
            if (separator <= 0)
                continue;

            var key = line[..separator].Trim();
            var value = Unquote(line[(separator + 1)..].Trim());

            if (key.Length == 0)
                continue;

            Values[key] = value;
            Environment.SetEnvironmentVariable(key, value);
        }
    }

    private static string Unquote(string value)
    {
        if (value.Length >= 2 &&
            ((value.StartsWith('"') && value.EndsWith('"')) ||
             (value.StartsWith('\'') && value.EndsWith('\''))))
        {
            return value[1..^1];
        }

        var commentIndex = value.IndexOf(" #", StringComparison.Ordinal);
        return commentIndex >= 0 ? value[..commentIndex].Trim() : value;
    }
}
