using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pr16_2_7
{
    public class Student
    {
        public string Name { get; set; }
        public List<SubjectGrade> Subjects { get; set; } = new();

        // Средняя оценка
        public double AverageGrade => Subjects.Count > 0
            ? Subjects.Average(s => s.Grade)
            : 0;

        // Процент хороших оценок (4 и 5)
        public double GoodPercent => Subjects.Count > 0
            ? (double)Subjects.Count(s => s.Grade >= 4) / Subjects.Count * 100
            : 0;

        // Допущен к сессии (>=60% хороших оценок)
        public bool IsAdmitted => GoodPercent >= 60;

        // Цвет средней оценки (для индикатора)
        public string AverageColor => AverageGrade switch
        {
            > 4 => "Green",
            >= 3 => "Gold",
            _ => "Red"
        };
    }

    public class SubjectGrade
    {
        public string Subject { get; set; }
        public int Grade { get; set; }
    }
}
