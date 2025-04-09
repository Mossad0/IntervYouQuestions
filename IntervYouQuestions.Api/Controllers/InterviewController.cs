namespace IntervYouQuestions.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InterviewController : ControllerBase
    {
        private readonly IInterviewService _interviewService;

        public InterviewController(
            IInterviewService interviewService)
        {
            _interviewService = interviewService;
        }


        [HttpPost("start")]
        public async Task<ActionResult<InterviewResponse>> StartInterview(StartInterviewRequest startInterviewRequest)
        {
            var interview = await _interviewService.StartInterviewAsync(startInterviewRequest);
            return Ok(interview);
        }

        [HttpPost("start/user/{userId}")]
        public async Task<ActionResult<InterviewResponse>> StartInterviewForUser(
            string userId,
            [FromQuery] int numberOfQuestions = 10,
            [FromQuery] int timeLimitInMinutes = 30)
        {
            var interview = await _interviewService.StartInterviewForUserAsync(userId, numberOfQuestions, timeLimitInMinutes);
            return Ok(interview);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInterview(int id)
        {
            await _interviewService.DeleteInterviewAsync(id);
            return NoContent();
        }


    }
}