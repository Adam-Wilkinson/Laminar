using System.Collections.Generic;

namespace Laminar.Avalonia.InitializationTargets;

public interface IAfterApplicationBuiltTarget
{
    public void OnApplicationBuilt();
}

public static class IAfterApplicationBuiltTargetExtensions
{
    public static void Initialize(this IEnumerable<IAfterApplicationBuiltTarget> targets)
    {
        foreach (var target in targets)
        {
            target.OnApplicationBuilt();
        }
    }
}