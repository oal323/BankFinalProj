using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;


namespace BankPrgm
{
    public class BankTests
    {
        [Fact]
        public void TestCreateUser()
        {
            // Arrange
            Bank bank = new Bank();
            string name = "John Doe";
            string address = "123 Main Street";
            string contactInformation = "john.doe@example.com";
            string password = "password";
            string userName = "johndoe";

            // Act
            bank.CreateUser(name, address, contactInformation, password, userName);

            // Assert
            Assert.Single(bank._users);
            Assert.Equal(name, bank._users[0].Name);
            Assert.Equal(address, bank._users[0].Address);
            Assert.Equal(contactInformation, bank._users[0].ContactInformation);
            Assert.Empty(bank._users[0].OwnedAccounts);
            Assert.Equal(password, bank._users[0].Password);
            Assert.Equal(userName, bank._users[0].UserName);
        }

        [Fact]
        public void TestLogin()
        {
            // Arrange
            Bank bank = new Bank();
            string name = "John Doe";
            string address = "123 Main Street";
            string contactInformation = "john.doe@example.com";
            string password = "password";
            string userName = "johndoe";
            bank.CreateUser(name, address, contactInformation, password, userName);

            // Act
            bank.Login(userName, password);

            // Assert
            Assert.Equal(name, bank.cUser.Name);
            Assert.Equal(address, bank.cUser.Address);
            Assert.Equal(contactInformation, bank.cUser.ContactInformation);
            Assert.Empty(bank.cUser.OwnedAccounts);
            Assert.Equal(password, bank.cUser.Password);
            Assert.Equal(userName, bank.cUser.UserName);
        }

        [Fact]
        public void TestCreateAccount()
        {
            // Arrange
            Bank bank = new Bank();
            string name = "John Doe";
            string address = "123 Main Street";
            string contactInformation = "john.doe@example.com";
            string password = "password";
            string userName = "johndoe";
            bank.CreateUser(name, address, contactInformation, password, userName);
            bank.Login(userName, password);
            decimal balance = 5000;
            AccountType type = AccountType.Checking;

            // Act
            bank.CreateAccount(balance, type);
        }
        [Fact]
        public void TestSelectAccount()
        {
            // Arrange
            Bank bank = new Bank();
            string name = "John Doe";
            string address = "123 Main Street";
            string contactInformation = "john.doe@example.com";
            string password = "password";
            string userName = "johndoe";
            bank.CreateUser(name, address, contactInformation, password, userName);
            bank.Login(userName, password);
            decimal balance = 5000;
            AccountType type = AccountType.Checking;
            bank.CreateAccount(balance, type);

            // Act
            bank.SelectAccount(1);

            // Assert
            Assert.Equal(1, bank.cUser.OwnedAccounts.Count);
            Assert.Equal(balance, bank.cAccount.Balance);
            Assert.Equal(type, bank.cAccount.Type);
            Assert.Equal(bank.cUser.Id, bank.cAccount.UserId);
            Assert.Equal(bank.cUser, bank.cAccount.owner);
        }

        [Fact]
        public void TestWithdrawl()
        {
            // Arrange
            Bank bank = new Bank();
            string name = "John Doe";
            string address = "123 Main Street";
            string contactInformation = "john.doe@example.com";
            string password = "password";
            string userName = "johndoe";
            bank.CreateUser(name, address, contactInformation, password, userName);
            bank.Login(userName, password);
            decimal balance = 5000;
            AccountType type = AccountType.Checking;
            bank.CreateAccount(balance, type);
            bank.SelectAccount(1);
            decimal amount = 2500;
            TransactionType transactionType = TransactionType.Withdrawal;

            // Act
            bank.ExecuteTransaction(transactionType, bank.cUser.Id, amount);

            // Assert
            Assert.Equal(1, bank.cUser.OwnedAccounts.Count);
            Assert.Equal(2500, bank.cAccount.Balance);
        }
        [Fact]
        public void TestTransaction()
        {
            // Arrange
            Bank bank = new Bank();
            string name = "John Doe";
            string address = "123 Main Street";
            string contactInformation = "john.doe@example.com";
            string password = "password";
            string userName = "johndoe";
            decimal balance = 5000;
            AccountType type = AccountType.Checking;
            bank.CreateUser(name, address, contactInformation, password, userName);
            bank.Login(userName, password);
            bank.CreateAccount(balance, type);
             name = "Jane Doe";
             address = "123 Main Street";
             contactInformation = "john.doe@example.com";
             password = "password";
             userName = "janedoe";
             bank.CreateUser(name, address, contactInformation, password, userName);
            bank.Login(userName, password);
            bank.CreateAccount(balance, type);
            bank.SelectAccount(1);
            decimal amount = 2500;
            TransactionType transactionType = TransactionType.Transfer;

            // Act
            bank.ExecuteTransaction(transactionType, bank.cUser.Id, amount,"johndoe");

            // Assert
            Assert.Equal(1, bank.cUser.OwnedAccounts.Count);
            Assert.Equal(2500, bank.cAccount.Balance);
            bank.Login("johndoe", password);
            bank.SelectAccount(1);
            Assert.Equal(7500, bank.cAccount.Balance);
        }
        public void TestOverDraft()
        {
            Bank bank = new Bank();
            string name = "John Doe";
            string address = "123 Main Street";
            string contactInformation = "john.doe@example.com";
            string password = "password";
            string userName = "johndoe";
            bank.CreateUser(name, address, contactInformation, password, userName);
            bank.Login(userName, password);
            decimal balance = 5000;
            AccountType type = AccountType.Checking;
            bank.CreateAccount(balance, type);
            bank.SelectAccount(1);
            decimal amount = 6000;
            TransactionType transactionType = TransactionType.Withdrawal;

            // Act
            try {
                
                bank.ExecuteTransaction(transactionType, bank.cUser.Id, amount);
                Assert.Fail("Exception Was not thrown");
            }catch (FormatException){}
            // Assert
            Assert.Equal(1, bank.cUser.OwnedAccounts.Count);
            
            Assert.Equal(5000, bank.cAccount.Balance);

        }
    }
}
