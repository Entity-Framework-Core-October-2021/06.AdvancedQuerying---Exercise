namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Globalization;

    public class StartUp
    {
        public static void Main()
        {
            using var dbContext = new BookShopContext();
            //DbInitializer.ResetDatabase(dbContext);

            //01. Age Restriction
            //string ageRestriction = Console.ReadLine();
            //string result = GetBooksByAgeRestriction(dbContext, ageRestriction);

            //02. Golden Books
            //string result = GetGoldenBooks(dbContext);

            //03. Books by Price
            //string result = GetBooksByPrice(dbContext);

            //04. Not Released In
            //int year = int.Parse(Console.ReadLine());
            //string result = GetBooksNotReleasedIn(dbContext, year);

            //05. Book Titles by Category
            //string categories = Console.ReadLine();
            //string result = GetBooksByCategory(dbContext, categories);

            //06. Released Before Date
            //string inputDate = Console.ReadLine();
            //string result = GetBooksReleasedBefore(dbContext, inputDate);

            //07. Author Search
            //string firstNameEndsLike = Console.ReadLine();
            //string result = GetAuthorNamesEndingIn(dbContext, firstNameEndsLike);

            //08. Book Search
            //string titleContains = Console.ReadLine();
            //string result = GetBookTitlesContaining(dbContext, titleContains);

            //09. Book Search by Author
            //string lastNameStartsWith = Console.ReadLine();
            //string result = GetBooksByAuthor(dbContext, lastNameStartsWith);

            //10. Count Books
            //int lengthCheck = int.Parse(Console.ReadLine());
            //Console.WriteLine(CountBooks(dbContext, lengthCheck));

            //11. Total Book Copies
            //string result = CountCopiesByAuthor(dbContext);

            //12. Profit by Category
            //string result = GetTotalProfitByCategory(dbContext);

            //13. Most Recent Books
            //string result = GetMostRecentBooks(dbContext);

            //14. Increase Prices
            //IncreasePrices(dbContext);

            //15. Remove Books
            Console.WriteLine(RemoveBooks(dbContext));
        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            StringBuilder sb = new StringBuilder();

            AgeRestriction ageRestriction = Enum.Parse<AgeRestriction>(command, true);

            string[] bookTitles = context.Books
                                    .Where(b => b.AgeRestriction == ageRestriction)
                                    .OrderBy(b => b.Title)
                                    .Select(b => b.Title)
                                    .ToArray();

            foreach (var book in bookTitles)
            {
                sb.AppendLine(book);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            string[] goldenBooks = context.Books
                                     .Where(b => b.EditionType == EditionType.Gold &&
                                                 b.Copies < 5000)
                                     .OrderBy(b => b.BookId)
                                     .Select(b => b.Title)
                                     .ToArray();

            foreach (var book in goldenBooks)
            {
                sb.AppendLine(book);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var goldenBooks = context.Books
                                     .Where(b => b.Price > 40)
                                     .OrderBy(b => b.BookId)
                                     .Select(b => new
                                     {
                                         b.Title,
                                         b.Price
                                     })
                                     .OrderByDescending(b => b.Price)
                                     .ToList();

            foreach (var book in goldenBooks)
            {
                sb.AppendLine($"{book.Title} - ${book.Price:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            StringBuilder sb = new StringBuilder();

            var notReleasedBooks = context.Books
                                          .Where(b => b.ReleaseDate.Value.Year != year)
                                          .OrderBy(b => b.BookId)
                                          .Select(b => b.Title)
                                          .ToArray();

            foreach (var book in notReleasedBooks)
            {
                sb.AppendLine(book);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();
            List<string> booksInfo = new List<string>();

            string[] categories = input
                                        .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                        .Select(c => c.ToLower())
                                        .ToArray();

            foreach (var category in categories)
            {
                List<string> currentBooksCategories = context
                                        .Books
                                        .Where(b => b.BookCategories.Any(bc => bc.Category.Name.ToLower() == category))
                                        .Select(b => b.Title)
                                        .ToList();

                booksInfo.AddRange(currentBooksCategories);
            }

            foreach (var abook in booksInfo.OrderBy(x => x))
            {
                sb.AppendLine(abook);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksReleasedBefore(BookShopContext context, string inputDate)
        {
            StringBuilder sb = new StringBuilder();

            DateTime date = DateTime.ParseExact(inputDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var booksInfo = context
                .Books
                .Where(b => b.ReleaseDate < date)
                .Select(b => new
                {
                    b.Title,
                    b.EditionType,
                    b.Price,
                    b.ReleaseDate
                })
                .OrderByDescending(b => b.ReleaseDate)
                .ToList();

            foreach (var book in booksInfo)
            {
                sb.AppendLine($"{book.Title} - {book.EditionType} - ${book.Price:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            var authorsInfo = context.Authors
                                   .Where(a => a.FirstName.EndsWith(input))
                                   .Select(a => new
                                   {
                                       AuthorFirstName = a.FirstName,
                                       AuthorLastName = a.LastName
                                   })
                                   .OrderBy(a => a.AuthorFirstName)
                                   .ThenBy(a => a.AuthorLastName)
                                   .ToArray();

            foreach (var author in authorsInfo)
            {
                sb.AppendLine($"{author.AuthorFirstName} {author.AuthorLastName}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            var booksInfo = context.Books
                                   .Where(b => b.Title.ToLower().Contains(input.ToLower()))
                                   .OrderBy(b => b.Title)
                                   .Select(b => b.Title)
                                   .ToArray();

            foreach (var book in booksInfo)
            {
                sb.AppendLine(book);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            var authors = context.Books
                                   .Select(b => new
                                   {
                                       b.BookId,
                                       b.Title,
                                       AuthorFirstName = b.Author.FirstName,
                                       AuthorLastName = b.Author.LastName
                                   })
                                  .Where(a => a.AuthorLastName.ToLower().StartsWith(input.ToLower()))
                                  .OrderBy(b => b.BookId)
                                  .ToList();

            foreach (var book in authors)
            {
                sb.AppendLine($"{book.Title} ({book.AuthorFirstName} {book.AuthorLastName})");
            }

            return sb.ToString().TrimEnd();
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var booksInfo = context.Books
                                   .Where(b => b.Title.Length > lengthCheck)
                                   .Select(b => b.Title)
                                   .ToList();

            return booksInfo.Count();
        }

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var authors = context
                                .Authors
                                .Select(a => new
                                {
                                    a.FirstName,
                                    a.LastName,
                                    CopiesCount = a.Books.Select(b => b.Copies).Sum()
                                })
                                .OrderByDescending(b => b.CopiesCount)
                                .ToList();

            foreach (var author in authors)
            {
                sb.AppendLine($"{author.FirstName} {author.LastName} - {author.CopiesCount}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var categoriesProfit = context.Categories
                                          .Select(c => new
                                          {
                                              c.Name,
                                              TotalProfit = c.CategoryBooks
                                                           .Select(cb => cb.Book.Price * cb.Book.Copies).Sum()
                                          })
                                          .OrderByDescending(c => c.TotalProfit)
                                          .ThenBy(c => c.Name)
                                          .ToList();

            foreach (var category in categoriesProfit)
            {
                sb.AppendLine($"{category.Name} ${category.TotalProfit:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetMostRecentBooks(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var categories = context.Categories
                                        .Select(c => new
                                        {
                                            c.Name,
                                            MostRecsentBooks = c.CategoryBooks
                                                                          .OrderByDescending(cb => cb.Book.ReleaseDate)
                                                                          .Take(3)
                                                                          .Select(bc => new
                                                                          {
                                                                              Title = bc.Book.Title,
                                                                              ReleaseDate = bc.Book.ReleaseDate.Value.Year
                                                                          })
                                                                          .ToList()
                                        })
                                        .OrderBy(c => c.Name)
                                        .ToList();

            foreach (var category in categories)
            {
                sb.AppendLine($"--{category.Name}");

                foreach (var book in category.MostRecsentBooks)
                {
                    sb.AppendLine($"{book.Title} ({book.ReleaseDate})");
                }
            }

            return sb.ToString().TrimEnd();
        }

        public static void IncreasePrices(BookShopContext context)
        {
            var booksForPriceIncreasment = context.Books
                               .Where(b => b.ReleaseDate.Value.Year < 2010)
                               .ToArray();

            foreach (var book in booksForPriceIncreasment)
            {
                book.Price += 5;
            }

            context.SaveChanges();
        }

        public static int RemoveBooks(BookShopContext context)
        {
            const int count = 4200;

            var booksToDelete = context
                .Books
                .Where(b => b.Copies < count)
                .ToList();

            context.RemoveRange(booksToDelete);
            context.SaveChanges();

            return booksToDelete.Count;
        }
    }
}
