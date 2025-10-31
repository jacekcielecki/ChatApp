using ChatApp.Shared.Data.BlobStorage;
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
    private readonly IFileStorage _fileStorage;

    public UpdateUser(GetUserByIdRepository getUserByIdRepository, UpdateUserRepository updateUserRepository, IFileStorage fileStorage)
    {
        _getUserByIdRepository = getUserByIdRepository;
        _updateUserRepository = updateUserRepository;
        _fileStorage = fileStorage;
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
            user.ProfilePictureUrl = null;

        if (dto is { DeleteProfilePicture: false, ProfilePicture: not null })
        {
            var filePath = $"profilePictures/{dto.ProfilePicture.FileName}";

            var upload = await _fileStorage.Upload(dto.ProfilePicture, filePath);
            if (upload.IsError)
            {
                errors.Add("Upload File", [upload.Status!]);
                return new ValidationErrors(errors);
            }
            user.ProfilePictureUrl = upload.FileUri;
        }

        await _updateUserRepository.Update(user);
        return new Success();
    }
}
