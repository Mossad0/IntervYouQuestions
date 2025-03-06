using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntervYouQuestions.Api.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(18,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Categori__19093A0BE68A9E3C", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Topics",
                columns: table => new
                {
                    TopicId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Topics__022E0F5D533AA597", x => x.TopicId);
                    table.ForeignKey(
                        name: "FK__Topics__Category__3B75D760",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    QuestionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false),
                    Difficulty = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TopicId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Question__0DC06FACD726E932", x => x.QuestionId);
                    table.ForeignKey(
                        name: "FK__Questions__Topic__3E52440B",
                        column: x => x.TopicId,
                        principalTable: "Topics",
                        principalColumn: "TopicId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Model_Answers",
                columns: table => new
                {
                    Model_Answer_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "text", nullable: false),
                    Key_Points = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Model_An__DFF72A5B619179FE", x => x.Model_Answer_Id);
                    table.ForeignKey(
                        name: "FK__Model_Ans__Quest__412EB0B6",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Question_Options",
                columns: table => new
                {
                    Option_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "text", nullable: false),
                    isCorrect = table.Column<bool>(type: "bit", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Question__3260907EADE631F9", x => x.Option_Id);
                    table.ForeignKey(
                        name: "FK__Question___Quest__440B1D61",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Model_Answers_QuestionId",
                table: "Model_Answers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Question_Options_QuestionId",
                table: "Question_Options",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_TopicId",
                table: "Questions",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_CategoryId",
                table: "Topics",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Model_Answers");

            migrationBuilder.DropTable(
                name: "Question_Options");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Topics");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
