using System.Security.Claims;
using app.Application.DTOs;
using app.Application.Errors;
using app.Domain.Constants;
using app.Domain.Interfaces;
using app.Domain.Models;
using Microsoft.AspNetCore.Http;
using FileEntity = app.Domain.Entities.File;

namespace app.Application.UseCases;

public class UpsertBranchImageUseCase
{
    private readonly IBranchRepository _branchRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IImageProcessor _imageProcessor;

    public UpsertBranchImageUseCase(
        IBranchRepository branchRepository,
        IImageProcessor imageProcessor,
        IFileStorageService fileStorageService)
    {
        _branchRepository = branchRepository;
        _imageProcessor = imageProcessor;
        _fileStorageService = fileStorageService;
    }

    public async Task<BranchImageDto> ExecuteAsync(
        ClaimsPrincipal user,
        ulong branchId,
        IFormFile image,
        CancellationToken cancellationToken = default)
    {
        var actorUserId = UserClaimResolver.TryGetUserId(user);

        await using var imageStream = image.OpenReadStream();
        var processedImage = await _imageProcessor.ResizeTo256Async(imageStream, cancellationToken);
        await using var processedStream = processedImage.Content;
        var normalizedFileName = $"{Path.GetFileNameWithoutExtension(image.FileName)}.{processedImage.FileExtension}";

        var uploadResult = await _fileStorageService.UploadAsync(new StorageUploadRequest
        {
            Content = processedStream,
            Size = processedImage.Size,
            FileName = normalizedFileName,
            ContentType = processedImage.ContentType,
            Folder = $"{FileConstants.BranchImageFolder}/{branchId}"
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
            CreatedBy = actorUserId
        };

        FileEntity? previousImage;
        try
        {
            previousImage = await _branchRepository.UpsertImageAsync(branchId, uploadedFile, actorUserId, cancellationToken);
        }
        catch (KeyNotFoundException)
        {
            await _fileStorageService.DeleteAsync(uploadedFile.ObjectKey, cancellationToken);
            throw new KeyNotFoundException(AppErrors.Branch.NotFound);
        }

        if (!string.IsNullOrWhiteSpace(previousImage?.ObjectKey))
        {
            await _fileStorageService.DeleteAsync(previousImage.ObjectKey, cancellationToken);
        }

        return new BranchImageDto
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
