using Linkedin.Models;
using Linkedin.Models.Score;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Linkedin.Controllers
{
    [System.Web.Http.RoutePrefix("api/LinkedinProfile")]
    public class LinkedinProfileController : ApiController
    {
        const string  linkedinWebSite = "https://www.linkedin.com";
        private readonly IScoreCalculator scoreCalc = new ScoreCalculator();
        private readonly HttpRequestService httpRequetsMaker = new HttpRequestService();
        private readonly LinkedinProfileHtmlParser linkedInProfileHtmlParser = new LinkedinProfileHtmlParser();
        private readonly PesrsistenceManager pesrsistenceManager;

        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;
        public LinkedinProfileController()
        {
            pesrsistenceManager = new PesrsistenceManager();
        }

        public LinkedinProfileController(UserProfileEntities db)
        {
            pesrsistenceManager = new PesrsistenceManager(db);
        }

        public LinkedinProfileController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public class LinkedinURL
        {
            public string LinkedInUri { get; set; }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        // POST api/LinkedinProfile/AddProfile
        /// <summary>
        /// Add new linkedin profile
        /// </summary>
        /// <param name="profile">URL address of the linkedin profile</param>
        [System.Web.Http.Route("AddProfile")]
        public async Task<IHttpActionResult> AddProfile(LinkedinURL profile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                string linkedInProfileUrl = profile.LinkedInUri;
                string linkedinProfileHtml;

                // If the user didnt put the url
                if (string.IsNullOrEmpty(linkedInProfileUrl))
                {
                    return BadRequest("is empty");
                }

                if (!UrlIsValid(linkedInProfileUrl))
                {
                    return BadRequest("url is not valid");
                }
                // Check if the url is of linkedin website
                int startIndex = linkedInProfileUrl.IndexOf(".com");
                string checkProfileUrl = linkedInProfileUrl.Substring(0, startIndex + 4);

                if (linkedinWebSite != checkProfileUrl)
                {
                    return BadRequest("url is not the requested url");
                }

                //  999 Request denied
                //try
                //{
                //     linkedinProfileHtml = httpRequetsMaker.getResponse(linkedInProfileUrl);
                //}

                //catch (Exception ex)
                //{
                //    return InternalServerError(ex);
                //}
                // ProfileDetails linkedinProfile = linkedInProfileHtmlParser.parse(linkedinProfileHtml);

                // read the html from file
                string strHtml = File.ReadAllText(@"C:\Users\aya\Documents\Visual Studio 2015\Projects\LinkedinProject\exempleProfile.txt");
                ProfileDetails linkedinProfile = linkedInProfileHtmlParser.parse(strHtml);

                ScorableData profileScoreData = new ScorableData(linkedinProfile.ProfileExperience, linkedinProfile.ProfileEducation, linkedinProfile.ProfileSkills);

                // calc the score
                linkedinProfile.Score = scoreCalc.calculateScore(profileScoreData);

                await pesrsistenceManager.addProfile(linkedinProfile);
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format(" Error:{0}", ex.ToString()));
                return InternalServerError(ex);
            }

            return Ok("Profile was added succesfuly");
        }

        public static bool UrlIsValid(string url)
        {
            Uri uriResult;
            return (Uri.TryCreate(url, UriKind.Absolute, out uriResult)
             && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps));
        }


        // GET api/LinkedinProfile/SearchProfileByFields
        /// <summary>
        /// Search linkedin profile by fields
        /// </summary>
        /// <param name="searchableFields">object of the fields for search </param>
        [System.Web.Http.Route("SearchProfileByFields")]
        public async Task<HttpResponseMessage> GetProfileByFields([FromUri]SearchableFields searchableFields)
        {
            HttpResponseMessage result;

            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState); 
            }

            try
            {
                if ((searchableFields.currentPosition == null) &&
                    (searchableFields.currentTitle == null) &&
                    (searchableFields.name == null) &&
                        (searchableFields.summary == null))
                {
                    result = new HttpResponseMessage(HttpStatusCode.OK);
                    result.Content = new StringContent("SearchProfileByFields must contain at least one value");
                    return result;
                }
                        
                List<ProfileDetails> matchProfiles = await pesrsistenceManager.searchByFields(searchableFields);

                if (matchProfiles != null && matchProfiles.Count > 0)
                {
                    var responseData = JsonConvert.SerializeObject(matchProfiles, new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });

                    result = new HttpResponseMessage(HttpStatusCode.OK);
                    result.Content = new StringContent(responseData);
                    return result;
                }
                else
                {
                    result = new HttpResponseMessage(HttpStatusCode.OK);
                    result.Content = new StringContent("Profile Not Found");
                    return result;
                }
            }
            catch (Exception ex)
            {
                // Log exception code goes here  
                result = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                result.Content = new StringContent("Error occured while executing GetProfileByFields" + ex.ToString());
                return result;
            }
        }


        // GET api/LinkedinProfile/SearchProfileBySkills
        /// <summary>
        /// Search linkedin profile by skills
        /// </summary>
        /// <param name="searchBySkills">list of strings </param>
        [System.Web.Http.Route("SearchProfileBySkills")]
        public async Task<HttpResponseMessage> GetProfileBySkills([FromUri]List<string> searchBySkills)
        {
            HttpResponseMessage result;

            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            try
            {
                if (searchBySkills == null || searchBySkills.Count < 1)
                {
                    result = new HttpResponseMessage(HttpStatusCode.BadRequest);
                    result.Content = new StringContent("searchBySkills must contain at least one value");
                    return result;
                }

                // Find the match profiles
                List<ProfileSkills> matchProfiles = await pesrsistenceManager.searchBySkills(searchBySkills);

                if (matchProfiles != null && matchProfiles.Count > 0)
                {
                    var responseData = JsonConvert.SerializeObject(matchProfiles, new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                    
                    result = new HttpResponseMessage(HttpStatusCode.OK);
                    result.Content = new StringContent(responseData);
                    return result;
                }
                else
                {
                    result = new HttpResponseMessage(HttpStatusCode.OK);
                    result.Content = new StringContent("Profile Not Found");
                    return result;
                }
            }
            catch (Exception ex)
            {
                result = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                result.Content = new StringContent("Error occured while executing GetProfileBySkills" + ex.ToString());
                return result;
            }
        }
    }
}
