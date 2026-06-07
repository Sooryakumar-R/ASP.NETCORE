using ASP.NETCORE.Data;
using ASP.NETCORE.Models;

namespace ASP.NETCORE.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly Data.ApplicationDbContext _context;
        public DepartmentRepository(ApplicationDbContext conetext) 
        {
            _context = conetext;
        }

        public Department Add(Department department)
        {
            _context.Departments.Add(department);
            _context.SaveChanges();
            return department;
        }

        public IEnumerable<Department> GetAll()
        {
            return _context.Departments;
        }

        public Department GetById(int id)
        {
            return _context.Departments.Find(id);
        }

    }
}
