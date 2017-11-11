using System;

namespace PicBook.Domain
{
    public interface IEntity
    {
        Guid Id { get; }
    }
}