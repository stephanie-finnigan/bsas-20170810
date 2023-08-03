using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using BSAS_20170810.Models;
using PagedList;
using System.Collections;
using System.Data;
using System.Data.Entity;

namespace BSAS_20170810.Controllers
{
    public class AdminController : Controller
    {
        private RoleManager _roleManager;
        private UserManager _userManager;

        private BSASDbContext db = new BSASDbContext();

        // Controllers

        // GET: Admin
        //[Authorize (Roles = "Administrator")]
        public ActionResult Index(string searchStringUserNameorEmail, string currentFilter, int? page)
        {
            try
            {
                int Page = 1;
                int PageSize = 5;
                int TotalPageCount = 0;
                if (searchStringUserNameorEmail != null)
                {
                    Page = 1;
                }
                else
                {
                    if (currentFilter != null)
                    {
                        searchStringUserNameorEmail = currentFilter;
                        Page = page ?? 1;
                    }
                    else
                    {
                        searchStringUserNameorEmail = "";
                        Page = page ?? 1;
                    }
                }
                ViewBag.CurrentFilter = searchStringUserNameorEmail;
                List<ExpandedUserDTO> UserRoles = new List<ExpandedUserDTO>();
                int Skip = (Page - 1) * PageSize;
                TotalPageCount = UserManager.Users
                    .Where(x => x.UserName.Contains(searchStringUserNameorEmail))
                    .Count();
                var result = UserManager.Users
                    .Include(u => u.Station)
                    .Where(x => x.UserName.Contains(searchStringUserNameorEmail))
                    .OrderBy(x => x.Id)
                    .Skip(Skip)
                    .Take(PageSize)
                    .ToList();
                foreach (var item in result)
                {
                    ExpandedUserDTO objUser = new ExpandedUserDTO();
                    objUser.userName = item.UserName;
                    objUser.surname = item.Surname;
                    objUser.rank = item.Rank;
                    objUser.email = item.Email;
                    objUser.stationId = item.StationId;
                    objUser.lockoutEndDateUtc = item.LockoutEndDateUtc;
                    UserRoles.Add(objUser);
                }
                // Set the number of pages
                var _UserAsIPagedList = new StaticPagedList<ExpandedUserDTO>(
                    UserRoles, Page, PageSize, TotalPageCount);

                return View(_UserAsIPagedList);
            }

            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);
                List<ExpandedUserDTO> UserRoles = new List<ExpandedUserDTO>();
                return View(UserRoles.ToPagedList(1, 25));
            }
        }

        // USERS ******************************

        // GET: /Admin/Edit/Create
        //[Authorize(Roles = "Administrator")]
        public ActionResult Create()
        {
            ExpandedUserDTO ExpandedUser = new ExpandedUserDTO();
            ViewBag.Roles = GetAllRolesAsSelectList();
            ViewBag.StationId = new SelectList(db.Stations, "stationId", "stationName");
            return View(ExpandedUser);
        }

        // PUT: /Admin/Create
        //[Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ExpandedUserDTO ExpandedUser)
        {
            try
            {
                if (ExpandedUser == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var UserName = ExpandedUser.userName.Trim();
                var FirstName = ExpandedUser.firstName.Trim();
                var Surname = ExpandedUser.surname.Trim();
                var Rank = ExpandedUser.rank.Trim();
                var StationId = ExpandedUser.stationId;
                var Email = ExpandedUser.email.Trim();
                var Password = ExpandedUser.password.Trim();

                var objNewAdminUser = new User
                {
                    UserName = UserName,
                    Email = Email,
                    FirstName = FirstName,
                    Surname = Surname,
                    Rank = Rank,
                    StationId = StationId
                };
                var AdminUserCreateResult = UserManager.Create(objNewAdminUser, Password);
                if (AdminUserCreateResult.Succeeded == true)
                {
                    string strNewRole = Convert.ToString(Request.Form["Roles"]);
                    if (strNewRole != "0")
                    {
                        // Put user in role
                        UserManager.AddToRole(objNewAdminUser.Id, strNewRole);
                    }
                    return Redirect("~/Admin");
                }
                else
                {
                    ViewBag.Roles = GetAllRolesAsSelectList();
                    ModelState.AddModelError(string.Empty, "Error: Failed to create the user. Check requirements.");
                    return View(ExpandedUser);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Roles = GetAllRolesAsSelectList();
                ModelState.AddModelError(string.Empty, "Error: " + ex);
                return View("Create");
            }
        }

        // GET: /Admin/EditUser
        //[Authorize(Roles = "Administrator")]
        public ActionResult EditUser(string UserName)
        {
            if (UserName == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExpandedUserDTO ExpandedUser = GetUser(UserName);
            if (ExpandedUser == null)
            {
                return HttpNotFound();
            }
            ViewBag.StationId = new SelectList(db.Stations, "stationId", "stationName", ExpandedUser.stationId);
            return View(ExpandedUser);
        }

        //PUT: /Admin/EditUser
        //[Authorize (Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(ExpandedUserDTO expandedUser)
        {
            try
            {
                if (expandedUser == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ExpandedUserDTO objExpUser = UpdateUser(expandedUser);

                if (objExpUser == null)
                {
                    return HttpNotFound();
                }
                return Redirect("~/Admin");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);
                ViewBag.StationId = new SelectList(db.Stations, "StationId", "StationName", expandedUser.stationId);
                return View("EditUser", GetUser(expandedUser.userName));
            }
        }

        // GET: /Admin/EditRoles
        //[Authorize(Roles = "Administrator")]
        public ActionResult EditRoles(string UserName)
        {
            if (UserName == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            UserName = UserName.ToLower();

            // Check that we have an actual user
            ExpandedUserDTO objExpandedUser = GetUser(UserName);

            if (objExpandedUser == null)
            {
                return HttpNotFound();
            }

            UserAndRolesDTO objUserAndRoles = GetUserAndRoles(UserName);

            return View(objUserAndRoles);
        }

        // PUT: /Admin/EditRoles
        //[Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditRoles(UserAndRolesDTO paramUserAndRoles)
        {
            try
            {
                if (paramUserAndRoles == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                string UserName = paramUserAndRoles.userName;
                string strNewRole = Convert.ToString(Request.Form["AddRole"]);

                if (strNewRole != "No Roles Found")
                {
                    // Go get the User
                    User user = UserManager.FindByName(UserName);

                    // Put user in role
                    UserManager.AddToRole(user.Id, strNewRole);
                }

                ViewBag.AddRole = new SelectList(RolesUserIsNotIn(UserName));

                UserAndRolesDTO objUserAndRoles = GetUserAndRoles(UserName);

                return View(objUserAndRoles);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);
                return View("EditRoles");
            }
        }

        // DELETE: /Admin/DeleteUser
        //[Authorize(Roles = "Administrator")]
        public ActionResult DeleteUser(string UserName)
        {
            try
            {
                if (UserName == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                if (UserName.ToLower() == User.Identity.Name.ToLower())
                {
                    ModelState.AddModelError(string.Empty, "Error: Cannot delete the current user.");
                    return View("EditUser");
                }

                ExpandedUserDTO objExpandedUser = GetUser(UserName);
                if (objExpandedUser == null)
                {
                    return HttpNotFound();
                }
                return Redirect("~/Admin");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);
                return View("EditUser", GetUser(UserName));
            }
        }

        // DELETE: /Admin/DeleteRole
        [Authorize(Roles = "Administrator")]
        public ActionResult DeleteRole(string UserName, string RoleName)
        {
            try
            {
                if ((UserName == null) || (RoleName == null))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                UserName = UserName.ToLower();

                // Check that we have an actual user
                ExpandedUserDTO objExpandedUser = GetUser(UserName);

                if (objExpandedUser == null)
                {
                    return HttpNotFound();
                }

                if (UserName.ToLower() == this.User.Identity.Name.ToLower() && RoleName == "Administrator")
                {
                    ModelState.AddModelError(string.Empty, "Error: Cannot delete Administrator Role for the current user");
                }

                // Go get the User
                User user = UserManager.FindByName(UserName);
                // Remove User from role
                UserManager.RemoveFromRoles(user.Id, RoleName);
                UserManager.Update(user);

                ViewBag.AddRole = new SelectList(RolesUserIsNotIn(UserName));

                return RedirectToAction("EditRoles", new { UserName = UserName });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);

                ViewBag.AddRole = new SelectList(RolesUserIsNotIn(UserName));

                UserAndRolesDTO objUserAndRoles = GetUserAndRoles(UserName);

                return View("EditRoles", objUserAndRoles);
            }
        }

        // ROLES *************************

        // GET: /Admin/ViewAllRoles
        [Authorize(Roles = "Administrator")]
        public ActionResult ViewAllRoles()
        {
            var roleManager = new RoleManager(new RoleStore(new BSASDbContext()));
            List<RoleDTO> UserRoles = (from objRole in roleManager.Roles
                               select new RoleDTO
                               {
                                   Id = objRole.Id,
                                   roleName = objRole.Name
                               }).ToList();
            return View(UserRoles);
        }

        // GET: /Admin/AddRole
        [Authorize(Roles = "Administrator")]
        public ActionResult AddRole()
        {
            RoleDTO objRole = new RoleDTO();
            return View(objRole);
        }
        
        // PUT: /Admin/AddRole
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddRole(RoleDTO paramRole)
        {
            try
            {
                if (paramRole == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var RoleName = paramRole.roleName.Trim();
                if (RoleName == "")
                {
                    throw new Exception("No RoleName");
                }
                // Create Role
                var roleManager = new RoleManager(new RoleStore(new BSASDbContext()));
                if (!roleManager.RoleExists(RoleName))
                {
                    roleManager.Create(new Role(RoleName));
                }
                return Redirect("~/Admin/ViewAllRoles");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);
                return View("AddRole");
            }
        }

        // DELETE: /Admin/Delete
        [Authorize(Roles = "Administrator")]
        public ActionResult DeleteUserRole(string RoleName)
        {
            try
            {
                if (RoleName == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                if (RoleName.ToLower() == "administrator")
                {
                    throw new Exception(String.Format("Cannoth delete {0} Role.", RoleName));
                }
                var roleManager = new RoleManager(new RoleStore(new BSASDbContext()));

                var UsersInRole = roleManager.FindByName(RoleName).Users.Count();

                if (UsersInRole > 0)
                {
                    throw new Exception(String.Format("Cannot delete {0} Role because it still has users.", RoleName));
                }
                var objRoleToDelete = (from objRole in roleManager.Roles
                                       where objRole.Name == RoleName
                                       select objRole).FirstOrDefault();
                if (objRoleToDelete != null)
                {
                    roleManager.Delete(objRoleToDelete);
                }
                else
                {
                    throw new Exception(String.Format("Cannot delete {0} Role does not exist.", RoleName));
                }

                List<RoleDTO> Roles = (from objRole in roleManager.Roles
                                       select new RoleDTO
                                       {
                                           Id = objRole.Id,
                                           roleName = objRole.Name
                                       }).ToList();
                return View("ViewAllRoles", Roles);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);
                var roleManager = new RoleManager(new RoleStore(new BSASDbContext()));
                List<RoleDTO> Roles = (from objRole in roleManager.Roles
                                       select new RoleDTO
                                       {
                                           Id = objRole.Id,
                                           roleName = objRole.Name
                                       }).ToList();
                return View("ViewAllRoles", Roles);
            }
        }

        // UTILITY *****************************************************************************
        // Add User Manager
        public UserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<UserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // Add Role Manager
        public RoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().GetUserManager<RoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }
        
        // List of Roles
        private List<SelectListItem> GetAllRolesAsSelectList()
        {
            List<SelectListItem> SelectRoleListItems = new List<SelectListItem>();

            var roleManager = new RoleManager(new RoleStore(new BSASDbContext()));
            var RoleSelectList = roleManager.Roles.OrderBy(x => x.Id).ToList();

            SelectRoleListItems.Add(
                new SelectListItem
                {
                    Text = "Select",
                    Value = "0"
                });
            foreach (var item in RoleSelectList)
            {
                SelectRoleListItems.Add(
                    new SelectListItem
                    {
                        Text = item.Name.ToString(),
                        Value = item.Name.ToString()
                    });
            }
            return SelectRoleListItems;
        }

        // Get User
        private ExpandedUserDTO GetUser(string UserName)
        {
            ExpandedUserDTO objExpUser = new ExpandedUserDTO();

            var result = UserManager.FindByName(UserName);

            if (result == null) throw new Exception("Could not find the user.");

            objExpUser.userName = result.UserName;
            objExpUser.firstName = result.FirstName;
            objExpUser.surname = result.Surname;
            objExpUser.rank = result.Rank;
            objExpUser.stationId = result.StationId;
            objExpUser.email = result.Email;
            objExpUser.lockoutEndDateUtc = result.LockoutEndDateUtc;
            objExpUser.accessFailedCount = result.AccessFailedCount;

            return objExpUser;
        }

        // Update User
        private ExpandedUserDTO UpdateUser(ExpandedUserDTO expandedUser)
        {
            User result = UserManager.FindByName(expandedUser.userName);
            // If can't find the user, throw exception
            if (result == null)
            {
                throw new Exception("Could not find the user.");
            }
            result.UserName = expandedUser.userName;
            // Check if account needs to be unlocked
            if (UserManager.IsLockedOut(result.Id))
            {
                // Unlock user
                UserManager.ResetAccessFailedCountAsync(result.Id);
            }
            UserManager.Update(result);
            // Was a password sent
            if (!string.IsNullOrEmpty(expandedUser.password))
            {
                var removePassword = UserManager.RemovePassword(result.Id);
                if (removePassword.Succeeded)
                {
                    var AddPassword = UserManager.AddPassword(result.Id, expandedUser.password);
                    if (AddPassword.Errors.Count() > 0)
                    {
                        throw new Exception(AddPassword.Errors.FirstOrDefault());
                    }
                }
            }
            return expandedUser;
        }

        // Delete User
        private void DeleteUser(ExpandedUserDTO paramExpandedUser)
        {
            User user = UserManager.FindByName(paramExpandedUser.userName);
            // If couldn't find user, throw exception
            if (user == null)
            {
                throw new Exception("Could not find the user.");
            }
            UserManager.RemoveFromRoles(user.Id, UserManager.GetRoles(user.Id).ToArray());
            UserManager.Update(user);
            UserManager.Delete(user);
        }

        // User and Role
        private UserAndRolesDTO GetUserAndRoles(string UserName)
        {
            // Go get the User
            User user = UserManager.FindByName(UserName);

            List<UserRoleDTO> UserRoles =
                (from objRole in UserManager.GetRoles(user.Id)
                 select new UserRoleDTO
                 {
                     roleName = objRole,
                     userName = UserName
                 }).ToList();

            if (UserRoles.Count() == 0)
            {
                UserRoles.Add(new UserRoleDTO { roleName = "No Roles Found" });
            }

            ViewBag.AddRole = new SelectList(RolesUserIsNotIn(UserName));

            // Create UserRolesAndPermissionsDTO
            UserAndRolesDTO objUserAndRolesDTO =
                new UserAndRolesDTO();
            objUserAndRolesDTO.userName = UserName;
            objUserAndRolesDTO.userRoles = UserRoles;
            return objUserAndRolesDTO;
        }

        // List of roles User Is Not In
        private List<string> RolesUserIsNotIn(string UserName)
        {
            // Get roles the user is not in
            var AllRoles = RoleManager.Roles.Select(x => x.Name).ToList();

            // Go get the roles for an individual
            User user = UserManager.FindByName(UserName);

            // If we could not find the user, throw an exception
            if (user == null)
            {
                throw new Exception("Could not find the User");
            }

            var RolesForUser = UserManager.GetRoles(user.Id).ToList();
            var RolesUserInNotIn = (from objRole in AllRoles
                                       where !RolesForUser.Contains(objRole)
                                       select objRole).ToList();

            if (RolesUserInNotIn.Count() == 0)
            {
                RolesUserInNotIn.Add("No Roles Found");
            }

            return RolesUserInNotIn;
        }

    }
}