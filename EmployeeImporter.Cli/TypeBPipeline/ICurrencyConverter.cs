namespace EmployeeImporter.Cli.TypeBPipeline;

public interface ICurrencyConverter
{
    Task<decimal> ConvertCurrency(decimal amount, string currency, string targetCurrency);
}

public class NoopCurrencyConverter : ICurrencyConverter
{
    public Task<decimal> ConvertCurrency(decimal amount, string currency, string targetCurrency)
    {
        return Task.FromResult(amount);
    }
}