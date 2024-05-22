namespace Bookify.Web.Services
{
	public interface IImageService
	{
		Task<(bool isUploaded, string? errorMessage)> UploadedAsync(IFormFile image, string imageName, string folderPath, bool hasThumbnail);
		void Delete(string imagePath , string? imageThumbnailPath = null);
	}
}
