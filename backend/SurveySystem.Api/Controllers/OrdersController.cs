using Microsoft.AspNetCore.Mvc;
using SurveySystem.Application.DTOs;
using SurveySystem.Application.Interfaces;

namespace SurveySystem.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersController(IOrderRepository orderRepository) : ControllerBase
    {
        private readonly IOrderRepository _orderRepository = orderRepository;

        /// <summary>
        /// POST /api/orders/import
        /// Завантаження CSV із замовленнями. Система обробляє, рахує податки та зберігає.
        /// </summary>
        [HttpPost("import")]
        public async Task<IActionResult> ImportCsv(IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { Error = "Файл не обрано або він порожній." });
            }

            if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { Error = "Допускаються лише файли формату CSV." });
            }

            using var stream = file.OpenReadStream();
            await _orderRepository.ImportFromCsvAsync(stream, cancellationToken);

            return Ok(new { Message = "CSV файл успішно оброблено. Замовлення збережені з розрахованими податками." });
        }

        /// <summary>
        /// POST /api/orders
        /// Створити замовлення вручну (lat, lon, subtotal) -> одразу порахувати й зберегти.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
        {
            if (request.Subtotal < 0)
            {
                return BadRequest(new { Error = "Сума замовлення (subtotal) не може бути від'ємною." });
            }

            var responseDto = await _orderRepository.CreateOrderAsync(
                request.Latitude,
                request.Longitude,
                request.Subtotal,
                cancellationToken);

            return CreatedAtAction(nameof(GetOrders), new { id = responseDto.Id }, responseDto);
        }

        /// <summary>
        /// GET /api/orders
        /// Таблиця замовлень з розрахованими податками + пагінація.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetOrders(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            CancellationToken cancellationToken = default)
        {
            if (page < 1 || pageSize < 1)
            {
                return BadRequest(new { Error = "Параметри пагінації (page та pageSize) повинні бути більшими за 0." });
            }

            var (items, totalCount) = await _orderRepository.GetOrdersAsync(page, pageSize, cancellationToken);

            var response = new
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                Items = items
            };

            return Ok(response);
        }
    }
}
