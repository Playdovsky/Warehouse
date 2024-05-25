using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace Main
{
    /// <summary>
    /// Service class contains all logic operations that are being done in the project,
    /// except logic operations that are directly connected to elements of wpf windows or controls.
    /// </summary>
    public static class Service
    {
        public static List<UserView> Users { get; set; }
        public static List<WarehouseView> Warehouse { get; set; }
        public static List<Products> Products { get; set; }
        public static List<ProductVAT> ProductVAT { get; set; }
        public static List<ProductType> ProductType { get; set; }
        public static List<ProductsHistory> ProductsHistory { get; set; }
        public static List<ProductStock> ProductStock { get; set; }

        /// <summary>
        /// Users list and permissions initialization.
        /// </summary>
        public static void UserDataInitialization()
        {
            using (var context = new WarehouseDatabaseEntities())
            {
                Users = context.UserView.ToList();
            }
        }

        /// <summary>
        /// Warehouse stock initialization.
        /// </summary>
        public static void WarehouseDataInitialization()
        {
            using (var context = new WarehouseDatabaseEntities())
            {
                Warehouse = context.WarehouseView.ToList();
            }
        }

        /// <summary>
        /// Products list initialization.
        /// </summary>
        public static void ProductDataInitialization()
        {
            using (var context = new WarehouseDatabaseEntities())
            {
                Products = context.Products.ToList();
                ProductVAT = context.ProductVAT.ToList();
                ProductType = context.ProductType.ToList();
            }
        }

        /// <summary>
        /// Product history initialization.
        /// </summary>
        public static void ProductHistoryInitialization()
        {
            using (var context = new WarehouseDatabaseEntities())
            {
                ProductsHistory = context.ProductsHistory.ToList();
            }
        }

        /// <summary>
        /// Product stock initialization.
        /// </summary>
        public static void ProductStockInitialization()
        {
            using (var context = new WarehouseDatabaseEntities())
            {
                ProductStock = context.ProductStock.ToList();
            }
        }

        /// <summary>
        /// Real Time user list update also known as 'refresh'.
        /// </summary>
        public static void LoadUsers()
        {
            Users.Clear();

            using (var context = new WarehouseDatabaseEntities())
            {
                Users = context.UserView.ToList();
            }
        }

        /// <summary>
        /// Real Time warehouse stock update also known as 'refresh'.
        /// </summary>
        public static void LoadWarehouse()
        {
            Warehouse.Clear();

            using (var context = new WarehouseDatabaseEntities())
            {
                Warehouse = context.WarehouseView.ToList();
            }
        }

        /// <summary>
        /// Real Time user list update also known as 'refresh'.
        /// </summary>
        public static void LoadProducts()
        {
            Products.Clear();

            using (var context = new WarehouseDatabaseEntities())
            {
                Products = context.Products.ToList();
            }
        }

        /// <summary>
        /// Real Time product stock update also known as 'refresh'.
        /// </summary>
        public static void LoadProductStock()
        {
            ProductStock.Clear();

            using (var context = new WarehouseDatabaseEntities())
            {
                ProductStock = context.ProductStock.ToList();
            }
        }

        /// <summary>
        /// Method which saves changes into tabase.
        /// </summary>
        /// <param name="selectedUser">Selected user in datagrid</param>
        public static void ApplyChanges(UserView selectedUser, User tempUser, bool includePass = false)
        {
            try {
                using (var context = new WarehouseDatabaseEntities())
                {
                    var userToUpdate = context.User.SingleOrDefault(u => u.Id == selectedUser.Id);

                    if (userToUpdate != null)
                    {
                        if (includePass)
                        {
                            userToUpdate.Password = tempUser.Password;
                        }
                        userToUpdate.FirstName = tempUser.FirstName;
                        userToUpdate.LastName = tempUser.LastName;
                        userToUpdate.Login = tempUser.Login;
                        userToUpdate.Email = tempUser.Email;
                        userToUpdate.City = tempUser.City;
                        userToUpdate.Street = tempUser.Street;
                        userToUpdate.PostalCode = tempUser.PostalCode;
                        userToUpdate.HouseNumber = tempUser.HouseNumber;
                        userToUpdate.ApartmentNumber = tempUser.ApartmentNumber;
                        userToUpdate.Pesel = tempUser.Pesel;
                        userToUpdate.PhoneNumber = tempUser.PhoneNumber;
                        userToUpdate.Role = tempUser.Role;
                        userToUpdate.Gender = tempUser.Gender;
                        userToUpdate.BirthDate = tempUser.BirthDate;
                        userToUpdate.UserPermissions.Clear();
                        userToUpdate.UserPermissions = tempUser.UserPermissions;

                        context.SaveChanges();
                    }
                    else
                    {
                        throw new Exception("User not found in database.");
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Adds user to database (most of this function is inside of UsersControl.xaml
        /// </summary>
        /// <param name="newUser">New user that is being created</param>
        public static void AddUser(User newUser)
        {
            using (var context = new WarehouseDatabaseEntities())
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
        public static void Removal(UserView selectedUser)
        {
            try
            {
                using (var context = new WarehouseDatabaseEntities())
                {
                    User user = context.User.FirstOrDefault(x => x.Id == selectedUser.Id);

                    if (context.Entry(user).State == EntityState.Detached)
                    {
                        context.User.Attach(user);
                    }

                    user.IsForgotten = true;
                    context.Entry(user).State = EntityState.Modified;
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
        /// Removes all permissions from selected user.
        /// Mainly used to avoid NULL entries when modifying user permissions.
        /// </summary>
        /// <param name="selectedUser">Selected user from datagrid</param>
        public static void PermissionsRemoval(UserView selectedUser)
        {
            using (var context = new WarehouseDatabaseEntities())
            {
                var userPermissions = context.UserPermissions.Where(up => up.UserId == selectedUser.Id);
                context.UserPermissions.RemoveRange(userPermissions);
                context.SaveChanges();
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

            using (var context = new WarehouseDatabaseEntities())
            {
                var existingUserWithPhoneNumber = context.User.FirstOrDefault(u => u.Pesel == pesel && u.IsForgotten == false);
                if (existingUserWithPhoneNumber != null)
                {
                    throw new FormatException("User with this pesel number already exists.");
                }
            }

            return controlNumber == int.Parse(pesel[10].ToString());
        }

        /// <summary>
        /// Method that check whether the Phone Number is correct or not.
        /// </summary>
        /// <param name="phoneNumber">phone number to be checked by a method</param>
        /// <returns>True if phone number format is correct or False if it is not correct</returns>
        public static bool ValidatePhoneNumber(string phoneNumber)
        {
            phoneNumber = ConvertPhoneNumber(phoneNumber);
            
            using (var context = new WarehouseDatabaseEntities())
            {
                var existingUserWithPhoneNumber = context.User.FirstOrDefault(u => u.PhoneNumber == phoneNumber && u.IsForgotten == false);
                if (existingUserWithPhoneNumber != null)
                {
                    throw new FormatException("User with this phone number already exists.");
                }
            }

            return phoneNumber.Length == 9 && phoneNumber.All(char.IsDigit);
        }

        /// <summary>
        /// This function unifies inserted phone number into one continuous string. 
        /// </summary>
        /// <param name="phoneNumber">phone number to be unified</param>
        /// <returns>unified phone number without spaces and dashes</returns>
        public static string ConvertPhoneNumber(string phoneNumber)
        {
            phoneNumber = phoneNumber.Replace(" ", "").Replace("-", "");

            return phoneNumber;
        } 

        /// <summary>
        /// Method that checks whether the email address is correct or not.
        /// </summary>
        /// <param name="email">email address to be checked by the method</param>
        /// <returns>True if the email format is correct, or False if it is not correct</returns>
        public static bool ValidateEmail(string email)
        {
            using (var context = new WarehouseDatabaseEntities())
            {
                var existingUser = context.User.FirstOrDefault(u => u.Email == email && u.IsForgotten == false);
                if (existingUser != null)
                {
                    throw new FormatException("User with this email already exists.");
                }
            }

            // If email is empty, return false
            if (string.IsNullOrEmpty(email))
                return false;

            // Regular expression pattern for email validation
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            // Check if email matches the pattern
            return Regex.IsMatch(email, pattern);
        }

        /// <summary>
        /// Validates a password against specified criteria.
        /// </summary>
        /// <param name="password">The password to validate.</param>
        /// <returns>True if the password meets all validation criteria, otherwise false.</returns>
        public static bool ValidatePassword(string password)
        {
            string summaryMessage = "Password";
            bool passedValidation = true;

            if (password.Length < 8 || password.Length > 15)
            {
                summaryMessage = summaryMessage + "\n- length must be between 8 and 15 characters. ";
                passedValidation = false;
            }

            if (!password.Any(char.IsUpper))
            {
                summaryMessage = summaryMessage + "\n- must contain at least one uppercase letter. ";
                passedValidation = false;
            }

            if (!password.Any(char.IsLower))
            {
                summaryMessage = summaryMessage + "\n- must contain at least one lowercase letter. ";
                passedValidation = false;
            }

            if (!password.Any(char.IsDigit))
            {
                summaryMessage = summaryMessage + "\n- must contain at least one digit. ";
                passedValidation = false;
            }

            string specialCharacters = "-_!*#$&";
            if (!password.Any(c => specialCharacters.Contains(c)))
            {
                summaryMessage = summaryMessage + "\n- must contain at least one special character (-, _, !, *, #, $, &). ";
                passedValidation = false;
            }

            if (!passedValidation)
            {
                MessageBox.Show(summaryMessage, "Password Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return passedValidation;
        }


        /// <summary>
        /// Ends application processes.
        /// </summary>
        public static void Exit()
        {
            MessageBoxResult result = MessageBox.Show("Do you really want to exit?", "Exit", MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                Environment.Exit(0);
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// Checks if a user with the provided login exists in the database.
        /// </summary>
        /// <param name="login">The login provided by the user</param>
        /// <returns>True if a user with the provided login exists, otherwise false</returns>
        public static bool ValidateLogin(string login)
        {
            using (var context = new WarehouseDatabaseEntities())
            {
                var user = context.User.FirstOrDefault(u => u.Login == login && u.IsForgotten == false);

                return user != null;
            }
        }

        /// <summary>
        /// Validates the password for the user with the provided login.
        /// </summary>
        /// <param name="login">The login of the user</param>
        /// <param name="password">The password provided by the user</param>
        /// <returns>True if the password matches the user's password, otherwise false</returns>
        public static bool ValidatePasswordLoginMatch(string login, string password)
        {
            using (var context = new WarehouseDatabaseEntities())
            {
                var user = context.User.FirstOrDefault(u => u.Login == login && u.IsForgotten == false);

                if (user == null)
                {
                    return false;
                }
                return user.Password == password;
            }
        }
        /// <summary>
        /// Retrieves the ID of the user based on their login.
        /// </summary>
        /// <param name="login">The login of the user.</param>
        /// <returns>The ID of the user.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the user with the specified login is not found.</exception>
        public static Guid GetUserId(string login)
        {
            using (var context = new WarehouseDatabaseEntities())
            {
                var user = context.User.FirstOrDefault(u => u.Login == login && u.IsForgotten == false);

                if (user != null)
                {
                    return user.Id;
                }
                else
                {
                    throw new InvalidOperationException("User not found.");
                }
            }
        }

        /// <summary>
        /// Retrieves the permissions assigned to the user based on their user ID.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A list of permission numbers assigned to the user.</returns>
        public static List<int> GetUserPermissions(Guid userId)
        {
            using (var context = new WarehouseDatabaseEntities())
            {
                var userPermissions = context.UserPermissions
                    .Where(up => up.UserId == userId && up.PermissionsId != null)
                    .Select(up => up.PermissionsId.Value)
                    .ToList();

                return userPermissions;
            }
        }

        /// <summary>
        /// Retrieves the user based on their ID.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <returns>The user entity if found, otherwise null.</returns>
        public static User GetUserById(Guid id)
        {
            using (var context = new WarehouseDatabaseEntities())
            {
                return context.User.FirstOrDefault(u => u.Id == id && !u.IsForgotten);
            }
        }

        /// <summary>
        /// Validates the provided email for the user with the given ID.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <param name="email">The email provided for recovery.</param>
        /// <returns>True if the provided email matches the user's email, otherwise false.</returns>
        public static bool ValidateRecoveryEmail(Guid id, string email)
        {
            var user = GetUserById(id);
            return user != null && user.Email == email;
        }

        /// <summary>
        /// Checks if the password recovery status is requested for the user with the specified ID.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>True if the password recovery status is requested, otherwise false.</returns>
        public static bool IsPasswordRecoveryRequested(Guid userId)
        {
            using (var context = new WarehouseDatabaseEntities())
            {
                var user = context.User.FirstOrDefault(u => u.Id == userId && !u.IsForgotten);
                return user.PasswordRecoveryStatus ?? false;
            }
        }

        /// <summary>
        /// Checks whether the new password provided by a user is unique compared to the previous passwords stored in the UserPasswordHistory table for the specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="newPassword">The new password to be checked.</param>
        /// <returns>True if the new password is unique, otherwise false.</returns>
        public static bool IsNewPasswordUnique(Guid userId, string newPassword)
        {
            using (var context = new WarehouseDatabaseEntities())
            {
                var previousPasswords = context.UserPasswordHistory
                    .Where(p => p.UserId == userId)
                    .Select(p => p.Password)
                    .ToList();

                foreach (var password in previousPasswords)
                {
                    if (password == newPassword)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Updates the user's password history with the new password.
        /// </summary>
        /// <param name="userId">The ID of the user whose password history should be updated.</param>
        /// <param name="newPassword">The new password to be added to the user's password history.</param>
        public static void UpdateUserPasswordHistory(Guid userId, string newPassword)
        {
            using (var context = new WarehouseDatabaseEntities())
            {
                var userPasswordHistory = context.UserPasswordHistory
                    .Where(p => p.UserId == userId)
                    .OrderBy(p => p.ChangeDate)
                    .ToList();

                if (userPasswordHistory.Count >= 3)
                {
                    context.Database.ExecuteSqlCommand("DeleteOldestUserPasswordHistory @UserId",
                    new SqlParameter("@UserId", userId));
                }

                context.Database.ExecuteSqlCommand("AddUserPasswordHistory @UserId, @Password, @ChangeDate",
                new SqlParameter("@UserId", userId),
                new SqlParameter("@Password", newPassword),
                new SqlParameter("@ChangeDate", DateTime.Now));

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Deletes all password history records for the specified user ID.
        /// </summary>
        /// <param name="userId">The ID of the user whose password history should be deleted.</param>
        public static void DeleteUserPasswordHistory(Guid userId)
        {
            using (var context = new WarehouseDatabaseEntities())
            {
                var userPasswordHistoryCount = context.UserPasswordHistory.Where(p => p.UserId == userId).Count();

                for (int i = 0; i < userPasswordHistoryCount; i++)
                {
                    context.Database.ExecuteSqlCommand("EXEC DeleteOldestUserPasswordHistory @UserId", new SqlParameter("@UserId", userId));
                }
            }
        }

        /// <summary>
        /// Executes sql procedure to find and modify existing system attributes.
        /// </summary>
        /// <param name="lockTimeValue">Time value which user have to wait until login will be unlocked</param>
        /// <param name="attemptsValue">How many times can user make mistake in password before login will be locked</param>
        public static void SaveSqlAttributeChanges(int lockTimeValue, int attemptsValue)
        {
            using (WarehouseDatabaseEntities context = new WarehouseDatabaseEntities())
            {
                context.Database.ExecuteSqlCommand("UpdateSystemAttribute @Name, @Value",
                    new SqlParameter("Name", "Lock time"),
                    new SqlParameter("Value", lockTimeValue));

                context.Database.ExecuteSqlCommand("UpdateSystemAttribute @Name, @Value",
                    new SqlParameter("Name", "Login attempts"),
                    new SqlParameter("Value", attemptsValue));
            }
        }

        /// <summary>
        /// Validates the input string as a price value and displays detailed error messages for invalid formats.
        /// </summary>
        /// <param name="priceInput">The input string representing the price value.</param>
        public static bool ValidatePrice(string priceInput)
        {
            string errorMessage = string.Empty;
            decimal pricePerUnit;

            if (string.IsNullOrWhiteSpace(priceInput))
            {
                errorMessage = "Enter a value for the price per unit. Price should be in the format (e.g., 10.00 or 0)";
            }
            else if (priceInput.Contains(","))
            {
                errorMessage = "Invalid number format entered. Price per unit should use '.' as the separator.";
            }
            else if (!decimal.TryParse(priceInput, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out pricePerUnit))
            {
                errorMessage = "The value entered is not a number. Price should be in the format (e.g., 10.00 or 0)";
            }
            else if (pricePerUnit < 0)
            {
                errorMessage = "Price per unit cannot be negative.";
            }
            else if (priceInput.Contains("."))
            {
                int decimalSeparatorCount = priceInput.Count(c => c == '.');
                if (decimalSeparatorCount > 1)
                {
                    errorMessage = "Invalid number format entered. Price per unit should be in the format (e.g., 10.00 or 0)";
                }
                else if (priceInput.IndexOf(".") != priceInput.LastIndexOf("."))
                {
                    errorMessage = "Invalid number format entered. Price per unit should be in the format (e.g., 10.00 or 0)";
                }
                else
                {
                    int index = priceInput.IndexOf('.');
                    if (priceInput.Length - index > 3)
                    {
                        errorMessage = "Price per unit should have at most 2 digits after the point.";
                    }
                }
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show(errorMessage, "Validation error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Adds product to database (most of this function is inside of WarehouseControl.xaml
        /// </summary>
        /// <param name="newProduct">New product that is being created</param>
        public static void AddProduct(Products newProduct)
        {
            using (var context = new WarehouseDatabaseEntities())
            {
                context.Products.Add(newProduct);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Adds delivery details to database (most of this function is inside of WarehouseControl.xaml
        /// </summary>
        /// <param name="newDelivery">New delivery that is being created</param>
        public static void AddProductHistory(ProductsHistory newDelivery)
        {
            using (var context = new WarehouseDatabaseEntities())
            {
                context.ProductsHistory.Add(newDelivery);
                context.SaveChanges();
            }
        }
        
        public static string GetUserRole(Guid userId)
        {
            using (var context = new WarehouseDatabaseEntities())
            {
                var user = context.User.FirstOrDefault(u => u.Id == userId);
                return user?.Role ?? string.Empty;
            }
        }
        
        public static void ApplyScheduledVatChanges()
        {
            using (var context = new WarehouseDatabaseEntities())
            {
                DateTime today = DateTime.Today;

                var vatChanges = context.ProductVATChange
                    .Where(c => c.EffectiveDate <= today)
                    .ToList();

                foreach (var vatChange in vatChanges)
                {
                    var product = context.Products.FirstOrDefault(p => p.Id == vatChange.ProductId);
                    if (product != null)
                    {
                        product.IdVAT = vatChange.VatId;
                        context.Products.Attach(product);
                        context.Entry(product).State = EntityState.Modified;
                    }

                    context.ProductVATChange.Remove(vatChange);
                }

                context.SaveChanges();
            }
        }
        public static void ApplyScheduledTypeVatChanges()
        {
            using (var context = new WarehouseDatabaseEntities())
            {
                DateTime today = DateTime.Today;

                var vatChanges = context.ProductTypeVATChange
                    .Where(v => v.EffectiveDate <= today)
                    .ToList();

                foreach (var vatChange in vatChanges)
                {

                    var productsToUpdate = context.Products
                        .Where(p => p.IdType == vatChange.ProductTypeId)
                        .ToList();


                    foreach (var product in productsToUpdate)
                        if (product != null)
                        {
                        product.IdVAT = vatChange.VatId;
                        context.Products.Attach(product);
                        context.Entry(product).State = EntityState.Modified;
                    }


                    context.ProductTypeVATChange.Remove(vatChange);
                }


                context.SaveChanges();
            }
        }

    }

}
