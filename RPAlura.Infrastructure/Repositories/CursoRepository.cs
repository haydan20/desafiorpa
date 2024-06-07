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

        public void CreateDatabase()
        {
            _dbConnection.Open();

            // Criar a tabela Cursos, se ela não existir
            string createTableQuery = @"
            CREATE TABLE IF NOT EXISTS Cursos (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Title TEXT NULL,
                Professor TEXT NULL,
                Duration INTEGER NULL,
                Description TEXT NULL
            );";

            _dbConnection.Execute(createTableQuery);
        }

        public IEnumerable<Cursos> GetAll()
        {
            return _dbConnection.Query<Cursos>("SELECT * FROM Cursos");
        }

    

        public void Save(Cursos curso)
        {
            string insert = "INSERT INTO Cursos (Title, Professor, Duration, Description) " +
                                 "VALUES (@Title, @Professor, @Duration, @Description)";
            _dbConnection.Execute(insert, curso);
        }
    }
}
