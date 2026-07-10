namespace task13;

using System;
using System.Collections.Generic;

public class Student
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public DateTime BirthDate { get; set; }
    public List<Subject> Grades { get; set; } = new List<Subject>();
}
