namespace Shufl.API.UploadModels.User
{
    public class UserUploadModel : IUploadModel
    {
        public string Email { get; set; }

        public string Username { get; set; }

        public string DisplayName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Password { get; set; }
    }
}
