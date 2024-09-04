namespace FinalProject.Models
{
    public class Offer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public decimal Price { get; set; }
        public byte[]? Photo { get; set; }
        public DateTime StartOffer { get; set; }
        public DateTime ExpireOffer { get; set; }

        public virtual ICollection<Meal>? Meals { get; set; }

    }
}
