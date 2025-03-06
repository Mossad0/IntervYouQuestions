namespace IntervYouQuestions.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class QuestionOptionsController : ControllerBase
{
    private readonly IQuestionOptionService _questionOptionService;

    public QuestionOptionsController(IQuestionOptionService questionOptionService)
    {
        _questionOptionService = questionOptionService;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAll()
    {
        var options = await _questionOptionService.GetAllAsync();
        var response = options.Adapt<IEnumerable<QuestionOptionResponse>>();
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] int id)
    {
        var option = await _questionOptionService.GetAsync(id);
        if (option == null) return NotFound();
        var response = option.Adapt<QuestionOptionResponse>();
        return Ok(response);
    }

    [HttpPost("")]
    public async Task<IActionResult> Add([FromBody] QuestionOptionRequest request)
    {
        var newOption = await _questionOptionService.AddAsync(request.Adapt<QuestionOption>());
        return CreatedAtAction(nameof(Get), new { id = newOption.OptionId }, newOption);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] QuestionOptionRequest request)
    {
        var updated = await _questionOptionService.UpdateAsync(id, request.Adapt<QuestionOption>());
        if (!updated) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var deleted = await _questionOptionService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
