using Application.LotPhotos.CompleteLotPhotoUpload;
using Application.LotPhotos.DeleteLotPhoto;
using Application.LotPhotos.GetLotPhotos;
using Application.LotPhotos.ReorderLotPhotos;
using Application.LotPhotos.RequestLotPhotoUploadUrl;
using Application.Lots.CancelLot;
using Application.Lots.CreateLot;
using Application.Lots.DeleteLot;
using Application.Lots.FinishLot;
using Application.Lots.GetLotById;
using Application.Lots.GetLots;
using Application.Lots.StartLot;
using Application.Lots.UpdateLot;
using Domain.Lots;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Api.Controllers;

[Route("lots")]
public sealed class LotsController(ISender sender) : BaseController
{
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLotRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateLotCommand(
            request.Title,
            request.Description,
            request.StartingPrice,
            request.MinBidStep,
            request.StartsAt,
            request.EndsAt);

        var result = await sender.Send(command, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetList(
        [FromQuery] LotStatus? status,
        [FromQuery(Name = "seller_id")] Guid? sellerId,
        [FromQuery] int page = 1,
        [FromQuery] int limit = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetLotsQuery(status, sellerId, page, limit);
        var result = await sender.Send(query, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetLotByIdQuery(id);
        var result = await sender.Send(query, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [Authorize]
    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateLotRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateLotCommand(
            id,
            request.Title,
            request.Description,
            request.StartingPrice,
            request.MinBidStep,
            request.StartsAt,
            request.EndsAt);

        var result = await sender.Send(command, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [Authorize]
    [HttpPost("{id:guid}/start")]
    public async Task<IActionResult> Start([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new StartLotCommand(id);
        var result = await sender.Send(command, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [Authorize]
    [HttpPost("{id:guid}/finish")]
    public async Task<IActionResult> Finish([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new FinishLotCommand(id);
        var result = await sender.Send(command, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [Authorize]
    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new CancelLotCommand(id);
        var result = await sender.Send(command, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteLotCommand(id);
        var result = await sender.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : HandleFailure(result);
    }

    [Authorize]
    [HttpPost("{lotId:guid}/photos/upload-url")]
    public async Task<IActionResult> RequestPhotoUploadUrl(
        [FromRoute] Guid lotId,
        [FromBody] RequestLotPhotoUploadUrlRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RequestLotPhotoUploadUrlCommand(lotId, request.ContentType);
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [Authorize]
    [HttpPost("{lotId:guid}/photos/complete")]
    public async Task<IActionResult> CompletePhotoUpload(
        [FromRoute] Guid lotId,
        [FromBody] CompleteLotPhotoUploadRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CompleteLotPhotoUploadCommand(lotId, request.UploadId);
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [AllowAnonymous]
    [HttpGet("{lotId:guid}/photos")]
    public async Task<IActionResult> GetPhotos([FromRoute] Guid lotId, CancellationToken cancellationToken)
    {
        var query = new GetLotPhotosQuery(lotId);
        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [Authorize]
    [HttpPatch("{lotId:guid}/photos/reorder")]
    public async Task<IActionResult> ReorderPhotos(
        [FromRoute] Guid lotId,
        [FromBody] ReorderLotPhotosRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ReorderLotPhotosCommand(lotId, request.PhotoIds ?? Array.Empty<Guid>());
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [Authorize]
    [HttpDelete("{lotId:guid}/photos/{photoId:guid}")]
    public async Task<IActionResult> DeletePhoto(
        [FromRoute] Guid lotId,
        [FromRoute] Guid photoId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteLotPhotoCommand(lotId, photoId);
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess ? Ok(new { success = true }) : HandleFailure(result);
    }
}