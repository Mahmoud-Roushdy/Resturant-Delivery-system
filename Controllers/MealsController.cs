using FinalProject.Data;
using FinalProject.DTO;
using FinalProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace FinalProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MealsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        [Obsolete]
        private readonly IHostingEnvironment _host;

        [Obsolete]
        public MealsController(ApplicationDbContext context, IHostingEnvironment host)
        {
            _context = context;
            _host = host;
        }
        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll()
        {
            var meals = await _context.Meals.OrderBy(g => g.Name).ToListAsync();
            return Ok(meals);
        }
        [HttpGet("getById/{id}")]
        //[Obsolete]
        public async Task<IActionResult> GetById(int id)
        {
            //TODO:: validate=> photo name do not repeat
            var meal = await _context.Meals.FindAsync(id);
            //var  newMeal = new Meal
            // {
            //     Id = meal.Id,
            //     Name = meal.Name,
            //     Details = meal.Details,
            //     Discount = meal.Discount,
            //     Price = meal.Price,
            //     CategoryId = meal.CategoryId,
            //    //assign in photo full path from wwwroot like(D:\ITI\.AA_Final_Project_ITI\FinalProject_DotNet_API\wwwroot/images/meals/hackerrank4.png)
            //    Photo = $"{_host.WebRootPath}/images/meals/{meal.Photo}", //get full path for image before sending it to angular
            //    //Photo = @"D:\\ITI\\.AA_Final_Project_ITI\\Angular\\src\\assets\\images\\meals" + "/images/meals/" + meal.Photo, //get full path for image before sending it to angular
            //};



            return Ok(meal);
        }

        [HttpPost("create")]

        public async Task<IActionResult> Create([FromBody] MealDto mealDto)
        {
            Console.WriteLine("mealdto");
            Console.WriteLine(mealDto);

            //if (User.Identity.IsAuthenticated) { }
            if (!ModelState.IsValid)
            {
                return BadRequest("Not valid Ahmed");
            }
            //if (mealDto.Photo == null)
            //    return BadRequest("photo is required!");


            var newMeal = new Meal
            {
                Name = mealDto.Name,
                Details = mealDto.Details,
                Price = mealDto.Price,
                Discount = mealDto.Discount,
                Photo = mealDto.Photo,
                CategoryId = mealDto.CategoryId,

            };
            #region Testing


            //using (var memoryStream = new MemoryStream())
            //{
            //    await mealDto.Photo.CopyToAsync(memoryStream);
            //    newMeal.Photo = memoryStream.ToArray();
            //}
            //handle image 
            //if (mealDto.Photo != null)
            //{
            //    using var dataStream = new MemoryStream();
            //    await mealDto.Photo.CopyToAsync(dataStream);
            //    newMeal.Photo = dataStream.ToArray();
            //}
            // Handle image upload
            //if (mealDto.Photo != null)
            //{
            //    using (var dataStream = new MemoryStream())
            //    {
            //        await mealDto.Photo.CopyToAsync(dataStream);

            //        // Save the image to the database and get the path
            //        newMeal.PhotoPath = SaveImageToDatabase(dataStream.ToArray());
            //    }
            //}
            #endregion
            await _context.Meals.AddAsync(newMeal);
            _context.SaveChanges();
            return Ok(newMeal);
        }
        [HttpPost("create2")]
        //[Route("create2")]
        //[Obsolete]
        public async Task<IActionResult> Create2()
        {

            //TODO:: validate=> photo name do not repeat
            var image = HttpContext.Request.Form.Files["image"];// this file not string from angular 
            var name = HttpContext.Request.Form["name"];
            var categoryId = HttpContext.Request.Form["categoryId"];
            var discount = HttpContext.Request.Form["discount"];
            var details = HttpContext.Request.Form["details"];
            var price = HttpContext.Request.Form["price"];
            Console.WriteLine(name);
            if (!string.IsNullOrEmpty(name) && image != null && image.Length > 0)
            {
                ////real path
                var filepath = Path.Combine(_host.WebRootPath + "/images/meals", image.FileName);//
                //var filepath = @"D:\\ITI\\.AA_Final_Project_ITI\\Angular\\src\\assets\\images\\meals" + "/images/meals" + image.FileName;
                using (FileStream filestream = new FileStream(filepath, FileMode.Create))
                {
                    await image.CopyToAsync(filestream);
                }
                var meal = new Meal
                {
                    Name = name,
                    CategoryId = int.Parse(categoryId),
                    Discount = int.Parse(discount),
                    Price = int.Parse(price),
                    Details = details,
                    Photo = image.FileName

                };
                _context.Meals.Add(meal);
                await _context.SaveChangesAsync();
                return Ok(meal);

            }
            return BadRequest();

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] MealDto mealDto)
        {
            var meal = await _context.Meals.SingleOrDefaultAsync(g => g.Id == id);
            if (meal == null)
            {
                return NotFound("this meal is not existed");
            }
            meal.Name = mealDto.Name;
            meal.Discount = mealDto.Discount;
            meal.Details = mealDto.Details;
            meal.Price = mealDto.Price;
            meal.CategoryId = mealDto.CategoryId;
            meal.Photo = mealDto.Photo;

            _context.SaveChanges();

            return Ok(meal);

        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var meal = await _context.Meals.SingleOrDefaultAsync(g => g.Id == id);
            if (meal == null)
            {
                return NotFound("this meal is not existed");
            }
            _context.Meals.Remove(meal);
            _context.SaveChanges();
            return Ok(meal);
        }


        /// Helper method to save image to the database and return the path
        //private string SaveImageToDatabase(byte[] imageData)
        //{
        //    // Implement your logic to save the image to the database
        //    // For simplicity, let's assume you're saving to a "Images" table with an "ImagePath" column

        //    // Example logic (you might need to adapt this based on your database structure)
        //    var imageEntity = new ImageEntity { ImageData = imageData };
        //    _context.Images.Add(imageEntity);
        //    _context.SaveChanges();

        //    // Return the path or identifier of the saved image
        //    return $"images/{imageEntity.Id}";
        //}
    }

    
}
