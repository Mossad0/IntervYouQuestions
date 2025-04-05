using IntervYouQuestions.Api.Contracts.Requests;
using IntervYouQuestions.Api.Contracts.Responses;
using IntervYouQuestions.Api.Entities;
using IntervYouQuestions.Api.Exceptions;
using IntervYouQuestions.Api.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntervYouQuestions.Api.Services
{
    public interface IUserAnswerService
    {
        Task SaveUserAnswerAsync(SubmitAnswerRequest request);
        Task<InterviewResultResponse> CalculateInterviewScoreAsync(int interviewId);
    }
}