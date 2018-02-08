using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using LocalTheatre.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace LocalTheatre.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private const string WriterRoleId = "2";
        private const string UserRoleId = "3";

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get => _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            private set => _signInManager = value;
        }

        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = UserManager.FindByEmailAsync(model.Email).Result;

            try
            {
                if (!user.IsEnable)
                {
                    return View("Lockout");
                }
            }
            catch
            {
                // ignored
            }

            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe,
                shouldLockout: false);

            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new {ReturnUrl = returnUrl, model.RememberMe});
                default:
                    ModelState.AddModelError("", @"Invalid login attempt.");
                    return View(model);
            }
        }

        // GET: /Account/ChangeName
        [Authorize]
        public ActionResult ChangeName()
        {
            ViewBag.CurrentDisplayName = UserManager.FindById(User.Identity.GetUserId()).DisplayName;
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeName(ChangeDisplayNameViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = UserManager.FindById(User.Identity.GetUserId());
                user.DisplayName = model.NewDisplayName;
                await UserManager.UpdateAsync(user);
                ViewBag.CurrentDisplayName = UserManager.FindById(User.Identity.GetUserId()).DisplayName;
                ViewBag.Confirmation = Message.NameChanged;
                return View();
            }

            ViewBag.CurrentDisplayName = UserManager.FindById(User.Identity.GetUserId()).DisplayName;

            return View(model);
        }

        // GET: /Account/ChangeName
        [Authorize]
        public ActionResult ChangeEmail(bool? confirm)
        {
            ViewBag.CurrentEmail = UserManager.FindById(User.Identity.GetUserId()).Email;

            if (confirm != null && confirm == true)
            {
                ViewBag.Confirmation = Message.EmailChanged;
            }

            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeEmail(ChangeEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = UserManager.FindById(User.Identity.GetUserId());
                user.Email = model.NewEmail;
                user.UserName = model.NewEmail;
                await UserManager.UpdateAsync(user);

                // Sign in user automatically after change of username
                await SignInManager.SignInAsync(user, true, true);

                return RedirectToAction("ChangeEmail", new {confirm = true});
            }

            ViewBag.CurrentEmail = UserManager.FindById(User.Identity.GetUserId()).Email;

            return View(model);
        }


        // GET: /Account/ChangeProfileImage
        [Authorize]
        public ActionResult ChangeProfileImage()
        {
            ViewBag.CurrentProfileUrl = UserManager.FindById(User.Identity.GetUserId()).ProfileUrl;
            ViewBag.CurentProfileFileName = UserManager.FindById(User.Identity.GetUserId()).ProfileFileName;
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeProfileImage(ChangeProfileImageViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = UserManager.FindById(User.Identity.GetUserId());
                user.ProfileUrl = model.NewProfileUrl;
                user.ProfileFileName = model.NewProfileFileName;

                await UserManager.UpdateAsync(user);
                ViewBag.CurrentProfileUrl = UserManager.FindById(User.Identity.GetUserId()).ProfileUrl;
                ViewBag.CurentProfileFileName = UserManager.FindById(User.Identity.GetUserId()).ProfileFileName;
                ViewBag.Confirmation = Message.ProfileImageChanged;
                return View();
            }

            return View(model);
        }


        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    DisplayName = model.DisplayName,
                    UserName = model.Email,
                    Email = model.Email,
                    ProfileUrl = model.ProfileUrl,
                    ProfileFileName = model.ProfileFileName,
                    IsEnable = true
                };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //Add default role to new user
                    var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());
                    var roleManager = new RoleManager<IdentityRole>(roleStore);
                    await roleManager.CreateAsync(new IdentityRole(RoleName.User));
                    await UserManager.AddToRoleAsync(user.Id, RoleName.User);

                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        [Authorize(Roles = RoleName.Admin)]
        public ActionResult UsersList(ApplicationDbContext context)
        {
            var allUsers = context.Users.ToList();

            var writers = allUsers.Where(x => x.Roles.Select(role => role.RoleId).Contains(WriterRoleId)).ToList();
            var writerVm = writers.Select(user => new UserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Role = string.Join(",", user.Roles.Select(role => role.RoleId)),
                DisplayName = user.DisplayName,
                IsEnable = user.IsEnable
            }).OrderBy(e => e.Email).ToList();


            var users = allUsers.Where(x => x.Roles.Select(role => role.RoleId).Contains(UserRoleId)).ToList();
            var userVm = users.Select(user => new UserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Role = string.Join(",", user.Roles.Select(role => role.RoleId)),
                DisplayName = user.DisplayName,
                IsEnable = user.IsEnable
            }).OrderBy(e => e.Email).ToList();

            var model = new GroupedUserViewModel {Writers = writerVm, Users = userVm};

            return View(model);
        }

        [Authorize(Roles = RoleName.Admin)]
        public ActionResult EditUser(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = UserManager.FindById(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            var model = new UserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Role = RoleName.FromId(string.Join(",", user.Roles.Select(role => role.RoleId))),
                DisplayName = user.DisplayName,
                IsEnable = user.IsEnable
            };

            var list = new SelectList(new[]
                {
                    new {ID = RoleName.Writer, Name = RoleName.Writer},
                    new {ID = RoleName.User, Name = RoleName.User},
                },
                "ID", "Name", 1);

            ViewData["roleList"] = list;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleName.Admin)]
        public async Task<ActionResult> EditUser(UserViewModel model, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                var user = UserManager.FindById(model.Id);

                if (!user.IsEnable.Equals(model.IsEnable))
                {
                    user.IsEnable = model.IsEnable;
                    await UserManager.UpdateAsync(user);
                }

                var oldRole = RoleName.FromId(user.Roles.SingleOrDefault()?.RoleId);
                var newRole = form["roleList"];

                if (!oldRole.Equals(newRole) && !newRole.IsNullOrWhiteSpace())
                {
                    await UserManager.RemoveFromRoleAsync(user.Id, oldRole);
                    await UserManager.AddToRoleAsync(user.Id, newRole);

                    await UserManager.UpdateSecurityStampAsync(user.Id);
                }

                Db.SaveChanges();

                return RedirectToAction("UsersList");
            }

            return View(model);
        }


        // GET: Account/DeleteUser/5
        [Authorize(Roles = RoleName.Admin)]
        public ActionResult DeleteUser(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = UserManager.FindById(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            var model = new UserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Role = RoleName.FromId(string.Join(",", user.Roles.Select(role => role.RoleId))),
                DisplayName = user.DisplayName,
                IsEnable = user.IsEnable
            };

            return View(model);
        }

        // POST: Account/DeleteUser/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteUser(string id, int? x)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = UserManager.Users.SingleOrDefault(u => u.Id == id);

            await UserManager.DeleteAsync(user);

            return RedirectToAction("UsersList");
        }


        #region Helpers

        private IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }
        }

        #endregion
    }
}