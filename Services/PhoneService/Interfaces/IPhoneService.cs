using System;
namespace VitcAuth.Services.PhoneService.Interfaces
{
    public interface IPhoneService
    {
        public Dictionary<string, dynamic> sendCode(string phone, string message);

    }
}

