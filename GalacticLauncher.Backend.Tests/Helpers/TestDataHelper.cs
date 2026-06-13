using Bogus;
using GalacticLauncher.Backend.Domain.Models;
using GalacticLauncher.Core.Models;

namespace GalacticLauncher.Backend.Tests.Helpers;

internal static class TestDataHelper
{
    public static AppConfig CreateDummyConfig(
        string adminUser = "test_admin", 
        string adminPass = "secure_password",
        int sessionSeconds = 3600,
        int gracePeriodSeconds = 0)
    {
        var dummyRule = new AppConfig.RateLimitingSection.RateLimitRule { Limit = 10, Seconds = 10 };
        return new AppConfig
        {
            Admin = new AppConfig.AdminSection 
            { 
                AdminSessionSeconds = sessionSeconds, 
                GracePeriodSeconds = gracePeriodSeconds, 
                Logins = [new() { Username = adminUser, Password = adminPass }] 
            },
            Listener = new AppConfig.ListenerSection { PrefixIPv4 = 24, PrefixIPv6 = 64, UseForwardedFor = false },
            Database = new AppConfig.DatabaseSection { Address = "localhost", Port = 3306, Database = "db", User = "usr", Password = "pwd" },
            Limiter = new AppConfig.RateLimitingSection { LowCost = dummyRule, MediumCost = dummyRule, HighCost = dummyRule, TelemetryCost = dummyRule, ReqCost = dummyRule },
            History = new AppConfig.HistorySection { MaxEntries = 100, CleanupIntervalSeconds = 3600, PageSize = 20 }
        };
    }
    
    public static GameData CreateDummyGameData() => new Faker<GameData>()
        .CustomInstantiator(f => new GameData
        {
            Id = f.Random.Long(1, 10000),
            Name = f.Commerce.ProductName(),
            Author = f.Name.FullName(),
            Description = f.Lorem.Paragraph(),
            IconUrl = f.Internet.Url(),
            TagIdList = null,
            Versions = [],
            Images = []
        }).Generate();

    public static Game CreateDummyGame() => new Faker<Game>()
        .CustomInstantiator(f => new Game
        {
            Id = f.Random.Long(1, 10000),
            Name = f.Commerce.ProductName(),
            Author = f.Name.FullName(),
            Description = f.Lorem.Paragraph(),
            IconUrl = f.Internet.Url(),
            TagIdList = $"{f.Random.Long(1, 10)},{f.Random.Long(11, 20)}"
        }).Generate();

    public static GamePlusEntity CreateDummyGamePlusEntity() => new Faker<GamePlusEntity>()
        .CustomInstantiator(f => new GamePlusEntity
        {
            Id = f.Random.Long(1, 10000),
            Name = f.Commerce.ProductName(),
            Author = f.Name.FullName(),
            Description = f.Lorem.Sentence(),
            IconUrl = f.Internet.Url(),
            TagIdList = null
        }).Generate();

    public static GameEntity CreateDummyGameEntity() => new Faker<GameEntity>()
        .CustomInstantiator(f => new GameEntity
        {
            Id = f.Random.Long(1, 10000),
            Name = f.Commerce.ProductName(),
            Author = f.Name.FullName(),
            Description = f.Lorem.Sentence()
        }).Generate();

    public static TagEntity CreateDummyTagEntity() => new Faker<TagEntity>()
        .CustomInstantiator(f => new TagEntity
        {
            Id = f.Random.Long(1, 10000),
            Name = f.Commerce.Department(),
            Description = f.Lorem.Sentence()
        }).Generate();

    public static VersionEntity CreateDummyVersionEntity() => new Faker<VersionEntity>()
        .CustomInstantiator(f => new VersionEntity
        {
            Id = f.Random.Long(1, 10000),
            Caption = f.System.Semver(),
            Type = "release",
            Description = f.Lorem.Sentence(),
            CliArgs = "",
            IsPrimary = f.Random.Bool(),
            ReleaseDate = DateOnly.FromDateTime(f.Date.Past()),
            Platform = "windows",
            DownloadUrl = f.Internet.Url(),
            ExecLocation = "run.exe",
            Sha256Hash = f.Random.Hash(),
            Alert = "stable",
            IdGame = f.Random.Long(1, 10000)
        }).Generate();

    public static ImageEntity CreateDummyImageEntity() => new Faker<ImageEntity>()
        .CustomInstantiator(f => new ImageEntity
        {
            Id = f.Random.Long(1, 10000),
            DownloadUrl = f.Internet.Url(),
            Type = "icon",
            SortIndex = f.Random.Int(0, 10),
            IdGame = f.Random.Long(1, 10000)
        }).Generate();

    public static GameTree CreateDummyGameTree() => new Faker<GameTree>()
        .CustomInstantiator(f => new GameTree
        {
            Id = f.Random.Long(1, 10000),
            Name = f.Commerce.ProductName(),
            Author = f.Name.FullName(),
            Description = f.Lorem.Sentence(),
            Versions = [],
            Images = [],
            TagIds = [f.Random.Long(1, 100)]
        }).Generate();

    public static GameRaw CreateDummyGameRaw() => new Faker<GameRaw>()
        .CustomInstantiator(f => new GameRaw
        {
            Id = f.Random.Long(1, 10000),
            Name = f.Commerce.ProductName(),
            Author = f.Name.FullName(),
            Description = f.Lorem.Sentence()
        }).Generate();

    public static Tag CreateDummyTag() => new Faker<Tag>()
        .CustomInstantiator(f => new Tag
        {
            Id = f.Random.Long(1, 10000),
            Name = f.Commerce.Department(),
            Description = f.Lorem.Sentence()
        }).Generate();
}