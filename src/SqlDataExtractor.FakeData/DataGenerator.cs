namespace SqlDataExtractor.FakeData;

public class DataGenerator
{
    private static readonly Dictionary<Locale, IFakeDataGenerator> _dataGenerators = new();

    public int GenerateInteger(int exclusiveMax) => GenerateInteger(0, exclusiveMax);

    public int GenerateInteger(int inclusiveMin, int exclusiveMax)
    {
        return Random.Shared.Next(inclusiveMin, exclusiveMax);
    }

    public T GetRandomElement<T>(T[] array)
    {
        return array[GenerateInteger(array.Length)];
    }

    public static IFakeDataGenerator GetDataGenerator(Locale locale)
    {
        if (_dataGenerators.TryGetValue(locale, out var dataGenerator))
        {
            return dataGenerator;
        }

        dataGenerator = locale switch
        {
            Locale.sv_SE => new SwedishFakeDataGenerator(new DataGenerator()),
            _ => throw new ArgumentException(paramName: nameof(locale), message: "")
        };

        _dataGenerators[locale] = dataGenerator;

        return dataGenerator;
    }
}
