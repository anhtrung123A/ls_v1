using System.Security.Claims;
using app.Application.DTOs;
using app.Application.Errors;
using app.Domain.Constants;
using app.Domain.Entities;
using app.Domain.Interfaces;
using app.Domain.Models;
using Microsoft.AspNetCore.Http;
using FileEntity = app.Domain.Entities.File;

namespace app.Application.UseCases;

public class UpsertUserAvatarUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IImageProcessor _imageProcessor;

    public UpsertUserAvatarUseCase(
        IUserRepository userRepository,
        IImageProcessor imageProcessor,
        IFileStorageService fileStorageService)
    {
        _userRepository = userRepository;
        _imageProcessor = imageProcessor;
        _fileStorageService = fileStorageService;
    }

    public async Task<UserAvatarDto> ExecuteAsync(
        ClaimsPrincipal user,
        IFormFile avatar,
        CancellationToken cancellationToken = default)
    {
        var email = UserClaimResolver.GetEmailOrThrow(user);
        var userWithAvatar = await _userRepository.GetWithAvatarByEmailAsync(email, cancellationToken);

        if (userWithAvatar.User is null)
        {
            throw new KeyNotFoundException(AppErrors.User.UserNotFoundByEmail);
        }

        await using var avatarStream = avatar.OpenReadStream();
        var processedImage = await _imageProcessor.ResizeTo256Async(avatarStream, cancellationToken);
        await using var processedStream = processedImage.Content;
        var normalizedFileName = $"{Path.GetFileNameWithoutExtension(avatar.FileName)}.{processedImage.FileExtension}";

        var uploadResult = await _fileStorageService.UploadAsync(new StorageUploadRequest
        {
            Content = processedStream,
            Size = processedImage.Size,
            FileName = normalizedFileName,
            ContentType = processedImage.ContentType,
            Folder = $"{FileConstants.AvatarFolder}/{userWithAvatar.User.Id}"
        }, cancellationToken);

        var now = DateTime.UtcNow;
        var uploadedFile = new FileEntity
        {
            ObjectKey = uploadResult.ObjectKey,
            BucketName = uploadResult.BucketName,
            FileName = uploadResult.FileName,
            ContentType = uploadResult.ContentType,
            Size = uploadResult.Size,
            IsPublic = true,
            CreatedAt = now,
            CreatedBy = userWithAvatar.User.Id
        };

        var previousAvatar = await _userRepository.UpsertAvatarAsync(userWithAvatar.User.Id, uploadedFile, cancellationToken);
        if (!string.IsNullOrWhiteSpace(previousAvatar?.ObjectKey))
        {
            await _fileStorageService.DeleteAsync(previousAvatar.ObjectKey, cancellationToken);
        }

        return new UserAvatarDto
        {
            Id = uploadedFile.Id,
            ObjectKey = uploadedFile.ObjectKey,
            Url = uploadResult.PublicUrl,
            FileName = uploadedFile.FileName ?? string.Empty,
            ContentType = uploadedFile.ContentType,
            Size = uploadedFile.Size,
            CreatedAt = uploadedFile.CreatedAt
        };
    }
}
