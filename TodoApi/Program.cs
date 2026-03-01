using Microsoft.EntityFrameworkCore;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);
// ** הוספת Swagger **
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// --- 1. רישום שירותים (Services) - חייב לקרות לפני 

// הגדרת פוליסי של CORS - מאפשר ל-React (שיושב בפורט אחר) לגשת ל-API הזה
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()   // מאפשר מכל כתובת
                        .AllowAnyMethod()   // מאפשר את כל הפעולות (GET, POST, וכו')
                        .AllowAnyHeader()); // מאפשר את כל סוגי ה-Headers
});

// שליפת מחרוזת החיבור מקובץ appsettings.json
var connectionString = builder.Configuration.GetConnectionString("ToDoDB");

// הזרקת ה-DbContext למערכת עם הגדרות MySQL
builder.Services.AddDbContext<ToDoDbContexttt>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));


// -----------------------------------------------------------------------

var app = builder.Build(); // בניית האפליקציה - מכאן והלאה לא ניתן להוסיף Services

// --- 2. הגדרת צנרת הבקשות (Middleware) ---
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty; // זה יפתח את Swagger ישירות ב-localhost:5070
});
// הפעלת ה-CORS - חייב להופיע לפני הגדרת הנתיבים (Routes)
app.UseCors("AllowAll");

// -----------------------------------------------------------------------

// --- 3. הגדרת נתיבי ה-API (Endpoints) ---
app.MapGet("/", () => "ברוכים הבאים ל-API של ToDo!"); // נתיב בסיסי לבדיקה
// שליפת כל המשימות - הכתובת: GET /items
app.MapGet("/items", async (ToDoDbContexttt db) =>
    await db.Items.ToListAsync());

// הוספת משימה חדשה - הכתובת: POST /items
app.MapPost("/items", async (ToDoDbContexttt db, Item task) => {
    db.Items.Add(task);
    await db.SaveChangesAsync();
    return Results.Created($"/items/{task.Id}", task);
});

// עדכון משימה קיימת - הכתובת: PUT /items/{id}
app.MapPut("/items/{id}", async (ToDoDbContexttt db, int id, Item inputItem) =>
{
    // חיפוש המשימה לפי ה-ID שנשלח בנתיב
    var item = await db.Items.FindAsync(id);
    
    // אם לא נמצאה משימה, החזר 404
    if (item is null) return Results.NotFound();

    // עדכון הערכים
  
    item.IsComplete = inputItem.IsComplete;

    await db.SaveChangesAsync(); // שמירת השינויים
    return Results.NoContent();  // החזרת קוד 204 (הצליח, אין תוכן להחזיר)
});

// מחיקת משימה - הכתובת: DELETE /items/{id}
app.MapDelete("/items/{id}", async (ToDoDbContexttt db, int id) =>
{
    var item = await db.Items.FindAsync(id);
    
    if (item is null) return Results.NotFound();

    db.Items.Remove(item);       // סימון למחיקה
    await db.SaveChangesAsync(); // ביצוע המחיקה בפועל במסד הנתונים
    
    return Results.NoContent();
});

// הרצת האפליקציה
app.Run();