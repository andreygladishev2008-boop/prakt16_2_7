using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace pr16_2_7.Models 
{
    public class Student : INotifyPropertyChanged
    {
        private string _name;
        private List<SubjectGrade> _subjects = new List<SubjectGrade>();

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(AverageGrade));
                OnPropertyChanged(nameof(AverageColor));
                OnPropertyChanged(nameof(GoodPercent));
                OnPropertyChanged(nameof(IsAdmitted));
            }
        }

        public List<SubjectGrade> Subjects
        {
            get => _subjects;
            set
            {
                _subjects = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(AverageGrade));
                OnPropertyChanged(nameof(AverageColor));
                OnPropertyChanged(nameof(GoodPercent));
                OnPropertyChanged(nameof(IsAdmitted));
            }
        }

        public double AverageGrade => Subjects.Count > 0 ? Subjects.Average(s => s.Grade) : 0;

        public string AverageColor
        {
            get
            {
                if (AverageGrade > 4) return "Green";
                if (AverageGrade >= 3) return "Gold";
                return "Red";
            }
        }

        public double GoodPercent
        {
            get
            {
                if (Subjects.Count == 0) return 0;
                int goodGrades = Subjects.Count(s => s.Grade >= 3);
                return (double)goodGrades / Subjects.Count * 100;
            }
        }

        public bool IsAdmitted => GoodPercent >= 60;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class SubjectGrade
    {
        public string Subject { get; set; }
        public int Grade { get; set; }
    }
}