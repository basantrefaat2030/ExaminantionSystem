using ExaminationSystem.Models;
using ExaminationSystem.Models.Enums;
using ExaminationSystem.Repositories;

namespace ExaminationSystem.Services
{

    public class RoleFeatureService
    {
        GeneralRepository<RoleFeature> _roleFeatureRepository;
        public RoleFeatureService(GeneralRepository<RoleFeature> roleFeatureRepository) 
        {
            _roleFeatureRepository = roleFeatureRepository;
        } 

        public void AssignFeatureToRole(Role role, Feature feature)
        {
            // Validate Feature not assigned to Role    

            var roleFeature = new RoleFeature
            {
                Role = role,
                Feature = feature
            };
            _roleFeatureRepository.AddAsync(roleFeature);
        }

        internal bool CheckFeatureAccess(Role role, Feature feature)
        {
            return _roleFeatureRepository
                    .Get(rf => rf.Role == role && rf.Feature == feature)
                    .Any();
        }
    }
}
