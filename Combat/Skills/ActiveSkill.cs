using System.Reflection;
using ConsoleGodmist.Characters;
using ConsoleGodmist.Combat.Battles;
using ConsoleGodmist.Enums;
using ConsoleGodmist.TextService;

namespace ConsoleGodmist.Combat.Skills;

public class ActiveSkill
{
    public string Name => locale.ResourceManager.GetString(Alias) == null ? Alias : 
        locale.ResourceManager.GetString(Alias);
    public string Alias { get; set; }
    public int ResourceCost { get; set; }
    public bool AlwaysHits { get; set; }
    public int Accuracy { get; set; }
    public List<IActiveSkillEffect> Effects { get; set; }

    public ActiveSkill()
    {
        
    }

    public ActiveSkill(string alias, int resourceCost, bool alwaysHits, int accuracy,
        List<IActiveSkillEffect> effects)
    {
        Alias = alias;
        ResourceCost = resourceCost;
        AlwaysHits = alwaysHits;
        Accuracy = accuracy;
        Effects = effects;
    }

    public void Use(Character caster, Character enemy)
    {
        if (!(caster.CurrentResource >= ResourceCost) && caster.ResourceType != ResourceType.Fury) return;
        ActiveSkillTextService.DisplayUseSkillText(caster, enemy, this);
        caster.UseResource(ResourceCost);
        foreach (var effect in Effects.Where(x => x.Target == SkillTarget.Self))
            effect.Execute(caster, enemy, Alias);
        if (!CheckHit(caster, enemy) && !AlwaysHits) return;
        foreach (var effect in Effects.Where(x => x.Target == SkillTarget.Enemy))
            effect.Execute(caster, enemy, Alias);
    }

    private bool CheckHit(Character caster, Character target)
    {
        var accuracy = (caster.Accuracy + Accuracy) / 2;
        var hitChance = accuracy * accuracy / (accuracy + target.Dodge);
        return Random.Shared.NextDouble() < hitChance;
    }
}