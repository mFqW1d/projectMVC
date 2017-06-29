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
            #region ������������� �����
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

            #region ������������� ������������� � ������
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
                    Fio = "������������� �������"
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
                    Fio = "�������� �������"
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
                    Fio = "������ ��������"
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
                    Fio = "������������ �������� 1"
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
                    Fio = "������������ �������� 2"
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
                    Fio = "������������ �������� 3"
                };
                managerUser.Create(user, "123456");
                managerUser.AddToRole(user.Id, "user");
            }
            #endregion

            #region �������������� ������� ������
            context.DemandStatuses.AddOrUpdate(
                    x => x.Id,
                    new Models.Data.DemandStatus { Id = 1, Caption = "�������� ������������� ����������" },
                    new Models.Data.DemandStatus { Id = 2, Caption = "�������� �������" },
                    new Models.Data.DemandStatus { Id = 3, Caption = "����� ��������" },
                    new Models.Data.DemandStatus { Id = 4, Caption = "� ������" },
                    new Models.Data.DemandStatus { Id = 5, Caption = "���������" },
                    new Models.Data.DemandStatus { Id = 6, Caption = "�������" }
                    ); 
            #endregion

            #region �������������� ����������
            context.Priorities.AddOrUpdate(
                    x => x.Id,
                    new Models.Data.Priority { Id = 1, Caption = "�������" },
                    new Models.Data.Priority { Id = 2, Caption = "�������" },
                    new Models.Data.Priority { Id = 3, Caption = "������" }
                    ); 
            #endregion
            context.SaveChanges();

            #region �������������� ������

            for (int i = 1; i < 25; i++)
            {
                context.Demands.AddOrUpdate(
                    x => x.Id,
                    new Models.Data.Demand
                    {
                        Id = i,
                        Date = DateTime.Now,
                        DescriptionIssue = "�������� �������� "+i,
                        Phone = "0500",
                        User = context.Users.SingleOrDefault(x => x.Id == "1003"),
                        Manager = null,
                        Master = context.Users.SingleOrDefault(x => x.Id == "1000"),
                        DecisionHours = 3,
                        DecisionDescription = "�������� "+i,
                        Equipment = "������������ "+i,
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
