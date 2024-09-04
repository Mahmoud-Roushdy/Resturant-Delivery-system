namespace FinalProject.DTO
{
    public class OrderDto
    {
        
        public string CustumerId { get; set; }
        public detail[] Details { get; set; }



        public class detail
        {
            public int MealId { get; set; }
            public int Quantity { get; set; }
        }

        //details: { mealId: number; quantity: number }
        //    []
        //custumerId: string


    }

  
}
