using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Ramsha;
using Ramsha.AspNetCore.Mvc;
using Ramsha.Common.Domain;
using Ramsha.Files;
using Ramsha.Identity.Domain;
using Ramsha.Localization;
using SimpleAppDemo.Identity;
using SimpleAppDemo.Resources;

namespace SimpleAppDemo.Controllers
{

    public record UploadDto(IFormFile File, string Directory, bool Overwrite = false, bool IsPublic = false);
    public class TestController(IRepository<Org, int> orgRepository, IRepository<Product, int> productRepository, IRepository<Country, int> countryRepository, IFileHandler fileHandler, ILocalizationLanguagesProvider languagesProvider, IStringLocalizer<AdditionalResource> addStringLocalizer, IStringLocalizer<AppResource> appStringLocalizer, IIdentityUserRepository<AppUser, int> repository) : RamshaApiController
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
             paginationParams,
             u => new UserDto(u.Id, u.UserName),
             criteria: x => x.Id > 1
             ));
        }

        [HttpPost(nameof(SeedProductIncludeTest))]
        public async Task<IActionResult> SeedProductIncludeTest(string name)
        {
            var demoOrg = new Org { Name = "Demo" };

            await orgRepository.AddAsync(demoOrg, true);


            var yemenCountry = new Country { Name = "Yemen", Org = demoOrg };
            var omanCountry = new Country { Name = "Oman", Org = demoOrg };

            await countryRepository.AddRangeAsync([yemenCountry, omanCountry], true);


            List<Inventory> inventories =
            [
               new Inventory
               {
                Name = name +omanCountry.Name+ "Inventory",
                Prices=[
                new Price
                    {
                        Value =100,
                        Discounts = [new PriceDiscount{Value = 1},new PriceDiscount{Value = 2}]
                    },
                new Price
                    {
                        Value= 200,
                        Discounts = [new PriceDiscount{Value = 2},new PriceDiscount{Value = 3}]
                    }
                ],
                Country = omanCountry
               },
            new Inventory
               {
                Name = name +yemenCountry.Name+ "Inventory",
                  Prices=[
                new Price
                    {
                        Value =300,
                        Discounts = [new PriceDiscount{Value = 3},new PriceDiscount{Value = 4}]
                    },
                new Price
                    {
                        Value= 400,
                        Discounts = [new PriceDiscount{Value = 4},new PriceDiscount{Value = 5}]
                    }
                ],
                Country = yemenCountry
               },
            ];

            var product = new Product
            {
                Name = name,
                Inventories = inventories
            };

            var createdProduct = await productRepository.AddAsync(product, true);

            return Ok(new
            {
                createdProduct?.Id
            });

        }

        [HttpGet(nameof(GetProductIncludeTest))]
        public async Task<IActionResult> GetProductIncludeTest([FromQuery] PaginationParams paginationParams)
        {
            //var specification = RamshaSpecification
            //.For<Product>()
            // .Include(
            //     [
            //      p => p.Inventories.Select(x => x.Country.Org),
            //      p => p.Inventories.Select(i=> i.Prices.Select(p=> p.Discounts))
            //     ])
            // .Create();

            return Ok(await productRepository.GetPagedAsync(new ProductPageSpec(paginationParams)));

            // return Ok(await productRepository.GetListAsync(
            //     [
            //      p => p.Inventories.Select(x => x.Country.Org),
            //      p => p.Inventories.Select(i=> i.Prices.Select(p=> p.Discounts))
            //     ]
            // ));
        }
    }

    public class ProductDetailSpec : Specification<Product>
    {
        public ProductDetailSpec()
        {
            Query.Include(p => p.Inventories)
               .ThenInclude(x => x.Country)
               .ThenInclude(x => x.Org)
               .Include(x => x.Inventories)
               .ThenInclude(x => x.Prices)
               .ThenInclude(x => x.Discounts);
        }

        public static ProductDetailSpec Create()
        {
            return new ProductDetailSpec();
        }



    }


    public record UserDto(int Id, string UserName);

    public class ProductPageSpec : PageSpecification<Product, ProductDto>
    {
        public ProductPageSpec(PaginationParams paginationParams)
        : base(paginationParams)
        {
            Query
            .Include(p => p.Inventories)
            .ThenInclude(x => x.Country)
            .ThenInclude(x => x.Org)
            .Include(x => x.Inventories)
            .ThenInclude(x => x.Prices)
            .ThenInclude(x => x.Discounts)
            .Select(p => new ProductDto(p.Id, p.Name, p.Inventories.Select(i => new InventoryDto(i.Id, i.Name)).ToList()));
        }
    }

    public record ProductDto(int Id, string Name, List<InventoryDto> Inventories);
    public record InventoryDto(int Id, string Name);




}