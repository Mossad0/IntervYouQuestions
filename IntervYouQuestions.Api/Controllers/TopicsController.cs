
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IntervYouQuestions.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class TopicsController(ITopicService topicService) : ControllerBase
{
    private readonly ITopicService _topicService = topicService;
    [HttpGet("")]
    public async Task<IActionResult> GetAll()
    {
        var topics = await _topicService.GetAllAsync();
        var response = topics.Adapt<IEnumerable<TopicResponse>>();
        return Ok(response);
    }
    [HttpGet("{Id}")]
    public async Task<IActionResult> Get([FromRoute] int Id)
    {
        var topic = await _topicService.GetAsync(Id);
        if (topic == null) return NotFound();
        var response = topic.Adapt<TopicResponse>();
        return Ok(response);
    }

    [HttpPost("")]
    public async Task<IActionResult> Add([FromBody] TopicRequest request)
    {

        var newTopic = await _topicService.AddAsync(request.Adapt<Topic>());
        return CreatedAtAction(nameof(Get), new { Id = newTopic.TopicId }, newTopic);
    }

    [HttpPut("{Id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] TopicRequest request)
    {
        var updatedTopic = await _topicService.UpdateAsync(id, request.Adapt<Topic>());
        if (!updatedTopic) return NotFound();
        return NoContent();
    }
    [HttpDelete("{Id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var deleted = await _topicService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
