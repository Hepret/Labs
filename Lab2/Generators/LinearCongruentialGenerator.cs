using Lab2.Generators;

namespace Lab2;

/// <summary>
///     Класс, реализующий линейный конгруэнтный генератор псевдослучайных чисел
/// </summary>
public class LinearCongruentialGenerator : IRandomGenerator
{
    /// <summary>
    ///     текущее значение генератора
    /// </summary>
    private int _seed;

    /// <summary>
    ///     Создает новый генератор псевдослучайных чисел со случайным начальным значением.
    ///     Конструктор без параметров использует текущее время в качестве начального значения генератора.
    /// </summary>
    public LinearCongruentialGenerator()
    {
        _seed = (int)DateTime.Now.Ticks;
    }

    /// <summary>
    ///     Создает новый генератор псевдослучайных чисел с заданным начальным значением.
    /// </summary>
    /// <param name="seed">Начальное значение для генерации</param>
    public LinearCongruentialGenerator(int seed)
    {
        _seed = seed;
    }

    /// <inheritdoc />
    public int Next()
    {
        _seed = (A * _seed + C) % M;
        return _seed;
    }

    /// <inheritdoc/>
    public double NextDouble()
    {
        double num = Next();
        num *= double.Epsilon;
        return double.Abs(num);
    }

    #region Константы для линейного конгруэнтного генератора

    /// <summary>
    ///     Множитель
    /// </summary>
    private const int A = 2147483629;

    /// <summary>
    ///     Приращение
    /// </summary>
    private const int C = 2147483587;

    /// <summary>
    ///     Модуль
    /// </summary>
    private const int M = int.MaxValue;

    #endregion
}