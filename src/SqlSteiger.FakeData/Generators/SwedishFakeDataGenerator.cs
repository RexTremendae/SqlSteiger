namespace SqlSteiger.FakeData;

public partial class SwedishFakeDataGenerator : IFakeDataGenerator
{
    private readonly DataGenerator _dataGenerator;

    public SwedishFakeDataGenerator(DataGenerator dataGenerator)
    {
        _dataGenerator = dataGenerator;
    }

    public string GenerateFirstName()
    {
        return _dataGenerator.GenerateInteger(2) == 1
            ? _dataGenerator.GetRandomElement(FirstNamesMale)
            : _dataGenerator.GetRandomElement(FirstNamesFemale);
    }

    public string GenerateLastName()
    {
        return _dataGenerator.GetRandomElement(LastNames);
    }

    public string GenerateEmail()
    {
        return $"{GenerateFirstName()}.{GenerateLastName()}@{_dataGenerator.GetRandomElement(DomainNames)}.{_dataGenerator.GetRandomElement(TopDomains)}";
    }

    public string GetAddressLine()
    {
        return $"{_dataGenerator.GetRandomElement(Addresses)} {_dataGenerator.GenerateInteger(200)}";
    }

    public string GetPostalcode()
    {
        var postalCode = _dataGenerator.GenerateInteger(10000, 99999).ToString();
        return $"{postalCode[..3]} {postalCode[3..]}";
    }

    public string GetCity()
    {
        return _dataGenerator.GetRandomElement(Cities);
    }

    public string GetCountry()
    {
        return _dataGenerator.GetRandomElement(Countries);
    }

    public string GetPhoneNumber()
    {
        var countryCode = "+46";
        var prefix = $"7{_dataGenerator.GenerateInteger(10)}";
        var group1 = _dataGenerator.GenerateInteger(1000).ToString().PadLeft(3, '0');
        var group2 = _dataGenerator.GenerateInteger(100).ToString().PadLeft(2, '0');
        var group3 = _dataGenerator.GenerateInteger(100).ToString().PadLeft(2, '0');

        return $"{countryCode} {prefix} {group1} {group2} {group3}";
    }

    public string GetPersonalIdentityNumber()
    {
        return _dataGenerator.GetRandomElement(PersonalIdentityNumbers);
    }
}
