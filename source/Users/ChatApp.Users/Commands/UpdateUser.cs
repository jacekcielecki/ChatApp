using ChatApp.Shared.Model.Users;
using ChatApp.Shared.Model.ValueObjects;
using ChatApp.Users.Core.Details;
using OneOf;
using OneOf.Types;

namespace ChatApp.Users.Commands;

public class UpdateUser
{
    private readonly GetUserByIdRepository _getUserByIdRepository;
    private readonly UpdateUserRepository _updateUserRepository;

    public UpdateUser(GetUserByIdRepository getUserByIdRepository, UpdateUserRepository updateUserRepository)
    {
        _getUserByIdRepository = getUserByIdRepository;
        _updateUserRepository = updateUserRepository;
    }

    public async Task<OneOf<Success, ValidationErrors>> Update(Guid userId, UserUpdateDto dto)
    {
        var errors = new Dictionary<string, string[]>();

        var user = await _getUserByIdRepository.Get(userId);
        if (user is null)
        {
            errors.Add("User", ["User not found."]);
            return new ValidationErrors(errors);
        }

        user.Bio = dto.Bio;
        if (dto.DeleteProfilePicture)
        {
            user.ProfilePicUrl = null;
        }
        else
        {
            //using var ms = new MemoryStream();
            //await dto.ProfilePicture.CopyToAsync(ms);
            //user.ProfilePicture = new ProfilePicture(ms.ToArray(), dto.ProfilePicture.ContentType);
        }

        await _updateUserRepository.Update(user);
        return new Success();
    }
}
