using HtmlAgilityPack;
using Linkedin.Models;
using System;
using System.Linq;

namespace Linkedin.Controllers
{
    public class LinkedinProfileHtmlParser
    {
        ProfileDetails linkedinProfile ;

        public ProfileDetails parse(string linkedinProfileHtml)
        {
            try
            {
                linkedinProfile = new ProfileDetails();
                HtmlDocument profileHTML = new HtmlDocument();
                profileHTML.LoadHtml(linkedinProfileHtml);
                HtmlNode[] nodes = profileHTML.DocumentNode.SelectNodes("//div").ToArray();

                foreach (HtmlNode item in nodes)
                {
                    // Name
                    if (item.Id == "name")
                    {
                        linkedinProfile.name = item.InnerText.ToString();
                    }
                    // Title
                    if (item.Id == "headline")
                    {
                        linkedinProfile.currentTitle = item.InnerText.ToString();
                    }
                    // Summery
                    if (item.Id == "summary-item-view")
                    {
                        linkedinProfile.Summary = item.FirstChild.FirstChild.InnerText.ToString();
                    }
                    // Experience
                    if (item.Id == "background-experience")
                    {
                        experienceParse(item);
                    }
                    // Education
                    if (item.Id == "background-education")
                        educationParse(item);

                }

                // Skills
                nodes = profileHTML.DocumentNode.SelectNodes("//li").ToArray();
                if (nodes != null)
                {
                    skillsParse(nodes);
                }

                // Current posstion
                nodes = profileHTML.DocumentNode.SelectNodes("//tr").ToArray();
                if (nodes != null)
                {
                    currentPosstionParse(nodes);
                }

                return linkedinProfile;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void skillsParse(HtmlNode[] nodes)
        {
            foreach (var tag in nodes)
            {
                if (tag.Attributes["class"] != null && tag.Attributes["class"].Value == "endorse-item has-endorsements ")
                {
                    ProfileSkills newSkill = new ProfileSkills();
                    newSkill.skill = tag.Attributes["data-endorsed-item-name"].Value;
                    linkedinProfile.ProfileSkills.Add(newSkill);

                }
            }
        }

        private void currentPosstionParse(HtmlNode[] nodes)
        {
            string strCurrentPostion = string.Empty;
            foreach (HtmlNode item in nodes)
            {
                if (item.Id == "overview-summary-current")
                {
                    HtmlNode foundedNode = item.ChildNodes[1].FirstChild;
                    foreach (HtmlNode currNode in foundedNode.ChildNodes)
                    {
                        if (string.IsNullOrEmpty(strCurrentPostion))
                        {
                            strCurrentPostion = currNode.InnerText;
                        }
                        else
                        {
                            strCurrentPostion += ", " + currNode.FirstChild.InnerText;
                        }
                    }

                    break;
                }
            }

            linkedinProfile.CurrentPosition = strCurrentPostion;
        }

        private void experienceParse(HtmlNode item)
        {
            HtmlNode currExperience;
            string strExperience = "";
            string strYears;
            string strMainTitle = "";
            string strTitle = "";
            foreach (HtmlNode currNode in item.ChildNodes)
            {
                if (currNode.Name == "div")
                {
                    ProfileExperience currExper = new ProfileExperience();
                    currExperience = currNode.ChildNodes[0].ChildNodes[0];
                    foreach (HtmlNode experNode in currExperience.ChildNodes)
                    {
                        if (experNode.Name == "h4")
                        {
                            strMainTitle = experNode.FirstChild.InnerText;
                        }
                        if (experNode.Name == "h5")
                        {
                            strTitle = experNode.FirstChild.InnerText;
                        }

                    }
                    strExperience = strMainTitle + " , " + strTitle;

                    //TODO : cut the number and put in the string 
                    strYears = currNode.ChildNodes[0].ChildNodes[1].InnerText;
                    strYears = strYears.Substring(strYears.IndexOf("("), strYears.LastIndexOf(")") - strYears.IndexOf("(") + 1);

                    currExper.Experience = strExperience + ", " + strYears;

                    linkedinProfile.ProfileExperience.Add(currExper);
                }
            }
        }

        private  void educationParse(HtmlNode item)
        {
            foreach (HtmlNode currNode in item.ChildNodes)
            {
                if (currNode.Name == "div")
                {
                    string strMainTitle = "";
                    string strTitle = "";
                    ProfileEducation currEducation = new ProfileEducation();
                    // TODO: CUT THE BAD STRING 
                    foreach (HtmlNode HeaderNode in currNode.FirstChild.FirstChild.ChildNodes)
                    {
                        if (HeaderNode.Name == "header")
                        {
                            foreach (HtmlNode educationNode in HeaderNode.ChildNodes)
                            {
                                if (educationNode.Name == "h4")
                                {
                                    strMainTitle = educationNode.InnerText;
                                }
                                if (educationNode.Name == "h5")
                                {
                                    strTitle = ", " + educationNode.InnerText;
                                }
                            }

                            currEducation.Education = strMainTitle + strTitle;

                        }
                    }

                    linkedinProfile.ProfileEducation.Add(currEducation);
                }
            }
        }
    }
}