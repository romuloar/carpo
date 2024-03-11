//using Carpo.Data.EFCore.Test.Migrations;

namespace Carpo.Data.EFCore.Test
{
    public record Documento
    {
        public int Id { get;set; }
        public string Name { get;set; }
    }

    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            //var init = new Initial();
            //init.Test();

            var doc = new Documento
            {
                Id = 1,
                Name = "Test doc",
            };
            Test2("param1", 123, "param2", "Hello", "param3", DateTime.Now, new { Name = "Romulo", Age = 40}, doc);

        }

        public void Test2(params object[] parameters) {
            if(parameters != null)
            {
                foreach (var item in parameters)
                {
                   
                }
                for (int i = 0; i < parameters.Length; i += 2)
                {
                    string paramName = parameters[i].ToString();
                    object paramValue = parameters[i + 1];

                    // Exibe o nome e o valor do parâmetro
                    Console.WriteLine($"Nome do Parâmetro: {paramName}, Valor do Parâmetro: {paramValue}");
                }
            }
        }
    }
}