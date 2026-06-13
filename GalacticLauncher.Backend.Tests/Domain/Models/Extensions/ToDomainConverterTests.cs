using GalacticLauncher.Core.Models;
using GalacticLauncher.Backend.Domain.Models;
using GalacticLauncher.Backend.Domain.Models.Extensions;

namespace GalacticLauncher.Backend.Tests.Domain.Models.Extensions;

[TestFixture]
public class ToDomainConvertersTests
{
    [Test]
    public void ToDomain_HistoryEntity_MapsPropertiesCorrectly()
    {
        var entity = new HistoryEntity
        {
            Id = 999,
            Info = "Database entry",
            Timestamp = new DateTime(2026, 6, 6, 9, 0, 0, DateTimeKind.Utc),
            IdGame = 111
        };

        History result = entity.ToDomain();

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(entity.Id));
        Assert.That(result.Info, Is.EqualTo(entity.Info));
        Assert.That(result.Timestamp, Is.EqualTo(entity.Timestamp));
        Assert.That(result.IdGame, Is.EqualTo(entity.IdGame));
    }
}