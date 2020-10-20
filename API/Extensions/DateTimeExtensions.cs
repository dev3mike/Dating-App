using System;

namespace API.Extensions
{
    public static class DateTimeExtensions
    {
        public static int CalculateAge(this DateTime dateOfBirth)
        {
            return DateTime.Today.Year - dateOfBirth.Year;
        }
    }
}