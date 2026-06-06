using Microsoft.EntityFrameworkCore;

namespace ASP.NETCORE.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly Data.ApplicationDbContext _context;
        public StudentRepository(Data.ApplicationDbContext context)
        {
            _context = context;
        }
        public List<Models.Student> GetAll()
        {
            return _context.Students.Include(s => s.Department).ToList();
        }
        public Models.Student? GetById(int id)
        {
            return _context.Students.Include(s => s.Department).FirstOrDefault(s => s.Id == id);
        }
        public Models.Student Add(Models.Student student)
        {
            _context.Students.Add(student);
            _context.SaveChanges();
            return student;
        }
        public void Delete(int id)
        {
            var student = _context.Students.Find(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                _context.SaveChanges();
            }
        }
    }
}
