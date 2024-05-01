using Microsoft.AspNetCore.Mvc;
using sp_project_guide_api.Models;

namespace sp_project_guide_api.Service
{
    public interface IMemberService
    {
        Task<Member> GetMember(int id);
        Task<List<Member>> GetMembers();
        Task<Member> PostMember(Member member);
        Task UpdateMember(int id, Member member);
        Task DeleteMember(int id);

    }
}
