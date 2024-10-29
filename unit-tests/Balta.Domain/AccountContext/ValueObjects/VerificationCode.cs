using Balta.Domain.AccountContext.ValueObjects.Exceptions;
using Balta.Domain.SharedContext.Abstractions;

namespace Balta.Domain.AccountContext.ValueObjects;

public class VerificationCode
{
    #region Constants

    private const int MinLength = 6;

    #endregion

    #region Constructors

    private VerificationCode(string code, DateTime expiresAtUtc)
    {
        Code = Guid.NewGuid().ToString("N")[..MinLength].ToUpper();
        ExpiresAtUtc = DateTime.UtcNow.AddMinutes(5);
    }

    #endregion

    #region Factories

    public static VerificationCode ShouldCreate(IDateTimeProvider dateTimeProvider) =>
        new(
            Guid.NewGuid().ToString("N")[..MinLength].ToUpper(),
            dateTimeProvider.UtcNow.AddMinutes(5));

    #endregion

    #region Properties

    public string Code { get; }
    public DateTime? ExpiresAtUtc { get; private set; }
    public DateTime? VerifiedAtUtc { get; private set; }
    public bool IsPending => VerifiedAtUtc == null && ExpiresAtUtc > DateTime.UtcNow;
    public bool IsActive => VerifiedAtUtc != null && ExpiresAtUtc is null;

    #endregion

    #region Methods

    public void ShouldVerify(string code)
    {
        if (string.IsNullOrEmpty(code) || string.IsNullOrWhiteSpace(code))
            throw new InvalidVerificationCodeException();

        if (code.Length != MinLength)
            throw new InvalidVerificationCodeException();

        if (ExpiresAtUtc <= DateTime.UtcNow)
            throw new InvalidVerificationCodeException();

        if (!IsPending)
            throw new InvalidVerificationCodeException();

        if (Code != code)
            throw new InvalidVerificationCodeException();

        VerifiedAtUtc = DateTime.UtcNow;
        ExpiresAtUtc = null;
    }

    #endregion

    #region Operators

    public static implicit operator string(VerificationCode verificationCode) => verificationCode.ToString();

    #endregion

    #region Others

    public override string ToString() => Code;

    #endregion
}