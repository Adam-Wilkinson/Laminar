using Laminar.Contracts.Base.UserInterface;
using Laminar.PluginFramework.UserInterface;

namespace Laminar.Implementation.Base.UserInterface;

internal class DisplayFactory(IUserInterfaceProvider userInterfaceProvider) : IDisplayFactory
{
    public IDisplay CreateDisplayForValue(IDisplayValue valueInfo)
    {
        return new Display(valueInfo, userInterfaceProvider);
    }
}
