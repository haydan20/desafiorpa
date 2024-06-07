using RPAlura.Domain.Entities;

namespace RPAlura.Domain.Repositories
{
    public interface ICursoRepository
    {
        IEnumerable<Cursos> GetAll();
        Cursos GetById(int id);
        void Add(Cursos curso);
        void CreateDatabaseAndTable();
    }
}
