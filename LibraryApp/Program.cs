
using System;
using System.Collections.Generic;
using LibraryData;
namespace LibraryApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Instantiate all the helper classes we created
            BooksManager booksManager = new BooksManager();
            MemberRepository memberRepo = new MemberRepository();
            BookTransactionManager transactionManager = new BookTransactionManager();
            BackupHelper backupHelper = new BackupHelper();
            BooksIssude booksIssude = new BooksIssude();
            
            string backupFilePath = "books_backup.json";
            bool isRunning = true;

            while (isRunning)
            {
                Console.WriteLine("\nLibrary Management System");
                Console.WriteLine("1. Add Book");
                Console.WriteLine("2. View All Books");
                Console.WriteLine("3. Add Member");
                Console.WriteLine("4. View All Members");
                Console.WriteLine("5. Issue Book to Member (Transaction)");
                Console.WriteLine("6. Return Book (Transaction)");
                Console.WriteLine("7. Export Books to JSON");
                Console.WriteLine("8. Import Books from JSON");
                Console.WriteLine("9. Exit");
                Console.Write("Enter your choice: ");

                string? choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        booksManager.AddBook();
                        break;

                    case "2":
                        List<Book> books = booksManager.RetrieveBook();
                        if (books.Count == 0)
                        {
                            Console.WriteLine("No books found.");
                        }
                        else
                        {
                            foreach (var b in books)
                            {
                                Console.WriteLine($"ID: {b.BookId} | Title: {b.Title} | Author: {b.Author} | Stock: {b.Stock}");
                            }
                        }
                        break;

                    case "3":
                        memberRepo.AddMember();
                        break;

                    case "4":
                        List<Member> members = memberRepo.GetAllMembers();
                        if (members.Count == 0)
                        {
                            Console.WriteLine("No members found.");
                        }
                        else
                        {
                            foreach (var m in members)
                            {
                                Console.WriteLine($"ID: {m.MemberId} | Name: {m.Name}");
                            }
                        }
                        break;

                    case "5":
                        Console.Write("Enter Book ID to issue: ");
                        int issueBookId = int.Parse(Console.ReadLine() ?? "0");
                        
                        Console.Write("Enter Member ID: ");
                        int issueMemberId = int.Parse(Console.ReadLine() ?? "0");

                        bool issueSuccess = booksIssude.issuedBoook(issueBookId, issueMemberId);
                        if (issueSuccess)
                            Console.WriteLine("Book issued successfully.");
                        else
                            Console.WriteLine("Failed to issue book (Check stock and IDs).");
                        break;

                    case "6":
                        Console.Write("Enter Issue ID to return: ");
                        int returnIssueId = int.Parse(Console.ReadLine() ?? "0");

                        bool returnSuccess = transactionManager.ReturnBook(returnIssueId);
                        if (returnSuccess)
                            Console.WriteLine("Book returned successfully.");
                        else
                            Console.WriteLine("Failed to return book (Invalid Issue ID or already returned).");
                        break;

                    case "7":
                        List<Book> booksToExport = booksManager.RetrieveBook();
                        backupHelper.SaveBooksToJson(booksToExport, backupFilePath);
                        break;

                    case "8":
                        List<Book> importedBooks = backupHelper.LoadBooksFromJson(backupFilePath);
                        Console.WriteLine($"Imported {importedBooks.Count} books from backup.");
                        foreach (var b in importedBooks)
                        {
                            Console.WriteLine($"ID: {b.BookId} | Title: {b.Title} | Author: {b.Author} | Stock: {b.Stock}");
                        }
                        break;

                    case "9":
                        Console.WriteLine("Exiting program. Goodbye!");
                        isRunning = false;
                        break;

                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }
    }
}