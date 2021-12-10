using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment.Models
{
    public class AssignmentDbContext : DbContext
    {
        // Tables
        public DbSet<Character> Characters { get; set; }
        public DbSet<Franchise> Franchises { get; set; }
        public DbSet<Movie> Movies { get; set; }
        
        // Override constructor with options 
        public AssignmentDbContext([NotNullAttribute] DbContextOptions options) : base(options)
        {
        }

        
        // Seed data in the OnModelCreating
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Franchises added
            // Adding a default franchise, if its unknown or non existant
            modelBuilder.Entity<Franchise>().HasData(new Franchise
            {
                Id = 1,
                Name = "No franchise",
                Description = "No known franchise or missing information"
            });

           
            modelBuilder.Entity<Franchise>().HasData(new Franchise
            {
                Id = 2,
                Name = "Fast And Furious",
                Description = "Fast cars and action!"
            });
            modelBuilder.Entity<Franchise>().HasData(new Franchise
            {
                Id = 3,
                Name = "Rocky",
                Description = "Boxing"
            });
            modelBuilder.Entity<Franchise>().HasData(new Franchise
            {
                Id = 4,
                Name = "Alien",
                Description = "SciFi, alien monsters attack!"
            });

            #endregion


            #region Movies added
            // Movies
            // Adding 1 as default franchise value
            modelBuilder.Entity<Movie>()
                .Property(b => b.FranchiseId)
                .HasDefaultValue(1);

            modelBuilder.Entity<Movie>().HasData(new Movie
            {
                Id = 1,
                Title = "Fast And furious 1",
                Genre = "Action, crime, thriller",
                Director = "Rob Cohen",
                ImageURL = "https://www.imdb.com/title/tt0232500/mediaviewer/rm3153527296/?ref_=ext_shr_lnk",
                TrailerURL = "https://www.imdb.com/video/vi2048898073?playlistId=tt0232500",
                FranchiseId = 2,
                ReleaseYear = "2001",
            });
            modelBuilder.Entity<Movie>().HasData(new Movie
            {
                Id = 2,
                Title = "Fast And furious 2",
                Genre = "Action, crime, thriller",
                Director = "Rob Cohen",
                ImageURL = "https://www.imdb.com/title/tt0232500/mediaviewer/rm3153527296/?ref_=ext_shr_lnk",
                TrailerURL = "https://www.imdb.com/video/vi2048898073?playlistId=tt0232500",
                FranchiseId = 2,
                ReleaseYear = "2002",
            });
            modelBuilder.Entity<Movie>().HasData(new Movie
            {
                Id = 3,
                Title = "Rocky",
                Genre = "Action, sport, boxing",
                Director = "John G. Avildsen",
                ImageURL = "https://www.imdb.com/title/tt0075148/mediaviewer/rm960529408/?ref_=ext_shr_lnk",
                TrailerURL = "https://www.imdb.com/video/vi2997093657?playlistId=tt0075148",
                FranchiseId = 3,
                ReleaseYear = "1976",
            });
            modelBuilder.Entity<Movie>().HasData(new Movie
            {
                Id = 4,
                Title = "Rocky 2",
                Genre = "Action, sport, boxing",
                Director = "Sylvester Stallone",
                ImageURL = "https://www.imdb.com/title/tt0075148/mediaviewer/rm960529408/?ref_=ext_shr_lnk",
                TrailerURL = "https://www.imdb.com/video/vi2997093657?playlistId=tt0075148",
                FranchiseId = 3,
                ReleaseYear = "1979",
            });
            modelBuilder.Entity<Movie>().HasData(new Movie
            {
                Id = 5,
                Title = "Alien",
                Genre = "Horror, Sci-Fi",
                Director = "Ridley Scott",
                ImageURL = "https://www.imdb.com/title/tt0078748/mediaviewer/rm2990766080/?ref_=ext_shr_lnk",
                TrailerURL = "https://www.imdb.com/video/vi1497801241?playlistId=tt0078748",
                FranchiseId = 4,
                ReleaseYear = "1979",
            });
            modelBuilder.Entity<Movie>().HasData(new Movie
            {
                Id = 6,
                Title = "Aliens",
                Genre = "Horror, Sci-Fi",
                Director = "James Cameron",
                ImageURL = "https://www.imdb.com/title/tt0078748/mediaviewer/rm2990766080/?ref_=ext_shr_lnk",
                TrailerURL = "https://www.imdb.com/video/vi1497801241?playlistId=tt0078748",
                FranchiseId = 4,
                ReleaseYear = "1986",
            });

            #endregion

            #region Characters added

            // Characters

            modelBuilder.Entity<Character>()
                .Property(b => b.Alias)
                .HasDefaultValue(null);
            modelBuilder.Entity<Character>()
                .Property(b => b.Gender)
                .HasDefaultValue(null);
            modelBuilder.Entity<Character>()
                .Property(b => b.ImageURL)
                .HasDefaultValue(null);

            modelBuilder.Entity<Character>().HasData(new Character
            {
                Id = 1,
                FullName = "Ellen Louise Ripley",
                Alias = "Ripley",
                Gender = "Female",
            });
            modelBuilder.Entity<Character>().HasData(new Character
            {
                Id = 2,
                FullName = "Robert Balboa",
                Alias = "Rocky",
                Gender = "Male",
            });
            modelBuilder.Entity<Character>().HasData(new Character
            {
                Id = 3,
                FullName = "Dominic Toretto",
                Alias = "Dom",
                Gender = "Male",
            });
            modelBuilder.Entity<Character>().HasData(new Character
            {
                Id = 4,
                FullName = "Brian O'Conner",
                Alias = "",
                Gender = "Male",
            });

            #endregion
            // m2m Character/Movie
            modelBuilder.Entity<Character>()
                .HasMany(c => c.Movies)
                .WithMany(m => m.Characters)
                .UsingEntity<Dictionary<string, object>>(
                    "CharacterMovie",
                    r => r.HasOne<Movie>().WithMany().HasForeignKey("MoviesId"),
                    l => l.HasOne<Character>().WithMany().HasForeignKey("CharactersId"),
                    ur =>
                    {
                        ur.HasKey("MoviesId", "CharactersId");
                        ur.HasData(
                            new { MoviesId = 3, CharactersId = 2 },
                            new { MoviesId = 4, CharactersId = 2 },
                            new { MoviesId = 5, CharactersId = 1 },
                            new { MoviesId = 6, CharactersId = 1 },
                            new { MoviesId = 1, CharactersId = 3 },
                            new { MoviesId = 1, CharactersId = 4 },
                            new { MoviesId = 2, CharactersId = 3 },
                            new { MoviesId = 2, CharactersId = 4 }
                            );
                    });
        }
        
    }
}
