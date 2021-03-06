using System;
using NLog.Web;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Media_Library_2
{
    class Program
    {
        // create static instance of Logger
        private static NLog.Logger logger = NLogBuilder.ConfigureNLog(Directory.GetCurrentDirectory() + "\\nlog.config").GetCurrentClassLogger();
        // create static MovieFile
        private static MovieFile mf;
        private static string ScrubbedFile;
        static void Main(string[] args)
        {

            logger.Info("Program started");

           // Movie movie = new Movie
            // {
            //     mediaId = 123,
            //     title = "Greatest Movie Ever, The (2020)",
            //     director = "Jeff Grissom",
            //     // timespan (hours, minutes, seconds)
            //     runningTime = new TimeSpan(2, 21, 23),
            //     genres = { "Comedy", "Romance" }
            // };

            // Console.WriteLine(movie.Display());

            // Album album = new Album
            // {
            //     mediaId = 321,
            //     title = "Greatest Album Ever, The (2020)",
            //     artist = "Jeff's Awesome Band",
            //     recordLabel = "Universal Music Group",
            //     genres = { "Rock" }
            // };
            // Console.WriteLine(album.Display());

            // Book book = new Book
            // {
            //     mediaId = 111,
            //     title = "Super Cool Book",
            //     author = "Jeff Grissom",
            //     pageCount = 101,
            //     publisher = "",
            //     genres = { "Suspense", "Mystery" }
            // };
            // Console.WriteLine(book.Display());

            ScrubbedFile = FileScrubber.ScrubMovies("movies.csv");
            logger.Info(ScrubbedFile);
            mf = new MovieFile(ScrubbedFile);
            int choice = -1;
            while(choice != 0 ) 
            {
                Console.WriteLine("[1]Add movie");
                Console.WriteLine("[2]Display All movies");
                Console.WriteLine("[3]Search for a movie");
                Console.WriteLine("[0]Quit");
                choice = Int16.Parse(Console.ReadLine());
                switch (choice) 
                {
                    case 2 :
                        mf.DisplayMovies();
                        break;
                    case 1 : 
                        AddMovie();
                        break;
                    case 3 :
                        SearchMovies();
                        break;
                    default : break;
                }
            }
            logger.Info("Program ended");
        }
        private static void AddMovie() {
            //get movie info
            ulong newId = mf.GetNewID();
            Console.WriteLine("Enter movie title:");
            string title = Console.ReadLine();

            if(FindDupeMovieByTitle(title)) { 
                Console.WriteLine("That movie is already in the list!");
                return;
            }
           
          
            Console.WriteLine("Enter genre (or done to quit");
            string genresString = Console.ReadLine();
            while (genresString != "done")
            {
            Console.WriteLine("Enter genre (or done to quit");
            genresString = Console.ReadLine();
            }
           
           
                Console.WriteLine("Enter director:");
            string director = Console.ReadLine();
            Console.WriteLine("Enter run time xx:xx:xx");
            string runTime = Console.ReadLine();
            //make movie obj
            Movie m = new Movie 
            {
                mediaId =  newId,
                title = title,
                director = director,
                runningTime = TimeSpan.Parse(runTime)
            };
            List<string> genres = new List<string>();
            foreach(string genre in genresString.Split("|")) {
                genres.Add(genre);
            }
            m.genres = genres;
            //add to MovieFile.MovieList
            mf.MovieList.Add(m);
            logger.Info("movie added!");
            m.Display();

            // add to csv file
            string fileLine = $"{newId}";
            logger.Info($"{fileLine} added ");
            StreamWriter sw = new StreamWriter(ScrubbedFile,append:true);
            sw.WriteLine(fileLine);
            sw.Close();
           }
        private static bool FindDupeMovieByTitle(string title)
        {
            //check every Movie.title for passed in title
            //compare titles with ToLower()
            if(mf.MovieList.ConvertAll(m => m.title.ToLower()).Contains(title.ToLower())) 
            {
                logger.Info($"found duplicate title {title}");
                return true;
            }
            return false;
        }
        private static void SearchMovies() 
        {
            Console.WriteLine("Enter a title to search for:");
            string title = Console.ReadLine();
            if(title != null && title != "") 
            {
                //make a query
                List<Movie> result = mf.MovieList.Where(m => m.title.Contains(title)).ToList<Movie>();
                //loop through results and display returned movies
                foreach (Movie m in result)
                {
                    Console.WriteLine(m.Display());
                }
                Console.WriteLine($"found {result.Count()} movies with title containing \"{title}\"");

            }         
        }
    }     
}


