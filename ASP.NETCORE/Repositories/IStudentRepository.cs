using ASP.NETCORE.Models;

namespace ASP.NETCORE.Repositories
{
    public interface IStudentRepository
    {
        List<Student> GetAll();

        Student? GetById(int id);

        Student Add(Student student);

        void Delete(int id);
    }
}
