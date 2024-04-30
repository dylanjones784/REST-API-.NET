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
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly BookSystemContext _context;

        public MembersController(BookSystemContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Member>>> GetMembers()
        {
            List<Member> lMember = await _context.Members.ToListAsync();
            foreach (Member member in lMember)
            {
                //foreach member add the hypermedia links related to the resource
                member.Links = new List<Link>
                {
                    new Link($"/api/Member/{member.Id}", "self", "GET",0),
                    new Link($"/api/Member/{member.Id}", "self", "PUT",1),
                    new Link($"/api/Member/{member.Id}", "self", "DELETE",0)
                };

            }
            return lMember;
        }

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

        [HttpPut("{id}")]
        [Consumes("application/json")]
        public async Task<IActionResult> PutMember(int id, Member member)
        {
            if (id != member.Id)
            {
                return BadRequest();
            }

            var existingMember = await _context.Members.FindAsync(id);
            if (existingMember == null)
            {
                return NotFound();
            }

            member.Name = SanitiseInput(member.Name);
            member.Address = SanitiseInput(member.Address);

            //further checks could be done on the datetime properly, i.e converting it with .Try

            _context.Entry(existingMember).CurrentValues.SetValues(member);

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

        [Authorize]
        [HttpPost]
        [Consumes("application/json")]
        public async Task<ActionResult<Member>> PostMember(Member member)
        {
            if (member != null)
            {

                if (member.DateRegistered.GetType() != typeof(DateTime))
                {
                    return BadRequest("Date Registered is not a valid value");

                }
                //check the inputs
                Member newMember = new Member
                {
                    Name = SanitiseInput(member.Name),
                    Address = SanitiseInput(member.Address),
                    DateRegistered = member.DateRegistered, 
                };

                //we've checked the data, now we can add it to the database
                _context.Members.Add(newMember);
                await _context.SaveChangesAsync();

                //add links to member object after saved for return
                newMember.Links = new List<Link>
                {
                    new Link($"/api/Members/{member.Id}", "self", "GET",0),
                    new Link($"/api/Members/{member.Id}", "self", "PUT",1),
                    new Link($"/api/Members/{member.Id}", "self", "DELETE",1)
                };

                return CreatedAtAction("GetMember", new { id = newMember.Id }, newMember);
            }

            return BadRequest();

        }



        public static string SanitiseInput(string i)
        {
            //Sanitising our Inputs - We put the string "i" into this function, and we have a set of accepted characters
            Regex r = new Regex("[^a-zA-Z0-9 ]");
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
