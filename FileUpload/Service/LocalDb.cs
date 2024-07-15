using System.Text.Json;

namespace FileUpload.Service;

public class LocalDb
{
    private const string _localDbPath = "LocalDb/localDb.txt";

    public async Task WriteAsync(LocalDbModel model)
    {
        var content = JsonSerializer.Serialize(model)+ Environment.NewLine;
        
        var path = Path.Combine(Directory.GetCurrentDirectory(), _localDbPath);

        using (var writer = new StreamWriter(path, append: true))
        {
            await writer.WriteLineAsync(content);
        }
    }

    public async Task<LocalDbModel?> ReadAsync(Guid id)
    {
        if (!File.Exists(_localDbPath))
        {
            return null;
        }

        var path = Path.Combine(Directory.GetCurrentDirectory(), _localDbPath);

        var lines = await File.ReadAllLinesAsync(path);

        foreach(var line in lines)
        {
            var model = JsonSerializer.Deserialize<LocalDbModel>(line);
            if(model.Id == id)
            {
                return model;
            }
        }

        return null;
    }

    public async Task<List<LocalDbModel>> ReadAllAsync()
    {
        if (!File.Exists(_localDbPath))
        {
            return new List<LocalDbModel>();
        }

        var path = Path.Combine(Directory.GetCurrentDirectory(), _localDbPath);

        var lines = await File.ReadAllLinesAsync(path);

        var models = new List<LocalDbModel>();

        foreach (var line in lines)
        {
            if(string.IsNullOrWhiteSpace(line))
            {
                continue;
            }
            var model = JsonSerializer.Deserialize<LocalDbModel>(line);
            models.Add(model);
        }

        return models;
    }
}

public class LocalDbModel
{
    public Guid Id { get; set; }
    public string FileAlias { get; set; } 
    public string FilePath { get; set; }
}