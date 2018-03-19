using System.Threading.Tasks;

namespace LinzLinienAlexaSkill.ModelUtil.Services
{
    public interface IAlexaModelCreationService
    {
        Task CreateAlexaInvocationModelAsync();
    }
}