using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using System.ComponentModel.DataAnnotations;

namespace DiaryApp
{
    public class Users
    {
        [Key]
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class Tasks
    {
        [Key]
        public int TaskId { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
    }

    public class DiaryDbContext : DbContext
    {
        public DbSet<Users> Users { get; set; }
        public DbSet<Tasks> Tasks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Username=postgres;Password=ShadowTen2!;Database=diary");
        }
    }

    public class Program
    {
        private static DiaryDbContext dbContext = new DiaryDbContext();
        private static Users LoggedInUser = null;

        static void Main()
        {
            while (true)
            {
                if (LoggedInUser == null)
                {
                    Console.WriteLine("1. Регистрация");
                    Console.WriteLine("2. Вход");
                    Console.WriteLine("3. Выход\n");

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
                            Console.WriteLine("\nНеверный выбор\n");
                            Console.WriteLine("Нажмите любую клавишу для продолжения\n");
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
                Console.WriteLine("Введите имя пользователя:");
                var username = Console.ReadLine();
                Console.WriteLine("Введите пароль:");
                var password = Console.ReadLine();

                var existingUser = dbContext.Users.FirstOrDefault(u => u.Username == username);
                if (existingUser != null)
                {
                    Console.WriteLine("Имя пользователя уже существует.");
                    return;
                }

                var newUser = new Users
                {
                    Username = username,
                    Password = password
                };

                dbContext.Users.Add(newUser);
                dbContext.SaveChanges();
                Console.WriteLine("\nПользователь зарегистрирован успешно.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nПроизошла ошибка: {ex.Message}");
            }
        }

        private static void Login()
        {
            try
            {
                Console.WriteLine("Введите имя пользователя:");
                var username = Console.ReadLine();
                Console.WriteLine("Введите пароль:");
                var password = Console.ReadLine();

                var user = dbContext.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
                if (user != null)
                {
                    LoggedInUser = user;
                    Console.WriteLine("Вход выполнен успешно.\n");
                }
                else
                {
                    Console.WriteLine("Неверное имя пользователя или пароль.\n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nПроизошла ошибка: {ex.Message}");
            }
        }

        private static void ShowMenu()
        {
            Console.WriteLine("1. Добавить задачу");
            Console.WriteLine("2. Редактировать задачу");
            Console.WriteLine("3. Удалить задачу");
            Console.WriteLine("4. Просмотр задач на сегодня");
            Console.WriteLine("5. Просмотр задач на завтра");
            Console.WriteLine("6. Просмотр задач на неделю");
            Console.WriteLine("7. Просмотр всех задач");
            Console.WriteLine("8. Просмотр невыполненных задач");
            Console.WriteLine("9. Просмотр просроченных задач");
            Console.WriteLine("10. Выйти из аккаунта");
            Console.WriteLine("11. Выйти из программы");

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
                    Console.WriteLine("\nНеверный выбор");
                    break;
            }
        }

        private static void AddTask()
        {
            try
            {
                Console.WriteLine("\nВведите заголовок задачи:");
                var title = Console.ReadLine();
                Console.WriteLine("\nВведите описание задачи:");
                var description = Console.ReadLine();
                Console.WriteLine("\nВведите срок выполнения (yyyy-MM-dd):");
                var dueDate = DateTime.Parse(Console.ReadLine());

                var newTask = new Tasks
                {
                    UserId = LoggedInUser.UserId,
                    Title = title,
                    Description = description,
                    DueDate = dueDate
                };

                dbContext.Tasks.Add(newTask);
                dbContext.SaveChanges();
                Console.WriteLine("\nЗадача успешно добавлена.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nПроизошла ошибка: {ex.Message}");
            }
        }

        private static void EditTask()
        {
            try
            {
                Console.WriteLine("\nВведите ID задачи для редактирования:");
                var taskId = int.Parse(Console.ReadLine());

                var task = dbContext.Tasks.FirstOrDefault(t => t.TaskId == taskId && t.UserId == LoggedInUser.UserId);
                if (task != null)
                {
                    Console.WriteLine($"Текущий заголовок: {task.Title}");
                    Console.WriteLine("Введите новый заголовок задачи (или нажмите Enter для сохранения текущего):");
                    var newTitle = Console.ReadLine();
                    if (!string.IsNullOrEmpty(newTitle))
                    {
                        task.Title = newTitle;
                    }

                    Console.WriteLine($"Текущее описание: {task.Description}");
                    Console.WriteLine("Введите новое описание задачи (или нажмите Enter для сохранения текущего):");
                    var newDescription = Console.ReadLine();
                    if (!string.IsNullOrEmpty(newDescription))
                    {
                        task.Description = newDescription;
                    }

                    Console.WriteLine($"Текущий срок выполнения: {task.DueDate:yyyy-MM-dd}");
                    Console.WriteLine("Введите новый срок выполнения (yyyy-MM-dd) (или нажмите Enter для сохранения текущего):");
                    var newDueDateInput = Console.ReadLine();
                    if (!string.IsNullOrEmpty(newDueDateInput))
                    {
                        task.DueDate = DateTime.Parse(newDueDateInput);
                    }

                    dbContext.SaveChanges();
                    Console.WriteLine("\nЗадача успешно обновлена.");
                }
                else
                {
                    Console.WriteLine("\nЗадача не найдена.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nПроизошла ошибка: {ex.Message}");
            }
        }

        private static void DeleteTask()
        {
            try
            {
                Console.WriteLine("\nВведите ID задачи для удаления:");
                var taskId = int.Parse(Console.ReadLine());

                var task = dbContext.Tasks.FirstOrDefault(t => t.TaskId == taskId && t.UserId == LoggedInUser.UserId);
                if (task != null)
                {
                    dbContext.Tasks.Remove(task);
                    dbContext.SaveChanges();
                    Console.WriteLine("\nЗадача успешно удалена.");
                }
                else
                {
                    Console.WriteLine("\nЗадача не найдена.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nПроизошла ошибка: {ex.Message}");
            }
        }

        private static void ViewTasksForToday()
{
    try
    {
        var tasksToday = dbContext.Tasks
            .Where(t => t.UserId == LoggedInUser.UserId && t.DueDate.Date == DateTime.Today)
            .ToList();

        Console.WriteLine("\nЗадачи на сегодня:");
        foreach (var task in tasksToday)
        {
            Console.WriteLine($"{task.TaskId}. {task.Title} - {task.Description} (Срок: {task.DueDate})");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\nПроизошла ошибка: {ex.Message}");
    }
}

        private static void ViewTasksForTomorrow()
        {
            try
            {
                var tomorrow = DateTime.Today.AddDays(1);
                var tasksTomorrow = dbContext.Tasks
                    .Where(t => t.UserId == LoggedInUser.UserId && t.DueDate.Date == tomorrow)
                    .ToList();

                Console.WriteLine("\nЗадачи на завтра:");
                foreach (var task in tasksTomorrow)
                {
                    Console.WriteLine($"{task.TaskId}. {task.Title} - {task.Description} (Срок: {task.DueDate})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nПроизошла ошибка: {ex.Message}");
            }
        }

        private static void ViewTasksForTheWeek()
        {
            try
            {
                var today = DateTime.Today;
                var endOfWeek = today.AddDays(7);

                var tasks = dbContext.Tasks
                    .Where(t => t.UserId == LoggedInUser.UserId && t.DueDate >= today && t.DueDate <= endOfWeek)
                    .ToList();

                Console.WriteLine("\nЗадачи на неделю:");
                foreach (var task in tasks)
                {
                    Console.WriteLine($"{task.TaskId}. {task.Title} - {task.Description} (Срок: {task.DueDate})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nПроизошла ошибка: {ex.Message}");
            }
        }

        private static void ViewAllTasks()
        {
            try
            {
                var tasks = dbContext.Tasks
                    .Where(t => t.UserId == LoggedInUser.UserId)
                    .ToList();

                Console.WriteLine("\nВсе задачи:");
                foreach (var task in tasks)
                {
                    Console.WriteLine($"{task.TaskId}. {task.Title} - {task.Description} (Срок: {task.DueDate})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nПроизошла ошибка: {ex.Message}");
            }
        }

        private static void ViewPendingTasks()
        {
            try
            {
                var tasks = dbContext.Tasks
                    .Where(t => t.UserId == LoggedInUser.UserId && t.DueDate >= DateTime.Today)
                    .ToList();

                Console.WriteLine("\nНевыполненные задачи:");
                foreach (var task in tasks)
                {
                    Console.WriteLine($"{task.TaskId}. {task.Title} - {task.Description} (Срок: {task.DueDate})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nПроизошла ошибка: {ex.Message}");
            }
        }

        private static void ViewOverdueTasks()
        {
            try
            {
                var tasks = dbContext.Tasks
                    .Where(t => t.UserId == LoggedInUser.UserId && t.DueDate < DateTime.Today)
                    .ToList();

                Console.WriteLine("\nПросроченные задачи:");
                foreach (var task in tasks)
                {
                    Console.WriteLine($"{task.TaskId}. {task.Title} - {task.Description} (Срок: {task.DueDate})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nПроизошла ошибка: {ex.Message}");
            }
        }

        private static void Logout()
        {
            LoggedInUser = null;
            Console.WriteLine("\nВыход выполнен.\n");
        }

        private static void Exit()
        {
            dbContext.Dispose();
            Environment.Exit(0);
        }
    }
}