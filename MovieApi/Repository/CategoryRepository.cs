using MovieApi.Data;
using MovieApi.Models;
using MovieApi.Repository.IRepository;

namespace MovieApi.Repository;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CategoryRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public ICollection<Category> GetCategories()
    {
        return _dbContext.Categories.OrderBy(c => c.Name).ToList();
    }

    public Category GetCategory(int idCategory)
    {
        return _dbContext.Categories.FirstOrDefault(c => c.Id == idCategory);
    }

    public bool CategoryExists(int idCategory)
    {
        return _dbContext.Categories.Any(c => c.Id == idCategory);
    }

    public bool CategoryExists(string nameCategory)
    {
        var categoryExists = _dbContext.Categories.Any(c => c.Name.ToLower().Trim() == nameCategory.ToLower().Trim());
        return categoryExists;
    }

    public bool CategoryCreate(Category category)
    {
        category.CreatedDate = DateTime.Now;
        _dbContext.Categories.Add(category);

        return CategorySaveChanges();
    }

    public bool CategoryUpdate(Category category)
    {
        category.CreatedDate = DateTime.Now;
        _dbContext.Categories.Update(category);

        return CategorySaveChanges();
    }

    public bool CategoryDelete(Category category)
    {
        _dbContext.Categories.Remove(category);
        return CategorySaveChanges();
    }

    public bool CategorySaveChanges()
    {
        return _dbContext.SaveChanges() >= 0;
    }
}