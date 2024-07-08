namespace DemoApi.Net6.Services
{
    public interface IOrderService
    {
        Task CircuitBreaker();

        Task<string> MyFallback();

        Task OnlyFallback();

        Task<string> Retry();

        Task<string> TimeOut();
    }
}