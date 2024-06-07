
using Microsoft.Extensions.DependencyInjection;
using RPAlura.Application.Services;
using RPAlura.Domain.Repositories;
using RPAlura.Infrastructure.Repositories;
namespace RPAlura.Console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Configuração do serviço de injeção de dependência
            var serviceProvider = new ServiceCollection()
                .AddSingleton<ICursoRepository, CursoRepository>(provider => new CursoRepository("Data Source=:memory:;Mode=Memory;Cache=Shared"))
                .AddTransient<Services>()
                .BuildServiceProvider();

            var cursoService = serviceProvider.GetService<Services>();

            cursoService.Iniciar("rpa");


        }


    }
}

