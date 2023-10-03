using System.Reflection;

namespace Lab2.Services;

/// <summary>
///     Конструктор с сидом, конструктор без параметров
/// </summary>
/// <param name="ConstructorWithSeed">Конструктор с сидом</param>
/// <param name="DefaultConstructor">Конструктор без параметров</param>
public record ConstructorWithSeedAndDefaultConstructor(ConstructorInfo ConstructorWithSeed,
    ConstructorInfo DefaultConstructor);