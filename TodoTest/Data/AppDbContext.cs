using Microsoft.EntityFrameworkCore;
using TodoTest.Models;

namespace TodoTest.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TodoItem> Todos { get; set; }
}