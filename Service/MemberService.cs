using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sp_project_guide_api.Models;
using System.Net;
using System.Text.RegularExpressions;

namespace sp_project_guide_api.Service
{
    public class MemberService : IMemberService
    {
        private readonly BookSystemContext _context;

        public MemberService(BookSystemContext context)
        {
            _context = context;
        }


        public async Task<Member> GetMember(int id)
        {
            var member = await _context.Members.FindAsync(id);

            if (member == null)
            {
                //return member back even if null, we handle it in Service layer
                return member;
            }
            //add hypermedia links
            member.Links = new List<Link>{
                    new Link($"/api/Members/{member.Id}", "self", "PUT",1),
                    new Link($"/api/Members/{member.Id}", "self", "DELETE", 0),
            };

            return member;
        }

        public async Task<List<Member>> GetMembers()
        {
            var lMember = await _context.Members.ToListAsync();
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

        public async Task<Member> PostMember(Member member) 
        {
            //check if there are any members already existing with the ID
            if (_context.Members.Any(b => b.Id == member.Id))
            {
                //throw the Key exception for when ID keys cant be found.
                throw new KeyNotFoundException($"Member already exists with ID of {member.Id}");
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
            return newMember;
        }

        public async Task UpdateMember(int id, Member member)
        {

            var existingMember = await _context.Members.FindAsync(id);
            if (existingMember == null)
            {
                throw new KeyNotFoundException($"Member ID does not exist with ID of{id}");
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
                    throw new KeyNotFoundException($"Member ID does not exist with ID of {id}");
                }
                else
                {
                    throw;
                }
            }
        }


        public async Task DeleteMember(int id)
        {
            //attempt to find the member
            var member = await _context.Members.FindAsync(id);
            if (member != null)
            {
                //since we found it, we can remove it
                _context.Members.Remove(member);
                await _context.SaveChangesAsync();
            }
            else
            {
                //throw exception to tell user ID doesnt exist.
                throw new KeyNotFoundException($"No member found with ID {id}");

            }
        }

        public static string SanitiseInput(string i)
        {
            Regex r = new Regex("[^a-zA-Z0-9 ]");
            return r.Replace(i, "");
        }
        private bool MemberExists(int id)
        {
            return _context.Members.Any(e => e.Id == id);
        }
    }

}
