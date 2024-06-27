using System;
using System.Collections.Generic;
using System.Linq;
using Npgsql;

namespace DiaryApp
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class Task
    {
        public int TaskId { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
    }

    public class Program
    {
        private static string connectionString = "Host=localhost;Username=postgres;Password=ShadowTen2!;Database=diary";
        private static User LoggedInUser = null;

        static void Main()
        {
            while (true)
            {
                if (LoggedInUser == null)
                {
                    Console.WriteLine("1. Register");
                    Console.WriteLine("2. Login");
                    Console.WriteLine("3. Exit\n");

                    var choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            Register();
                            break;
                        case "2":
                            Login();
                            break;
                        case "3":
                            Exit();
                            break;
                        default:
                            Console.WriteLine("\nInvalid choice\n");
                            Console.WriteLine("Input any key to continue\n");
                            Console.ReadKey();
                            Console.Clear();
                            break;
                    }
                }
                else
                {
                    ShowMenu();
                }
            }
        }

        private static void Register()
        {
            try
            {
                Console.WriteLine("Enter username:");
                var username = Console.ReadLine();
                Console.WriteLine("Enter password:");
                var password = Console.ReadLine();

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    string checkUserQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
                    using (var command = new NpgsqlCommand(checkUserQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        long userCount = (long)command.ExecuteScalar();
                        if (userCount > 0)
                        {
                            Console.WriteLine("Username already exists.");
                            return;
                        }
                    }

                    string insertUserQuery = "INSERT INTO Users (Username, Password) VALUES (@Username, @Password)";
                    using (var command = new NpgsqlCommand(insertUserQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Password", password);
                        command.ExecuteNonQuery();
                    }
                    Console.WriteLine("\nUser registered successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nAn error occurred: {ex.Message}");
            }
        }

        private static void Login()
        {
            try
            {
                Console.WriteLine("Enter username:");
                var username = Console.ReadLine();
                Console.WriteLine("Enter password:");
                var password = Console.ReadLine();

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    string loginQuery = "SELECT UserId, Username, Password FROM Users WHERE Username = @Username AND Password = @Password";
                    using (var command = new NpgsqlCommand(loginQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Password", password);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                LoggedInUser = new User
                                {
                                    UserId = reader.GetInt32(0),
                                    Username = reader.GetString(1),
                                    Password = reader.GetString(2)
                                };
                                Console.WriteLine("Logged in successfully.\n");
                            }
                            else
                            {
                                Console.WriteLine("Invalid username or password.\n");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nAn error occurred: {ex.Message}");
            }
        }

        private static void ShowMenu()
        {
            Console.WriteLine("1. Add Task");
            Console.WriteLine("2. Edit Task");
            Console.WriteLine("3. Delete Task");
            Console.WriteLine("4. View Tasks for Today");
            Console.WriteLine("5. View Tasks for Tomorrow");
            Console.WriteLine("6. View Tasks for the Week");
            Console.WriteLine("7. View All Tasks");
            Console.WriteLine("8. View Pending Tasks");
            Console.WriteLine("9. View Overdue Tasks");
            Console.WriteLine("10. Logout");
            Console.WriteLine("11. Exit");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddTask();
                    break;
                case "2":
                    EditTask();
                    break;
                case "3":
                    DeleteTask();
                    break;
                case "4":
                    ViewTasksForToday();
                    break;
                case "5":
                    ViewTasksForTomorrow();
                    break;
                case "6":
                    ViewTasksForTheWeek();
                    break;
                case "7":
                    ViewAllTasks();
                    break;
                case "8":
                    ViewPendingTasks();
                    break;
                case "9":
                    ViewOverdueTasks();
                    break;
                case "10":
                    Logout();
                    break;
                case "11":
                    Exit();
                    break;
                default:
                    Console.WriteLine("\nInvalid choice");
                    break;
            }
        }

        private static void AddTask()
        {
            try
            {
                Console.WriteLine("\nEnter task title:");
                var title = Console.ReadLine();
                Console.WriteLine("\nEnter task description:");
                var description = Console.ReadLine();
                Console.WriteLine("\nEnter due date (yyyy-MM-dd):");
                var dueDate = DateTime.Parse(Console.ReadLine());

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    string insertTaskQuery = "INSERT INTO Tasks (UserId, Title, Description, DueDate) VALUES (@UserId, @Title, @Description, @DueDate)";
                    using (var command = new NpgsqlCommand(insertTaskQuery, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", LoggedInUser.UserId);
                        command.Parameters.AddWithValue("@Title", title);
                        command.Parameters.AddWithValue("@Description", description);
                        command.Parameters.AddWithValue("@DueDate", dueDate);
                        command.ExecuteNonQuery();
                    }
                    Console.WriteLine("\nTask added successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nAn error occurred: {ex.Message}");
            }
        }

        private static void EditTask()
        {
            try
            {
                Console.WriteLine("\nEnter task ID to edit:");
                var taskId = int.Parse(Console.ReadLine());

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    string selectTaskQuery = "SELECT TaskId, Title, Description, DueDate FROM Tasks WHERE TaskId = @TaskId AND UserId = @UserId";
                    using (var command = new NpgsqlCommand(selectTaskQuery, connection))
                    {
                        command.Parameters.AddWithValue("@TaskId", taskId);
                        command.Parameters.AddWithValue("@UserId", LoggedInUser.UserId);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var task = new Task
                                {
                                    TaskId = reader.GetInt32(0),
                                    Title = reader.GetString(1),
                                    Description = reader.GetString(2),
                                    DueDate = reader.GetDateTime(3)
                                };

                                Console.WriteLine($"Current Title: {task.Title}");
                                Console.WriteLine("Enter new task title (or press Enter to keep current):");
                                var newTitle = Console.ReadLine();
                                if (!string.IsNullOrEmpty(newTitle))
                                {
                                    task.Title = newTitle;
                                }

                                Console.WriteLine($"Current Description: {task.Description}");
                                Console.WriteLine("Enter new task description (or press Enter to keep current):");
                                var newDescription = Console.ReadLine();
                                if (!string.IsNullOrEmpty(newDescription))
                                {
                                    task.Description = newDescription;
                                }

                                Console.WriteLine($"Current Due Date: {task.DueDate:yyyy-MM-dd}");
                                Console.WriteLine("Enter new due date (yyyy-MM-dd) (or press Enter to keep current):");
                                var newDueDateInput = Console.ReadLine();
                                if (!string.IsNullOrEmpty(newDueDateInput))
                                {
                                    task.DueDate = DateTime.Parse(newDueDateInput);
                                }

                                reader.Close();

                                string updateTaskQuery = "UPDATE Tasks SET Title = @Title, Description = @Description, DueDate = @DueDate WHERE TaskId = @TaskId";
                                using (var updateCommand = new NpgsqlCommand(updateTaskQuery, connection))
                                {
                                    updateCommand.Parameters.AddWithValue("@Title", task.Title);
                                    updateCommand.Parameters.AddWithValue("@Description", task.Description);
                                    updateCommand.Parameters.AddWithValue("@DueDate", task.DueDate);
                                    updateCommand.Parameters.AddWithValue("@TaskId", task.TaskId);
                                    updateCommand.ExecuteNonQuery();
                                }
                                Console.WriteLine("\nTask updated successfully.");
                            }
                            else
                            {
                                Console.WriteLine("\nTask not found.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nAn error occurred: {ex.Message}");
            }
        }

        private static void DeleteTask()
        {
            try
            {
                Console.WriteLine("\nEnter task ID to delete:");
                var taskId = int.Parse(Console.ReadLine());

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    string deleteTaskQuery = "DELETE FROM Tasks WHERE TaskId = @TaskId AND UserId = @UserId";
                    using (var command = new NpgsqlCommand(deleteTaskQuery, connection))
                    {
                        command.Parameters.AddWithValue("@TaskId", taskId);
                        command.Parameters.AddWithValue("@UserId", LoggedInUser.UserId);
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("\nTask deleted successfully.");
                        }
                        else
                        {
                            Console.WriteLine("\nTask not found.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nAn error occurred: {ex.Message}");
            }
        }

        private static void ViewTasksForToday()
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    Console.WriteLine();
                    connection.Open();
                    string selectTasksQuery = "SELECT TaskId, Title, Description, DueDate FROM Tasks WHERE UserId = @UserId AND DueDate::date = CURRENT_DATE";
                    using (var command = new NpgsqlCommand(selectTasksQuery, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", LoggedInUser.UserId);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine($"\n{reader.GetInt32(0)}. {reader.GetString(1)} - {reader.GetString(2)} (Due: {reader.GetDateTime(3)})");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nAn error occurred: {ex.Message}");
            }
        }

        private static void ViewTasksForTomorrow()
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    Console.WriteLine();
                    connection.Open();
                    string selectTasksQuery = "SELECT TaskId, Title, Description, DueDate FROM Tasks WHERE UserId = @UserId AND DueDate::date = CURRENT_DATE + INTERVAL '1 day'";
                    using (var command = new NpgsqlCommand(selectTasksQuery, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", LoggedInUser.UserId);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine($"\n{reader.GetInt32(0)}. {reader.GetString(1)} - {reader.GetString(2)} (Due: {reader.GetDateTime(3)})");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nAn error occurred: {ex.Message}");
            }
        }

        private static void ViewTasksForTheWeek()
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    Console.WriteLine();
                    connection.Open();
                    string selectTasksQuery = "SELECT TaskId, Title, Description, DueDate FROM Tasks WHERE UserId = @UserId AND DueDate::date >= CURRENT_DATE AND DueDate::date <= CURRENT_DATE + INTERVAL '7 days'";
                    using (var command = new NpgsqlCommand(selectTasksQuery, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", LoggedInUser.UserId);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine($"\n{reader.GetInt32(0)}. {reader.GetString(1)} - {reader.GetString(2)} (Due: {reader.GetDateTime(3)})");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nAn error occurred: {ex.Message}");
            }
        }

        private static void ViewAllTasks()
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    Console.WriteLine();
                    connection.Open();
                    string selectTasksQuery = "SELECT TaskId, Title, Description, DueDate FROM Tasks WHERE UserId = @UserId";
                    using (var command = new NpgsqlCommand(selectTasksQuery, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", LoggedInUser.UserId);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine($"\n{reader.GetInt32(0)}. {reader.GetString(1)} - {reader.GetString(2)} (Due: {reader.GetDateTime(3)})");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nAn error occurred: {ex.Message}");
            }
        }

        private static void ViewPendingTasks()
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    Console.WriteLine();
                    connection.Open();
                    string selectTasksQuery = "SELECT TaskId, Title, Description, DueDate FROM Tasks WHERE UserId = @UserId AND DueDate > CURRENT_TIMESTAMP";
                    using (var command = new NpgsqlCommand(selectTasksQuery, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", LoggedInUser.UserId);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine($"\n{reader.GetInt32(0)}. {reader.GetString(1)} - {reader.GetString(2)} (Due: {reader.GetDateTime(3)})");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nAn error occurred: {ex.Message}");
            }
        }

        private static void ViewOverdueTasks()
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    Console.WriteLine();
                    connection.Open();
                    string selectTasksQuery = "SELECT TaskId, Title, Description, DueDate FROM Tasks WHERE UserId = @UserId AND DueDate < CURRENT_TIMESTAMP";
                    using (var command = new NpgsqlCommand(selectTasksQuery, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", LoggedInUser.UserId);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine($"\n{reader.GetInt32(0)}. {reader.GetString(1)} - {reader.GetString(2)} (Due: {reader.GetDateTime(3)})");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nAn error occurred: {ex.Message}");
            }
        }

        private static void Logout()
        {
            try
            {
                LoggedInUser = null;
                Console.WriteLine("\nLogged out successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nAn error occurred: {ex.Message}");
            }
        }

        private static void Exit()
        {
            try
            {
                Console.WriteLine("\nExiting the application. Goodbye!");
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nAn error occurred: {ex.Message}");
            }
        }
    }
}