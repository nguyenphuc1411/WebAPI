using PFood.Data;

namespace PFood.Services
{
    public interface IFeedbackRepository
    {
        Task<bool> Post(Feedback feedback);
        Task<List<Feedback>> Get();
    }
}
