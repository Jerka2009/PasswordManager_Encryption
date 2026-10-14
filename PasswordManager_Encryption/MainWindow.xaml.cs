using System.Collections.ObjectModel;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace PasswordManager_Encryption
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string Password_Path = "Passwords.mnd";
        byte[] salt = new byte[16];
        int HashLength = 36;
        Dictionary<string, byte[]> passwordsListt = new();
        Collection<string> passwords = new();

        void LoadPasswords()
        {
            if (File.Exists(Password_Path))
            {
                try
                {
                    var file_t = File.ReadAllText(Password_Path);
                    Collection<PasswordEntry> passwordsSource = JsonSerializer.Deserialize<Collection<PasswordEntry>>(file_t);
                    foreach (PasswordEntry pe in passwordsSource)
                    {
                        passwords.Add(pe.Password);
                        passwordsListt.Add(pe.Password, pe.Hash.ToArray<byte>());
                    }
                    PasswordList.ItemsSource = passwords;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            else
            {
                using (StreamWriter sw = new StreamWriter(Password_Path))
                {
                    sw.Write("""
                        []
                        """);
                }
                try
                {
                    var file_t = File.ReadAllText(Password_Path);
                    Collection<PasswordEntry> passwordsSource = JsonSerializer.Deserialize<Collection<PasswordEntry>>(file_t);
                    foreach (PasswordEntry pe in passwordsSource)
                    {
                        passwords.Add(pe.Password);
                        passwordsListt.Add(pe.Password, pe.Hash);
                    }
                    PasswordList.ItemsSource = passwords;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

        void SaveJson()
        {
            try
            {
                Collection<PasswordEntry> passwordCollection = new Collection<PasswordEntry>(
                    passwordsListt.Select(item => new PasswordEntry
                    {
                        Password = item.Key,
                        Hash = item.Value
                    }).ToList()
                );
                var jsonParse = JsonSerializer.Serialize<Collection<PasswordEntry>>(passwordCollection);
                File.WriteAllText(Password_Path, jsonParse);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        void UpdateList()
        {
            passwords.Clear();
            foreach (var samp in passwordsListt.Keys)
            {
                passwords.Add(samp);
            }
            PasswordList.ItemsSource = null;
            PasswordList.ItemsSource = passwords;
        }

        public MainWindow()
        {
            InitializeComponent();

            LoadPasswords();
        }

        private void OnItemSelected(object sender, RoutedEventArgs e)
        {
            if (PasswordList.SelectedItem != null)
            {
                Login_Box.Text = PasswordList.SelectedItem.ToString();
                string pass_heash = string.Empty;
                passwordsListt.TryGetValue(PasswordList.SelectedItem.ToString(), out byte[] hash_pas);
                using (Aes aesAlg = Aes.Create())
                {
                    pass_heash = EncryptionService.Decrypt(hash_pas);
                }
                Password_Box.Text = pass_heash;
            }
        }

        private void OnSave_click(object sender, RoutedEventArgs e)
        {
            if (Login_Box.Text != null && Login_Box.Text != string.Empty)
            {
                if (Password_Box.Text != null && Password_Box.Text != string.Empty)
                {
                    var EncryptedPass = EncryptionService.Encrypt(Password_Box.Text);
                    passwordsListt.TryGetValue(Login_Box.Text, out byte[] hash);
                    if (hash == null)
                    {
                        passwordsListt.Add(Login_Box.Text, EncryptedPass);
                        UpdateList();
                    }
                    else
                    {
                        if (hash != EncryptedPass)
                        {
                            foreach (var h in passwordsListt)
                            {
                                if (h.Key == Login_Box.Text)
                                {
                                    passwordsListt[h.Key] = EncryptedPass;
                                    UpdateList();
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }

        private void OnRemove_click(object sender, RoutedEventArgs e)
        {
            if (Login_Box != null)
            {
                foreach (var key in passwordsListt.Keys)
                {
                    if (key == Login_Box.Text)
                    {
                        passwordsListt.Remove(key);
                        passwords.Remove(key);
                        UpdateList();
                    }
                }
            }
            else
            {
                return;
            }
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveJson();
        }
    }
}