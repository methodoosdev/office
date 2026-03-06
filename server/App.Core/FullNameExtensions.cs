using App.Core.Domain.Common;
using App.Core.Infrastructure.Mapper;

namespace App.Core
{
    public static class FullNameExtensions
    {
        public static string FullName<T>(this T model) where T : IFullName
        {
            var firstName = model.FirstName;
            var lastName = model.LastName;

            var fullName = string.Empty;
            if (!string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(lastName))
                fullName = $"{lastName} {firstName}";
            else
            {
                if (!string.IsNullOrWhiteSpace(firstName))
                    fullName = firstName;

                if (!string.IsNullOrWhiteSpace(lastName))
                    fullName = lastName;
            }

            return fullName;
        }
        public static string FullNameDecrypt<T>(this T model) where T : IFullName
        {
            var firstName = AesEncryption.Decrypt(model.FirstName);
            var lastName = AesEncryption.Decrypt(model.LastName);

            var fullName = string.Empty;
            if (!string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(lastName))
                fullName = $"{lastName} {firstName}";
            else
            {
                if (!string.IsNullOrWhiteSpace(firstName))
                    fullName = firstName;

                if (!string.IsNullOrWhiteSpace(lastName))
                    fullName = lastName;
            }

            return fullName;
        }
    }
}
