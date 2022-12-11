using System;
using System.Collections.Generic;
using System.Text;

namespace BankPrgm
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string ContactInformation { get; set; }
        public List<Account> OwnedAccounts { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }

        public User()
        {
            Id = Guid.NewGuid();
            OwnedAccounts = new List<Account>();
        }
    }

    public class Account
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public decimal Balance { get; set; }
        public AccountType Type { get; set; }
        public User owner { get; set; }

        public Account()
        {
            Id = Guid.NewGuid();
        }
    }

    public enum AccountType
    {
        Checking,
        Savings
    }

    public class Transaction
    {
        public Guid Id { get; set; }
        public TransactionType Type { get; set; }
        public Guid Destination { get; set; }
        public Guid Source { get; set; }
        public decimal Amount { get; set; }

        public Transaction()
        {
            Id = Guid.NewGuid();
        }
    }

    public enum TransactionType
    {
        Deposit,
        Withdrawal,
        Transfer,
        Purchase
    }
// handles main logic of the bank including console menue
    public class Bank
    {
        public readonly List<User> _users = new List<User>();
        public User cUser = new User();
        public Account cAccount = new Account();
        private Guid emptGuid = Guid.Empty;
        public static void throwError(string message)
        {
            throw new FormatException(message);
        }
        public void CreateUser(string Name, string Address, string ContactInformation, string Password, string UserName)

        {
            User temp = new User();
            temp = QueryUser(UserName);
            if (_users.Count == 0 || temp == null)
            {
                User newUser = new User();
                newUser.Address = Address;
                newUser.Name = Name;
                newUser.ContactInformation = ContactInformation;
                newUser.OwnedAccounts = new List<Account>(); ;
                newUser.Password = Password;
                newUser.UserName = UserName;
                _users.Add(newUser);
                
            }
            else
            {
                //username taken
                System.Console.WriteLine("Enter a unique user name");
            }
        }
        //selects account based on order of creation
        public void SelectAccount(int selection)
        {
            try
            {
                cAccount = cUser.OwnedAccounts[selection - 1];
            }
            catch (ArgumentOutOfRangeException e)
            {
                System.Console.WriteLine("Error select a correct account");
                return;
            }

        }
        //Logins the user by chaning the cUser var
        public void Login(string UserName, string Password)
        {
            User user = QueryUser(UserName);
            if (user != null)
            {
                if (user.Password == Password)
                {
                    Console.WriteLine("Welcome " + user.Name);
                    cUser = user;
                    return;
                }
                else
                {
                    throwError("Wrong Pass");
                }
            }
            else
            {
                throwError("Wrong username");
            }
        }
        // creates a new account object 
        public void CreateAccount(decimal Balance, AccountType Type)
        {
            Account newAccount = new Account();
            User user = _users.Find(x => x.Id == cUser.Id);
            if (user != null)
            {
                newAccount.UserId = cUser.Id;
                newAccount.Balance = Balance;
                newAccount.Type = Type;
                newAccount.owner = user;
                user.OwnedAccounts.Add(newAccount);
            }
            else
            {
                throwError("Error user doesnt exist");
            }
        }
        //executes one of 4 transaction types. Purchase is basically the same as throwing money away.
        public void ExecuteTransaction(TransactionType Type, Guid Destination, decimal Amount, string destUsername = "")
        {
            Transaction _Transaction = new Transaction();
            Account destinationAccount = new Account();

            if (cAccount == null)
            {
                throwError("Error select an account");
                return;
            }
            switch (Type)
            {
                case TransactionType.Deposit:
                    _Transaction.Destination = cAccount.Id;
                    cAccount.Balance += Amount;
                    break;
                case TransactionType.Withdrawal:
                    _Transaction.Destination = cAccount.Id;
                    if (cAccount.Balance - Amount > 0)
                    {
                        cAccount.Balance -= Amount;
                    }
                    else
                    {
                        throwError("Error overdraft");
                        Thread.Sleep(1000);
                    }
                    break;
                case TransactionType.Transfer:
                    if (destUsername != "")
                    {
                        User destUser = QueryUser(destUsername);
                        if (destUser == null)
                        {
                            throwError("Error username doesnt exist");
                            return;
                        }
                        destinationAccount = destUser.OwnedAccounts.First();
                        if (destinationAccount == null)
                        {
                            throwError("Error user has no accounts");
                            return;
                        }
                        if (cAccount.Balance - Amount > 0)
                        {
                            destinationAccount.Balance += Amount;
                            cAccount.Balance -= Amount;
                        }
                        else
                        {
                            throwError("Error overdraft");
                            Thread.Sleep(1000);
                        }

                    }
                    else
                    {
                        destinationAccount = cUser.OwnedAccounts.Find(x => x.Id == Destination);

                        if (destinationAccount == null)
                        {
                            throwError("Error wrong account");
                            return;
                        }
                        if (destinationAccount == cAccount)
                        {
                            throwError("Error same account");
                        }
                        _Transaction.Destination = destinationAccount.Id;
                        if (cAccount.Balance - Amount > 0)
                        {
                            destinationAccount.Balance += Amount;
                            cAccount.Balance -= Amount;
                        }
                        else
                        {
                            throwError("Error overdraft");
                            Thread.Sleep(1000);
                        }

                    }



                    break;
                case TransactionType.Purchase:
                    if (cAccount.Balance - Amount > 0)
                    {
                        System.Console.WriteLine("You just spent $" + Amount);
                        cAccount.Balance -= Amount;
                    }
                    else
                    {
                        throwError("Error overdraft");
                    }
                    break;
                default:
                    throwError("Invalid");
                    break;
            }


        }
        //returns user based on username
        public User QueryUser(string UserName)
        {
            User retUser = new User();
            retUser = _users.Find(x => x.UserName == UserName);
            return retUser;
        }
        //returns user based on guid
        public User QueryUser(Guid Id)
        {
            User retUser = new User();
            retUser = _users.Find(x => x.Id == Id);
            return retUser;
        }
        //returns all user accounts based on user object
        public List<Account> QueryAccounts(User user)
        {
            List<Account> accounts = new List<Account>();
            accounts = user.OwnedAccounts;
            return accounts;
        }
        //returns all user accounts based on guid
        public List<Account> QueryAccounts(Guid userId)
        {
            List<Account> accounts = new List<Account>();
            User user = new User();
            user = QueryUser(userId);
            accounts = user.OwnedAccounts;
            return accounts;
        }

        class Program
        {


            static Bank _Bank = new Bank();
            static User cUser = _Bank.cUser;
            static void Main(string[] args)
            {
                /* new BankTests().TestCreateUser();
                new BankTests().TestCreateAccount();
                new BankTests().TestLogin();
                new BankTests().TestSelectAccount();
                new BankTests().TestTransaction();
                new BankTests().TestOverDraft();
                new BankTests().TestWithdrawl(); */
                
                while (MainMenu()) ;
            }
            private static void printInfo()
            {
                int i = 1;
                foreach (var account in cUser.OwnedAccounts)
                {
                    var data = new StringBuilder();
                    data.AppendLine($"Balance: {Decimal.Round(account.Balance)}");
                    data.AppendLine($"Account ID: {account.Id}");
                    data.AppendLine($"User IDs Name: {account.UserId}");
                    data.AppendLine($"User Name: {account.owner.Name}");
                    data.AppendLine($"Acct Num: {i}");
                    Console.WriteLine(data.ToString());
                    i++;
                }
            }
            public static void checkInput(string input)
            {
                if (input.Trim() == "")
                {
                    Bank.throwError("Error Empty Input");
                }

            }
            private static bool MainMenu()
            {
                try
                {
                    if (cUser.Name == null)
                    {
                        Console.Clear();
                        Console.WriteLine("Choose an option:");
                        Console.WriteLine("1) Create Account");
                        Console.WriteLine("2) Sign In");
                        Console.WriteLine("3) Exit");
                        Console.Write("\r\nSelect an option: ");
                        switch (Console.ReadLine())
                        {
                            case "1":
                                string name = "";
                                string address = "";
                                string contactInformation = "";
                                string password = "";
                                string userName = "";

                                Console.Write("Enter your name: ");
                                name = Console.ReadLine();
                                checkInput(name);
                                Console.Write("Enter your contact information: ");
                                contactInformation = Console.ReadLine();
                                checkInput(contactInformation);
                                Console.Write("Enter your address: ");
                                address = Console.ReadLine();
                                checkInput(address);
                                Console.Write("Enter your username: ");
                                userName = Console.ReadLine();
                                checkInput(userName);
                                Console.Write("Enter your password: ");
                                password = "password"; Console.ReadLine();
                                checkInput(password);
                                _Bank.CreateUser(name, address, contactInformation, password, userName);
                                _Bank.Login(userName, password);
                                cUser = _Bank.cUser;
                                return true;
                            case "2":
                                Console.Write("Enter your username: ");
                                userName = Console.ReadLine();
                                Console.Write("Enter your password: ");
                                password = Console.ReadLine();
                                _Bank.Login(userName, password);
                                return true;
                            case "3":
                                return false;
                            default:
                                return true;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Press 1 to print account info 2 to add account 3 to move the moners 4 to select account 5 to exit");
                        switch (Console.ReadLine())
                        {
                            case "1":
                                printInfo();
                                return true;
                            case "2":
                                System.Console.WriteLine("Enter Account Type: 1 for Checking, 2 for Savings");
                                AccountType acctType = new AccountType();
                                decimal initBal;
                                switch (Console.ReadLine())
                                {
                                    case "1":
                                        Console.WriteLine("Enter Starting Balance");
                                        //input = //Console.ReadLine();
                                        initBal = 1500;//Decimal.Parse(input);
                                        Console.WriteLine("Enter Account Type");
                                        //input = Console.ReadLine();
                                        acctType = AccountType.Checking;

                                        _Bank.CreateAccount(initBal, acctType);
                                        break;
                                    case "2":
                                        Console.WriteLine("Enter Starting Balance");
                                        //input = //Console.ReadLine();
                                        initBal = 1500;//Decimal.Parse(input);
                                        Console.WriteLine("Enter Account Type");
                                        //input = Console.ReadLine();
                                        acctType = AccountType.Savings;
                                        break;
                                }
                                return true;
                            case "3":
                                TransactionType _type = new TransactionType();
                                System.Console.WriteLine("Enter Transacton Type: 1 for deposit, 2 for withdrawl, 3 for transfer between 2 of your accounts, 4 to send to account you dont own");
                                switch (Console.ReadLine())
                                {
                                    case "1":
                                        _type = TransactionType.Deposit;
                                        System.Console.WriteLine("Enter Amount to Deposit");
                                        decimal amount;
                                        try
                                        {
                                            amount = decimal.Parse(Console.ReadLine());
                                        }
                                        catch (Exception e)
                                        {
                                            System.Console.WriteLine("error enter a num");
                                            return true;
                                        }
                                        _Bank.ExecuteTransaction(_type, cUser.Id, amount);
                                        break;
                                    case "2":
                                        _type = TransactionType.Withdrawal;
                                        System.Console.WriteLine("Enter Amount to Withdraw");
                                        try
                                        {
                                            amount = decimal.Parse(Console.ReadLine());
                                        }
                                        catch (Exception e)
                                        {
                                            System.Console.WriteLine("error enter a num");
                                            return true;
                                        }
                                        _Bank.ExecuteTransaction(_type, cUser.Id, amount);
                                        break;
                                    case "3":
                                        _type = TransactionType.Transfer;
                                        Account DestAccount = null;
                                        System.Console.WriteLine("Enter Dest Num");
                                        try
                                        {
                                            DestAccount = cUser.OwnedAccounts[int.Parse(Console.ReadLine()) - 1];
                                        }
                                        catch (Exception ArgumentOutOfRangeException)
                                        {
                                            System.Console.WriteLine("Error you don't own this account");
                                            break;
                                        }
                                        System.Console.WriteLine("Enter Amount to Transfer");
                                        try
                                        {
                                            amount = decimal.Parse(Console.ReadLine());
                                        }
                                        catch (Exception e)
                                        {
                                            System.Console.WriteLine("error enter a num");
                                            return true;
                                        }
                                        _Bank.ExecuteTransaction(_type, DestAccount.Id, amount);
                                        break;
                                    case "4":
                                        _type = TransactionType.Purchase;
                                        break;
                                    default:
                                        System.Console.WriteLine("write a valid num");
                                        return true;
                                }
                                return true;
                            case "4":
                                System.Console.WriteLine("Select account based on account num");
                                try
                                {
                                    _Bank.SelectAccount(int.Parse(Console.ReadLine()));
                                }
                                catch (Exception e)
                                {
                                    System.Console.WriteLine("Error enter a number");
                                    return true;
                                }
                                return true;

                            case "5":
                                cUser = new User();
                                return true;
                            default:
                                return true;

                        }
                    }
                }
                catch (FormatException f)
                {
                    System.Console.WriteLine(f.Message);
                    Thread.Sleep(1000);
                    return true;
                }
            }
        }
    }
}