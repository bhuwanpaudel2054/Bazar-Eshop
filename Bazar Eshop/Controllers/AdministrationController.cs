using Bazar_Eshop.Models;
using Bazar_Eshop.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Bazar_Eshop.Controllers
{
    [Authorize(Roles = ("Admin ,Super Admin ,Moderator"))]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IWebHostEnvironment hostEnvironment;
        private readonly AppDbContext context;

        public AdministrationController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager,IWebHostEnvironment hostEnvironment, AppDbContext context)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.hostEnvironment = hostEnvironment;
            this.context = context;
        }
        public IActionResult Index()
        {
            return View();
        }






        [HttpGet]
        [Authorize(Policy = "NonAdminCreateClaimPolicy")]
        public IActionResult CreateRole()
        {
            return View();

        }
        [HttpPost]
        [Authorize(Policy = "NonAdminCreateClaimPolicy")]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName

                };
                IdentityResult result = await roleManager.CreateAsync(identityRole);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }






        public IActionResult ListRoles()
        {
            var roles = roleManager.Roles;
            return View(roles);
        }


        public IActionResult ListUsers()
        {
            var users = userManager.Users;
            return View(users);
        }











        [HttpGet]
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> ManageRolesForUser(string userId)
        {
            ViewBag.userId = userId;
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id ={userId} cannot found ";
                return View("NotFound");
            }
            var model = new List<ManageRolesForUserViewModel>();

            foreach (var role in roleManager.Roles)
            {
                var manageRolesForUser = new ManageRolesForUserViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    manageRolesForUser.IsSelected = true;
                }
                else
                {
                    manageRolesForUser.IsSelected = false;
                }
                model.Add(manageRolesForUser);

            }
            return View(model);
        }
        [HttpPost]
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> ManageRolesForUser(List<ManageRolesForUserViewModel> model, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot  be found";
                return View("NotFound");
            }
            var roles = await userManager.GetRolesAsync(user);
            var result = await userManager.RemoveFromRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "cannot add select roles to user");
                return View(model);
            }
            IList<string> a = model.Where(x => x.IsSelected).Select(y => y.RoleName).ToList();
            for (int i = 0; i < a.Count; i++)
            {
                result = await userManager.AddToRoleAsync(user, a[i]);

            }
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "cannot add select roles to user");
                return View(model);
            }
            return RedirectToAction("ListUsers");
        }
        [HttpPost]
        [Authorize(Policy = "NonAdminDeleteClaimPolicy")]
        public async Task<IActionResult> DeleteRole(String id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role  with Id = {id} cannot found";
                return View("NotFound");
            }
            else
            {
                try
                {
                    var result = await roleManager.DeleteAsync(role);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListRoles");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View("ListRoles");
                }
                catch (DbUpdateException ex)
                {
                    ViewBag.Errortitle = $"{role.Name} role is in use";
                    ViewBag.ErrorMessage = $"{role.Name} role cannot be deleted delete the remove form assigned role";
                    return View("Error");
                }
            }
        }






        [HttpGet]

        [Authorize(Policy = "NonAdminEditClaimPolicy")]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"role with id ={id} is not found";
                return View("NotFound");
            }
            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };
            foreach (var user in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }
            return View(model);
        }
        [HttpPost]
        [Authorize(Policy = "NonAdminEditClaimPolicy")]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);


            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id= {model.Id} is not found";
                return View("NotFound");
            }

            else
            {
                role.Name = model.RoleName;
                var result = await roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }

        }













        [HttpGet]
        [Authorize(Policy = "NonAdminEditClaimPolicy")]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id ={id} cannot be found";
                return View("NotFound");
            }
            var userRoles = await userManager.GetRolesAsync(user);
            var userClaims = await userManager.GetClaimsAsync(user);
            var model = new EditUserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Gender = user.Gender,
                City = user.City,
                ExistingPhotoPath = user.PhotoPath,
                Claims = userClaims.Select(c => c.Type + ":" +c.Value).ToList(),
                Roles = userRoles
            };
            return View(model);
        }
        [HttpPost]
        [Authorize(Policy = "NonAdminEditClaimPolicy")]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
           
                var user = await userManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    ViewBag.ErrorMessage = $"User with Id{model.Id} cannot be found";
                    return View("NotFound");
                }
                else
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Email = model.Email;
                    user.UserName = model.UserName;
                    user.PhoneNumber = model.PhoneNumber;
                    user.Gender = model.Gender;
                    user.City = model.City;
                    if (model.Photo !=null)
                    {
                        if(model.ExistingPhotoPath !=null)
                        {
                            string filepath = Path.Combine(hostEnvironment.WebRootPath, "images", model.ExistingPhotoPath);
                            System.IO.File.Delete(filepath);
                        }
                        user.PhotoPath = UploadProcessModel(model);
                    }
                    var result = await userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListUsers");
                    }
                    foreach(var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
                

            
        }
        private string UploadProcessModel(RegisterViewModel model)
        {
            string uniqueFileName = null;
            if (model.Photo != null)
            {
                string uploadsFolder = Path.Combine(hostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                model.Photo.CopyTo(new FileStream(filePath, FileMode.Create));


            }
            return uniqueFileName;
        }






        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> DeleteUser (string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if(user == null)
            {
                ViewBag.ErrorMessage = $"the user with id = {id} is not found !!";
                return View("NotFound");
            }
            else
            {
                var result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);

                }
                return View("ListUsers");
            }
        }



        [HttpGet]
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult>ManageUsersForRole(string roleId)
        {
            ViewBag.roleId = roleId;
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null )
            { ViewBag.ErrorMessage = $"we cannot rolefind with Id={roleId}";
                return View("NotFound");
            }
            var model = new List<ManageUsersForRoleViewModel>();
            foreach(var user in userManager.Users)
            {
                var manageUsersForRoleViewModel = new ManageUsersForRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    manageUsersForRoleViewModel.IsSelected = true;
                }
                else
                {
                    manageUsersForRoleViewModel.IsSelected = false;
                }
                model.Add(manageUsersForRoleViewModel);
            }
            return View(model);

        }
        [HttpPost]
        [Authorize(Policy  = "NonAdminEditClaimPolicy")]
        public async Task<IActionResult> ManageUsersForRole(List<ManageUsersForRoleViewModel> model, string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }
            for(int i= 0;i<model.Count;i++)
            {
                var user = await userManager.FindByIdAsync(model[i].UserId);
                IdentityResult result = null;
                if(model[i].IsSelected &&!(await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.AddToRoleAsync(user, role.Name);
                }
                else if(!model[i].IsSelected && await userManager.IsInRoleAsync(user,role.Name))
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }
                if(result.Succeeded)
                {
                    if (i < (model.Count - 1))
                        continue;
                    else
                        return RedirectToAction("EditRole", new { Id = roleId });
                }

            }
            return RedirectToAction("EditRole", new { Id = roleId });
        }

        [HttpGet]
        [Authorize(Policy  = "NonAdminEditClaimPolicy")]
        public async Task<IActionResult>ManageUserClaim(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"user  with Id ={userId} Not Found !!";
                return View("NotFound");
            }
            var existingUserClaims = await userManager.GetClaimsAsync(user);
            var model = new UserClaimViewModel
            {
                UserId = userId

            };
            foreach(Claim claim in ClaimStore.AllClaims)
            {
                UserClaim userClaim = new UserClaim
                {
                    ClaimType = claim.Type
                };
                if(existingUserClaims.Any(c=> c.Type ==claim.Type && c.Value =="true"))
                {
                    userClaim.IsSelected = true;
                }
                model.Claims.Add(userClaim);
            }
            return View(model);

        }

        [HttpPost]
        [Authorize(Policy = "NonAdminEditClaimPolicy")]
        public async Task<IActionResult> ManageUserClaim(UserClaimViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"user with id = {model.UserId} is nOt found";
                return View("NotFoumd");
            }
            var claim = await userManager.GetClaimsAsync(user);
            var result = await userManager.RemoveClaimsAsync(user, claim);
            if(!result.Succeeded)
            {
                ModelState.AddModelError("", "cannot remove claims existing user");
                return View(model);
            }
            result = await userManager.AddClaimsAsync(user,
                model.Claims.Select(c => new Claim(c.ClaimType, c.IsSelected ? "true" : "false")));
            if(!result.Succeeded)
            {
                ModelState.AddModelError("", "cannot add selected claims to user");
                return View(model);
            }
            return RedirectToAction("EditUser", new { Id = model.UserId });
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
        [HttpGet]
        public IActionResult DashBoard()
        {
            ViewBag.invoices = context.Invoices.OrderByDescending(i => i.Id).ToList();
            ViewBag.ToalProduct = context.Products;
            var SalesCount = context.InvoiceDetailses.Include(c=>c.Invoice);
            /*ViewBag.SalesCount = context.InvoiceDetailses;*/
            ViewBag.TotalProduct = context.Products.Count();
            ViewBag.NoOfUsers = userManager.Users.Count();
            return View(SalesCount);
        }
        public async Task<IActionResult> ClaimList()
        {
            var model = new UserClaimViewModel();
            foreach (Claim claim in ClaimStore.AllClaims)
            {
                UserClaim userClaim = new UserClaim
                {
                    ClaimType = claim.Type
                };  
                model.Claims.Add(userClaim);
            }
            return View(model);
        }
    }
}




