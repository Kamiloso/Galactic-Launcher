using GalacticLauncher.Core.Models;
using GalacticLauncher.Backend.Domain.Models;
using GalacticLauncher.Backend.Domain.Models.Extensions;

namespace GalacticLauncher.Backend.Tests.Domain.Models.Extensions;

[TestFixture]
public class ToEntityConvertersTests
{
    [Test]
    public void ToEntity_Tag_MapsPropertiesCorrectly()
    {
        var domainTag = new Tag
        {
            Id = 789,
            Name = "Sci-Fi",
            Description = "Science Fiction Games"
        };

        TagEntity result = domainTag.ToEntity();

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(domainTag.Id));
        Assert.That(result.Name, Is.EqualTo(domainTag.Name));
        Assert.That(result.Description, Is.EqualTo(domainTag.Description));
    }

    [Test]
    public void ToEntity_History_MapsPropertiesCorrectly()
    {
        var domainHistory = new History
        {
            Id = 123,
            Info = "Test history log",
            Timestamp = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc),
            IdGame = 456
        };

        HistoryEntity result = domainHistory.ToEntity();

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(domainHistory.Id));
        Assert.That(result.Info, Is.EqualTo(domainHistory.Info));
        Assert.That(result.Timestamp, Is.EqualTo(domainHistory.Timestamp));
        Assert.That(result.IdGame, Is.EqualTo(domainHistory.IdGame));
    }
}