using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ConsoleGodmist.Combat.Skills;
using Spectre.Console;

namespace SkillEditor
{
    public partial class SkillWindow : Window
    {
        public ActiveSkill Skill { get; private set; }

        public SkillWindow()
        {
            InitializeComponent();
            Skill = new ActiveSkill
            {
                Effects = []
            };
            DataContext = Skill;
        }

        public SkillWindow(ActiveSkill skill)
        {
            Skill = skill;
            DataContext = Skill;
            InitializeComponent();
        }

        private void AddEffect_Click(object sender, RoutedEventArgs e)
        {
            var effectWindow = new EffectWindow();
            if (effectWindow.ShowDialog() == true)
            {
                Skill.Effects.Add(effectWindow.Effect);
                EffectsListView.Items.Refresh();
            }
        }
        private void RemoveEffect_Click(object sender, RoutedEventArgs e)
        {
            if (EffectsListView.SelectedItem is IActiveSkillEffect selectedEffect)
            {
                var result = MessageBox.Show("Are you sure you want to remove this effect?",
                    "Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes) return;
                Skill.Effects.Remove(selectedEffect);
                EffectsListView.Items.Refresh();
            }
            else
            {
                MessageBox.Show("Please select an effect to remove.");
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Skill.Alias = AliasTextBox.Text;
            Skill.ResourceCost = int.TryParse(ResourceCostTextBox.Text, out var resourceCost) ? resourceCost : 0;
            Skill.ActionCost = double.TryParse(ActionCostTextBox.Text, out var actionCost) ? actionCost : 0;
            Skill.Accuracy = int.TryParse(AccuracyTextBox.Text, out var accuracy) ? accuracy : 0;
            Skill.Hits = int.TryParse(HitsTextBox.Text, out var hits) ? hits : 0;
            Skill.AlwaysHits = AlwaysHitsCheckBox.IsChecked ?? false;
            DialogResult = true;
            Close();
        }
        private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]+"); // Only allow digits
            e.Handled = regex.IsMatch(e.Text);
        }

        // Restrict input to decimals
        private void DecimalTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9.,]+"); // Allow digits and a single decimal point
            e.Handled = regex.IsMatch(e.Text);

            // Ensure only one decimal point is allowed
            var textBox = sender as TextBox;
            if ((e.Text == "." && textBox.Text.Contains('.')) || (e.Text == "," && textBox.Text.Contains(',')))
            {
                e.Handled = true;
            }
        }

        // Prevent spaces and other unwanted keys
        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true; // Block spaces
            }
        }
    }
}