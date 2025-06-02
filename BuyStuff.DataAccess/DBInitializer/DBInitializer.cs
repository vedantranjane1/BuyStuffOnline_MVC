using BuyStuffOnline.DataAccess.Data;
using BuyStuffOnline.Models;
using BuyStuffOnline.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuyStuffOnline.DataAccess.DBInitializer
{
	public class DBInitializer : IDBInitializer


	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly ApplicationDbContext _db;

		public DBInitializer(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext applicationDbContext)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_db = applicationDbContext;

		}
		public void Intialize()
		{
			//migrations if they are not applied

			try
			{
				if (_db.Database.GetPendingMigrations().Count() > 0)
				{
					_db.Database.Migrate();
				}
			}
			catch (Exception ex) { }

			//create roles if not created
			if (!_roleManager.RoleExistsAsync(StaticDetails.Role_Admin).GetAwaiter().GetResult())
			{
				_roleManager.CreateAsync(new IdentityRole(StaticDetails.Role_Admin)).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new IdentityRole(StaticDetails.Role_Customer)).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new IdentityRole(StaticDetails.Role_Company)).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new IdentityRole(StaticDetails.Role_Employee)).GetAwaiter().GetResult();


				//create admin user is role are not created 

				_userManager.CreateAsync(new ApplicationUser
				{
					UserName = "MasterUser",
					Email = "MasterUser@gmail.com",
					Name = "Vedant Ranjane",
					PhoneNumber = "1234567890",
					Street = "L.B.S Road",
					State = "Maharashtra",
					PostalCode = "12345",
					City = "Mumbai"
				}, "Admin123*").GetAwaiter().GetResult();

				ApplicationUser user = _db.applicationUsers.FirstOrDefault(u => u.Email == "MasterUser@gmail.com");
				_userManager.AddToRoleAsync(user, StaticDetails.Role_Admin).GetAwaiter().GetResult();
			}

			return;
		}
	}
}
