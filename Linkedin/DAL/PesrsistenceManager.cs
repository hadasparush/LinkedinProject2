using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Linkedin.Models
{
    public class PesrsistenceManager
    {

        private readonly UserProfileEntities db ;

        public PesrsistenceManager() { db = new UserProfileEntities(); }
        public PesrsistenceManager(UserProfileEntities db) { this.db = db; }

        public async Task addProfile(ProfileDetails linkedinProfile)
        {
            db.ProfileDetails.Add(linkedinProfile);
            
            try
            {
               await db.SaveChangesAsync();
            }
           
            catch (Exception e)
            {
                Console.WriteLine(String.Format("Failed saving to DB {0}. Error:{1}", linkedinProfile, e.ToString()));
                throw e;
            }
        }

        public async Task <List<ProfileDetails>> searchByFields(SearchableFields searchableFields)
        {
            try
            {
                List<ProfileDetails> matchProfiles =  (from profile in db.ProfileDetails
                                                      where (searchableFields.name == null || profile.name == searchableFields.name) &&
                                                     (searchableFields.currentTitle == null || profile.currentTitle == searchableFields.currentTitle) &&
                                                     (searchableFields.currentPosition == null || profile.CurrentPosition == searchableFields.currentPosition) &&
                                                     (searchableFields.summary == null || profile.Summary == searchableFields.summary)
                                                      select profile).ToList<ProfileDetails>();

                return matchProfiles;
            }
            catch (Exception e)
            {
                Console.WriteLine(String.Format("Failed searchByFields {0}. Error:{1}", searchableFields, e.ToString()));
                throw e;
            }
        }

        public async Task<List<ProfileSkills>> searchBySkills(List<string> searchBySkills)
        {
            try
            {
                List<ProfileSkills> matchProfiles = (from profileSkills in db.ProfileSkills
                                                     where (profileSkills.skill != null && searchBySkills.Contains(profileSkills.skill))
                                                     select profileSkills).ToList<ProfileSkills>();

                return matchProfiles;
            }
            catch (Exception e)
            {
                Console.WriteLine(String.Format("Failed searchByFields {0}. Error:{1}", searchBySkills, e.ToString()));
                throw e;
            }
        }

    }
}