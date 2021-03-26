namespace Shufl.API.UploadModels.User
{
    public class PasswordResetUploadModel : IUploadModel
    {
        public string PasswordResetToken { get; set; }

        public string NewPassword { get; set; }
    }
}
