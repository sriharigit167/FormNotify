namespace FormSubmission_API.Models
{
    // Models/ContactFormModel.cs
    public class ContactFormModel
    {
        public string Purpose { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Company { get; set; }
        public string Phone { get; set; }
        public string Message { get; set; }
        public IFormFile Document { get; set; }
    }
}
