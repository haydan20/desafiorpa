using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using RPAlura.Domain.Entities;
using System.Web;

namespace RPAlura.Infrastructure.RPA
{
    public class AluraSearchService
    {
        private readonly IWebDriver _driver;

        public AluraSearchService()
        {
            _driver = new ChromeDriver();
        }

        //BUSCA NA PAGINA PRINCIPAL
        public List<Cursos> Search(string term)
        {
            _driver.Navigate().GoToUrl("https://www.alura.com.br/");
            var searchBox = _driver.FindElement(By.Id("header-barraBusca-form-campoBusca"));
            searchBox.SendKeys(term);
            searchBox.Submit();
            return GetSearchResults();
        }

        //ENCONTRA O ELEMENTO OU RETORNA NULO
        private IWebElement FindElementOrNull(By by)
        {
            try
            {
                return _driver.FindElement(by);
            }
            catch (NoSuchElementException)
            {
                return null;
            }
        }

        //VERIFICA SE É DIFERENTE DE CURSO
        private bool IsCurso(string linkCurso)
        {
            var nonCursoIdentifiers = new[] { "artigo", "/extra", "/podcast" };
            return !nonCursoIdentifiers.Any(linkCurso.Contains);
        }

        //VERIFICA A QUANTIDADE DE PAGINAS PARA PAGINAÇÃO
        private int GetTotalPages()
        {
            var paginationElement = FindElementOrNull(By.ClassName("busca-paginacao-links"));
            if (paginationElement == null) return 1;

            var pageLinks = paginationElement.FindElements(By.TagName("a"));
            var lastPageLink = pageLinks.LastOrDefault();
            return lastPageLink != null && int.TryParse(lastPageLink.Text, out int lastPageNumber) ? lastPageNumber : 1;
        }

        //ALTERA A PAGINA
        private void ChangePage(int pageNumber)
        {
            var url = _driver.Url;
            var uriBuilder = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["pagina"] = pageNumber.ToString();
            uriBuilder.Query = query.ToString();
            _driver.Navigate().GoToUrl(uriBuilder.ToString());
        }

        //BUSCA INFORMAÇÕES DO CURSO
        private Cursos GetCurseInfo(IWebElement result)
        {
            var tituloElement = result.FindElement(By.ClassName("busca-resultado-nome"));
            var tituloCurso = tituloElement?.Text ?? string.Empty;

            var linkCursoElement = result.FindElement(By.ClassName("busca-resultado-link"));
            var linkCurso = linkCursoElement?.GetAttribute("href") ?? string.Empty;
            if (!IsCurso(linkCurso)) return null;

            var descricaoElement = result.FindElement(By.ClassName("busca-resultado-descricao"));
            var descricao = descricaoElement?.Text ?? string.Empty;

            //NAVEGA PARA PÁGINA DO CURSO
            _driver.Navigate().GoToUrl(linkCurso);

            var instructor = GetInstrutor();

            var durationCurso = GetDurationCurso();



            //VOLTA PARA PAGINA DE PESQUISA
            _driver.Navigate().Back();

            return new Cursos
            {
                Title = tituloCurso,
                Professor = instructor,
                Duration = durationCurso,
                Description = descricao
            };
        }

        private string GetDurationCurso()
        {

            var durationElement = FindElementOrNull(By.ClassName("courseInfo-card-wrapper-infos"));
            if (durationElement == null)
            {
                var HorasCurso = FindElementOrNull(By.ClassName("formacao__info-destaque"));
                if (HorasCurso != null)
                {
                    return HorasCurso.Text;
                }

            }
            else
            {
                return durationElement.Text;
            }


            return string.Empty;

        }



        //BUSCA INSTRUTOR
        private string GetInstrutor()
        {
            var instructorElement = FindElementOrNull(By.ClassName("instructor-title--name"));
            if (instructorElement != null)
            {
                return instructorElement.Text;
            }

            var instrutoresElement = FindElementOrNull(By.Id("instrutores"));
            if (instrutoresElement != null)
            {
                var instructorNames = instrutoresElement.FindElements(By.ClassName("formacao-instrutor-nome"));
                return string.Join(", ", instructorNames.Select(e => e.Text));
            }

            return string.Empty;
        }

        public List<Cursos> GetSearchResults()
        {
            var courses = new List<Cursos>();
            int totalPages = GetTotalPages();

            for (int page = 1; page <= totalPages; page++)
            {
                if (page > 1)
                {
                    ChangePage(page);
                }

                var results = _driver.FindElements(By.CssSelector(".busca-resultado"));
                for (int i = 0; i < results.Count; i++)
                {
                    try
                    {

                        results = _driver.FindElements(By.CssSelector(".busca-resultado"));
                        var curso = GetCurseInfo(results[i]);
                        if (curso != null)
                        {
                            courses.Add(curso);
                        }
                    }
                    catch (NoSuchElementException ex)
                    {
                        Console.WriteLine($"Elemento não encontrado: {ex.Message}");
                        _driver.Navigate().Back();
                    }
                    catch (WebDriverException ex)
                    {
                        Console.WriteLine($"Erro do WebDriver: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro inesperado: {ex.Message}");
                    }
                }
            }

            return courses;
        }

        public void Dispose()
        {
            _driver.Quit();
        }
    }
}
