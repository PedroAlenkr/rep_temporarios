using Api.Entities.Jsons.JsonInput;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Api.Services;
using Api.Models;
using Api.Services.Validations;
using System.Reflection;

namespace Api.Controllers
{
    [Route("[controller]")]
    public class ApiController : Controller
    {
        private readonly ApiDAO ApiDAO;

        public ApiController(ApiDAO apiDAO)
        {
            ApiDAO = apiDAO;           
        }

        [HttpPost]
        public ActionResult Post()
        {
            try
            {
                object result;

                using (var reader = new StreamReader(Request.Body))
                {
                    string stringInput = reader.ReadToEnd();
                    result = Call(stringInput);
                }
                return Ok(new {Message= "método executado com sucesso", result});          
            }
            catch (Exception ex)
            {
                if(ex.InnerException!=null)
                    return BadRequest(new {Message= ex.InnerException.Message});    
                else 
                    return BadRequest(new {Message= ex.Message});
            }
        }

        public object Call(string stringInput)
        {
            JsonMethod? methodName = JsonSerializer.Deserialize<JsonMethod>(stringInput);

            Type controllerType = typeof(ApiController);

            MethodInfo method = controllerType.GetMethod(methodName.Method, BindingFlags.NonPublic | BindingFlags.Instance);

            return method == null ? null : method.Invoke(this, new object[] { stringInput });
        }

        private object CreateUser(string stringInput)
        {
            JsonUser? jsonCreateUser = JsonSerializer.Deserialize<JsonUser>(stringInput);

            if(!ModelState.IsValid||jsonCreateUser == null) return BadRequest();
        
            UsersModel user = Mapper.JsonUserToModel(jsonCreateUser);

            Validator.ExecuteValidator(ApiDAO, user);
            
            ApiDAO.Create(user);

            return jsonCreateUser;
        }

        private object UpdateUser(string stringInput)
        {
            JsonUser? jsonUpdateUser = JsonSerializer.Deserialize<JsonUser>(stringInput);

            if(!ModelState.IsValid||jsonUpdateUser == null) return BadRequest();
            
            UsersModel user = Mapper.JsonUserToModel(jsonUpdateUser);
            
            Validator.ExecuteValidator(ApiDAO, user);

            ApiDAO.Update(user);

            return Ok();
        }

        private object DeleteUser(string stringInput)
        {
            JsonEmailAndPassword? jsonEmailAndPassword = JsonSerializer.Deserialize<JsonEmailAndPassword>(stringInput);

            if(!ModelState.IsValid || jsonEmailAndPassword == null) return BadRequest();

            UsersModel user = ApiDAO.GetUserByEmailAndPassword(jsonEmailAndPassword.Email, jsonEmailAndPassword.Password);

            ApiDAO.DeleteUserById(user.Id);

            return Ok();
        }

        private object GetUserByEmailAndPassword(string stringInput)
        {
            JsonEmailAndPassword? jsonEmailAndPassword = JsonSerializer.Deserialize<JsonEmailAndPassword>(stringInput);

            if(!ModelState.IsValid || jsonEmailAndPassword == null) return BadRequest();

            UsersModel user = ApiDAO.GetUserByEmailAndPassword(jsonEmailAndPassword.Email, jsonEmailAndPassword.Password);
            
            return user;
        }

        private object GetUsers(string stringInput)
        {
            List<UsersModel> users = ApiDAO.GetUsers();
            
            return users;
        }

        private object CreateProduct(string stringInput)
        {
            JsonProduct? jsonCreateProduct = JsonSerializer.Deserialize<JsonProduct>(stringInput);

            if(!ModelState.IsValid||jsonCreateProduct == null) return BadRequest();
        
            ProductsModel product = Mapper.JsonProductToModel(jsonCreateProduct);

            Validator.ExecuteValidator(ApiDAO, product);
            
            ApiDAO.Create(product);

            return jsonCreateProduct;
        }

        private object UpdateProduct(string stringInput)
        {
            JsonProduct? jsonUpdateProduct = JsonSerializer.Deserialize<JsonProduct>(stringInput);

            if(!ModelState.IsValid||jsonUpdateProduct == null) return BadRequest();
            
            ProductsModel product = Mapper.JsonProductToModel(jsonUpdateProduct);
            
            Validator.ExecuteValidator(ApiDAO, product);

            ApiDAO.Update(product);

            return Ok();
        }

        private object DeleteProduct(string stringInput)
        {
            JsonProduct? jsonProduct = JsonSerializer.Deserialize<JsonProduct>(stringInput);

            if(!ModelState.IsValid || jsonProduct == null) return BadRequest();

            ApiDAO.DeleteProductById(jsonProduct.Id);

            return Ok();
        }

        private object GetProductByName(string stringInput)
        {
            JsonProduct? jsonProduct = JsonSerializer.Deserialize<JsonProduct>(stringInput);

            if(!ModelState.IsValid || jsonProduct == null || jsonProduct.Name == null) return BadRequest();

            List<ProductsModel> product = ApiDAO.GetProductsByName(jsonProduct.Name);
            
            return product;
        }

        private object GetProducts(string stringInput)
        {
            List<ProductsModel> products = ApiDAO.GetProducts();
            
            return products;
        }
    }
}