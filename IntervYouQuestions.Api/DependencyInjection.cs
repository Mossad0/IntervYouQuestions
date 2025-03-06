using FluentValidation.AspNetCore;
using IntervYouQuestions.Api.Services;
using MapsterMapper;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SurveyBasket.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection services)
    {
        services.AddControllers();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
        });
        services
            .AddSwaggerServices()
            .AddMapsterServices()
            .AddFluentValidationServices();

        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ITopicService, TopicService>();
        services.AddScoped<IQuestionService, QuestionService>();
        services.AddScoped<IQuestionOptionService, QuestionOptionService>();
        services.AddScoped<IModelAnswerService, ModelAnswerService>();
        return services;
    }

    public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
    {
       
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

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



}
