using Newtonsoft.Json;

namespace TRGE.Core;

public class Config : Dictionary<string, object>
{
    public Config()
        : base() { }

    public Config(Config otherConfig)
        : base(otherConfig ?? new Config()) { }

    public static Config Read(string filePath, bool isCompressed = true)
    {
        return File.Exists(filePath) ? 
            JsonConvert.DeserializeObject<Config>(isCompressed ? new FileInfo(filePath).ReadCompressedText() : File.ReadAllText(filePath)) : 
            null;
    }

    public void Write(string filePath, bool isCompressed = true, Formatting formatting = Formatting.None)
    {
        string json = JsonConvert.SerializeObject(this, formatting);
        if (isCompressed)
        {
            new FileInfo(filePath).WriteCompressedText(json); //#48
        }
        else
        {
            File.WriteAllText(filePath, json);
        }
    }

    public object Get(string key, object defaultValue = null)
    {
        return ContainsKey(key) ? this[key] : defaultValue;
    }

    public string GetString(string key, string defaultValue = "")
    {
        return ContainsKey(key) ? this[key].ToString() : defaultValue;
    }

    public int GetInt(string key, int defaultValue = -1)
    {
        object value = Get(key);
        return value == null ? defaultValue : int.Parse(value.ToString());
    }

    public double GetDouble(string key, int defaultValue = -1)
    {
        object value = Get(key);
        return value == null ? defaultValue : double.Parse(value.ToString());
    }

    public uint GetUInt(string key, uint defaultValue = 0)
    {
        object value = Get(key);
        return value == null ? defaultValue : uint.Parse(value.ToString());
    }

    public bool GetBool(string key, bool defaultValue = false)
    {
        object value = Get(key);
        return value == null ? defaultValue : bool.Parse(value.ToString());
    }

    public object GetEnum(string key, Type enumType, object defaultValue)
    {
        object value = Get(key);
        return value == null ? defaultValue : Enum.ToObject(enumType, value);
    }

    public T[] GetArray<T>(string key)
    {
        object value = Get(key);
        return value == null ? null : JsonConvert.DeserializeObject<T[]>(value.ToString());
    }

    public GameMode GetGameMode(string key, GameMode defaultValue = GameMode.Normal)
    {
        return (GameMode)GetEnum(key, typeof(GameMode), defaultValue);
    }

    public Organisation GetOrganisation(string key, Organisation defaultValue = Organisation.Default)
    {
        return (Organisation)GetEnum(key, typeof(Organisation), defaultValue);
    }

    public Hardware GetHardware(string key, Hardware defaultValue = Hardware.PC)
    {
        return (Hardware)GetEnum(key, typeof(Hardware), defaultValue);
    }

    public TRVersion GetTRVersion(string key, TRVersion defaultValue = TRVersion.Unknown)
    {
        return (TRVersion)GetEnum(key, typeof(TRVersion), defaultValue);
    }

    public Config GetSubConfig(string key, Config defaultValue = null)
    {
        object value = Get(key);
        return value == null ? defaultValue : JsonConvert.DeserializeObject<Config>(value.ToString());
    }
}