using AutoMapper;
using FribergCarRental.Data;
using FribergCarRental.Models;
using Microsoft.AspNetCore.Mvc;

namespace FribergCarRental.Controllers
{
    public class CarsController : Controller
    {
        private readonly IMapper _mapper;
        private List<CarViewModel> _carViewModels = new List<CarViewModel>();
        private CarRepository _carRepository;
        private ImageRepository _imageRepository;


        public CarsController(ApplicationDbContext context, IMapper mapper, CarRepository carRepository, ImageRepository imageRepository)
        {
            _mapper = mapper;
            _carRepository = carRepository;
            _imageRepository = imageRepository;
        }

        // GET: Cars
        public async Task<IActionResult> Index()
        {
            var cars = await _carRepository.GetAllAsync();
            _carViewModels = _mapper.Map<List<CarViewModel>>(cars);
            foreach (var car in _carViewModels)
            {
                car.Images = _mapper.Map<List<ImageViewModel>>(await _imageRepository.GetAllImagesByCarIdAsync(car.CarId));
            }
            return View(_carViewModels);
        }

        // GET: Cars/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _carRepository.GetByIdAsync(id);
            var carViewModel = _mapper.Map<CarViewModel>(car);
            if (carViewModel == null)
            {
                return NotFound();
            }

            return View(carViewModel);
        }

        public async Task<IActionResult> Images(int id)
        {
            var car = await _carRepository.GetByIdAsync(id);
            var images = await _imageRepository.GetAllImagesByCarIdAsync(id);

            var viewModel = _mapper.Map<CarViewModel>(car);
            viewModel.Images = _mapper.Map<List<ImageViewModel>>(images);
            return View(viewModel);
        }
    }
}
