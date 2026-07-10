namespace task13;

using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

public static class JsonStudentSerializer
{
    private static JsonSerializerOptions GetDefaultOptions()
    {
        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        options.Converters.Add(new DateTimeConverter());
        return options;
    }

    public static string Serialize(Student student)
    {
        var options = GetDefaultOptions();
        return JsonSerializer.Serialize(student, options);
    }

    public static void SaveToFile(Student student, string filePath)
    {
        var json = Serialize(student);
        File.WriteAllText(filePath, json);
    }

    public static Student Deserialize(string json)
    {
        var options = GetDefaultOptions();
        var student = JsonSerializer.Deserialize<Student>(json, options);
        if (student == null)
            throw new InvalidOperationException("Десериализация вернула null.");

        ValidateStudent(student);
        return student;
    }

    public static Student LoadFromFile(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Файл {filePath} не найден.");

        var json = File.ReadAllText(filePath);
        return Deserialize(json);
    }

    private static void ValidateStudent(Student student)
    {
        if (string.IsNullOrWhiteSpace(student.FirstName))
            throw new ArgumentException("FirstName не может быть пустым.");
        if (string.IsNullOrWhiteSpace(student.LastName))
            throw new ArgumentException("LastName не может быть пустым.");
        if (student.BirthDate > DateTime.Now)
            throw new ArgumentException("Дата рождения не может быть в будущем.");
        if (student.Grades == null)
            throw new ArgumentException("Список оценок не может быть null.");

        foreach (var subject in student.Grades)
        {
            if (string.IsNullOrWhiteSpace(subject.Name))
                throw new ArgumentException("Название предмета не может быть пустым.");
            if (subject.Grade < 0 || subject.Grade > 100)
                throw new ArgumentException($"Оценка по предмету {subject.Name} должна быть в диапазоне 0-100.");
        }
    }
}
