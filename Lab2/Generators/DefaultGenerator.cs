namespace Lab2.Generators;

/// <summary>
/// Обертка для стандартного генератора
/// </summary>
public class DefaultGenerator : IRandomGenerator
{
    /// <summary>
    /// Рандом
    /// </summary>
    private readonly Random _random;

    public DefaultGenerator(int seed)
    {
        _random = new Random(seed);
    }

    public DefaultGenerator()
    {
        _random = new Random();
    }

    /// <inheritdoc/>
    public int Next()
    {
        return _random.Next(int.MinValue, int.MaxValue);
    }
}