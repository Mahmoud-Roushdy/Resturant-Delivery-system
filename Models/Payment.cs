namespace FinalProject.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }  
        public decimal Amount { get; set; }  
        public DateTime PaymentDate { get; set; }

        /// <summary>
        /// relations
        /// </summary>
        public int CustomerId { get; set; }
        public virtual Customer? Customer { get; set; }
        public int OrderId { get; set; }
        public virtual Order? Order { get; set;}

    }
}
