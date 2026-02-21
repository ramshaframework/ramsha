using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Ramsha;
using Ramsha.AspNetCore.Mvc;
using Ramsha.Files;
using Ramsha.Identity.Domain;
using Ramsha.Localization;
using SimpleAppDemo.Identity;
using SimpleAppDemo.Resources;

namespace SimpleAppDemo.Controllers
{

    public record UploadDto(IFormFile File, string Directory, bool Overwrite = false, bool IsPublic = false);
    public class TestController(IFileHandler fileHandler, ILocalizationLanguagesProvider languagesProvider, IStringLocalizer<AdditionalResource> addStringLocalizer, IStringLocalizer<AppResource> appStringLocalizer, IIdentityUserRepository<AppUser, int> repository) : RamshaApiController
    {
        [HttpPost("file-upload")]
        public async Task<ActionResult<FileStoreResponse>> UploadFile([FromForm] UploadDto uploadDto)
        {
            await using var stream = uploadDto.File.OpenReadStream();
            return RamshaResult(await fileHandler.SaveAsync(stream, new(uploadDto.File.FileName, uploadDto.Directory, uploadDto.Overwrite, uploadDto.IsPublic)));
        }

        [HttpDelete("file-delete")]
        public async Task<ActionResult<FileStoreResponse>> DeleteFile(RamshaFileInfo fileInfo)
        {
            return RamshaResult(await fileHandler.DeleteAsync(fileInfo));
        }

        [HttpGet("video")]
        public async Task<IActionResult> GetVideo([FromQuery] RamshaFileInfo fileInfo)
        {
            var stream = await fileHandler.GetAsync(fileInfo);
            if (stream == null)
                return NotFound();

            return File(stream, "video/mp4", enableRangeProcessing: true);
        }

        [HttpGet("image-read")]
        public async Task<IActionResult> GetFile([FromQuery] RamshaFileInfo fileInfo)
        {
            return File(await fileHandler.GetAsync(fileInfo), "image/png");
        }

        // [HttpPost("send-simple-message")]
        // public async Task<IActionResult> SendSimpleMessage(string text)
        // {
        //     await bus.PublishAsync(new SimpleMessage(text));
        //     return Ok();
        // }

        [HttpGet("defaultLanguage")]
        public async Task<ActionResult> DefaultLanguage()
        {
            return Ok(await languagesProvider.GetDefaultLanguage());
        }

        [HttpGet("supported-languages")]
        public async Task<ActionResult> GetSupportedLanguages()
        {
            return Ok(await languagesProvider.GetSupportedLanguagesAsync());
        }

        [HttpGet("localize-app")]
        public async Task<ActionResult> Localize(string key)
        {
            return Ok(appStringLocalizer[key]);
        }

        [HttpGet("localize-add")]
        public async Task<ActionResult> LocalizeAdd(string key)
        {
            return Ok(addStringLocalizer[key]);
        }


        [HttpPost]
        public async Task<ActionResult<List<UserDto>>> GetPaged(PaginationParams paginationParams)
        {
            return RamshaResult(await repository.GetPagedAsync(
             q => q.Where(x => x.Id > 1),
             paginationParams,
             u => new UserDto(u.Id, u.UserName)));
        }
    }


    public record UserDto(int Id, string UserName);
}