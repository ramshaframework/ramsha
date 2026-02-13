// using Microsoft.AspNetCore.Mvc;
// using Ramsha.AspNetCore.Mvc;
// using Ramsha.Caching;
// using Ramsha.Common.Domain;
// using Ramsha.Localization;
// using SimpleAppDemo.LocalizationModule;

// namespace SimpleAppDemo.Controllers
// {
//     public class LocalizationModuleController(IRamshaCache ramshaCache, IResourcesDefinitionsProvider resourcesDefinitionsProvider, IRepository<LocalizationText, Guid> repository) : RamshaApiController
//     {
//         [HttpGet("resources")]
//         public async Task<IActionResult> GetResources()
//         {
//             return Ok(await resourcesDefinitionsProvider.GetAllResourcesAsync());
//         }

//         [HttpGet]
//         public async Task<IActionResult> GetList()
//         {
//             return Ok(await repository.GetListAsync());
//         }

//         [HttpPut]
//         public async Task<IActionResult> Set(string resourceName, string culture, string key, string value)
//         {
//             var text = await repository.FindAsync(x =>
//              x.ResourceName == resourceName
//              && x.Culture == culture
//              && x.Key == key);

//             if (text is null)
//             {
//                 text = new LocalizationText(
//                     Guid.NewGuid(),
//                      resourceName,
//                       culture,
//                        key);

//                 await repository.AddAsync(text);
//             }

//             text.SetValue(value);


//             await ramshaCache.RemoveByTagAsync(RamshaResourcesLoader.ResourceTagPrefix + resourceName);

//             return Ok(text);

//         }
//     }
// }