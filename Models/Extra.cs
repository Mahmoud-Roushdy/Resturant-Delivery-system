namespace FinalProject.Models
{
    public class Extra
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        //relations
        //extra
        public int MealId { get; set; }
        public virtual Meal? Meal { get; set; }

    }
}
