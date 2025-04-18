﻿using FluentValidation.AspNetCore;
using IntervYouQuestions.Api.Authentication;
using IntervYouQuestions.Api.Services;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace IntervYouQuestions.Api;

public static class DependencyInjection
{
    
    public static IServiceCollection AddDependencies(this IServiceCollection services)
    {
        services.AddControllers();

        services
            .AddSwaggerServices()
            .AddMapsterServices()
            .AddFluentValidationServices()
            .AddAuthServices()
            ;


        

        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ITopicService, TopicService>();
        services.AddScoped<IQuestionService, QuestionService>();
        services.AddScoped<IInterviewService, InterviewService>();
        services.AddScoped<IEmailService, MailKitEmailService>();

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserAnswerService, UserAnswerService>();
        return services;
    }

    public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "IntervYou Questions API",
                Version = "v1",
                Description = "API for managing interview questions and categories"
            });
        });

        return services;
    }

    public static IServiceCollection AddMapsterServices(this IServiceCollection services)
    {
        var mappingConfig = TypeAdapterConfig.GlobalSettings;
        mappingConfig.Scan(Assembly.GetExecutingAssembly());
        services.AddSingleton<IMapper>(new Mapper(mappingConfig));

        return services;
    }

    public static IServiceCollection AddFluentValidationServices(this IServiceCollection services)
    {
        services
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly())
            .AddFluentValidationAutoValidation();
        return services;
    }
    public static IServiceCollection AddAuthServices(this IServiceCollection services)
    {
        services.AddIdentity<AppUser, IdentityRole>()
               .AddEntityFrameworkStores<InterviewModuleContext>()
               .AddDefaultTokenProviders()
               ;

        services.Configure<IdentityOptions>(options =>
        {
            options.SignIn.RequireConfirmedEmail = true;
            options.Password.RequiredLength = 8;
        });



        return services;
    }
}
