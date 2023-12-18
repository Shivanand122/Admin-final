using AdminDAL.Entities2;

using System.Threading.Tasks;

namespace AdminDAL
{
    public interface IFeatureRepository
    {
        IEnumerable<Feature> GetAllFeatures(string user);
        IEnumerable<Feature> GetAllAsync();
        Task<Feature> GetByIdAsync(int id);
        Task CreateAsync(Feature entity);
        Task UpdateAsync(Feature entity);
        Task DeleteAsync(int id);
        Feature GetFeatureWithDetails(int featureId);
        void UpdateFeatureStatus(int featureId, int newStatus);
        void UpdateFeatureComment(int featureId, string comment);

    }
} 
