using Microsoft.Data.SqlClient;
using Dapper;
using baltaDapper.Models;
using System;
using System.Collections.Generic;

const string connectionString = "Server=localhost,1433;Database=balta;User ID=sa;Password=1q2w3e4r@#$;Encrypt=True;TrustServerCertificate=true;";

       
        
        
using (var connection = new SqlConnection(connectionString))
{
    //CreateCategory(connection);
    //ListCategories(connection);
    //UpdateCategory(connection);
    //OneToOne(connection);
    OneToMany(connection); 
};

Console.WriteLine("_______________________________________________________");

static void ListCategories(SqlConnection connection )
{
    var categories = connection.Query<Category>("SELECT [Id], [Title] FROM [Category]");
    foreach (var category in categories)
    {
        Console.WriteLine($"{category.Id} - {category.Title}");
        Console.WriteLine("----------------------------------");
    }

}
static void CreateCategory(SqlConnection connection ) 
{
     var category =new Category();
        category.Id = Guid.NewGuid();
        category.Title = "Amazon AWS";
        category.Url = "amazon";
        category.Description = "Categoria destinada a AWS";
        category.Order = 8;
        category.Summary = "AWS";
        category.Featured = false;

        var insertSql=@"INSERT INTO 
            [Category] 
        VALUES(
            @Id,
            @Title,
            @Url,
            @Summary,
            @Order,
            @Description,
            @Featured)";

        var rows = connection.Execute(insertSql,new{
        category.Id,
        category.Title,
        category.Url,
        category.Summary,
        category.Order,
        category.Description,
        category.Featured
        });
        Console.WriteLine($"{rows} linhas afetadas");
        Console.WriteLine("----------------------------------");
}
//Abaixo exemplo ADO.net
/*using (var connection = new SqlConnection(connectionString))
{
    connection.Open();
    using (var command = connection.CreateCommand())
    {
        command.Connection = connection;
        command.CommandType = System.Data.CommandType.Text;
        command.CommandText = "SELECT [Id], [Title] FROM [Category]";
        var reader = command.ExecuteReader();
        while (reader.Read()){
            Console.WriteLine($"{reader.GetGuid(0)} - {reader.GetString(1)}") ;
        }
    }    
};*/
static void UpdateCategory(SqlConnection connection )
{
    var updateQuery = @"UPDATE [Category] SET [Title]=@Title WHERE [Id]= @Id ";
    var rows = connection.Execute(updateQuery,new{
        Id = new Guid("32705e38-de8a-4463-8fdf-222d4e0b4edf"),
        Title = "Front end 2024",
    });
    Console.WriteLine($"{rows} linhas atualizadas");
    Console.WriteLine("----------------------------------");
}

static void OneToOne(SqlConnection connection)
{
    var sql = @"SELECT
                 * 
                FROM [CareerItem] 
                INNER JOIN [Course] 
                ON[CareerItem].[CourseId]=[Course].[Id]";
    var items = connection.Query<CareerItem,Course,CareerItem>(sql,
    (careerItem, course) => {
        careerItem.Course = course;
        return careerItem;
    }, splitOn:"Id");

    foreach (var item in items)
    {
        Console.WriteLine(item.Course.Title) ;
        Console.WriteLine("-------------------------------------------") ;  
    }
    
}

static void OneToMany(SqlConnection connection)
{
    var sql = @"SELECT 
                    [Career].[Id],
                    [Career].[Title],
                    [CareerItem].[CareerId],
                    [Career].[Title]
                FROM 
                    [Career] 
                INNER JOIN
                    [CareerItem] ON[CareerItem].[CareerId]=[Career].[Id]
                ORDER BY
                    [Career].[Title]";
    var careers = connection.Query<Career,CareerItem,Career>(
        sql,
        (career, item) => {
               return career;
        }, splitOn:"CareerId");

    foreach (var career in careers)
    {
        Console.WriteLine($"{career.Title}");
        foreach (var item in career.Items)
        {
            Console.WriteLine($"{item.Title}");
        }
        Console.WriteLine("-------------------------------------------") ;  
    }
    
}

