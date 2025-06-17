namespace DTOs
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Descipition { get; set; }
        public int? Price { get; set; }
        public string Url { get; set; }
        // אם רוצים לכלול גם את שם הקטגוריה לדוגמה – אפשר להוסיף:
        // public string CategoryName { get; set; }
    }
}
