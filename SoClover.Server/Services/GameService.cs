using SoClover.Server.Context;

namespace SoClover.Server.Services
{
    public interface IGameService
    {

    }

    public class GameService(SoCloverDBContext context) : IGameService
    {
        private readonly SoCloverDBContext _context = context;
    }
}
