using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoreApi5.Models;

namespace CoreApi5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private readonly DeviceDbContext _context;
        private readonly IWebHostEnvironment _env;

        public DevicesController(DeviceDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: api/Devices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Device>>> GetDevices()
        {
            return await _context.Devices.ToListAsync();
        }
        // GET: api/Devices/Specs/Include
        [HttpGet("Specs/Include")]
        public async Task<ActionResult<IEnumerable<Device>>> GetDevicesWithSpecs()
        {
            return await _context.Devices.Include(x => x.Specs).ToListAsync();
        }

        // GET: api/Devices/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Device>> GetDevice(int id)
        {
            var device = await _context.Devices.FindAsync(id);

            if (device == null)
            {
                return NotFound();
            }

            return device;
        }
        // GET: api/Devices/Specs/Include/5
        [HttpGet("Specs/Include/{id}")]
        public async Task<ActionResult<Device>> GetDeviceWithSpecs(int id)
        {
            var device = await _context.Devices.Include(x=>x.Specs).FirstOrDefaultAsync(x=>x.DeviceId==id);

            if (device == null)
            {
                return NotFound();
            }

            return device;
        }

        // PUT: api/Devices/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDevice(int id, Device device)
        {
            if (id != device.DeviceId)
            {
                return BadRequest();
            }

            var d = await _context.Devices.Include(x=>x.Specs).FirstOrDefaultAsync(x=>x.DeviceId==id);
            if (d == null) { return  NotFound(); }  
            d.DeviceName = device.DeviceName;
            d.DeviceType = device.DeviceType;
            d.ReleaseDate = device.ReleaseDate;
            d.Price = device.Price;
            d.Picture = device.Picture;
            d.InStock = device.InStock;
            await _context.Database.ExecuteSqlInterpolatedAsync($"Delete From Specs Where DeviceId={id}");
            foreach(var s in device.Specs)
            {
                _context.Specs.Add(new Spec { DeviceId=s.DeviceId, SpecName=s.SpecName, SpecValue=s.SpecValue});
            }
                
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeviceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Devices
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Device>> PostDevice(Device device)
        {
            _context.Devices.Add(device);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDevice", new { id = device.DeviceId }, device);
        }

        // POST: api/Devices/Picture/Upload
        [HttpPost("Picture/Upload")]
        public async Task<ActionResult<string>> Upload(IFormFile pic)
        {
            string ext = Path.GetExtension(pic.FileName);
            string f = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ext;
            string savePath = Path.Combine(_env.WebRootPath, "Pictures" ,f);
            FileStream fs = new FileStream(savePath, FileMode.Create);  
            await pic.CopyToAsync(fs);
            fs.Close();
            return f;
        }

        // DELETE: api/Devices/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDevice(int id)
        {
            var device = await _context.Devices.FindAsync(id);
            if (device == null)
            {
                return NotFound();
            }

            _context.Devices.Remove(device);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DeviceExists(int id)
        {
            return _context.Devices.Any(e => e.DeviceId == id);
        }
    }
}
