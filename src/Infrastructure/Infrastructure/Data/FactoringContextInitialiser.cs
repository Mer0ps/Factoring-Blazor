using Microsoft.Extensions.Logging;

namespace Infrastructure.Data;

public class FactoringContextInitialiser
{

    private readonly ILogger<FactoringContextInitialiser> _logger;
    private readonly FactoringContext _context;

    public FactoringContextInitialiser(ILogger<FactoringContextInitialiser> logger, FactoringContext context)
    {
        _logger = logger;
        _context = context;
    }
    public async Task InitialiseAsync()
    {
        try
        {
            //await _context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }
}
