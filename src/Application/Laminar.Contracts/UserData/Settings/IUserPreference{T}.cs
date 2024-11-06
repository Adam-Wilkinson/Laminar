namespace Laminar.Contracts.UserData.Settings;

public interface IUserPreference<T> : IUserPreference
{
    public T Value { get; set; }
}
