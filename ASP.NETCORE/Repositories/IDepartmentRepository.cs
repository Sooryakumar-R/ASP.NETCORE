using ASP.NETCORE.Models;
namespace ASP.NETCORE.Repositories
{
    public interface IDepartmentRepository
    {
        IEnumerable<Department> GetAll();
        Department GetById(int id);
        Department Add(Department department);

    }
}
