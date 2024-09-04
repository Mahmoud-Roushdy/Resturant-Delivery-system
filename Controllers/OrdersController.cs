using FinalProject.Data;
using FinalProject.DTO;
using FinalProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create( OrderDto orderDto)
        {
            var order = new Order
            {
                CashierId = 1,
                DeliveryBoyId = 1,
                CustomerId =1,
                Detail= "aaa",
                DeliveryTime = DateTime.Now,
                OrderDate= DateTime.Now,

            };
            _context.Orders.Add(order);
            _context.SaveChanges();
            Console.WriteLine(order);
            decimal oderPrice = 0;
            foreach ( var item in orderDto.Details)
            {
                decimal price = 0;
                if (_context.Meals.Find(item.MealId).Discount > 0)
                {
                      price = _context.Meals.Find(item.MealId).Discount ;
                }
                else
                {
                    price = _context.Meals.Find(item.MealId).Price;

                }
                var orderMeal = new OrderMeal
                {
                    OrderId = order.Id,
                    MealId = item.MealId,
                    Amount = item.Quantity,
                    TotalPrice = price * item.Quantity
                };
                oderPrice += orderMeal.TotalPrice;
                _context.OrderMeals.Add(orderMeal);
            }
           //var total =  order.OrderMeals.Where(s => s.OrderId == order.Id).Select(s => s.TotalPrice).Sum(); 

            order.OrderPrice = oderPrice;
            _context.SaveChanges();

            return Ok(order);
        }



     }
}
