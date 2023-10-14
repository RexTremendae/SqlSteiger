namespace SqlSteiger.UnitTests.TestDataSets;

using System.Data;

public static class RockBands
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
            new DatabaseColumnMetadata("Id",        SqlDbType.Int,      typeof(int),    IsNullable: false, IsIdentity: true,  IsPrimaryKeyPart: true),
            new DatabaseColumnMetadata("FirstName", SqlDbType.NVarChar, typeof(string), IsNullable: false, IsIdentity: false, IsPrimaryKeyPart: false),
            new DatabaseColumnMetadata("LastName",  SqlDbType.NVarChar, typeof(string), IsNullable: false, IsIdentity: false, IsPrimaryKeyPart: false)
        });

    public static readonly object?[][] PeopleTableData = new object?[][]
    {
        new object?[] {  1, "John",   "Lennon"    },
        new object?[] {  2, "Paul",   "McCartney" },
        new object?[] {  3, "Ringo",  "Starr"     },
        new object?[] {  4, "George", "Harrison"  },
        new object?[] {  5, "James",  "Hetfield"  },
        new object?[] {  6, "Lars",   "Ulrich"    },
        new object?[] {  7, "Kirk",   "Hammet"    },
        new object?[] {  8, "Cliff",  "Burton"    }
    };


    // ▛▀▀▀▀▀▀▀▜
    // ▌ Bands ▐
    // ▙▄▄▄▄▄▄▄▟
    public const string BandsTableName = "Bands";
    public static readonly DatabaseTableMetadata BandsTable = new(
        Schema: SchemaName,
        Name: BandsTableName,
        Columns: new[]
        {
            new DatabaseColumnMetadata("Id",        SqlDbType.Int,      typeof(int),    IsNullable: false, IsIdentity: true,  IsPrimaryKeyPart: true),
            new DatabaseColumnMetadata("Name",      SqlDbType.NVarChar, typeof(string), IsNullable: false, IsIdentity: false, IsPrimaryKeyPart: false),
            new DatabaseColumnMetadata("StartYear", SqlDbType.Int,      typeof(int),    IsNullable: false, IsIdentity: false, IsPrimaryKeyPart: false),
            new DatabaseColumnMetadata("EndYear",   SqlDbType.Int,      typeof(int),    IsNullable: true,  IsIdentity: false, IsPrimaryKeyPart: false)
        });

    public static readonly object?[][] BandsTableData = new object?[][]
    {
        new object?[] {  1,  "the Beatles",  1960,  1970 },
        new object?[] {  1,  "Metallica",    1981,  null }
    };


    // ▛▀▀▀▀▀▀▀▀▀▀▀▀▀▜
    // ▌ BandMembers ▐
    // ▙▄▄▄▄▄▄▄▄▄▄▄▄▄▟
    public const string BandsMembersTableName = "BandMembers";
    public static readonly DatabaseTableMetadata BandsMembersTable = new(
        Schema: SchemaName,
        Name: BandsMembersTableName,
        Columns: new[]
        {
            new DatabaseColumnMetadata("BandId",   SqlDbType.Int, typeof(int), IsNullable: false, IsIdentity: false, IsPrimaryKeyPart: false),
            new DatabaseColumnMetadata("PersonId", SqlDbType.Int, typeof(int), IsNullable: false, IsIdentity: false, IsPrimaryKeyPart: false)
        });

    public static readonly object?[][] BandsMembersTableData = new object?[][]
    {
        new object?[] { 1, 1 },
        new object?[] { 1, 2 },
        new object?[] { 1, 3 },
        new object?[] { 1, 4 },
        new object?[] { 2, 5 },
        new object?[] { 2, 6 },
        new object?[] { 2, 7 },
        new object?[] { 2, 8 }
    };


    // ▛▀▀▀▀▀▀▀▀▜
    // ▌ Albums ▐
    // ▙▄▄▄▄▄▄▄▄▟
    public const string AlbumsTableName = "Albums";
    public static readonly DatabaseTableMetadata AlbumsTable = new(
        Schema: SchemaName,
        Name: AlbumsTableName,
        Columns: new[]
        {
            new DatabaseColumnMetadata("Id",     SqlDbType.Int,      typeof(int),    IsNullable: false, IsIdentity: true,  IsPrimaryKeyPart: true),
            new DatabaseColumnMetadata("BandId", SqlDbType.Int,      typeof(int),    IsNullable: false, IsIdentity: false, IsPrimaryKeyPart: false),
            new DatabaseColumnMetadata("Name",   SqlDbType.NVarChar, typeof(string), IsNullable: false, IsIdentity: false, IsPrimaryKeyPart: false),
            new DatabaseColumnMetadata("Year",   SqlDbType.Int,      typeof(int),    IsNullable: false, IsIdentity: false, IsPrimaryKeyPart: false)
        });

    public static readonly object?[][] AlbumsTableData = new object?[][]
    {
        new object?[] { 1, 1, "Revolver",                              1966 },
        new object?[] { 2, 1, "Sgt. Pepper's Lonely Hearts Club Band", 1967 },
        new object?[] { 3, 1, "Abbey Road",                            1969 },
        new object?[] { 4, 2, "Kill 'em All",                          1983 },
        new object?[] { 4, 2, "Ride the Lightning",                    1984 },
        new object?[] { 4, 2, "Master of Puppets",                     1986 }
    };
}

