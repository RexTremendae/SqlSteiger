namespace SqlSteiger.FakeData;

public interface IFakeDataGenerator
{
    public string GenerateFirstName();

    public string GenerateLastName();

    public string GenerateEmail();

    public string GetAddressLine();

    public string GetPostalcode();

    public string GetCity();

    public string GetCountry();

    public string GetPhoneNumber();

    public string GetPersonalIdentityNumber();
}
