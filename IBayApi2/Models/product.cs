namespace IBayApi2.Models;

public class product
{
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
        public bool Available { get; set; }
        public DateTime AddedTime { get; set; }
}