using Core.Models;

namespace CoreTests;

public class BlastTests
{
    [Theory]
    [InlineData(Status.Charged)]
    [InlineData(Status.Ready)]
    public void SetStatus_WithInvalidStatus_ThrowsArgumentException(Status invalidStatus)
    {
        // Arrange
        var blast = new Blast();

        // Act
        var exception = Assert.Throws<ArgumentException>(() => blast.SetStatus(invalidStatus));

        // Assert
        Assert.Equal("status", exception.ParamName);
    }

    [Theory]
    [InlineData(Status.Charged)]
    [InlineData(Status.Ready)]
    public void StatusSetter_WithInvalidStatus_ThrowsArgumentException(Status invalidStatus)
    {
        // Arrange
        var blast = new Blast();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => blast.Status = invalidStatus);
    }

    [Theory]
    [InlineData(Status.Planned)]
    [InlineData(Status.Loaded)]
    [InlineData(Status.Blasted)]
    public void SetStatus_WithValidStatus_SetsStatusCorrectly(Status validStatus)
    {
        // Arrange
        var blast = new Blast();

        // Act
        blast.SetStatus(validStatus);

        // Assert
        Assert.Equal(validStatus, blast.Status);
    }

    [Theory]
    [InlineData(Status.Planned)]
    [InlineData(Status.Loaded)]
    [InlineData(Status.Blasted)]
    public void StatusSetter_WithValidStatus_SetsStatusCorrectly(Status validStatus)
    {
        // Arrange
        var blast = new Blast();

        // Act
        blast.Status = validStatus;

        // Assert
        Assert.Equal(validStatus, blast.Status);
    }

    [Fact]
    public void StatusSetter_WhenSetToInvalidValueAfterValidValue_KeepsPreviousValidValue()
    {
        // Arrange
        var blast = new Blast();
        blast.Status = Status.Loaded;

        // Act
        Assert.Throws<ArgumentException>(() => blast.Status = Status.Charged);

        // Assert : la valeur précédente doit être conservée
        Assert.Equal(Status.Loaded, blast.Status);
    }

    [Fact]
    public void Status_DefaultValue_IsPlanned()
    {
        // Arrange
        var blast = new Blast();

        // Act & Assert
        // Status.Planned correspond à la valeur 0 de l'enum,
        // c'est donc la valeur par défaut du champ privé _status
        // avant tout appel à SetStatus. Elle est valide (ni Charged ni Ready).
        Assert.Equal(Status.Planned, blast.Status);
    }

    [Fact]
    public void Blast_PropertiesCanBeSetAndRetrieved()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "Test Blast";
        var date = DateTimeOffset.UtcNow;

        // Act
        var blast = new Blast
        {
            Id = id,
            Name = name,
            DateBlasted = date,
        };

        // Assert
        Assert.Equal(id, blast.Id);
        Assert.Equal(name, blast.Name);
        Assert.Equal(date, blast.DateBlasted);
    }
}
