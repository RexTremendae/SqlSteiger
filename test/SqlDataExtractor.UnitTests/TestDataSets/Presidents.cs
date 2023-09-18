namespace SqlDX.UnitTests.TestDataSets;

using System.Data;

public static class Presidents
{
    public const string SchemaName = "dbo";

    // ▛▀▀▀▀▀▀▀▀▜
    // ▌ People ▐
    // ▙▄▄▄▄▄▄▄▄▟
    public const string PeopleTableName = "People";
    public static readonly DatabaseTableMetadata PeopleTable = new(
        Schema: SchemaName,
        Name: PeopleTableName,
        Columns: new[]
        {
            new DatabaseColumnMetadata("Id",          SqlDbType.Int,      typeof(int),    IsNullable: false, IsIdentity: true,  IsPrimaryKeyPart: true),
            new DatabaseColumnMetadata("FirstName",   SqlDbType.NVarChar, typeof(string), IsNullable: false, IsIdentity: false, IsPrimaryKeyPart: false),
            new DatabaseColumnMetadata("LastName",    SqlDbType.NVarChar, typeof(string), IsNullable: false, IsIdentity: false, IsPrimaryKeyPart: false),
            new DatabaseColumnMetadata("YearOfBirth", SqlDbType.Int,      typeof(int),    IsNullable: false, IsIdentity: false, IsPrimaryKeyPart: false),
            new DatabaseColumnMetadata("YearOfDeath", SqlDbType.Int,      typeof(int),    IsNullable: true,  IsIdentity: false, IsPrimaryKeyPart: false),
            new DatabaseColumnMetadata("BirthState",  SqlDbType.Int,      typeof(int),    IsNullable: false, IsIdentity: false, IsPrimaryKeyPart: false),
        });

    public static readonly object?[][] PeopleTableData = new object?[][]
    {
        new object?[] {  1, "George",      "Washington",   1732, 1799, 1 },
        new object?[] {  2, "John",        "Adams",        1735, 1826, 2 },
        new object?[] {  3, "Thomas",      "Jefferson",    1743, 1826, 1 },
        new object?[] {  4, "James",       "Madison",      1751, 1836, 1 },
        new object?[] {  5, "James",       "Monroe",       1758, 1831, 1 },
        new object?[] {  6, "John",        "Quincy Adams", 1767, 1848, 2 },
        new object?[] {  7, "Andrew",      "Jackson",      1767, 1845, 3 },
        new object?[] {  8, "Abraham",     "Lincoln",      1809, 1865, 5 },
        new object?[] {  9, "Theodore",    "Roosevelt",    1901, 1909, 6 },
        new object?[] { 10, "Franklin D.", "Roosevelt",    1901, 1909, 6 }
    };


    // ▛▀▀▀▀▀▀▀▀▀▀▀▀▜
    // ▌ Precidency ▐
    // ▙▄▄▄▄▄▄▄▄▄▄▄▄▟
    public const string PrecidencyTableName = "Precidency";
    public static readonly DatabaseTableMetadata PrecidencyTable = new(
        Schema: SchemaName,
        Name: PrecidencyTableName,
        Columns: new[]
        {
            new DatabaseColumnMetadata("Id",        SqlDbType.Int, typeof(int), IsNullable: false, IsIdentity: true,  IsPrimaryKeyPart: true),
            new DatabaseColumnMetadata("PersonId",  SqlDbType.Int, typeof(int), IsNullable: false, IsIdentity: false, IsPrimaryKeyPart: false),
            new DatabaseColumnMetadata("StartYear", SqlDbType.Int, typeof(int), IsNullable: false, IsIdentity: false, IsPrimaryKeyPart: false),
            new DatabaseColumnMetadata("EndYear",   SqlDbType.Int, typeof(int), IsNullable: true,  IsIdentity: false, IsPrimaryKeyPart: false)
        });

    public static readonly object?[][] PrecidencyTableData = new object?[][]
    {
        new object?[] {  1,  1, 1789, 1797 },
        new object?[] {  2,  2, 1797, 1801 },
        new object?[] {  3,  3, 1801, 1809 },
        new object?[] {  4,  4, 1809, 1817 },
        new object?[] {  5,  5, 1817, 1825 },
        new object?[] {  6,  6, 1825, 1829 },
        new object?[] {  7,  7, 1829, 1837 },
        new object?[] { 16,  8, 1861, 1865 },
        new object?[] { 25,  9, 1901, 1909 },
        new object?[] { 31, 10, 1933, 1945 }
    };


    // ▛▀▀▀▀▀▀▀▀▜
    // ▌ States ▐
    // ▙▄▄▄▄▄▄▄▄▟
    public const string StatesTableName = "States";
    public static readonly DatabaseTableMetadata StatesTable = new(
        Schema: SchemaName,
        Name: StatesTableName,
        Columns: new[]
        {
            new DatabaseColumnMetadata("Id",          SqlDbType.Int,      typeof(int),    IsNullable: false, IsIdentity: true,  IsPrimaryKeyPart: true),
            new DatabaseColumnMetadata("Name",        SqlDbType.NVarChar, typeof(string), IsNullable: false, IsIdentity: false, IsPrimaryKeyPart: false),
            new DatabaseColumnMetadata("YearOfUnion", SqlDbType.Int,      typeof(int),    IsNullable: false, IsIdentity: false, IsPrimaryKeyPart: false)
        });

    public static readonly object?[][] StatesTableData = new object?[][]
    {
        new object?[] { 1, "Virgina",        1788 },
        new object?[] { 2, "Massachusetts",  1788 },
        new object?[] { 3, "North Carolina", 1789 },
        new object?[] { 4, "South Carolina", 1788 },
        new object?[] { 5, "Kentucky",       1792 },
        new object?[] { 6, "New York",       1788 }
    };
}

