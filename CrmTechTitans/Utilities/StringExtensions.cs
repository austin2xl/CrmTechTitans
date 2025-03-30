using System;
using System.Text.RegularExpressions;

namespace CrmTechTitans.Utilities
{
    public static class StringExtensions
    {
        /// <summary>
        /// Formats a 10-digit phone number into (XXX) XXX-XXXX format
        /// </summary>
        /// <param name="phoneNumber">The raw phone number (digits only)</param>
        /// <returns>Formatted phone number or the original if invalid</returns>
        public static string FormatPhoneNumber(this string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return string.Empty;
            }

            // Remove any non-digit characters
            string digitsOnly = Regex.Replace(phoneNumber, @"\D", "");

            // Check if we have a 10-digit number
            if (digitsOnly.Length == 10)
            {
                return $"({digitsOnly.Substring(0, 3)}) {digitsOnly.Substring(3, 3)}-{digitsOnly.Substring(6)}";
            }
            // Handle 11-digit numbers that may start with 1 (US country code)
            else if (digitsOnly.Length == 11 && digitsOnly.StartsWith("1"))
            {
                return $"({digitsOnly.Substring(1, 3)}) {digitsOnly.Substring(4, 3)}-{digitsOnly.Substring(7)}";
            }
            // If not a valid phone number format, return the original
            return phoneNumber;
        }
    }
} 