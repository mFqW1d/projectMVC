using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using arm_repairs_project.Models;
using arm_repairs_project.Models.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace arm_repairs_project.Controllers
{
    [Authorize]  //Контроллер доступен только авторизованным пользователям
    public class ChiefController : Controller
    {
        private ApplicationUserManager _userManager; //класс для управления пользователями

        public ChiefController() { }

        public ChiefController(ApplicationUserManager userManager)
        {
            UserManager = userManager; //делаем инъекцию через конструктор
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [Authorize(Roles = "chief")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "chief")]
        public ActionResult Users()
        {
            List<ApplicationUser> users = new List<ApplicationUser>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                users = db.Users.ToList();
            }
            var model = new ManageUserViewModels.MangeUsers
            {
                Users = users
            };
            return View(model);
        }

        [Authorize(Roles = "chief")]
        public ActionResult UserEdit(string id)
        {
            var user = UserManager.FindById(id);
            var model = new ManageUserViewModels.User();
            if (user != null)
            {
                model.Id = user.Id;
                model.Fio = user.Fio;
                model.UserName = user.UserName;
                model.EmailConfirmed = user.EmailConfirmed;
                model.Email = user.Email;
                model.IsChief = UserManager.GetRoles(user.Id).Contains("chief");
                model.IsManager = UserManager.GetRoles(user.Id).Contains("manager");
                model.IsMaster = UserManager.GetRoles(user.Id).Contains("master");
                model.IsUser = UserManager.GetRoles(user.Id).Contains("user");
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "chief")]
        [ValidateAntiForgeryToken]
        public ActionResult UserEdit(ManageUserViewModels.User model)
        {
            //Проверяем модель на валидность
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = UserManager.FindById(model.Id);
            //Если пользователь не найден то возвращаем ошибку
            if (user == null)
            {
                ModelState.AddModelError("", "Пользователь не найден.");
                return View(model);
            }

            //Ищем пользователя с таким же email
            var email = UserManager.FindByEmail(model.Email);
            //Если найден пользователь с таким же email и это другой пользователь то возварщаем ошибку
            if ((email != null) && (email.Id != user.Id))
            {
                ModelState.AddModelError("", "Пользователь с таким Email уже существует.");
                return View(model);
            }

            //Ищем пользователя с таким же UserName
            var userName = UserManager.FindByName(model.UserName);
            //Если найден пользователь с таким же user name и это другой пользователь то возварщаем ошибку
            if ((userName != null) && (userName.Id != user.Id))
            {
                ModelState.AddModelError("", "Пользователь с таким UserName уже существует.");
                return View(model);
            }

            user.Fio = model.Fio;
            user.UserName = model.UserName;
            user.Email = model.Email;
            user.EmailConfirmed = model.EmailConfirmed;
            UserManager.Update(user);

            //Присваиваем или удалем роли у пользователя
            if (model.IsChief) UserManager.AddToRole(model.Id, "chief"); else UserManager.RemoveFromRole(model.Id, "chief");
            if (model.IsManager) UserManager.AddToRole(model.Id, "manager"); else UserManager.RemoveFromRole(model.Id, "manager");
            if (model.IsMaster) UserManager.AddToRole(model.Id, "master"); else UserManager.RemoveFromRole(model.Id, "master");
            if (model.IsUser) UserManager.AddToRole(model.Id, "user"); else UserManager.RemoveFromRole(model.Id, "user");

            TempData["success"] = "Данные успешно сохранены";
            return View(model);
        }

        [Authorize(Roles = "chief")]
        public ActionResult UserDelete(string id)
        {
            var user = UserManager.FindById(id);
            var model = new ManageUserViewModels.User();
            if (user != null)
            {
                model.Id = user.Id;
                model.Fio = user.Fio;
                model.UserName = user.UserName;
                model.EmailConfirmed = user.EmailConfirmed;
                model.Email = user.Email;
                model.IsChief = UserManager.GetRoles(user.Id).Contains("chief");
                model.IsManager = UserManager.GetRoles(user.Id).Contains("manager");
                model.IsMaster = UserManager.GetRoles(user.Id).Contains("master");
                model.IsUser = UserManager.GetRoles(user.Id).Contains("user");
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "chief")]
        [ValidateAntiForgeryToken]
        public ActionResult UserDelete(ManageUserViewModels.User model)
        {
            //Проверяем модель на валидность
            if ((model.Id == null) || (model.Id == String.Empty))
            {
                ModelState.AddModelError("", "Идентификатор пользователя не определен.");
                return View(model);
            }
            var user = UserManager.FindById(model.Id);
            //Если пользователь не найден то возвращаем ошибку
            if (user == null)
            {
                ModelState.AddModelError("", "Пользователь не найден.");
                return View(model);
            }
            UserManager.Delete(user);
            return RedirectToAction("Users", "Chief");
        }

        [Authorize(Roles = "chief")]
        public ActionResult ChangePassword(string id)
        {
            var user = UserManager.FindById(id);
            var model = new ManageUserViewModels.ChangePasswordViewModel();
            if (user != null)
            {
                model.Id = user.Id;
                model.Fio = user.Fio;
                model.UserName = user.UserName;

                string code = UserManager.GeneratePasswordResetToken(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                UserManager.SendEmail(user.Id, "Сброс пароля", "Для сброса пароля перейдите по ссылке <a href=\"" + callbackUrl + "\">here</a>");
            }
            return View(model);
        }

        [Authorize(Roles = "chief, manager")]
        public ActionResult Demands()
        {
            List<Demand> demands;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                demands = db.Demands.Include(x => x.Master).Include(x => x.Priority).Include(x => x.Status).Include(x => x.User).ToList();
            }
            var model = new Demands
            {
                DemandsList = demands
            };
            return View(model);
        }

        [Authorize(Roles = "chief, manager")]
        public ActionResult DemandCreate()
        {
            return View(new DemandModel());
        }

        [HttpPost]
        [Authorize(Roles = "chief, manager")]
        [ValidateAntiForgeryToken]
        public ActionResult DemandCreate(DemandModel model)
        {
            //Проверяем модель на валидность
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            int id;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var newDemand = new Demand
                {
                    Date = DateTime.Now,
                    DescriptionIssue = model.DescriptionIssue,
                    Status = db.DemandStatuses.SingleOrDefault(x => x.Id == model.Status),
                    Priority = db.Priorities.SingleOrDefault(x => x.Id == model.Priority),
                    User = db.Users.SingleOrDefault(x => x.Id == model.User),
                    Phone = model.Phone,
                    Master = model.Master != null ? db.Users.SingleOrDefault(x => x.Id == model.Master) : null,
                    Manager = model.Manager != null ? db.Users.SingleOrDefault(x => x.Id == model.Manager) : null,
                    DecisionDescription = model.DecisionDescription,
                    Equipment = model.Equipment,
                    DecisionHours = model.DecisionHours
                };

                db.Demands.Add(newDemand);
                db.SaveChanges();
                id = newDemand.Id;
            }
            TempData["success"] = "Новая завка создана под номером " + id;
            return RedirectToAction("Demands", "Chief");
        }

        [Authorize(Roles = "chief, manager")]
        public ActionResult DemandEdit(int id)
        {
            Demand demand;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                demand = db.Demands.Include(x => x.Master).Include(x => x.Priority).Include(x => x.Status).Include(x => x.User).Include(x => x.Manager).SingleOrDefault(x => x.Id == id);
            }
            var model = new DemandModel();
            if (demand != null)
            {
                model.Id = demand.Id;
                model.Priority = demand.Priority.Id;
                model.User = demand.User.Id;
                model.Manager = demand.Manager?.Id;
                model.Master = demand.Master?.Id;
                model.DescriptionIssue = demand.DescriptionIssue;
                model.Status = demand.Status.Id;
                model.Phone = demand.Phone;
                model.DecisionDescription = demand.DecisionDescription;
                model.DecisionHours = demand.DecisionHours;
                model.Equipment = demand.Equipment;
                model.Date = demand.Date;
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "chief, manager")]
        [ValidateAntiForgeryToken]
        public ActionResult DemandEdit(DemandModel model)
        {
            //Проверяем модель на валидность
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var demand = db.Demands.SingleOrDefault(x => x.Id == model.Id);
                if (demand == null)
                {
                    ModelState.AddModelError("", "Заявка не найдена");
                    return View(model);
                }
                var oldStatus = demand.Status;
                var newStatus = db.DemandStatuses.SingleOrDefault(x=>x.Id==model.Status);
                demand.DecisionDescription = model.DecisionDescription;
                demand.DecisionHours = model.DecisionHours;
                demand.Equipment = model.Equipment;
                demand.Manager = model.Manager != null ? db.Users.SingleOrDefault(x => x.Id == model.Manager) : null;
                demand.Master = model.Master != null ? db.Users.SingleOrDefault(x => x.Id == model.Master) : null;
                demand.Priority = db.Priorities.SingleOrDefault(x => x.Id == model.Priority);
                demand.Status = db.DemandStatuses.SingleOrDefault(x => x.Id == model.Status);
                demand.User = db.Users.SingleOrDefault(x => x.Id == model.User);
                demand.DescriptionIssue = model.DescriptionIssue;
                demand.Phone = model.Phone;
                db.SaveChanges();
                //Если статус заявки изменился отправляем пользователю письмо
                if (oldStatus.Id != newStatus.Id) UserManager.SendEmail(demand.User.Id,"Изменение статуса заявки","Статус Вашей заявки №"+demand.Id+" изменился с \""+oldStatus.Caption+"\" на \""+newStatus.Caption+"\"");
                TempData["success"] = "Завка №" + demand.Id + " успешно изменена";
                return RedirectToAction("Demands", "Chief");
            }
        }

        [Authorize(Roles = "chief, manager")]
        public ActionResult DemandDelete(int id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var demand = db.Demands.SingleOrDefault(x => x.Id == id);
                var model=new DemandModel();
                if (demand != null)
                {
                    model.Id = demand.Id;
                    model.Date = demand.Date;
                }
                return View(model);
            }
        }

        [HttpPost]
        [Authorize(Roles = "chief, manager")]
        [ValidateAntiForgeryToken]
        public ActionResult DemandDelete(DemandModel model)
        {
            //Проверяем модель на валидность
            if (model== null)
            {
                ModelState.AddModelError("", "Идентификатор заявки не определен.");
                return View(model);
            }
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var demand = db.Demands.SingleOrDefault(x => x.Id == model.Id);
                //Если заявка не найдена то возвращаем ошибку
                if (demand == null)
                {
                    ModelState.AddModelError("", "Заявка не найдена.");
                    return View(model);
                }
                var id=demand.Id;
                db.Demands.Remove(demand);
                db.SaveChanges();
                TempData["success"] = "Завка №" + id + " успешно удалена";
                return RedirectToAction("Demands", "Chief");
            }
        }
    }
}
