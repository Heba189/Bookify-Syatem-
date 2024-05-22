using Bookify.Web.Core.Models;

namespace Bookify.Web.Services
{
	public class ImageService : IImageService
	{
		private readonly IWebHostEnvironment _webHostEnvironment;
		private List<string> _allowedExtensions = new() { ".jpg", ".jpeg", ".png" };
		private int _maxAllowedSize = 2097152;
		public ImageService(IWebHostEnvironment webHostEnvironment)
		{
			_webHostEnvironment = webHostEnvironment;
		}

	

		public async Task<(bool isUploaded, string? errorMessage)> UploadedAsync(IFormFile image, string imageName, string folderPath, bool hasThumbnail)
		{
			var extension = Path.GetExtension(image.FileName);

			if (!_allowedExtensions.Contains(extension))
				return (isUploaded: false, errorMessage: Errors.NotAllowedExtension);

			if (image.Length > _maxAllowedSize)
				return (isUploaded: false, errorMessage: Errors.MaxSize);

			var path = Path.Combine($"{_webHostEnvironment.WebRootPath}/{folderPath}", imageName);

			using var stream = File.Create(path);
			await image.CopyToAsync(stream);
			stream.Dispose();

			if (hasThumbnail)
			{
			var thumbPath = Path.Combine($"{_webHostEnvironment.WebRootPath}/{folderPath}/thumb", imageName);

	        using var loadedimage = Image.Load(image.OpenReadStream());
			var ratio = (float)loadedimage.Width / 200;
			var height = loadedimage.Height / ratio;
			loadedimage.Mutate(i => i.Resize(width: 200, height: (int)height));
			loadedimage.Save(thumbPath);
			}

			return (isUploaded: true, errorMessage: null);

		}

		public void Delete(string imagePath, string? imageThumbnailPath = null)
		{
			var oldImagePath = $"{_webHostEnvironment.WebRootPath}{imagePath}";

			if(File.Exists(oldImagePath))
				File.Delete(oldImagePath);

			if(!string.IsNullOrEmpty(imageThumbnailPath))
			{
			var oldThumbPath = $"{_webHostEnvironment.WebRootPath}{imageThumbnailPath}";
				if (System.IO.File.Exists(oldThumbPath))
					System.IO.File.Delete(oldThumbPath);
			}
	
		}
	}
}
