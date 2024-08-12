using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;

public static class DbSetMockExtensions
{
    public static DbSet<T> BuildMockDbSet<T>(this IEnumerable<T> source) where T : class
    {
        var queryable = source.AsQueryable();
        var mockSet = new Mock<DbSet<T>>();

        mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
        mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
        mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

        mockSet.Setup(m => m.Add(It.IsAny<T>())).Callback<T>(s => source.ToList().Add(s));
        mockSet.Setup(m => m.Remove(It.IsAny<T>())).Callback<T>(s => source.ToList().Remove(s));

        return mockSet.Object;
    }
}
