namespace APIEquipManage.Handlers
{
    public class UploadHandler
    {
        private readonly List<string> _validExtensions = new() { ".png", ".jpg", ".jpeg" };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

        public string UploadImages(IFormFile file)
        {
            string extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!_validExtensions.Contains(extension))
            {
                throw new ArgumentException($"Extension '{extension}' is not valid. Allowed: {string.Join(", ", _validExtensions)}");
            }

            if (file.Length > MaxFileSize)
            {
                throw new ArgumentException("Maximum file size is 5MB.");
            }

            string filename = Guid.NewGuid() + extension;
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Img");

            string fullPath = Path.Combine(path, filename);
            using FileStream stream = new FileStream(fullPath, FileMode.Create);
            file.CopyTo(stream);

            return fullPath;
        }
    }
}
