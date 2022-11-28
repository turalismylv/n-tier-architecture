namespace Web.ViewModels.Expert
{
    public class ExpertCreateVM
    {
       
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Position { get; set; }
        public IFormFile MainPhoto { get; set; }
    }
}
