using Application.DataTransferObjects;
using Application.Helpers;
using Application.Contracts;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Contracts;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Application.Services
{
    public class SingleUserService : ISingleUserService
    {
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public SingleUserService(IRepositoryManager repository, IMapper mapper, IConfiguration configuration)
        {
            _repository = repository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<SuccessResponse<UserReadListDTO>> CreateReadList(Guid userId, CreateReadListDTO model)
        {
            var user = await _repository.User.GetByIdAsync(userId);

            if (user == null)
                throw new RestException(HttpStatusCode.NotFound, "User not found");

            if (user.ReadListId != Guid.Empty)
            {
                throw new RestException(HttpStatusCode.BadRequest, "User already has readlist");
            }
           
            var userReadList = _mapper.Map<ReadList>(model);
            userReadList.UserId = userId;
            await _repository.ReadList.AddAsync(userReadList);

            user.ReadListId = userReadList.Id;

            _repository.User.Update(user);
            await _repository.SaveChangesAsync();
            
            return new SuccessResponse<UserReadListDTO>
            {
                Data = _mapper.Map<UserReadListDTO>(userReadList),
                Message = "Read List Created"
            };
        }

        public async Task<SuccessResponse<UserReadListDTO>> AddBookToLReadist(Guid userId, Guid bookId)
        {
            var book = await _repository.Book.GetByIdAsync(bookId);
            if (book is null)
                throw new RestException(HttpStatusCode.NotFound, "Book cannot be found");

            var user = await _repository.User.GetByIdAsync(userId);
            if (user is null)
                throw new RestException(HttpStatusCode.NotFound, "User not found");

            var userReadList = await _repository.ReadList.FirstOrDefaultAsync(x => x.Id == user.ReadListId);

            userReadList.Books.Add(book);
            _repository.ReadList.Update(userReadList);
            await _repository.SaveChangesAsync();

            return new SuccessResponse<UserReadListDTO>
            {
                Data = _mapper.Map<UserReadListDTO>(userReadList),
                Message = "Book Added to Read List"
            };

        }

        public async Task RemoveBookFromReadList(Guid userId, Guid bookId)
        {
            var book = await _repository.Book.GetByIdAsync(bookId);
            if (book is null)
                throw new RestException(HttpStatusCode.NotFound, "Book cannot be found");

            var user = await _repository.User.GetByIdAsync(userId);
            if (user is null)
                throw new RestException(HttpStatusCode.NotFound, "User not found");

            var userReadList = await _repository.ReadList.FirstOrDefaultAsync(x => x.Id == user.ReadListId);

            userReadList.Books.Remove(book);
            _repository.ReadList.Update(userReadList);
            await _repository.SaveChangesAsync();

        }

        public async Task<SuccessResponse<UserReadListDTO>> RenameReadList(Guid readListId, Guid userId, CreateReadListDTO model)
        {
            var readlist = await _repository.ReadList.GetByIdAsync(readListId);
            if (readlist is null)
                throw new RestException(HttpStatusCode.NotFound, "Read List does not exist");

            var user = await _repository.User.GetByIdAsync(userId);
            if (user is null)
                throw new RestException(HttpStatusCode.NotFound, "User not found");

            if (user.ReadListId != readListId)
                throw new RestException(HttpStatusCode.BadRequest, "ReadList does not match user signature");

            readlist.Name = model.Name;
            _repository.ReadList.Update(readlist);
            await _repository.SaveChangesAsync();

            return new SuccessResponse<UserReadListDTO>
            {
                Data = _mapper.Map<UserReadListDTO>(readlist),
                Message = "Read List Renamed"
            };
        }

        public async Task<SuccessResponse<UserReadListDTO>> GetUserReadList(Guid userId, Guid readListId)
        {
            var readlist = await _repository.ReadList.GetByIdAsync(readListId);
            if (readlist is null)
                throw new RestException(HttpStatusCode.NotFound, "Read List does not exist");

            var user = await _repository.User.GetByIdAsync(userId);
            if (user is null)
                throw new RestException(HttpStatusCode.NotFound, "User not found");

            if (user.ReadListId != readListId)
                throw new RestException(HttpStatusCode.BadRequest, "ReadList does not match user signature");

            return new SuccessResponse<UserReadListDTO>
            {
                Data = _mapper.Map<UserReadListDTO>(readlist)
            };
        }

        public async Task<SearchBooksDTO> SearchBookCategoriesAuthors(ResourceParameter parameter)
        {
            var booksQuery = _repository.Book.QueryAll()
                .Select(x => new ViewBookDTO
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    Author = x.Author,
                });

            var categoriesQuery = _repository.Category.QueryAll()
                .Select(x => new CreateCategoryDTO
                {
                    Name = x.Name,
                });

            if (!string.IsNullOrWhiteSpace(parameter.Search))
            {
                string search = parameter.Search.Trim();
                booksQuery = booksQuery.Where(x =>
                    x.Title.Contains(search) ||
                    x.Author.Equals(search) ||
                    x.Description.Contains(search));

                categoriesQuery = categoriesQuery.Where(x =>
                x.Name.Contains(search));
            }

            booksQuery.OrderByDescending(x => x.Title);
            categoriesQuery.OrderByDescending(x => x.Name);

            var books = await PagedList<ViewBookDTO>.Create(booksQuery, parameter.PageNumber, parameter.PageSize);
            var categories = await PagedList<CreateCategoryDTO>.Create(categoriesQuery, parameter.PageNumber, parameter.PageSize);

            var response = new SearchBooksDTO
            {
                Books = books,
                Categories = categories
            };

            return response;
        }

    }
}
