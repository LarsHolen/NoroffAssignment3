using Microsoft.EntityFrameworkCore.Migrations;

namespace Assignment.Migrations
{
    public partial class initializeAndSeedDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Character",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Alias = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    ImageURL = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Character", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Franchise",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Franchise", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Movie",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Genre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ReleaseYear = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    Director = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ImageURL = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    TrailerURL = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    FranchiseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movie", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Movie_Franchise_FranchiseId",
                        column: x => x.FranchiseId,
                        principalTable: "Franchise",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterMovie",
                columns: table => new
                {
                    MoviesId = table.Column<int>(type: "int", nullable: false),
                    CharactersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterMovie", x => new { x.MoviesId, x.CharactersId });
                    table.ForeignKey(
                        name: "FK_CharacterMovie_Character_CharactersId",
                        column: x => x.CharactersId,
                        principalTable: "Character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterMovie_Movie_MoviesId",
                        column: x => x.MoviesId,
                        principalTable: "Movie",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Character",
                columns: new[] { "Id", "Alias", "FullName", "Gender", "ImageURL" },
                values: new object[,]
                {
                    { 1, "Ripley", "Ellen Louise Ripley", "Female", null },
                    { 2, "Rocky", "Robert Balboa", "Male", null },
                    { 3, "Dom", "Dominic Toretto", "Male", null },
                    { 4, "", "Brian O'Conner", "Male", null }
                });

            migrationBuilder.InsertData(
                table: "Franchise",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Fast cars and action!", "Fast And Furious" },
                    { 2, "Boxing", "Rocky" },
                    { 3, "SciFi, alien monsters attack!", "Alien" }
                });

            migrationBuilder.InsertData(
                table: "Movie",
                columns: new[] { "Id", "Director", "FranchiseId", "Genre", "ImageURL", "ReleaseYear", "Title", "TrailerURL" },
                values: new object[,]
                {
                    { 1, "Rob Cohen", 1, "Action, crime, thriller", "https://www.imdb.com/title/tt0232500/mediaviewer/rm3153527296/?ref_=ext_shr_lnk", "2001", "Fast And furious 1", "https://www.imdb.com/video/vi2048898073?playlistId=tt0232500" },
                    { 2, "Rob Cohen", 1, "Action, crime, thriller", "https://www.imdb.com/title/tt0232500/mediaviewer/rm3153527296/?ref_=ext_shr_lnk", "2002", "Fast And furious 2", "https://www.imdb.com/video/vi2048898073?playlistId=tt0232500" },
                    { 3, "John G. Avildsen", 2, "Action, sport, boxing", "https://www.imdb.com/title/tt0075148/mediaviewer/rm960529408/?ref_=ext_shr_lnk", "1976", "Rocky", "https://www.imdb.com/video/vi2997093657?playlistId=tt0075148" },
                    { 4, "Sylvester Stallone", 2, "Action, sport, boxing", "https://www.imdb.com/title/tt0075148/mediaviewer/rm960529408/?ref_=ext_shr_lnk", "1979", "Rocky 2", "https://www.imdb.com/video/vi2997093657?playlistId=tt0075148" },
                    { 5, "Ridley Scott", 3, "Horror, Sci-Fi", "https://www.imdb.com/title/tt0078748/mediaviewer/rm2990766080/?ref_=ext_shr_lnk", "1979", "Alien", "https://www.imdb.com/video/vi1497801241?playlistId=tt0078748" },
                    { 6, "James Cameron", 3, "Horror, Sci-Fi", "https://www.imdb.com/title/tt0078748/mediaviewer/rm2990766080/?ref_=ext_shr_lnk", "1986", "Aliens", "https://www.imdb.com/video/vi1497801241?playlistId=tt0078748" }
                });

            migrationBuilder.InsertData(
                table: "CharacterMovie",
                columns: new[] { "CharactersId", "MoviesId" },
                values: new object[,]
                {
                    { 3, 1 },
                    { 4, 1 },
                    { 2, 3 },
                    { 2, 4 },
                    { 1, 5 },
                    { 1, 6 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterMovie_CharactersId",
                table: "CharacterMovie",
                column: "CharactersId");

            migrationBuilder.CreateIndex(
                name: "IX_Movie_FranchiseId",
                table: "Movie",
                column: "FranchiseId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterMovie");

            migrationBuilder.DropTable(
                name: "Character");

            migrationBuilder.DropTable(
                name: "Movie");

            migrationBuilder.DropTable(
                name: "Franchise");
        }
    }
}
