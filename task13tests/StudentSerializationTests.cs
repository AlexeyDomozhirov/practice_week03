namespace task13tests;

using task13;

public class StudentSerializationTests
{
    [Fact]
    public void Serialize_Student_ReturnsValidJson()
    {
        var student = new Student
        {
            FirstName = "Иван",
            LastName = "Петров",
            BirthDate = new DateTime(2000, 5, 15),
            Grades = new List<Subject>
            {
                new Subject { Name = "Математика", Grade = 85 },
                new Subject { Name = "Физика", Grade = 90 }
            }
        };

        var json = JsonStudentSerializer.Serialize(student);

        Assert.Contains("\"FirstName\": \"Иван\"", json);
        Assert.Contains("\"LastName\": \"Петров\"", json);
        Assert.Contains("\"BirthDate\": \"2000-05-15\"", json);
        Assert.Contains("\"Name\": \"Математика\"", json);
        Assert.Contains("\"Grade\": 85", json);
        Assert.DoesNotContain("\"null\"", json);
    }

    [Fact]
    public void Deserialize_ValidJson_ReturnsStudent()
    {
        var json = @"{
            ""FirstName"": ""Анна"",
            ""LastName"": ""Смирнова"",
            ""BirthDate"": ""1995-08-20"",
            ""Grades"": [
                { ""Name"": ""Химия"", ""Grade"": 78 }
            ]
        }";

        var student = JsonStudentSerializer.Deserialize(json);

        Assert.Equal("Анна", student.FirstName);
        Assert.Equal("Смирнова", student.LastName);
        Assert.Equal(new DateTime(1995, 8, 20), student.BirthDate);
        Assert.Single(student.Grades);
        Assert.Equal("Химия", student.Grades[0].Name);
        Assert.Equal(78, student.Grades[0].Grade);
    }

    [Fact]
    public void Deserialize_InvalidDate_ThrowsException()
    {
        var json = @"{""FirstName"":""Петр"",""LastName"":""Иванов"",""BirthDate"":""2030-01-01"",""Grades"":[]}";

        Assert.Throws<ArgumentException>(() => JsonStudentSerializer.Deserialize(json));
    }

    [Fact]
    public void Deserialize_InvalidGrade_ThrowsException()
    {
        var json = @"{
            ""FirstName"": ""Ольга"",
            ""LastName"": ""Кузнецова"",
            ""BirthDate"": ""1990-03-10"",
            ""Grades"": [
                { ""Name"": ""Биология"", ""Grade"": 105 }
            ]
        }";

        var ex = Assert.Throws<ArgumentException>(() => JsonStudentSerializer.Deserialize(json));
        Assert.Contains("должна быть в диапазоне 0-100", ex.Message);
    }

    [Fact]
    public void SaveAndLoad_Student_WorksCorrectly()
    {
        var original = new Student
        {
            FirstName = "Сергей",
            LastName = "Сидоров",
            BirthDate = new DateTime(2002, 11, 5),
            Grades = new List<Subject>
            {
                new Subject { Name = "Информатика", Grade = 95 }
            }
        };
        var filePath = "test_student.json";

        try
        {
            JsonStudentSerializer.SaveToFile(original, filePath);
            var loaded = JsonStudentSerializer.LoadFromFile(filePath);

            Assert.Equal(original.FirstName, loaded.FirstName);
            Assert.Equal(original.LastName, loaded.LastName);
            Assert.Equal(original.BirthDate, loaded.BirthDate);
            Assert.Equal(original.Grades.Count, loaded.Grades.Count);
            Assert.Equal(original.Grades[0].Name, loaded.Grades[0].Name);
            Assert.Equal(original.Grades[0].Grade, loaded.Grades[0].Grade);
        }
        finally
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }
}
