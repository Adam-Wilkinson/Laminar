using System.IO;
using Laminar.Contracts.UserData;

namespace Laminar.Implementation.UserData;

public class LaminarRootFolder : ILaminarRootFolder
{
    public LaminarRootFolder(string path)
    {
        Path = path;
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
    
    public string Path { get; }

    public bool IsEnabled { get; set; } = true;
}