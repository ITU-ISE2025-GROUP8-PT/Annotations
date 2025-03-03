using Xunit;

namespace DefaultNamespace;

public class UserDataAPITest
{
    [Fact]
    public void userTest()
    {
        //arrange
        String firstName = "Mester";
        String lastName = "Jakob";
        //act
        String lyrics = firstName + " " + lastName + " Sover du? Sover du?";
        //assert
        Assert.Equal("mester Jakob Sover du? Sover du?", lyrics);
    }
    
}