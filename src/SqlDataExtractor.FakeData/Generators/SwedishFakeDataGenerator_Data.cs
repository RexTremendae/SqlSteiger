namespace SqlDataExtractor.FakeData;

public partial class SwedishFakeDataGenerator
{
    private static readonly string[] FirstNamesMale = new[]
    {
        "Adam", "Alexander", "Anders", "August",
        "Bertil", "Björn",
        "Christian",
        "David",
        "Emanuel",
        "Folke", "Fredrik",
        "Gunnar",
        "Hans", "Henrik", "Hugo",
        "Ivar",
        "Jan-Erik",
        "Lars", "Liam", "Lukas",
        "Melker", "Mikael",
        "Nils", "Noah",
        "Oliver",
        "Per-Erik", "Peter",
        "Samuel", "Sixten", "Sture",
        "Tage",
        "Viktor",
        "William",
        "Örjan"
    };

    private static readonly string[] FirstNamesFemale = new[]
    {
        "Alma", "Andrea", "Anita", "Anna-Lena", "Annika",
        "Birgitta", "Britta",
        "Carina",
        "Disa",
        "Elisabeth", "Ellinor",
        "Felicia",
        "Gunborg",
        "Inga-Lill", "Ingrid", "Isabella",
        "Johanna",
        "Karin", "Kerstin",
        "Linda", "Lisa", "Louise",
        "Marie-Louise", "Melissa", "Monica",
        "Rebecka", "Rose-Marie",
        "Sandra", "Sofia",
        "Ulrika",
        "Vera",
        "Wilma"
    };

    private static readonly string[] LastNames = new[]
    {
        "Andersson",
        "Bengtsson", "Bergman", "Blomqvist",
        "Dahlberg",
        "Ekström", "Eriksson",
        "Gunnarsson", "Gyllenstierna",
        "Hansson", "Håkansson", "Holmgren",
        "Isaksson",
        "Johansson", "Jönsson",
        "Karlsson",
        "Larsson", "Lindgren", "Lindholm", "Löfgren",
        "Magnusson", "Modig",
        "Nilsson",
        "Olsson",
        "Persson",
        "Rask",
        "Selander", "Sjöberg", "Svensson", "Söderberg",
        "Tapper",
        "Widman", "Wikström",
        "Åberg",
        "Öberg",
    };

    private static readonly string[] DomainNames = new[]
    {
        "arbalest",
        "bondpiga",
        "citron",
        "fiske", "fylla",
        "kanel", "kanon",
        "liljekonvalj",
        "medioker",
        "obskyr", "ordbok",
        "palatin", "pittoresk", "professionell",
        "regional",
        "samtalspartner", "skor",
        "tandem",
        "undantagstillstånd",
        "vanilj", "vistelse",
        "xylofon"
    };

    private static readonly string[] TopDomains = new[]
    {
        ".eu",
        ".com",
        ".info",
        ".net",
        ".nu",
        ".org",
        ".se"
    };

    private static readonly string[] Addresses = new[]
    {
        "Anna Whitlocks Gata",
        "Brännkyrkagatan",
        "Centralvägen",
        "Dovhjortstigen", "Döbelnsgatan",
        "Frösekevägen",
        "Genvägen", "Gondolgatan", "Gustav Adolfs Väg",
        "Herkulesvägen",
        "Ingentingsgatan",
        "Jacob Hanssons Väg",
        "Kabelgatan", "Karlavägen", "Köpmangatan",
        "Lugna Gatan",
        "Magnus Ladulåsgatan",
        "Nina Einhorns Gata",
        "Olympiavägen",
        "Petréns Allé",
        "Ringvägen",
        "S:t Persgatan", "Snapphanevägen", "Sonja Kovalevskys Gata", "Storgatan", "Strandvägen",
        "Tegnérgatan",
        "Vädursgatan",
        "Waldenströmsplan",
        "Åsgatan",
        "Ängsälvsvägen",
        "Övre Olskroksgatan"
    };

    private static readonly string[] Countries = new[]
    {
        "Australien",
        "Belize",
        "Cypern",
        "Danmark",
        "Estland",
        "Finland", "Frankrike",
        "Grekland",
        "Haiti",
        "Island",
        "Jamaica",
        "Kroatien",
        "Lettland",
        "Maldiverna",
        "Norge", "Nya Zeeland",
        "Oman",
        "Palestina",
        "Qatar",
        "Rwanda",
        "Storbritannien", "Sverige",
        "Tyskland",
        "USA", "Uzbekistan",
        "Vatikanstaten",
        "Zimbabwe",
        "Österrke"
    };

    private static readonly string[] Cities = new[]
    {
        "Allingsås", "Arboga", "Arvika",
        "Boden",
        "Charlottenberg",
        "Djursholm",
        "Eskilstuna", "Eslöv",
        "Fagersta", "Falun",
        "Gränna", "Göteborg",
        "Haparanda", "Hjo", "Hudiksvall",
        "Iggesund",
        "Jönköping",
        "Kalmar",
        "Luleå", "Lund", "Lysekil",
        "Malmö", "Motala",
        "Nässjö",
        "Oskarshamn",
        "Piteå",
        "Ronneby",
        "Stockholm", "Sundsvall", "Sölvesborg",
        "Trosa",
        "Uppsala", "Umeå",
        "Växjö",
        "Ystad",
        "Åtvidaberg",
        "Älmhult",
        "Östersund"
    };

    // Fake personal identities (testpersonnummer) from skatteverket.se
    private static readonly string[] PersonalIdentityNumbers = new[]
    {
        "201711102382", "201706052394", "201701092395",
        "201610222380", "201603222389", "201601182387",
        "201511092395", "201508112396", "201502082397",
        "201410062390", "201406152387", "201401192396",
        "201307252393", "201305052381", "201301092399",
        "201212121212", "201209022399", "201205302381",
        "201111112395", "201108232388", "201103172399",
        "201012282388", "201009112390", "201003172382",
        "200909092389", "200904272390", "200902222397",
        "200812212397", "200810222398", "200808082390",
        "200712222397", "200708272380", "200702132382",
        "200612132381", "200609182381", "200604212399",
        "200508182383", "200506062397", "200503292385",
        "200409242385", "200405302399", "200402082390",
        "200309092393", "200306232398", "200302202395",
        "200211272380", "200208062398", "200204222392",
        "200112262399", "200106272396", "200101012383",
        "200009012394", "200006111181", "200002222396",
        "199906192381", "199501062385", "199403162382",
        "199108192395", "198708122398", "198107202395",
        "198002282393", "197910112395", "197707152398",
        "196604072618", "195807112643", "195008072307",
        "194603213424", "193909268462", "193111158808",
        "192508298037", "192112249202", "191006089807",
        "190807189808", "190308289800", "190103169819"
    };
}