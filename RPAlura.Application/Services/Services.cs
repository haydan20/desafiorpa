using RPAlura.Domain.Entities;
using RPAlura.Domain.Repositories;
using RPAlura.Infrastructure.RPA;

namespace RPAlura.Application.Services
{
    public class Services
    {


        private readonly ICursoRepository _cursoRepository;



        public Services(ICursoRepository cursoRepository)
        {
            _cursoRepository = cursoRepository;

        }

        public void Save(Cursos curso)
        {
            _cursoRepository.Add(curso);
        }

        public void Iniciar(string busca)
        {


            _cursoRepository.CreateDatabaseAndTable();

            var aluraService = new AluraSearchService();

            var lstCursos = aluraService.Search(busca);

            for (int i = 0; i < lstCursos.Count; i++)
            {
                Save(lstCursos[i]);
                Console.WriteLine("Salvando " + (i+1) + " de " + lstCursos.Count );
            }
          

            aluraService.Dispose();


        }


    }
}
