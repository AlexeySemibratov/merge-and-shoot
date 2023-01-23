
using System.Collections.Generic;

public interface IDamageSkillRegistry
{
    IEnumerable<IDamageSkill> GetDamageSkills();
}
