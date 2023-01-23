using System.Collections.Generic;
using System.Linq;

public class SkillContainer : IDamageSkillRegistry
{
    private List<ISkill> _skillList = new();

    public IEnumerable<IDamageSkill> GetDamageSkills()
    {
        return _skillList.OfType<IDamageSkill>();
    }

    public void AddSkill(ISkill skill)
    {
        _skillList.Add(skill);
    }
}
