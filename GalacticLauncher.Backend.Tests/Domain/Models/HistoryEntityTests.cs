using GalacticLauncher.Backend.Domain.Models;
using GalacticLauncher.Backend.Tests.TestHelpers;

namespace GalacticLauncher.Backend.Tests.Domain.Models;

[TestFixture]
public class HistoryEntityTests : ModelTestBase<HistoryEntity>
{
    [Test]
    public void IsPublicClass()
    {
        this.AssertThatClassIsPublic(isSealed: false);
    }
    
    [TestCase("Id", typeof(long), null)]
    [TestCase("Info", typeof(string), null)]
    [TestCase("Timestamp", typeof(DateTime), null)]
    [TestCase("IdGame", typeof(long?), null)]
    public void HasProperty(string propertyName, Type propertyType, string? columnName)
    {
        this.AssertThatClassHasProperty(propertyName, propertyType, columnName!);
    }
}