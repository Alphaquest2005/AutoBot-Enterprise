namespace Core.Common.Contracts
{
    public interface IClientServiceFactory
    {
        TClient CreateClient<TClient>()
            where TClient : IClientService;
    }
}