using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;
using pr16_2_7.Models;
using pr16_2_7.Services;

namespace pr16_2_7
{
    public partial class MainWindow : Window
    {
        private Dictionary<string, Student> students = new Dictionary<string, Student>();
        private Student currentStudent;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            string name = txtName.Text.Trim();
            string subject = txtSubject.Text.Trim();

            if (!int.TryParse(txtGrade.Text, out int grade) || grade < 1 || grade > 5)
            {
                MessageBox.Show("Оценка должна быть от 1 до 5!", "Ошибка ввода",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(subject))
            {
                MessageBox.Show("Введите имя студента и предмет!", "Ошибка ввода",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!students.ContainsKey(name))
                students[name] = new Student { Name = name };

            students[name].Subjects.Add(new SubjectGrade { Subject = subject, Grade = grade });

            UpdateStudentList();
            ClearInputs();

            // Уведомление
            MessageBox.Show($"Оценка {grade} по предмету '{subject}' добавлена студенту {name}!",
                          "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CmbStudents_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (cmbStudents.SelectedItem != null)
            {
                string selectedName = cmbStudents.SelectedItem.ToString();
                if (students.ContainsKey(selectedName))
                {
                    currentStudent = students[selectedName];
                    UpdateStudentInfo();
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (students.Count == 0)
            {
                MessageBox.Show("Нет данных для сохранения!", "Предупреждение",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new SaveFileDialog
            {
                Filter = "CSV файлы (*.csv)|*.csv|Все файлы (*.*)|*.*",
                DefaultExt = ".csv",
                Title = "Сохранить данные студентов"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    CsvService.Save(dialog.FileName, students);
                    MessageBox.Show($"Данные успешно сохранены в файл:\n{dialog.FileName}",
                                  "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении:\n{ex.Message}", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "CSV файлы (*.csv)|*.csv|Все файлы (*.*)|*.*",
                Title = "Загрузить данные студентов"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    students = CsvService.Load(dialog.FileName);
                    UpdateStudentList();
                    MessageBox.Show($"Загружено {students.Count} студентов(а) из файла:\n{dialog.FileName}",
                                  "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке:\n{ex.Message}", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearInputs();
        }

        private void UpdateStudentList()
        {
            var studentNames = students.Keys.ToList();
            cmbStudents.ItemsSource = studentNames;

            if (studentNames.Any())
            {
                cmbStudents.SelectedIndex = 0;
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

            SolidColorBrush colorBrush;
            switch (currentStudent.AverageColor)
            {
                case "Green":
                    colorBrush = new SolidColorBrush(Colors.LightGreen);
                    break;
                case "Gold":
                    colorBrush = new SolidColorBrush(Colors.Gold);
                    break;
                case "Red":
                    colorBrush = new SolidColorBrush(Colors.LightCoral);
                    break;
                default:
                    colorBrush = new SolidColorBrush(Colors.LightGray);
                    break;
            }
            borderAverage.Background = colorBrush;

            // Прогресс-бар
            double goodPercent = currentStudent.GoodPercent;
            progressBar.Value = goodPercent;
            tbPercent.Text = $"{goodPercent:F0}%";

            // Цвет прогресс-бара
            progressBar.Foreground = goodPercent >= 60
                ? new SolidColorBrush(Colors.Green)
                : new SolidColorBrush(Colors.Red);

            // Статус
            bool isAdmitted = currentStudent.IsAdmitted;
            tbStatus.Text = isAdmitted ? "ДОПУЩЕН К СЕССИИ" : "НЕ ДОПУЩЕН К СЕССИИ";

            // Меняем цвет фона статуса
            var statusBorderBrush = isAdmitted
                ? new SolidColorBrush(Colors.LightGreen)
                : new SolidColorBrush(Colors.LightCoral);
            statusBorder.Background = statusBorderBrush;
        }

        private void ClearDisplay()
        {
            tbAverage.Text = "0";
            borderAverage.Background = new SolidColorBrush(Colors.LightGray);
            progressBar.Value = 0;
            tbPercent.Text = "0%";
            tbStatus.Text = "НЕТ СТУДЕНТОВ";
            statusBorder.Background = new SolidColorBrush(Colors.LightGray);
        }

        private void ClearInputs()
        {
            txtName.Text = "";
            txtSubject.Text = "";
            txtGrade.Text = "";

            txtName.Focus();
        }
    }
}