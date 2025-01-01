using System.Collections.Generic;

namespace Laminar.Avalonia.InitializationTargets;

public interface IBeforeApplicationBuiltTarget
{
   public void BeforeApplicationBuiltInitialization();
}

public static class BeforeApplicationBuiltTargetExtensions
{
   public static void Initialize(this IEnumerable<IBeforeApplicationBuiltTarget> targets)
   {
      foreach (var target in targets)
      {
         target.BeforeApplicationBuiltInitialization();
      }
   }
}