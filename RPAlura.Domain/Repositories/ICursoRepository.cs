using RPAlura.Domain.Entities;

namespace RPAlura.Domain.Repositories
{
    public interface ICursoRepository
    {
  
        void Save(Cursos curso);
        void CreateDatabase();
        IEnumerable<Cursos> GetAll();
    }
}
