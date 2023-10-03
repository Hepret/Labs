using System.Drawing;
using System.Drawing.Imaging;
using Lab2.Generators;
using Microsoft.Extensions.Configuration;

namespace Lab2.Services;

/// <summary>
///     Сервис для генерации картинок для генератора случайных чисел
/// </summary>
public class PrngImageGeneratorService
{
    /// <summary>
    ///     Стандартный путь к изображениям
    /// </summary>
    private const string DefaultPath = "\\images\\";

    /// <summary>
    ///     Стандартный размер изображения
    /// </summary>
    private const int DefaultSize = 512;

    /// <summary>
    ///     Стандартный seed
    /// </summary>
    private const int DefaultSeed = 0;

    /// <summary>
    ///     Генераторы
    /// </summary>
    private readonly Dictionary<Type, ConstructorWithSeedAndDefaultConstructor> _generators;

    /// <summary>
    ///     Размер картинки в px
    /// </summary>
    private readonly int _size;

    /// <summary>
    ///     Путь к папке, где надо сохранять картинки
    /// </summary>
    public readonly string ImagesPath;

    /// <summary>
    ///     Seed
    /// </summary>
    public readonly int Seed;

    public PrngImageGeneratorService(IConfiguration configuration)
    {
        _generators = new Dictionary<Type, ConstructorWithSeedAndDefaultConstructor>();
        var defaultResourcesPath = new DirectoryInfo(Environment.CurrentDirectory)!.Parent!.Parent!.Parent!;

        var resourcesPath = configuration.GetSection("resourcesPath").Value ?? defaultResourcesPath.FullName;

        var imagesPathSection = configuration.GetSection("imagesPath");

        var imagesPath = imagesPathSection.Value ?? DefaultPath;

        ImagesPath = resourcesPath + imagesPath;
        var imageDir = new DirectoryInfo(ImagesPath);
        if (!imageDir.Exists) imageDir.Create();

        if (!int.TryParse(configuration.GetSection("seed").Value, out Seed)) Seed = DefaultSeed;

        if (!int.TryParse(configuration.GetSection("imageSize").Value, out _size)) _size = DefaultSize;
    }

    /// <summary>
    ///     Получает i-ый бит числа
    /// </summary>
    private static uint GetBytePart(int num, int byteIndex)
    {
        var bitIndex = byteIndex * 4;
        var numByte = (uint)((num >> bitIndex) ^ 0xF);
        return numByte;
    }

    /// <summary>
    ///     Получает цвет из числа
    /// </summary>
    private static Color GetColor(int i)
    {
        return Color.FromArgb(i);
    }

    /// <summary>
    ///     Генерирует картинки для всех генераторов по конкретному сиду
    /// </summary>
    public void CreateImageOnOneSeed()
    {
        foreach (var generatorType in _generators.Keys)
        {
            var constructor = _generators[generatorType].ConstructorWithSeed;
            var parameters = new object[] { Seed };
            var generator = (IRandomGenerator)constructor.Invoke(parameters);

            using var image = new Bitmap(_size, _size);
            using var graphics = Graphics.FromImage(image);
            var pen = new Pen(Color.Aqua);

            for (var i = 0; i < _size; i++)
            for (var j = 0; j < _size; j++)
            {
                var num = generator.Next();
                pen.Color = GetColor(num);
                graphics.DrawEllipse(pen, i, j, 1, 1);
            }

            var path = ImagesPath + generatorType.Name;
            var dir = new DirectoryInfo(path);
            if (!dir.Exists)
                dir.Create();
            var fullPath = path + "\\oneSeed.jpeg";
            image.Save(fullPath, ImageFormat.Jpeg);
        }
    }

    /// <summary>
    ///     Генерирует картинку по разным сидам
    /// </summary>
    public void CreateImageDifferentSeeds()
    {
        foreach (var generatorType in _generators.Keys)
        {
            var constructor = _generators[generatorType].ConstructorWithSeed;

            using var image = new Bitmap(_size, _size);
            using var graphics = Graphics.FromImage(image);
            var pen = new Pen(Color.Aqua);

            var seed = 0;

            for (var i = 0; i < _size; i++)
            for (var j = 0; j < _size; j++)
            {
                var parameters = new object[] { seed };
                var generator = (IRandomGenerator)constructor.Invoke(parameters);
                var num = generator.Next();
                pen.Color = GetColor(num);
                graphics.DrawEllipse(pen, i, j, 1, 1);
                seed++;
            }

            var path = ImagesPath + generatorType.Name;
            var dir = new DirectoryInfo(path);
            if (!dir.Exists)
                dir.Create();
            var fullPath = path + "\\manySeedsOneGeneration.jpeg";
            image.Save(fullPath, ImageFormat.Jpeg);
        }
    }

    /// <summary>
    ///     Генерирует картинки по всем тестам
    /// </summary>
    public void CreateImages()
    {
        CreateImageOnOneSeed();
        CreateImageDifferentSeeds();
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
}