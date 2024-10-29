using System.Text;
using Balta.Domain.SharedContext.Extensions;

namespace Balta.Domain.Test.SharedContext.Extensions;

public class StringExtensionsTests
{
    [Fact]
    public void ShouldGenerateBase64FromString()
    {
        var input = "Hello, World!";
        var expectedBase64 = Convert.ToBase64String(Encoding.ASCII.GetBytes(input));

        var actualBase64 = input.ToBase64();

        Assert.Equal(expectedBase64, actualBase64);
    }
}