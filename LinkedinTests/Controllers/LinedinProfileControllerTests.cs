using Linkedin.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using static Linkedin.Controllers.LinkedinProfileController;

namespace Linkedin.Controllers.Tests
{
    [TestClass()]
    public class LinkedinProfileControllerTests
    {

        private Mock<DbSet<ProfileDetails>> mockProfileDetailsSet;
        private Mock<DbSet<ProfileSkills>> mockProfileSkillsSet;
        private Mock<UserProfileEntities> mockUserProfileEntitiesContext;
        private LinkedinProfileController linkedinProfileController;

        [TestInitialize()]
        public void Initialize()
        {   
            createControllerWithMockedDb(out mockProfileDetailsSet, out mockProfileSkillsSet, out mockUserProfileEntitiesContext, out linkedinProfileController);
        }

        private static void createControllerWithMockedDb(out Mock<DbSet<ProfileDetails>> mockProfileDetailsSet, out Mock<DbSet<ProfileSkills>> mockProfileSkillsSet, out Mock<UserProfileEntities> mockUserProfileEntitiesContext, out LinkedinProfileController linkedinProfileController)
        {
            mockProfileDetailsSet = new Mock<DbSet<ProfileDetails>>();
            mockProfileSkillsSet = new Mock<DbSet<ProfileSkills>>();
            mockUserProfileEntitiesContext = new Mock<UserProfileEntities>();
            mockUserProfileEntitiesContext.Setup(m => m.ProfileDetails).Returns(mockProfileDetailsSet.Object);
            mockUserProfileEntitiesContext.Setup(m => m.ProfileSkills).Returns(mockProfileSkillsSet.Object);
            linkedinProfileController = new LinkedinProfileController(mockUserProfileEntitiesContext.Object);
        }


        #region AddProfile

        [TestMethod()]
        public async Task AddProfile_UrlIsNull_ReturnBadMessageRequest()
        {
     
            //Act
            IHttpActionResult res = await linkedinProfileController.AddProfile(new LinkedinURL());

            //Assert
            Assert.IsInstanceOfType(res, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod()]
        public async Task AddProfile_UrlIsEmpty_ReturnFailedResult()
        {
            //Arrange
            LinkedinURL urlLink = new LinkedinURL();
            urlLink.LinkedInUri = string.Empty;
         
            //Act
            IHttpActionResult res = await linkedinProfileController.AddProfile(urlLink);

            //Assert
            Assert.IsInstanceOfType(res, typeof(BadRequestErrorMessageResult));
        }



        [TestMethod()]
        public async Task AddProfile_UrlIsNotLinkedinURL_ReturnFailedResult()
        {
            //Arrange
            LinkedinURL urlLink = new LinkedinURL();
            urlLink.LinkedInUri = "https://www.google.com/in/williamhgates";

            //Act
            IHttpActionResult res = await linkedinProfileController.AddProfile(urlLink);

            //Assert
            Assert.IsInstanceOfType(res, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod()]
        public async Task AddProfile_UrlIsValid_NewProfileWasAddedToDb()
        {
            //Arrange
            LinkedinURL urlLink = new LinkedinURL();
            urlLink.LinkedInUri = "https://www.linkedin.com/in/williamhgates";

            //Act
            var res = await linkedinProfileController.AddProfile(urlLink);
            
            //Assert          
            Assert.IsInstanceOfType(res, typeof(OkNegotiatedContentResult<string>));
        }

        
        
        #endregion

        #region  GetProfileByFields


        [TestMethod()]
        public async Task GetProfileByFieldsTest_WhenMatchInDBByOneField_ReturnResultsString()
        {
            //Arange
            var data = new List<ProfileDetails>
            {
                new ProfileDetails { name = "BBB" }
            }.AsQueryable();


            mockProfileDetailsSet.As<IQueryable<ProfileDetails>>().Setup(m => m.Provider).Returns(data.Provider);
            mockProfileDetailsSet.As<IQueryable<ProfileDetails>>().Setup(m => m.Expression).Returns(data.Expression);
            mockProfileDetailsSet.As<IQueryable<ProfileDetails>>().Setup(m => m.ElementType).Returns(data.ElementType);

            //Act
            SearchableFields searchableFields = new SearchableFields();
            searchableFields.name = "BBB";
            HttpResponseMessage res = await linkedinProfileController.GetProfileByFields(searchableFields);

            //Assert
            if (res.StatusCode == HttpStatusCode.OK)
            {
                Assert.IsInstanceOfType(res, typeof(HttpResponseMessage));
            }
            else
            {
                Assert.Fail();
            }
        }

        [TestMethod()]
        public async Task GetProfileByFieldsTest_WhenMatchInDBByMoreThenOneField_ReturnResultsString()
        {
            //Arange
            var data = new List<ProfileDetails>
            {
                new ProfileDetails { name = "BBB" , currentTitle = "ccc" }
            }.AsQueryable();


            mockProfileDetailsSet.As<IQueryable<ProfileDetails>>().Setup(m => m.Provider).Returns(data.Provider);
            mockProfileDetailsSet.As<IQueryable<ProfileDetails>>().Setup(m => m.Expression).Returns(data.Expression);
            mockProfileDetailsSet.As<IQueryable<ProfileDetails>>().Setup(m => m.ElementType).Returns(data.ElementType);

            //Act
            SearchableFields searchableFields = new SearchableFields();
            searchableFields.name = "BBB";
            searchableFields.currentTitle = "ccc";
            HttpResponseMessage res = await linkedinProfileController.GetProfileByFields(searchableFields);

            //Assert
            if (res.StatusCode == HttpStatusCode.OK)
            {
                Assert.IsInstanceOfType(res, typeof(HttpResponseMessage));
            }
            else
            {
                Assert.Fail();
            }
        }

        [TestMethod()]
        public async Task GetProfileByFieldsTest_NoFieldsForSearch_ReturnNoResultsString()
        {
            //Arange
            var data = new List<ProfileDetails>
            {
                new ProfileDetails { name = "BBB" , currentTitle = "ccc" }
            }.AsQueryable();


            mockProfileDetailsSet.As<IQueryable<ProfileDetails>>().Setup(m => m.Provider).Returns(data.Provider);
            mockProfileDetailsSet.As<IQueryable<ProfileDetails>>().Setup(m => m.Expression).Returns(data.Expression);
            mockProfileDetailsSet.As<IQueryable<ProfileDetails>>().Setup(m => m.ElementType).Returns(data.ElementType);

            //Act
            SearchableFields searchableFields = new SearchableFields();
            HttpResponseMessage res =await linkedinProfileController.GetProfileByFields(searchableFields);

            //Assert
            if (res.StatusCode == HttpStatusCode.OK)
            {
                var responseString = res.Content.ReadAsStringAsync().Result;
                if ("SearchProfileByFields must contain at least one value" == responseString.ToString())
                {
                    Assert.IsInstanceOfType(res, typeof(HttpResponseMessage));
                }
                else
                {
                    Assert.Fail();
                }
            }
            else
            {
                Assert.Fail();
            }
        }

        [TestMethod()]
        public async Task GetProfileByFieldsTest_WhenNoMatchInDBByOneField_ReturnNoResultsString()
        {
            //Arange
            var data = new List<ProfileDetails>
            {
                new ProfileDetails { name = "BBB" }
            }.AsQueryable();


            mockProfileDetailsSet.As<IQueryable<ProfileDetails>>().Setup(m => m.Provider).Returns(data.Provider);
            mockProfileDetailsSet.As<IQueryable<ProfileDetails>>().Setup(m => m.Expression).Returns(data.Expression);
            mockProfileDetailsSet.As<IQueryable<ProfileDetails>>().Setup(m => m.ElementType).Returns(data.ElementType);

            //Act
            SearchableFields searchableFields = new SearchableFields();
            searchableFields.name = "aaa";
            HttpResponseMessage res =await linkedinProfileController.GetProfileByFields(searchableFields);

            //Assert
            if (res.StatusCode == HttpStatusCode.OK)
            {
                var responseString = res.Content.ReadAsStringAsync().Result;
                if ("Profile Not Found" == responseString.ToString())
                {
                    Assert.IsInstanceOfType(res, typeof(HttpResponseMessage));
                }
                else
                {
                    Assert.Fail();
                }
            }
            else
            {
                Assert.Fail();
            }
        }

   
        #endregion

        #region  GetProfileBySkills


        [TestMethod()]
        public async Task GetProfileBySkillsTest_WhenNoMatchInDB_ReturnNoResultsString()
        {
            //Arange
            var data = new List<ProfileSkills>
            {
                new ProfileSkills { skill = "ccc" }
            }.AsQueryable();


            mockProfileSkillsSet.As<IQueryable<ProfileSkills>>().Setup(m => m.Provider).Returns(data.Provider);
            mockProfileSkillsSet.As<IQueryable<ProfileSkills>>().Setup(m => m.Expression).Returns(data.Expression);
            mockProfileSkillsSet.As<IQueryable<ProfileSkills>>().Setup(m => m.ElementType).Returns(data.ElementType);

            //Act

            List<string> lst = new List<string> { "aaa" };
            HttpResponseMessage res =await linkedinProfileController.GetProfileBySkills(lst);

            //Assert
            if (res.StatusCode == HttpStatusCode.OK)
            {
                Assert.IsInstanceOfType(res, typeof(HttpResponseMessage));
            }
            else
            {
                Assert.Fail();
            }
        }

        [TestMethod()]
        public async Task GetProfileBySkillsTest_NoSkillsForSearch_ReturnNoResultsString()
        {
            //Arange
            var data = new List<ProfileSkills>
            {
                new ProfileSkills { skill = "ccc" }
            }.AsQueryable();


            mockProfileSkillsSet.As<IQueryable<ProfileSkills>>().Setup(m => m.Provider).Returns(data.Provider);
            mockProfileSkillsSet.As<IQueryable<ProfileSkills>>().Setup(m => m.Expression).Returns(data.Expression);
            mockProfileSkillsSet.As<IQueryable<ProfileSkills>>().Setup(m => m.ElementType).Returns(data.ElementType);

            //Act

            List<string> lst = new List<string> { };
            HttpResponseMessage res = await linkedinProfileController.GetProfileBySkills(lst);

            //Assert
            if (res.StatusCode == HttpStatusCode.BadRequest)
            {
                var responseString = res.Content.ReadAsStringAsync().Result;
                if ("searchBySkills must contain at least one value" == responseString.ToString())
                {
                    Assert.IsInstanceOfType(res, typeof(HttpResponseMessage));
                }
                else
                {
                    Assert.Fail();
                }
            }
            else
            {
                Assert.Fail();
            }
        }

        [TestMethod()]
        public async Task GetProfileBySkillsTest_WhenHaveMatchInDBByOneSkill_ReturnResultsString()
        {
            //Arange
            var data = new List<ProfileSkills>
            {
                new ProfileSkills { skill = "ccc" }
            }.AsQueryable();


            mockProfileSkillsSet.As<IQueryable<ProfileSkills>>().Setup(m => m.Provider).Returns(data.Provider);
            mockProfileSkillsSet.As<IQueryable<ProfileSkills>>().Setup(m => m.Expression).Returns(data.Expression);
            mockProfileSkillsSet.As<IQueryable<ProfileSkills>>().Setup(m => m.ElementType).Returns(data.ElementType);

            //Act

            List<string> lst = new List<string> { "ccc" };
            HttpResponseMessage res = await linkedinProfileController.GetProfileBySkills(lst);

            //Assert
            if (res.StatusCode == HttpStatusCode.OK)
            {
                Assert.IsInstanceOfType(res, typeof(HttpResponseMessage));
            }
            else
            {
                Assert.Fail();
            }
        }

        [TestMethod()]
        public async Task GetProfileBySkillsTest_WhenHaveMatchInDBByMoreThenOneSkill_ReturnResultsString()
        {
            //Arange
            var data = new List<ProfileSkills>
            {
                new ProfileSkills { skill = "ccc" },
                new ProfileSkills { skill = "bbb" }
            }.AsQueryable();


            mockProfileSkillsSet.As<IQueryable<ProfileSkills>>().Setup(m => m.Provider).Returns(data.Provider);
            mockProfileSkillsSet.As<IQueryable<ProfileSkills>>().Setup(m => m.Expression).Returns(data.Expression);
            mockProfileSkillsSet.As<IQueryable<ProfileSkills>>().Setup(m => m.ElementType).Returns(data.ElementType);

            //Act

            List<string> lst = new List<string> { "aaa", "bbb" };
            HttpResponseMessage res = await linkedinProfileController.GetProfileBySkills(lst);

            //Assert
            if (res.StatusCode == HttpStatusCode.OK)
            {
                Assert.IsInstanceOfType(res, typeof(HttpResponseMessage));
            }
            else
            {
                Assert.Fail();
            }
        }

        #endregion
    }
}
