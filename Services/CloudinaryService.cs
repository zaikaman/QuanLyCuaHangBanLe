using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace QuanLyCuaHangBanLe.Services
{
    public interface ICloudinaryService
    {
        Task<string?> UploadImageAsync(IFormFile file, string folder = "products");
        Task<bool> DeleteImageAsync(string publicId);
        string? GetPublicIdFromUrl(string? url);
    }

    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService()
        {
            var cloudName = Environment.GetEnvironmentVariable("CLOUDINARY_CLOUD_NAME");
            var apiKey = Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY");
            var apiSecret = Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET");

            if (string.IsNullOrEmpty(cloudName) || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiSecret))
            {
                throw new InvalidOperationException("Cloudinary configuration is missing. Please check environment variables.");
            }

            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
            _cloudinary.Api.Secure = true;
        }

        /// <summary>
        /// Upload ảnh lên Cloudinary
        /// </summary>
        /// <param name="file">File ảnh cần upload</param>
        /// <param name="folder">Thư mục lưu trữ trên Cloudinary</param>
        /// <returns>URL của ảnh đã upload hoặc null nếu thất bại</returns>
        public async Task<string?> UploadImageAsync(IFormFile file, string folder = "products")
        {
            if (file == null || file.Length == 0)
                return null;

            // Kiểm tra định dạng file
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(extension))
            {
                throw new InvalidOperationException($"Định dạng file không hợp lệ. Chỉ chấp nhận: {string.Join(", ", allowedExtensions)}");
            }

            // Kiểm tra kích thước file (tối đa 5MB)
            const int maxFileSize = 5 * 1024 * 1024; // 5MB
            if (file.Length > maxFileSize)
            {
                throw new InvalidOperationException("Kích thước file quá lớn. Tối đa 5MB.");
            }

            try
            {
                using var stream = file.OpenReadStream();
                
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = folder,
                    Transformation = new Transformation()
                        .Width(800)
                        .Height(800)
                        .Crop("limit")
                        .Quality("auto")
                        .FetchFormat("auto")
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    Console.WriteLine($"❌ Lỗi upload Cloudinary: {uploadResult.Error.Message}");
                    return null;
                }

                Console.WriteLine($"✅ Upload thành công: {uploadResult.SecureUrl}");
                return uploadResult.SecureUrl?.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi upload: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Xóa ảnh khỏi Cloudinary
        /// </summary>
        /// <param name="publicId">Public ID của ảnh trên Cloudinary</param>
        /// <returns>True nếu xóa thành công</returns>
        public async Task<bool> DeleteImageAsync(string publicId)
        {
            if (string.IsNullOrEmpty(publicId))
                return false;

            try
            {
                var deleteParams = new DeletionParams(publicId);
                var result = await _cloudinary.DestroyAsync(deleteParams);

                if (result.Result == "ok")
                {
                    Console.WriteLine($"✅ Xóa ảnh thành công: {publicId}");
                    return true;
                }

                Console.WriteLine($"⚠️ Không thể xóa ảnh: {result.Result}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi xóa ảnh: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Lấy Public ID từ URL Cloudinary
        /// </summary>
        /// <param name="url">URL đầy đủ của ảnh</param>
        /// <returns>Public ID hoặc null</returns>
        public string? GetPublicIdFromUrl(string? url)
        {
            if (string.IsNullOrEmpty(url))
                return null;

            try
            {
                // URL format: https://res.cloudinary.com/{cloud_name}/image/upload/v{version}/{folder}/{public_id}.{format}
                var uri = new Uri(url);
                var path = uri.AbsolutePath;
                
                // Tìm vị trí "upload/" và lấy phần sau đó
                var uploadIndex = path.IndexOf("/upload/");
                if (uploadIndex == -1)
                    return null;

                var afterUpload = path.Substring(uploadIndex + 8); // 8 = "/upload/".Length
                
                // Bỏ version (v123456789/)
                if (afterUpload.StartsWith("v") && afterUpload.Contains("/"))
                {
                    var versionEnd = afterUpload.IndexOf('/');
                    afterUpload = afterUpload.Substring(versionEnd + 1);
                }

                // Bỏ extension
                var lastDot = afterUpload.LastIndexOf('.');
                if (lastDot > 0)
                {
                    afterUpload = afterUpload.Substring(0, lastDot);
                }

                return afterUpload;
            }
            catch
            {
                return null;
            }
        }
    }
}
