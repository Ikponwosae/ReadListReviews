using Application.Contracts;
using Application.DataTransferObjects;
using Application.Helpers;
using Application.Helpers.External;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Application.Services
{
    public class AdminUserService : IAdminUserService
    {
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public AdminUserService(IRepositoryManager repository, IMapper mapper, IConfiguration configuration, HttpClient httpClient)
        {
            _repository = repository;
            _mapper = mapper;
            _configuration = configuration;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(_configuration["NYTimesUrl"]);
        }

        public async Task<SuccessResponse<CategoryDTO>> CreateCategory(Guid userId, CreateCategoryDTO model)
        {
            var user = await _repository.User.GetByIdAsync(userId);
            if (user is null)
                throw new RestException(HttpStatusCode.NotFound, "User not found");

            if (user.Role != EUserRole.Admin.ToString())
                throw new RestException(HttpStatusCode.Unauthorized, "User is not authorized to perform this action");

            var categoryExists = await _repository.Category.ExistsAsync(x => x.Name == model.Name);
            if (categoryExists)
                throw new RestException(HttpStatusCode.Conflict, "A category with that name already exists");

            var newCategory = _mapper.Map<Category>(model);
            newCategory.CreatedById = user.Id;

            _repository.Category.Create(newCategory);

            await _repository.SaveChangesAsync();

            return new SuccessResponse<CategoryDTO>
            {
                Data = _mapper.Map<CategoryDTO>(newCategory),
                Message = "Category Created"
            };
        }

        public async Task<SuccessResponse<IEnumerable<CategoryDTO>>> GetAllCategories()
        {
            var categoriesQuery = _repository.Category.QueryAll()
                .Select(x => new CategoryDTO
                {
                    CategoryId = x.Id,
                    Name = x.Name,
                    Books = x.Books.Select(x => new ViewBookDTO
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Description = x.Description,
                        Author = x.Author,
                    }),
                });

            return new SuccessResponse<IEnumerable<CategoryDTO>>
            {
                Data = categoriesQuery
            };
        }

        public async Task<PagedResponse<IEnumerable<BookDTO>>> GetAllBooksInACategory(Guid categoryId, string actionName, ResourceParameter parameter, IUrlHelper urlHelper)
        {
            var category = await _repository.Category.GetByIdAsync(categoryId);
            if (category is null)
                throw new RestException(HttpStatusCode.NotFound, "Category does not exist");

            var categoryBooks = _repository.Book.QueryAll(x => x.CategoryId == categoryId)
                .Select(x => new BookDTO
                {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                Author = x.Author,
                AuthorDetails = x.AuthorDetails,
                ShopLink = x.ShopLink,
                Publisher = x.Publisher,
                BookFormat = x.BookFormat,
                Isbn = x.Isbn,
                Rating = x.Rating,
                BookImage = _mapper.Map<BookImageDTO>(x.BookImage),
                BookImageUrl = x.BookImageUrl,
        });

            if (!string.IsNullOrWhiteSpace(parameter.Search))
            {
                string search = parameter.Search.Trim();
                categoryBooks = categoryBooks.Where(x =>
                    x.Title.Contains(search) ||
                    x.Author.Contains(search) ||
                    x.Description.Contains(search));
            }

            categoryBooks.OrderByDescending(x => x.Title);

            var books = await PagedList<BookDTO>.Create(categoryBooks, parameter.PageNumber, parameter.PageSize, parameter.Sort);
            var page = PageUtility<BookDTO>.CreateResourcePageUrl(parameter, actionName, books, urlHelper);

            return new PagedResponse<IEnumerable<BookDTO>>
            {
                Data = books,
                Message = $"{category.Name} Books",
                Meta = new Meta
                {
                    Pagination = page
                }
            };
        }

        public async Task<SuccessResponse<IEnumerable<FetchBook>>> FetchBooksExternal(Guid categoryId)
        {
            var category = await _repository.Category.GetByIdAsync(categoryId);
            if (category == null)
                throw new RestException(HttpStatusCode.NotFound, "Category does not exist");

            var apiKey = _configuration["api-key"];
            var jsonResponse = await _httpClient.GetAsJson($"?api-key={apiKey}", _configuration);

            if (!jsonResponse.IsSuccessStatusCode)
                throw new RestException(jsonResponse.StatusCode, "Unable to get your session summary");

            var response = await jsonResponse.ReadContentAs<FetchBooksDTO>();

            List<FetchBook> booksList = new List<FetchBook>();
            var listfetch = response.Results.BestLists;

            foreach(var list in listfetch)
            {
                for(int i = 0; i < list.Books.Count; i++)
                {
                    var newBook = _mapper.Map<Book>(list.Books[i]);
                    newBook.CategoryId = categoryId;
                    _repository.Book.Create(newBook);

                    booksList.Add(list.Books[i]);
                }
            }

            await _repository.SaveChangesAsync();

            return new SuccessResponse<IEnumerable<FetchBook>>
            {
                Data = booksList
            };
        }

        public async Task<SuccessResponse<BookDTO>> ChangeBookCategory(Guid bookId, Guid categoryId)
        {
            var category = await _repository.Category.GetByIdAsync(categoryId);
            if (category == null)
                throw new RestException(HttpStatusCode.NotFound, "Category does not exist");

            var book = await _repository.Book.Get(x => x.Id == bookId).FirstOrDefaultAsync();
            if (book == null)
                throw new RestException(HttpStatusCode.NotFound, "Book cannot be found");

            if (book.CategoryId == categoryId)
                throw new RestException(HttpStatusCode.BadRequest, "Same category");
            
            book.CategoryId = categoryId;
            _repository.Book.Update(book);
            await _repository.SaveChangesAsync();

            var response = _mapper.Map<BookDTO>(book);
            response.Category = _mapper.Map<CreateCategoryDTO>(await _repository.Category.GetByIdAsync(book.CategoryId));

            return new SuccessResponse<BookDTO>
            {
                Data = response
            };
        }

        public async Task<SuccessResponse<BookDTO>> GetBookById(Guid bookId)
        {
            var book = await _repository.Book.GetByIdAsync(bookId);
            if (book == null)
                throw new RestException(HttpStatusCode.NotFound, "Book cannot be found");

            var response = _mapper.Map<BookDTO>(book);
            response.BookImage = _mapper.Map<BookImageDTO>(await _repository.Photo.Get(x => x.BookId == bookId).FirstOrDefaultAsync());
            response.Category = _mapper.Map<CreateCategoryDTO>(await _repository.Category.GetByIdAsync(book.CategoryId));

            return new SuccessResponse<BookDTO>
            {
                Data = response
            };
        }

        public async Task<SuccessResponse<ViewBookDTO>> AddBook(Guid categoryId, AddBookDTO model)
        {
            var category = await _repository.Category.GetByIdAsync(categoryId);
            if (category == null)
                throw new RestException(HttpStatusCode.NotFound, "Category does not exist");

            var newBook = _mapper.Map<Book>(model);
            newBook.Category = category;
            newBook.CategoryId = categoryId;

            _repository.Book.Create(newBook);

            await _repository.SaveChangesAsync();

            return new SuccessResponse<ViewBookDTO>
            {
                Data = _mapper.Map<ViewBookDTO>(newBook)
            };
        }

        public async Task<SuccessResponse<BookDTO>> UpdateBook(Guid bookId, UpdateBookDTO model)
        {
            var book = await _repository.Book.Get(x => x.Id == bookId).FirstOrDefaultAsync();
            if (book == null)
                throw new RestException(HttpStatusCode.NotFound, "Book does not exist");
            
            var updatedBook = _mapper.Map<Book>(model);
            updatedBook.UpdatedAt = DateTime.Now;
            updatedBook.CategoryId = book.CategoryId;

            var bookImage = model.BookImage;
            if(bookImage.Length > 0)
            {
                using(var memoryStream = new MemoryStream())
                {
                    await bookImage.CopyToAsync(memoryStream);
                    //Upload file if less than 2MB
                    if(memoryStream.Length < 2097152)
                    {
                        var newPhoto = new Photo()
                        {
                            Bytes = memoryStream.ToArray(),
                            Description = bookImage.FileName,
                            FileExtension = Path.GetExtension(bookImage.FileName),
                            Size = bookImage.Length,
                        };

                        updatedBook.BookImage = newPhoto;
                    }
                    else
                    {
                        throw new RestException(HttpStatusCode.InternalServerError, "The image uploaded is too large.");
                    }
                }
            }

            _repository.Book.Update(updatedBook);
            await _repository.SaveChangesAsync();

            var response = _mapper.Map<BookDTO>(updatedBook);
            response.Category = _mapper.Map<CreateCategoryDTO>(await _repository.Category.GetByIdAsync(book.CategoryId));

            return new SuccessResponse<BookDTO>
            {
                Data = response
            };
        }

        public async Task DeleteBook(Guid bookId)
        {
            var book = await _repository.Book.GetByIdAsync(bookId);
            if (book == null)
                throw new RestException(HttpStatusCode.NotFound, "Book does not exist");

            _repository.Book.Delete(book);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteCategory(Guid categoryId)
        {
            var category = await _repository.Category.Get(x => x.Id == categoryId).FirstOrDefaultAsync();
            if (category == null)
                throw new RestException(HttpStatusCode.NotFound, "Category does not exist");

            _repository.Category.Delete(category);
            await _repository.SaveChangesAsync();
        }
    }
}
