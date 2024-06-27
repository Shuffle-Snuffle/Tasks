using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AvaloniaApplicationPracticeOne.Models;

namespace AvaloniaApplicationPracticeOne;

public partial class RegistrationWindow : Window
{
    private TextBox loginTBox;
    private TextBox passwordTBox;
    private TextBox nameTBox;
    private TextBox surnameTBox;
    private TextBox phonenumberTBox;
    private DatePicker birthdateDPicker;
    
    public RegistrationWindow()
    {
        InitializeComponent();

        loginTBox = this.FindControl<TextBox>("LoginTBox");
        passwordTBox = this.FindControl<TextBox>("PasswordTBox");
        nameTBox = this.FindControl<TextBox>("NameTBox");
        surnameTBox = this.FindControl<TextBox>("SurnameTBox");
        phonenumberTBox = this.FindControl<TextBox>("PhonenumberTBox");
        birthdateDPicker = this.FindControl<DatePicker>("BirthdateDPicker");

#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void RegBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(loginTBox.Text) &&
            !string.IsNullOrWhiteSpace(passwordTBox.Text) &&
            !string.IsNullOrWhiteSpace(nameTBox.Text) &&
            !string.IsNullOrWhiteSpace(surnameTBox.Text) &&
            !string.IsNullOrWhiteSpace(phonenumberTBox.Text) &&
            !string.IsNullOrWhiteSpace(birthdateDPicker.SelectedDate.ToString()))
        {
            var newUser = new User()
            {
                Id = Service.GetDbContext().Users.Max(q=>q.Id) + 1,
                Login = loginTBox.Text,
                Password = passwordTBox.Text,
                Name = nameTBox.Text,
                Surname = surnameTBox.Text,
                PhoneNumber = phonenumberTBox.Text,
                Birthdate = birthdateDPicker.SelectedDate.ToString(),
                IdRole = 1
            };
            Service.GetDbContext().Users.Add(newUser);
            Service.GetDbContext().SaveChanges();
            
            new MainWindow().Show();
            Close();
        }
    }

    private void BackBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        new MainWindow().Show();
        Close();
    }
}