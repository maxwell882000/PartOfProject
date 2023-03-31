using System;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using RestSharp;
using VitcAuth.Services.PhoneService.Exceptions;
using VitcAuth.Services.PhoneService.Interfaces;
using VitcAuth.Services.PhoneService.Model;

namespace VitcAuth.Services.PhoneService.Service
{
    public class PhoneService : IPhoneService
    {
        public const string SERVER = "https://notify.eskiz.uz/api/";
        private string token = "";
        private RestClient client;
        private IConfiguration configuration;

        public PhoneService(IConfiguration configuration)
        {
            this.client = new RestClient(SERVER);
            this.configuration = configuration;
        }

        private void authorize()
        {
            var request = new RestRequest("auth/login", Method.Post);
            request.AddJsonBody(new
            {
                email = this.configuration.GetValue<string>("SMS:email"),
                password = this.configuration.GetValue<string>("SMS:password")
            });
            var response = this.client.Execute(request);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new PhoneException("Failed To Authorize to Sms channel");
            }
            var content = JsonSerializer.Deserialize<PhoneAuth>(response.Content);
            var data = content.data;
            this.token = data["token"];
        }

        private string leftOnlyIntegers(string phone)
        {
            return Regex.Replace(phone, @"[^\d]", "");
        }

        public Dictionary<string, dynamic> sendCode(string phone, string message)
        {
            var request = new RestRequest("message/sms/send", Method.Post);
            request.AddHeader("Authorization", "Bearer " + this.token);
            request.AddJsonBody(new
            {
                mobile_phone = this.leftOnlyIntegers(phone),
                message = message,
                from = "Eavtomaktab"
            });
            var response = this.client.Execute(request);
            var content = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(response.Content);
            if (response.StatusCode == HttpStatusCode.Unauthorized
            || (
                content.ContainsKey("status_code") && content["status_code"] == 500
                 ))
            {
                this.authorize();
                return this.sendCode(phone, message);
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                if (content.ContainsKey("message") && content["message"].ContainsKey("mobile_phone"))
                {
                    throw new PhoneException(content["message"]["mobile_phone"][0]);
                }
                throw new PhoneException(response.Content);
            }
            return content;
        }
        
    }
}

