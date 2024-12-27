namespace E_Tickets.Models.ViewModels
{
    public class TicketDetailsVM
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string MovieName { get; set; }
        public string CinemaName { get; set; }
        public double Price { get; set; }
        public int Count { get; set; }
        public double TotalPrice { get; set; }
        public DateTime Date { get; set; }
    }
}
