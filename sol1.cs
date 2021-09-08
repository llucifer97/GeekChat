

 Sql Server Codes 

Create database TweetApp

use TweetApp

create table Users(UserId int primary key, FirstName varchar(30),LastName varchar(30),Gender varchar(10),DateOfBirth Date,Email varchar(50),Password varchar(30));

create table Tweets(
TweetId int primary key,
UserName varchar(60),
Message varchar(200),
CreateId datetime,
UserId int FOREIGN KEY REFERENCES Users(UserId)
);


TweeterApp.Entities
//Users.cs

using System;

namespace TweeterApp.Entities
{
    public class Users
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

    }
}


//Tweets.cs

using System;


namespace TweeterApp.Entities
{
    class Tweets
    {
        public int TweetId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public DateTime CreateId { get; set; }

    }
}

TweeterApp.CustomException
DuplicateEmailIdException.cs

using System;

namespace TweeterApp.CustomException
{
    public class DuplicateEmailIdException : Exception
    {
        public DuplicateEmailIdException()
        {
        }

        public DuplicateEmailIdException(string message) : base("This Email Id already Exists! Try Logging In")
        {
        }
    }
}

//DataAccessLayer
//UserData.cs

using System;
using System.Data;
using System.Data.SqlClient;
using TweeterApp.CustomException;
using TweeterApp.Entities;

namespace TweeterApp.DataAccessLayer
{
    public class UserData
    {
        static string ConnectionString = "data source=.; database=TweetApp; integrated security=SSPI";
        public static int AddUser(Users user)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    string query = "select count(*) from Users where Email=\'"+user.Email+"\'";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    connection.Open();
                    if ((Int32)cmd.ExecuteScalar() > 0)
                    {
                        throw new DuplicateEmailIdException();
                    }
                    else
                    {
                        query = "insert into Users values(" + user.UserId + ",\'" + user.FirstName + "\',\'" + user.LastName + "\',\'" + user.Gender + "\',\'" + user.DateOfBirth + "\',\'" + user.Email + "\',\'" + user.Password + "\')";
                        cmd = new SqlCommand(query, connection);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        Console.WriteLine("Inserted Rows = " + rowsAffected);
                        return rowsAffected;
                    }
                    
                }

            }
            catch(DuplicateEmailIdException e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }

        }
       
        public static Users CheckUser(string Email,string Password)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    string query = "select count(*) from Users where Email=\'" + Email + "\' and Password=\'"+Password+"\'";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    connection.Open();
                    if ((Int32)cmd.ExecuteScalar() > 0)
                    {
                        return(getUser(Email));
                    }
                    else
                    {
                        return null;
                    }

                }

            }
        
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
        public static Users getUser(String Email)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    string query = "select * from Users where Email=\'" + Email + "\'";
                    connection.Open();
                    SqlDataAdapter da = new SqlDataAdapter(query, connection);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    Users user = new Users();
                    foreach (DataRow row in dt.Rows)
                    {
                        user.UserId = int.Parse(row["UserId"].ToString());
                        user.FirstName = row["FirstName"].ToString();
                        user.LastName = row["LastName"].ToString();
                        user.Gender = row["Gender"].ToString();
                        user.DateOfBirth=Convert.ToDateTime(row["DateOfBirth"].ToString());
                        user.Email = row["Email"].ToString();
                        user.Password = row["Password"].ToString();
                    }
                    return user;
                }

            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}

public static void updatePassword(string pass, Users user)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    string query = "update Users set Password = \'"+pass+"\' where UserId ="+ user.UserId ;
                    SqlCommand cmd = new SqlCommand(query, connection);
                    connection.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected == 1)
                    {
                        Console.WriteLine("Password changed Successfully");
                    }
                    else
                    {
                        Console.WriteLine("Password changing Failed");
                    }
                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("OOPs, something went wrong.\n" + e);
            }
        }






//TweetData.cs

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TweeterApp.Entities;

namespace TweeterApp.DataAccessLayer
{
    public class TweetData
    {
        static string ConnectionString = "data source=.; database=TweetApp; integrated security=SSPI";
        public static void insertTweet(Users user, string message, int tweetId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    string query = "insert into Tweets values(" + tweetId + ",\'" + user.FirstName + user.LastName + "\',\'" + message + "\',\'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\'," + user.UserId + ")";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    connection.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    Console.WriteLine("Uploaded Tweets = " + rowsAffected);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("OOPs, something went wrong.\n" + e);
            }
        }
        public static void removeTweet(int tweetId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    string query = "Delete from Tweets where TweetId = "+tweetId;
                    SqlCommand cmd = new SqlCommand(query, connection);
                    connection.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    Console.WriteLine("Tweets Deleted= " + rowsAffected);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("OOPs, something went wrong.\n" + e);
            }
        }

        public static List<Tweets> getTweets(Users user)
        {
            List<Tweets> tweets = new List<Tweets>();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = "select * from Tweets where UserId=" + user.UserId;
                SqlDataAdapter da = new SqlDataAdapter(query, connection);

                //Using Data Table
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    Tweets t = new Tweets()
                    {
                        TweetId = int.Parse(row["TweetId"].ToString()),
                        Message = row["Message"].ToString(),
                        UserName = row["UserName"].ToString()
                    };
                    tweets.Add(t);
                }
                return tweets;
            }
        }
    }
            

}



//BusinessLogicLayer
//UserAuth.cs

using System;
using TweeterApp.DataAccessLayer;
using TweeterApp.Entities;
using static System.Console;

namespace TweeterApp.BusinessLogicLayer
{
    public class UserAuth
    {
        public static void Register()
        {
            try
            {
                Users user = new Users();
                WriteLine("Register Screen:");
                WriteLine("Enter User ID:");
                user.UserId = int.Parse(ReadLine());
                WriteLine("Enter Firstname:");
                user.FirstName = ReadLine();
                WriteLine("Enter Lastname:");
                user.LastName = ReadLine();
                WriteLine("Enter Gender:");
                user.Gender = ReadLine();
                WriteLine("Enter Date Of Birth(yyyy/mm/dd):");
                user.DateOfBirth = Convert.ToDateTime(ReadLine());
                WriteLine("Enter Email:");
                user.Email = ReadLine();
                WriteLine("Enter password:");
                user.Password = ReadLine();
                WriteLine("Press 1: Submit");
                WriteLine("Press 2: Cancel and Go Back");
                int ch = int.Parse(ReadLine());
                if (ch == 1)
                {
                    int n = UserData.AddUser(user);
                    if (n == 0)
                    {
                        WriteLine("Something Went wrong please try again");
                    }
                    else
                    {
                        WriteLine("You are registered! Try Logging in");
                    }
                }
                else
                {
                    WriteLine("Registration Cancelled");
                    
                }
            }
            catch (Exception e)
            {
                WriteLine(e.Message);
            }
       
        }
        public static Users LogIn()
        {
            WriteLine("Login Screen");
            WriteLine("Enter Email id:");
            string Email = ReadLine();
            WriteLine("Enter password:");
            string Password = ReadLine();
            WriteLine("Press 1: Submit");
            WriteLine("Press 2: Cancel and Go Back");
            int ch = int.Parse(ReadLine());
            if (ch == 1)
            {
                Users n=UserData.CheckUser(Email, Password);
                if (n == null)
                {
                    WriteLine("Wrong Email or Password");
                    return null;
                }
                else
                {
                    WriteLine("Logged in");
                    return n;
                }
            }
            else
            {
                WriteLine("Login Cancelled");
                return null;
            }
        }
    }
}

 public static void resetPassword(Users user)
        {
            WriteLine("Reset Password");
            WriteLine("Enter new Password:");
            string pass = ReadLine();
            WriteLine("Press 1: Submit");
            WriteLine("Press 2: Cancel and Go back");
            int ch = int.Parse(ReadLine());
            if(ch==1)
                UserData.updatePassword(pass, user);
        }


//TweetOperations.cs

using System;
using System.Collections.Generic;
using TweeterApp.DataAccessLayer;
using TweeterApp.Entities;
using static System.Console;

namespace TweeterApp.BusinessLogicLayer
{
    public class TweetOperations
    {
        public static void createTweet(Users user)
        {
            WriteLine("Create Tweet");
            WriteLine("Enter Message:");
            string message = ReadLine();
            WriteLine("Tweet Id:");
            int tweetId = int.Parse(ReadLine());
            WriteLine("Press 1: Submit");
            WriteLine("Press 2: Cancel and Go Back");
            int ch = int.Parse(ReadLine());
            if (ch == 1)
            {
                TweetData.insertTweet(user, message, tweetId);
            }
        }
        public static void ViewTweets(Users user)
        {
            WriteLine("View Tweets");
            List<Tweets> tweets=TweetData.getTweets(user);
            if(tweets is null)
            {
                WriteLine("No Tweets Found");
            }
            else
            {
                foreach (var t in tweets)
                {
                    WriteLine("TweetID:{0} | Message:{1} | UserName:{2}", t.TweetId, t.Message, t.UserName);
                }
            }
            
            WriteLine("Press 1: Go Back");
            ReadKey();
        }
        public static void deleteTweet()
        {
            WriteLine("Delete Tweet");
            WriteLine("Enter TweetId:");
            int tweetId = int.Parse(ReadLine());
            TweetData.removeTweet(tweetId);
        }
        
    }
}


//PresentationLayer
//Program.cs

using System;
using TweeterApp.BusinessLogicLayer;
using TweeterApp.Entities;
using static System.Console;

namespace TweeterApp.PresentationLayer
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().MainPage();
        }
        public void MainPage()
        {
            WriteLine("Welcome To Tweet App");
            WriteLine("Press 1: Register");
            WriteLine("Press 2: Login");
            int ch1 = int.Parse(ReadLine());
            switch (ch1)
            {
                case 1: UserAuth.Register();
                    MainPage();
                    break;
                case 2: Users user= UserAuth.LogIn();
                    if (user is null)
                    {
                        MainPage();
                    }
                    else
                    {
                        homePage(user);
                    }
                    
                    break;
                default:
                    WriteLine("Wrong choice");
                    break;
            }
        }
        public void homePage(Users user)
        {
            try
            {
                while (true)
                {
                    WriteLine("Welcome to Tweet App");
                    WriteLine("1. Create new Tweet");
                    WriteLine("2. View All Tweet");
                    WriteLine("3. Delete All Tweets");
                    WriteLine("4. Reset Password");
                    int ch = int.Parse(ReadLine());
                    switch (ch)
                    {
                        case 1: TweetOperations.createTweet(user);
                            break;
                        case 2: TweetOperations.ViewTweets(user);
                            break;
                        case 3: TweetOperations.deleteTweet();
                            break;
                        case 4: UserAuth.resetPassword(user);
                            break;
                        default: WriteLine("Invalid Selection");
                            break;
                    }
                }
            }
            catch (Exception e)
            {
WriteLine(e.Message);
            }
        }
    }
}



