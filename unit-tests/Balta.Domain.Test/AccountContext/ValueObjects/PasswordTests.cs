using Balta.Domain.AccountContext.ValueObjects;
using Balta.Domain.AccountContext.ValueObjects.Exceptions;
using Balta.Domain.SharedContext.Abstractions;
using Balta.Domain.SharedContext.Extensions;

namespace Balta.Domain.Test.AccountContext.ValueObjects;

public class PasswordTests
{
    [Fact]
    public void ShouldFailIfPasswordIsNull()
    {
        Assert.Throws<InvalidPasswordException>(() =>
        {
            Password.ShouldCreate(null);
        });
    }

    [Fact]
    public void ShouldFailIfPasswordIsEmpty()
    {
        Assert.Throws<InvalidPasswordException>(() =>
        {
            Password.ShouldCreate(string.Empty);
        });

    }

    [Fact]
    public void ShouldFailIfPasswordIsWhiteSpace()
    {
        Assert.Throws<InvalidPasswordException>(() =>
        {
            Password.ShouldCreate("     ");
        });
    }

    [Fact]
    public void ShouldFailIfPasswordLenIsLessThanMinimumChars()
    {
        string password = "senha";
        Assert.Throws<InvalidPasswordException>(() =>
        {
            Password.ShouldCreate(password);
        });
    }

    [Fact]
    public void ShouldFailIfPasswordLenIsGreaterThanMaxChars()
    {
        string password = "aBcD1fGhIjKl2MnOpQrSt3UvWxYz4AbCdEfGhIjKl5MnOpQrS";
        Assert.Throws<InvalidPasswordException>(() =>
        {
            Password.ShouldCreate(password);
        });
    }

    [Fact]
    public void ShouldHashPassword()
    {

        var plainTextPassword = "Password123!";
        var passwords = Password.ShouldCreate(plainTextPassword);
        Assert.True(Password.ShouldMatch(passwords.Hash, plainTextPassword));

    }

    [Fact]
    public void ShouldVerifyPasswordHash()
    {

        string passwordText = "passwordDeTeste";
        Password password = Password.ShouldCreate(passwordText);

        bool isMatch = Password.ShouldMatch(password.Hash, passwordText);

        Assert.True(isMatch, "senhas diferentes");

    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void ShouldGenerateStrongPassword(bool includeSpecialChars, bool upperCase)
    {

        var passwordGenerated = Password.ShouldGenerate(16, includeSpecialChars, upperCase);
        const string smallLetters = "abcdefghijklmnopqrstuvwxyz";
        const string bigLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string numbers = "1234567890";
        const string special = "!@#$%ˆ&*(){}[];";

        Assert.Equal(16, passwordGenerated.Length);
        Assert.Contains(passwordGenerated, p => numbers.Contains(p));
        Assert.Contains(passwordGenerated, p => smallLetters.Contains(p));

        if (upperCase)
        {
            Assert.Contains(passwordGenerated, p => bigLetters.Contains(p));
        }
        else
        {
            Assert.DoesNotContain(passwordGenerated, p => bigLetters.Contains(p));
        }

        if (includeSpecialChars)
        {
            Assert.Contains(passwordGenerated, p => special.Contains(p));
        }
        else
        {
            Assert.DoesNotContain(passwordGenerated, p => special.Contains(p));
        }
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void ShouldFailIfNonMinimalLengthIsProvided(bool includeSpecialChars, bool upperCase)
    {
        Assert.Throws<InvalidDataException>(() =>
        {
            Password.ShouldGenerate(7, includeSpecialChars, upperCase);
        });
    }

    [Fact]
    public void ShouldImplicitConvertToString()
    {
        var plainTextPassword = "Password123!";
        var password = Password.ShouldCreate(plainTextPassword);

        string passwordAsString = password;

        Assert.Equal(password.Hash, passwordAsString);
    }

    [Fact]
    public void ShouldReturnHashAsStringWhenCallToStringMethod()
    {
        var plainTextPassword = "Password123!";
        var password = Password.ShouldCreate(plainTextPassword);

        string passwordToString = password.ToString();

        Assert.Equal(password.Hash, passwordToString);
    }

    [Fact]
    public void ShouldMarkPasswordAsExpired()
    {
        var plainTextPassword = "Password123!";
        var password = Password.ShouldCreate(plainTextPassword);

        password.MarkAsExpired();

        Assert.NotNull(password.ExpiresAtUtc); 
        Assert.True(password.ExpiresAtUtc <= DateTime.UtcNow);
    }

    [Fact]
    public void ShouldFailIfPasswordIsExpired()
    {
        var plainTextPassword = "Password123!";
        var password = Password.ShouldCreate(plainTextPassword);

        password.MarkAsExpired();

        Assert.Throws<InvalidPasswordException>(() => Password.ShouldVerify(password));
    }

    [Fact]
    public void ShouldMarkPasswordAsMustChange()
    {
        var plainTextPassword = "Password123!";
        var password = Password.ShouldCreate(plainTextPassword);

        password.MarkAsMustChange();

        Assert.True(password.MustChange);
    }

    [Fact]
    public void ShouldFailIfPasswordIsMarkedAsMustChange()
    {
        var plainTextPassword = "Password123!";
        var password = Password.ShouldCreate(plainTextPassword);

        password.MarkAsMustChange();

        Assert.Throws<InvalidPasswordException>(() => Password.ShouldVerify(password));
    }
}