namespace task13tests;

using task13console;

public class ConsoleSerializationTests
{
    [Fact]
    public void Run_OutputsExpectedStrings_AndDoesNotThrow()
    {
        using var stringWriter = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(stringWriter);

        try
        {
            Program.Run(stringWriter);
            var output = stringWriter.ToString();

            Assert.Contains("Исходный студент:", output);
            Assert.Contains("Сериализованный JSON:", output);
            Assert.Contains("Десериализованный студент:", output);
            Assert.Contains("Сохранён в файл: student.json", output);
            Assert.Contains("Загруженный из файла студент:", output);
            Assert.Contains("--- Проверка валидации ---", output);
            
            Assert.Contains("Ошибка валидации:", output);
            
            Assert.Contains("Алексей", output);
            Assert.Contains("Иванов", output);
            Assert.Contains("2001-03-15", output);
            Assert.Contains("Математика", output);
            Assert.Contains("92", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }
}
