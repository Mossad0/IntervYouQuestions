namespace IntervYouQuestions.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class QuestionsController : ControllerBase
{
    private readonly IQuestionService _questionService;

    public QuestionsController(IQuestionService questionService)
    {
        _questionService = questionService;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAll()
    {
        var questions = await _questionService.GetAllAsync();
        var response = questions.Adapt<IEnumerable<QuestionResponse>>();
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] int id)
    {
        var question = await _questionService.GetAsync(id);
        if (question == null) return NotFound();
        var response = question.Adapt<QuestionResponse>();
        return Ok(response);
    }

    [HttpPost("")]
    public async Task<IActionResult> Add([FromBody] QuestionRequest request)
    {
        var newQuestion = await _questionService.AddAsync(request.Adapt<Question>());
        return CreatedAtAction(nameof(Get), new { id = newQuestion.QuestionId }, newQuestion);
    }

    [HttpPost("add-with-options")]
    public async Task<IActionResult> AddWithOptions([FromBody] QuestionWithOptionsRequest request)
    {
        if (request == null)
        {
            return BadRequest("Invalid request.");
        }

        var newQuestion = await _questionService.AddWithOptionAsync(request);
        return CreatedAtAction(nameof(Get), new { id = newQuestion.QuestionId }, newQuestion);
    }
    [HttpPost("add-with-model-answers")]
    public async Task<IActionResult> AddWithModelAnswers([FromBody] QuestionWithModelAnswerRequest request)
    {
        if (request == null)
        {
            return BadRequest("Invalid request.");
        }

        var newQuestion = await _questionService.AddWithModelAnswerAsync(request);
        return CreatedAtAction(nameof(Get), new { id = newQuestion.QuestionId }, newQuestion);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] QuestionRequest request)
    {
        var updated = await _questionService.UpdateAsync(id, request.Adapt<Question>());
        if (!updated) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var deleted = await _questionService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
