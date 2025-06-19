using AmitalCloud.Invoice.WebAPI.Areas.HelpPage.ModelDescriptions;
using AmitalCloud.Invoice.WebAPI.Areas.HelpPage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System;

namespace AmitalCloud.Invoice.WebAPI.Areas.HelpPage.Controllers
{
    /// <summary>
    /// The controller that will handle requests for the help page.
    /// </summary>
    public class HelpController : Controller
    {
        private const string ErrorViewName = "Error";
        private readonly ModelDescriptionGenerator _modelDescriptionGenerator;
        private readonly IApiDescriptionGroupCollectionProvider _apiExplorer;

        public HelpController(IApiDescriptionGroupCollectionProvider apiExplorer, ModelDescriptionGenerator modelDescriptionGenerator)
        {
            _apiExplorer = apiExplorer;
            _modelDescriptionGenerator = modelDescriptionGenerator;
        }

        public IActionResult Index()
        {
            var apiDescriptions = _apiExplorer.ApiDescriptionGroups;
            return View(apiDescriptions);
        }

        public ActionResult Api(string apiId)
        {
            if (!String.IsNullOrEmpty(apiId))
            {
                var description = _apiExplorer.ApiDescriptionGroups.Items
                                .SelectMany(group => group.Items)
                                .FirstOrDefault(d => d.RelativePath.Equals(apiId, StringComparison.OrdinalIgnoreCase));

                if (description != null)
                {
                    return View(description);
                }
            }

            return View(ErrorViewName);
        }

        public IActionResult ResourceModel(string modelName)
        {
            if (!string.IsNullOrEmpty(modelName))
            {
                if (_modelDescriptionGenerator.GeneratedModels.TryGetValue(modelName, out var modelDescription))
                {
                    return View(modelDescription);
                }
            }

            return View(ErrorViewName);
        }
    }
}