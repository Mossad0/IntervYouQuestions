
using IntervYouQuestions.Api.Contracts.Requests;

namespace IntervYouQuestions.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    private readonly ICategoryService _categoryService = categoryService ;
    [HttpGet("")]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _categoryService.GetAllAsync();
        var response = categories.Adapt<IEnumerable<CategoryResponse>>();
        return Ok(response);
    }
    [HttpGet("{Id}")]
    public async Task<IActionResult> Get([FromRoute] int Id)
    {
        var category = await _categoryService.GetAsync(Id);
        if (category == null) return NotFound();
        var response = category.Adapt<CategoryResponse>();
        return Ok(response);
    }
    [HttpPost("")]
    public async Task<IActionResult> Add([FromBody] CategoryRequest request)
    {

        var newCategory = await _categoryService.AddAsync(request.Adapt<Category>());
        return CreatedAtAction(nameof(Get), new { Id = newCategory.CategoryId }, newCategory);
    }

    [HttpPut("{Id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody]CategoryRequest request)
    {
        var updatedCategory = await _categoryService.UpdateAsync(id,request.Adapt<Category>());
        if (!updatedCategory) return NotFound();
        return NoContent();
    }
    [HttpDelete("{Id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var deleted = await _categoryService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
