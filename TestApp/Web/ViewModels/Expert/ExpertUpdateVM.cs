namespace Web.ViewModels.Expert
{
    public class ExpertUpdateVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Position { get; set; }

        public string? MainPhotoName { get; set; }
        public IFormFile? MainPhoto { get; set; }
    }
}
