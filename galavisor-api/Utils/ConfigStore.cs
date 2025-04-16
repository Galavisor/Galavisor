using System.Text.Json;

public static class ConfigStore
{
    private static readonly string ConfigFile = "server_config.json";

    private static Dictionary<string, string> _config = Load();

    private static Dictionary<string, string> Load()
    {
        if (!File.Exists(ConfigFile))
            return [];

        var json = File.ReadAllText(ConfigFile);
        return JsonSerializer.Deserialize<Dictionary<string, string>>(json)
                ?? [];
    }

    private static void Save()
    {
        var json = JsonSerializer.Serialize(_config, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(ConfigFile, json);
    }

    public static void Set(string key, string value)
    {
        _config[key] = value;
        Save();
    }

    public static string Get(string key)
    {
        if(_config.TryGetValue(key, out var value)){
            return value;
        } else{
            throw new Exception($"Env not found: {key}.");
        }
    }

    public static void Remove(string key)
    {
        if (_config.Remove(key))
            Save();
    }

    public static void Clear()
    {
        _config.Clear();
        Save();
    }

    public static bool Exists(string key)
    {
        try{
            return Get(key) != "";
        } catch (Exception)
        {
            return false;
        }
    }
}
