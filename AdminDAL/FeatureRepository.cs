using AdminDAL.Entities2;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace AdminDAL.Repositories
{
    public class FeatureRepository : IFeatureRepository
    {
        private readonly AdminCont _context;

        public FeatureRepository(AdminCont context)
        {
            _context = context;
        }

        public IEnumerable<Feature> GetAllAsync()
        {

            return _context.Features.Include(f => f.EntityName).ToList();
        }

        public async Task<Feature> GetByIdAsync(int id)
        {
            return await _context.Features.FirstOrDefaultAsync(f => f.FeatureId == id);
        }

        public async Task CreateAsync(Feature entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await _context.Features.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Feature entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            _context.Features.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entityToDelete = await _context.Features.FindAsync(id);
            if (entityToDelete != null)
            {
                _context.Features.Remove(entityToDelete);
                await _context.SaveChangesAsync();
            }
        }

        public Feature GetFeatureWithDetails(int featureId)
        {
            return _context.Features
        .Include(f => f.EntityNameNavigation) // Include related entity if needed
        .FirstOrDefault(f => f.FeatureId == featureId);
        }

        public void UpdateFeatureStatus(int featureId, int newStatus)
        {
            var featureToUpdate = _context.Features.FirstOrDefault(f => f.FeatureId == featureId);
            if (featureToUpdate != null)
            {
                featureToUpdate.ApprovalStatus = (byte)newStatus;
                _context.SaveChanges();
            }
        }

        public void UpdateFeatureComment(int featureId, string comment)
        {
            var featureToUpdate = _context.Features.FirstOrDefault(f => f.FeatureId == featureId);
            if (featureToUpdate != null)
            {
                // Append the new comment to the existing comments
                featureToUpdate.AdminComments += $" {comment}";
                _context.SaveChanges();
            }
        }

        public IEnumerable<Feature> GetAllFeatures(string adminUserName)
        {
            // Fetch the associated Usernames from UserAdminRole for the provided adminUserName
            var associatedUsernames = _context.UserAdminRoles
                .Where(uar => uar.AdminUserName == adminUserName)
                .Select(uar => uar.UserName)
                .ToList();
            Console.WriteLine(associatedUsernames);

            // Retrieve Features based on associatedUsernames
            return _context.Features
                .Include(f => f.EntityNameNavigation)
                .Where(f => associatedUsernames.Contains(f.UserName))
                .ToList();
        }
    }
}
