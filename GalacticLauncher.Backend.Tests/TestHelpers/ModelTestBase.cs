using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace GalacticLauncher.Backend.Tests.TestHelpers; 

public abstract class ModelTestBase<T> where T : class
{
    private Type? _classType;

    private Type ClassType
    {
        get
        {
            return this._classType!;
        }
    }

    [SetUp]
    public void SetUp()
    {
        this._classType = typeof(T);
    }

    protected void AssertThatClassIsPublic(bool isSealed)
    {
        Assert.That(this.ClassType.IsClass, Is.True);
        Assert.That(this.ClassType.IsPublic, Is.True);
        Assert.That(this.ClassType.IsAbstract, Is.False);
        Assert.That(this.ClassType.IsSealed, isSealed ? Is.True : Is.False);
    }

    protected PropertyInfo AssertThatClassHasProperty(string propertyName, Type expectedPropertyType, string? columnName)
    {
        var propertyInfo = this.ClassType.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);

        Assert.That(propertyInfo, Is.Not.Null);
        Assert.That(propertyInfo.PropertyType, Is.EqualTo(expectedPropertyType));

        Assert.That(propertyInfo.GetMethod!.IsPublic, Is.True);
        Assert.That(propertyInfo.SetMethod!.IsPublic, Is.True);

        if (columnName is not null)
        {
            var columnAttribute = propertyInfo.GetCustomAttribute<ColumnAttribute>();
            Assert.That(columnAttribute, Is.Not.Null);
            Assert.That(columnAttribute!.Name, Is.EqualTo(columnName));
        }

        return propertyInfo;
    }
}