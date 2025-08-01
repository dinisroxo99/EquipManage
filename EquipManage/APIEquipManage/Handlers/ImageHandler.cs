namespace APIEquipManage.Handlers
{
    public class ImageHandler
    {
        private readonly List<string> _validExtensions = new() { ".png", ".jpg", ".jpeg" };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

        public string UploadImages(IFormFile file)
        {
            string extension = Path.GetExtension(file.FileName).ToLower();

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

            return filename;
        }

        public string DeleteImage(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentException("Filename cannot be null or empty.");
            }

            string path = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Img", filename);

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Image not found.", filename);
            }

            File.Delete(path);

            return $"Image '{filename}' successfully deleted.";
        }
    }
}
