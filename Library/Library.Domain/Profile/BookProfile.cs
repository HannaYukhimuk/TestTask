using AutoMapper;
using Library.Domain.Entities;
using Library.Domain.Models;

public class BookProfile : Profile
{
    public BookProfile()
    {
        CreateMap<BookCreateDto, Book>()
            .ForMember(dest => dest.Author, opt => opt.Ignore())  
            .ForMember(dest => dest.Id, opt => opt.Ignore()); 

        CreateMap<Book, BookCreateDto>(); 
    }
}
