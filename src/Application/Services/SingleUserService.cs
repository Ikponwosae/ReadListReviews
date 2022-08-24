using Application.DataTransferObjects;
using Application.Helpers;
using Application.Contracts;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Contracts;
using Microsoft.Extensions.Configuration;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

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

        public async Task<PagedResponse<IEnumerable<BookDTO>>> GetAllBooks(string actionName, ResourceParameter parameters, IUrlHelper urlHelper)
        {
            var booksQuery = _repository.Book.QueryAll()
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

            if (!string.IsNullOrWhiteSpace(parameters.Search))
            {
                string search = parameters.Search.Trim();
                booksQuery = booksQuery.Where(x =>
                    x.Title.Contains(search) ||
                    x.Author.Contains(search) ||
                    x.Description.Contains(search));
            }

            booksQuery.OrderByDescending(x => x.Title);

            var books = await PagedList<BookDTO>.Create(booksQuery, parameters.PageNumber, parameters.PageSize);
            var page = PageUtility<BookDTO>.CreateResourcePageUrl(parameters, actionName, books, urlHelper);

            return new PagedResponse<IEnumerable<BookDTO>>
            {
                Data = books,
                Message = "All Books",
                Meta = new Meta
                {
                    Pagination = page
                }
            };
        }


        public async Task<PagedResponse<IEnumerable<ReviewDTO>>> GetBookReviews(Guid bookId, string actionName, ResourceParameter parameter, IUrlHelper urlHelper)
        {
            var book = await _repository.Book.Get(x => x.Id == bookId).FirstOrDefaultAsync();
            if (book == null)
                throw new RestException(HttpStatusCode.NotFound, "Book cannot be found");

            var reviewsList = _repository.Review.QueryAll(x => x.BookId == bookId)
                .Select(x => new ReviewDTO
                {
                    Id = x.Id,
                    Description = x.Description,
                    Status = x.Status,
                    CreatedAt = x.CreatedAt,
                    UserId = x.UserId,
                    BookId = bookId,
                });

            reviewsList.OrderByDescending(x => x.CreatedAt);

            var reviews = await PagedList<ReviewDTO>.Create(reviewsList, parameter.PageNumber, parameter.PageSize, parameter.Sort);
            var page = PageUtility<ReviewDTO>.CreateResourcePageUrl(parameter, actionName, reviews, urlHelper);

            return new PagedResponse<IEnumerable<ReviewDTO>>
            {
                Data = reviews,
                Message = $"{book.Title} Reviews",
                Meta = new Meta
                {
                    Pagination = page
                }
            };
        }
        public async Task<SuccessResponse<ReviewDTO>> ReviewABook(Guid userId, Guid bookId, CreateReviewDTO model)
        {
            var user = await _repository.User.Get(x => x.Id == userId).FirstOrDefaultAsync();
            if (user == null)
                throw new RestException(HttpStatusCode.NotFound, "User does not exist");

            var book = await _repository.Book.Get(x => x.Id == bookId).FirstOrDefaultAsync();
            if (book == null)
                throw new RestException(HttpStatusCode.NotFound, "Book cannot be found");

            var review = _mapper.Map<Review>(model);
            review.Status = EReviewStatus.Posted.ToString();
            review.CreatedAt = DateTime.UtcNow;
            review.UserId = userId;
            review.Owner = user;

            _repository.Review.Create(review);

            user.Reviews.Add(review);
            book.Reviews.Add(review);

            _repository.User.Update(user);
            _repository.Book.Update(book);
            await _repository.SaveChangesAsync();

            var response = _mapper.Map<ReviewDTO>(review);

            return new SuccessResponse<ReviewDTO>
            {
                Data = response,
                Message = "Review created"
            };
        }
        public async Task<SuccessResponse<ReviewDTO>> GetReviewById(Guid reviewId)
        {
            var review = await _repository.Review.Get(x => x.Id == reviewId).FirstOrDefaultAsync();
            if (review == null)
                throw new RestException(HttpStatusCode.NotFound, "Review does not exist");

            var response = _mapper.Map<ReviewDTO>(review);

            return new SuccessResponse<ReviewDTO>
            {
                Data = response,
                Message = "Review retrieved"
            };
        }

        public async Task DeleteReview(Guid userId, Guid reviewId)
        {
            var review = await _repository.Review.Get(x => x.Id == reviewId).FirstOrDefaultAsync();
            if (review == null)
                throw new RestException(HttpStatusCode.NotFound, "Review does not exist");

            var user = await _repository.User.GetByIdAsync(userId);
            if (user is null)
                throw new RestException(HttpStatusCode.NotFound, "User not found");

            if (review.UserId != userId)
                throw new RestException(HttpStatusCode.Unauthorized, "You do not own this review");

            _repository.Review.Delete(review);
            await _repository.SaveChangesAsync();
        }
        
        public async Task<SuccessResponse<AllReadListDTO>> GetUserReadLists(Guid userId)
        {
            var user = _repository.User.Get(x => x.Id == userId);
            if (user == null)
                throw new RestException(HttpStatusCode.NotFound, "User not found");

            var readList = _repository.ReadList.Get(x => x.UserId == userId).FirstOrDefault();

            return new SuccessResponse<AllReadListDTO>
            {
                Data = _mapper.Map<AllReadListDTO>(readList),
                Message = "User read lists retrieved"
            };
        }
    }
}
