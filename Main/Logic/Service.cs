using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;

namespace Main
{
    /// <summary>
    /// Service class contains all logic operations that are being done in the project,
    /// except logic operations that are directly connected to elements of wpf windows or controls.
    /// </summary>
    public static class Service
    {
        public static List<User> Users { get; set; }

        /// <summary>
        /// User list initialization.
        /// </summary>
        public static void UserListInitialization()
        {
            Users = new List<User>();

            //Retrieves data from "User" table and binds it with users list and sets the data context.
            using (var context = new WarehouseDBEntities())
            {
                var users = from u in context.User select u;
                foreach (var user in users)
                {
                    Users.Add(user);
                }
            }
        }

        /// <summary>
        /// Real Time user list update also known as 'refresh'.
        /// </summary>
        public static void LoadUsers()
        {
            Users.Clear();

            using (var context = new WarehouseDBEntities())
            {
                var users = context.User.ToList();
                foreach (var user in users)
                {
                    Users.Add(user);
                }
            }
        }

        /// <summary>
        /// Method which saves changes to database.
        /// </summary>
        /// <param name="selectedUser">Selected user in datagrid</param>
        public static void ApplyChanges(User selectedUser)
        {
            using (var context = new WarehouseDBEntities())
            {
                context.Entry(selectedUser).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Adds user to database (most of this function is inside of UsersControl.xaml
        /// </summary>
        /// <param name="newUser">New user that is being created</param>
        public static void AddUser(User newUser)
        {
            newUser.Id = Guid.NewGuid();
            using (var context = new WarehouseDBEntities())
            {
                context.User.Add(newUser);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Removes user from database.
        /// </summary>
        /// <param name="selectedUser">Selected user from datagrid</param>
        /// <exception cref="ArgumentNullException">Is thrown when user has not been selected</exception>
        public static void Removal(User selectedUser)
        {
            try
            {
                using (var context = new WarehouseDBEntities())
                {
                    if (context.Entry(selectedUser).State == EntityState.Detached)
                    {
                        context.User.Attach(selectedUser);
                    }

                    context.User.Remove(selectedUser);
                    context.SaveChanges();
                }

                LoadUsers();
            }
            catch (ArgumentNullException anex)
            {
                MessageBox.Show($"{anex.Message}.\nIf you want to delete user you have to select him in the first place.");
            }
        }

        /// <summary>
        /// Method that check whether the PESEL number is correct or not.
        /// </summary>
        /// <param name="pesel">pesel number to be checked by method</param>
        /// <returns>True if pesel format is correct or False if it is not correct</returns>
        public static bool ValidatePESEL(string pesel, DateTime birthDate, string gender)
        {
            if (pesel.Length != 11 || !pesel.All(char.IsDigit))
            {
                return false;
            }

            int year = int.Parse(pesel.Substring(0, 2));
            int month = int.Parse(pesel.Substring(2, 2));
            int day = int.Parse(pesel.Substring(4, 2));

            int birthYear = 1900 + year;
            if (month > 12)
            {
                birthYear += 100;
                month -= 20;
            }

            if (birthDate.Year != birthYear || birthDate.Month != month || birthDate.Day != day)
            {
                return false;
            }

            int genderDigit = int.Parse(pesel.Substring(9, 1));
            bool isMale = genderDigit % 2 == 1;

            if ((gender == "Male" && !isMale) || (gender == "Female" && isMale))
            {
                return false;
            }

            int[] weights = { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3 };
            int sum = 0;

            for (int i = 0; i < 10; i++)
            {
                sum += int.Parse(pesel[i].ToString()) * weights[i];
            }

            int controlNumber = (10 - (sum % 10)) % 10;

            return controlNumber == int.Parse(pesel[10].ToString());
        }


        /// <summary>
        /// Method that check whether the Phone Number is correct or not.
        /// </summary>
        /// <param name="phoneNumber">phone number to be checked by method</param>
        /// <returns>True if phone number format is correct or False if it is not correct</returns>
        public static bool ValidatePhoneNumber(string phoneNumber)
        {
            phoneNumber = phoneNumber.Replace(" ", "").Replace("-", "");

            return phoneNumber.Length == 9 && phoneNumber.All(char.IsDigit);
        }
    }
}