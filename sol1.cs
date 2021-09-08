

 Sql Server Codes 

Create database LinkedInApp

use LinkedInApp

create table Users(UserId int primary key, FirstName varchar(30),LastName varchar(30),Gender varchar(10),DateOfBirth Date,Email varchar(50),Password varchar(30));

create table Posts(
LinkedInId int primary key,
UserName varchar(60),
Message varchar(200),
CreateId datetime,
UserId int FOREIGN KEY REFERENCES Users(UserId)
);


LinkedInApp.Entities
//Users.cs

using System;

namespace LinkedInApp.Entities
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


//Posts.cs

using System;


namespace LinkedInApp.Entities
{
    class Posts
    {
        public int LinkedInId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public DateTime CreateId { get; set; }

    }
}

LinkedInApp.CustomException
DuplicateEmailIdException.cs

using System;

namespace LinkedInApp.CustomException
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
using LinkedInApp.CustomException;
using LinkedInApp.Entities;

namespace LinkedInApp.DataAccessLayer
{
    public class UserData
    {
        static string ConnectionString = "data source=.; database=LinkedInApp; integrated security=SSPI";
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






//LinkedInData.cs

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using LinkedInApp.Entities;

namespace LinkedInApp.DataAccessLayer
{
    public class LinkedInData
    {
        static string ConnectionString = "data source=.; database=LinkedInApp; integrated security=SSPI";
        public static void insertPost(Users user, string message, int tweetId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    string query = "insert into Posts values(" + tweetId + ",\'" + user.FirstName + user.LastName + "\',\'" + message + "\',\'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\'," + user.UserId + ")";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    connection.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    Console.WriteLine("Uploaded Posts = " + rowsAffected);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("OOPs, something went wrong.\n" + e);
            }
        }
        public static void removePost(int tweetId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    string query = "Delete from Posts where LinkedInId = "+tweetId;
                    SqlCommand cmd = new SqlCommand(query, connection);
                    connection.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    Console.WriteLine("Posts Deleted= " + rowsAffected);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("OOPs, something went wrong.\n" + e);
            }
        }

        public static List<Posts> getPosts(Users user)
        {
            List<Posts> tweets = new List<Posts>();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = "select * from Posts where UserId=" + user.UserId;
                SqlDataAdapter da = new SqlDataAdapter(query, connection);

                //Using Data Table
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    Posts t = new Posts()
                    {
                        LinkedInId = int.Parse(row["LinkedInId"].ToString()),
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
using LinkedInApp.DataAccessLayer;
using LinkedInApp.Entities;
using static System.Console;

namespace LinkedInApp.BusinessLogicLayer
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


//LinkedInOperations.cs

using System;
using System.Collections.Generic;
using LinkedInApp.DataAccessLayer;
using LinkedInApp.Entities;
using static System.Console;

namespace LinkedInApp.BusinessLogicLayer
{
    public class LinkedInOperations
    {
        public static void createPost(Users user)
        {
            WriteLine("Create Post");
            WriteLine("Enter Message:");
            string message = ReadLine();
            WriteLine("LinkedIn Id:");
            int tweetId = int.Parse(ReadLine());
            WriteLine("Press 1: Submit");
            WriteLine("Press 2: Cancel and Go Back");
            int ch = int.Parse(ReadLine());
            if (ch == 1)
            {
                LinkedInData.insertPost(user, message, tweetId);
            }
        }
        public static void ViewPosts(Users user)
        {
            WriteLine("View Posts");
            List<Posts> tweets=LinkedInData.getPosts(user);
            if(tweets is null)
            {
                WriteLine("No Posts Found");
            }
            else
            {
                foreach (var t in tweets)
                {
                    WriteLine("LinkedInId:{0} | Message:{1} | UserName:{2}", t.LinkedInId, t.Message, t.UserName);
                }
            }
            
            WriteLine("Press 1: Go Back");
            ReadKey();
        }
        public static void deletePost()
        {
            WriteLine("Delete Post");
            WriteLine("Enter LinkedInId:");
            int tweetId = int.Parse(ReadLine());
            LinkedInData.removePost(tweetId);
        }
        
    }
}


//PresentationLayer
//Program.cs

using System;
using LinkedInApp.BusinessLogicLayer;
using LinkedInApp.Entities;
using static System.Console;

namespace LinkedInApp.PresentationLayer
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().MainPage();
        }
        public void MainPage()
        {
            WriteLine("Welcome To Post App");
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
                    WriteLine("Welcome to LinkdeIn App");
                    WriteLine("1. Create new Post");
                    WriteLine("2. View All Posts");
                    WriteLine("3. Delete All Posts");
                    WriteLine("4. Reset Password");
                    int ch = int.Parse(ReadLine());
                    switch (ch)
                    {
                        case 1: PostOperations.createPost(user);
                            break;
                        case 2: PostOperations.ViewPosts(user);
                            break;
                        case 3: PostOperations.deletePost();
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



