//using Admin.Data;
//using Admin.Models;
using AdminDAL.Entities2;
using AdminDAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Configuration;
using Newtonsoft.Json;
using Microsoft.FeatureManagement;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Hosting.Internal;

namespace Admin.Controllers
{
    // [Authorize]
    public class AdminMController : Controller
    {
        private readonly IFeatureManager _featureManager;
        private readonly IFeatureRepository _featureRepository;
        private readonly AdminCont _db;
        private List<PendingChangeModel> pendingChanges = new List<PendingChangeModel>();




        public AdminMController(IFeatureRepository featureRepository, AdminCont db, IConfiguration configuration, IFeatureManager featureManager)
        {
            _featureRepository = featureRepository;
            _db = db;
            _featureManager = featureManager;
        }

        [HttpGet]
        public IActionResult IsAdminSideEnabled()
        {
            // Replace this with your actual logic to check the feature flag status
            var isAdminSideEnabled = _featureManager.IsEnabledAsync("AdminSide").Result;

            return Json(isAdminSideEnabled);
        }


        //private readonly AdminContext _db;


        public class PendingChangeModel
        {
            public int FeatureID { get; set; }
            public string Action { get; set; }
            public string Comment { get; set; }
        }

        [AllowAnonymous]
        public IActionResult Log()
        {
            return View();
        }


        [OutputCache(Duration = 4000)]
        public async Task<IActionResult> Index(string username)
        {
            string name = username;
            if (string.IsNullOrEmpty(name))
            {
                // Redirect to the "Log" view when username is null or empty
                return RedirectToAction("Log");
            }


            IEnumerable<Feature> features = _featureRepository.GetAllFeatures(name);

            return View(features);


        }

        [AllowAnonymous]
        public IActionResult Authentication(string username)
        {
            string name = username;
            if (string.IsNullOrEmpty(name))
            {
                // Redirect to the "Log" view when username is null or empty
                return RedirectToAction("Log");
            }




            return View();
        }




        [HttpPost]
        public IActionResult UpdateStatus(int featureId, int newStatus)
        {

            // Retrieve the feature from the database
            Feature feature = _db.Features.FirstOrDefault(f => f.FeatureId == featureId);

            if (feature == null)
            {
                // Handle the case where the feature is not found
                return NotFound();
            }

            // Update the status
            feature.ApprovalStatus = (byte)newStatus;

            if ((byte)newStatus == 0)
            {
                TempData["success"] = "Approved successfully";
            }
            else
            {
                TempData["error"] = "Rejected successfully";
            }

            // Save changes to the database
            _db.SaveChanges();

            // Signal that the change has been successfully made
            return Ok(new { FeatureId = featureId, NewStatus = newStatus });
        }



        private static List<PendingChangeModel> _pendingChanges = new List<PendingChangeModel>();

        [HttpPost]
        public IActionResult UpdateComment(int featureId, string comment)
        {

            Feature feature = _db.Features.FirstOrDefault(f => f.FeatureId == featureId);

            if (feature == null)
            {
                return NotFound();
            }

            feature.AdminComments += $"  { comment}"; // Assuming AdminComments is the field for storing comments

            _pendingChanges.Add(new PendingChangeModel
            {
                FeatureID = featureId,
                Action = "Comment",
                Comment = comment // Include the comment in the pending changes
            });

            //_db.SaveChanges();

            TempData["error"] = "Comment updated successfully";

            return Ok("Comment updated successfully");
        }








        [HttpPost]
        public async Task<IActionResult> UpdateAllChangesAsync()
        {

            foreach (var change in _pendingChanges.ToList())
            {
                var feature = _db.Features.FirstOrDefault(f => f.FeatureId == change.FeatureID);

                if (feature != null)
                {
                    switch (change.Action.ToLower())
                    {
                        case "accept":

                            feature.ApprovalStatus = 0;
                            _db.SaveChanges();
                            // Assuming your Azure Function URL and payload format
                            var functionUrl = "https://functionmeshapp.azurewebsites.net/api/FunctionApp1?code=RrtFkS5U6WsNJ3Q1Xk61YQiOQrXN2EANnMX7EJhXvcLhAzFu2EtAjA==";


                            using (var client = new HttpClient())
                            {
                                var response = await client.GetAsync(functionUrl);

                                if (response.IsSuccessStatusCode)
                                {
                                    // Azure Function triggered successfully
                                    // Handle any further logic after Azure Function invocation

                                }
                                else
                                {
                                    Console.WriteLine("Failed to trigger Azure Function");
                                    // Handle the failure scenario accordingly
                                }
                            }
                            break;

                        case "reject":
                            feature.ApprovalStatus = 1;
                            break;
                        case "pending":
                            feature.ApprovalStatus = 2;

                            break;
                        case "comment":
                            feature.AdminComments = change.Comment;
                            break;
                            // Handle other cases if needed
                    }
                }
                else
                {
                    Console.WriteLine($"Feature with ID {change.FeatureID} not found.");
                }

                _pendingChanges.Remove(change);
            }

            // Save changes to the database
            _db.SaveChanges();
            TempData["error"] = "Comment updated successfully";
            return Ok("All changes applied successfully");
        }








        [HttpGet]
        public IActionResult FeatureDetails(int featureId)
        {
            Feature feature = _db.Features.Include(f => f.EntityNameNavigation).FirstOrDefault(f => f.FeatureId == featureId);

            if (feature == null)
            {
                return NotFound();
            }

            return PartialView("_FeatureDetailsPartial", feature); // Pass a single Feature object
        }





        public void ClearPendingChanges()
        {
            _pendingChanges.Clear();
        }






        [HttpPost]
        public IActionResult SearchFeatures(string searchTerm)
        {

            // Perform a search in your database based on the searchTerm
            // Assuming 'FeatureName' is the property you want to search against

            IEnumerable<Feature> searchedFeatures = _db.Features
                .Where(f => f.FeatureName.Contains(searchTerm))
                .ToList();

            if (searchedFeatures.Any())
            {
                // Assuming 'Index' view is designed to display the searched features
                return View("Index", searchedFeatures);
            }
            else
            {
                TempData["error"] = "No features found matching the search term";
                return RedirectToAction("Index");
            }
        }








        [HttpPost]
        public IActionResult AcceptFeature(int featureId)
        {

            // Check if there's an existing pending change for the same feature
            var existingChange = _pendingChanges.FirstOrDefault(p => p.FeatureID == featureId);

            if (existingChange != null)
            {
                // Update the existing pending change action to 'Accept'
                existingChange.Action = "Accept";
            }
            else
            {
                // Add a new pending change for acceptance
                _pendingChanges.Add(new PendingChangeModel
                {
                    FeatureID = featureId,
                    Action = "Accept"
                });
            }

            return Ok("Feature marked for acceptance");
        }


        [HttpPost]
        public IActionResult RejectFeature(int featureId)
        {

            // Check if there's an existing pending change for the same feature
            var existingChange = _pendingChanges.FirstOrDefault(p => p.FeatureID == featureId);



            if (existingChange != null)
            {

                existingChange.Action = "Reject";

            }
            else
            {
                // Add a new pending change for rejection
                _pendingChanges.Add(new PendingChangeModel
                {
                    FeatureID = featureId,
                    Action = "Reject"
                });
            }


            return Ok("Feature marked for rejection");
        }






        [HttpGet]
        public IActionResult CommentBox(int featureId)
        {
            // Assuming _db is your database context and Feature is your model
            var feature = _db.Features.FirstOrDefault(f => f.FeatureId == featureId);

            if (feature == null)
            {
                return NotFound();
            }

            return PartialView("_CommentBoxPatial", feature);

        }




    }
}

