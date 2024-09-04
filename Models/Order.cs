namespace FinalProject.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string Detail { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DeliveryTime { get; set; }
        public decimal OrderPrice { get; set; }
        //public Status Status { get; set; } //enum //tree cases
        // ppending / accepting /complete

        //relations
        public int CustomerId { get; set; }
        public virtual Customer? Customer { get; set; }
        public int CashierId { get; set; }
        public Cashier? Cashier { get; set; }
        public int DeliveryBoyId { get; set; }
        public DeliveryBoy? DeliveryBoy  { get; set; }
        public virtual ICollection<OrderMeal>? OrderMeals { get; set; }
    }

   
}
