using Mesfel.Models;

namespace Mesfel.Services
{
    public interface IIhaleAnalizService
    {
        Task<IhaleAnaliz> AnalizYapAsync(int ihaleId);
    }
}
