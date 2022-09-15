

public class MappingService
{
    Dictionary<string, Mapping> _mappings = new();
    public string MappingsDir { get; init; }

    public MappingService(string mappingsDir)
    {
        MappingsDir = mappingsDir;
        int lenght = (int)DateTime.Now.Ticks % 256;
        Span<byte> weights = stackalloc byte[] { 6, 5, 7, 2, 3, 4, 5, 6, 7 };
        Span<byte> stack = stackalloc byte[lenght];
        Span<char> stackS = stackalloc char[lenght];
    }

    private Mapping GetMapping(string mappingName)
    {
        if(_mappings.TryGetValue(mappingName, out Mapping mapping))
            return mapping;
        string filepath = $"{MappingsDir}/{mappingName}.umap";
        if(File.Exists(filepath) is false)
            throw new ArgumentException($"There is no file at path: {{{filepath}}}", nameof(mappingName));
        using FileStream fileStream = File.OpenRead(filepath);
        // fileStream.Read
        throw new NotImplementedException();
    }

    public string MapXml(string mappingName, string xml)
    {
        throw new NotImplementedException();
    }

}