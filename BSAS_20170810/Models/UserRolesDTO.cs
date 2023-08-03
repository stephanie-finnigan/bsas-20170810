using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BSAS_20170810.Models
{
    public class ExpandedUserDTO
    {
        [Key]
        [Display (Name = "Username")]
        public string userName { get; set; }
        [Display (Name = "First Name")]
        public string firstName { get; set; }
        [Display (Name = "Surname")]
        public string surname { get; set; }
        [Display (Name = "Rank")]
        public string rank { get; set; }
        [Display (Name = "Email")]
        public string email { get; set; }
        public int stationId { get; set; }
        public virtual Station station { get; set; }
        
        [Display (Name = "Password")]
        public string password { get; set; }
        [Display (Name = "Lockout Until")]
        public DateTime? lockoutEndDateUtc { get; set; }
        public int accessFailedCount { get; set; }
        public IEnumerable<UserRolesDTO> roles { get; set; }
    }

    public class UserRolesDTO
    {
        [Key]
        [Display(Name = "Role Name")]
        public string roleName { get; set; }
    }

    public class UserRoleDTO
    {
        [Key]
        [Display(Name = "User Name")]
        public string userName { get; set; }

        [Display(Name = "Role Name")]
        public string roleName { get; set; }
    }

    public class RoleDTO
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Role Name")]
        public string roleName { get; set; }
    }

    public class UserAndRolesDTO
    {
        [Key]
        [Display(Name = "User Name")]
        public string userName { get; set; }
        public List<UserRoleDTO> userRoles { get; set; }
    }
}