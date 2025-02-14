using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Windows;
using ConsoleGodmist.Combat.Skills;
using Newtonsoft.Json;

namespace SkillEditor
{
    public partial class MainWindow
    {
        private ObservableCollection<ActiveSkill> _skills = [];

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            _skills = [];
            LoadSkills();
            SkillsDataGrid.ItemsSource = _skills;
        }

        private void LoadSkills()
        {
            if (File.Exists("skills.json"))
            {
                string json = File.ReadAllText("skills.json");
                var loadedSkills = JsonConvert.DeserializeObject<ObservableCollection<ActiveSkill>>(json);
        
                if (loadedSkills != null)
                {
                    _skills.Clear();
                    foreach (var skill in loadedSkills)
                    {
                        _skills.Add(skill); // This preserves binding
                    }
                }
            }
        }


        private void SaveSkills()
        {
            var settings = new JsonSerializerSettings
            {
                Culture = CultureInfo.CurrentCulture,
                Formatting = Formatting.Indented
            };
            string json = JsonConvert.SerializeObject(_skills, settings);
            File.WriteAllText("skills.json", json);
        }

        private void AddSkill_Click(object sender, RoutedEventArgs e)
        {
            var skillWindow = new SkillWindow();
            if (skillWindow.ShowDialog() == true)
            {
                _skills.Add(skillWindow.Skill);
            }
        }
        private void RemoveSkill_Click(object sender, RoutedEventArgs e)
        {
            if (SkillsDataGrid.SelectedItem is ActiveSkill selectedSkill)
            {
                var result = MessageBox.Show("Are you sure you want to remove this skill?",
                    "Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes) return;
                _skills.Remove(selectedSkill);
                SkillsDataGrid.Items.Refresh();
            }
            else
            {
                MessageBox.Show("Please select a skill to edit.");
            }
        }

        private void EditSkill_Click(object sender, RoutedEventArgs e)
        {
            if (SkillsDataGrid.SelectedItem is ActiveSkill selectedSkill)
            {
                var skillWindow = new SkillWindow(selectedSkill);
                if (skillWindow.ShowDialog() == true)
                {
                    
                }
            }
            else
            {
                MessageBox.Show("Please select a skill to edit.");
            }
        }

        private void SaveAll_Click(object sender, RoutedEventArgs e)
        {
            SaveSkills();
            MessageBox.Show("All skills saved to skills.json");
        }
    }
}