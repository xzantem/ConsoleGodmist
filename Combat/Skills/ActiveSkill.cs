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
    public double ActionCost { get; set; }
    public bool AlwaysHits { get; set; }
    public int Accuracy { get; set; }
    public int Hits { get; set; }
    public List<IActiveSkillEffect> Effects { get; set; }

    public ActiveSkill()
    {
        
    }

    public ActiveSkill(string alias, int resourceCost, double actionCost, bool alwaysHits, int accuracy,
        List<IActiveSkillEffect> effects, int hits = 1)
    {
        Alias = alias;
        ResourceCost = resourceCost;
        ActionCost = actionCost;
        AlwaysHits = alwaysHits;
        Accuracy = accuracy;
        Effects = effects;
        Hits = hits;
    }

    public bool Use(BattleUser caster, Character enemy)
    {
        if ((!(caster.User.CurrentResource >= ResourceCost) && 
            (caster.User.ResourceType != ResourceType.Fury || 
             !(Math.Abs(caster.User.MaximalResource - caster.User.CurrentResource) < 0.001)) && ResourceCost != -1) || 
            caster.CurrentActionPoints < caster.MaxActionPoints.Value() * ActionCost) return false;
        ActiveSkillTextService.DisplayUseSkillText(caster.User, enemy, this);
        caster.User.UseResource(ResourceCost);
        caster.UseActionPoints(caster.MaxActionPoints.Value() * ActionCost);
        foreach (var effect in Effects.Where(x => x.Target == SkillTarget.Self)) 
            effect.Execute(caster.User, enemy, Alias);
        if (Effects.All(x => x.Target != SkillTarget.Enemy)) return true;
        {
            for (var i = 0; i < Hits; i++)
            {
                {
                    if (!CheckHit(caster.User, enemy) && !AlwaysHits)
                    {
                        ActiveSkillTextService.DisplayMissText(caster.User);
                        continue;
                    }
                    foreach (var effect in Effects.Where(x => x.Target == SkillTarget.Enemy))
                        effect.Execute(caster.User, enemy, Alias);
                }
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