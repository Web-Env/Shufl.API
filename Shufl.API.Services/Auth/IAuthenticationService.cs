using Shufl.API.DownloadModels.Auth;
using Shufl.API.UploadModels.Auth;
using Shufl.Domain.Repositories.User.Interfaces;
using System.Threading.Tasks;

namespace Shufl.API.Services.Auth
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest model, IUserRepository userRepository);
    }
}
