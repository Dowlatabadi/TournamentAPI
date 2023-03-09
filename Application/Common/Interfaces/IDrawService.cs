
namespace Tournament.Application.Common.Interfaces;

public interface IDrawService
{
	IEnumerable<(T item,int order)> Draw<T>(List<(T item,double weight)> ItemWeights,int ToDraw);
}
