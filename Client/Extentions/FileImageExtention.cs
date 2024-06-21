
namespace Client.Extentions
{
    public static class FileImageExtention
    {
        public static bool IsValidImageFile(this IFormFile file)
        {
            // Define the allowed extensions
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

            // Get the file extension
            string fileExtension = Path.GetExtension(file.FileName).ToLower();

            // Check if the file extension is in the list of allowed extensions
            return allowedExtensions.Contains(fileExtension);
        }
    }
}
