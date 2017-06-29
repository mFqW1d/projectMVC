using arm_repairs_project.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace arm_repairs_project.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<arm_repairs_project.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(arm_repairs_project.Models.ApplicationDbContext context)
        {
            var storeRole = new RoleStore<IdentityRole>(context);
            var managerRole = new RoleManager<IdentityRole>(storeRole);
            #region Инициализация ролей
            if (!context.Roles.Any(r => r.Name == "chief"))
            {
                var role = new IdentityRole { Name = "chief" };
                managerRole.Create(role);
            }
            if (!context.Roles.Any(r => r.Name == "manager"))
            {
                var role = new IdentityRole { Name = "manager" };
                managerRole.Create(role);
            }
            if (!context.Roles.Any(r => r.Name == "master"))
            {
                var role = new IdentityRole { Name = "master" };
                managerRole.Create(role);
            }
            if (!context.Roles.Any(r => r.Name == "user"))
            {
                var role = new IdentityRole { Name = "user" };
                managerRole.Create(role);
            }
            #endregion

            #region Инициализация пользователей с ролями
            var storeUser = new UserStore<ApplicationUser>(context);
            var managerUser = new UserManager<ApplicationUser>(storeUser);
            if (!context.Users.Any(u => u.UserName == "admin"))
            {
                var user = new ApplicationUser
                {
                    Id = "1000",
                    EmailConfirmed = true,
                    Email = "infoarmrepairs@gmail.com",
                    UserName = "admin",
                    Fio = "Администратор системы"
                };
                managerUser.Create(user, "123456");
                managerUser.AddToRole(user.Id, "chief");
                managerUser.AddToRole(user.Id, "manager");
                managerUser.AddToRole(user.Id, "master");
                managerUser.AddToRole(user.Id, "user");
            }
            if (!context.Users.Any(u => u.UserName == "manager"))
            {
                var user = new ApplicationUser
                {
                    Id = "1001",
                    EmailConfirmed = true,
                    Email = "manager@gmail.com",
                    UserName = "manager",
                    Fio = "Менеджер системы"
                };
                managerUser.Create(user, "123456");
                managerUser.AddToRole(user.Id, "manager");
            }
            if (!context.Users.Any(u => u.UserName == "master"))
            {
                var user = new ApplicationUser
                {
                    Id = "1002",
                    EmailConfirmed = true,
                    Email = "master@gmail.com",
                    UserName = "master",
                    Fio = "Мастер тестовый"
                };
                managerUser.Create(user, "123456");
                managerUser.AddToRole(user.Id, "master");
            }
            if (!context.Users.Any(u => u.UserName == "user1"))
            {
                var user = new ApplicationUser
                {
                    Id = "1003",
                    EmailConfirmed = true,
                    Email = "user1@gmail.com",
                    UserName = "user1",
                    Fio = "Пользователь тестовый 1"
                };
                managerUser.Create(user, "123456");
                managerUser.AddToRole(user.Id, "user");
            }
            if (!context.Users.Any(u => u.UserName == "user2"))
            {
                var user = new ApplicationUser
                {
                    Id = "1004",
                    EmailConfirmed = true,
                    Email = "user2@gmail.com",
                    UserName = "user2",
                    Fio = "Пользователь тестовый 2"
                };
                managerUser.Create(user, "123456");
                managerUser.AddToRole(user.Id, "user");
            }
            if (!context.Users.Any(u => u.UserName == "user3"))
            {
                var user = new ApplicationUser
                {
                    Id = "1005",
                    EmailConfirmed = true,
                    Email = "user3@gmail.com",
                    UserName = "user3",
                    Fio = "Пользователь тестовый 3"
                };
                managerUser.Create(user, "123456");
                managerUser.AddToRole(user.Id, "user");
            }
            #endregion

            #region Инициализируем статусы заявок
            context.DemandStatuses.AddOrUpdate(
                    x => x.Id,
                    new Models.Data.DemandStatus { Id = 1, Caption = "Ожидание подтверждения менеджером" },
                    new Models.Data.DemandStatus { Id = 2, Caption = "Ожидание мастера" },
                    new Models.Data.DemandStatus { Id = 3, Caption = "Заказ запчасти" },
                    new Models.Data.DemandStatus { Id = 4, Caption = "В работе" },
                    new Models.Data.DemandStatus { Id = 5, Caption = "Выполнено" },
                    new Models.Data.DemandStatus { Id = 6, Caption = "Отменен" }
                    ); 
            #endregion

            #region Инициализируем приоритеты
            context.Priorities.AddOrUpdate(
                    x => x.Id,
                    new Models.Data.Priority { Id = 1, Caption = "Высокий" },
                    new Models.Data.Priority { Id = 2, Caption = "Средний" },
                    new Models.Data.Priority { Id = 3, Caption = "Низкий" }
                    ); 
            #endregion
            context.SaveChanges();

            #region Инициализируем заявки

            for (int i = 1; i < 25; i++)
            {
                context.Demands.AddOrUpdate(
                    x => x.Id,
                    new Models.Data.Demand
                    {
                        Id = i,
                        Date = DateTime.Now,
                        DescriptionIssue = "Описание проблемы "+i,
                        Phone = "0500",
                        User = context.Users.SingleOrDefault(x => x.Id == "1003"),
                        Manager = null,
                        Master = context.Users.SingleOrDefault(x => x.Id == "1000"),
                        DecisionHours = 3,
                        DecisionDescription = "Описание "+i,
                        Equipment = "Оборудование "+i,
                        Priority = context.Priorities.SingleOrDefault(x => x.Id == 1),
                        Status = context.DemandStatuses.SingleOrDefault(x => x.Id == 1)
                    }
                    );
            }
            #endregion



            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
