using VitcAuth.Models;
using VitcAuth.Models.Collections;

namespace VitcAuth.Collections.Interfaces
{
    public interface IPaymentSeed : IPaymentCollection
    {
        public void Create(List<User> users);
        public void Create(List<StudentJob> users);
    }
}