using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    static List<Transaction> transactions = new List<Transaction>();
    const string LicenseFilePath = "license.txt";
    const string LicenseKey = "CS1DFJDXF545"; // Belirlediğiniz lisans anahtarı

    static void Main(string[] args)
    {
        if (!IsLicenseValid())
        {
            Console.WriteLine("Lisans süresi dolmuş veya geçersiz. Uygulama çalışmayacak.");
            Environment.Exit(0);
        }

        while (true)
        {
            Console.WriteLine("1. Giriş Yap");
            Console.WriteLine("2. Üye Ol");
            Console.WriteLine("3. Çıkış");
            Console.Write("Seçiminizi yapın: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    if (Login())
                    {
                        Console.WriteLine("Giriş başarılı!");
                        ShowMenu(); // Giriş başarılıysa menüyü göster
                    }
                    else
                    {
                        Console.WriteLine("Giriş başarısız! Kullanıcı adı veya parola hatalı.");
                    }
                    break;
                case "2":
                    SignUp();
                    break;
                case "3":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Geçersiz seçenek!");
                    break;
            }
        }
    }

    static void ShowMenu()
    {
        LoadTransactions(); // Önceki işlemleri yükle
        while (true)
        {
            Console.WriteLine("1. Gelir Ekle");
            Console.WriteLine("2. Gider Ekle");
            Console.WriteLine("3. Tüm İşlemleri Listele");
            Console.WriteLine("4. Çıkış");
            Console.Write("Seçiminizi yapın: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddTransaction(true);
                    break;
                case "2":
                    AddTransaction(false);
                    break;
                case "3":
                    ListTransactions();
                    break;
                case "4":
                    SaveTransactions(); // Programdan çıkarken işlemleri kaydet
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Geçersiz seçenek!");
                    break;
            }
        }
    }

    static void SignUp()
    {
        while (true)
        {
            Console.Write("Yeni kullanıcı adı: ");
            string newUsername = Console.ReadLine();
            Console.Write("Yeni parola: ");
            string newPassword = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(newUsername) || string.IsNullOrWhiteSpace(newPassword))
            {
                Console.WriteLine("Kullanıcı adı ve parola boş olamaz. Lütfen tekrar deneyin.");
                continue;
            }

            // Aynı kullanıcı adı kontrolü
            if (IsUsernameTaken(newUsername))
            {
                Console.WriteLine("Bu kullanıcı adı zaten alınmış. Lütfen başka bir kullanıcı adı seçin.");
                continue;
            }

            string userData = $"{newUsername},{newPassword}";
            File.AppendAllText("users.txt", userData + Environment.NewLine);
            Console.WriteLine("Üyelik başarıyla oluşturuldu. Lütfen giriş yapın.");
            break;
        }
    }

    static bool IsUsernameTaken(string username)
    {
        if (!File.Exists("users.txt"))
        {
            return false;
        }

        string[] users = File.ReadAllLines("users.txt");
        foreach (string user in users)
        {
            string[] parts = user.Split(',');
            if (parts[0] == username)
            {
                return true;
            }
        }
        return false;
    }

    static bool Login()
    {
        while (true)
        {
            Console.Write("Kullanıcı Adı: ");
            string username = Console.ReadLine();
            Console.Write("Parola: ");
            string password = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("Kullanıcı adı ve parola boş olamaz. Lütfen tekrar deneyin.");
                continue;
            }

            if (ValidateUser(username, password))
            {
                return true;
            }
            else
            {
                Console.WriteLine("Kullanıcı adı veya parola hatalı. Tekrar denemek ister misiniz? (E/H): ");
                string retry = Console.ReadLine().ToUpper();
                if (retry != "E")
                {
                    Console.WriteLine("Program kapatılıyor...");
                    Environment.Exit(0);
                }
            }
        }
    }

    static bool ValidateUser(string username, string password)
    {
        string[] users = File.ReadAllLines("users.txt");
        foreach (string user in users)
        {
            string[] parts = user.Split(',');
            if (parts.Length == 2 && parts[0] == username && parts[1] == password)
            {
                return true;
            }
        }
        return false;
    }

    static void AddTransaction(bool isIncome)
    {
        while (true)
        {
            Console.Write("Açıklama: ");
            string description = Console.ReadLine();
            decimal amount;
            while (true)
            {
                Console.Write("Tutar: ");
                if (!decimal.TryParse(Console.ReadLine(), out amount))
                {
                    Console.WriteLine("Geçersiz tutar. Lütfen tekrar girin.");
                    continue;
                }
                break;
            }
            DateTime date = DateTime.Now;

            Transaction newTransaction = new Transaction
            {
                Description = description,
                Amount = isIncome ? Math.Abs(amount) : -Math.Abs(amount),
                Date = date
            };

            transactions.Add(newTransaction);
            SaveTransactions();
            Console.WriteLine("İşlem başarıyla eklendi.");

            Console.Write("Başka bir işlem yapmak ister misiniz? (E/H): ");
            string continueChoice = Console.ReadLine().ToUpper();
            if (continueChoice == "H")
            {
                Console.WriteLine("Program kapatılıyor...");
                Environment.Exit(0);
            }
            break; // Doğru giriş yapıldığında döngüyü kır
        }
    }

    static void SaveTransactions()
    {
        using (StreamWriter writer = new StreamWriter("transactions.txt"))
        {
            foreach (var transaction in transactions)
            {
                writer.WriteLine($"{transaction.Description},{transaction.Amount},{transaction.Date}");
            }
        }
    }

    static void ListTransactions()
    {
        Console.WriteLine("Tüm İşlemler:");
        foreach (var transaction in transactions)
        {
            Console.WriteLine($"Tarih: {transaction.Date}, Açıklama: {transaction.Description}, Tutar: {transaction.Amount}");
        }
    }

    static void LoadTransactions()
    {
        if (File.Exists("transactions.txt"))
        {
            string[] lines = File.ReadAllLines("transactions.txt");
            foreach (string line in lines)
            {
                string[] parts = line.Split(',');
                Transaction transaction = new Transaction
                {
                    Description = parts[0],
                    Amount = decimal.Parse(parts[1]),
                    Date = DateTime.Parse(parts[2])
                };
                transactions.Add(transaction);
            }
        }
    }

    // Lisans Anahtarını ve Geçerlilik Tarihini Kontrol Eden Metod
    static bool IsLicenseValid()
    {
        if (!File.Exists(LicenseFilePath))
        {
            Console.Write("Lisans anahtarını girin: ");
            string inputLicenseKey = Console.ReadLine();

            if (inputLicenseKey == LicenseKey)
            {
                File.WriteAllText(LicenseFilePath, $"{LicenseKey},{DateTime.Now}");
                return true;
            }
            else
            {
                Console.WriteLine("Geçersiz lisans anahtarı.");
                return false;
            }
        }
        else
        {
            string[] licenseData = File.ReadAllText(LicenseFilePath).Split(',');

            if (licenseData.Length == 2 && licenseData[0] == LicenseKey)
            {
                DateTime licenseDate = DateTime.Parse(licenseData[1]);
                if (DateTime.Now < licenseDate.AddYears(1))
                {
                    return true;
                }
                else
                {
                    Console.WriteLine("Lisans süresi dolmuş.");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Geçersiz lisans anahtarı.");
                return false;
            }
        }
    }
}

class Transaction
{
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}
