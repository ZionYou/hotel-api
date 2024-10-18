namespace test.Server.Service
{
    public class NewsService
    {

        public DateTime getToday()
        {
            DateTime today = DateTime.Now;
            string myDateString = today.ToString("yyyy-MM-dd");
            DateTime dt = Convert.ToDateTime(myDateString);

            return dt;
        }
    }
}
