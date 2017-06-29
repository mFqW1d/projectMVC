using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using arm_repairs_project.Models;
using arm_repairs_project.Models.Data;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using Microsoft.AspNet.Identity;

namespace arm_repairs_project.Controllers
{
    [Authorize]  //Контроллер доступен только авторизованным пользователям
    public class UserController : Controller
    {
        private ApplicationUserManager _userManager; //класс для управления пользователями

        public UserController() { }

        public UserController(ApplicationUserManager userManager)
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

        [Authorize(Roles = "user")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "user")]
        public ActionResult Demands()
        {
            List<Demand> demands;
            var user_id = User.Identity.GetUserId();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Configuration.LazyLoadingEnabled = false;
                demands = db.Demands
                    .Include(x => x.Master)
                    .Include(x => x.Priority)
                    .Include(x => x.Status)
                    .Include(x => x.User)
                    .Where(x=>x.User.Id==user_id)
                    .ToList();
            }
            var model = new Demands
            {
                DemandsList= demands
            };
            return View(model);
        }

        [Authorize(Roles = "user")]
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
                model.DescriptionIssue = demand.DescriptionIssue;
                model.User = demand.User.Id;
                model.Phone = demand.Phone;

                //Если исполнитель по заявке не текущий пользователь редиректим назад
                if (demand.User?.Id != User.Identity.GetUserId())
                {
                    TempData["errors"] = "У Вас нет доступа к завке №" + demand.Id + ", т.к. она Вам не принадлежит";
                    return RedirectToAction("Demands", "User");
                }
            }
            return View(model);
        }

        [Authorize(Roles = "user")]
        public ActionResult DemandCreate()
        {
            var model=new DemandModel
            {
                User = User.Identity.GetUserId()
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "user")]
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
            return RedirectToAction("Demands", "User");
        }
    }
}