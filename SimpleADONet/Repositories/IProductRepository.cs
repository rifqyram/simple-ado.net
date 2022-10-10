using SimpleADONet.Models;

namespace SimpleADONet.Repositories;

public interface IProductRepository
{
    void CreateTable();
    void Save(Product product);
    List<Product> FindAll();
    Product FindById(int id);
    void Update(Product newProduct);
    void DeleteById(int id);
}