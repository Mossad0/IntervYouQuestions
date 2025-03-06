namespace IntervYouQuestions.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ModelAnswersController(IModelAnswerService modelAnswerService) : ControllerBase
{
    private readonly IModelAnswerService _modelAnswerService = modelAnswerService;

    [HttpGet("")]
    public async Task<IActionResult> GetAll()
    {
        var answers = await _modelAnswerService.GetAllAsync();
        var response = answers.Adapt<IEnumerable<ModelAnswerResponse>>();
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] int id)
    {
        var answer = await _modelAnswerService.GetAsync(id);
        if (answer == null) return NotFound();
        var response = answer.Adapt<ModelAnswerResponse>();
        return Ok(response);
    }

    //[HttpGet("question/{questionId}")]
    //public async Task<IActionResult> GetByQuestionId([FromRoute] int questionId)
    //{
    //    var answers = await _modelAnswerService.GetByQuestionIdAsync(questionId);
    //    var response = answers.Adapt<IEnumerable<ModelAnswerResponse>>();
    //    return Ok(response);
    //}

    [HttpPost("")]
    public async Task<IActionResult> Add([FromBody] ModelAnswerRequest request)
    {
        var newAnswer = await _modelAnswerService.AddAsync(request.Adapt<ModelAnswer>());
        return CreatedAtAction(nameof(Get), new { id = newAnswer.ModelAnswerId }, newAnswer);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] ModelAnswerRequest request)
    {
        var updated = await _modelAnswerService.UpdateAsync(id, request.Adapt<ModelAnswer>());
        if (!updated) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var deleted = await _modelAnswerService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
