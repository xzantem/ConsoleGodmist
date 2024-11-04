using System.Reflection;
using ConsoleGodmist.Characters;
using ConsoleGodmist.Combat.Battles;
using ConsoleGodmist.Enums;
using ConsoleGodmist.TextService;
using ConsoleGodmist.Utilities;

namespace ConsoleGodmist.Combat.Skills;

public class ActiveSkill
{
    public string Name => NameAliasHelper.GetName(Alias);
    public string Alias { get; set; }
    public int ResourceCost { get; set; }
    public bool AlwaysHits { get; set; }
    public int Accuracy { get; set; }
    public int Hits { get; set; }
    public List<IActiveSkillEffect> Effects { get; set; }

    public ActiveSkill()
    {
        
    }

    public ActiveSkill(string alias, int resourceCost, bool alwaysHits, int accuracy,
        List<IActiveSkillEffect> effects, int hits = 1)
    {
        Alias = alias;
        ResourceCost = resourceCost;
        AlwaysHits = alwaysHits;
        Accuracy = accuracy;
        Effects = effects;
        Hits = hits;
    }

    public bool Use(Character caster, Character enemy)
    {
        if (!(caster.CurrentResource >= ResourceCost) && 
            (caster.ResourceType != ResourceType.Fury || 
             !(Math.Abs(caster.MaximalResource - caster.CurrentResource) < 0.001)) && ResourceCost != -1) return false;
        ActiveSkillTextService.DisplayUseSkillText(caster, enemy, this);
        caster.UseResource(ResourceCost);
        foreach (var effect in Effects.Where(x => x.Target == SkillTarget.Self)) 
            effect.Execute(caster, enemy, Alias);
        for (var i = 0; i < Hits; i++)
        {
            {
                if (!CheckHit(caster, enemy) && !AlwaysHits)
                {
                    ActiveSkillTextService.DisplayMissText(caster);
                    continue;
                }
                foreach (var effect in Effects.Where(x => x.Target == SkillTarget.Enemy))
                    effect.Execute(caster, enemy, Alias);
            }
        }
        return true;
    }

    private bool CheckHit(Character caster, Character target)
    {
        var accuracy = (caster.Accuracy + Accuracy) / 2;
        var hitChance = accuracy * accuracy / (accuracy + target.Dodge);
        return Random.Shared.NextDouble() * 100 < hitChance;
    }
}