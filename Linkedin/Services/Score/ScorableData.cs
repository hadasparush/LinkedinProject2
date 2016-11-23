using System;
using System.Collections.Generic;

namespace Linkedin.Models
{
    public class ScorableData
    {
        public readonly ICollection<ProfileExperience> experince;
        public readonly ICollection<ProfileEducation> education;
        public readonly ICollection<ProfileSkills> skills;

        public ScorableData(ICollection<ProfileExperience> experince, ICollection<ProfileEducation> education, ICollection<ProfileSkills> skills)
        {
            this.experince = experince;
            this.education = education;
            this.skills = skills;
        }
        
    }
}