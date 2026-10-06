using Dapper;
using Luilton_P1_P4.Models;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace Luilton_P1_P4.Services;

public class AutorService(IConfiguration config)
{
    private readonly string _cs = config.GetConnectionString("DefaultConnection")!;

    private SqliteConnection Conn() => new(_cs);

    private static AutorGet ToGet(dynamic r) =>
        new((int)r.IdAutor, (string)r.FechaNacimiento, (string)r.Nombres, (int)r.Sueldo, (string)r.Nacionalidad);

    public async Task InicializeAsync()
    {
        using var c = Conn();
        await c.ExecuteAsync(@"CREATE TABLE IF NOT EXISTS Autor (
                IdAutor INTEGER PRIMARY KEY AUTOINCREMENT,
                FechaNacimiento TEXT NOT NULL,
                Nombres TEXT NOT NULL,
                Nacionalidad TEXT NOT NULL,
                Sueldo INTEGER NOT NULL)");
    }

public async Task<AutorGet> SaveAsync(AutorSet r)
{
    using var c = Conn();
    
    var IdAutor = await c.ExecuteAsync(
        @"INSERT INTO Autor (FechaNacimiento, Nombres, Sueldo, Nacionalidad)
        VALUES (@FechaNacimiento, @Nombres, @Sueldo, @Nacionalidad);
        SELECT last_insert_rowid();",
        new { r.FechaNacimiento, r.Nombres, r.Sueldo, r.Nacionalidad });
        
    return new AutorGet(IdAutor, r.FechaNacimiento, r.Nombres, r.Sueldo, r.Nacionalidad);
}

    public async Task<bool> UpdateAsync(int id, AutorSet r)
    {
        using var c = Conn();
        var filas = await c.ExecuteAsync(
            @"UPDATE Autor
            SET FechaNacimiento=@FechaNacimiento, Nombres=@Nombres, Nacionalidad=@Nacionalidad, Sueldo=@Sueldo
            WHERE IdAutor=@IdAutor",
            new { IdAutor = id, r.FechaNacimiento, r.Nombres, r.Sueldo, r.Nacionalidad });
        return filas > 0;
    }

    public async Task<AutorGet?> GetByIdAsync(int id)
    {
        using var c = Conn();
        var fila = await c.QuerySingleOrDefaultAsync(
            "SELECT * FROM Autor WHERE IdAutor=@id", new { id });
        return fila is null ? null : ToGet(fila);
    }
    
    public async Task<List<AutorGet>> GetListAsync()
    {
        using var c = Conn();
        var filas = await c.QueryAsync("SELECT * FROM Autor ORDER BY IdAutor DESC");
        return filas.Select(f => (AutorGet)ToGet(f)).ToList();
    }
    public async Task<bool> DeleteAsync(int id)
    {
        using var c = Conn();

        var filasAfectadas = await c.ExecuteAsync(
            "DELETE FROM Autor WHERE IdAutor = @Id",
            new { Id = id }
        );
        return filasAfectadas > 0;
    }
}
