using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using pr16_2_7.Models;

namespace pr16_2_7.Services 
{
    public static class CsvService
    {
        public static void Save(string filename, Dictionary<string, Student> students)
        {
            var lines = new List<string>();
            lines.Add("StudentName,Subject,Grade");

            foreach (var student in students.Values)
            {
                foreach (var subject in student.Subjects)
                {
                    lines.Add($"{EscapeCsv(student.Name)},{EscapeCsv(subject.Subject)},{subject.Grade}");
                }
            }

            File.WriteAllLines(filename, lines);
        }

        public static Dictionary<string, Student> Load(string filename)
        {
            var students = new Dictionary<string, Student>();
            var lines = File.ReadAllLines(filename);

            for (int i = 1; i < lines.Length; i++)
            {
                var parts = ParseCsvLine(lines[i]);
                if (parts.Length >= 3)
                {
                    string name = UnescapeCsv(parts[0]);
                    string subject = UnescapeCsv(parts[1]);
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

        private static string EscapeCsv(string value)
        {
            if (value.Contains(",") || value.Contains("\""))
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            return value;
        }

        private static string UnescapeCsv(string value)
        {
            if (value.StartsWith("\"") && value.EndsWith("\""))
                return value.Substring(1, value.Length - 2).Replace("\"\"", "\"");
            return value;
        }

        private static string[] ParseCsvLine(string line)
        {
            var result = new List<string>();
            bool inQuotes = false;
            int start = 0;

            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '"')
                    inQuotes = !inQuotes;
                else if (line[i] == ',' && !inQuotes)
                {
                    result.Add(line.Substring(start, i - start));
                    start = i + 1;
                }
            }
            result.Add(line.Substring(start));

            return result.ToArray();
        }
    }
}