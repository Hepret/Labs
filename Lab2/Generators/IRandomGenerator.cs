namespace Lab2.Generators;

/// <summary>
///     Интерфейс для генератора
/// </summary>
public interface IRandomGenerator
{
    /// <summary>
    ///     Генерирует следующее псевдослучайное число.
    /// </summary>
    int Next();

    /// <summary>
    /// Генерирует случайное число из интервала от 0 до 1
    /// </summary>
    /// <returns></returns>
    double NextDouble();
}