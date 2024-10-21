using ConsoleGodmist.Characters;
using ConsoleGodmist.Enums;
using Newtonsoft.Json;

namespace ConsoleGodmist.Combat.Skills;

[JsonConverter(typeof(ActiveSkillEffectConverter))]
public interface IActiveSkillEffect
{
    public SkillTarget Target { get; set; }

    public void Execute(Character caster, Character enemy, string source);
}