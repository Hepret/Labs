namespace Lab2.Generators;

/// <summary>
/// Интерфейс для генератора
/// </summary>
public interface IRandomGenerator
{
    /// <summary>
    /// Генерирует следующее псевдослучайное число.
    /// </summary>
    int Next();
}