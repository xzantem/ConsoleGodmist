using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Data;
using ConsoleGodmist.Combat.Skills;

public class EffectsListConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is List<IActiveSkillEffect> effects)
        {
            return string.Join(", ", effects.Select(x => x.GetType().Name)); 
        }
        if (value is IActiveSkillEffect effect)
        {
            Type myType = effect.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());
            var values = new List<string>();
            foreach (PropertyInfo prop in props)
            {
                object propValue = prop.GetValue(effect, null);

                values.Add($"{prop.Name}: {propValue}");
            }

            return string.Join(", ", values);
        } 
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}   