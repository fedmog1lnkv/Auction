using SharedKernel;

namespace Domain.Lots;

public static class LotPhotoErrors
{
    public static readonly Error InvalidSortOrder = new(
        "LotPhotos.InvalidSortOrder",
        "sort_order must be greater than 0.",
        ErrorType.Validation);

    public static readonly Error InvalidKeys = new(
        "LotPhotos.InvalidKeys",
        "Photo storage keys are invalid.",
        ErrorType.Validation);

    public static readonly Error InvalidContentType = new(
        "LotPhotos.InvalidContentType",
        "Unsupported image content type.",
        ErrorType.Validation);

    public static readonly Error TooManyPhotos = Error.Conflict(
        "LotPhotos.TooManyPhotos",
        "Maximum number of lot photos reached.");

    public static readonly Error UploadNotFound = Error.NotFound(
        "LotPhotos.UploadNotFound",
        "Uploaded temporary object was not found.");

    public static readonly Error InvalidImage = new(
        "LotPhotos.InvalidImage",
        "Uploaded file is not a valid image.",
        ErrorType.Validation);

    public static readonly Error FileTooLarge = new(
        "LotPhotos.FileTooLarge",
        "Uploaded file exceeds the maximum allowed size.",
        ErrorType.Validation);

    public static readonly Error PhotoNotFound = Error.NotFound(
        "LotPhotos.NotFound",
        "Photo was not found.");

    public static readonly Error PhotosCanBeManagedOnlyInDraft = Error.Conflict(
        "LotPhotos.ManageOnlyInDraft",
        "Photos can be managed only for DRAFT lots.");

    public static readonly Error InvalidReorderPayload = new(
        "LotPhotos.InvalidReorderPayload",
        "Invalid photo_ids reorder payload.",
        ErrorType.Validation);

    public static readonly Error InvalidUploadId = new(
        "LotPhotos.InvalidUploadId",
        "upload_id must be a valid UUID.",
        ErrorType.Validation);
}