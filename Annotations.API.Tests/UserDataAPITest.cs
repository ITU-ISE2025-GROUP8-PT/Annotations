using Xunit;

namespace Annotations.API.Tests;

public class UserDataAPITest
{
    [Fact]
    public void userTest()
    {
        //arrange
        string firstName = "Mester";
        string lastName = "Jakob";
        //act
        string lyrics = firstName + " " + lastName + " Sover du? Sover du?";
        //assert
        Assert.Equal("Mester Jakob Sover du? Sover du?", lyrics);
    }
}