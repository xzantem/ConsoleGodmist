using System;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ConsoleGodmist.Combat.Skills;
using ConsoleGodmist.Combat.Skills.ActiveSkillEffects;
using ConsoleGodmist.Enums;

namespace SkillEditor
{
    public partial class EffectWindow : Window
    {
        public IActiveSkillEffect Effect { get; private set; }
        private Type _selectedEffectType;

        public EffectWindow()
        {
            InitializeComponent();
            EffectTypeComboBox.SelectedIndex = 0; // Default to the first effect type
        }
        public EffectWindow(IActiveSkillEffect effect)
        {
            Effect = effect;
            _selectedEffectType = effect.GetType();
            InitializeComponent();
        }

        private void EffectTypeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Clear the current content
            EffectPropertiesStackPanel.Children.Clear();

            // Determine the selected effect type
            _selectedEffectType = EffectTypeComboBox.SelectedIndex switch
            {
                0 => typeof(AdvanceMove),
                1 => typeof(BuffStat),
                2 => typeof(ClearStatusEffect),
                3 => typeof(DealDamage),
                4 => typeof(DebuffResistance),
                5 => typeof(DebuffStat),
                6 => typeof(ExtendDoT),
                7 => typeof(GainShield),
                8 => typeof(HealTarget),
                9 => typeof(InflictDoTStatusEffect),
                10 => typeof(InflictGenericStatusEffect),
                11 => typeof(InflictTimedPassiveEffect),
                12 => typeof(RegenResource),
                13 => typeof(TradeHealthForResource),
                _ => throw new ArgumentException("Invalid effect type")
            };

            // Generate controls for the selected effect type
            GenerateControlsForEffectType(_selectedEffectType);
        }

        private void GenerateControlsForEffectType(Type effectType)
        {
            // Clear the current content
            EffectPropertiesStackPanel.Children.Clear();

            // Get all public properties of the effect class
            var properties = effectType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                // Create a label for the property name
                var label = new Label { Content = property.Name };

                // Create a control based on the property type
                Control inputControl = null;

                if (property.PropertyType == typeof(string))
                {
                    inputControl = new TextBox();
                }
                else if (property.PropertyType == typeof(int))
                {
                    var textBox = new TextBox();
                    textBox.PreviewTextInput += NumericTextBox_PreviewTextInput;
                    textBox.PreviewKeyDown += TextBox_PreviewKeyDown;
                    inputControl = textBox;
                }
                else if (property.PropertyType == typeof(double))
                {
                    var textBox = new TextBox();
                    textBox.PreviewTextInput += DecimalTextBox_PreviewTextInput;
                    textBox.PreviewKeyDown += TextBox_PreviewKeyDown;
                    inputControl = textBox;
                }
                else if (property.PropertyType == typeof(bool))
                {
                    inputControl = new CheckBox();
                }
                else if (property.PropertyType.IsEnum)
                {
                    var comboBox = new ComboBox();
                    comboBox.ItemsSource = Enum.GetValues(property.PropertyType);
                    comboBox.SelectedIndex = 0;
                    inputControl = comboBox;
                }

                // Add the label and input control to the stack panel
                if (inputControl != null)
                {
                    EffectPropertiesStackPanel.Children.Add(label);
                    EffectPropertiesStackPanel.Children.Add(inputControl);
                }
            }
        }
        
        // Restrict input to integers
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

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of the selected effect type
            Effect = (IActiveSkillEffect)Activator.CreateInstance(_selectedEffectType);

            // Set the property values using reflection
            for (int i = 0; i < EffectPropertiesStackPanel.Children.Count; i += 2)
            {
                var label = (Label)EffectPropertiesStackPanel.Children[i];
                var inputControl = (Control)EffectPropertiesStackPanel.Children[i + 1];
                var property = _selectedEffectType.GetProperty(label.Content.ToString());
                
                if (inputControl is TextBox textBox)
                {
                    if (string.IsNullOrEmpty(textBox.Text))
                    {
                        MessageBox.Show($"Property '{label.Content.ToString()}' is required.");
                        return;
                    }
                    if (property.PropertyType == typeof(int))
                    {
                        property.SetValue(Effect, int.Parse(textBox.Text));
                    }
                    else if (property.PropertyType == typeof(double))
                    {
                        property.SetValue(Effect, double.Parse(textBox.Text.Replace('.', ',')));
                    }
                    else if (property.PropertyType == typeof(string))
                    {
                        property.SetValue(Effect, textBox.Text);
                    }
                }
                else if (inputControl is CheckBox checkBox)
                {
                    property.SetValue(Effect, checkBox.IsChecked ?? false);
                }
                else if (inputControl is ComboBox comboBox)
                {
                    property.SetValue(Effect, comboBox.SelectedValue);
                }
            }

            DialogResult = true;
            Close();
        }
    }
}