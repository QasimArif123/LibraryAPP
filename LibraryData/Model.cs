using System;
using System.Text.Json;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Data;

namespace LibraryData 
{
public class Book
{
public int BookId { get; set; }
public string? Title { get; set; }
public string? Author { get; set; }
public int Stock { get; set; }
public Book()
        {
            
        }
public Book(int bId , string T ,string A , int s)
        {
            BookId = bId;
            Title = T;
            Author = A;
            Stock = s;

        }
}
public class Member
{
public int MemberId { get; set; }
public string? Name { get; set; }
public Member(int m , string name)
        {
            MemberId = m;
            Name = name;
        }
}

    public class BooksManager
    {
        private string connectionString = "Server=localhost\\SQLEXPRESS;Database=Libaray;Trusted_Connection=True;TrustServerCertificate=True;";
        public void AddBook()
        {
            Book book = new Book();
            Console.Write("Enter Book Title: ");
            book.Title = Console.ReadLine();
            Console.Write("Enter Book Author : ");
            book.Author = Console.ReadLine();
            Console.Write("Enter Book Stock : ");
            book.Stock = int.Parse(Console.ReadLine() ?? "0");
           using( SqlConnection connection =new SqlConnection(connectionString)){
            string? query = "insert into Books(Title,Author,Stock) values(@T,@A,@S)"; 
            using (SqlCommand command = new SqlCommand(query,connection)){
            command.Parameters.AddWithValue("@T",book.Title);
            command.Parameters.AddWithValue("@A",book.Author);
            command.Parameters.AddWithValue("@S",book.Stock);
            connection.Open();
            
            command.ExecuteNonQuery();
            }
           }
        }
        public List<Book> RetrieveBook()
        {
            List<Book> AllBooks = new List<Book>();
            using(SqlConnection connection =new SqlConnection(connectionString)){
            string? query = "Select * from Books";
            using (SqlCommand command = new SqlCommand(query,connection)){
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            while(reader.Read())
            {
                Book currentBook = new Book();
                    currentBook.BookId = Convert.ToInt32(reader["BookId"]);
                    currentBook.Title = reader["Title"].ToString();
                    currentBook.Author = reader["Author"].ToString();
                    currentBook.Stock = Convert.ToInt32(reader["Stock"]);

                    AllBooks.Add(currentBook);
                }
                    return AllBooks;
            }
            }
        }
        public void RemoveBook()
        {
            Console.Write("Enter Book ID to remove from database : ");
            int id = int.Parse(Console.ReadLine() ?? "-1") ;
            if(id == -1)
            {
                Console.WriteLine("Invalid Id ... ");
                return;
            }
            
            string? query = "Delete From Books where BookId = @id";
            using (SqlConnection connection =new SqlConnection(connectionString)){
            using (SqlCommand command = new SqlCommand(query,connection)){
            command.Parameters.AddWithValue("@id",id);
            connection.Open();
            int rows = command.ExecuteNonQuery();
            
            if(rows > 0)
            {
                Console.WriteLine("Successfully deleted");
            }
            }

        }
        
        }
    }   


    public class MemberRepository
    {
        private string connectionString = "Server=localhost\\SQLEXPRESS;Database=Libaray;Trusted_Connection=True;TrustServerCertificate=True;";

        public void AddMember()
        {
            Console.Write("Enter Member Name: ");
            string? name = Console.ReadLine();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Members (Name) VALUES (@name)";
                
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", name ?? "Unknown");                    
                    connection.Open();
                    command.ExecuteNonQuery();
                    Console.WriteLine("Member added successfully.");
                }
            }
        }
        public List<Member> GetAllMembers()
        {
            List<Member> membersList = new List<Member>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT MemberId, Name FROM Members";
                
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = Convert.ToInt32(reader["MemberId"]);
                            string name = reader["Name"].ToString() ?? "Unknown";

                            Member currentMember = new Member(id, name);
                            
                            membersList.Add(currentMember);
                        }
                    }
                }
            }

            return membersList;
        }
    }
    
    public class BooksIssude{
         private string connectionString = "Server=localhost\\SQLEXPRESS;Database=Libaray;Trusted_Connection=True;TrustServerCertificate=True;";

         public bool issuedBoook(int Bookid,int MemberId)
        {
         using ( SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string? query = "SELECT STOCK FROM BOOKS WHERE BOOKID = @id ";
                        using (SqlCommand checkcommand = new SqlCommand(query, connection,transaction))
                        {
                        checkcommand.Parameters.AddWithValue("@id",Bookid);   
                        object result = checkcommand.ExecuteScalar();
                            
                            // If the book does not exist (null) or stock is 0, cancel everything
                            if (result == null || Convert.ToInt32(result) <= 0)
                            {
                                transaction.Rollback();
                                return false;
                            }
                        }
                    string insertQuery = "insert into IssuedBooks(Bookid,memberid,issuedate) values(@bid,@mid,@issuedate)";
                    using (SqlCommand InsertCommand = new SqlCommand(insertQuery, connection,transaction))
                        {
                            InsertCommand.Parameters.AddWithValue("@bid",Bookid);
                            InsertCommand.Parameters.AddWithValue("@mid",MemberId);
                            InsertCommand.Parameters.AddWithValue("@issuedate",DateTime.Now.Date);
                            InsertCommand.ExecuteNonQuery();
                        }
                    string updateStockQuery = "UPDATE Books SET Stock = Stock - 1 WHERE BookId = @bookId";
                        using (SqlCommand updateCmd = new SqlCommand(updateStockQuery, connection, transaction))
                        {
                            updateCmd.Parameters.AddWithValue("@bookId", Bookid);
                            
                            updateCmd.ExecuteNonQuery();
                        }
                        transaction.Commit();
                        return true;       
                    }
                    catch(Exception e)
                    {
                        transaction.Rollback();
                        Console.WriteLine("Transaction failed due to following issue \n" + e.Message);
                        return false;
                    }
                }


            }  
        }
        }

    public class BookTransactionManager
    {
        private string connectionString = "Server=localhost\\SQLEXPRESS;Database=Libaray;Trusted_Connection=True;TrustServerCertificate=True;";

        public bool ReturnBook(int issueId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        int bookId = -1;
                        string checkQuery = "SELECT BookId, ReturnDate FROM IssuedBooks WHERE IssueId = @issueId";
                        using (SqlCommand Checkcommand= new SqlCommand(checkQuery, connection, transaction))
                        {
                            Checkcommand.Parameters.AddWithValue("@issueId", issueId);
                            
                            using (SqlDataReader reader = Checkcommand.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    if (!reader.IsDBNull(reader.GetOrdinal("ReturnDate")))
                                    {
                                        transaction.Rollback();
                                        return false; 
                                    }
                                    bookId = Convert.ToInt32(reader["BookId"]);
                                }
                                else
                                {
                                    transaction.Rollback();
                                    return false;
                                }
                            }
                        }
                        string returnQuery = "UPDATE IssuedBooks SET ReturnDate = @returnDate WHERE IssueId = @issueId";
                        using (SqlCommand returnCmd = new SqlCommand(returnQuery, connection, transaction))
                        {
                            returnCmd.Parameters.AddWithValue("@issueId", issueId);
                            returnCmd.Parameters.AddWithValue("@returnDate", DateTime.Now.Date);
                            
                            returnCmd.ExecuteNonQuery();
                        }
                        string updateStockQuery = "UPDATE Books SET Stock = Stock + 1 WHERE BookId = @bookId";
                        using (SqlCommand updateCmd = new SqlCommand(updateStockQuery, connection, transaction))
                        {
                            updateCmd.Parameters.AddWithValue("@bookId", bookId);
                            
                            updateCmd.ExecuteNonQuery();
                        }
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine($"Transaction failed: {ex.Message}");
                        return false;
                    }
                }
            }
        }
    }
    public class BackupHelper
    {
        public void SaveBooksToJson(List<Book> books, string filePath)
        {
            JsonSerializerOptions options = new JsonSerializerOptions 
            { 
                WriteIndented = true 
            };
            string jsonString = JsonSerializer.Serialize(books, options);
            
           using (StreamWriter writer = new StreamWriter(filePath, append: false)){
                writer.Write(jsonString);
            }
            Console.WriteLine($"Backup successfully saved to {filePath}");
        }
        public List<Book> LoadBooksFromJson(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Backup file not found. Returning an empty list.");
                return new List<Book>();
            }
            string jsonString = File.ReadAllText(filePath);
            List<Book>? backedUpBooks = JsonSerializer.Deserialize<List<Book>>(jsonString);
            return backedUpBooks ?? new List<Book>();
        }
    }   
}