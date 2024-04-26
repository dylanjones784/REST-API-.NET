using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sp_project_guide_api.Models;

namespace sp_project_guide_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly BookSystemContext _context;

        public MembersController(BookSystemContext context)
        {
            _context = context;
        }

        // GET: api/Members
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Member>>> GetMembers([FromQuery] int? page, [FromQuery] int? pageSize, [FromQuery] string? sort, [FromQuery] string? fields)
        {
            return await _context.Members.ToListAsync();
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Member>> GetMember(int id)
        {
            var member = await _context.Members.FindAsync(id);

            if (member == null)
            {
                return NotFound();
            }

            member.Links = new List<Link>{
                    new Link($"/api/Members/{member.Id}", "self", "PUT",1),
                    new Link($"/api/Members/{member.Id}", "self", "DELETE", 0),
            };

            return member;
        }
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMember(int id, Member member)
        {
            if (id != member.Id)
            {
                return BadRequest();
            }

            _context.Entry(member).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MemberExists(id))
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

        // POST: api/Members
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Member>> PostMember(Member member)
        {
            if (member != null)
            {
                //check the inputs
                Member newMember = new Member();
                if (member.DateRegistered.GetType() == typeof(DateTime))
                {
                    newMember.DateRegistered = member.DateRegistered;
                }

                newMember.Name = SanitiseInput(member.Name);
                newMember.Address = SanitiseInput(member.Address);
                //we've checked the data, now we can add it to the database

                //add links to book object after saved for return
                member.Links = new List<Link>
                {
                    new Link($"/api/Members/{member.Id}", "self", "GET",0),
                    new Link($"/api/Members/{member.Id}", "self", "PUT",1),
                    new Link($"/api/Members/{member.Id}", "self", "DELETE",1)
                };


                _context.Members.Add(member);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetMember", new { id = member.Id }, member);
            }

            return BadRequest();

        }



        public static string SanitiseInput(string i)
        {
            //Sanitising our Inputs - We put the string "i" into this function, and we have a set of accepted characters
            Regex r = new Regex("[^a-zA-Z0-9]");
            // Replace any characters not matching the pattern with an empty string
            return r.Replace(i, "");
        }
        // DELETE: api/Members/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMember(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }

            _context.Members.Remove(member);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MemberExists(int id)
        {
            return _context.Members.Any(e => e.Id == id);
        }
    }
}
