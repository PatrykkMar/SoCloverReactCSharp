using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SoClover.Server.Migrations
{
    /// <inheritdoc />
    public partial class SeedCardsFromJson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "Cards");

            migrationBuilder.InsertData(
                table: "Cards",
                columns: new[] { "Id", "WordBottom", "WordLeft", "WordRight", "WordTop" },
                values: new object[,]
                {
                    { 1, "Wilk", "Grzyby", "Jagoda", "Las" },
                    { 2, "Cukier", "Poranek", "Kubek", "Kawa" },
                    { 3, "Woda", "Słońce", "Piasek", "Plaża" },
                    { 4, "Tablica", "Książka", "Uczeń", "Szkoła" },
                    { 5, "Silnik", "Droga", "Koło", "Samochód" },
                    { 6, "Rakieta", "Planeta", "Gwiazda", "Kosmos" },
                    { 7, "Narty", "Lód", "Śnieg", "Zima" },
                    { 8, "Taniec", "Gitara", "Radio", "Muzyka" },
                    { 9, "Popcorn", "Ekran", "Film", "Kino" },
                    { 10, "Mąka", "Ciasto", "Chleb", "Piekarnia" },
                    { 11, "Bateria", "Aplikacja", "Ekran", "Telefon" },
                    { 12, "Plecak", "Wspinaczka", "Szczyt", "Góry" },
                    { 13, "Kelner", "Talerz", "Menu", "Restauracja" },
                    { 14, "Ziemia", "Łopata", "Kwiat", "Ogród" },
                    { 15, "Pilot", "Chmury", "Bilet", "Lotnisko" },
                    { 16, "Mecz", "Kibic", "Bramka", "Piłka" },
                    { 17, "Mech", "Sarna", "Drzewo", "Las" },
                    { 18, "Aktor", "Kurtyna", "Scena", "Teatr" },
                    { 19, "Budzik", "Minuta", "Czas", "Zegar" },
                    { 20, "Stacja", "Wagon", "Tor", "Pociąg" },
                    { 21, "Statek", "Sól", "Fala", "Morze" },
                    { 22, "Garnek", "Przyprawa", "Nóż", "Kuchnia" },
                    { 23, "Kawa", "Fotel", "Laptop", "Biuro" },
                    { 24, "Sen", "Gwiazdy", "Noc", "Księżyc" },
                    { 25, "Wędka", "Kajak", "Most", "Rzeka" },
                    { 26, "Karma", "Spacer", "Smycz", "Pies" },
                    { 27, "Mleko", "Pazur", "Mysz", "Kot" },
                    { 28, "Recepta", "Syrop", "Lek", "Apteka" },
                    { 29, "Popiół", "Ogień", "Lawa", "Wulkan" },
                    { 30, "Wielbłąd", "Oaza", "Piach", "Pustynia" },
                    { 31, "Owoce", "Trawa", "Koc", "Piknik" },
                    { 32, "Żagiel", "Kapitan", "Kotwica", "Statek" },
                    { 33, "Łóżko", "Karetka", "Lekarz", "Szpital" },
                    { 34, "Nora", "Ogon", "Lis", "Las" },
                    { 35, "Wieszak", "Lustro", "Buty", "Szafa" },
                    { 36, "Drzwi", "Dom", "Zamek", "Klucz" },
                    { 37, "Autor", "Tytuł", "Kartka", "Książka" },
                    { 38, "Talerz", "Warzywa", "Łyżka", "Zupa" },
                    { 39, "Kałuża", "Chmura", "Parasol", "Deszcz" },
                    { 40, "Drewno", "Ciepło", "Dym", "Ogień" },
                    { 41, "Miód", "Gawra", "Niedźwiedź", "Las" },
                    { 42, "Afryka", "Ucho", "Trąba", "Słoń" },
                    { 43, "Jajko", "Skrzydło", "Gniazdo", "Ptak" },
                    { 44, "Obraz", "Sztaluga", "Pędzel", "Farba" },
                    { 45, "Łańcuch", "Pedał", "Kask", "Rower" },
                    { 46, "Grzebień", "Lustro", "Nożyczki", "Fryzjer" },
                    { 47, "Okulary", "Chlor", "Ręcznik", "Basen" },
                    { 48, "Noc", "Dziupla", "Sowa", "Las" },
                    { 49, "Kanapka", "Pizza", "Dziury", "Ser" },
                    { 50, "Kwiat", "Ul", "Pszczoła", "Miód" },
                    { 51, "Latawiec", "Liście", "Wiatrak", "Wiatr" },
                    { 52, "Znaczek", "Paczka", "List", "Poczta" },
                    { 53, "Rzeźba", "Wystawa", "Sztuka", "Muzeum" },
                    { 54, "Korona", "Rycerz", "Król", "Zamek" },
                    { 55, "Mucha", "Noga", "Sieć", "Pająk" },
                    { 56, "Olej", "Brama", "Narzędzia", "Garaż" },
                    { 57, "Trzcina", "Komar", "Łódka", "Jezioro" },
                    { 58, "Sól", "Ziemniak", "Ketchup", "Frytki" },
                    { 59, "Banan", "Liana", "Małpa", "Dżungla" },
                    { 60, "Woda", "Gąbka", "Mydło", "Prysznic" },
                    { 61, "Lizak", "Tort", "Czekolada", "Słodycze" },
                    { 62, "Kocioł", "Ogień", "Diabeł", "Piekło" },
                    { 63, "Klatka", "Bilet", "Lew", "ZOO" },
                    { 64, "Chmura", "Skrzydła", "Anioł", "Niebo" },
                    { 65, "Siano", "Farma", "Pole", "Traktor" },
                    { 66, "Flash", "Uśmiech", "Zdjęcie", "Aparat" },
                    { 67, "Mapa", "Wyspa", "Złoto", "Skarb" },
                    { 68, "Namiot", "Akrobata", "Klaun", "Cyrk" },
                    { 69, "Zimno", "Gałka", "Wafel", "Lody" },
                    { 70, "Kura", "Stodoła", "Krowa", "Wieś" },
                    { 71, "Kora", "Stukanie", "Dzięcioł", "Las" },
                    { 72, "Widok", "Poręcz", "Kwiaty", "Balkon" },
                    { 73, "Klawiatura", "Monitor", "Myszka", "Komputer" },
                    { 74, "Czosnek", "Ząb", "Krew", "Wampir" },
                    { 75, "Armata", "Rum", "Papuga", "Piraci" },
                    { 76, "Żołnierz", "Hełm", "Czołg", "Wojna" },
                    { 77, "Igły", "Jabłko", "Jeż", "Las" },
                    { 78, "Gniazdo", "Śniadanie", "Kura", "Jajko" },
                    { 79, "Głośnik", "Piosenka", "Antena", "Radio" },
                    { 80, "Poduszka", "Piżama", "Kołdra", "Łóżko" },
                    { 81, "Miód", "Żądło", "Ul", "Pszczoła" },
                    { 82, "Komin", "Popiół", "Ogień", "Kominek" },
                    { 83, "Kilof", "Winda", "Węgiel", "Kopalnia" },
                    { 84, "Blacha", "Zapach", "Piec", "Piekarnik" },
                    { 85, "Szyba", "Woda", "Ryba", "Akwarium" },
                    { 86, "Szklanka", "Biały", "Krowa", "Mleko" },
                    { 87, "Kuchnia", "Słony", "Pieprz", "Sól" },
                    { 88, "Orzech", "Dziupla", "Wiewiórka", "Las" },
                    { 89, "Meta", "Pot", "Buty", "Bieg" },
                    { 90, "Okulary", "Lato", "Dzień", "Słońce" },
                    { 91, "Nóż", "Skórka", "Masło", "Chleb" },
                    { 92, "Regał", "Czytanie", "Biblioteka", "Książka" },
                    { 93, "Skrzydło", "Silnik", "Niebo", "Samolot" },
                    { 94, "Spacer", "Ptaki", "Ścieżka", "Las" },
                    { 95, "Buty", "Mgła", "Szlak", "Góra" },
                    { 96, "Parasol", "Upał", "Piach", "Morze" },
                    { 97, "Płot", "Dach", "Ogród", "Dom" },
                    { 98, "Ryba", "Brzeg", "Woda", "Rzeka" },
                    { 99, "Bukiet", "Wazon", "Róża", "Kwiaciarnia" },
                    { 100, "Ulica", "Sklep", "Blok", "Miasto" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 64);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 65);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 66);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 67);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 68);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 69);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 70);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 71);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 72);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 73);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 74);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 75);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 76);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 77);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 78);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 79);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 80);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 81);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 82);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 83);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 84);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 85);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 86);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 87);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 88);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 89);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 90);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 91);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 92);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 93);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 94);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 95);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 96);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 97);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 98);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 99);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 100);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Cards",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "Cards",
                type: "datetime2",
                nullable: true);
        }
    }
}
