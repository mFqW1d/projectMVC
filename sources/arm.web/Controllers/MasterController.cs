using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using arm_repairs_project.Models;
using arm_repairs_project.Models.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace arm_repairs_project.Controllers
{
    [Authorize]
    public class MasterController : Controller
    {
        private ApplicationUserManager _userManager; //класс для управления пользователями

        public MasterController() { }

        public MasterController(ApplicationUserManager userManager)
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

        [Authorize(Roles = "master")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "master")]
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

        [Authorize(Roles = "master")]
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
                if (demand.Master?.Id != User.Identity.GetUserId())
                {
                    TempData["errors"] = "У Вас нет доступа к завке №" + demand.Id + ", т.к. Вы не исполнитель";
                    return RedirectToAction("Demands", "Master");
                }
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "master")]
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
                var newStatus = db.DemandStatuses.SingleOrDefault(x => x.Id == model.Status);
                demand.DecisionDescription = model.DecisionDescription;
                demand.DecisionHours = model.DecisionHours;
                demand.Equipment = model.Equipment;
                demand.Status = db.DemandStatuses.SingleOrDefault(x => x.Id == model.Status);
                db.SaveChanges();
                //Если статус заявки изменился отправляем пользователю письмо
                if (oldStatus.Id != newStatus.Id) UserManager.SendEmail(demand.User.Id, "Изменение статуса заявки", "Статус Вашей заявки №" + demand.Id + " изменился с \"" + oldStatus.Caption + "\" на \"" + newStatus.Caption + "\"");
                TempData["success"] = "Завка №" + demand.Id + " успешно изменена";
                return RedirectToAction("Demands", "Master");
            }
        }
    }
}