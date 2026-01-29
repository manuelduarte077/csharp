using MovieApi.Models;

namespace MovieApi.Repository.IRepository;

public interface ICategoryRepository
{
    ICollection<Category> GetCategories();

    Category GetCategory(int idCategory);

    bool CategoryExists(int idCategory);

    bool CategoryExists(string nameCategory);

    bool CategoryCreate(Category category);

    bool CategoryUpdate(Category category);

    bool CategoryDelete(Category category);

    bool CategorySaveChanges();
}