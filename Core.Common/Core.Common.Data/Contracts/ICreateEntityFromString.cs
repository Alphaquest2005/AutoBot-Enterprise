namespace Core.Common.Data.Contracts
{
    public interface ICreateEntityFromString<T> where T : IIdentifiableEntity
    {
        T CreateEntityFromString(string value);
    }
}