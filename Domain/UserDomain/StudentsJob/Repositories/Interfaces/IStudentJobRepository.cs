using VitcAuth.Models;
using VitcLibrary.Repositories.Interfaces;

namespace VitcAuth.Repository.Interfaces
{
    public interface IStudentJobRepository : IGenericRepository<StudentJob>
    {
        public StudentJob UpdateStudent(StudentJob user);
        public StudentJob AddStudent(StudentJob studentJob);
    }
}