using System.ComponentModel;
using Laminar.Contracts.Base.UserInterface;

namespace Laminar.Contracts.UserData.Settings;

public interface IUserPreference : INotifyPropertyChanged
{
    public IDisplay Display { get; }

    public void Reset();
}
