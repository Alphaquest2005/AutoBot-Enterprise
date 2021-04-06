namespace Core.Common.Contracts
{
    public interface IBusinessServiceFactory
    {
        T CreateBusinessService<T>() where T : IBusinessService;
    }
}