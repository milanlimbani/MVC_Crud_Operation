using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Saturday.Data;
using Saturday.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Saturday.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AccountController(
            AppDbContext appDbContext,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _appDbContext = appDbContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login(string returnurl = null)
        {
            ViewData["ReturnUrl"] = returnurl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel, string returnurl = null)
        {
            ViewData["ReturnUrl"] = returnurl;
            returnurl = returnurl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var item = await _signInManager.PasswordSignInAsync(loginViewModel.Email, loginViewModel.Password, loginViewModel.Remember, lockoutOnFailure: true);
                if (item.Succeeded)
                {
                    return LocalRedirect(returnurl);
                }
                if (item.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Only 2 attempts allowed. Your account is locked for 2 minutes.");
                    return RedirectToAction("Locked", "Account");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Oops! Something went wrong.");
                    return View(loginViewModel);
                }
            }
            return View(loginViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Signup(string returnurl = null)
        {
            ViewData["ReturnUrl"] = returnurl;
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
                await _roleManager.CreateAsync(new IdentityRole("User"));
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Signup(RegisterViewModel registerViewModel, string returnurl = null)
        {
            ViewData["ReturnUrl"] = returnurl;
            returnurl = returnurl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var data = new IdentityUser
                {
                    UserName = registerViewModel.Email,
                    Email = registerViewModel.Email
                };
                var item = await _userManager.CreateAsync(data, registerViewModel.Password);
                if (item.Succeeded)
                {
                    int count = _userManager.Users.Count();
                    if (count == 1)
                    {
                        await _userManager.AddToRoleAsync(data, "Admin");
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(data, "User");
                    }
                    await _signInManager.SignInAsync(data, isPersistent: false);
                    return LocalRedirect(returnurl);
                }
                ModelState.AddModelError(string.Empty, "Oops! Something went wrong.");
            }
            return View(registerViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Locked()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult NewBus()
        {
            return View();
        }

        [HttpPost]
        public IActionResult NewBus(AddBus addBus)
        {
            if (ModelState.IsValid)
            {
                var data = new AddBus
                {
                    BusName = addBus.BusName,
                    BusNumber = addBus.BusNumber,
                    Source = addBus.Source,
                    Departuare = addBus.Departuare,
                    DepartuareTime = addBus.DepartuareTime,
                    Destination = addBus.Destination,
                    Arriaval = addBus.Arriaval,
                    MaxCapisity = addBus.MaxCapisity,
                    ArrivalTime = addBus.ArrivalTime,
                    CreationDate = DateTime.Now
                };
                var item = _appDbContext.addBuses.Add(data);
                _appDbContext.SaveChanges();
                return RedirectToAction("AdminViewBus", "Account");
            }
            ModelState.AddModelError(string.Empty, "Oops! Something went wrong.");
            return View(addBus);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult AdminViewBus()
        {
            ViewBag.viewBuses = _appDbContext.addBuses.ToList();
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult EditBus(int id)
        {
            var bus = _appDbContext.addBuses.FirstOrDefault(o => o.Id == id);
            if (bus == null)
            {
                return NotFound();
            }
            var editBusViewModel = new EditBusViewModel
            {
                Id = bus.Id,
                BusName = bus.BusName,
                BusNumber = bus.BusNumber,
                Source = bus.Source,
                Departuare = bus.Departuare,
                DepartuareTime = bus.DepartuareTime,
                Destination = bus.Destination,
                Arriaval = bus.Arriaval,
                MaxCapisity = bus.MaxCapisity,
                ArrivalTime = bus.ArrivalTime,
                IsClose = bus.IsClose
            };

            return View(editBusViewModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult EditBus(EditBusViewModel editBusViewModel)
        {
            if (ModelState.IsValid)
            {
                var bus = _appDbContext.addBuses.FirstOrDefault(o => o.Id == editBusViewModel.Id);
                if (bus == null)
                {
                    return NotFound();
                }
                bus.BusName = editBusViewModel.BusName;
                bus.BusNumber = editBusViewModel.BusNumber;
                bus.Source = editBusViewModel.Source;
                bus.Departuare = editBusViewModel.Departuare;
                bus.DepartuareTime = editBusViewModel.DepartuareTime;
                bus.Destination = editBusViewModel.Destination;
                bus.Arriaval = editBusViewModel.Arriaval;
                bus.ArrivalTime = editBusViewModel.ArrivalTime;
                bus.MaxCapisity = editBusViewModel.MaxCapisity;
                bus.IsClose = editBusViewModel.IsClose;
                bus.UpdationDate = DateTime.Now;
                _appDbContext.SaveChanges();
                return RedirectToAction("AdminViewBus", "Account");
            }
            return View(editBusViewModel);
        }

        [HttpGet]
        [Authorize(Roles = "User,Admin")]
        public IActionResult BusDetails()
        {
            var busDetailsList = _appDbContext.addBuses.Where(o => o.IsClose == false).ToList();
            ViewBag.BusDetailsList = busDetailsList;
            var busDetails = new TicketReservation();
            return View(busDetails);
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult BusDetails(TicketReservation ticketReservation)
        {
            string busNumber = ticketReservation.BusNumber;
            AddBus selectedBus = _appDbContext.addBuses.FirstOrDefault(o => o.BusNumber == busNumber);
            if (selectedBus == null)
            {
                return NotFound();
            }
            ticketReservation.SeatAvability = GenerateAvailability(selectedBus.MaxCapisity, busNumber, ticketReservation.DateTime);
            var busDetailsList = _appDbContext.addBuses.Where(o => o.IsClose == false).ToList();
            ViewBag.BusDetailsList = busDetailsList;
            ticketReservation.SelectedSeat = Request.Form["selectedseat"].Select(int.Parse).ToList();
            ticketReservation.Email = User.Identity.Name;
            return View(ticketReservation);
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult ConfirmReservation(TicketReservation ticketReservation)
        {
            if (ModelState.IsValid)
            {
                string email = User.Identity.Name;
                string busNumber = ticketReservation.BusNumber;
                AddBus addBus = _appDbContext.addBuses.FirstOrDefault(bus => bus.BusNumber == busNumber);
                if (addBus == null)
                {
                    return NotFound();
                }
                List<bool> seatAvailability = GenerateAvailability(addBus.MaxCapisity, busNumber, ticketReservation.DateTime);
                string selectedSeats = string.Join(",", ticketReservation.SelectedSeat); // Retrieve selected seats from ticketReservation object
                var transaction = new Reservation
                {
                    Email = email,
                    BusNumber = busNumber,
                    selectedseat = selectedSeats,
                    DateTime = DateTime.Now,
                    JourneyDate = ticketReservation.JourneyDate
                };
                _appDbContext.reservations.Add(transaction);
                _appDbContext.SaveChanges();
                seatAvailability = UpdateSeatAvailability(seatAvailability, selectedSeats);
                ticketReservation.SeatAvability = seatAvailability;
                return RedirectToAction("ViewTicket", "Account");
            }
            return View(ticketReservation);
        }


        private List<bool> GenerateAvailability(int maxCapacity, string busNumber, DateTime datetime)
        {
            List<bool> seatAvailability = new List<bool>();
            DateTime dateOnly = datetime.Date;

            try
            {
                for (int i = 0; i < maxCapacity; i++)
                {
                    bool isSeatAvailable = true;

                    foreach (var reservation in _appDbContext.reservations)
                    {
                        if (reservation.BusNumber == busNumber &&
                            reservation.JourneyDate.Date == dateOnly.Date)
                        {
                            var selectedSeats = reservation.selectedseat.Split(',');

                            foreach (var seat in selectedSeats)
                            {
                                if (seat == i.ToString())
                                {
                                    isSeatAvailable = false;
                                    break;
                                }
                            }

                            if (!isSeatAvailable)
                                break;
                        }
                    }

                    seatAvailability.Add(isSeatAvailable);
                }
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine($"Error occurred in GenerateAvailability: {ex.Message}");
                // You can also use a logging framework like Serilog or NLog to log the exception.

                // If needed, handle the exception or return a default availability list
                // For simplicity, let's return an empty availability list in case of an error
                return new List<bool>();
            }

            return seatAvailability;
        }

        private List<bool> UpdateSeatAvailability(List<bool> seatAvailability, string selectedSeats)
        {
            List<int> selectedNumbers = selectedSeats.Split(',').Select(int.Parse).ToList();
            for (int i = 0; i < seatAvailability.Count; i++)
            {
                if (selectedNumbers.Contains(i + 1))
                {
                    seatAvailability[i] = false;
                }
            }
            return seatAvailability;
        }

        [HttpGet]
        [Authorize(Roles = "User,Admin")]
        public IActionResult ViewTicket()
        {
            string email = User.Identity.Name;
            var reservationsQuery = _appDbContext.reservations.Where(r => r.Email == email);

            var reservations = reservationsQuery.OrderByDescending(r => r.Id).ToList();
            ViewBag.Reservations = reservations;
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult ViewTicket(int id)
        {
            string email = User.Identity.Name;
            var reservationsQuery = _appDbContext.reservations.Where(r => r.Email == email);
            if (id != 0)
            {
                // Filter reservations based on the search id
                reservationsQuery = reservationsQuery.Where(r => r.Id == id);
            }
            var reservations = reservationsQuery.OrderByDescending(r => r.Id).ToList();
            ViewBag.Reservations = reservations;
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult DeleteTicket(int id)
        {
            var item = _appDbContext.reservations.FirstOrDefault(o => o.Id == id);
            _appDbContext.Remove(item);
            _appDbContext.SaveChanges();
            TempData["RecordDeleted"] = true;
            return RedirectToAction("ViewTicket", "Account");
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult AddImage()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult AddImage(AddImageViewModel addImageViewModel, IFormFile productImage, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            returnUrl = returnUrl ?? Url.Content("~/");
            string filename = "";

            if (productImage == null || productImage.Length == 0)
            {
                // No image uploaded, save the default image
                filename = "available.png";
            }
            else
            {
                filename = AddImages(addImageViewModel, productImage); // Upload the selected image
            }

            var item = new AddImages
            {
                ImageTitle = addImageViewModel.ImageTitle,
                ProductImage = filename,
                ImageDescription = addImageViewModel.ImageDescription,
                CreationDate = DateTime.Now
            };

            _appDbContext.AddImages.Add(item);
            _appDbContext.SaveChanges();

            return RedirectToAction("ViewImage", "Account");
        }

        private string AddImages(AddImageViewModel addImageViewModel, IFormFile productImage)
        {
            string fileName = "";

            if (productImage != null && productImage.Length > 0)
            {
                string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "Image");
                fileName = Guid.NewGuid().ToString() + "-" + productImage.FileName;
                string filePath = Path.Combine(uploadPath, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    productImage.CopyTo(fileStream);
                }
            }

            return fileName;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult EditImage(EditImages editImages)
        {

            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult EditImage(AddImageViewModel addImageViewModel,IFormFile ProductImage)
        {
            if (ModelState.IsValid)
            {
                var myprod = _appDbContext.AddImages.FirstOrDefault(o => o.Id == addImageViewModel.Id);
                if (myprod == null)
                {
                    return RedirectToAction("ViewImage", "Account");
                }
                bool items = false;
                if(addImageViewModel.IsClose)
                {
                    items = true;
                }
                myprod.ImageTitle = addImageViewModel.ImageTitle;
                myprod.ImageDescription = addImageViewModel.ImageDescription;
                myprod.UpdatetionDate = System.DateTime.Now;
                myprod.IsClose = items;
                if (ProductImage != null && ProductImage.Length>0)
                {
                    string filepath = AddImages(addImageViewModel, ProductImage);
                    myprod.ProductImage = filepath;
                }
                _appDbContext.SaveChanges();
                return RedirectToAction("ViewImage", "Account");
            }
            return View(addImageViewModel);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult ViewImage()
        {
            var item = _appDbContext.AddImages.ToList();
            return View(item);
        }
        [HttpGet]
        public IActionResult UserImage()
        {
            var item = _appDbContext.AddImages.Where(i=>i.IsClose==false).ToList();
            return View(item);
        }
    }
}
