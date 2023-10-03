using Lab2.Generators;
using Microsoft.Extensions.Configuration;

namespace Lab2.Services;

/// <summary>
///     Реализация статистического теста на основе теста хи-квадрат
/// </summary>
public class PrngStatisticTest
{
    /// <summary>
    ///     Стандартный seed
    /// </summary>
    private const int DefaultSeed = 0;


    /// <summary>
    ///     Хранит значения Хи-квадрат
    /// </summary>
    private static readonly Dictionary<int, double> ChiSquaredTable = new()
    {
        { 1, 3.841 },
        { 2, 5.991 },
        { 3, 7.815 },
        { 4, 9.488 },
        { 5, 11.070 },
        { 6, 12.592 },
        { 7, 14.067 },
        { 8, 15.507 },
        { 9, 16.919 },
        { default, 0 }
    };

    /// <summary>
    ///     Генераторы
    /// </summary>
    private readonly Dictionary<Type, ConstructorWithSeedAndDefaultConstructor> _generators;

    /// <summary>
    ///     Путь к папке с результатами
    /// </summary>
    private readonly string _resultTestPath;

    /// <summary>
    ///     Seed
    /// </summary>
    private readonly int _seed;

    public PrngStatisticTest(IConfiguration configuration)
    {
        _generators = new Dictionary<Type, ConstructorWithSeedAndDefaultConstructor>();
        var defaultResourcesPath = new DirectoryInfo(Environment.CurrentDirectory).Parent!.Parent!.Parent!;
        const string defaultTestResultPath = "\\testResults\\";
        var resourcesPath = configuration.GetSection("resourcesPath").Value ?? defaultResourcesPath.FullName;

        var testResultPath = configuration.GetSection("testPath").Value ?? defaultTestResultPath;

        if (!int.TryParse(configuration.GetSection("seed").Value, out _seed)) _seed = DefaultSeed;

        var testResultsPath = resourcesPath + testResultPath;
        var imageDir = new DirectoryInfo(testResultsPath);
        if (!imageDir.Exists) imageDir.Create();
        _resultTestPath = testResultsPath;
    }

    /// <summary>
    ///     Запуск тестов
    /// </summary>
    public void Run()
    {
        foreach (var generatorConstructors in _generators.Values)
        {
            var parameters = new object[] { _seed };
            var generator = (IRandomGenerator)generatorConstructors.ConstructorWithSeed.Invoke(parameters);
            RunTest(generator);
        }
    }

    /// <summary>
    ///     Запускает тест для генератора
    /// </summary>
    private void RunTest(IRandomGenerator generator)
    {
        const int totalNums = 1000;
        var data = new double[totalNums];
        for (var i = 0; i < totalNums; i++) data[i] = generator.NextDouble();

        var result = ChiSquareTest(data);
        var generatorName = generator.GetType().Name;
        WriteResults(generatorName, result);
    }

    /// <summary>
    ///     Хи-квадрат тест (уровень значимости 0.05)
    /// </summary>
    /// <param name="data">Данные</param>
    /// <param name="numBins">Количество разбиений</param>
    private static ChiSquaredTestResults ChiSquareTest(double[] data, int numBins = 10)
    {
        var histogram = new int[numBins];
        var min = data.Min();
        var max = data.Max();

        // Заполнение гистограммы
        foreach (var value in data)
        {
            int bin;
            if (max != min)
                bin = (int)((value - min) / (max - min) * numBins);
            else
                bin = 0; // Если max и min равны, то bin 0
            if (bin < 0)
                bin = 0;
            else if (bin >= numBins) bin = numBins - 1;

            histogram[bin]++;
        }

        var chiSquared = 0.0;
        var expectedFrequency = (double)data.Length / numBins;
        for (var i = 0; i < numBins; i++)
            chiSquared += Math.Pow(histogram[i] - expectedFrequency, 2) / expectedFrequency;

        var degreesOfFreedom = numBins - 1;

        var criticalValue = ChiSquaredTable[degreesOfFreedom];
        var result = chiSquared <= criticalValue;
        return new ChiSquaredTestResults(result, chiSquared, degreesOfFreedom, criticalValue);
    }


    /// <summary>
    ///     Записывает результаты
    /// </summary>
    private void WriteResults(string generatorName, ChiSquaredTestResults results)
    {
        using var writer = new StreamWriter(_resultTestPath + generatorName);
        writer.WriteLine("Тест для генератора: " + generatorName);
        writer.WriteLine("Значение хи-квадрат: " + results.ChiSquared);
        writer.WriteLine("Степени свободы: " + results.DegreesOfFreedom);
        writer.WriteLine("Критическое значение: " + results.CriticalValue);
        writer.WriteLine("Уровень значимости: " + results.SignificanceLevel);
        writer.WriteLine(results.IsOk
            ? "Результат теста не отвергает нулевую гипотезу о равномерном распределении"
            : "Результат теста отвергает нулевую гипотезу о равномерном распределении");
    }

    /// <summary>
    ///     Добавляет генератор
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void AddGenerator<T>() where T : IRandomGenerator
    {
        var generatorType = typeof(T);

        var constructorWithSeed = generatorType
                                      .GetConstructors()
                                      .FirstOrDefault(ctor =>
                                          ctor.GetParameters().Length == 1
                                          && ctor.GetParameters()[0].ParameterType == typeof(int))
                                  ?? throw new ArgumentException("Неправильный формат конструктора");
        var defaultConstructor = generatorType.GetConstructor(Type.EmptyTypes)
                                 ?? throw new ArgumentException("Неправильный формат конструктора");

        var constructors = new ConstructorWithSeedAndDefaultConstructor(constructorWithSeed, defaultConstructor);

        _generators[generatorType] = constructors;
    }

    /// <summary>
    ///     Результаты теста
    /// </summary>
    /// <param name="IsOk">Принимается ли гипотеза</param>
    /// <param name="ChiSquared">Значение Хи-квадрат</param>
    /// <param name="DegreesOfFreedom">Степени свободы</param>
    /// <param name="CriticalValue">Критическое значение</param>
    /// <param name="SignificanceLevel">Уровень значимости</param>
    private record ChiSquaredTestResults(bool IsOk, double ChiSquared, int DegreesOfFreedom, double CriticalValue,
        double SignificanceLevel = 0.05);
}