using AutoMapper;
using FribergCarRental.Core.Classes;
using FribergCarRental.Core.Dtos;
using FribergCarRental.DAL.Data;
using Microsoft.AspNetCore.Mvc;

namespace FribergCarRental.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {

        public ImageRepository _imageRepository { get; set; }
        public IMapper _mapper { get; }

        public ImageController(ImageRepository imageRepository, IMapper mapper)
        {
            _imageRepository = imageRepository;
            _mapper = mapper;
        }

        [HttpGet("bycar/{id}")]
        public async Task<ActionResult<List<ImageDto>>> GetImagesByCarId(int id)
        {
            var images = await _imageRepository.GetAllImagesByCarIdAsync(id);
            var imageDtos = _mapper.Map<List<ImageDto>>(images);
            return imageDtos;
        }

        [HttpPost]
        public async Task<ActionResult<ImageDto>> AddImage([FromBody] ImageDto imageDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var image = _mapper.Map<Image>(imageDto);
            await _imageRepository.AddAsync(image);
            await _imageRepository.SaveChangesAsync();

            var createdImageDto = _mapper.Map<ImageDto>(image);
            return CreatedAtAction(nameof(GetById), new { id = image.ImageId }, createdImageDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ImageDto>> GetById(int id)
        {
            var image = await _imageRepository.GetByIdAsync(id);
            if (image == null)
            {
                return NotFound($"No image with id {id} was found");
            }
            var imageDto = _mapper.Map<ImageDto>(image);
            return imageDto;
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteImage(int id)
        {
            var image = await _imageRepository.GetByIdAsync(id);
            if (image == null)
            {
                return NotFound();
            }

            _imageRepository.Delete(image);
            await _imageRepository.SaveChangesAsync();
            return Ok();
        }

    }
}
