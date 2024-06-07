using System.Data;
using System.Data.SQLite;
using RPAlura.Domain.Entities;
using RPAlura.Domain.Repositories;
using Dapper;

namespace RPAlura.Infrastructure.Repositories
{
    public class CursoRepository : ICursoRepository
    {
        private readonly IDbConnection _dbConnection;

        public CursoRepository(string connectionString)
        {
            _dbConnection = new SQLiteConnection(connectionString);
        }

        public void CreateDatabaseAndTable()
        {
            _dbConnection.Open();

            // Criar a tabela Cursos, se ela não existir
            string createTableQuery = @"
            CREATE TABLE IF NOT EXISTS Cursos (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Titulo TEXT NULL,
                Professor TEXT NULL,
                QuantidadeHoras INTEGER NULL,
                Descricao TEXT NULL
            );";

            _dbConnection.Execute(createTableQuery);
        }

        public IEnumerable<Cursos> GetAll()
        {
            return _dbConnection.Query<Cursos>("SELECT * FROM Cursos");
        }

        public Cursos GetById(int id)
        {
            return _dbConnection.QuerySingleOrDefault<Cursos>("SELECT * FROM Cursos WHERE Id = @Id", new { Id = id });
        }

        public void Add(Cursos curso)
        {
            string insertQuery = "INSERT INTO Cursos (Titulo, Professor, QuantidadeHoras, Descricao) " +
                                 "VALUES (@Title, @Professor, @Duration, @Description)";
            _dbConnection.Execute(insertQuery, curso);
        }
    }
}
