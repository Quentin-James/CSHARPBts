using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AjoutSpectaclesEnfants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Artistes",
                columns: table => new
                {
                    ArtisteID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Artistes__6635EC444C40FE8D", x => x.ArtisteID);
                });

            migrationBuilder.CreateTable(
                name: "GroupesSpectacles",
                columns: table => new
                {
                    GroupeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomGroupe = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__GroupesS__5C811B3043F4298F", x => x.GroupeID);
                });

            migrationBuilder.CreateTable(
                name: "Spectacles",
                columns: table => new
                {
                    SpectacleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titre = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    Duree = table.Column<TimeOnly>(type: "time", nullable: true),
                    Saison = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    SpectacleEnfant1Id = table.Column<int>(type: "int", nullable: true),
                    SpectacleEnfant2Id = table.Column<int>(type: "int", nullable: true),
                    SpectacleEnfant3Id = table.Column<int>(type: "int", nullable: true),
                    DeconseilleAuxEnfants = table.Column<bool>(type: "bit", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Spectacl__55DD5C655BDE6A97", x => x.SpectacleID);
                    table.ForeignKey(
                        name: "FK_Spectacle_SpectacleEnfant1",
                        column: x => x.SpectacleEnfant1Id,
                        principalTable: "Spectacles",
                        principalColumn: "SpectacleID");
                    table.ForeignKey(
                        name: "FK_Spectacle_SpectacleEnfant2",
                        column: x => x.SpectacleEnfant2Id,
                        principalTable: "Spectacles",
                        principalColumn: "SpectacleID");
                    table.ForeignKey(
                        name: "FK_Spectacle_SpectacleEnfant3",
                        column: x => x.SpectacleEnfant3Id,
                        principalTable: "Spectacles",
                        principalColumn: "SpectacleID");
                });

            migrationBuilder.CreateTable(
                name: "TypesTarifs",
                columns: table => new
                {
                    TarifID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomTarif = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TypesTar__57971C5133704716", x => x.TarifID);
                });

            migrationBuilder.CreateTable(
                name: "GroupesSpectaclesOrganisation",
                columns: table => new
                {
                    GroupeID = table.Column<int>(type: "int", nullable: false),
                    TypeSpectacle = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    Duree = table.Column<TimeOnly>(type: "time", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__GroupesS__5C811B3044AF5A05", x => x.GroupeID);
                    table.ForeignKey(
                        name: "FK__GroupesSp__Group__5535A963",
                        column: x => x.GroupeID,
                        principalTable: "GroupesSpectacles",
                        principalColumn: "GroupeID");
                });

            migrationBuilder.CreateTable(
                name: "ArtistesSpectacles",
                columns: table => new
                {
                    ArtisteID = table.Column<int>(type: "int", nullable: false),
                    SpectacleID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Artistes__23683982BC8F2C45", x => new { x.ArtisteID, x.SpectacleID });
                    table.ForeignKey(
                        name: "FK__ArtistesS__Artis__412EB0B6",
                        column: x => x.ArtisteID,
                        principalTable: "Artistes",
                        principalColumn: "ArtisteID");
                    table.ForeignKey(
                        name: "FK__ArtistesS__Spect__4222D4EF",
                        column: x => x.SpectacleID,
                        principalTable: "Spectacles",
                        principalColumn: "SpectacleID");
                });

            migrationBuilder.CreateTable(
                name: "Programmation",
                columns: table => new
                {
                    ProgrammationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Heure = table.Column<TimeOnly>(type: "time", nullable: false),
                    Lieu = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    SpectacleID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Programm__3AFFDBE06F636068", x => x.ProgrammationID);
                    table.ForeignKey(
                        name: "FK__Programma__Spect__44FF419A",
                        column: x => x.SpectacleID,
                        principalTable: "Spectacles",
                        principalColumn: "SpectacleID");
                });

            migrationBuilder.CreateTable(
                name: "SpectaclesGroupes",
                columns: table => new
                {
                    GroupeID = table.Column<int>(type: "int", nullable: false),
                    SpectacleID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Spectacl__19DCCEF6096A829E", x => new { x.GroupeID, x.SpectacleID });
                    table.ForeignKey(
                        name: "FK__Spectacle__Group__4D94879B",
                        column: x => x.GroupeID,
                        principalTable: "GroupesSpectacles",
                        principalColumn: "GroupeID");
                    table.ForeignKey(
                        name: "FK__Spectacle__Spect__4E88ABD4",
                        column: x => x.SpectacleID,
                        principalTable: "Spectacles",
                        principalColumn: "SpectacleID");
                });

            migrationBuilder.CreateTable(
                name: "TarifsGroupes",
                columns: table => new
                {
                    TarifID = table.Column<int>(type: "int", nullable: false),
                    GroupeID = table.Column<int>(type: "int", nullable: false),
                    Prix = table.Column<decimal>(type: "decimal(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TarifsGr__425F0DE2907FCD83", x => new { x.TarifID, x.GroupeID });
                    table.ForeignKey(
                        name: "FK__TarifsGro__Group__52593CB8",
                        column: x => x.GroupeID,
                        principalTable: "GroupesSpectacles",
                        principalColumn: "GroupeID");
                    table.ForeignKey(
                        name: "FK__TarifsGro__Tarif__5165187F",
                        column: x => x.TarifID,
                        principalTable: "TypesTarifs",
                        principalColumn: "TarifID");
                });

            migrationBuilder.CreateTable(
                name: "TarifsSpectacles",
                columns: table => new
                {
                    TarifID = table.Column<int>(type: "int", nullable: false),
                    SpectacleID = table.Column<int>(type: "int", nullable: false),
                    Prix = table.Column<decimal>(type: "decimal(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TarifsSp__12CAC99767A5BAE6", x => new { x.TarifID, x.SpectacleID });
                    table.ForeignKey(
                        name: "FK__TarifsSpe__Spect__3E52440B",
                        column: x => x.SpectacleID,
                        principalTable: "Spectacles",
                        principalColumn: "SpectacleID");
                    table.ForeignKey(
                        name: "FK__TarifsSpe__Tarif__3D5E1FD2",
                        column: x => x.TarifID,
                        principalTable: "TypesTarifs",
                        principalColumn: "TarifID");
                });

            migrationBuilder.CreateTable(
                name: "Billets",
                columns: table => new
                {
                    BilletID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Civilite = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    Nom = table.Column<string>(type: "varchar(12)", unicode: false, maxLength: 12, nullable: false),
                    Prenom = table.Column<string>(type: "varchar(12)", unicode: false, maxLength: 12, nullable: false),
                    PrixAchat = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    TarifID = table.Column<int>(type: "int", nullable: true),
                    ProgrammationID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Billets__39A625695730C264", x => x.BilletID);
                    table.ForeignKey(
                        name: "FK__Billets__Program__48CFD27E",
                        column: x => x.ProgrammationID,
                        principalTable: "Programmation",
                        principalColumn: "ProgrammationID");
                    table.ForeignKey(
                        name: "FK__Billets__TarifID__47DBAE45",
                        column: x => x.TarifID,
                        principalTable: "TypesTarifs",
                        principalColumn: "TarifID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArtistesSpectacles_SpectacleID",
                table: "ArtistesSpectacles",
                column: "SpectacleID");

            migrationBuilder.CreateIndex(
                name: "IX_Billets_ProgrammationID",
                table: "Billets",
                column: "ProgrammationID");

            migrationBuilder.CreateIndex(
                name: "IX_Billets_TarifID",
                table: "Billets",
                column: "TarifID");

            migrationBuilder.CreateIndex(
                name: "IX_Programmation_SpectacleID",
                table: "Programmation",
                column: "SpectacleID");

            migrationBuilder.CreateIndex(
                name: "IX_Spectacles_SpectacleEnfant1Id",
                table: "Spectacles",
                column: "SpectacleEnfant1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Spectacles_SpectacleEnfant2Id",
                table: "Spectacles",
                column: "SpectacleEnfant2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Spectacles_SpectacleEnfant3Id",
                table: "Spectacles",
                column: "SpectacleEnfant3Id");

            migrationBuilder.CreateIndex(
                name: "IX_SpectaclesGroupes_SpectacleID",
                table: "SpectaclesGroupes",
                column: "SpectacleID");

            migrationBuilder.CreateIndex(
                name: "IX_TarifsGroupes_GroupeID",
                table: "TarifsGroupes",
                column: "GroupeID");

            migrationBuilder.CreateIndex(
                name: "IX_TarifsSpectacles_SpectacleID",
                table: "TarifsSpectacles",
                column: "SpectacleID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArtistesSpectacles");

            migrationBuilder.DropTable(
                name: "Billets");

            migrationBuilder.DropTable(
                name: "GroupesSpectaclesOrganisation");

            migrationBuilder.DropTable(
                name: "SpectaclesGroupes");

            migrationBuilder.DropTable(
                name: "TarifsGroupes");

            migrationBuilder.DropTable(
                name: "TarifsSpectacles");

            migrationBuilder.DropTable(
                name: "Artistes");

            migrationBuilder.DropTable(
                name: "Programmation");

            migrationBuilder.DropTable(
                name: "GroupesSpectacles");

            migrationBuilder.DropTable(
                name: "TypesTarifs");

            migrationBuilder.DropTable(
                name: "Spectacles");
        }
    }
}
