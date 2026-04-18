// Controllers/SkillController.cs
using Common.Dto.Skills;
using Common.StaticData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SkillController : ControllerBase
{
    // GET /api/Skill
    [HttpGet]
    public ActionResult<IEnumerable<SkillDto>> GetAll()
    {
        var skills = Skill.AllSkills.Select(kvp => new SkillDto
        {
            SkillId = kvp.Key,
            Name = kvp.Value.Name,
            Description = kvp.Value.Description,
            RecommendedLevelId = kvp.Value.RecommendedLevelId
        });

        return Ok(skills);
    }

    // GET /api/Skill/{id}
    [HttpGet("{id}")]
    public ActionResult<SkillDto> GetById(int id)
    {
        if (id <= 0)
            return BadRequest("Invalid id");
        if (!Skill.AllSkills.TryGetValue(id, out var skill))
            return NotFound();

        return Ok(new SkillDto
        {
            SkillId = id,
            Name = skill.Name,
            Description = skill.Description,
            RecommendedLevelId = skill.RecommendedLevelId
        });
    }
}