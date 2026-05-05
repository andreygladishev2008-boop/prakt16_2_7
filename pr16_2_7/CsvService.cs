using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using StudentApp.Models;    

namespace pr16_2_7
{
    public class CsvService
    {
        private const string Header = "Студент,Предмет,Оценка";

        // Сохранить всех студентов в CSV
        public static void Save(string filePath, Dictionary<string, Student> students)
        {
            using var writer = new StreamWriter(filePath, false, Encoding.UTF8);
            writer.WriteLine(Header);

            foreach (var student in students)
            {
                foreach (var subject in student.Value.Subjects)
                {
                    writer.WriteLine($"{student.Key},{subject.Subject},{subject.Grade}");
                }
            }
        }

        // Загрузить студентов из CSV
        public static Dictionary<string, Student> Load(string filePath)
        {
            var lines = File.ReadAllLines(filePath, Encoding.UTF8);
            if (lines.Length < 2)
                throw new Exception("Файл пуст");

            var students = new Dictionary<string, Student>();

            for (int i = 1; i < lines.Length; i++)
            {
                var parts = lines[i].Split(',');
                if (parts.Length >= 3)
                {
                    string name = parts[0].Trim();
                    string subject = parts[1].Trim();

                    if (int.TryParse(parts[2], out int grade))
                    {
                        if (!students.ContainsKey(name))
                            students[name] = new Student { Name = name };

                        students[name].Subjects.Add(new SubjectGrade
                        {
                            Subject = subject,
                            Grade = grade
                        });
                    }
                }
            }

            return students;
        }
    }
}
