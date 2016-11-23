using System;
using System.Collections.Generic;

namespace Linkedin.Models.Score
{
    public class ScoreCalculator : IScoreCalculator
    {
        public int calculateScore(ScorableData ScorableData)
        {
            int nExperinceScore = 0;
            int nSkillsScore = 0;
            int nEducationScore = 0;
            
            if (ScorableData.experince.Count > 0 )
            {
                int nCurrYears = 0;
                string strYears = "0";

                // To find years of experience
                foreach (ProfileExperience nYear in ScorableData.experince)
                {
                    int nPlaceMonths;
                    int nPlaceYear = nYear.Experience.IndexOf("years");
                    if (nPlaceYear == -1)
                    {
                        nPlaceMonths = nYear.Experience.IndexOf("months");
                        if (nPlaceMonths != -1)
                        {
                            nCurrYears = 1;
                         }
                    }
                    else
                    {
                        strYears = nYear.Experience.Substring(nYear.Experience.IndexOf("(") + 1, nPlaceYear - nYear.Experience.IndexOf("(") - 1).ToString().Trim();
                        Int32.TryParse(strYears, out nCurrYears);
                    }
                    
                    nExperinceScore += nCurrYears * 100;
                }
            }

            if (ScorableData.skills.Count > 0)
            {
                nSkillsScore = ScorableData.skills.Count;
            }

            // Calc education score
            if (ScorableData.education.Count > 0)
                nEducationScore = CalcEducationScore(ScorableData.education);

            return nEducationScore + nExperinceScore + nSkillsScore;
           
        }

        private static int CalcEducationScore(ICollection<ProfileEducation> educationData)
        {
            int nEducationScore;

            {
                int indexOfUniversty = 0;
                int indexOfCollage = 0;
                int indexOfschool = 0;

                bool UniversityStudy = false;
                bool CollageStudy = false;

                foreach (ProfileEducation currEducation in educationData)
                {
                    indexOfUniversty = currEducation.Education.ToString().IndexOf("University");
                    indexOfCollage = currEducation.Education.ToString().IndexOf("Collage");
                    indexOfschool = currEducation.Education.ToString().IndexOf("School");

                    if (indexOfUniversty != -1)
                    {
                        UniversityStudy = true;
                        break;
                    }

                    if (indexOfCollage != -1)
                    {
                        CollageStudy = true;
                    }
                }

                if (UniversityStudy)
                {
                    nEducationScore = 30;
                }
                else if (CollageStudy)
                {
                    nEducationScore = 20;
                }
                else
                {
                    nEducationScore = 10;
                }
            }

            return nEducationScore;
        }
    }
}