using Core.Models;

namespace CoreTests;

public class HoleTests
{
    #region Direction

    [Theory]
    [InlineData(0)]
    [InlineData(360)]
    [InlineData(180)]
    [InlineData(45.5)]
    public void SetDirection_WithValidValue_SetsDirectionCorrectly(double validDirection)
    {
        // Arrange
        var hole = new Hole();

        // Act
        hole.SetDirection(validDirection);

        // Assert
        Assert.Equal(validDirection, hole.Direction);
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(-1)]
    [InlineData(360.1)]
    [InlineData(361)]
    public void SetDirection_WithOutOfRangeValue_ThrowsArgumentOutOfRangeException(
        double invalidDirection
    )
    {
        // Arrange
        var hole = new Hole();

        // Act
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            hole.SetDirection(invalidDirection)
        );

        // Assert
        Assert.Equal("direction", exception.ParamName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(360)]
    public void DirectionSetter_WithValidValue_SetsDirectionCorrectly(double validDirection)
    {
        // Arrange
        var hole = new Hole();

        // Act
        hole.Direction = validDirection;

        // Assert
        Assert.Equal(validDirection, hole.Direction);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(361)]
    public void DirectionSetter_WithOutOfRangeValue_ThrowsArgumentOutOfRangeException(
        double invalidDirection
    )
    {
        // Arrange
        var hole = new Hole();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => hole.Direction = invalidDirection);
    }

    [Fact]
    public void Direction_DefaultValue_IsZero()
    {
        // Arrange
        var hole = new Hole();

        // Act & Assert
        Assert.Equal(0, hole.Direction);
    }

    [Fact]
    public void DirectionSetter_WhenSetToInvalidValueAfterValidValue_KeepsPreviousValidValue()
    {
        // Arrange
        var hole = new Hole();
        hole.Direction = 90;

        // Act
        Assert.Throws<ArgumentOutOfRangeException>(() => hole.Direction = -10);

        // Assert
        Assert.Equal(90, hole.Direction);
    }

    #endregion

    #region Inclination

    [Theory]
    [InlineData(-90)]
    [InlineData(90)]
    [InlineData(0)]
    [InlineData(-45.5)]
    public void SetInclination_WithValidValue_SetsInclinationCorrectly(double validInclination)
    {
        // Arrange
        var hole = new Hole();

        // Act
        hole.SetInclination(validInclination);

        // Assert
        Assert.Equal(validInclination, hole.Inclination);
    }

    [Theory]
    [InlineData(-90.1)]
    [InlineData(-91)]
    [InlineData(90.1)]
    [InlineData(91)]
    public void SetInclination_WithOutOfRangeValue_ThrowsArgumentOutOfRangeException(
        double invalidInclination
    )
    {
        // Arrange
        var hole = new Hole();

        // Act
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            hole.SetInclination(invalidInclination)
        );

        // Assert
        Assert.Equal("inclination", exception.ParamName);
    }

    [Theory]
    [InlineData(-90)]
    [InlineData(90)]
    public void InclinationSetter_WithValidValue_SetsInclinationCorrectly(double validInclination)
    {
        // Arrange
        var hole = new Hole();

        // Act
        hole.Inclination = validInclination;

        // Assert
        Assert.Equal(validInclination, hole.Inclination);
    }

    [Theory]
    [InlineData(-91)]
    [InlineData(91)]
    public void InclinationSetter_WithOutOfRangeValue_ThrowsArgumentOutOfRangeException(
        double invalidInclination
    )
    {
        // Arrange
        var hole = new Hole();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => hole.Inclination = invalidInclination);
    }

    [Fact]
    public void Inclination_DefaultValue_IsZero()
    {
        // Arrange
        var hole = new Hole();

        // Act & Assert
        Assert.Equal(0, hole.Inclination);
    }

    [Fact]
    public void InclinationSetter_WhenSetToInvalidValueAfterValidValue_KeepsPreviousValidValue()
    {
        // Arrange
        var hole = new Hole();
        hole.Inclination = 45;

        // Act
        Assert.Throws<ArgumentOutOfRangeException>(() => hole.Inclination = 91);

        // Assert
        Assert.Equal(45, hole.Inclination);
    }

    #endregion

    #region Status

    [Theory]
    [InlineData(Status.Loaded)]
    [InlineData(Status.Blasted)]
    public void SetStatus_WithInvalidStatus_ThrowsArgumentException(Status invalidStatus)
    {
        // Arrange
        var hole = new Hole();

        // Act
        var exception = Assert.Throws<ArgumentException>(() => hole.SetStatus(invalidStatus));

        // Assert
        Assert.Equal("status", exception.ParamName);
    }

    [Theory]
    [InlineData(Status.Loaded)]
    [InlineData(Status.Blasted)]
    public void StatusSetter_WithInvalidStatus_ThrowsArgumentException(Status invalidStatus)
    {
        // Arrange
        var hole = new Hole();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => hole.Status = invalidStatus);
    }

    [Theory]
    [InlineData(Status.Planned)]
    [InlineData(Status.Charged)]
    [InlineData(Status.Ready)]
    public void SetStatus_WithValidStatus_SetsStatusCorrectly(Status validStatus)
    {
        // Arrange
        var hole = new Hole();

        // Act
        hole.SetStatus(validStatus);

        // Assert
        Assert.Equal(validStatus, hole.Status);
    }

    [Theory]
    [InlineData(Status.Planned)]
    [InlineData(Status.Charged)]
    [InlineData(Status.Ready)]
    public void StatusSetter_WithValidStatus_SetsStatusCorrectly(Status validStatus)
    {
        // Arrange
        var hole = new Hole();

        // Act
        hole.Status = validStatus;

        // Assert
        Assert.Equal(validStatus, hole.Status);
    }

    [Fact]
    public void Status_DefaultValue_IsPlanned()
    {
        // Arrange
        var hole = new Hole();

        // Act & Assert
        Assert.Equal(Status.Planned, hole.Status);
    }

    [Fact]
    public void StatusSetter_WhenSetToInvalidValueAfterValidValue_KeepsPreviousValidValue()
    {
        // Arrange
        var hole = new Hole();
        hole.Status = Status.Ready;

        // Act
        Assert.Throws<ArgumentException>(() => hole.Status = Status.Loaded);

        // Assert
        Assert.Equal(Status.Ready, hole.Status);
    }

    #endregion

    #region Other properties

    [Fact]
    public void Hole_PropertiesCanBeSetAndRetrieved()
    {
        // Arrange
        var id = Guid.NewGuid();
        var blastId = Guid.NewGuid();
        var name = "Test Hole";
        var date = DateTimeOffset.UtcNow;
        var position = new Position
        {
            X = 1.5,
            Y = 2.5,
            Z = -3.5,
        };

        // Act
        var hole = new Hole
        {
            Id = id,
            BlastId = blastId,
            Name = name,
            Position = position,
        };

        // Assert
        Assert.Equal(id, hole.Id);
        Assert.Equal(blastId, hole.BlastId);
        Assert.Equal(name, hole.Name);
        Assert.Same(position, hole.Position);
    }

    [Fact]
    public void Hole_Position_DefaultsToNewInstance()
    {
        // Arrange & Act
        var hole = new Hole();

        // Assert
        Assert.NotNull(hole.Position);
        Assert.Equal(0, hole.Position.X);
        Assert.Equal(0, hole.Position.Y);
        Assert.Equal(0, hole.Position.Z);
    }

    #endregion
}
