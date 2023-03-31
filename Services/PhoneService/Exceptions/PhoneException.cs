using System;
namespace VitcAuth.Services.PhoneService.Exceptions
{
    public class PhoneException : Exception
    {
        public PhoneException(string? message) : base(message) { }
    }
}

