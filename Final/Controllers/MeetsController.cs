using AutoMapper;
using Final.Data;
using Final.DTOs;
using Final.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
namespace Final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetController : ControllerBase
    {
        private readonly AppDbContext appDbContext;
        private readonly IMapper mapper;

        public MeetController(AppDbContext appDbContext, IMapper mapper)
        {
            this.appDbContext = appDbContext;
            this.mapper = mapper;
        }


        [HttpGet]
        public async Task<ActionResult<List<ResponseDTO>>> GetMeets()
        {

            var meets = await appDbContext.meets.ToListAsync();
            return Ok(meets.Select(mapper.Map<ResponseDTO>));

        }

        [HttpPost]
        public async Task<ActionResult<Meet>> PostMeet(Meet meet)
        {
            if (appDbContext.Meets == null)
            {
                return Problem("Entity set 'ApiDbContext.Meets'  is null.");
            }
            appDbContext.Meets.Add(meet);
            await appDbContext.SaveChangesAsync();

            return CreatedAtAction("GetMeet", new { id = meet.Id }, meet);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDTO>> GetMeet(int id)
        {
            var meet = await appDbContext.meets.FindAsync(id);

            if (meet == null)
            {
                return NotFound();
            }

            var meetDTO = mapper.Map<ResponseDTO>(meet);

            return Ok(meetDTO);
        }
       
        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseDTO>> PutMeet(int id, RequestDTO updatedMeet)
        {
            if (id != updatedMeet.Id)
            {
                return BadRequest();
            }

            var existingMeet = await appDbContext.meets.FindAsync(id);

            if (existingMeet == null)
            {
                return NotFound();
            }

            // Маппинг обновленных данных из RequestDTO в существующий объект Meet
            mapper.Map(updatedMeet, existingMeet);

            appDbContext.Entry(existingMeet).State = EntityState.Modified;

            try
            {
                await appDbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MeetExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // После обновления возвращаем обновленную встречу в виде ResponseDTO
            var updatedMeetDTO = mapper.Map<ResponseDTO>(existingMeet);
            return Ok(updatedMeetDTO);
        }
      

        private bool MeetExists(int id)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Meet>> DeleteMeet(int id)
        {
            if (appDbContext.meets == null)
            {
                return NotFound();
            }

            var meet = await appDbContext.meets.FindAsync(id);
            if (meet == null)
            {
                return NotFound();
            }

            var meetDTO = mapper.Map<Meet>(meet);

            appDbContext.meets.Remove(meet);
            await appDbContext.SaveChangesAsync();

            return meetDTO;
        }
    }
}
