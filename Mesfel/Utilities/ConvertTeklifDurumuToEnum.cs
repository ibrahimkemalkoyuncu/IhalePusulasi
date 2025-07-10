using Microsoft.EntityFrameworkCore.Migrations;

namespace Mesfel.Utilities
{
    public partial class ConvertTeklifDurumuToEnum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TeklifDurumu",
                table: "IhaleTeklifleri",
                type: "nvarchar(20)",
                nullable: false,
                defaultValue: "VERILDI",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldNullable: true);

            migrationBuilder.Sql(@"
            UPDATE IhaleTeklifleri 
            SET TeklifDurumu = CASE
                WHEN TeklifDurumu = 'Beklemede' THEN 'VERILDI'
                WHEN TeklifDurumu = 'Değerlendiriliyor' THEN 'DEGERLENDIRILIYOR'
                WHEN TeklifDurumu = 'Kabul Edildi' THEN 'KABUL'
                WHEN TeklifDurumu = 'Reddedildi' THEN 'REDDEDILDI'
                WHEN TeklifDurumu = 'Geçersiz' THEN 'GECERSIZ'
                ELSE 'VERILDI'
            END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Rollback işlemleri
        }
    }
}
