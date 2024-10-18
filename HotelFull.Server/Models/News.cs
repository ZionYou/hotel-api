namespace HotelAPI.Models
{
    public class News
    {
        public int id { get; set; }

        public string subject { get; set; }

        public string content { get; set; }    
        public DateTime publishDate { get; set; }

        public bool visable { get; set; }
    }
}
