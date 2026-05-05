using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;
using StudentApp.Models;
using StudentApp.Services;

namespace pr16_2_7
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<string, Student> students = new();
        private Student currentStudent;

        public MainWindow()
        {
            InitializeComponent();
        }

        // Добавление оценки
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            string name = txtName.Text.Trim();
            string subject = txtSubject.Text.Trim();

            if (!int.TryParse(txtGrade.Text, out int grade) || grade < 1 || grade > 5)
            {
                MessageBox.Show("Оценка должна быть от 1 до 5!");
                return;
            }

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(subject))
            {
                MessageBox.Show("Введите имя студента и предмет!");
                return;
            }

            if (!students.ContainsKey(name))
                students[name] = new Student { Name = name };

            students[name].Subjects.Add(new SubjectGrade { Subject = subject, Grade = grade });

            UpdateStudentList();
            ClearInputs();
        }

        // Выбор студента
        private void CmbStudents_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (cmbStudents.SelectedItem != null)
            {
                string selectedName = cmbStudents.SelectedItem.ToString();
                currentStudent = students[selectedName];
                UpdateStudentInfo();
            }
        }

        // Сохранение в CSV
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (students.Count == 0)
            {
                MessageBox.Show("Нет данных!");
                return;
            }

            var dialog = new SaveFileDialog { Filter = "CSV|*.csv", DefaultExt = ".csv" };
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    CsvService.Save(dialog.FileName, students);
                    MessageBox.Show("Сохранено!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
        }

        // Загрузка из CSV
        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog { Filter = "CSV|*.csv" };
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    students = CsvService.Load(dialog.FileName);
                    UpdateStudentList();
                    MessageBox.Show($"Загружено {students.Count} студентов!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
        }

        // Вспомогательные методы
        private void UpdateStudentList()
        {
            cmbStudents.ItemsSource = null;
            cmbStudents.ItemsSource = students.Keys.ToList();

            if (students.Any())
            {
                currentStudent = students.Values.First();
                UpdateStudentInfo();
            }
            else
            {
                ClearDisplay();
            }
        }

        private void UpdateStudentInfo()
        {
            if (currentStudent == null) return;

            // Средняя оценка
            tbAverage.Text = currentStudent.AverageGrade.ToString("F2");

            // Цветной индикатор
            borderAverage.Background = currentStudent.AverageColor switch
            {
                "Green" => new SolidColorBrush(Colors.LightGreen),
                "Gold" => new SolidColorBrush(Colors.Gold),
                "Red" => new SolidColorBrush(Colors.LightCoral),
                _ => borderAverage.Background
            };

            // Прогресс-бар
            progressBar.Value = currentStudent.GoodPercent;
            tbPercent.Text = $"{currentStudent.GoodPercent:F0}%";

            // Цвет прогресс-бара
            progressBar.Foreground = currentStudent.GoodPercent >= 60
                ? new SolidColorBrush(Colors.Green)
                : new SolidColorBrush(Colors.Red);

            // Статус
            tbStatus.Text = currentStudent.IsAdmitted
                ? "ДОПУЩЕН К СЕССИИ"
                : "НЕ ДОПУЩЕН К СЕССИИ";
        }

        private void ClearDisplay()
        {
            tbAverage.Text = "0";
            borderAverage.Background = new SolidColorBrush(Colors.LightGray);
            progressBar.Value = 0;
            tbPercent.Text = "0%";
            tbStatus.Text = " НЕТ СТУДЕНТОВ";
        }

        private void ClearInputs()
        {
            txtName.Text = "";
            txtSubject.Text = "";
            txtGrade.Text = "";
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearInputs();
        }
    }
}
