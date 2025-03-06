namespace SurveyBasket.Api.Mapping;

public class MappingConfigurations : IRegister
{
    public void Register(TypeAdapterConfig config)
    { 
        config
            .NewConfig<Category, CategoryResponse>()
            .Map(dest => dest.Topics, src => src.Topics.Adapt<IEnumerable<TopicResponse>>());
        config
            .NewConfig<Topic,TopicResponse>()
            .Map(dest => dest.CategoryId, src => src.Category.CategoryId);
    }
}
