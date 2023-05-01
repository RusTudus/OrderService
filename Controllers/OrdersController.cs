using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OrderService.Models;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : Controller
    {
        private readonly ApiDbContext _context;

        public OrdersController(ApiDbContext context)
        {
            _context = context;
        }

        // GET: Orders/<guid>
        [HttpGet]
        public async Task<Order?> Get(Guid? id)
        {
            if (id == null || _context.Orders == null)
                return null;

            var order = await _context.Orders.Include(i => i.Lines)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
                return null;

            return order;
        }

        // POST: Orders/Create
        [HttpPost]
        public async Task<ObjectResult> Create(Order order)
        {
            if (ModelState.IsValid)
            {
                if (order.Lines.Count == 0)
                    return BadRequest("Lines are not exists");
                if (QtyCheck(order))
                    return BadRequest("Qty in lines is zero or below zero");
                order.Status = StatusEnum.New;
                _context.Add(order);
                await _context.SaveChangesAsync();
            }
            return Ok(order);
        }        

        // POST: Orders/
        [HttpPut]
        public async Task<ObjectResult> Edit(Guid id, [Bind("Status,Lines")] Order order)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Order? _order = await _context.Orders.FindAsync(id);
                    if (_order.Status == StatusEnum.New || _order.Status == StatusEnum.WaitForPay)
                    {
                        _order.Lines = order.Lines;
                        _order.Status = order.Status;

                        if (order.Lines.Count == 0)
                            return BadRequest("Lines are not exists");
                        if (QtyCheck(_order))
                            return BadRequest("Qty in lines is zero or below zero");
                        _context.Update(_order);
                        await _context.SaveChangesAsync();
                    }
                    else return BadRequest("Order isn't in status New or Wait for payment. You can't edit it.");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
                        return null;
                    else                   
                        throw;
                }                
            }
            return Ok(order);
        }

        // POST: Orders/<guid>
        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (_context.Orders == null)
                return Problem("Entity set 'ApiDbContext.Orders'  is null.");
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                if (order.Status == StatusEnum.WaitForDelivery || order.Status == StatusEnum.Delivered || order.Status == StatusEnum.Completed)
                    return BadRequest("You can't delete order in this status");
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
                return Ok();
            }          
            return BadRequest("Order not exists");
        }

        private bool OrderExists(Guid id)
        {
          return (_context.Orders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        /// <summary>
        /// Check if qty in lines is >= 0
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        private bool QtyCheck(Order order)
        {
            return order.Lines.Where(i => i.Qty <= 0).Count() == 0 ? true : false;
        }
    }
}
