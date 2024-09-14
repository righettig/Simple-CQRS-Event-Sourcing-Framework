namespace Framework.Core;

public interface IQueryHandler<TQuery, TQueryResult> where TQuery : class, IQuery
{
    TQueryResult Handle(TQuery query);
}