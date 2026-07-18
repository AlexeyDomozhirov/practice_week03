namespace task13console;

using task13;

public class Program
{
    public static void Main(string[] args)
    {
        Run(Console.Out);
    }

    public static void Run(TextWriter output)
    {
        var student = new Student
        {
            FirstName = "Алексей",
            LastName = "Иванов",
            BirthDate = new DateTime(2001, 3, 15),
            Grades = new List<Subject>
            {
                new Subject { Name = "Математика", Grade = 92 },
                new Subject { Name = "Физика", Grade = 88 },
                new Subject { Name = "Информатика", Grade = 95 }
            }
        };

        output.WriteLine("Исходный студент:");
        PrintStudent(student, output);

        string json = JsonStudentSerializer.Serialize(student);
        output.WriteLine("\nСериализованный JSON:");
        output.WriteLine(json);

        var deserializedStudent = JsonStudentSerializer.Deserialize(json);
        output.WriteLine("\nДесериализованный студент:");
        PrintStudent(deserializedStudent, output);

        string filePath = "student.json";
        JsonStudentSerializer.SaveToFile(student, filePath);
        output.WriteLine($"\nСохранён в файл: {filePath}");

        var loadedStudent = JsonStudentSerializer.LoadFromFile(filePath);
        output.WriteLine("\nЗагруженный из файла студент:");
        PrintStudent(loadedStudent, output);

        output.WriteLine("\n--- Проверка валидации ---");
        string invalidJson = @"
        {
            ""FirstName"": ""Пётр"",
            ""LastName"": ""Сидоров"",
            ""BirthDate"": ""2030-01-01"",
            ""Grades"": [
                { ""Name"": ""История"", ""Grade"": 120 }
            ]
        }";
        try
        {
            var invalidStudent = JsonStudentSerializer.Deserialize(invalidJson);
        }
        catch (Exception ex)
        {
            output.WriteLine($"Ошибка валидации: {ex.Message}");
        }

        if (File.Exists(filePath))
            File.Delete(filePath);
    }

    private static void PrintStudent(Student student, TextWriter output)
    {
        output.WriteLine($"  Имя: {student.FirstName}");
        output.WriteLine($"  Фамилия: {student.LastName}");
        output.WriteLine($"  Дата рождения: {student.BirthDate:yyyy-MM-dd}");
        output.WriteLine("  Оценки:");
        foreach (var subj in student.Grades)
        {
            output.WriteLine($"    {subj.Name}: {subj.Grade}");
        }
    }
}
